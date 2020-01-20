using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.Schema;
using AdventOfCode.Graphs;

namespace AdventOfCode
{
    internal class Day19
    {
        static void Main(string[] args)
        {
            var fileText = File.ReadAllText(@"Datasets\day19.txt");
            var strCode = fileText.Split(",").ToArray();
            var size = 50;
            var screen = Enumerable.Range(0, size).Select(r => Enumerable.Range(0, size).Select(c => '█').ToArray())
                .ToArray();
            var result = 0;
            var input = 0;
            var count = 0;
            //var program = new Program(strCode);
            foreach (var line in screen)
            {
                Console.WriteLine(string.Join("", line));
            }

            //Console.Clear();
            //while (result != 1)
            //{
            //for (var x = 0; x < size; x++)
            //{
            //    for (var y = 0; y < size; y++)
            //    {
            //        var program = new Program(strCode);
            //        program.SendInput(y);
            //        result = program.RunProgram(input);
            //        while (result != 2)
            //        {
            //            result = program.RunProgram(input);
            //        }
            //        program.SendInput(x);
            //        while (result != 1)
            //        {
            //            result = program.RunProgram(input);
            //        }
            //        //result = program.RunProgram(input);
            //        //result = program.RunProgram(input);
            //        //if (result == 1) continue;
            //        var tile = int.Parse(program.Outputs.Last());
            //        //row.Add((char) tile);
            //        if (tile == 1) count++;
            //        Console.SetCursorPosition(x, y);
            //        Console.Write(tile);
            //        //if (tile == 10)
            //        //{
            //        //    screen.Add(row);
            //        //    row = new List<char>();
            //        //}

            //        program.Outputs = new List<string>();
            //    }
            //}
            //Console.WriteLine(count);

            int x0 = 0;
            int y1 = 150;
            while (true)
            {
                //Find first x coordinate of beam.

                while (!TestCoordinate(strCode, x0, y1))
                {
                    x0++;
                }

                if (TestCoordinate(strCode, x0 + 99, y1 - 99))
                {
                    break;
                }

                y1++;
            }

            Console.WriteLine($"The answer is: {10000 * x0 + (y1 - 99)}");

        }


        private class Program
        {
            private List<int> inputQueue = new List<int>();
            private bool waitingForInput = false;
            private int _relativeBase;
            private int _pointerPosition;
            private readonly string[] _memory;

            public List<string> Outputs = new List<string>();

            public Program(string[] strCode)
            {
                var memory = Enumerable.Range(0, 500000).Select(x => "0").ToArray();
                _memory = strCode.Concat(memory).ToArray();
            }

            public void SendInput(int input)
            {
                inputQueue.Add(input);
                waitingForInput = false;
            }

            public int RunProgram(int input)
            {
                //var memory = Enumerable.Range(0, 50000).Select(x => "0").ToArray();
                //memory = strCode.Concat(memory).ToArray();
                for (var pointerPosition = _pointerPosition; pointerPosition < _memory.Length; pointerPosition++)
                {
                    var codeDouble = double.Parse(_memory[pointerPosition]);
                    var codeStr = ReverseDouble(codeDouble).ToString();
                    var modeA = codeStr.Length > 4 ? codeStr.ElementAt(4).ToString() : "0";
                    var modeB = codeStr.Length > 3 ? codeStr.ElementAt(3).ToString() : "0";
                    var modeC = codeStr.Length > 2 ? codeStr.ElementAt(2).ToString() : "0";
                    var code = codeStr.Length > 1
                        ? int.Parse(codeStr[1].ToString() + codeStr[0].ToString())
                        : int.Parse(codeStr);

                    var argument1Pointer = pointerPosition + 1;
                    var relativeArgument1Pointer =
                        modeC == "2" ? int.Parse(_memory[argument1Pointer]) + _relativeBase : 0;
                    var pointer1Position = modeC == "0" ? int.Parse(_memory[argument1Pointer]) : 0;

                    var argument2Pointer = pointerPosition + 2;
                    var relativeArgument2Pointer =
                        modeB == "2" ? int.Parse(_memory[argument2Pointer]) + _relativeBase : 0;
                    var pointer2Position = modeB == "0" ? int.Parse(_memory[argument2Pointer]) : 0;

                    var resultPointer = pointerPosition + 3;
                    var relativeResultPointer = modeA == "2" ? int.Parse(_memory[resultPointer]) + _relativeBase : 0;
                    var resultPointerPosition = modeA == "0" && code != 4 ? int.Parse(_memory[resultPointer]) : 0;

                    if (code == 99)
                    {
                        _pointerPosition = pointerPosition;
                        break;
                    }

                    var argument1 = modeC switch
                    {
                        "1" => _memory[argument1Pointer],
                        "2" => _memory[relativeArgument1Pointer],
                        _ => _memory[pointer1Position]
                    };
                    var argument2 = modeB switch
                    {
                        "1" => _memory[argument2Pointer],
                        "2" => _memory[relativeArgument2Pointer],
                        _ => _memory[pointer2Position]
                    };

                    var resultPosition = modeA switch
                    {
                        "1" => resultPointer,
                        "2" => relativeResultPointer,
                        _ => resultPointerPosition
                    };

                    if (code == 1)
                    {
                        _memory[resultPosition] = (double.Parse(argument1) + double.Parse(argument2)).ToString();
                        pointerPosition += 3;
                    }
                    else if (code == 2)
                    {
                        _memory[resultPosition] = (double.Parse(argument1) * double.Parse(argument2)).ToString();
                        pointerPosition += 3;
                    }
                    else if (code == 3)
                    {
                        if (waitingForInput)
                        {
                            _pointerPosition = pointerPosition;
                            waitingForInput = false;
                            return 2;
                        }

                        if (inputQueue.Count > 0)
                        {
                            input = inputQueue[0];
                            inputQueue.RemoveAt(0);
                            var inputPointer = modeC != "2" ? pointer1Position : relativeArgument1Pointer;
                            _memory[inputPointer] = input.ToString();
                            //Console.WriteLine($"Input: {input}, Input Taken: {memory[inputPointer]}");
                            pointerPosition++;
                        }
                        else
                        {
                            _pointerPosition = pointerPosition;
                            return 2;
                        }

                        //var inputPointer = modeC != "2" ? pointer1Position : relativeArgument1Pointer;
                        //_memory[inputPointer] = input.ToString();
                        ////Console.WriteLine($"Input: {input}, Input Taken: {memory[inputPointer]}");
                        //pointerPosition++;
                    }
                    else if (code == 4)
                    {
                        //Console.WriteLine($"Output: " + argument1);
                        Outputs.Add(argument1);
                        pointerPosition++;
                        _pointerPosition = pointerPosition + 1;
                        return 0;
                    }
                    else if (code == 5)
                    {
                        if (argument1 != "0")
                        {
                            pointerPosition = int.Parse(argument2);
                            pointerPosition--;
                        }
                        else
                        {
                            pointerPosition += 2;
                        }
                    }
                    else if (code == 6)
                    {
                        if (argument1 == "0")
                        {
                            pointerPosition = int.Parse(argument2);
                            pointerPosition--;
                        }
                        else
                        {
                            pointerPosition += 2;
                        }
                    }
                    else if (code == 7)
                    {
                        if (double.Parse(argument1) < double.Parse(argument2))
                        {
                            _memory[resultPosition] = "1";
                        }
                        else
                        {
                            _memory[resultPosition] = "0";
                        }

                        pointerPosition += 3;
                    }
                    else if (code == 8)
                    {
                        if (argument1 == argument2)
                        {
                            _memory[resultPosition] = "1";
                        }
                        else
                        {
                            _memory[resultPosition] = "0";
                        }

                        pointerPosition += 3;
                    }
                    else if (code == 9)
                    {
                        _relativeBase += int.Parse(argument1);
                        pointerPosition++;
                    }
                    else if (code == 99)
                    {
                        break;
                    }
                    else
                    {
                        throw new Exception();
                    }
                }

                return 1;
            }
        }

        private static double ReverseDouble(double src)
        {
            double dst = 0;
            int decPoint = 0;

            while (src - (long)src > 0)
            {
                src = src * 10;
                decPoint++;
            }

            int totalDigits = 0;

            while (src > 0)
            {
                int d = (int)src % 10;
                dst = dst * 10 + d;
                src = (long)(src / 10);
                totalDigits++;
            }

            if (decPoint > 0)
            {
                int reversedDecPoint = totalDigits - decPoint;
                for (int i = 0; i < reversedDecPoint; i++) dst = dst / 10;
            }

            return dst;
        }

        public static bool TestCoordinate(string[] strCode, int x, int y)
        {
            var program = new Program(strCode);
            program.SendInput(x);
            var result = program.RunProgram(0);
            while (result != 2)
            {
                result = program.RunProgram(0);
            }
            program.SendInput(y);
            while (result != 1)
            {
                result = program.RunProgram(0);
            }
            var tile = int.Parse(program.Outputs.Last());


            return tile == 1;
        }

        //    private const string inputFile = @"Datasets\day19.txt";

        //    static IEnumerable<long> regs;

        //    static void Main(string[] args)
        //    {
        //        Console.WriteLine("Day 19 - Tractor Beam");
        //        Console.WriteLine("Star 1");
        //        Console.WriteLine();

        //        regs = File.ReadAllText(inputFile).Split(',').Select(long.Parse).ToArray();

        //        long totalAffected = 0;


        //        for (int x = 0; x < 50; x++)
        //        {
        //            for (int y = 0; y < 50; y++)
        //            {
        //                IntCode machine = new IntCode(
        //                    "Star 1",
        //                    regs,
        //                    fixedInputs: new long[] { x, y },
        //                    output: n => totalAffected += n);

        //                machine.SyncRun();
        //            }
        //        }

        //        Console.WriteLine($"The answer is: {totalAffected}");

        //        Console.WriteLine();
        //    }

        //    public class IntCode
        //    {
        //        public long lastOutput = 0;
        //        public string Name { get; }

        //        private int instr;

        //        private bool done = false;

        //        private readonly List<long> regs;

        //        private long fixedInputIndex = 0;
        //        private readonly long[] fixedInputs;

        //        private readonly Func<long> input;
        //        private readonly Action<long> output;
        //        private readonly Channel<long> inputChannel = Channel.CreateUnbounded<long>();

        //        private long relativeBase = 0;


        //        public enum State
        //        {
        //            Continue = 0,
        //            Output,
        //            Terminate
        //        }

        //        public enum Instr
        //        {
        //            Add = 1,
        //            Multiply = 2,
        //            Input = 3,
        //            Output = 4,
        //            JIT = 5,
        //            JIF = 6,
        //            LT = 7,
        //            EQ = 8,
        //            ADJ = 9,
        //            Terminate = 99
        //        }

        //        public enum Mode
        //        {
        //            position = 0,
        //            value,
        //            relative
        //        }

        //        public long this[int i]
        //        {
        //            get => regs[i];
        //            set => regs[i] = value;
        //        }

        //        public IntCode(
        //            string name,
        //            IEnumerable<long> regs,
        //            IEnumerable<long> fixedInputs = null,
        //            Func<long> input = null,
        //            Action<long> output = null)
        //        {
        //            instr = 0;
        //            Name = name;

        //            this.regs = new List<long>(regs);

        //            this.fixedInputs = fixedInputs?.ToArray() ?? Array.Empty<long>();
        //            this.input = input;
        //            this.output = output;
        //        }

        //        private long GetValue(long reg, Mode mode)
        //        {
        //            switch (mode)
        //            {
        //                case Mode.position:
        //                    return GetIndex(GetIndex(reg));

        //                case Mode.value:
        //                    return GetIndex(reg);

        //                case Mode.relative:
        //                    return GetIndex(GetIndex(reg) + relativeBase);

        //                default:
        //                    throw new Exception();
        //            }
        //        }

        //        private long GetIndex(long index)
        //        {
        //            if (index < regs.Count)
        //            {
        //                return regs[(int)index];
        //            }

        //            return 0L;
        //        }


        //        private void SetValue(long reg, Mode setMode, long value)
        //        {
        //            switch (setMode)
        //            {
        //                case Mode.position:
        //                    SetValue((int)reg, value);
        //                    break;

        //                case Mode.relative:
        //                    SetValue((int)(relativeBase + reg), value);
        //                    break;

        //                case Mode.value:
        //                default:
        //                    throw new Exception();
        //            }
        //        }

        //        private void SetValue(int index, long value)
        //        {
        //            if (index < regs.Count)
        //            {
        //                regs[(int)index] = value;
        //            }
        //            else
        //            {
        //                while (regs.Count < index)
        //                {
        //                    regs.Add(0L);
        //                }

        //                regs.Add(value);
        //            }

        //        }

        //        public void WriteValue(long value)
        //        {
        //            inputChannel.Writer.WriteAsync(value);
        //        }

        //        public Task Run()
        //        {
        //            Task task = new Task(RunToEnd);
        //            task.Start();
        //            return task;
        //        }

        //        public void SyncRun()
        //        {
        //            while (SyncExecute() != State.Terminate) { }
        //        }

        //        private void RunToEnd()
        //        {
        //            State state = State.Continue;
        //            while (state != State.Terminate)
        //            {
        //                Task<State> executeTask = Execute();
        //                executeTask.Wait();
        //                state = executeTask.Result;
        //            }
        //        }

        //        public async Task<State> Execute()
        //        {
        //            if (done)
        //            {
        //                return State.Terminate;
        //            }

        //            Instr instruction = (Instr)(regs[instr] % 100);
        //            Mode oneMode = (Mode)((regs[instr] / 100) % 10);
        //            Mode twoMode = (Mode)((regs[instr] / 1000) % 10);
        //            Mode threeMode = (Mode)((regs[instr] / 10000) % 10);

        //            switch (instruction)
        //            {
        //                case Instr.Add:
        //                    SetValue(regs[instr + 3], threeMode, GetValue(instr + 1, oneMode) + GetValue(instr + 2, twoMode));
        //                    instr += 4;
        //                    return State.Continue;

        //                case Instr.Multiply:
        //                    SetValue(regs[instr + 3], threeMode, GetValue(instr + 1, oneMode) * GetValue(instr + 2, twoMode));
        //                    instr += 4;
        //                    return State.Continue;

        //                case Instr.Input:
        //                    long inputValue;
        //                    if (fixedInputIndex < fixedInputs.Length)
        //                    {
        //                        inputValue = fixedInputs[fixedInputIndex++];
        //                    }
        //                    else if (input != null)
        //                    {
        //                        inputValue = input.Invoke();
        //                    }
        //                    else
        //                    {
        //                        inputValue = await inputChannel.Reader.ReadAsync();
        //                    }

        //                    SetValue(regs[instr + 1], oneMode, inputValue);
        //                    instr += 2;
        //                    return State.Continue;

        //                case Instr.Output:
        //                    lastOutput = GetValue(instr + 1, oneMode);
        //                    instr += 2;
        //                    output?.Invoke(lastOutput);
        //                    return State.Output;

        //                case Instr.JIT:
        //                    if (GetValue(instr + 1, oneMode) != 0)
        //                    {
        //                        instr = (int)GetValue(instr + 2, twoMode);
        //                    }
        //                    else
        //                    {
        //                        instr += 3;
        //                    }
        //                    return State.Continue;

        //                case Instr.JIF:
        //                    if (GetValue(instr + 1, oneMode) == 0)
        //                    {
        //                        instr = (int)GetValue(instr + 2, twoMode);
        //                    }
        //                    else
        //                    {
        //                        instr += 3;
        //                    }
        //                    return State.Continue;

        //                case Instr.LT:
        //                    SetValue(regs[instr + 3], threeMode, GetValue(instr + 1, oneMode) < GetValue(instr + 2, twoMode) ? 1 : 0);
        //                    instr += 4;
        //                    return State.Continue;

        //                case Instr.EQ:
        //                    SetValue(regs[instr + 3], threeMode, GetValue(instr + 1, oneMode) == GetValue(instr + 2, twoMode) ? 1 : 0);
        //                    instr += 4;
        //                    return State.Continue;

        //                case Instr.ADJ:
        //                    relativeBase += GetValue(instr + 1, oneMode);
        //                    instr += 2;
        //                    return State.Continue;

        //                case Instr.Terminate:
        //                    done = true;
        //                    return State.Terminate;

        //                default: throw new Exception($"Unsupported instruction: {instruction}");
        //            }
        //        }

        //        public State SyncExecute()
        //        {
        //            if (done)
        //            {
        //                return State.Terminate;
        //            }

        //            Instr instruction = (Instr)(regs[instr] % 100);
        //            Mode oneMode = (Mode)((regs[instr] / 100) % 10);
        //            Mode twoMode = (Mode)((regs[instr] / 1000) % 10);
        //            Mode threeMode = (Mode)((regs[instr] / 10000) % 10);

        //            switch (instruction)
        //            {
        //                case Instr.Add:
        //                    SetValue(regs[instr + 3], threeMode, GetValue(instr + 1, oneMode) + GetValue(instr + 2, twoMode));
        //                    instr += 4;
        //                    return State.Continue;

        //                case Instr.Multiply:
        //                    SetValue(regs[instr + 3], threeMode, GetValue(instr + 1, oneMode) * GetValue(instr + 2, twoMode));
        //                    instr += 4;
        //                    return State.Continue;

        //                case Instr.Input:
        //                    long inputValue;
        //                    if (fixedInputIndex < fixedInputs.Length)
        //                    {
        //                        inputValue = fixedInputs[fixedInputIndex++];
        //                    }
        //                    else if (input != null)
        //                    {
        //                        inputValue = input.Invoke();
        //                    }
        //                    else
        //                    {
        //                        throw new NotSupportedException();
        //                    }

        //                    SetValue(regs[instr + 1], oneMode, inputValue);
        //                    instr += 2;
        //                    return State.Continue;

        //                case Instr.Output:
        //                    lastOutput = GetValue(instr + 1, oneMode);
        //                    instr += 2;
        //                    output?.Invoke(lastOutput);
        //                    return State.Output;

        //                case Instr.JIT:
        //                    if (GetValue(instr + 1, oneMode) != 0)
        //                    {
        //                        instr = (int)GetValue(instr + 2, twoMode);
        //                    }
        //                    else
        //                    {
        //                        instr += 3;
        //                    }
        //                    return State.Continue;

        //                case Instr.JIF:
        //                    if (GetValue(instr + 1, oneMode) == 0)
        //                    {
        //                        instr = (int)GetValue(instr + 2, twoMode);
        //                    }
        //                    else
        //                    {
        //                        instr += 3;
        //                    }
        //                    return State.Continue;

        //                case Instr.LT:
        //                    SetValue(regs[instr + 3], threeMode, GetValue(instr + 1, oneMode) < GetValue(instr + 2, twoMode) ? 1 : 0);
        //                    instr += 4;
        //                    return State.Continue;

        //                case Instr.EQ:
        //                    SetValue(regs[instr + 3], threeMode, GetValue(instr + 1, oneMode) == GetValue(instr + 2, twoMode) ? 1 : 0);
        //                    instr += 4;
        //                    return State.Continue;

        //                case Instr.ADJ:
        //                    relativeBase += GetValue(instr + 1, oneMode);
        //                    instr += 2;
        //                    return State.Continue;

        //                case Instr.Terminate:
        //                    done = true;
        //                    return State.Terminate;

        //                default: throw new Exception($"Unsupported instruction: {instruction}");
        //            }
        //        }


        //}

    }

}

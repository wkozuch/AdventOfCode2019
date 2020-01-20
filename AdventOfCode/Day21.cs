using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading;

namespace AdventOfCode
{
    internal class Day21
    {
        static void Main(string[] args)
        {
            var fileText = File.ReadAllText(@"Datasets\day21.txt");
            var strCode = fileText.Split(",").ToArray();
            var screen = new List<List<char>>();
            //var screen = Enumerable.Range(0, size).Select(r => Enumerable.Range(0, size).Select(c => '█').ToArray())
            //    .ToArray();
            var result = 0;
            var input = 0;
            var program = new Program(strCode);
            //var inst = "AND D J\n".ToCharArray();
            var A = "OR A T\n";
            var B = "AND B T\n";
            var C = "AND C T\n";
            var D = "AND D T\n";
            var E = "NOT T J\n";
            var F = "AND D J\n";
            var G = "OR E T\n";
            var H = "OR H T\n";
            var I = "AND T J \n";
            var J = "AND H T\n";
            var walk = "WALK\n".ToCharArray();
            var run = "RUN\n".ToCharArray();
            //var inst = (A + B + C + D + E + F).ToCharArray();
            var inst = (A + B + C + D + E + F + G + H + I).ToCharArray();
            //input = Convert.ToChar(inst[j]);
            //program.SendInput(input);

            while (result != 1)
            {

                result = program.RunProgram(input);
                var output = program.Outputs.Last();
                ////foreach (var output in program.Outputs)
                ////{
                //Console.Write(Convert.ToChar(System.Convert.ToInt32(output)));
                ////}

                if (result == 2)
                {
                    for (int i = 0; i < inst.Length; i++)
                    {
                        var character = inst[i];
                        if (character == '\n') input = 10;
                        input = Convert.ToChar(character);
                        program.SendInput(input);
                    }
                    //for (int i = 0; i < walk.Length; i++)
                    //{
                    //    var character = walk[i];
                    //    if (character == '\n') input = 10;
                    //    input = Convert.ToChar(character);
                    //    program.SendInput(input);
                    //}
                    for (int i = 0; i < run.Length; i++)
                    {
                        var character = run[i];
                        if (character == '\n') input = 10;
                        input = Convert.ToChar(character);
                        program.SendInput(input);
                    }

                    program.Outputs = new List<string>();
                }

            }


            foreach (var output in program.Outputs)
            {
                Console.Write(Convert.ToChar(System.Convert.ToInt32(output)));
            }

        }

        private static IEnumerable<T> Flatten<T>(T[][] map)
        {
            for (int row = 0; row < map.GetLength(0); row++)
            {
                for (int col = 0; col < map[0].Length; col++)
                {
                    yield return map[row][col];
                }
            }
        }

        private static IEnumerable<T> Flatten<T>(List<List<T>> map)
        {
            for (int row = 0; row < map.Count; row++)
            {
                for (int col = 0; col < map[0].Count; col++)
                {
                    yield return map[row][col];
                }
            }
        }

        private static int Mod(int x, int m)
        {
            int r = x % m;
            return r < 0 ? r + m : r;
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


    }

}
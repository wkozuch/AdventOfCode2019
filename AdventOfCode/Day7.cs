using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;

namespace AdventOfCode
{
    internal class Day7
    {
        static void Main(string[] args)
        {
            var fileText = File.ReadAllText(@"Datasets\day7.txt");
            var intCode = fileText.Split(",").Select(int.Parse).ToArray();

            var phases = new List<int>() { 9, 8, 7, 6, 5 };
            var permuations = GetPermutations(phases, 5).ToList();
            var results = new List<int>();
            //var opt = 0;
            //foreach (var phase in phases)
            //{
            //    var program = new Program();
            //    var end = program.RunProgram(intCode, phase, opt);
            //    opt = program.Outputs.Last();

            //}

            //foreach (var phaseSettings in permuations)
            //{
            //phases = phaseSettings.ToList();
            // var result = RunCode(phases.ToArray(), intCode);

            var result = 0;
            var output = 0;
            var program1 = new Program();
            var program2 = new Program();
            var program3 = new Program();
            var program4 = new Program();
            var program5 = new Program();
            var intCode1 = fileText.Split(",").Select(int.Parse).ToArray();
            var intCode2 = fileText.Split(",").Select(int.Parse).ToArray();
            var intCode3 = fileText.Split(",").Select(int.Parse).ToArray();
            var intCode4 = fileText.Split(",").Select(int.Parse).ToArray();
            var intCode5 = fileText.Split(",").Select(int.Parse).ToArray();

            while (true)
            {
                result += program1.RunProgram(intCode1, phases[0], output);
                output = program1.Outputs.Last();

                result += program2.RunProgram(intCode2, phases[1], output);
                output = program2.Outputs.Last();

                result += program3.RunProgram(intCode3, phases[2], output);
                output = program3.Outputs.Last();

                result += program4.RunProgram(intCode4, phases[3], output);
                output = program4.Outputs.Last();

                result += program5.RunProgram(intCode5, phases[4], output);
                output = program5.Outputs.Last();


                if (result != 0)
                {
                    results.Add(output);
                    break;
                }

            }
            //}

            Console.WriteLine($"Max:{results.Max()}");
        }

        private static int RunCode(int[] phaseSettings, int[] intCode)
        {
            var inputIndex = 0;
            var inputConsumed = false;
            var inputSettings = new int[] { phaseSettings[0], 0, phaseSettings[1], 0, phaseSettings[2], 0, phaseSettings[3], 0, phaseSettings[4], 0 };
            var arg1 = 0;
            var input = 0;
            var nextInput = 0;
            var interactions = 0;

            for (var phaseindex = 0; phaseindex < phaseSettings.Length; phaseindex++)
            {
                var phase = phaseSettings[phaseindex];

                for (var i = 0; i < intCode.Length; i++)
                {
                    var codeStr = ReverseInt(intCode[i]).ToString();
                    var modeB = codeStr.Length > 3 ? int.Parse(codeStr[3].ToString()) : 0;
                    var modeC = codeStr.Length > 2 ? int.Parse(codeStr[2].ToString()) : 0;
                    var code = codeStr.Length > 1
                        ? int.Parse(codeStr[1].ToString() + codeStr[0].ToString())
                        : int.Parse(codeStr);

                    if (code == 99) break;

                    arg1 = modeC == 1 ? intCode[i + 1] : intCode[intCode[i + 1]];
                    var arg2 = 0;
                    var resultIx = intCode[i + 3];

                    if (code == 1)
                    {
                        arg2 = modeB == 1 ? intCode[i + 2] : intCode[intCode[i + 2]];
                        intCode[resultIx] = arg1 + arg2;
                        i += 3;
                    }
                    else if (code == 2)
                    {
                        arg2 = modeB == 1 ? intCode[i + 2] : intCode[intCode[i + 2]];
                        intCode[resultIx] = arg1 * arg2;
                        i += 3;
                    }
                    else if (code == 3)
                    {
                        var index = intCode[i + 1];
                        intCode[index] = inputSettings[inputIndex];
                        //intCode[index] = inputConsumed ? input : phase;
                        Console.WriteLine(
                            $"Phase: {phase}, Input index: {inputIndex}, Input Taken: {intCode[index]}");
                        if (!inputConsumed)
                        {
                            inputConsumed = true;
                            inputIndex++;
                        }

                        i++;
                    }
                    else if (code == 4)
                    {
                        Console.WriteLine($"Output: {arg1}");
                        if (inputIndex < inputSettings.Length - 1) inputSettings[inputIndex + 3] = arg1;
                        phaseindex++;
                        phase = phaseSettings[phaseindex];
                        inputIndex++;

                        if (phaseindex + 1 == phaseSettings.Length)
                        {
                            phaseindex = -1;
                        }

                        i++;
                    }
                    else if (code == 5)
                    {
                        if (arg1 != 0)
                        {
                            arg2 = modeB == 1 ? intCode[i + 2] : intCode[intCode[i + 2]];
                            i = arg2;
                            i--;
                        }
                        else
                        {
                            i += 2;
                        }
                    }
                    else if (code == 6)
                    {
                        if (arg1 == 0)
                        {
                            arg2 = modeB == 1 ? intCode[i + 2] : intCode[intCode[i + 2]];
                            i = arg2;
                            i--;
                        }
                        else
                        {
                            i += 2;
                        }
                    }
                    else if (code == 7)
                    {
                        if (arg1 < arg2)
                        {
                            intCode[resultIx] = 1;
                        }
                        else
                        {
                            intCode[resultIx] = 0;
                        }

                        i += 3;
                    }
                    else if (code == 8)
                    {
                        if (arg1 == arg2)
                        {
                            intCode[resultIx] = 1;
                        }
                        else
                        {
                            intCode[resultIx] = 0;
                        }

                        i += 3;
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

                inputIndex++;
                inputConsumed = false;
            }

            return arg1;
        }

        public static int ReverseInt(int num)
        {
            var reversed = 0;
            while (num > 0)
            {
                reversed = reversed * 10 + num % 10;
                num /= 10;
            }
            return reversed;
        }

        private class Program
        {
            private int _pointerPosition;
            private bool _inputConsumed;
            public readonly List<int> Outputs = new List<int>();
            public int RunProgram(int[] intCode, int phase, int input)
            {
                for (var i = _pointerPosition; i < intCode.Length; i++)
                {
                    var codeStr = ReverseInt(intCode[i]).ToString();
                    var modeB = codeStr.Length > 3 ? int.Parse(codeStr[3].ToString()) : 0;
                    var modeC = codeStr.Length > 2 ? int.Parse(codeStr[2].ToString()) : 0;
                    var code = codeStr.Length > 1
                        ? int.Parse(codeStr[1].ToString() + codeStr[0].ToString())
                        : int.Parse(codeStr);

                    if (code == 99)
                    {
                        _pointerPosition = i;
                        break;
                    }

                    var arg1 = modeC == 1 ? intCode[i + 1] : intCode[intCode[i + 1]];
                    var arg2 = 0;
                    var resultIx = intCode[i + 3];

                    if (code == 1)
                    {
                        arg2 = modeB == 1 ? intCode[i + 2] : intCode[intCode[i + 2]];
                        intCode[resultIx] = arg1 + arg2;
                        i += 3;
                    }
                    else if (code == 2)
                    {
                        arg2 = modeB == 1 ? intCode[i + 2] : intCode[intCode[i + 2]];
                        intCode[resultIx] = arg1 * arg2;
                        i += 3;
                    }
                    else if (code == 3)
                    {
                        var index = intCode[i + 1];
                        intCode[index] = _inputConsumed ? input : phase;
                        Console.WriteLine($"Phase: {phase}, Input: {input}, Input Taken: {intCode[index]}");
                        if (!_inputConsumed)
                        {
                            _inputConsumed = true;
                        }
                        i++;
                    }
                    else if (code == 4)
                    {
                        arg1 = modeC == 1 ? intCode[i + 1] : intCode[intCode[i + 1]];
                        Console.WriteLine($"Output: {arg1}");
                        Outputs.Add(arg1);
                        _pointerPosition = i + 2;
                        return 0;
                    }
                    else if (code == 5)
                    {
                        if (arg1 != 0)
                        {
                            arg2 = modeB == 1 ? intCode[i + 2] : intCode[intCode[i + 2]];
                            i = arg2;
                            i--;
                        }
                        else
                        {
                            i += 2;
                        }
                    }
                    else if (code == 6)
                    {
                        if (arg1 == 0)
                        {
                            arg2 = modeB == 1 ? intCode[i + 2] : intCode[intCode[i + 2]];
                            i = arg2;
                            i--;
                        }
                        else
                        {
                            i += 2;
                        }
                    }
                    else if (code == 7)
                    {
                        if (arg1 < arg2)
                        {
                            intCode[resultIx] = 1;
                        }
                        else
                        {
                            intCode[resultIx] = 0;
                        }

                        i += 3;
                    }
                    else if (code == 8)
                    {
                        if (arg1 == arg2)
                        {
                            intCode[resultIx] = 1;
                        }
                        else
                        {
                            intCode[resultIx] = 0;
                        }

                        i += 3;
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

        static IEnumerable<IEnumerable<T>> GetPermutations<T>(IEnumerable<T> list, int length)
        {
            if (length == 1) return list.Select(t => new T[] { t });

            return GetPermutations(list, length - 1)
                .SelectMany(t => list.Where(e => !t.Contains(e)),
                    (t1, t2) => t1.Concat(new T[] { t2 }));
        }
    }
}

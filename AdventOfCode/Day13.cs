using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;

namespace AdventOfCode
{
    internal class Day13
    {
        static void Main(string[] args)
        {
            var fileText = File.ReadAllText(@"Datasets\day13.txt");
            var strCode = fileText.Split(",").ToArray();
            var size = 40;
            var screen = Enumerable.Range(0, size).Select(r => Enumerable.Range(0, size).Select(c => '█').ToArray())
                .ToArray();
            var x = 0;
            var y = 0;
            var result = 0;
            var score = 0;
            var input = 0;
            var paddlepos = 0;
            var ballpos = 0;
            var program = new Program(strCode);
            foreach (var line in screen)
            {
                Console.WriteLine(string.Join("", line));
            }

            Console.Clear();

            while (result != 1)
            {
                result = program.RunProgram(input);
                if (result == 1)
                {
                    foreach (var line in screen)
                    {
                        Console.WriteLine(string.Join("", line));
                    }

                    break;
                }

                x = int.Parse(program.Outputs.First());
                y = int.Parse(program.Outputs.Skip(1).First());
                var tile = int.Parse(program.Outputs.Last());
                var drawing = '█';
                if (tile == 1) drawing = '▓';
                else if (tile == 2) drawing = '░';
                else if (tile == 3)
                {
                    drawing = '_';
                    paddlepos = x;
                }
                else if (tile == 4)
                {
                    drawing = 'o';
                    ballpos = x;
                }

                screen[y][x] = drawing;

                program.Outputs = new List<string>();
            }

            var count = Flatten(screen).Count(c => c == '░');
            Console.WriteLine(count);

            strCode[0] = "2";
            program = new Program(strCode);

            while (true)
            {
                result = program.RunProgram(input);

                if (result == 1)
                {
                    Console.Clear();
                    break;
                }

                if (result == 2)
                {
                    Console.Clear();
                    if (paddlepos < ballpos)
                        input = 1;
                    else if (paddlepos > ballpos)
                        input = -1;
                    else
                        input = 0;

                    program.SendInput(input);

                    foreach (var line in screen)
                    {
                        Console.WriteLine(string.Join("", line));
                    }
                }
                else
                {
                    x = int.Parse(program.Outputs[0]);
                    y = int.Parse(program.Outputs[1]);

                    if (x == -1 & y == 0)
                    {
                        score = int.Parse(program.Outputs[2]);
                    }
                    else
                    {
                        var tile = int.Parse(program.Outputs[2]);
                        var drawing = '█';
                        switch (tile)
                        {
                            case 1:
                                drawing = '▓';
                                break;
                            case 2:
                                drawing = '░';
                                break;
                            case 3:
                                drawing = '_';
                                paddlepos = x;
                                break;
                            case 4:
                                drawing = 'o';
                                ballpos = x;
                                break;
                        }
                        screen[y][x] = drawing;
                    }

                    program.Outputs = new List<string>();
                }

            }
            Console.WriteLine(score);
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
                        //if (waitingForInput)
                        //{
                        //    _pointerPosition = pointerPosition;
                        //    waitingForInput = false;
                        //    return 2;
                        //}

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
                        if (Outputs.Count < 3) continue;
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
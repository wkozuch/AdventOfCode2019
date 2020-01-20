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
    internal class Day15
    {
        static void Main(string[] args)
        {
            //var fileText = File.ReadAllText(@"Datasets\day15.txt");
            //var strCode = fileText.Split(",").ToArray();
            //var size = 43;
            //var screen = Enumerable.Range(0, size).Select(r => Enumerable.Range(0, size).Select(c => '█').ToArray())
            //    .ToArray();
            //var visited = Enumerable.Range(0, size).Select(r => Enumerable.Range(0, size).Select(c => false).ToArray())
            //    .ToArray();
            //var x = size / 2;
            //var y = size / 2;
            //screen[y][x] = 'S';
            //visited[y][x] = true;
            //var result = 0;
            //var input = 0;
            //var program = new Program(strCode);
            //foreach (var line in screen)
            //{
            //    Console.WriteLine(string.Join("", line));
            //}

            //Console.Clear();

            //var directions = new[] { ConsoleKey.UpArrow, ConsoleKey.LeftArrow, ConsoleKey.DownArrow, ConsoleKey.RightArrow };
            //var directionIndex = 0;
            //var inputKey = directions[directionIndex];
            //var drawing = '█';
            //while (drawing != 'C')
            //{
            //    Thread.Sleep(TimeSpan.FromSeconds(1.0 / 25));
            //    Console.Clear();
            //    //inputKey = Console.ReadKey().Key;
            //    //directionIndex = Mod(++directionIndex, directions.Length);
            //    //inputKey = directions[directionIndex];
            //    var new_x = x;
            //    var new_y = y;
            //    switch (inputKey)
            //    {
            //        case ConsoleKey.UpArrow:
            //            input = 1;
            //            new_y = y - 1;
            //            break;
            //        case ConsoleKey.DownArrow:
            //            input = 2;
            //            new_y = y + 1;
            //            break;
            //        case ConsoleKey.LeftArrow:
            //            input = 3;
            //            new_x = x - 1;
            //            break;
            //        case ConsoleKey.RightArrow:
            //            input = 4;
            //            new_x = x + 1;
            //            break;
            //    }
            //    if (input != 1 && input != 2 && input != 3 && input != 4) continue;

            //    program.RunProgram(input);

            //    //if (result == 1)
            //    //{
            //    //    var str = string.Join("", screen.Select(line => string.Join("", line) + Environment.NewLine));
            //    //    Console.Write(str);

            //    //    break;
            //    //}

            //    //if (result == 2)
            //    //{
            //    //    program.SendInput(input);

            //    //    foreach (var line in screen)
            //    //    {
            //    //        Console.WriteLine(string.Join("", line));
            //    //    }
            //    //}
            //    //else
            //    //{
            //    var tile = int.Parse(program.Outputs.Last());

            //    switch (tile)
            //    {
            //        case 0:
            //            drawing = '#';
            //            screen[new_y][new_x] = drawing;
            //            directionIndex = Mod(++directionIndex, directions.Length);
            //            inputKey = directions[directionIndex];
            //            break;
            //        case 1:
            //            drawing = '.';
            //            //screen[y][x] = visited[y][x] ? '\0' : drawing;
            //            if (screen[y][x] != 'O') screen[y][x] = drawing;
            //            visited[y][x] = true;
            //            screen[new_y][new_x] = 'D';
            //            y = new_y;
            //            x = new_x;
            //            directionIndex = Mod(--directionIndex, directions.Length);
            //            inputKey = directions[directionIndex];
            //            break;
            //        case 2:
            //            drawing = 'O';
            //            screen[y][x] = '.';
            //            visited[y][x] = true;
            //            screen[new_y][new_x] = drawing;
            //            y = new_y;
            //            x = new_x;
            //            directionIndex = Mod(++directionIndex, directions.Length);
            //            inputKey = directions[directionIndex];
            //            break;
            //    }
            //    //screen[size / 2][size / 2] = 'S';
            //    //foreach (var line in screen)
            //    //{


            //    //}
            //    var st = string.Join("", screen.Select(line => string.Join("", line) + Environment.NewLine));
            //    Console.Write(st);
            //    File.WriteAllText(@"Datasets\day15_2.txt", st);
            //    //}

            //    program.Outputs = new List<string>();
            //    //}
            //}

            //var count = Flatten(screen).Count(c => c == '.');
            //Console.WriteLine(count);

            var fileText = File.ReadAllLines(@"Datasets\day15_2.txt");
            var screen = fileText.Select(x => x.ToCharArray()).ToArray();
            var x0 = Enumerable.Range(0, screen.Length).Single(x => screen[x].Contains('O'));
            var y0 = Enumerable.Range(0, screen[x0].Length).Single(y => screen[x0][y] == 'O');
            var count = Flatten(screen).Count(c => c == '.');
            var time = 0;
            List<Tuple<int, int>> oxygens = new List<Tuple<int, int>>() { new Tuple<int, int>(x0, y0) };
            while (count != 0)
            {
                Console.Clear();
                var current = new List<Tuple<int, int>>(oxygens);
                var nextStage = new List<Tuple<int, int>>();
                for (var i = current.Count - 1; i >= 0; i--)
                {
                    var (x, y) = current[i];

                    if (screen[x + 1][y] == '.')
                    {
                        screen[x + 1][y] = 'O';
                        nextStage.Add(new Tuple<int, int>(x + 1, y));
                    }

                    if (screen[x - 1][y] == '.')
                    {
                        screen[x - 1][y] = 'O';
                        nextStage.Add(new Tuple<int, int>(x - 1, y));
                    }

                    if (screen[x][y + 1] == '.')
                    {
                        screen[x][y + 1] = 'O';
                        nextStage.Add(new Tuple<int, int>(x, y + 1));
                    }

                    if (screen[x][y - 1] == '.')
                    {
                        screen[x][y - 1] = 'O';
                        nextStage.Add(new Tuple<int, int>(x, y - 1));
                    }
                    oxygens.RemoveAt(i);
                }

                oxygens = new List<Tuple<int, int>>(nextStage);

                time++;

                var st = string.Join("", screen.Select(line => string.Join("", line) + Environment.NewLine));
                Console.Write(st);
                Thread.Sleep(TimeSpan.FromSeconds(1.0 / 25));
                count = Flatten(screen).Count(c => c == '.');
            }

            Console.WriteLine(time);


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

            //public void SendInput(int input)
            //{
            //    inputQueue.Add(input);
            //    waitingForInput = false;
            //}

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

                        //if (inputQueue.Count > 0)
                        //{
                        //    input = inputQueue[0];
                        //    inputQueue.RemoveAt(0);
                        var inputPointer = modeC != "2" ? pointer1Position : relativeArgument1Pointer;
                        _memory[inputPointer] = input.ToString();
                        //Console.WriteLine($"Input: {input}, Input Taken: {memory[inputPointer]}");
                        pointerPosition++;
                        //}
                        //else
                        //{
                        _pointerPosition = pointerPosition;
                        //    return 2;
                        //}

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
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AdventOfCode
{
    internal class Day11
    {
        static void Main(string[] args)
        {
            var fileText = File.ReadAllText(@"Datasets\day11.txt");
            var strCode = fileText.Split(",").ToArray();
            var size = 501;
            var surface = Enumerable.Range(0, size).Select(x => Enumerable.Range(0, size).Select(y => '.').ToArray()).ToArray();
            var coloredTiles = Enumerable.Range(0, size).Select(i => Enumerable.Range(0, size).Select(j => 0).ToArray()).ToArray();
            var x = size / 2;
            var y = size / 2;
            var result = 0;
            var program = new Program(strCode);
            var directions = new[] { '<', '^', '>', 'v' };
            var directionIndex = 1;
            var step = 0;
            var direction = directions[directionIndex];
            var color = '#';
            surface[x][y] = direction;
            foreach (var line in surface)
            {
                Console.WriteLine(string.Join("", line));
            }

            surface[x][y] = color;
            Console.Clear();

            while (result != 1)
            {
                color = surface[x][y];
                var input = color == '.' ? 0 : 1;
                result = program.RunProgram(input);
                if (result == 1) break;
                //Console.Clear();
                color = program.Outputs.First() == "0" ? '.' : '#';
                surface[x][y] = color;
                coloredTiles[x][y] = 1;
                directionIndex = program.Outputs.Last() == "0"
                    ? Mod(--directionIndex, directions.Length)
                    : Mod(++directionIndex, directions.Length);
                direction = directions[directionIndex];

               Console.WriteLine($"Step {step++} color: {color} input: {input} output1: {program.Outputs.First()}  output2: {program.Outputs.Last()} direction: {direction}");

                if (direction == '<') y--;
                else if (direction == '^') x--;
                else if (direction == 'v') x++;
                else if (direction == '>') y++;
                program.Outputs = new List<string>();
            }

            foreach (var line in surface)
            {
                Console.WriteLine(string.Join("", line));
            }

            var count = Flatten<int>(coloredTiles).Count(x => x == 1);
            Console.WriteLine(result);
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
            private int _relativeBase;
            private int _pointerPosition;
            private readonly string[] _memory;

            public List<string> Outputs = new List<string>();

            public Program(string[] strCode)
            {
                var memory = Enumerable.Range(0, 500000).Select(x => "0").ToArray();
                _memory = strCode.Concat(memory).ToArray();
            }

            public int RunProgram( int input)
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
                    var relativeArgument1Pointer = modeC == "2" ? int.Parse(_memory[argument1Pointer]) + _relativeBase : 0;
                    var pointer1Position = modeC == "0" ? int.Parse(_memory[argument1Pointer]) : 0;

                    var argument2Pointer = pointerPosition + 2;
                    var relativeArgument2Pointer = modeB == "2" ? int.Parse(_memory[argument2Pointer]) + _relativeBase : 0;
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
                        var inputPointer = modeC != "2" ? pointer1Position : relativeArgument1Pointer;
                        _memory[inputPointer] = input.ToString();
                        //Console.WriteLine($"Input: {input}, Input Taken: {memory[inputPointer]}");
                        pointerPosition++;
                    }
                    else if (code == 4)
                    {
                        //Console.WriteLine($"Output: " + argument1);
                        Outputs.Add(argument1);
                        pointerPosition++;
                        _pointerPosition = pointerPosition + 1;
                        if (Outputs.Count < 2) continue; ;
                        //_pointerPosition = pointerPosition + 1;
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

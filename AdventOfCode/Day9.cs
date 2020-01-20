using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AdventOfCode
{
    internal class Day9
    {
        static void Main(string[] args)
        {
            var fileText = File.ReadAllText(@"Datasets\day9.txt");
            var strCode = fileText.Split(",").ToArray();
            var program = new Program();
            var input = 2 ;
            program.RunProgram(strCode, input);
            var result = string.Join(",", program.Outputs);
            Console.WriteLine(result);
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
            
            public readonly List<string> Outputs = new List<string>();
          
            public int RunProgram(string[] strCode, int input)
            {
                var memory = Enumerable.Range(0, 50000).Select(x => "0").ToArray();
                memory = strCode.Concat(memory).ToArray();
                for (var pointerPosition = 0; pointerPosition < strCode.Length; pointerPosition++)
                {
                    var codeDouble = double.Parse(memory[pointerPosition]);
                    var codeStr = ReverseDouble(codeDouble).ToString();
                    var modeA = codeStr.Length > 4 ? codeStr.ElementAt(4).ToString() : "0";
                    var modeB = codeStr.Length > 3 ? codeStr.ElementAt(3).ToString() : "0";
                    var modeC = codeStr.Length > 2 ? codeStr.ElementAt(2).ToString() : "0";
                    var code = codeStr.Length > 1
                        ? int.Parse(codeStr[1].ToString() + codeStr[0].ToString())
                        : int.Parse(codeStr);

                    var argument1Pointer = pointerPosition + 1;
                    var relativeArgument1Pointer = modeC == "2" ? int.Parse(memory[argument1Pointer]) + _relativeBase : 0;
                    var pointer1Position = modeC == "0" ? int.Parse(memory[argument1Pointer]) : 0;

                    var argument2Pointer = pointerPosition + 2;
                    var relativeArgument2Pointer = modeB == "2" ? int.Parse(memory[argument2Pointer]) + _relativeBase : 0;
                    var pointer2Position = modeB == "0" ? int.Parse(memory[argument2Pointer]) : 0;

                    var resultPointer = pointerPosition + 3;
                    var relativeResultPointer = modeA == "2" ? int.Parse(memory[resultPointer]) + _relativeBase : 0;
                    var resultPointerPosition = modeA == "0" ?  int.Parse(memory[resultPointer]) : 0;

                    if (code == 99)
                    {
                        _pointerPosition = pointerPosition;
                        break;
                    }

                    var argument1 = modeC switch
                    {
                        "1" => memory[argument1Pointer],
                        "2" => memory[relativeArgument1Pointer],
                        _ => memory[pointer1Position]
                    };
                    var argument2 = modeB switch
                    {
                        "1" => memory[argument2Pointer],
                        "2" => memory[relativeArgument2Pointer],
                        _ => memory[pointer2Position]
                    };

                    var resultPosition = modeA switch
                    {
                        "1" => resultPointer,
                        "2" => relativeResultPointer,
                        _ => resultPointerPosition
                    };

                    if (code == 1)
                    {
                        memory[resultPosition] = (double.Parse(argument1) + double.Parse(argument2)).ToString();
                        pointerPosition += 3;
                    }
                    else if (code == 2)
                    {
                        memory[resultPosition] = (double.Parse(argument1) * double.Parse(argument2)).ToString();
                        pointerPosition += 3;
                    }
                    else if (code == 3)
                    {
                        var inputPointer = modeC != "2" ? pointer1Position : relativeArgument1Pointer;
                        memory[inputPointer] = input.ToString();
                        Console.WriteLine($"Input: {input}, Input Taken: {memory[inputPointer]}");
                        pointerPosition++;
                    }
                    else if (code == 4)
                    {
                        Console.WriteLine($"Output: " + argument1);
                        Outputs.Add(argument1);
                        pointerPosition++;
                        //return 0;
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
                            memory[resultPosition] = "1";
                        }
                        else
                        {
                            memory[resultPosition] = "0";
                        }
                        pointerPosition += 3;
                    }
                    else if (code == 8)
                    {
                        if (argument1 == argument2)
                        {
                            memory[resultPosition] = "1";
                        }
                        else
                        {
                            memory[resultPosition] = "0";
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

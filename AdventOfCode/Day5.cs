using System;
using System.IO;
using System.Linq;

namespace AdventOfCode
{
    internal class Day5
    {
        static void Main(string[] args)
        {
            var fileText = File.ReadAllText(@"Datasets\day5.txt");
            var intCode = fileText.Split(",").Select(int.Parse).ToArray();
            var input = 5;

            for (var i = 0; i < intCode.Length; i++)
            {
                var codeStr = ReverseInt(intCode[i]).ToString();
                var modeB = codeStr.Length > 3 ? int.Parse(codeStr[3].ToString()) : 0;
                var modeC = codeStr.Length > 2 ? int.Parse(codeStr[2].ToString()) : 0;
                var code = codeStr.Length > 1 ? int.Parse(codeStr[1].ToString() + codeStr[0].ToString()) : int.Parse(codeStr);

                if (code == 99) break;

                var arg1 = modeC == 1 ? intCode[i + 1] : intCode[intCode[i + 1]];
                var arg2 = modeB == 1 ? intCode[i + 2] : intCode[intCode[i + 2]];
                var resultIx = intCode[i + 3];

                if (code == 1)
                {
                    intCode[resultIx] = arg1 + arg2;
                    i += 3;
                }
                else if (code == 2)
                {
                    intCode[resultIx] = arg1 * arg2;
                    i += 3;
                }
                else if (code == 3)
                {
                    var index = intCode[i + 1];
                    intCode[index] = input;
                    i++;
                }
                else if (code == 4)
                {
                    Console.WriteLine($"Output: {arg1}");
                    i++;
                }
                else if (code == 5)
                {
                    if (arg1 != 0)
                    {
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
        }

        public static int ReverseInt(int num)
        {
            int result = 0;
            while (num > 0)
            {
                result = result * 10 + num % 10;
                num /= 10;
            }
            return result;
        }

    }
}

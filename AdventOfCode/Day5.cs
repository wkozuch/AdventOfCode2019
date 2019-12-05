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
            var strCode = fileText.Split(",").ToArray();
            var intCode = fileText.Split(",").Select(x => int.Parse(x)).ToArray();
            var input = 5; 

            for (var i = 0; i < intCode.Length; i++)
            {
                var codeStr = ReverseInt(intCode[i]).ToString();
                var modeA =  0;
                var modeB = codeStr.Length > 3 ? int.Parse(codeStr[3].ToString()) : 0;
                var modeC = codeStr.Length > 2 ? int.Parse(codeStr[2].ToString()) : 0;
                var code = codeStr.Length > 1 ? int.Parse(codeStr[1].ToString() + codeStr[0].ToString()) : int.Parse(codeStr);

                if (code == 99) break;
                
                var arg1 = modeC == 1 ? intCode[i + 1] : intCode[intCode[i + 1]];
                
                if (code == 1)
                {
                    var arg2 = modeB == 1 ? intCode[i + 2] : intCode[intCode[i + 2]];
                    var resultIx = intCode[i + 3];
                    intCode[resultIx] = arg1 + arg2;
                    i += 3;
                }
                else if (code == 2)
                {
                    var arg2 = modeB == 1 ? intCode[i + 2] : intCode[intCode[i + 2]];
                    var resultIx = intCode[i + 3];
                    intCode[resultIx] = arg1 * arg2;
                    i += 3;
                }
                else if (code == 3)
                {
                    intCode[arg1] = input;
                    i++;
                }
                else if (code == 4)
                {
                    //var result = input != 0 ? "1" : "0";
                    //var result = input < 8 ? "999" : input == 8 ? "1000" : "1001";
                    Console.WriteLine($"Output: {arg1}");
                    i++;
                }
                else if (code == 5)
                {
                    if (arg1 != 0)
                    {
                        intCode[i] = modeB == 0 ? intCode[i + 2] : intCode[intCode[i + 2]];
                       i--;
                    }
                }
                else if (code == 6)
                {
                    if (arg1 == 0)
                    {
                        intCode[i] = modeB == 0 ? intCode[i + 2] : intCode[intCode[i + 2]];
                       i--;
                    }
                }
                else if (code == 7)
                {
                    var arg2 = modeB == 0 ? intCode[i + 2] : intCode[intCode[i + 2]];
                    if (arg1 < arg2)
                    {
                        var resultIx = modeA == 0 ? intCode[i + 3] : intCode[intCode[i + 3]];
                        intCode[resultIx] = 1;
                    }
                    else
                    {
                        var resultIx = modeA == 0 ? intCode[i + 3] : intCode[intCode[i + 3]];
                        intCode[resultIx] = 0;
                    }
                    i += 3;
                }
                else if (code == 8)
                {
                    var arg2 = modeB == 0 ? intCode[i + 2] : intCode[intCode[i + 2]];
                    if (arg1 == arg2)
                    {
                        var resultIx = modeA == 0 ? intCode[i + 3] : intCode[intCode[i + 3]];
                        intCode[resultIx] = 1;
                    }
                    else
                    {
                        var resultIx = modeA == 0 ? intCode[i + 3] : intCode[intCode[i + 3]];
                        intCode[resultIx] = 0;
                    }
                    i += 3;
                }
                else if (code == 99)
                {
                    break;
                }

            }
        }



        public static string Reverse(string s)
        {
            char[] charArray = s.ToCharArray();
            Array.Reverse(charArray);
            return new string(charArray);
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

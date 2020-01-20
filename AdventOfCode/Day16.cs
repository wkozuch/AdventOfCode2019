using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;

namespace AdventOfCode
{
    internal class Day16
    {
        static void Main(string[] args)
        {
            var fileText = File.ReadAllText(@"Datasets\day16.txt");
            var input = fileText.ToCharArray().Select(x => int.Parse(x.ToString())).ToList();

            var phases = 100;
            var pattern = new List<int> { 0, 1, 0, -1 };
            Console.WriteLine($"Input signal: {string.Join("", input)}");

            //for (var i = 1; i <= phases; i++)
            //{
            //    var results = new List<int>();
            //    for (var n = 0; n < input.Count; n++)
            //    {
            //        var result = 0;
            //        var add = true;
            //        var a = n;
            //        while (a < input.Count)
            //        {
            //            var code = add ? 1 : -1;
            //            var count = 1;
            //            for (var b = a; b < Math.Min(a + n + 1, input.Count); b++)
            //            {
            //                count++;
            //                var digit = input[b];
            //                var op = b + 1 >= input.Count ? "=" : "+";
            //                result += code * digit;
            //                Console.Write($" {digit}*{code} {op} ");
            //            }
            //            add = !add;
            //            a += count + n;
            //        }
            //        //for (var j = n; j < input.Count; j++)
            //        //{
            //        //    if (indexCount++ == n)
            //        //    {
            //        //        indexCount = 0;
            //        //        index = Mod(++index, pattern.Count);
            //        //    }
            //        //    if (index == 0 || index == 2) continue;
            //        //    var code = pattern[index];
            //        //    var digit = input[j];
            //        //    var op = j + 1 == input.Count ? "=" : "+";
            //        //    result += code * digit;
            //        //    Console.Write($" {digit}*{code} {op} ");
            //        //}
            //        results.Add(Math.Abs(result % 10));
            //        Console.Write($" {result}  ({Math.Abs(result % 10)}) {Environment.NewLine}");
            //    }

            //    input = results;
            //    Console.WriteLine($"After {i} phase: {string.Join("", results)} ");
            //}



            input = fileText.ToCharArray().Select(x => int.Parse(x.ToString())).ToList();
            var repeat = 10000;
            var repeated_input = new List<int>();
            for (var i = 0; i < repeat; i++)
            {
                repeated_input.AddRange(input);
            }
            var offset = repeated_input.Take(7).ToList();
            var offsetNumber = 0;
            for (var i = 6; i >= 0; i--)
            {
                offsetNumber += (int)(offset[6 - i] * Math.Pow(10, i));
            }
            
            var to_skip = repeated_input.Count / 2;
            
            for (var i = 1; i <= phases; i++)
            {
                //var results = new List<int>();
                for (var n = repeated_input.Count - 2; n >= to_skip; n--)
                {
                    repeated_input[n] += repeated_input[n + 1];
                    repeated_input[n] %= 10;
                    //Console.Write($" {result}  ({Math.Abs(result % 10)}) {Environment.NewLine}");
                }

                //input = results;
                Console.WriteLine($"After {i} phase: {string.Join("", repeated_input)} ");
            }

            Console.WriteLine($"Skip {offsetNumber} Take{8}:  {string.Join("", repeated_input.Skip(offsetNumber).Take(8))}");

            //var offsetNumber = (int)Enumerable.Range(offset.Count(), 0).Select(x => offset[x] * Math.Pow(10, x)).Sum();

            for (var i = 1; i <= phases; i++)
            {
                var results = new List<int>();
                for (var n = 0; n < repeated_input.Count; n++)
                {
                    var result = 0;
                    //var indexCount = n;

                    var add = true;
                    var a = n;
                    while (a < repeated_input.Count)
                    {
                        var code = add ? 1 : -1;
                        var count = 1;
                        for (var b = a; b < Math.Min(a + n + 1, repeated_input.Count); b++)
                        {
                            count++;
                            var digit = repeated_input[b];
                            var op = b + 1 >= repeated_input.Count ? "=" : "+";
                            result += code * digit;
                            //  Console.Write($" {digit}*{code} {op} ");
                        }
                        add = !add;
                        a += count + n;
                    }

                    //for (var j = n; j < repeated_input.Count; j++)
                    //{
                    //    if (indexCount++ == n)
                    //    {
                    //        indexCount = 0;
                    //        index = Mod(++index, pattern.Count);
                    //    }
                    //    if (index == 0 || index == 2) continue;

                    //    var code = pattern[index];
                    //    if (code == 0) continue;
                    //    var digit = repeated_input[j];
                    //    //var op = j + 1 == repeated_input.Count ? "=" : "+";
                    //    result += code * digit;
                    //    // Console.Write($" {digit}*{code} {op} ");
                    //}
                    results.Add(Math.Abs(result % 10));
                    //Console.Write($" {result}  ({result %10}) {Environment.NewLine}");
                }

                repeated_input = results;
                Console.WriteLine($"After {i} phase: {string.Join("", results)} ");
            }
            Console.WriteLine($"Skip {offsetNumber} Take{8}:  {string.Join("", repeated_input.Skip(offsetNumber).Take(8))}");
        }

        private static int Mod(int x, int m)
        {
            var r = x % m;
            return r < 0 ? r + m : r;
        }
    }


}





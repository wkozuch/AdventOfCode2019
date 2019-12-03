using System;
using System.IO;
using System.Net.Http.Headers;
using System.Threading;

namespace AdventOfCode
{
   internal class Day2
   {
       static void Main(string[] args)
       {
           var calculator = new Calculator();
           //var lines = File.ReadAllLines(@"C:\Users\wk3213\source\repos\CalculateFuel\CalculateFuel\Datasets\day2.txt");

           for (int noun = 0; noun < 100; noun++)

           {
               for (int verb = 0; verb < 100; verb++)
               {
                   var intCode = new int[]
                   {
                       1, noun, verb, 3, 1, 1, 2, 3, 1, 3, 4, 3, 1, 5, 0, 3, 2, 6, 1, 19, 1, 19, 5, 23, 2, 9, 23, 27,
                       1, 5,
                       27, 31, 1, 5, 31, 35, 1, 35, 13, 39, 1, 39, 9, 43, 1, 5, 43, 47, 1, 47, 6, 51, 1, 51, 13, 55, 1,
                       55,
                       9, 59, 1, 59, 13, 63, 2, 63, 13, 67, 1, 67, 10, 71, 1, 71, 6, 75, 2, 10, 75, 79, 2, 10, 79, 83,
                       1,
                       5, 83, 87, 2, 6, 87, 91, 1, 91, 6, 95, 1, 95, 13, 99, 2, 99, 13, 103, 1, 103, 9, 107, 1, 10,
                       107,
                       111, 2, 111, 13, 115, 1, 10, 115, 119, 1, 10, 119, 123, 2, 13, 123, 127, 2, 6, 127, 131, 1, 13,
                       131,
                       135, 1, 135, 2, 139, 1, 139, 6, 0, 99, 2, 0, 14, 0
                   };

                   for (int i = 0; i < intCode.Length; i += 4)
                   {
                       var code = intCode[i];
                       if (code == 99) break;
                       var arg1Ix = intCode[i + 1];
                       var arg2Ix = intCode[i + 2];

                       var resultIx = intCode[i + 3];
                       if (code == 1)
                       {
                           intCode[resultIx] = intCode[arg1Ix] + intCode[arg2Ix];

                       }
                       else if (code == 2)
                       {
                           intCode[resultIx] = intCode[arg1Ix] * intCode[arg2Ix];
                       }
                       else if (code == 99)
                       {
                           break;
                       }

                   }

                   if (intCode[0] == 19690720)
                   {
                       Console.WriteLine($"intCode: +  100 * {noun} + {verb} = {100 * noun + verb}");
                       Console.ReadKey();
                   }
               }
           }

       }


   public class Calculator
   {
       public double CalculateFuel(double module_mass)
       {
           var fuel = Math.Floor(module_mass / 3.0) - 2.0;
           if (fuel <= 0)
           {
               return 0;
           }
           if (fuel > 0)
           {
               fuel += CalculateFuel(fuel);
           }
           return fuel;
       }
   }

}
}

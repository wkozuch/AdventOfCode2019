using System;
using System.IO;
using System.Net.Http.Headers;
using System.Threading;

namespace AdventOfCode
{
   class Day1
   {
       //static void Main(string[] args)
       //{
       //    var calculator = new Calculator();
       //    var lines = File.ReadAllLines(@"C:\Users\wk3213\source\repos\CalculateFuel\CalculateFuel\Datasets\day1.txt");
       //    double totalFuel = 0;
       //    foreach (var line in lines)
       //    {
       //        var mass = double.Parse(line);
       //        var fuel = calculator.CalculateFuel(mass);
       //        totalFuel += fuel;
       //    }

       //    Console.WriteLine($"Fuel: +  {totalFuel}");
       //    Console.ReadKey();
       //}

       static void Main(string[] args)
       {
           var calculator = new Calculator();
           var lines = File.ReadAllLines(@"Datasets\day1.txt");
           double totalFuel = 0;
           foreach (var line in lines)
           {
               var mass = double.Parse(line);
               var fuel = calculator.CalculateFuel(mass);
               totalFuel += fuel;
           }

           Console.WriteLine($"Fuel: +  {totalFuel}");
           Console.ReadKey();
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

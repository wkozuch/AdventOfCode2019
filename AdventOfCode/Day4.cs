using System;
using System.Linq;

namespace AdventOfCode
{
    internal class Day4
    {
        static void Main(string[] args)
        {
            var fileText = "138307-654504";
            var min_str = fileText.Split("-").First();
            var max_str = fileText.Split("-").Last();
            var min = int.Parse(min_str);
            var max = int.Parse(max_str);
            var count = 0;
            for (var i = min; i < max; i++)
            {
                if (Day4.Validate(i, min, max)) count++;
            }
            Console.WriteLine($"Passwords count: {count}");
        }

        static bool Validate(int number, int min, int max)
        {
            var charArray = number.ToString().ToCharArray();
            if (number > max) return false;
            if (number < min) return false;

            for (var i = 0; i < number.ToString().Length - 1; i++)
            {
                if (charArray[i] > charArray[i + 1]) return false;
            }
            var pair = number.ToString().GroupBy(x => x).Any(g => g.Count() == 2);
            if ( !pair) return false;

            return charArray.Distinct().Count() != 6;
        }
    }
}

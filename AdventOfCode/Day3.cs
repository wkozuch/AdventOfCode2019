using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AdventOfCode
{
    internal class Day3
    {
        static void Main(string[] args)
        {
            var fileText =
                File.ReadAllLines(@"Datasets\day3.txt");
            var wirePaths1 = fileText.First().Split(",");
            var wirePaths2 = fileText.Last().Split(",");
            int x = 0;
            int y = 0;
            var wirePoints1 = new List<Point>();
            var wirePoints2 = new List<Point>();
            var steps = 0;
            foreach (var wire in wirePaths1)
            {
                var direction = wire[0].ToString();
                var count = int.Parse(string.Join("", wire.Skip(1)));
                for (var i = 0; i < count; i++)
                {
                    if (direction == "R")
                        y += 1;
                    else if (direction == "L")
                        y -= 1;
                    else if (direction == "U")
                        x += 1;
                    else if (direction == "D") x -= 1;

                    wirePoints1.Add(new Point(x, y, ++steps));
                }
            }

            x = 0;
            y = 0;
            steps = 0;
            foreach (var wire in wirePaths2)
            {
                var direction = wire[0].ToString();
                var count = int.Parse(string.Join("", wire.Skip(1)));
                for (var i = 0; i < count; i++)
                {
                    if (direction == "R")
                        y = y + 1;
                    else if (direction == "L")
                        y = y - 1;
                    else if (direction == "U")
                        x = x + 1;
                    else if (direction == "D") x = x - 1;

                    wirePoints2.Add(new Point(x, y, ++steps));
                }
            }

            var intersections = new List<int>();
            for (var i = 0; i < wirePoints1.Count; i++)
            {
                var wire = wirePoints1[i];
                for (var j = 0; j < wirePoints2.Count; j++)
                {
                    var wire2 = wirePoints2[j];
                    if (wire.X == wire2.X & wire.Y == wire2.Y)
                    {
                        var step = wire.Steps + wire2.Steps;
                        intersections.Add(step);
                    }
                }
            }

            var distance = intersections.Min();
            Console.WriteLine($"Distance: +  {distance}");
        }

        public class Point
        {
            public Point(int x, int y, int steps)
            {
                X = x;
                Y = y;
                Steps = steps;
            }
            public int X { get; }
            public int Y { get; }

            public int Steps { get; set; }
            public int Distance()
            {
                return Math.Abs(X) + Math.Abs(Y);
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.Tracing;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;

namespace AdventOfCode
{
    internal class Day10
    {
        static void Main(string[] args)
        {
            var skymap = File.ReadAllLines(@"Datasets\day10.txt").Select(l => l.Select(i => i).ToArray())
                .ToArray(); ;
            //var strCode = fileText.Select(x => x.Select(x => ",")",")(",").ToArray();

            var detections = new int[skymap.Length, skymap[0].Length];

            var result = 0;
            var r = 0;
            var c = 0;
            for (var row = 0; row < skymap.Length; row++)
            {
                for (var col = 0; col < skymap[0].Length; col++)
                {
                    if (skymap[row][col] != '#') continue;
                    detections[row, col] = Count(skymap, row, col);

                    if (result < detections[row, col])
                    {
                        result = detections[row, col];
                        r = row;
                        c = col;
                    }
                }
            }
            Console.WriteLine(result);
            Console.WriteLine("Location: (" + c + "," + r + ")");

            var destroyedSkyMap = DestroyAsteroids(skymap, r, c);
        }

        private static int Count(char[][] skymap, int r, int c)
        {
            var tang = new List<double>();
            var tangs = new double[skymap.Length, skymap[0].Length];
            var index = 0;
            for (var row = 0; row < skymap.Length; row++)
            {
                for (var col = 0; col < skymap[0].Length; col++)
                {
                    if (skymap[row][col] != '#') continue;
                    if (row == r && col == c) continue;

                    var deg = Math.Atan((double)(row - r) / (double)(col - c)) * (180 / Math.PI);
                    if (col >= c)
                    {
                        deg = deg + 180;
                    }

                    tang.Add(deg);
                    tangs[row, col] = deg;
                }
            }

            return tang.Distinct().Count();
        }

        private static char[][] DestroyAsteroids(char[][] skymap, int r, int c)
        {
            var tang = new List<double>();
            var tangs = Enumerable.Range(0, skymap.Length).Select(x => Enumerable.Range(0, skymap[0].Length).Select(y => double.NaN).ToArray()).ToArray();
            for (var row = 0; row < skymap.Length; row++)
            {
                for (var col = 0; col < skymap[0].Length; col++)
                {
                    if (skymap[row][col] != '#') continue;
                    if (row == r && col == c) continue;

                    var deg = Math.Atan((double)(row - r) / (double)(col - c)) * (180 / Math.PI);
                    if (col >= c)
                    {
                        deg += 180;
                    }

                    tang.Add(deg);
                    tangs[row][col] = deg;
                }
            }

            var visibleAsteroids = tang.Distinct().OrderBy(x => x).ToList();
            var laserStartAngle = visibleAsteroids.IndexOf(visibleAsteroids.First(x => x >= 90));
            var i = 0;

            //For each visible asteroid find the one closest for a given angle and destroy it
            for (var index = laserStartAngle; index < visibleAsteroids.Count + laserStartAngle; index++)
            {
                var angle = visibleAsteroids[index % visibleAsteroids.Count];
                var destroyedR = 0;
                var destroyedC = 0;
                var distance = int.MaxValue;
                for (var row = 0; row < skymap.Length; row++)
                {
                    for (var col = 0; col < skymap[0].Length; col++)
                    {
                        if (tangs[row][col] != angle || (Math.Abs(row - r) + Math.Abs(col - c)) >= distance) continue;
                        destroyedR = row;
                        destroyedC = col;
                        distance = Math.Abs(row - r) + Math.Abs(col - c);
                    }
                }

                Console.WriteLine(@"Destroyed #" + ++i + "Location: (" + destroyedC + ", " + destroyedR + ")");
                skymap[destroyedR][destroyedC] = '.';
            }

            return skymap;
        }

    }
}

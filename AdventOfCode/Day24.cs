using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading;

namespace AdventOfCode
{
    internal class Day24
    {
        static void Main(string[] args)
        {
            var fileText = File.ReadAllLines(@"Datasets\day24.txt");
            var map = fileText.Select(x => x.ToCharArray()).ToList();
            var counter = 0;
            var results = new List<double>();
            while (true)
            {
                var newMinute = new List<char[]>();

                for (int r = 0; r < map.Count; r++)
                {
                    var row = new char[map[0].Length];
                    for (int c = 0; c < map[0].Length; c++)
                    {
                        row[c] = GetNextTile(map, c, r);

                    }

                    newMinute.Add(row);
                }

                map = newMinute;
                counter++;
                var result = PrintNewMinute(map);
                if (results.Contains(result)) break;
                results.Add(result);
                //Thread.Sleep(1000);
            }

        }


        public class Grid : Tile
        {
            public List<Tile[]> Tiles { get; }
            public int Level { get; }

            public Grid(int level, int x, int y) : base(level, x, y)
            {

            }

            public List<Tile> GetAdjacentTiles(Tile tile)
            {

            }

            public List<Tile[]> Update()
            {
                var newMinute = new List<Tile[]>();

                for (var r = 0; r < Tiles.Count; r++)
                {
                    var row = Tiles[r];
                    for (var c = 0; c < row.Length; c++)
                    {
                        var tile = row[c];
                        var grid = tile as Grid;
                        if (tile is Grid)
                        {
                            row[c] = grid.Update();
                            continue;
                        }

                        var adjacentTiles = GetAdjacentTiles(tile);
                        var newChar = row[c].Value;
                        if (tile.Value == '#' && adjacentTiles.Count(x => x.Value.Equals('#')) == 1) newChar = '#';
                        if (tile.Value == '#' && adjacentTiles.Count(x => x.Value.Equals('#')) != 1) newChar = '.';
                        if (tile.Value == '.' && adjacentTiles.Count(x => x.Value.Equals('#')) == 1 ||
                            adjacentTiles.Count(x => x.Value.Equals('#')) == 2) newChar = '#';
                        row[c].Value = newChar;
                    }
                    newMinute.Add(row);
                }

                return newMinute;
            }
        }
        public class Tile : List<Tile[]>
        {
            public int Level { get; }
            public int X { get; }
            public int Y { get; }

            public char Value { get; set; }

            public Tile(int level, int x, int y, char value)
            {
                Level = level;
                X = x;
                Y = y;
                Value = value;
            }



        }
        private static double PrintNewMinute(List<char[]> map)
        {

            var i = 0;
            var result = 0.0;
            for (int r = 0; r < map.Count; r++)
            {
                for (int c = 0; c < map[0].Length; c++)
                {
                    Console.SetCursorPosition(c, r);
                    Console.Write(map[r][c]);
                    var count = map[r][c] == '#' ? 1 : 0;
                    result += count * Math.Pow(2, i++);
                }
            }
            Console.SetCursorPosition(0, map.Count + 1);
            Console.Write($"Result: {result}");
            return result;
        }

        private static char GetNextTile(List<char[]> map, in int x, in int y)
        {
            var neigbours = new List<char>();
            if (x - 1 >= 0) neigbours.Add(map[y][x - 1]);
            if (y - 1 >= 0) neigbours.Add(map[y - 1][x]);
            if (x + 1 < map[0].Length) neigbours.Add(map[y][x + 1]);
            if (y + 1 < map.Count) neigbours.Add(map[y + 1][x]);

            if (map[y][x] == '#' && neigbours.Count(x => x.Equals('#')) == 1) return '#';
            if (map[y][x] == '#' && neigbours.Count(x => x.Equals('#')) != 1) return '.';
            if (map[y][x] == '.' && neigbours.Count(x => x.Equals('#')) == 1 || neigbours.Count(x => x.Equals('#')) == 2) return '#';
            return map[y][x];
        }
    }
}
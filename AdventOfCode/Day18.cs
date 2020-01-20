using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Xml.Linq;
using System.Xml.Schema;
using AdventOfCode.Graphs;

namespace AdventOfCode
{
    internal class Day18
    {
        static void Main(string[] args)
        {
            var fileText = File.ReadAllLines(@"Datasets\day18.txt");
            var screen = fileText.Select(x => x.ToCharArray()).ToArray();
            var labyrinth = new Labyrinth(screen);
            //labyrinth.Print();
            var result = 0; 
            var allDoorsOpened = false;

            //while (!allDoorsOpened)
            //{
                labyrinth.FindObjects();
                labyrinth.CalculateReachableKeysAndDoors();
                var dict = new Dictionary<Point, int>();
                var keyDictionary = new Dictionary<char, List<Point>>();
                foreach (var rKey in labyrinth.Keys)
                {
                    Console.WriteLine($"Key: {rKey.Name}");
                    var area = labyrinth.Area.Clone() as char[][];
                    var lab = new Labyrinth(area);
                    var ent = new Point ('@', lab.Entrance.X, lab.Entrance.Y);
                    lab.MoveEntrance(rKey);
                    lab.FindObjects();
                    lab.Print();
                    lab.CalculateReachableKeysAndDoors();
                    lab.MoveEntrance(ent);
                    lab.Area[rKey.Y][rKey.X] = rKey.Name;
                    var distance = lab.GetClosestKeyDistance();
                    dict.Add(rKey, distance);
                    keyDictionary.Add(rKey.Name, lab.ReachableKeys.Keys.ToList());

                }
                //result += dict.OrderBy(x => x.Value).First().Value;
                //var key = dict.OrderBy(x => x.Value).First().Key;
                //labyrinth.OpenDoor(key);

                labyrinth.Print();
            //    if (labyrinth.Doors.Count == 0 && labyrinth.Keys.Count == 0) allDoorsOpened = true;
            //}

            Console.WriteLine($"Distance: {result}");
        }

        public class Point
        {
            public char Name;
            public int X;
            public int Y;

            public Point(char name, int x, int y)
            {
                Name = name;
                X = x;
                Y = y;
            }

        }

        public class Labyrinth
        {
            public Point Entrance;

            public Dictionary<Point, int> ReachableDoors = new Dictionary<Point, int>();
            public Dictionary<Point, int> ReachableKeys = new Dictionary<Point, int>();

            public List<Point> Doors = new List<Point>();
            public List<Point> Keys = new List<Point>();
            public List<Point> Walls = new List<Point>();
            public List<Point> Paths = new List<Point>();

            public int Distance = 0;

            public char[][] Area;

            public Labyrinth(char[][] area)
            {
                Area = area;

                var x0 = Enumerable.Range(0, Area.Length).Single(x => Area[x].Contains('@'));
                var y0 = Enumerable.Range(0, Area[x0].Length).Single(y => Area[x0][y] == '@');
                Entrance = new Point('@', y0, x0);
            }

            public void MoveEntrance(Point newEntrance)
            {
                Area[Entrance.Y][Entrance.X] = '.';
                Area[newEntrance.Y][newEntrance.X] = '@';
                Entrance.X = newEntrance.X;
                Entrance.Y = newEntrance.Y;
            }

            public void FindObjects()
            {
                Doors = new List<Point>();
                Keys = new List<Point>();

                var x0 = Enumerable.Range(0, Area.Length).Single(x => Area[x].Contains('@'));
                var y0 = Enumerable.Range(0, Area[x0].Length).Single(y => Area[x0][y] == '@');
                Entrance = new Point('@', y0, x0);

                for (var row = 0; row < Area.Length; row++)
                {
                    var line = Area[row];
                    for (var col = 0; col < line.Length; col++)
                    {
                        var name = line[col];
                        if (line[col] == '#' || line[col] == '.' || line[col] == '@') continue;

                        if (line[col] == char.ToUpper(line[col]))
                        {
                            Doors.Add(new Point(name, col, row));
                        }
                        else
                        {
                            Keys.Add(new Point(name, col, row));
                        }
                    }
                }
            }

            public void CalculateReachableKeysAndDoors()
            {
                ReachableDoors = new Dictionary<Point, int>();
                ReachableKeys = new Dictionary<Point, int>();

                foreach (var key in Keys)
                {
                    var start = new Location(Entrance.Name, Entrance.X, Entrance.Y);
                    var target = new Location(key.Name, key.X, key.Y);
                    var area = Area.Clone() as char[][];
                    var reachable = CalculateDistance(start, target, area, out var distance);
                    if (reachable)
                        ReachableKeys.Add(key, distance);
                }

                foreach (var door in Doors)
                {
                    var start = new Location(Entrance.Name, Entrance.X, Entrance.Y);
                    var target = new Location(door.Name, door.X, door.Y);
                    var area = Area.Clone() as char[][];
                    var reachable = CalculateDistance(start, target, area, out var distance);
                    if (reachable)
                        ReachableDoors.Add(door, distance);
                }


                //while (!cleared)
                //{
                //    area = FindPathFromEntrance(area, out cleared);
                //}
            }

            private char[][] FindPathFromEntrance(char[][] screen, out bool cleared)
            {
                var directions = new[] { '>', 'v', '<', '^' };
                //var visited = Enumerable.Range(0, screen.Length)
                //    .Select(r => Enumerable.Range(0, screen[0].Length).Select(c => false).ToArray()).ToArray();
                var directionIndex = 0;
                var inputKey = directions[directionIndex];
                var x = Entrance.X;
                var y = Entrance.Y;
                var found = false;
                var wallCount = 0;
                var distance = new Distance(x, y);
                cleared = false;
                while (!found)
                {
                    var newX = x;
                    var newY = y;
                    switch (inputKey)
                    {
                        case '^':
                            newY = y - 1;
                            break;
                        case 'v':
                            newY = y + 1;
                            break;
                        case '<':
                            newX = x - 1;
                            break;
                        case '>':
                            newX = x + 1;
                            break;
                    }

                    var tile = screen[newY][newX];
                    //Console.WriteLine();
                    switch (tile)
                    {
                        case '#':
                            directionIndex = Mod(++directionIndex, directions.Length);
                            inputKey = directions[directionIndex];
                            //screen[y][x] = inputKey;
                            wallCount++;
                            break;
                        case '.':
                        case '@':
                            directionIndex = Mod(--directionIndex, directions.Length);
                            inputKey = directions[directionIndex];
                            screen[y][x] = '.';
                            var dist = distance.Update(newX, newY);
                            //visited[y][x] = true;
                            ////screen[newY][newX] = inputKey;

                            //if (!visited[newY][newX]) distance++;
                            //else distance--;

                            //visited[newY][newX] = true;
                            y = newY;
                            x = newX;
                            break;
                        default:
                            screen[newY][newX] = '#';
                            if (tile == char.ToUpper(tile))
                            {
                                ReachableDoors.Add(new Point(tile, newX, newY), distance.Update(newX, newY));
                            }
                            else
                            {
                                ReachableKeys.Add(new Point(tile, newX, newY), distance.Update(newX, newY));
                            }

                            directionIndex = Mod(++directionIndex, directions.Length);
                            inputKey = directions[directionIndex];
                            found = true;
                            break;
                    }

                    //var str = string.Join("", Area.Select(line => string.Join("", line) + Environment.NewLine));
                    //Console.Write(str);

                    if (wallCount > 500)
                    {
                        cleared = true;
                        break;
                    }
                }

                return screen;
            }

            private static int Mod(int x, int m)
            {
                var r = x % m;
                return r < 0 ? r + m : r;
            }

            public int GetClosestKeyDistance()
            {
                if (ReachableKeys.Any())
                    return ReachableKeys.OrderBy(x => x.Key.Name).OrderBy(x => x.Value).First().Value;
                return int.MaxValue;
            }

            public Point GetClosestKey()
            {
                var key = ReachableKeys.OrderBy(x => x.Key.Name).OrderBy(x => x.Value).First().Key;
                var closestKeys = ReachableKeys.OrderBy(x => x.Key.Name).OrderBy(x => x.Value).ToList();
                var distance = int.MaxValue;
                foreach (var k in closestKeys)
                {
                    if (ReachableDoors.Keys.Select(x => x.Name).Contains(char.ToUpper(k.Key.Name)))
                    {
                        //var door = ReachableDoors.First(x => x.Key.Name == char.ToUpper(k.Key.Name));
                        //if (distance > door.Value + k.Value)
                        //{
                        key = k.Key;
                        break;
                        //    distance = door.Value + k.Value;
                        //}
                    }
                }

                Distance += ReachableKeys[key];
                Console.WriteLine($"Closest key: {key.Name} Distance: {ReachableKeys[key]} TotalDistance: {Distance}");
                return key;
            }

            public void OpenDoor(Point key)
            {
                //MoveEntrance(key);
                Area[Entrance.Y][Entrance.X] = '.';
                Area[key.Y][key.X] = '@';
                Entrance.X = key.X;
                Entrance.Y = key.Y;
                ReachableKeys.Remove(key);
                Keys.RemoveAll(x => x.Name == key.Name);
                if (Doors.Any(x => x.Name == char.ToUpper(key.Name)))
                {
                    var door = Doors.Single(x => x.Name == char.ToUpper(key.Name));
                    Console.WriteLine($"Opened Door: {door.Name}");
                    Area[door.Y][door.X] = '.';
                    Doors.RemoveAll(x => x.Name == door.Name);
                }
            }

            public void Print()
            {
                var str = string.Join("", Area.Select(line => string.Join("", line) + Environment.NewLine));
                Console.Write(str);
            }
        }


        internal class Location : Point
        {
            public int F;
            public int G;
            public int H;
            public Location Parent;

            public Location(char name, int x, int y) : base(name, x, y)
            {
            }
        }

        static bool CalculateDistance(Location start, Location target, char[][] map, out int distance)
        {
            Location current = null;
            var openList = new List<Location>();
            var closedList = new List<Location>();
            var g = 0;
            //Console.Clear();
            //var str = string.Join("", map.Select(line => string.Join("", line) + Environment.NewLine));
            //Console.Write(str);

            // start by adding the original position to the open list
            openList.Add(start);

            while (openList.Count > 0)
            {
                // get the square with the lowest F score
                var lowest = openList.Min(l => l.F);
                current = openList.First(l => l.F == lowest);

                // add the current square to the closed list
                closedList.Add(current);

                //// show current square on the map
                //map[current.Y][current.X] = 'O';
                //var str = string.Join("", map.Select(line => string.Join("", line) + Environment.NewLine));
                //Console.Write(str);
                //Console.SetCursorPosition(current.X, current.Y);
                //Console.Write('_');
                //Console.SetCursorPosition(current.X, current.Y);
                //System.Threading.Thread.Sleep(100);

                // remove it from the open list
                openList.Remove(current);

                // if we added the destination to the closed list, we've found a path
                if (closedList.FirstOrDefault(l => l.X == target.X && l.Y == target.Y) != null)
                    break;

                var adjacentSquares = GetWalkableAdjacentSquares(current.X, current.Y, target, map);
                g++;

                foreach (var adjacentSquare in adjacentSquares)
                {
                    // if this adjacent square is already in the closed list, ignore it
                    if (closedList.FirstOrDefault(l => l.X == adjacentSquare.X
                                                       && l.Y == adjacentSquare.Y) != null)
                        continue;

                    // if it's not in the open list...
                    if (openList.FirstOrDefault(l => l.X == adjacentSquare.X
                                                     && l.Y == adjacentSquare.Y) == null)
                    {
                        // compute its score, set the parent
                        adjacentSquare.G = g;
                        adjacentSquare.H = ComputeHScore(adjacentSquare.X, adjacentSquare.Y, target.X, target.Y);
                        adjacentSquare.F = adjacentSquare.G + adjacentSquare.H;
                        adjacentSquare.Parent = current;

                        // and add it to the open list
                        openList.Insert(0, adjacentSquare);
                    }
                    else
                    {
                        // test if using the current G score makes the adjacent square's F score
                        // lower, if yes update the parent because it means it's a better path
                        if (g + adjacentSquare.H < adjacentSquare.F)
                        {
                            adjacentSquare.G = g;
                            adjacentSquare.F = adjacentSquare.G + adjacentSquare.H;
                            adjacentSquare.Parent = current;
                        }
                    }
                }
            }

            distance = -1;
            //// assume path was found; let's show it
            while (current != null)
            {
                //Console.SetCursorPosition(current.X, current.Y);
                //Console.Write('_');
                //Console.SetCursorPosition(current.X, current.Y);
                current = current.Parent;
                //System.Threading.Thread.Sleep(100);
                distance++;
            }
            return closedList.FirstOrDefault(l => l.X == target.X && l.Y == target.Y) != null;

            // end

            //Console.ReadLine();
        }

        static List<Location> GetWalkableAdjacentSquares(int x, int y, Location target, char[][] map)
        {
            var proposedLocations = new List<Location>()
            {
                new Location (map[y-1][x], x, y - 1),
                new Location (map[y+1][x], x, y + 1),
                new Location (map[y][x-1], x-1, y),
                new Location (map[y][x+1], x+1, y),
            };

            return proposedLocations.Where(l => (l.X == target.X && l.Y == target.Y) || (
                map[l.Y][l.X] != '#' & !char.IsUpper(map[l.Y][l.X]))).ToList();
        }

        static int ComputeHScore(int x, int y, int targetX, int targetY)
        {
            return Math.Abs(targetX - x) + Math.Abs(targetY - y);
        }


        internal class Distance
        {
            private int _x;
            private int _y;
            private List<int> xDeltas = new List<int>();
            private List<int> yDeltas = new List<int>();

            public Distance(int x, in int y)
            {
                _x = x;
                _y = y;
            }

            public int Update(int new_x, int new_y)
            {
                xDeltas.Add(new_x - _x);
                yDeltas.Add(new_y - _y);
                _x = new_x;
                _y = new_y;

                return Math.Abs(yDeltas.Sum() + xDeltas.Sum());
            }


        }
    }
}
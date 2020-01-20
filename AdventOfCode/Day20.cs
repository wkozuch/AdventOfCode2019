using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Reflection;
using System.Reflection.Metadata.Ecma335;
using System.Reflection.PortableExecutable;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks.Dataflow;
using System.Xml.Linq;
using System.Xml.Schema;
using AdventOfCode.Graphs;

namespace AdventOfCode
{
    internal class Day20
    {
        static void Main(string[] args)
        {
            var fileText = File.ReadAllLines(@"Datasets\day20.txt");
            var screen = fileText.Select(x => x.ToCharArray()).ToArray();
            var labyrinth = new Labyrinth(screen);
            labyrinth.Print();
            labyrinth.FindObjects();
            labyrinth.CalculateReachableKeysAndDoors();
        }

        internal class Point
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

            public List<string> Portals => OuterPortals.Select(x => x.Name).Union(InnerPortals.Select(x => x.Name).ToList())
                .Distinct().OrderBy(x => x).ToList();
            public List<Portal> OuterPortals = new List<Portal>();
            public List<Portal> InnerPortals = new List<Portal>();

            public char[][] Area;

            public Labyrinth(char[][] area)
            {
                Area = area;
            }

            public void FindObjects()
            {
                OuterPortals = new List<Portal>();
                InnerPortals = new List<Portal>();
                var entranceRows = new List<int> { 0, Area.Length - 2 };
                var entranceColumns = new List<int> { 0, Area[0].Length - 2 };

                foreach (var row in entranceRows)
                {
                    var line = Area[row];
                    var lineBelow = Area[row + 1];
                    for (var col = 0; col < line.Length; col++)
                    {
                        var name = line[col];
                        if (name == '#' || name == '.' || name == ' ') continue;

                        var entrance = new Point(name, col, row);
                        var exit = new Point(lineBelow[col], col, row + 1);
                        var portal = new Portal(entrance, exit, false);
                        OuterPortals.Add(portal);
                    }
                }

                foreach (var col in entranceColumns)
                {
                    for (var row = 0; row < Area.Length; row++)
                    {
                        var nextColumn = Area[row][col + 1];

                        var name = Area[row][col];
                        if (name == '#' || name == '.' || name == ' ') continue;

                        var entrance = new Point(name, col, row);
                        var exit = new Point(nextColumn, col + 1, row);
                        var portal = new Portal(entrance, exit, false);
                        OuterPortals.Add(portal);
                    }
                }

                var innerTopLeft = new Point(' ', 2, 2);
                while (Area[innerTopLeft.Y][innerTopLeft.X] != ' ')
                {
                    //Console.SetCursorPosition(innerTopLeft.X, innerTopLeft.Y);
                    //Thread.Sleep(25);
                    innerTopLeft.X++;
                    if (innerTopLeft.X != Area[0].Length - 3) continue;
                    innerTopLeft.X = 2;
                    innerTopLeft.Y++;
                }

                var innerBottomRight = new Point(' ', Area[0].Length - 3, Area.Length - 3);
                while (Area[innerBottomRight.Y][innerBottomRight.X] != ' ')
                {
                    //Console.SetCursorPosition(innerBottomRight.X, innerBottomRight.Y);
                    //Thread.Sleep(25);
                    innerBottomRight.Y--;
                    if (innerBottomRight.Y != 2) continue;
                    innerBottomRight.Y = Area.Length - 3;
                    innerBottomRight.X--;
                }

                var innerRows = new List<int> { innerTopLeft.Y, innerBottomRight.Y - 1 };
                var innerColumns = new List<int> { innerTopLeft.X, innerBottomRight.X - 1 };

                foreach (var row in innerRows)
                {
                    var line = Area[row];
                    var lineBelow = Area[row + 1];
                    for (var col = innerColumns[0]; col < innerColumns[1]; col++)
                    {
                        var name = line[col];
                        //Console.SetCursorPosition(col, row);
                        if (name == '#' || name == '.' || name == ' ' || lineBelow[col] == ' ') continue;
                        var entrance = new Point(name, col, row);
                        var exit = new Point(lineBelow[col], col, row + 1);
                        var portal = new Portal(entrance, exit, true);
                        InnerPortals.Add(portal);
                    }
                }

                foreach (var col in innerColumns)
                {
                    for (var row = innerRows[0]; row < innerRows[1] + 1; row++)
                    {
                        var nextColumn = Area[row][col + 1];

                        var name = Area[row][col];
                        //Console.SetCursorPosition(col, row);
                        if (name == '#' || name == '.' || name == ' ' || nextColumn == ' ') continue;
                        var entrance = new Point(name, col, row);
                        var exit = new Point(nextColumn, col + 1, row);
                        var portal = new Portal(entrance, exit, true);
                        InnerPortals.Add(portal);
                    }
                }

                Console.WriteLine(string.Join(",", Portals));
            }

            public void CalculateReachableKeysAndDoors()
            {
                var graph = new Graph(Portals.Count);

                var names = OuterPortals.Select(x => x.Name + x.Suffix).Union(InnerPortals.Select(x => x.Name + x.Suffix)).ToArray();
                var graph2 = Enumerable.Range(0, names.Length).Select(x => Enumerable.Range(0, names.Length).Select(x => x).ToArray()).ToArray();

                var otherPortals = OuterPortals.Union(OuterPortals.Select(x => x.Reverse()).Union(InnerPortals).Union(InnerPortals.Select(x => x.Reverse()))).ToList();
                var vertexes = otherPortals.Select(x => new Vertex(x.Name + x.Suffix)).ToList();
                var index = 0; 
                foreach (var outerPortal in otherPortals)
                {
                    var vertex = vertexes[index++];

                    foreach (var portal in otherPortals)
                    {
                        var start = new Location(outerPortal.Name.Last(), outerPortal.Exit.X, outerPortal.Exit.Y);
                        var target = new Location(portal.Name.First(), portal.Entry.X, portal.Entry.Y);
                        var outerPortalType = outerPortal.Inner ? "Inner" : "Outer";
                        var innerPortalType = portal.Inner ? "Inner" : "Outer";
                        if (outerPortal.Name == portal.Name || outerPortal.Name == Reverse(portal.Name))
                        {
                            if (outerPortal.Inner != portal.Inner)
                            {
                                //if (graph.Vertices.All(x => x.Value != vertex.Value))
                                //{
                                //    graph.Vertices.First(x => x.Value == vertex.Value).AddEdge(new Vertex<string>(portal.Name + portal.Suffix), 0);
                                //}

                                if (vertex.Neighbors.All(x => x.Value != portal.Name + portal.Suffix))
                                {
                                    var self = vertexes.First(x => x.Value == portal.Name + portal.Suffix);
                                    vertex.AddEdge(self, 0);
                                    Console.WriteLine(
                                        $"{outerPortalType}Portal {outerPortal.Name + outerPortal.Suffix}-> {innerPortalType}Portal {portal.Name + portal.Suffix} Distance {0}");
                                }
                            }

                            continue;
                        }

                        var reachable = CalculateDistance(start, target, Area, out var distance);
                        if (!reachable) continue;
                        Console.WriteLine(
                            $"{outerPortalType}Portal {outerPortal.Name} -> {innerPortalType}Portal {portal.Name} Distance {distance}");
                        var next = vertexes.First(x => x.Value == portal.Name + portal.Suffix);
                        vertex.AddEdge(next, distance);

                    }

                    if (graph.Vertices.Any(x => x.Value == vertex.Value))
                    {
                        foreach (var aver in vertex.Neighbors)
                            if (graph.Vertices.First(x => x.Value == vertex.Value).Neighbors
                                .All(x => x.Value != aver.Value))
                            {
                                graph.Vertices.First(x => x.Value == vertex.Value).AddEdge(aver);
                            };
                    }

                    if (vertex.NeighborsCount > 0 && !graph.HasVertex(vertex))
                    {
                        graph.AddVertex(vertex);
                    }
                }

                var source = graph.Vertices.First(x => x.Value == "AA_out");
                var distanceTable = new Dictionary<string, Tuple<int, string>>();
                foreach (var vertex in graph.Vertices)
                {
                    distanceTable.Add(vertex.Value, null);
                }
                distanceTable[source.Value] = new Tuple<int, string>(0, source.Value);

                var queue = new Stack<Vertex>();
                queue.Push(source);

                while (queue.Count > 0)
                {
                    var currentVertex = queue.Pop();
                    var currentDistance = distanceTable[currentVertex.Value].Item1;
                    foreach (var neighbour in currentVertex.Neighbors)
                    {
                        var weight = currentVertex.NeighborsWeigths[neighbour];
                        var value = distanceTable[neighbour.Value];
                        if (value == null)
                        {
                            distanceTable[neighbour.Value] = new Tuple<int, string>(currentDistance + weight, currentVertex.Value);
                        }

                        if (neighbour.Neighbors.Count > 0)
                        {
                            queue.Push(neighbour);
                        }
                    }
                }

                var destination = graph.Vertices.Single(x => x.Value == "ZZ_out");
                var path = 0;
                var previousVertex = distanceTable[destination.Value];

                while (previousVertex != null & previousVertex.Item2 != "AA_out")
                {
                    path += previousVertex.Item1;
                    previousVertex = distanceTable[previousVertex.Item2];
                }
                Console.Write($"Path: {path}");

            }

            //private char[][] FindPathFromEntrance(char[][] screen, out bool cleared)
            //{
            //    var directions = new[] { '>', 'v', '<', '^' };
            //    //var visited = Enumerable.Range(0, screen.Length)
            //    //    .Select(r => Enumerable.Range(0, screen[0].Length).Select(c => false).ToArray()).ToArray();
            //    var directionIndex = 0;
            //    var inputKey = directions[directionIndex];
            //    var x = Entrance.X;
            //    var y = Entrance.Y;
            //    var found = false;
            //    var wallCount = 0;
            //    var distance = new Distance(x, y);
            //    cleared = false;
            //    while (!found)
            //    {
            //        var newX = x;
            //        var newY = y;
            //        switch (inputKey)
            //        {
            //            case '^':
            //                newY = y - 1;
            //                break;
            //            case 'v':
            //                newY = y + 1;
            //                break;
            //            case '<':
            //                newX = x - 1;
            //                break;
            //            case '>':
            //                newX = x + 1;
            //                break;
            //        }

            //        var tile = screen[newY][newX];
            //        //Console.WriteLine();
            //        switch (tile)
            //        {
            //            case '#':
            //                directionIndex = Mod(++directionIndex, directions.Length);
            //                inputKey = directions[directionIndex];
            //                //screen[y][x] = inputKey;
            //                wallCount++;
            //                break;
            //            case '.':
            //            case '@':
            //                directionIndex = Mod(--directionIndex, directions.Length);
            //                inputKey = directions[directionIndex];
            //                screen[y][x] = '.';
            //                var dist = distance.Update(newX, newY);
            //                //visited[y][x] = true;
            //                ////screen[newY][newX] = inputKey;

            //                //if (!visited[newY][newX]) distance++;
            //                //else distance--;

            //                //visited[newY][newX] = true;
            //                y = newY;
            //                x = newX;
            //                break;
            //            default:
            //                screen[newY][newX] = '#';
            //                if (tile == char.ToUpper(tile))
            //                {
            //                    ReachableDoors.Add(new Point(tile, newX, newY), distance.Update(newX, newY));
            //                }
            //                else
            //                {
            //                    ReachableKeys.Add(new Point(tile, newX, newY), distance.Update(newX, newY));
            //                }

            //                directionIndex = Mod(++directionIndex, directions.Length);
            //                inputKey = directions[directionIndex];
            //                found = true;
            //                break;
            //        }

            //        //var str = string.Join("", Area.Select(line => string.Join("", line) + Environment.NewLine));
            //        //Console.Write(str);

            //        if (wallCount > 500)
            //        {
            //            cleared = true;
            //            break;
            //        }
            //    }

            //    return screen;
            //}

            private static int Mod(int x, int m)
            {
                var r = x % m;
                return r < 0 ? r + m : r;
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

        internal class Portal
        {
            public string Suffix => _suffix;
            public string Name { get; set; }

            public Point Entry { get; }
            public Point Exit { get; }

            public bool Inner;
            private string _suffix;

            internal Portal(Point entry, Point exit, bool inner)
            {
                Entry = entry;
                Exit = exit;
                Inner = inner;
                _suffix = inner ? "_in" : "_out";
                Name = $"{Entry.Name}{Exit.Name}";
            }

            public Portal Reverse()
            {
                return new Portal(Exit, Entry, this.Inner) { Name = $"{Entry.Name}{Exit.Name}" };
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

            distance = -2;
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

            if (start.Name == 'Z' || target.Name == 'Z') distance--;
            return closedList.FirstOrDefault(l => l.X == target.X && l.Y == target.Y) != null;

            // end

            //Console.ReadLine();
        }

        static List<Location> GetWalkableAdjacentSquares(int x, int y, Location target, char[][] map)
        {
            var proposedLocations = new List<Location>();
            if (x - 1 >= 0) proposedLocations.Add(new Location(map[y][x - 1], x - 1, y));
            if (y - 1 >= 0) proposedLocations.Add(new Location(map[y - 1][x], x, y - 1));
            if (x + 1 < map[0].Length) proposedLocations.Add(new Location(map[y][x + 1], x + 1, y));
            if (y + 1 < map.Length) proposedLocations.Add(new Location(map[y + 1][x], x, y + 1));

            return proposedLocations.Where(l => (l.X == target.X && l.Y == target.Y) ||
                map[l.Y][l.X] == '.').ToList();
        }

        static int ComputeHScore(int x, int y, int targetX, int targetY)
        {
            return Math.Abs(targetX - x) + Math.Abs(targetY - y);
        }



        public static string Reverse(string s)
        {
            char[] charArray = s.ToCharArray();
            Array.Reverse(charArray);
            return new string(charArray);
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
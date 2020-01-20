using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.WebSockets;
using System.Numerics;
using System.Reflection.Emit;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text.RegularExpressions;

namespace AdventOfCode
{
    internal class Day12
    {
        static void Main(string[] args)
        {
            var fileText = File.ReadAllText(@"Datasets\day11.txt");
            var strCode = fileText.Split(",").ToArray();
            long count = 0;
            var energy = 0;
            var Moons = new List<Moon>
            {
                new Moon("Io", -8, -18, 6),
                new Moon("Europa", -11, -14, 4),
                new Moon("Ganymede", 8, -3, -10),
                new Moon("Callisto", -2, -16, 1),
                //new Moon("Io", -1, 0, 2),
                //new Moon("Europa", 2, -10, -7),
                //new Moon("Ganymede", 4, -8, 8),
                //new Moon("Callisto", 3, 5, -1),
            };

            Console.WriteLine("After 0 steps:");

            //foreach (var moon in Moons)
            //{
            //    Console.WriteLine(moon.ToString());
            //}

            energy = Moons.Select(x => x.CalculateEnergy()).Sum();
            var targetEnerg = energy;
            var stable = false;
            var position = 0;
            var counts = new List<long>();
            while (position != 3)
            //for (var x = 0; x < count; x++)
            {
                for (var i = 0; i < Moons.Count; i++)
                {
                    for (var j = 0; j < Moons.Count; j++)
                    {
                        if (i == j) continue;

                        if (Moons[i].Position.X < Moons[j].Position.X) Moons[i].Velocity.X += 1;
                        else if (Moons[i].Position.X > Moons[j].Position.X) Moons[i].Velocity.X -= 1;

                        if (Moons[i].Position.Y < Moons[j].Position.Y) Moons[i].Velocity.Y += 1;
                        else if (Moons[i].Position.Y > Moons[j].Position.Y) Moons[i].Velocity.Y -= 1;

                        if (Moons[i].Position.Z < Moons[j].Position.Z) Moons[i].Velocity.Z += 1;
                        else if (Moons[i].Position.Z > Moons[j].Position.Z) Moons[i].Velocity.Z -= 1;

                    }
                }

                ++count;
                //Console.WriteLine($"After {++count} steps:");

                //var stability = true;

                foreach (var moon in Moons)
                {
                    moon.ApplyGravity();
                    moon.UpdatePosition();
                    //Console.WriteLine(moon.ToString());
                }

                //energy = Moons.Select(x => x.CalculateEnergy()).Sum();

                //if (energy == targetEnerg)
                //{
                stable = true;
                foreach (var moon in Moons)
                {
                    stable &= moon.IsAPreviousState(position);
                    if (!stable) break;
                }

                if( stable) { 
                    position++;
                    counts.Add(count);
                    count = 0; 
                    Moons = new List<Moon>
                    {
                        new Moon("Io", -8, -18, 6),
                        new Moon("Europa", -11, -14, 4),
                        new Moon("Ganymede", 8, -3, -10),
                        new Moon("Callisto", -2, -16, 1),
                        //new Moon("Io", -1, 0, 2),
                        //new Moon("Europa", 2, -10, -7),
                        //new Moon("Ganymede", 4, -8, 8),
                        //new Moon("Callisto", 3, 5, -1),
                    };
                }
                //}

            }



            //List<List<Coord>> input = new List<List<Coord>>();

            //for (int i = 0; i < 3; i++)
            //    input.Add(new List<Coord>());
            //Regex re = new Regex(@"<x=(.+), y=(.+), z=(.+)>");
            //string line;
            //var n = 0;
            //var cont = true;
            //while (cont)
            //{
            //    line = Console.ReadLine();
            //    foreach (Match match in re.Matches(line))
            //    {
            //        int x = Int32.Parse(match.Groups[1].Value);
            //        int y = Int32.Parse(match.Groups[2].Value);
            //        int z = Int32.Parse(match.Groups[3].Value);
            //        input[0].Add(new Coord(x));
            //        input[1].Add(new Coord(y));
            //        input[2].Add(new Coord(z));
            //    }

            //    n++;
            //    if (n > 3) cont=false;
            //}

            //long period = 1;
            //foreach (List<Coord> coords in input)
            //{
            //    period = LCM(FindCycle(coords), period);
            //}
            //Console.WriteLine(period);
            count = counts.First();

                count = LCM(count, LCM ( counts[1], counts[2]));

                Console.WriteLine(count);
        }

        public static long GCD(long a, long b)
        {
            while (a != b)
            {
                if (a % b == 0) return b;
                if (b % a == 0) return a;
                if (a > b)
                    a -= b;
                if (b > a)
                    b -= a;
            }
            return a;
        }

        public static Int64 LCM(Int64 a, Int64 b)
        {
            return a * b / GCD(a, b);
        }

        public static Int64 FindCycle(List<Coord> coords)
        {
            List<Coord> initial = new List<Coord>();
            foreach (Coord c in coords) initial.Add(new Coord(c));

            Int64 step = 0;
            while (true)
            {
                Step(coords);
                step++;
                if (coordsEqual(initial, coords))
                    return step;
            }
        }

        public static bool coordsEqual(List<Coord> a, List<Coord> b)
        {
            for (int i = 0; i < a.Count; i++)
            {
                if (a[i].pos != b[i].pos) return false;
                if (a[i].v != b[i].v) return false;
            }
            return true;
        }

        public static void Step(List<Coord> coords)
        {
            for (int i = 0; i < coords.Count; i++)
            {
                Coord a = coords[i];
                for (int j = 0; j < coords.Count; j++)
                    if (i != j)
                    {
                        Coord b = coords[j];
                        a.v += sign(b.pos - a.pos);
                    }
            }
            foreach (Coord c in coords)
            {
                c.pos += c.v;
            }
        }

        public static int sign(int x)
        {
            return (x > 0) ? 1 : (x < 0) ? -1 : 0;
        }

        public static int abs(int x)
        {
            return (x >= 0) ? x : -x;
        }

        public class Coord
        {
            public int pos;
            public int v;

            public Coord(int pos)
            {
                this.pos = pos;
                v = 0;
            }

            public Coord(Coord c)
            {
                pos = c.pos;
                v = c.v;
            }
        }

        public class Moon
        {
            public string Name;

            public Vector InitialPosition;
            public Vector InitialVelocity;

            public List<Vector> PreviousVelocities = new List<Vector>();
            public List<int> PreviousEnergyStates = new List<int>();
            public List<int> PreviousKinEnergies = new List<int>();
            public List<int> PreviousPotEnergies = new List<int>();


            public Vector Position { get; }
            public Vector Velocity { get; set; }
            public Vector Gravity { get; set; }

            public Moon(string name, int x, int y, int z)
            {
                Name = name;
                Position = new Vector() { X = x, Y = y, Z = z };
                Velocity = new Vector() { X = 0, Y = 0, Z = 0 };
                Gravity = new Vector();

                InitialPosition = new Vector() { X = x, Y = y, Z = z };
                InitialVelocity = new Vector() { X = 0, Y = 0, Z = 0 };
            }

            public void ApplyGravity()
            {
                Velocity.X += Gravity.X;
                Velocity.Y += Gravity.Y;
                Velocity.Z += Gravity.Z;
            }

            public void UpdatePosition()
            {
                Position.X += Velocity.X;
                Position.Y += Velocity.Y;
                Position.Z += Velocity.Z;
            }

            public bool IsAPreviousState(int position)
            {
                var pos = InitialPosition;
                var vel = InitialVelocity;
                if (position == 0)
                {
                    return vel.X == Velocity.X && pos.X == Position.X;
                }
                if (position == 1)
                {
                    return vel.Y == Velocity.Y && pos.Y == Position.Y;
                }
                if (position == 2)
                {
                    return vel.Z == Velocity.Z && pos.Z == Position.Z;
                }

                return false;
                //return vel.X == Velocity.X && vel.Y == Velocity.Y && vel.Z == Velocity.Z && pos.X == Position.X &&
                //       pos.Y == Position.Y && pos.Z == Position.Z;
            }

            public int CalculateEnergy()
            {
                return Velocity.AbsoluteSum() * Position.AbsoluteSum();
            }

            public override string ToString()
            {
                return
                    $"pos=<x={Position.X}, y={Position.Y}, z={Position.Z}>, vel=<x={Velocity.X}, y={Velocity.Y}, z={Velocity.Z}>";
            }

        }

        public class Vector : ICloneable
        {
            public int X;
            public int Y;
            public int Z;

            public Vector()
            {
                X = 0;
                Y = 0;
                Z = 0;
            }

            public int AbsoluteSum()
            {
                return Math.Abs(X) + Math.Abs(Y) + Math.Abs(Z);
            }

            public object Clone()
            {
                return new Vector() { X = this.X, Y = this.Y, Z = this.Z };
            }
        }
    }
}

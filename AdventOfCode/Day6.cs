using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AdventOfCode
{
    internal class Day6
    {
        static void Main(string[] args)
        {
            var fileText = File.ReadAllLines(@"Datasets\day6.txt");
            var space = new Space();
            var result = PartOne(fileText);

            //foreach (var line in fileText)
            //{
            //    var objects = line.Split(")");
            //    var obj = objects.First();
            //    var satellite = objects.Last();
            //    space.AddSatellite(obj, satellite);
            //}

            var satelliteCount = space.SatellitesCount();
        }


        public class Space
        {
            public List<SpaceObject> SpaceObjects = new List<SpaceObject>();

            public void AddSatellite(string name, string satellite)
            {
                if (SpaceObjects.All(x => x.Name != name))
                {
                    SpaceObjects.Add(new SpaceObject(name));
                }

                if (SpaceObjects.All(x => x.Name != satellite))
                {
                    SpaceObjects.Add(new SpaceObject(satellite));
                }

                foreach (var obj in SpaceObjects)
                {
                    if (name == obj.Name)
                    {
                        obj.AddSatellite(satellite);
                    }

                    if (obj.Satellites.Any(x => x == name))
                    {
                        obj.AddSatellite(satellite);
                    }
                }
            }

            public int SatellitesCount()
            {
                var count = 0;
                foreach (var satellite in SpaceObjects)
                {
                    count += satellite.Satellites.Count();
                }
                return count;
            }
        }


        static Dictionary<string, string> ParseTree(string[] input) =>
              input
                  .Select(line => line.Split(")"))
                  .ToDictionary(
                      parent_child => parent_child[1],
                      parent_child => parent_child[0]
                  );

        static int PartOne(string[] input)
        {
            var childToParent = ParseTree(input);
            foreach (var node in childToParent.Keys)
            {
                var allAncestors = GetAncestors(childToParent, node);
            }
            return ( from node in childToParent.Keys select GetAncestors(childToParent, node).Count()
            ).Sum();
        }

        static int PartTwo(string[] input)
        {
            var childToParent = ParseTree(input);
            var ancestors1 = new Stack<string>(GetAncestors(childToParent, "YOU"));
            var ancestors2 = new Stack<string>(GetAncestors(childToParent, "SAN"));
            while (ancestors1.Peek() == ancestors2.Peek())
            {
                ancestors1.Pop();
                ancestors2.Pop();
            }
            return ancestors1.Count + ancestors2.Count;
        }

        static IEnumerable<string> GetAncestors(Dictionary<string, string> childToParent, string node)
        {
            var result = new List<string>();

            for (var parent = childToParent[node]; parent != null; parent = childToParent.GetValueOrDefault(parent, null))
            {
                result.Add(parent);
            }

            return result;
        }

        public class SpaceObject
        {
            public string Name;
            public List<string> Satellites = new List<string>();

            public SpaceObject(string name)
            {
                Name = name;
            }

            public void AddSatellite(string obj)
            {
                Satellites.Add(obj);
            }
        }
    }
}

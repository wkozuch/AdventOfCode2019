using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Transactions;

namespace AdventOfCode
{
    internal class Day8
    {
        static void Main(string[] args)
        {
            var fileText = File.ReadAllText(@"Datasets\day8.txt").ToArray().Select(x => int.Parse(x.ToString()))
                .ToArray();
            var height = 6;
            var width = 25;
            var layers = new List<Layer>();
            var counter = 0;
            for (var i = 0; i < fileText.Length; i += (width * height))
            {
                Console.WriteLine($"Layer #{++counter}:");
                var layer = new Layer();

                for (var h = 0; h < height; h++)
                {
                    var row = new List<int>();
                    for (var w = 0; w < width; w++)
                    {
                        var index = i + w + width * h;
                        row.Add(fileText[index]);
                    }

                    Console.WriteLine($"Row: {string.Join("", row)}");
                    layer.Rows.Add(row);
                }
                Console.WriteLine($"0s: {layer.Count(0)}, 1s: {layer.Count(1)}, 2s: {layer.Count(2)}, 1sx2s: {layer.Count(1) * layer.Count(2)}");
                layers.Add(layer);
            }

            var count = int.MaxValue;
            var operation = 0;
            var lowestLayer = new Layer();
            foreach (var layer in layers)
            {
                if (layer.Count(0) < count)
                {
                    operation = layer.Count(1) * layer.Count(2);
                    count = layer.Count(0);
                    lowestLayer = layer;
                }
            }

            Console.WriteLine($"Result: {lowestLayer.Count(1) * lowestLayer.Count(2)}");

            var result = new Layer();
            var layersCount = layers.Count();

            for (var h = 0; h < height; h++)
            {
                var row = new List<int>();
                for (var w = 0; w < width; w++)
                {
                    for (var i = 0; i < layersCount; i++)
                    {
                        if (layers[i].Rows[h][w] == 2) continue;
                        row.Add(layers[i].Rows[h][w]);
                        break;
                    }

                }
                Console.WriteLine($"{string.Join("", row)}");
                result.Rows.Add(row);
            }
        }

        public class Layer
        {
            public List<List<int>> Rows = new List<List<int>>();

            public int Count(int number)
            {
                var count = 0;
                foreach (var row in Rows)
                {
                    count += row.Count(x => x == number);
                }

                return count;
            }

        }

    }
}

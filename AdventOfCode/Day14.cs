//using System;
//using System.Collections;
//using System.Collections.Generic;
//using System.Collections.Specialized;
//using System.ComponentModel.DataAnnotations;
//using System.IO;
//using System.Linq;
//using System.Security.Cryptography.X509Certificates;
//using System.Threading;

//namespace AdventOfCode
//{
//    internal class Day14
//    {
//        static void Main(string[] args)
//        {
//            var fileText = File.ReadAllLines(@"Datasets\day14.txt");
//            var Reactions = new List<Reaction>();
//            var chemsToProduct = new Dictionary<string, Reaction>();
//            foreach (var line in fileText)
//            {
//                var chems = line.Split("=>")[0].Split(", ").ToList().Select(x => new Chemical() { Amount = int.Parse(x.Split(" ")[0]), Name = x.Split(" ")[1] }).ToList();

//                var outcome = line.Split("=> ").Last().Split(" ");
//                var product = new Chemical() { Amount = int.Parse(outcome[0]), Name = outcome[1] };
//                var reaction = new Reaction() { Chemicals = chems, Product = product };
//                chemsToProduct.Add(product.Name, reaction);
//                Reactions.Add(reaction);
//            }

//            //foreach (var product in chemsToProduct.Keys)
//            //{
//            Console.Write($"PRODUCT 1 FUEL == ");
//            var basicProducts = BasicProducts(chemsToProduct);
//            var result = GetAncestors(chemsToProduct, new Chemical() { Amount = 1, Name = "FUEL" });
//            var oreCount = 0;
//            foreach (var product in basicProducts)
//            {
//                var sum = result.Where(x => x.Name == product.Name).Sum(x => x.Amount);
//                var ceiling = Math.Ceiling(sum / (double)product.Amount);
//                var count = (int)(Math.Ceiling((sum / (double)product.Amount)) * product.OreAmount);
//                oreCount += count;
//                Console.Write($"{sum} {product.Name} ({count} ORE) ");
//            }

//            Console.Write($" == {oreCount} ORE");
//        }

//        static List<Chemical> GetAncestors(Dictionary<string, Reaction> chemsToProduct, Chemical product)
//        {
//            var result = new List<Chemical>();
//            var reaction = chemsToProduct[product.Name];

//            if (reaction.Chemicals.Any(x => x.Name != "ORE"))
//            {
//                reaction.Multiply(product.Amount);

//                foreach (var chem in reaction.Chemicals)
//                {
//                    //Console.Write($"{chem.Amount} {chem.Name} ({product.Amount}{product.Name})");
//                    result.AddRange(GetAncestors(chemsToProduct, chem));
//                }
//            }
//            else
//            {
//                var ore = reaction.Chemicals.Single();
//                var prod = reaction.Product;
//                result.Add(new Chemical() { Name = prod.Name, Amount = product.Amount });
//                //Console.Write($" + {amount} * {prod.Name} ({product.Amount}{product.Name})");
//            }

//            return result;
//        }

//        static Dictionary<string, double> GetTotalChemicals(Dictionary<string, Reaction> chemsToProduct, string productName)
//        {
//            var result = new Dictionary<string, double>();
//            var reaction = chemsToProduct[productName];


//            foreach (var chem in reaction.Chemicals)
//            {
//                result.Add(chem.Name, chem.Amount);
//            }

//            foreach (var key in result.Keys)
//            {
//                GetTotalChemicals(chemsToProduct, key);
//            }
//        }

//        static List<Chemical> BasicProducts(Dictionary<string, Reaction> chemsToProduct)
//        {
//            var result = new List<Chemical>();

//            foreach (var product in chemsToProduct.Keys)
//            {
//                var reaction = chemsToProduct[product];
//                if (reaction.Chemicals.Any(x => x.Name == "ORE"))
//                {
//                    var ore = reaction.Chemicals.Single();
//                    var prod = reaction.Product;
//                    result.Add(new Chemical() { Name = prod.Name, Amount = prod.Amount, OreAmount = ore.Amount });
//                }
//            }

//            return result;
//        }

//        public class Reaction
//        {
//            public List<Chemical> Chemicals = new List<Chemical>();
//            public Chemical Product;

//            public void Multiply(double amount)
//            {
//                for (var i = 0; i < Chemicals.Count; i++)
//                {
//                    Chemicals[i].Amount *= amount;
//                }

//                Product.Amount *= amount;
//            }
//        }

//        public class Chemical
//        {
//            public double OreAmount;
//            public double Amount;
//            public string Name;
//        }
//    }

//}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;


namespace AdventOfCode
{

    class Day14
    {
        static void Main(string[] args)
        {
            System.Console.WriteLine("Day 14");

            string filename = @"Datasets\day14.txt";
            Dictionary<string, Reaction> reaction = new Dictionary<string, Reaction>();
            reaction.Add("ORE", new Reaction("ORE"));

            using (StreamReader sr = new StreamReader(filename))
            {
                string[] data = sr.ReadToEnd().Trim().Split("\n");
                foreach (string line in data)
                {
                    string[] formula = line.Split("=>");

                    // result
                    string[] item = formula[1].Trim().Split(" ");
                    if (!reaction.ContainsKey(item[1]))
                        reaction.Add(item[1], new Reaction(item[1], Int64.Parse(item[0])));
                    Reaction result = reaction[item[1]];
                    result.Output = Int64.Parse(item[0]);

                    // sources
                    foreach (string s in formula[0].Split(","))
                    {
                        item = s.Trim().Split(" ");
                        result.AddSource(item[1], Int64.Parse(item[0]));
                        if (!reaction.ContainsKey(item[1]))
                            reaction.Add(item[1], new Reaction(item[1], 0));    // Skeleton reaction, to add product to
                        reaction[item[1]].AddProduct(result.Name, result.Output);
                    }
                }
            }

            // Part 1
            long requiredOre = GetRequiredOre(reaction);
            System.Console.WriteLine($"1. {requiredOre}");

            // Part 2
            long target = 1_000_000_000_000;
            long lower = (target / requiredOre) - 1_000;
            long higher = (target / requiredOre) + 1_000_000_000;
            while (lower < higher)
            {
                long mid = (lower + higher) / 2;
                long guess = GetRequiredOre(reaction, mid);
                if (guess > target)
                {
                    // System.Console.WriteLine($"MORE: {guess}");
                    higher = mid;
                }
                else if (guess < target)
                {
                    // System.Console.WriteLine($"LESS: {guess}");
                    if (mid == lower) break;
                    lower = mid;
                }
                else
                {
                    lower = mid;
                    break;
                }
            }
            System.Console.WriteLine($"2. {lower}");
        }

        private static long GetRequiredOre(Dictionary<string, Reaction> reaction, long fuelTarget = 1)
        {
            IEnumerable<string> ordered = new Topological(reaction).GetOrdered();
            Dictionary<string, long> quantity = new Dictionary<string, long>(ordered.Count());
            quantity["FUEL"] = fuelTarget;

            foreach (string item in ordered)
            {
                long output = reaction[item].Output;
                long needed = quantity[item];
                long toMake = (long)Math.Ceiling((decimal)needed / output);
                foreach (var dependency in reaction[item].GetDependencies())
                {
                    if (quantity.ContainsKey(dependency.Key))
                        quantity[dependency.Key] += dependency.Value * toMake;
                    else
                        quantity.Add(dependency.Key, dependency.Value * toMake);
                }
            }
            return quantity["ORE"];
        }
    }

    class Topological
    {
        private List<string> depthFirstOrder;
        private HashSet<string> marked;

        public Topological(Dictionary<string, Reaction> reaction)
        {
            depthFirstOrder = new List<string>(reaction.Count);
            marked = new HashSet<string>(reaction.Count);

            foreach (string item in reaction.Keys)
                if (!marked.Contains(item))
                    DepthFirstSearch(reaction, item);

            // depthFirstOrder.Reverse();
        }

        private void DepthFirstSearch(Dictionary<string, Reaction> reaction, string start)
        {
            marked.Add(start);

            foreach (var item in reaction[start].GetProducts())
                if (!marked.Contains(item.Key))
                    DepthFirstSearch(reaction, item.Key);

            depthFirstOrder.Add(start);
        }

        public IEnumerable<string> GetOrdered() => depthFirstOrder;
    }

    class Reaction
    {
        public string Name { get; }
        public long Output { get; set; }
        private Dictionary<string, long> input = new Dictionary<string, long>();       // Chemicals that go into Name
        private Dictionary<string, long> product = new Dictionary<string, long>();    // Chemicals that require Name

        public Reaction(string name, long output = 1)
        {
            this.Name = name;
            this.Output = output;
        }

        public void AddSource(string name, long quantity)
        {
            input.Add(name, quantity);
        }

        public void AddProduct(string name, long quantity)
        {
            product.Add(name, quantity);
        }

        public IEnumerable<KeyValuePair<string, long>> GetDependencies() => input;

        public IEnumerable<KeyValuePair<string, long>> GetProducts() => product;
    }

    class Ore : Reaction
    {
        public Ore() : base("Ore") { }
    }
}


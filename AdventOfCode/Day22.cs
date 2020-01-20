using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Threading;

namespace AdventOfCode
{
    internal class Day22
    {
        static void Main(string[] args)
        {
            var fileText = File.ReadAllLines(@"Datasets\day22.txt");
            var newSize = 119315717514047;
            var newIterations = 101741582076661;
            var it = newIterations % ((newSize - 1));
            var size = 10007;
            var deck = Enumerable.Range(0, size).ToList();
            var initialDeck = Enumerable.Range(0, size).ToList();
            var j = 0;
            Console.WriteLine($"{j} {deck.IndexOf(2019)}");
            while (true)
            {
                //for (int i = 0; i < iterations; i++)
                //{
                foreach (var line in fileText)
                {
                    var arguments = line.Split(" ");
                    var parameter = arguments.Last();
                    if (arguments.Contains("cut"))
                    {
                        deck = CutNCards(deck, int.Parse(parameter));
                    }

                    if (arguments.Contains("increment"))
                    {
                        deck = DealWithIncrement(deck, int.Parse(parameter));
                    }

                    if (arguments.Contains("stack"))
                    {
                        deck = DealIntoNewStack(deck);
                    }

                    j++;
                    Console.WriteLine($"{j} {deck.IndexOf(2019)}");
                    //if (Equality(initialDeck, deck, 2020))
                    //{
                    //    Console.WriteLine($"{j} {deck[2020]}");
                    //}

                    if (Equality(initialDeck, deck))
                        break;

                }
                // Print(deck);


                //}
            }


        }

        static List<int> DealIntoNewStack(List<int> deck)
        {
            deck.Reverse();
            return deck;
        }

        private static List<int> CutNCards(List<int> deck, int n)
        {
            var take = n > 0 ? n : deck.Count - Math.Abs(n);
            var skip = n > 0 ? deck.Count - n : Math.Abs(n);
            var top = deck.Take(take).ToList();
            var bottom = deck.Skip(take).Take(skip).ToList();
            bottom.AddRange(top);
            return bottom;
        }

        private static List<int> DealWithIncrement(List<int> deck, int n)
        {
            var suffled = new int[deck.Count];
            for (int i = 0; i < deck.Count; i++)
            {
                suffled[i * n % deck.Count] = deck[i];
            }

            return suffled.ToList();
        }

        private static void Print(List<int> deck)
        {
            Console.WriteLine(string.Join(" ", deck));
        }

        private static bool Equality(List<int> startingDeck, List<int> deck)
        {
            for (var i = 0; i < startingDeck.Count; i++)
            {
                if (startingDeck[i] != deck[i]) return false;
            }

            return true;
        }

        private static bool Equality(List<int> startingDeck, List<int> deck, int i)
        {
            return startingDeck[i] == deck[i];
        }
    }

}
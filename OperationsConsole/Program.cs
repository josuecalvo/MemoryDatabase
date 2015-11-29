using JosueCalvo.Toolkit.MemoryStructures;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace OperationsConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            var chronos = new Stopwatch();

            var list = new List<string>();

            chronos.Start();

            for (var i = 0; i < 1000000; i++)
            {
                list.Add(i.ToString());
            }
            Console.WriteLine($"Inserted in {chronos.ElapsedMilliseconds}");
            list.Sort();
            chronos.Stop();
            Console.WriteLine($"Sorted in {chronos.ElapsedMilliseconds}");

            chronos.Reset();
            var index = new Index<bool>(new CharSet());

            chronos.Start();

            for (var i = 0; i < 1000000; i++)
            {
                if (i != 0 && i % 100000 == 0)
                    Console.WriteLine($"{i} in {chronos.ElapsedMilliseconds}");

                index.AddKey(i.ToString(), false);
            }
            chronos.Stop();
            Console.WriteLine($"Inserted in {chronos.ElapsedMilliseconds}");

            chronos.Reset();
            var sortedDictionary = new SortedDictionary<string, bool>();

            chronos.Start();
            for (var i = 0; i < 1000000; i++)
            {
                if (i != 0 && i % 100000 == 0)
                    Console.WriteLine($"{i} in {chronos.ElapsedMilliseconds}");

                sortedDictionary.Add(i.ToString(), false);
            }

            chronos.Stop();
            Console.WriteLine($"Inserted in {chronos.ElapsedMilliseconds}");

            string lastKey = null;
            foreach (var item in index)
            {
                //Console.WriteLine(item.Key);
                if (item.Key.CompareTo(lastKey) < 0)
                {
                    throw new Exception("The keys are not sorted!");
                }
                lastKey = item.Key;
            }

            Console.WriteLine("Press any key...");
            Console.ReadKey();
        }
    }
}

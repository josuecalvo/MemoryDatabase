using JosueCalvo.Toolkit.MemoryStructures;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace OperationsConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            var index = new Index<string>(" .01234567890abcdefghijklmnopqrstuvwxyz");

            var chronos = new Stopwatch();
            chronos.Start();
            for (var i = 0; i < 1000000; i++)
            {
                if (i != 0 && i % 100000 == 0)
                    Console.WriteLine($"{i} in {chronos.ElapsedMilliseconds}");

                index.AddKey(i.ToString(), $"Value{i}");
            }
            Console.WriteLine($"Inserted in {chronos.ElapsedMilliseconds}");

            var all = index.GetAll();

            chronos.Stop();
            Console.WriteLine($"Enumerated in {chronos.ElapsedMilliseconds}");

            var sortedDictionary = new SortedDictionary<string, string>();
            chronos.Reset();
            chronos.Start();
            for (var i = 0; i < 1000000; i++)
            {
                if (i != 0 && i % 100000 == 0)
                    Console.WriteLine($"{i} in {chronos.ElapsedMilliseconds}");

                sortedDictionary.Add(i.ToString(), $"Value{i}");
            }

            chronos.Stop();
            Console.WriteLine($"Inserted in {chronos.ElapsedMilliseconds}");

            Console.WriteLine("Press any key...");
            Console.ReadKey();
        }
    }
}

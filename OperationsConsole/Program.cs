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
            var index = new Index<string>(new CharSet());

            var chronos = new Stopwatch();
            chronos.Start();

            for (var i = 0; i < 1000000; i++)
            {
                if (i != 0 && i % 100000 == 0)
                    Console.WriteLine($"{i} in {chronos.ElapsedMilliseconds}");

                index.AddKey(i.ToString(), $"Value{i}");
            }
            chronos.Stop();
            Console.WriteLine($"Inserted in {chronos.ElapsedMilliseconds}");

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

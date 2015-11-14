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
            var index = new Index<string>(" .01234567890abcdefghijklmnopqrstuvwxyz");

            var chronos = new Stopwatch();
            chronos.Start();
            for (var i = 0; i < 100000; i++)
            {
                index.AddKey(i.ToString(), $"Value{i}");
            }
            chronos.Stop();
            Console.WriteLine($"Inserted in {chronos.ElapsedMilliseconds}");

            var sortedList = new SortedDictionary<string, string>();
            chronos.Reset();
            chronos.Start();
            for (var i = 0; i < 100000; i++)
            {
                sortedList.Add(i.ToString(), $"Value{i}");
            }
            chronos.Stop();
            Console.WriteLine($"Inserted in {chronos.ElapsedMilliseconds}");


            Console.WriteLine("Press any key...");
            Console.ReadKey();
        }
    }
}

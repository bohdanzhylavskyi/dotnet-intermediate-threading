/*
 * 5. Write a program which creates two threads and a shared collection:
 * the first one should add 10 elements into the collection and the second should print all elements
 * in the collection after each adding.
 * Use Thread, ThreadPool or Task classes for thread creation and any kind of synchronization constructions.
 */
using System;
using System.Collections.Generic;
using System.Threading;

namespace MultiThreading.Task5.Threads.SharedCollection
{
    class Program
    {
        static AutoResetEvent PrintingDoneSignal = new(true);
        static AutoResetEvent ElementAddedSignal = new(false);

        static void Main(string[] args)
        {
            Console.WriteLine("5. Write a program which creates two threads and a shared collection:");
            Console.WriteLine("the first one should add 10 elements into the collection and the second should print all elements in the collection after each adding.");
            Console.WriteLine("Use Thread, ThreadPool or Task classes for thread creation and any kind of synchronization constructions.");
            Console.WriteLine();

            var sharedCollection = new List<int>();
            var printThreadCts = new CancellationTokenSource();

            var addingThread = CreateAddingThread(sharedCollection);
            var printingThread = CreatePrintingThread(sharedCollection, printThreadCts.Token);

            addingThread.Start();
            printingThread.Start();

            addingThread.Join();
            printThreadCts.Cancel();
        }

        static Thread CreateAddingThread(List<int> collection)
        {
            var thread = new Thread(() =>
            {
                for (var i = 0; i < 10; i++)
                {
                    PrintingDoneSignal.WaitOne();

                    collection.Add(i + 1);

                    ElementAddedSignal.Set();
                }
            });

            return thread;
        }

        static Thread CreatePrintingThread(List<int> collection, CancellationToken ct)
        {
            ThreadStart threadCb = () =>
            {
                while (true)
                {
                    WaitHandle.WaitAny([ElementAddedSignal, ct.WaitHandle]);

                    if (ct.IsCancellationRequested)
                    {
                        break;
                    }


                    PrintCollection(collection);

                    PrintingDoneSignal.Set();
                }
            };

            var thread = new Thread(threadCb);

            return thread;
        }

        static void PrintCollection(List<int> collection)
        {
            Console.WriteLine("[ " + string.Join(", ", collection.ToArray()) + " ]");
        }
    }
}
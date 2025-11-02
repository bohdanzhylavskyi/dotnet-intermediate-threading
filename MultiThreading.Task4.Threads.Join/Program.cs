/*
 * 4.	Write a program which recursively creates 10 threads.
 * Each thread should be with the same body and receive a state with integer number, decrement it,
 * print and pass as a state into the newly created thread.
 * Use Thread class for this task and Join for waiting threads.
 * 
 * Implement all of the following options:
 * - a) Use Thread class for this task and Join for waiting threads.
 * - b) ThreadPool class for this task and Semaphore for waiting threads.
 */

using System;
using System.Threading;

namespace MultiThreading.Task4.Threads.Join
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("4.	Write a program which recursively creates 10 threads.");
            Console.WriteLine("Each thread should be with the same body and receive a state with integer number, decrement it, print and pass as a state into the newly created thread.");
            Console.WriteLine("Implement all of the following options:");
            Console.WriteLine();
            Console.WriteLine("- a) Use Thread class for this task and Join for waiting threads.");
            Console.WriteLine("- b) ThreadPool class for this task and Semaphore for waiting threads.");

            var threadsCount = 10;


            Console.WriteLine("\n\n");

            Console.WriteLine("Executing threads with 'Join'");
            RunThreadsWithJoin(threadsCount);

            Console.WriteLine("\n\n===========================================\n\n");


            Console.WriteLine("Executing threads with semaphore");
            RunThreadsWithSemaphore(threadsCount);

            Console.WriteLine("\n\n");
        }

        static void RunThreadsWithJoin(int maxIterations)
        {
            RunThread(iteration: maxIterations).Join();
        }

        static void RunThreadsWithSemaphore(int iteration)
        {
            using var semaphore = new SemaphoreSlim(0, 1);

            RunThreadInPool(
                iteration,
                signalCompletion: () => semaphore.Release()
            );

            semaphore.Wait();
        }

        static Thread RunThread(int iteration)
        {
            ParameterizedThreadStart threadCb = (Object state) =>
            {
                Console.WriteLine($"Thread #{iteration}");

                if (iteration > 1)
                {
                    RunThread(iteration - 1).Join();
                }
            };

            var thread = new Thread(threadCb);

            thread.Start();

            return thread;
        }

        static void RunThreadInPool(int iteration, Action signalCompletion)
        {
            ThreadPool.QueueUserWorkItem((object _) =>
            {
                Console.WriteLine($"Thread #{iteration}");

                if (iteration > 1)
                {
                    var semaphore = new SemaphoreSlim(0, 1);

                    RunThreadInPool(
                        iteration - 1,
                        signalCompletion: () => semaphore.Release()
                    );

                    semaphore.Wait();
                }

                signalCompletion();
            });
        }
    }
}

/*
 * 2.	Write a program, which creates a chain of four Tasks.
 * First Task – creates an array of 10 random integer.
 * Second Task – multiplies this array with another random integer.
 * Third Task – sorts this array by ascending.
 * Fourth Task – calculates the average value. All this tasks should print the values to console.
 */
using System;
using System.Linq;
using System.Threading.Tasks;

namespace MultiThreading.Task2.Chaining
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine(".Net Mentoring Program. MultiThreading V1 ");
            Console.WriteLine("2.	Write a program, which creates a chain of four Tasks.");
            Console.WriteLine("First Task – creates an array of 10 random integer.");
            Console.WriteLine("Second Task – multiplies this array with another random integer.");
            Console.WriteLine("Third Task – sorts this array by ascending.");
            Console.WriteLine("Fourth Task – calculates the average value. All this tasks should print the values to console");
            Console.WriteLine();

            Create().ContinueWith((t) => Multiply(t.Result)).Unwrap()
                .ContinueWith((t) => Sort(t.Result)).Unwrap()
                .ContinueWith((t) => Average(t.Result)).Unwrap()
                .ContinueWith((t) => Console.WriteLine("Done"));

            Console.ReadLine();
        }

        static Task<int[]> Create()
        {
            return Task.Run(() =>
            {
                var result = new int[10];

                for (var i = 0; i < result.Length; i++)
                {
                    result[i] = new Random().Next(1, 100);
                }

                PrintTaskResult(nameof(Create), ArrayToString(result));

                return result;
            });
        }

        static Task<int[]> Multiply(int[] input)
        {
            return Task.Run(() =>
            {
                var result = new int[input.Length];
                var multipliers = new int[input.Length];
                var random = new Random();

                for (var i = 0; i < result.Length; i++)
                {
                    var multiplier = random.Next(1, 100);

                    result[i] = input[i] * multiplier;
                    multipliers[i] = multiplier;
                }


                PrintTaskResult(nameof(Multiply), ArrayToString(result), $"Multipliers: {ArrayToString(multipliers)}");

                return result;
            });
        }

        static Task<int[]> Sort(int[] input)
        {
            return Task.Run(() =>
            {
                var result = input.OrderByDescending(x => x).ToArray();

                PrintTaskResult(nameof(Sort), ArrayToString(result));

                return result;
            });
        }

        static Task<double> Average(int[] input)
        {
            return Task.Run(() =>
            {
                var result = input.Average();

                PrintTaskResult(nameof(Average), result.ToString());

                return result;
            });
        }

        static void PrintTaskResult(string taskName, string result, string additionalText = "")
        {
            Console.WriteLine($"'{taskName}' task result: {result}. {additionalText ?? ""}");
        }

        static string ArrayToString(int[] array)
        {
            return "[" + string.Join(", ", array) + "]";
        }
    }
}

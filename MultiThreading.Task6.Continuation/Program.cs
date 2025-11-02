/*
*  Create a Task and attach continuations to it according to the following criteria:
   a.    Continuation task should be executed regardless of the result of the parent task.
   b.    Continuation task should be executed when the parent task finished without success.
   c.    Continuation task should be executed when the parent task would be finished with fail and parent task thread should be reused for continuation
   d.    Continuation task should be executed outside of the thread pool when the parent task would be cancelled
   Demonstrate the work of the each case with console utility.
*/
using System;
using System.Threading;
using System.Threading.Tasks;

namespace MultiThreading.Task6.Continuation
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Create a Task and attach continuations to it according to the following criteria:");
            Console.WriteLine("a.    Continuation task should be executed regardless of the result of the parent task.");
            Console.WriteLine("b.    Continuation task should be executed when the parent task finished without success.");
            Console.WriteLine("c.    Continuation task should be executed when the parent task would be finished with fail and parent task thread should be reused for continuation.");
            Console.WriteLine("d.    Continuation task should be executed outside of the thread pool when the parent task would be cancelled.");
            Console.WriteLine("Demonstrate the work of the each case with console utility.");
            Console.WriteLine();

            A();
            B();
            C();
            D();
        }

        static void A()
        {
            PrintTaskDefinition("Continuation task should be executed regardless of the result of the parent task.");

            var taskSuccess = CreateSucceededTask();
            var taskFaulted = CreateFaultedTask();
            var taskCancelled = CreateCancelledTask();
            var taskContinuationOptions = TaskContinuationOptions.None;

            var continuationAction = CreateContinuationAction(taskContinuationOptions);

            var succeededContinuationTask = taskSuccess.ContinueWith(
                continuationAction,
                taskContinuationOptions
            );

            var faultedContinuationTask = taskFaulted.ContinueWith(
                continuationAction,
                taskContinuationOptions
            );

            var cancelledContinuationTask = taskCancelled.ContinueWith(
                continuationAction,
                taskContinuationOptions
            );

            succeededContinuationTask.Wait();
            faultedContinuationTask.Wait();
            cancelledContinuationTask.Wait();
        }

        static void B()
        {
            PrintTaskDefinition("Continuation task should be executed when the parent task finished without success.");

            var taskFaulted = CreateFaultedTask();
            var taskCancelled = CreateCancelledTask();
            var taskContinuationOptions = TaskContinuationOptions.NotOnRanToCompletion;

            var continuationAction = CreateContinuationAction(taskContinuationOptions);

            var faultedContinuationTask = taskFaulted.ContinueWith(
                continuationAction,
                taskContinuationOptions
            );

            var cancelledContinuationTask = taskCancelled.ContinueWith(
                continuationAction,
                taskContinuationOptions
            );

            faultedContinuationTask.Wait();
            cancelledContinuationTask.Wait();
        }

        static void C()
        {
            PrintTaskDefinition("Continuation task should be executed when the parent task would be finished with fail and parent task thread should be reused for continuation.");

            var taskFaulted = CreateFaultedTask();
            var taskContinuationOptions = TaskContinuationOptions.NotOnRanToCompletion
                | TaskContinuationOptions.ExecuteSynchronously;

            var continuationAction = CreateContinuationAction(taskContinuationOptions);

            var faultedContinuationTask = taskFaulted.ContinueWith(
                continuationAction,
                taskContinuationOptions
            );

            faultedContinuationTask.Wait();
        }

        static void D()
        {
            PrintTaskDefinition("Continuation task should be executed outside of the thread pool when the parent task would be cancelled.");

            var taskCancelled = CreateCancelledTask();
            var taskContinuationOptions = TaskContinuationOptions.OnlyOnCanceled | TaskContinuationOptions.LongRunning;

            var continuationAction = CreateContinuationAction(taskContinuationOptions);

            var cancelledContinuationTask = taskCancelled.ContinueWith(
                continuationAction,
                taskContinuationOptions
            );

            cancelledContinuationTask.Wait();
        }

        static Action<Task> CreateContinuationAction(TaskContinuationOptions continuationOptions)
        {
            return (Task t) =>
            {
                var status = t.Status;

                Console.WriteLine($"Continuation for task with status='{status}' has been invoked. TaskContinuationOptions: {continuationOptions}");
            };
        }

        static Task CreateSucceededTask()
        {
            var task = Task.Run(() =>
            {
                Console.WriteLine("The task that will succeed has been started...");
                Thread.Sleep(2000);
            });

            return task;
        }

        static Task CreateFaultedTask()
        {
            var task = Task.Run(() =>
            {
                Console.WriteLine("The task that will fail has been started...");
                Thread.Sleep(2000);

                throw new InvalidOperationException();
            });

            return task;
        }

        static Task CreateCancelledTask()
        {
            var cts = new CancellationTokenSource();
            var task = Task.Run(() =>
            {
                Console.WriteLine("The task that will be cancelled has been started...");
                Thread.Sleep(100);
                cts.Token.ThrowIfCancellationRequested();
            }, cts.Token);

            cts.CancelAfter(0);

            return task;
        }

        static void PrintTaskDefinition(string task)
        {
            Console.WriteLine("\n\n\n------------------------------------------------------------------------------");
            Console.WriteLine(task);
            Console.WriteLine("------------------------------------------------------------------------------\n");
        }
    }
}

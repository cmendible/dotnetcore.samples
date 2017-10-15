using System;
using System.Threading;
using System.Threading.Tasks;

namespace docker.controlc
{
    class Program
    {
        // AutoResetEvent to signal when to exit the application.
        private static readonly AutoResetEvent waitHandle = new AutoResetEvent(false);

        static void Main(string[] args)
        {
            // Fire and forget
            Task.Run(() =>
            {
                var random = new Random(10);
                while (true)
                {
                    // Write here whatever your side car applications needs to do.
                    // In this sample we are just writing a random number to the Console (stdout)
                    Console.WriteLine($"Loop = {random.Next()}");

                    // Sleep as long as you need.
                    Thread.Sleep(1000);
                }
            });

            // Handle Control+C or Control+Break
            Console.CancelKeyPress += (o, e) =>
            {
                Console.WriteLine("Exit");

                // Allow the manin thread to continue and exit...
                waitHandle.Set();
            };

            // Wait
            waitHandle.WaitOne();
        }
    }
}

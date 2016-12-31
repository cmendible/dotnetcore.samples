namespace Retry.Sample
{
    using System;
    using Polly;

    class Program
    {
        static void Main(string[] args)
        {
            var service = new UnreliableService();

            var breaker = Policy
                .Handle<TimeoutException>()
                .Retry(2);

            while (true)
            {
                var result = breaker.ExecuteAndCapture(
                    () => service.GetTime()
                );

                if (result.Outcome == OutcomeType.Failure)
                {
                    Console.ForegroundColor = ConsoleColor.DarkRed;
                    Console.WriteLine("Operation failed!!!"); 
                    Console.ForegroundColor = ConsoleColor.White; 
                }
            }
        }
    }
}
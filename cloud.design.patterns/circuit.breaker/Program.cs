namespace CircuitBreaker.Sample
{
    using System;
    using System.Threading;
    using Polly;

    class Program
    {
        static void Main(string[] args)
        {
            var service = new UnreliableService();

            var breaker = Policy
                .Handle<TimeoutException>()
                .CircuitBreaker(
                    2, // Number of consecutive exceptions before opening the circuit
                    TimeSpan.FromSeconds(20), // How long the circuit will be open
                    (exception, timespan) => 
                    { 
                        Console.ForegroundColor = ConsoleColor.DarkRed;
                        Console.WriteLine("Operation failed!!!"); 
                        Console.ForegroundColor = ConsoleColor.White;
                    },
                    () =>
                    {
                        Console.ForegroundColor = ConsoleColor.DarkGreen;
                        Console.WriteLine($"Circuit is Closed!!!");
                        Thread.Sleep(2000);
                        Console.ForegroundColor = ConsoleColor.White;
                    },
                    () =>
                    {
                        Console.ForegroundColor = ConsoleColor.DarkYellow;
                        Console.WriteLine($"Circuit is Half Open!!!");
                        Thread.Sleep(2000);
                        Console.ForegroundColor = ConsoleColor.White;
                    });

            while (true)
            {
                var result = breaker.ExecuteAndCapture(
                    () => service.GetTime()
                );
            }

            /*
            // Monitor the circuit state, for example for health reporting.
            CircuitState state = breaker.CircuitState;

            CircuitState.Closed - Normal operation. Execution of actions allowed.
            CircuitState.Open - The automated controller has opened the circuit. Execution of actions blocked.
            CircuitState.HalfOpen - Recovering from open state, after the automated break duration has expired. Execution of actions permitted. Success of subsequent action/s controls onward transition to Open or Closed state.
            CircuitState.Isolated - Circuit held manually in an open state. Execution of actions blocked.

            Manually open (and hold open) a circuit breaker - for example to manually isolate a downstream service.
            breaker.Isolate();

            Reset the breaker to closed state, to start accepting actions again.
            breaker.Reset(); */
        }
    }
}
namespace CircuitBreaker.Sample
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
                .CircuitBreaker(
                    2, // Number of consecutive exceptions before opening the circuit
                    TimeSpan.FromMinutes(1), // How long the circuit will be open
                    (exception, timespan) => { Console.WriteLine("Operation failed!!!"); },
                    () => { Console.WriteLine($"Circuit is Closed!!!"); },
                    () => { Console.WriteLine($"Circuit is Half Open!!!"); });

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
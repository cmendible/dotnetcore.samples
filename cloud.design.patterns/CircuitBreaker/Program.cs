namespace CircuitBreaker.Sample
{
    using System;

    class Program
    {
        static void Main(string[] args)
        {
            var service = new UnreliableService();
            var circuitBreaker = new CircuitBreaker(typeof(UnreliableService).FullName, 1000);

            while (true)
            {
                circuitBreaker.Invoke(
                   // This is the operation we want to execute.
                   () => service.GetTime(),
                   // Circuit is open we should take another path in our aplication.
                   (circuitBreakerOpenException) => Console.WriteLine("Circuit is Open!!!"),
                   // Other exception was thown. Retry?
                   (exception) => Console.WriteLine("Operation failed!!!")
                   );
            }
        }
    }
}
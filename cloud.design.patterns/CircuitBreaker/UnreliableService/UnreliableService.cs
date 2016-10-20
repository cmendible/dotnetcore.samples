namespace CircuitBreaker.Sample
{
    using System;
    using System.Threading;

    class UnreliableService
    {
        static readonly Random Random = new Random();
        public DateTime GetTime()
        {
            if (Random.Next(10) == 1)
            {
                Console.WriteLine("UnreliableService is unreliable");
                Thread.Sleep(500);
                throw new TimeoutException();
            }

            Console.WriteLine("UnreliableService is working");
            return DateTime.Now;
        }
    }
}

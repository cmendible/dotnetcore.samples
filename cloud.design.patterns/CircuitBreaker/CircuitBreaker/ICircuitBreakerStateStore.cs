namespace CircuitBreaker.Sample
{
    using System;

    // reference: http://msdn.microsoft.com/en-us/library/dn589784.aspx
    public interface ICircuitBreakerStateStore
    {
        string Name { get; } // set by a factory, depends on what Type is fed to the breaker ctor
        CircuitBreakerStateEnum State { get; } // closed = green light, open = red light, half-open = yellow light
        Exception LastException { get; } // set when the stateStore trips (breaker state changes)
        DateTime? LastStateChangeDateUtc { get; } // same as lastException
        void Trip(Exception ex); // executed when exceptions occur; the breaker eats all exceptions
        void Reset(); // executed after a half-open breaker doesn't hit more exceptions
        void HalfOpen(); // executed after a breaker has been open for 30s w/o exceptions
        bool IsClosed { get; } // check the stateStore to see if the break is closed
    }
}

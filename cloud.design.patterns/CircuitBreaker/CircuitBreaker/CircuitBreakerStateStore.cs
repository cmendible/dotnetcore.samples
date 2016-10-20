namespace CircuitBreaker.Sample
{
    using System;
    using System.Collections.Concurrent;

    public class CircuitBreakerStateStore : ICircuitBreakerStateStore
    {
        private ConcurrentStack<Exception> exceptionsSinceLastStateChange;

        public CircuitBreakerStateStore(string key)
        {
            this.exceptionsSinceLastStateChange = new ConcurrentStack<Exception>();
            this.Name = key;
        }

        public string Name { get; private set; }

        private CircuitBreakerStateEnum state;
        public CircuitBreakerStateEnum State
        {
            get
            {
                if (this.state.Equals(CircuitBreakerStateEnum.None))
                {
                    this.state = CircuitBreakerStateEnum.Closed;
                }
                return this.state;
            }
            private set
            {
                this.state = value;
            }
        }

        public Exception LastException
        {
            get
            {
                Exception lastException = null;
                exceptionsSinceLastStateChange.TryPeek(out lastException);
                return lastException;
            }
        }

        private DateTime? lastStateChangeDateUtc;
        public DateTime? LastStateChangeDateUtc
        {
            get
            {
                return this.lastStateChangeDateUtc;
            }
            private set
            {
                this.lastStateChangeDateUtc = value;
            }
        }

        public void Trip(Exception ex)
        {
            this.ChangeState(CircuitBreakerStateEnum.Open);
            exceptionsSinceLastStateChange.Push(ex);
        }

        public void Reset()
        {
            this.ChangeState(CircuitBreakerStateEnum.Closed);
            exceptionsSinceLastStateChange.Clear();
        }

        public void HalfOpen()
        {
            this.ChangeState(CircuitBreakerStateEnum.HalfOpen);
        }

        public bool IsClosed
        {
            get
            {
                return this.State.Equals(CircuitBreakerStateEnum.Closed);
            }
        }

        public void ChangeState(CircuitBreakerStateEnum state)
        {
            this.State = state;
            this.LastStateChangeDateUtc = DateTime.UtcNow;
        }
    }
}

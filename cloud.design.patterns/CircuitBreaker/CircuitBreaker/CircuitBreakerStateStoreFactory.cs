namespace CircuitBreaker.Sample
{
    using System.Collections.Concurrent;

    public class CircuitBreakerStateStoreFactory
    {
        private static ConcurrentDictionary<string, ICircuitBreakerStateStore> stateStores = new ConcurrentDictionary<string, ICircuitBreakerStateStore>();

        internal static ICircuitBreakerStateStore GetCircuitBreakerStateStore(string key)
        {
            // There is only one type of ICircuitBreakerStateStore to return...
            // The ConcurrentDictionary keeps track of ICircuitBreakerStateStore objects (across threads)
            // For example, a store for a db connection, web service client, and NAS storage could exist

            if (!stateStores.ContainsKey(key))
            {
                stateStores.TryAdd(key, new CircuitBreakerStateStore(key));
            }

            return stateStores[key];
        }
    }
}

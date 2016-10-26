namespace CacheAside
{
    using StackExchange.Redis;
    using System;

    public class CacheAsideRepository<T> where T : class, IEntity
    {
        private IDatabase cache;
        private IStore<T> store;

        public CacheAsideRepository(IStore<T> entityStore, IDatabase cacheClient)
        {
            store = entityStore;
            cache = cacheClient;
        }

        public T GetById(int id)
        {
            var key = this.GetCacheKey(id);
            var expiration = TimeSpan.FromMinutes(3);
            bool cacheException = false;

            try
            {
                // Try to get the entity from the cache.
                string cacheItem = cache.StringGet(key);
                if (cacheItem != null)
                {
                    Console.WriteLine($"Entity {id} fetched from cache");
                    return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(cacheItem);
                }
            }
            catch (Exception)
            {
                // If there is a cache related issue, raise an exception 
                // and avoid using the cache for the rest of the call.
                cacheException = true;
            }

            // If there is a cache miss, get the entity from the original store and cache it.
            // Code has been omitted because it is data store dependent.  
            var entity = this.store.GetById(id);

            if (!cacheException)
            {
                try
                {
                    // Avoid caching a null value.
                    if (entity != null)
                    {
                        // Put the item in the cache with a custom expiration time that 
                        // depends on how critical it might be to have stale data.
                        cache.StringSet(key, Newtonsoft.Json.JsonConvert.SerializeObject(entity), expiration);
                        Console.WriteLine($"Entity {id} loaded in to cache");
                    }
                }
                catch (Exception)
                {
                    // If there is a cache related issue, ignore it
                    // and just return the entity.
                }
            }

            return entity;
        }

        public void Update(T entity)
        {
            // Update the object in the original data store
            this.store.Update(entity);

            // Get the correct key for the cached object.
            var key = this.GetCacheKey(entity.Id);

            // Then, invalidate the current cache object
            this.cache.KeyDelete(key);
            Console.WriteLine($"Entity {entity.Id} was removed form the cache");
        }

        private string GetCacheKey(int objectId)
        {
            return $"{typeof(T).FullName}_{objectId}";
        }
    }
}
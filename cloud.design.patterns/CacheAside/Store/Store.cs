namespace CacheAside
{
    using System.Collections.Generic;

    public class Store<T> : IStore<T> where T : class, IEntity
    {
        private IDictionary<int, T> store = new Dictionary<int, T>();

        public void Add(T entity)
        {
            store[entity.Id] = entity;
        }

        public T GetById(int id)
        {
            return store[id];
        }

        public void Update(T entity)
        {
            store[entity.Id] = entity;
        }
    }
}

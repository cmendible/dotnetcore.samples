namespace CacheAside
{
    public interface IStore<T> where T : class, IEntity
    {
        void Add(T entity);
        T GetById(int id);
        void Update(T entity);
    }
}

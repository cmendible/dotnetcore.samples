namespace CacheAside
{
    using Microsoft.Extensions.Configuration;
    using StackExchange.Redis;
    
    public class Program
    {
        public static void Main(string[] args)
        {
            // Get the configuration
            var configuration = BuildConfiguration();

            var store = new Store<MyEntity>();
            store.Add(new MyEntity() { Id = 1 });

            string cacheConnectionString = configuration["redisConnectionString"];

            ConnectionMultiplexer connection = ConnectionMultiplexer.Connect(cacheConnectionString);

            var repo = new CacheAsideRepository<MyEntity>(store, connection.GetDatabase());
            var entity = repo.GetById(1);
            entity = repo.GetById(1);
            repo.Update(entity);
            entity = repo.GetById(1);
        }

        /// <summary>
        /// Build the confguration
        /// </summary>
        /// <returns>Returns the configuration</returns>
        private static IConfigurationRoot BuildConfiguration()
        {
            // Enable to app to read json setting files
            var builder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

#if DEBUG
            // We use user secrets in Debug mode so API keys are not uploaded to source control 
            builder.AddUserSecrets("cmendible3-dotnetcore.samples-cacheAside");
#endif

            return builder.Build();
        }
    }
}

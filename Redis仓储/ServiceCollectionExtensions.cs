namespace RedisRepository1
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddRedis(this IServiceCollection services, Action<RedisOptions> options)
        {
            if (options == null)
            {
                throw new ArgumentNullException("options");
            }

            services.Configure(options);
            services.AddSingleton<IRedisManager, RedisManager>();
            return services;
        }
    }
}

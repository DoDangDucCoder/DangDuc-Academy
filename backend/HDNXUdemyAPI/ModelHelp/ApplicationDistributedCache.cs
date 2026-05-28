namespace HDNXUdemyAPI.ModelHelp
{
    /// <summary>
    /// Configures the distributed cache provider for the API.
    /// </summary>
    public static class ApplicationDistributedCache
    {
        /// <summary>
        /// Adds Redis distributed cache when Redis settings are present; otherwise falls back to in-memory cache.
        /// </summary>
        /// <param name="services">The application service collection.</param>
        /// <param name="configuration">The application configuration.</param>
        public static void ApplicationDistributedConfigulation(this IServiceCollection services, IConfiguration configuration)
        {
            string redisHost = configuration["Redis:Host"] ?? string.Empty;
            string redisPort = configuration["Redis:Port"] ?? string.Empty;
            if (string.IsNullOrWhiteSpace(redisHost) || !int.TryParse(redisPort, out int parsedRedisPort))
            {
                services.AddDistributedMemoryCache();
                return;
            }

            services.AddStackExchangeRedisCache(
                option =>
                {
                    option.ConfigurationOptions = new StackExchange.Redis.ConfigurationOptions()
                    {
                        User = configuration["Redis:User"] ?? string.Empty,
                        Password = configuration["Redis:Password"] ?? string.Empty,
                        AllowAdmin = true,
                        EndPoints =
                        {
                            { redisHost, parsedRedisPort }
                        },

                        AbortOnConnectFail = false
                    };
                });
        }
    }
}

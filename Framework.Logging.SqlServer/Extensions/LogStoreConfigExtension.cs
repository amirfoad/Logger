using Framework.Logging.SqlServer;

namespace Framework.Logging.Dapper.Extensions
{
    public static class LogStoreConfigExtension
    {
        private static readonly string _storeName = $"{nameof(SqlServerLogStore)}-{Guid.NewGuid():N}";

        public static LogBuilder SqlServer(this LogStoreConfig config,
            string SqlConnection,
            LogLevel logLevel)
        {
            var DapperLogConfig = new SqlServerLogConfig()
            {
                SqlConnection = SqlConnection,
                Level = logLevel
            };

            var context = config.LoggerContext;
            context.LogConfigurations.Add(_storeName, DapperLogConfig);

            return config.Store(new SqlServerLogStore(config.LoggerContext, _storeName, DapperLogConfig));
        }
    }
}
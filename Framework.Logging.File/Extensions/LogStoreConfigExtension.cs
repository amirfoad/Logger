namespace Framework.Logging.Files.Extensions
{
    public static class LogStoreConfigExtension
    {
        private static readonly string _storeName = $"{nameof(FileLogStore)}-{Guid.NewGuid():N}";

        public static LogBuilder File(this LogStoreConfig config, LogLevel? level, string path = "", RollingInterval rollingInterval = RollingInterval.Day)
        {
            var fileLogConfig = new FileLogConfig()
            {
                Level = level,
                Name = path,
                RollingInterval = rollingInterval
            };

            var context = config.LoggerContext;
            context.LogConfigurations.Add(_storeName, fileLogConfig);

            return config.Store(new FileLogStore(config.LoggerContext, _storeName));
        }

        public static LogBuilder File(this LogStoreConfig config, string path = "", RollingInterval rollingInterval = RollingInterval.Day)
        {
            var fileLogConfig = new FileLogConfig()
            {
                Name = path,
                RollingInterval = rollingInterval
            };

            var context = config.LoggerContext;
            context.LogConfigurations.Add(_storeName, fileLogConfig);

            return config.Store(new FileLogStore(config.LoggerContext, _storeName));
        }
    }
}
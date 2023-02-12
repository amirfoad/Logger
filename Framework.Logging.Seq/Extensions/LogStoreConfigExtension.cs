namespace Framework.Logging.Seq.Extensions
{
    public static class LogStoreConfigExtension
    {
        private static readonly string _storeName = $"{nameof(SeqLogStore)}-{Guid.NewGuid():N}";

        public static LogBuilder Seq(this LogStoreConfig config, string connection, string apiKey = "", LogLevel level = LogLevel.Info)
        {
            var fileLogConfig = new SeqLogConfig()
            {
                ApiKey = apiKey,
                ConnectionString = connection
            };

            var context = config.LoggerContext;
            context.LogConfigurations.Add(_storeName, fileLogConfig);

            return config.Store(new SeqLogStore(_storeName, config.LoggerContext));
        }
    }
}
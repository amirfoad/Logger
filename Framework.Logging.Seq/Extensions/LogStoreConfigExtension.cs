using System;
namespace Framework.Logging.Seq.Extensions
{
	public static class LogStoreConfigExtension
	{
        private static readonly string _storeName = $"{nameof(SeqLogStore)}-{Guid.NewGuid():N}";


        public static LogBuilder ElasticSearch(this LogStoreConfig config, string connection)

        {
            var elasticConfig = new SeqLogConfig()
            {
                ConnectionString = connection,
            };
            config.LoggerContext.LogConfigurations.Add(_storeName, elasticConfig);
            return config.Store(new SeqLogStore(_storeName,config.LoggerContext));
        }
    }
}


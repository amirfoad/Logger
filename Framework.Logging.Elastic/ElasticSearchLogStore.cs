using Framework.Logging.Common;
using Nest;

namespace Framework.Logging.Elastic
{
    public class ElasticSearchLogStore : ILogStore
    {
        private readonly IElasticClient _elasticClient;
        private readonly LoggerContext _context;
        private readonly ElasticSearchLogConfig _elasticSearchLogConfig;

        public ElasticSearchLogStore(string name, IElasticClient elasticClient, LoggerContext context)
        {
            Name = name;
            _elasticClient = elasticClient;
            _elasticSearchLogConfig = (ElasticSearchLogConfig)context.LogConfigurations[Name];
            _context = context;
        }

        public string Name { get; private set; }

        #region Read

        public IList<Log<T>> Read<T>(DateTime date) where T : class
        {
            var result = _elasticClient.Search<Log<T>>(s =>
              s.Index(nameof(Log).ToLower())
              .Query(q => q
              .DateRange(b => b
                      .Field(field => field.LogDate)
                      .GreaterThanOrEquals(GenerateStartDate(date))
                      .LessThanOrEquals(GenerateEndDate(date)))));

            if (!result.IsValid)
                throw new Exception(result.OriginalException.Message);

            return result.Documents.ToList();
        }

        public IList<Log<T>> Read<T>(DateTime date, string level) where T : class
        {
            var result = _elasticClient.Search<Log<T>>(s =>
                     s.Index(nameof(Log).ToLower())
                     .Query(q => q
                         .DateRange(b => b
                         .Field(field => field.LogDate)
                         .GreaterThanOrEquals(GenerateStartDate(date))
                         .LessThanOrEquals(GenerateEndDate(date))))
                      .Query(q => q.Match(m => m.Field(f => f.Level).Query(level))));

            if (!result.IsValid)
                throw new Exception(result.OriginalException.Message);

            return result.Documents.ToList();
        }

        public IList<Log<T>> Read<T>(string category, DateTime date) where T : class
        {
            var result = _elasticClient.Search<Log<T>>(s =>
                      s.Index(nameof(Log).ToLower())
                      .Query(q => q
                          .DateRange(b => b
                          .Field(field => field.LogDate)
                          .GreaterThanOrEquals(GenerateStartDate(date))
                          .LessThanOrEquals(GenerateEndDate(date))))
                       .Query(q => q.Match(m => m.Field(f => f.Category).Query(category))));
            if (!result.IsValid)
                throw new Exception(result.OriginalException.Message);

            return result.Documents.ToList();
        }

        public IList<Log<T>> Read<T>(DateTime fromDate, DateTime toDate) where T : class
        {
            var result = _elasticClient.Search<Log<T>>(s =>
            s.Index(nameof(Log).ToLower())
            .Query(q => q
                .DateRange(b => b
                        .Field(field => field.LogDate)
                        .GreaterThanOrEquals(GenerateStartDate(fromDate))
                        .LessThanOrEquals(GenerateEndDate(toDate)))));

            if (!result.IsValid)
                throw new Exception(result.OriginalException.Message);

            return result.Documents.ToList();
        }

        public IList<Log<T>> Read<T>(DateTime fromDate, DateTime toDate, string level) where T : class
        {
            var result = _elasticClient.Search<Log<T>>(s =>
            s.Index(nameof(Log).ToLower())
            .Query(q => q
                .DateRange(b => b
                .Field(field => field.LogDate)
                .GreaterThanOrEquals(GenerateStartDate(fromDate))
                .LessThanOrEquals(GenerateEndDate(toDate))))
             .Query(q => q.Match(m => m.Field(f => f.Level).Query(level))));

            if (!result.IsValid)
                throw new Exception(result.OriginalException.Message);

            return result.Documents.ToList();
        }

        #endregion Read

        #region Write

        public void Write<T>(ILog<T> log) where T : class
        {
            if (log.Level.IsLevelEnabled(_context.MinimumLevel, _elasticSearchLogConfig.Level))
            {
                var result = _elasticClient.Index(log, i => i
                  .Index(nameof(Log).ToLower())
                  .Refresh(Elasticsearch.Net.Refresh.True));
                if (!result.IsValid)
                    throw new Exception(result.ServerError.Status.ToString());
            }
        }

        #endregion Write

        #region Private Methods

        private DateTime GenerateStartDate(DateTime date)
        {
            return new DateTime(date.Year, date.Month, date.Day, 0, 0, 0);
        }

        private DateTime GenerateEndDate(DateTime date)
        {
            return new DateTime(date.Year, date.Month, date.Day, 23, 59, 59);
        }

        #endregion Private Methods
    }
}
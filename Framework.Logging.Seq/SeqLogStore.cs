using System;
using Seq.Api;
using Seq.Api.Client;

namespace Framework.Logging.Seq
{
    public class SeqLogStore : ILogStore
    {
        private readonly SeqApiClient _seqClient;
        private readonly SeqConnection _seqConnection;
        private readonly SeqLogConfig _seqConfig;
        private readonly LoggerContext _context;
        public SeqLogStore(string name,LoggerContext context)
        {
            Name = name;
            _context = context;
            _seqConfig = (SeqLogConfig)context.LogConfigurations[name];
            _seqClient = new SeqApiClient(_seqConfig.ConnectionString);
            _seqConnection = new SeqConnection(_seqConfig.ConnectionString);
        }
        public string Name { get; private set; }

        public IList<Log<T>> Read<T>(DateTime date) where T : class
        {
            throw new NotImplementedException();
        }

        public IList<Log<T>> Read<T>(DateTime date, string level) where T : class
        {
            throw new NotImplementedException();
        }

        public IList<Log<T>> Read<T>(string category, DateTime date) where T : class
        {
            throw new NotImplementedException();
        }

        public IList<Log<T>> Read<T>(DateTime fromDate, DateTime toDate) where T : class
        {
            throw new NotImplementedException();
        }

        public IList<Log<T>> Read<T>(DateTime fromDate, DateTime toDate, string level) where T : class
        {
            throw new NotImplementedException();
        }

        public void Write<T>(ILog<T> log) where T : class
        {
            var signal = _seqConnection.Signals.TemplateAsync().Result;
            signal.Title = "Log";
            var result = _seqConnection.Signals.AddAsync(signal);
            throw new NotImplementedException();
        }
    }
}


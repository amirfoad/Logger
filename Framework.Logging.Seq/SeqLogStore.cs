using Framework.Logging.Common;
using System.Text;

namespace Framework.Logging.Seq
{
    public class SeqLogStore : ILogStore
    {
        private readonly LoggerContext _context;
        private readonly SeqLogConfig _seqLogConfig;
        private readonly int _messageNumber;
        private readonly HttpClient Client = new HttpClient();

        public SeqLogStore(string name, LoggerContext context)
        {
            Name = name;
            _messageNumber = 0;
            _context = context;
            _seqLogConfig = (SeqLogConfig)context.LogConfigurations[name];
        }

        public string Name { get; private set; }

        public IList<Log<T>> Read<T>(DateTime date) where T : class
        {
            return new List<Log<T>>();
        }

        public IList<Log<T>> Read<T>(DateTime date, string level) where T : class
        {
            return new List<Log<T>>();
        }

        public IList<Log<T>> Read<T>(string category, DateTime date) where T : class
        {
            return new List<Log<T>>();
        }

        public IList<Log<T>> Read<T>(DateTime fromDate, DateTime toDate) where T : class
        {
            return new List<Log<T>>();
        }

        public IList<Log<T>> Read<T>(DateTime fromDate, DateTime toDate, string level) where T : class
        {
            return new List<Log<T>>();
        }

        public void Write<T>(ILog<T> log) where T : class
        {
            if (log.Level.IsLevelEnabled(_context.MinimumLevel, _seqLogConfig.Level))
            {
                try
                {
                    string msg = log.GenerateLogMessage(_messageNumber, log.Level);
                    StringContent i = new StringContent(msg, Encoding.UTF8, "application/json");
                    string u = _seqLogConfig.ConnectionString.NormalizeServerBaseAddress() + "api/events/raw?clef";
                    if (!string.IsNullOrEmpty(_seqLogConfig.ApiKey))
                    {
                        u = _seqLogConfig.ConnectionString.NormalizeServerBaseAddress() + "api/events/raw?clef&apikey=" + _seqLogConfig.ApiKey;
                    }

                    HttpResponseMessage result = Client.PostAsync(u, i).Result;
                    string resultMessage = result.Content.ReadAsStringAsync().Result;
                    if (!result.IsSuccessStatusCode)
                    {
                        Console.WriteLine(resultMessage);
                        Console.WriteLine(result);
                    }
                }
                catch (Exception ex)
                {
                    Exception e = ex;
                    Console.WriteLine(e);
                }
            }
        }
    }
}
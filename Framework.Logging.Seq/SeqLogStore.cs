using Framework.Logging.Common;
using System.Text;
using System.Text.Json;

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

        #region Read

        public IList<Log<T>> Read<T>(DateTime date) where T : class
        {
            var query = "LogDate%3D'{date}'";
            return ReadFromSeq<T>(query);
        }

        public IList<Log<T>> Read<T>(DateTime date, string level) where T : class
        {
            var query = "LogDate%3D'{date}'%20and%20Level%20%3D%20'{level}'%20";
            return ReadFromSeq<T>(query);
        }

        public IList<Log<T>> Read<T>(string category, DateTime date) where T : class
        {
            var query = "LogDate%3D'{date}'%20and%20Category%20%3D%20'{category}'%20";
            return ReadFromSeq<T>(query);
        }

        public IList<Log<T>> Read<T>(DateTime fromDate, DateTime toDate) where T : class
        {
            var query = "LogDate%20>%3D'{fromDate}'%20and%20LogDate%20<%3D%20'{toDate}'";
            return ReadFromSeq<T>(query);
        }

        public IList<Log<T>> Read<T>(DateTime fromDate, DateTime toDate, string level) where T : class
        {
            var query = "LogDate%20>%3D'{fromDate}'%20and%20LogDate%20<%3D%20'{toDate}'%20and%20Level%20%3D%20'{level}'";
            return ReadFromSeq<T>(query);
        }

        #endregion Read

        #region Write

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

        #endregion Write

        #region Private Methods

        private IList<Log<T>> ReadFromSeq<T>(string query) where T : class
        {
            // Add the Seq API key to the request headers
            Client.DefaultRequestHeaders.Add("X-Seq-ApiKey", _seqLogConfig.ApiKey);

            // Send an HTTP GET request to the /api/events endpoint with a filter for Error level events
            var response = Client.GetAsync($"{_seqLogConfig.ConnectionString.NormalizeServerBaseAddress()}api/events?filter={query}").Result;

            // Throw an exception if the response is not successful
            response.EnsureSuccessStatusCode();

            // Deserialize the response body to a list of event objects
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var events = JsonSerializer.Deserialize<List<Log<T>>>(response.Content.ReadAsStream(), options);
            return events;
        }

        #endregion Private Methods
    }
}
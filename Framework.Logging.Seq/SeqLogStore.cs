using Framework.Logging.Common;
using Newtonsoft.Json;
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

        public Task<IList<Log<T>>> Read<T>(DateTime date) where T : class
        {
            var query = $"LogDate%20>%3D'{date:yyyy-MM-dd}'%20and%20LogDate%20<%20'{date.AddDays(1):yyyy-MM-dd}'";
            return ReadFromSeq<T>(query);
        }

        public async Task<IList<Log<T>>> Read<T>(DateTime date, string level) where T : class
        {
            var query = $"LogDate%20>%3D'{date:yyyy-MM-dd}'%20and%20LogDate%20<%3D%20'{date.AddDays(1):yyyy-MM-dd}'%20and%20Level%20%3D%20'{level}'";
            return await ReadFromSeq<T>(query);
        }

        public async Task<IList<Log<T>>> Read<T>(string category, DateTime date) where T : class
        {
            var query = $"LogDate%20>%3D'{date:yyyy-MM-dd}'%20and%20LogDate%20<%20'{date.AddDays(1)}'%20and%20Category%20%3D%20'{category}'";
            return await ReadFromSeq<T>(query);
        }

        public async Task<IList<Log<T>>> Read<T>(DateTime fromDate, DateTime toDate) where T : class
        {
            var query = $"LogDate%20>%3D'{fromDate:yyyy-MM-dd}'%20and%20LogDate%20<%3D%20'{toDate:yyyy-MM-dd}'";
            return await ReadFromSeq<T>(query);
        }

        public async Task<IList<Log<T>>> Read<T>(DateTime fromDate, DateTime toDate, string level) where T : class
        {
            var query = $"LogDate%20>%3D'{fromDate:yyyy-MM-dd}'%20and%20LogDate%20<%3D%20'{toDate:yyyy-MM-dd}'%20and%20Level%20%3D%20'{level}'";
            return await ReadFromSeq<T>(query);
        }

        #endregion Read

        #region Write

        public async Task Write<T>(ILog<T> log) where T : class
        {
            if (log.Level.IsLevelEnabled(_context.MinimumLevel, _seqLogConfig.Level))
            {
                try
                {
                    string msg = log.GenerateLogMessage(_messageNumber, log.Level.ToUpper());
                    StringContent i = new StringContent(msg, Encoding.UTF8, "application/json");
                    string u = _seqLogConfig.ConnectionString.NormalizeServerBaseAddress() + "api/events/raw?clef";
                    if (!string.IsNullOrEmpty(_seqLogConfig.ApiKey))
                    {
                        u = _seqLogConfig.ConnectionString.NormalizeServerBaseAddress() + "api/events/raw?clef&apikey=" + _seqLogConfig.ApiKey;
                    }

                    HttpResponseMessage result = await Client.PostAsync(u, i);
                    string resultMessage = await result.Content.ReadAsStringAsync();
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

        private async Task<IList<Log<T>>> ReadFromSeq<T>(string query) where T : class
        {
            // Add the Seq API key to the request headers
            Client.DefaultRequestHeaders.Add("X-Seq-ApiKey", _seqLogConfig.ApiKey);

            // Send an HTTP GET request to the /api/events endpoint with a filter for Error level events
            var response = await Client.GetAsync($"{_seqLogConfig.ConnectionString.NormalizeServerBaseAddress()}api/events?filter={query}");

            // Throw an exception if the response is not successful
            response.EnsureSuccessStatusCode();
            var stringResposne = response.Content.ReadAsStringAsync().Result;
            var responseAsStream = await response.Content.ReadAsStreamAsync();
            var properties = await System.Text.Json.JsonSerializer.DeserializeAsync<List<Event>>(responseAsStream);

            var result = ConvertEventsToLogs<T>(properties);
            return result;
        }

        private class Event
        {
            [JsonProperty("MessageTemplateTokens")]
            public List<MessageTemplateTokens> MessageTemplateTokens { get; set; }

            [JsonProperty("Properties")]
            public List<Properties> Properties { get; set; }
        }

        private class Properties
        {
            public string Name { get; set; }
            public string Value { get; set; }
        }

        private class MessageTemplateTokens
        {
            public string Text { get; set; }
        }

        private IList<Log<T>> ConvertEventsToLogs<T>(List<Event> events) where T : class
        {
            var logs = new List<Log<T>>();

            foreach (var property in events)
            {
                logs.Add(new Log<T>
                {
                    LogDate = Convert.ToDateTime(property.Properties.FirstOrDefault(p => p.Name == nameof(Log.LogDate)).Value),
                    Level = property.Properties.FirstOrDefault(p => p.Name == nameof(Log.Level)).Value,
                    Message = (T)Convert.ChangeType(property.MessageTemplateTokens.FirstOrDefault().Text, typeof(T)),
                    Category = property.Properties.FirstOrDefault(p => p.Name == nameof(Log.Category)).Value,
                    TraceId = property.Properties.FirstOrDefault(p => p.Name == nameof(Log.TraceId)).Value,
                    SectionId = property.Properties.FirstOrDefault(p => p.Name == nameof(Log.SectionId)).Value,
                    ServiceId = property.Properties.FirstOrDefault(p => p.Name == nameof(Log.ServiceId)).Value
                });
            }

            return logs;
        }

        #endregion Private Methods
    }
}
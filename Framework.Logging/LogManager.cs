using Framework.Core.Extensions;
using Framework.Logging.Common;

namespace Framework.Logging
{
    public class LogManager<T> : ILogReader<T>, ILogger<T> where T : class
    {
        private readonly Dictionary<string, ILogStore> _logStores = new();
        private readonly LoggerContext _logContext;

        public LogManager(Dictionary<string, ILogStore> logStores, LoggerContext logContext)
        {
            _logStores = logStores;
            _logContext = logContext;
        }

        #region Read

        public async Task<IList<Log<T>>> Debug(DateTime date)
        {
            var logs = new List<Log<T>>();
            foreach (var log in _logStores.Values)
            {
                logs.AddRange(await log.Read<T>(date, LogLevel.Debug.LevelDescriptor()));
            }
            return logs;
        }

        public async Task<IList<Log<T>>> Debug(DateTime fromDate, DateTime toDate)
        {
            var logs = new List<Log<T>>();
            foreach (var log in _logStores.Values)
            {
                logs.AddRange(await log.Read<T>(fromDate, toDate, LogLevel.Debug.LevelDescriptor()));
            }
            return logs;
        }

        public async Task<IList<Log<T>>> Read(DateTime date)
        {
            var logs = new List<Log<T>>();
            foreach (var log in _logStores.Values)
            {
                logs.AddRange(await log.Read<T>(date));
            }
            return logs;
        }

        public async Task<IList<Log<T>>> Read(DateTime date, LogLevel level)
        {
            var logs = new List<Log<T>>();
            foreach (var log in _logStores.Values)
            {
                logs.AddRange(await log.Read<T>(date, level.LevelDescriptor()));
            }
            return logs;
        }

        public async Task<IList<Log<T>>> Read(string category, DateTime date)
        {
            var logs = new List<Log<T>>();
            foreach (var log in _logStores.Values)
            {
                logs.AddRange(await log.Read<T>(category, date));
            }
            return logs;
        }

        public async Task<IList<Log<T>>> Read(DateTime fromDate, DateTime toDate)
        {
            var logs = new List<Log<T>>();
            foreach (var log in _logStores.Values)
            {
                logs.AddRange(await log.Read<T>(fromDate, toDate));
            }
            return logs;
        }

        public async Task<IList<Log<T>>> Read(DateTime fromDate, DateTime toDate, LogLevel level)
        {
            var logs = new List<Log<T>>();
            foreach (var log in _logStores.Values)
            {
                logs.AddRange(await log.Read<T>(fromDate, toDate, level.LevelDescriptor()));
            }
            return logs;
        }

        public async Task<IList<Log<T>>> Info(DateTime date)
        {
            var logs = new List<Log<T>>();
            foreach (var log in _logStores.Values)
            {
                logs.AddRange(await log.Read<T>(date, LogLevel.Info.LevelDescriptor()));
            }
            return logs;
        }

        public async Task<IList<Log<T>>> Warning(DateTime date)
        {
            var logs = new List<Log<T>>();
            foreach (var log in _logStores.Values)
            {
                logs.AddRange(await log.Read<T>(date, LogLevel.Warning.LevelDescriptor()));
            }
            return logs;
        }

        public async Task<IList<Log<T>>> Trace(DateTime date)
        {
            var logs = new List<Log<T>>();
            foreach (var log in _logStores.Values)
            {
                logs.AddRange(await log.Read<T>(date, LogLevel.Trace.LevelDescriptor()));
            }
            return logs;
        }

        public async Task<IList<Log<T>>> Error(DateTime date)
        {
            var logs = new List<Log<T>>();
            foreach (var log in _logStores.Values)
            {
                logs.AddRange(await log.Read<T>(date, LogLevel.Error.LevelDescriptor()));
            }
            return logs;
        }

        public async Task<IList<Log<T>>> Fatal(DateTime date)
        {
            var logs = new List<Log<T>>();
            foreach (var log in _logStores.Values)
            {
                logs.AddRange(await log.Read<T>(date, LogLevel.Fatal.LevelDescriptor()));
            }
            return logs;
        }

        public async Task<IList<Log<T>>> Info(DateTime fromDate, DateTime toDate)
        {
            var logs = new List<Log<T>>();
            foreach (var log in _logStores.Values)
            {
                logs.AddRange(await log.Read<T>(fromDate, toDate, LogLevel.Info.LevelDescriptor()));
            }
            return logs;
        }

        public async Task<IList<Log<T>>> Warning(DateTime fromDate, DateTime toDate)
        {
            var logs = new List<Log<T>>();
            foreach (var log in _logStores.Values)
            {
                logs.AddRange(await log.Read<T>(fromDate, toDate, LogLevel.Error.LevelDescriptor()));
            }
            return logs;
        }

        public async Task<IList<Log<T>>> Trace(DateTime fromDate, DateTime toDate)
        {
            var logs = new List<Log<T>>();
            foreach (var log in _logStores.Values)
            {
                logs.AddRange(await log.Read<T>(fromDate, toDate, LogLevel.Trace.LevelDescriptor()));
            }
            return logs;
        }

        public async Task<IList<Log<T>>> Error(DateTime fromDate, DateTime toDate)
        {
            var logs = new List<Log<T>>();
            foreach (var log in _logStores.Values)
            {
                logs.AddRange(await log.Read<T>(fromDate, toDate, LogLevel.Error.LevelDescriptor()));
            }
            return logs;
        }

        public async Task<IList<Log<T>>> Fatal(DateTime fromDate, DateTime toDate)
        {
            var logs = new List<Log<T>>();
            foreach (var log in _logStores.Values)
            {
                logs.AddRange(await log.Read<T>(fromDate, toDate, LogLevel.Fatal.LevelDescriptor()));
            }
            return logs;
        }

        #endregion Read

        #region Write

        public async Task Log(string level, T message, string traceId, string sectionId, string serviceId, string category = "")
        {
            Log<T> log;

            var logDate = DateTime.Now;

            if (_logContext.UseUtcTime)
            {
                logDate = DateTime.UtcNow;
            }
            if (category.IsNullOrEmpty())
            {
                log = new Log<T>
                {
                    Level = level.ToUpper(),
                    Message = message,
                    LogDate = logDate,
                    TraceId = traceId,
                    SectionId = sectionId,
                    ServiceId = serviceId,
                };
            }
            else
            {
                log = new Log<T>(category)
                {
                    Level = level.ToUpper(),
                    Message = message,
                    LogDate = logDate,
                    TraceId = traceId,
                    SectionId = sectionId,
                    ServiceId = serviceId,
                };
            }

            foreach (var logStore in _logStores)
            {
                await logStore.Value.Write(log);
            }
        }

        public async Task Info(T message, string traceId, string sectionId, string serviceId, string category = "") => await Log(nameof(LogLevel.Info), message, traceId, sectionId, serviceId, category);

        public async Task Debug(T message, string traceId, string sectionId, string serviceId, string category = "") => await Log(nameof(LogLevel.Debug), message, traceId, sectionId, serviceId, category);

        public async Task Error(T message, string traceId, string sectionId, string serviceId, string category = "") => await Log(nameof(LogLevel.Error), message, traceId, sectionId, serviceId, category);

        public async Task Fatal(T message, string traceId, string sectionId, string serviceId, string category = "") => await Log(nameof(LogLevel.Fatal), message, traceId, sectionId, serviceId, category);

        public async Task Trace(T message, string traceId, string sectionId, string serviceId, string category = "") => await Log(nameof(LogLevel.Trace), message, traceId, sectionId, serviceId, category);

        public async Task Warning(T message, string traceId, string sectionId, string serviceId, string category = "") => await Log(nameof(LogLevel.Warning), message, traceId, sectionId, serviceId, category);

        #endregion Write
    }
}
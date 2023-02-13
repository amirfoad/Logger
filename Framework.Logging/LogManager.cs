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

        public IList<Log<T>> Debug(DateTime date)
        {
            var logs = new List<Log<T>>();
            foreach (var log in _logStores.Values)
            {
                logs.AddRange(log.Read<T>(date, "Debug"));
            }
            return logs;
        }

        public IList<Log<T>> Debug(DateTime fromDate, DateTime toDate)
        {
            var logs = new List<Log<T>>();
            foreach (var log in _logStores.Values)
            {
                logs.AddRange(log.Read<T>(fromDate, toDate,"Debug"));
            }
            return logs;
        }

        public IList<Log<T>> Read(DateTime date)
        {
            var logs = new List<Log<T>>();
            foreach (var log in _logStores.Values)
            {
                logs.AddRange(log.Read<T>(date));
            }
            return logs;
        }

        public IList<Log<T>> Read(DateTime date, LogLevel level)
        {
            var logs = new List<Log<T>>();
            foreach (var log in _logStores.Values)
            {
                logs.AddRange(log.Read<T>(date, level.LevelDescriptor()));
            }
            return logs;
        }

        public IList<Log<T>> Read(string category, DateTime date)
        {
            var logs = new List<Log<T>>();
            foreach (var log in _logStores.Values)
            {
                logs.AddRange(log.Read<T>(category, date));
            }
            return logs;
        }

        public IList<Log<T>> Read(DateTime fromDate, DateTime toDate)
        {
            var logs = new List<Log<T>>();
            foreach (var log in _logStores.Values)
            {
                logs.AddRange(log.Read<T>(fromDate, toDate));
            }
            return logs;
        }

        public IList<Log<T>> Read(DateTime fromDate, DateTime toDate, LogLevel level)
        {
            var logs = new List<Log<T>>();
            foreach (var log in _logStores.Values)
            {
                logs.AddRange(log.Read<T>(fromDate, toDate, level.LevelDescriptor()));
            }
            return logs;
        }

        public IList<Log<T>> Info(DateTime date)
        {
            var logs = new List<Log<T>>();
            foreach (var log in _logStores.Values)
            {
                logs.AddRange(log.Read<T>(date, "Info"));
            }
            return logs;
        }

        public IList<Log<T>> Warning(DateTime date)
        {
            var logs = new List<Log<T>>();
            foreach (var log in _logStores.Values)
            {
                logs.AddRange(log.Read<T>(date, "Warning"));
            }
            return logs;
        }

        public IList<Log<T>> Trace(DateTime date)
        {
            var logs = new List<Log<T>>();
            foreach (var log in _logStores.Values)
            {
                logs.AddRange(log.Read<T>(date, "Trace"));
            }
            return logs;
        }

        public IList<Log<T>> Error(DateTime date)
        {
            var logs = new List<Log<T>>();
            foreach (var log in _logStores.Values)
            {
                logs.AddRange(log.Read<T>(date, "Error"));
            }
            return logs;
        }

        public IList<Log<T>> Fatal(DateTime date)
        {
            var logs = new List<Log<T>>();
            foreach (var log in _logStores.Values)
            {
                logs.AddRange(log.Read<T>(date, "Fatal"));
            }
            return logs;
        }

        public IList<Log<T>> Info(DateTime fromDate, DateTime toDate)
        {
            var logs = new List<Log<T>>();
            foreach (var log in _logStores.Values)
            {
                logs.AddRange(log.Read<T>(fromDate, toDate,"Info"));
            }
            return logs;
        }

        public IList<Log<T>> Warning(DateTime fromDate, DateTime toDate)
        {
            var logs = new List<Log<T>>();
            foreach (var log in _logStores.Values)
            {
                logs.AddRange(log.Read<T>(fromDate, toDate, "Warning"));
            }
            return logs;
        }

        public IList<Log<T>> Trace(DateTime fromDate, DateTime toDate)
        {
            var logs = new List<Log<T>>();
            foreach (var log in _logStores.Values)
            {
                logs.AddRange(log.Read<T>(fromDate, toDate, "Trace"));
            }
            return logs;
        }

        public IList<Log<T>> Error(DateTime fromDate, DateTime toDate)
        {
            var logs = new List<Log<T>>();
            foreach (var log in _logStores.Values)
            {
                logs.AddRange(log.Read<T>(fromDate, toDate, "Error"));
            }
            return logs;
        }

        public IList<Log<T>> Fatal(DateTime fromDate, DateTime toDate)
        {
            var logs = new List<Log<T>>();
            foreach (var log in _logStores.Values)
            {
                logs.AddRange(log.Read<T>(fromDate, toDate, "Fatal"));
            }
            return logs;
        }

        #endregion Read

        #region Write

        public void Log(string level, T message, string traceId, string sectionId, string serviceId, string category = "")
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
                    Level = level,
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
                    Level = level,
                    Message = message,
                    LogDate = logDate,
                    TraceId = traceId,
                    SectionId = sectionId,
                    ServiceId = serviceId,
                };
            }

            foreach (var logStore in _logStores)
            {
                logStore.Value.Write(log);
            }
        }

        public void Info(T message, string traceId, string sectionId, string serviceId, string category = "") => Log(nameof(LogLevel.Info), message, traceId, sectionId, serviceId, category);

        public void Debug(T message, string traceId, string sectionId, string serviceId, string category = "") => Log(nameof(LogLevel.Debug), message, traceId, sectionId, serviceId, category);

        public void Error(T message, string traceId, string sectionId, string serviceId, string category = "") => Log(nameof(LogLevel.Error), message, traceId, sectionId, serviceId, category);

        public void Fatal(T message, string traceId, string sectionId, string serviceId, string category = "") => Log(nameof(LogLevel.Fatal), message, traceId, sectionId, serviceId, category);

        public void Trace(T message, string traceId, string sectionId, string serviceId, string category = "") => Log(nameof(LogLevel.Trace), message, traceId, sectionId, serviceId, category);

        public void Warning(T message, string traceId, string sectionId, string serviceId, string category = "") => Log(nameof(LogLevel.Warning), message, traceId, sectionId, serviceId, category);


        #endregion Write
    }
}
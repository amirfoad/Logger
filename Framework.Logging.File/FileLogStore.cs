using Framework.Core;
using Framework.Logging.Common;
using Framework.Logging.Files.Helpers;
using Newtonsoft.Json;

namespace Framework.Logging.Files
{
    public class FileLogStore : ILogStore, IDisposable
    {
        private readonly Func<string, string> _sanitizeLevel = level => level.Trim().ToUpper();
        private readonly object _lock = new();
        private readonly LoggerContext _context;
        private readonly FileLogConfig _fileConfig;
        private readonly OpenStreams _openStreams;

        private bool Disposed { get; set; }

        private static ObjectDisposedException ObjectDisposedException
        {
            get
            {
                return new ObjectDisposedException("Cannot access a disposed object.");
            }
        }

        private DateTime Now
        {
            get
            {
                return _context.UseUtcTime ? DateTime.UtcNow : DateTime.Now;
            }
        }

        public FileLogStore(LoggerContext context, string name)
        {
            Argument.IsNotNull(() => context);
            _context = context;
            Name = name;
            _fileConfig = (FileLogConfig)context.LogConfigurations[Name];
            _openStreams = new OpenStreams(_fileConfig);
        }

        public string Name { get; private set; }

        #region Read

        public IList<Log<T>> Read<T>(DateTime date) where T : class
        {
            var data = string.Empty;
            lock (_lock)
            {
                if (Disposed)
                    throw ObjectDisposedException;

                data = _openStreams.Read(date.Date);
            }

            var result = JsonConvert.DeserializeObject<IList<Log<T>>>(data);

            return result;
        }

        public IList<Log<T>> Read<T>(string category, DateTime date) where T : class
        {
            return Read<T>(date.Date).Where(r => r.Category == category).ToList();
        }

        public IList<Log<T>> Read<T>(DateTime date, string level) where T : class
        {
            return Read<T>(date).Where(c => c.Level == level).ToList();
        }

        public IList<Log<T>> Read<T>(DateTime fromDate, DateTime toDate) where T : class
        {
            var data = string.Empty;
            lock (_lock)
            {
                if (Disposed)
                    throw ObjectDisposedException;

                data = _openStreams.Read(fromDate.Date, toDate.Date);
            }
            var result = JsonConvert.DeserializeObject<IList<Log<T>>>(data);
            return result;
        }

        public IList<Log<T>> Read<T>(DateTime fromDate, DateTime toDate, string level) where T : class
        {
            return Read<T>(fromDate, toDate).Where(p => p.Level == level).ToList();
        }

        #endregion Read

        #region Write

        public void Write<T>(ILog<T> log) where T : class
        {
            if (log.Level.IsLevelEnabled(_context.MinimumLevel, _fileConfig.Level))
            {
                Argument.IsNotNull(() => log);

                log.Level = _sanitizeLevel(log.Level);
                var line = JsonConvert.SerializeObject(log);

                lock (_lock)
                {
                    if (Disposed)
                        throw ObjectDisposedException;

                    _openStreams.Append(Now, line);
                }
            }
        }

        #endregion Write

        public void Dispose()
        {
            lock (_lock)
            {
                if (Disposed)
                    return;

                if (_openStreams != null)
                    _openStreams.Dispose();

                Disposed = true;
            }
        }
    }
}
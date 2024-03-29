﻿using Framework.Logging.Common;

namespace Framework.Logging.Console
{
    public class ConsoleLogStore : ILogStore
    {
        private readonly LoggerContext _context;
        private readonly ConsoleLogConfig _consoleConfig;
        private readonly Func<string, string> _sanitizeLevel = level => level.Trim().ToUpper();

        public ConsoleLogStore(string name, LoggerContext context)
        {
            Name = name;
            _context = context;
            _consoleConfig = (ConsoleLogConfig)_context.LogConfigurations[name];
        }

        public string Name { get; private set; }

        #region Read

        public async Task<IList<Log<T>>> Read<T>(DateTime date) where T : class
        {
            return new List<Log<T>>();
        }

        public async Task<IList<Log<T>>> Read<T>(DateTime date, string level) where T : class
        {
            return new List<Log<T>>();
        }

        public async Task<IList<Log<T>>> Read<T>(string category, DateTime date) where T : class
        {
            return new List<Log<T>>();
        }

        public async Task<IList<Log<T>>> Read<T>(DateTime fromDate, DateTime toDate) where T : class
        {
            return new List<Log<T>>();
        }

        public async Task<IList<Log<T>>> Read<T>(DateTime fromDate, DateTime toDate, string level) where T : class
        {
            return new List<Log<T>>();
        }

        #endregion Read

        #region Write

        public Task Write<T>(ILog<T> log) where T : class
        {
            if (log.Level.IsLevelEnabled(_context.MinimumLevel, _consoleConfig.Level))
            {
                log.Level = _sanitizeLevel(log.Level);
                System.Console.ForegroundColor = log.Level switch
                {
                    "DEBUG" => ConsoleColor.White,
                    "WARNING" => ConsoleColor.Yellow,
                    "ERROR" => ConsoleColor.Red,
                    "FATAL" => ConsoleColor.Magenta,
                    "INFO" => ConsoleColor.Cyan,
                    "TRACE" => ConsoleColor.Green,
                    _ => ConsoleColor.White,
                };
                System.Console.WriteLine(log.ToString());
                System.Console.ResetColor();
            }

            return Task.CompletedTask;
        }

        #endregion Write
    }
}
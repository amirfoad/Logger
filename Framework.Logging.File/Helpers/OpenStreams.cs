﻿using System.Globalization;
using System.Text;

namespace Framework.Logging.Files.Helpers
{
    internal sealed class OpenStreams : IDisposable
    {
        private readonly FileLogConfig _config;
        private readonly Dictionary<DateTime, StreamWriter> _streams;
        private readonly Timer _timer;
        private readonly object _lock;

        internal OpenStreams(FileLogConfig config)
        {
            _config = config;
            _streams = new Dictionary<DateTime, StreamWriter>();
            _lock = new object();
            _timer = new Timer(ClosePastStreams, null, 0, (long)TimeSpan.FromHours(2).TotalMilliseconds);
        }

        public void Dispose()
        {
            _timer.Dispose();
            CloseAllStreams();
        }

        internal void Append(DateTime date, string content)
        {
            lock (_lock)
            {
                CreateStream(date.Date).WriteLine($"{content}[%f20]");
            }
        }

        internal string Read(DateTime date)
        {
            var hasKey = false;

            if (_streams.ContainsKey(date))
            {
                CloseStream(date);
                hasKey = true;
            }
            var filepath = CalculateFilePath(date);
            var result = string.Empty;

            lock (_lock)
            {
                if (File.Exists(filepath.FullPath))
                {
                    result = File.ReadAllText(filepath.FullPath, Encoding.UTF8);
                }
            }

            if (hasKey)
            {
                CreateStream(date);
            }

            const string start = "[";
            result = result.Replace("[%f20]", ",");
            const string end = "]";
            return $"{start}{result}{end}";
        }

        internal string Read(DateTime fromDate, DateTime toDate)
        {
            var hasKey = false;
            var result = string.Empty;

            DirectoryInfo info = new DirectoryInfo(string.IsNullOrEmpty(_config.Name) ? "Logs" : _config.Name);
            FileInfo[] files = info.GetFiles()
                .OrderBy(p => p.CreationTime)
                .Where(p => p.CreationTime >= fromDate && p.CreationTime <= toDate).ToArray();
            foreach (FileInfo file in files)
            {
                if (_streams.ContainsKey(file.CreationTime.Date))
                {
                    CloseStream(file.CreationTime.Date);
                    hasKey = true;
                }
                lock (_lock)
                {
                    if (File.Exists(CalculateFilePath(file.CreationTime).FullPath))
                    {
                        result += File.ReadAllText(CalculateFilePath(file.CreationTime).FullPath, Encoding.UTF8);
                    }
                }
                if (hasKey)
                {
                    CreateStream(file.CreationTime.Date);
                }
            }
            const string start = "[";
            result = result.Replace("[%f20]", ",");
            const string end = "]";
            return $"{start}{result}{end}";
        }

        internal string[] Filepaths() =>
            _streams.Values.Select(s => s.BaseStream).Cast<FileStream>().Select(s => s.Name).ToArray();

        private void ClosePastStreams(object ignored)
        {
            lock (_lock)
            {
                var today = DateTime.Today;
                var past = _streams.Where(kvp => kvp.Key < today).ToList();

                foreach (var kvp in past)
                {
                    kvp.Value.Dispose();
                    _streams.Remove(kvp.Key);
                }
            }
        }

        private void CloseAllStreams()
        {
            lock (_lock)
            {
                foreach (var stream in _streams.Values)
                    stream.Dispose();

                _streams.Clear();
            }
        }

        private StreamWriter CreateStream(DateTime date)
        {
            // Opening the stream if needed
            if (!_streams.ContainsKey(date))
            {
                // Building stream's filepath
                var filepath = CalculateFilePath(date);
                // Making sure the directory exists
                Directory.CreateDirectory(filepath.Directory);

                // Opening the stream
                var stream = new StreamWriter(
                    // https://stackoverflow.com/q/1862309
                    new FileStream(filepath.FullPath, FileMode.Append, FileAccess.Write, FileShare.Write, 4096, FileOptions.None)
                )
                {
                    AutoFlush = true
                };

                // Storing the created stream
                _streams[date] = stream;
            }

            return _streams[date];
        }

        private void CloseStream(DateTime date)
        {
            if (_streams.ContainsKey(date))
            {
                lock (_lock)
                {
                    _streams[date].Dispose();
                    _streams.Remove(date);
                }
            }
        }

        private FilePath CalculateFilePath(DateTime date)
        {
            var filename = "";
            var extension = "";
            var folderName = "";
            if (!string.IsNullOrEmpty(_config.Name))
            {
                extension = Path.GetExtension(_config.Name);
                filename = Path.GetFileNameWithoutExtension(_config.Name);
                folderName = Path.GetDirectoryName(_config.Name);
                if (string.IsNullOrEmpty(extension))
                    extension = ".log";
                if (string.IsNullOrEmpty(folderName))
                    folderName = "Logs";
            }
            else
            {
                folderName = "Logs";
                extension = ".log";
            }

            switch (_config.RollingInterval)
            {
                case RollingInterval.Day:
                    filename = $"{filename}{date.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)}{extension}";
                    break;

                case RollingInterval.Month:
                    filename = $"{filename}{date.ToString("yyyy-MM", CultureInfo.InvariantCulture)}{extension}";
                    break;

                case RollingInterval.Year:
                    filename = $"{filename}{date.ToString("yyyy", CultureInfo.InvariantCulture)}{extension}";
                    break;
            }
            return new FilePath
            {
                FullPath = AppDomain.CurrentDomain.BaseDirectory + "/" + folderName + "/" + filename,
                Directory = AppDomain.CurrentDomain.BaseDirectory + "/" + folderName
            };
        }
    }
}
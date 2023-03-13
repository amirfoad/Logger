using Framework.Core;
using Framework.DataAccess.Dapper;
using Framework.Logging.Common;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Framework.Logging.SqlServer
{
    public class SqlServerLogStore : ILogStore
    {
        public string Name { get; private set; }
        private bool Disposed { get; set; }
        private readonly LoggerContext _context;
        private readonly SqlServerLogConfig _dapperConfig;
        private readonly SqlConnection _sqlConnection;

        public SqlServerLogStore(LoggerContext context, string name,
            SqlServerLogConfig dapperConfig)
        {
            Argument.IsNotNull(() => context);
            _context = context;
            Name = name;
            _dapperConfig = (SqlServerLogConfig)context.LogConfigurations[Name];

            _sqlConnection = new SqlConnection(dapperConfig.SqlConnection);
        }

        public IList<Log<T>> Read<T>(DateTime date) where T : class
        {
            var _dataService = new DataService<Log<T>>(_sqlConnection);
            var result = _dataService.Find("LogDate  = @LogDate", new { LogDate = date }, false).ToList();
            return result;
        }

        public IList<Log<T>> Read<T>(DateTime date, string level) where T : class
        {
            return Read<T>(date).Where(c => c.Level == level).ToList();
        }

        public IList<Log<T>> Read<T>(string category, DateTime date) where T : class
        {
            return Read<T>(date).Where(c => c.Category == category).ToList();
        }

        public IList<Log<T>> Read<T>(DateTime fromDate, DateTime toDate) where T : class
        {
            return Read<T>(fromDate, toDate).ToList();
        }

        public IList<Log<T>> Read<T>(DateTime fromDate, DateTime toDate, string level) where T : class
        {
            return Read<T>(fromDate, toDate).Where(p => p.Level == level).ToList();
        }

        public void Write<T>(ILog<T> log) where T : class
        {
            var _dataService = new DataService<Log>(_sqlConnection);
            var message = JsonConvert.SerializeObject(log.Message);
            var data = new Log(log.Level.LevelParser(), log.Category)
            {
                LogDate = log.LogDate,
                Message = message,
                SectionId = log.SectionId,
                ServiceId = log.ServiceId,
                TraceId = log.TraceId
            };

            ///  var result = _dataService.Find("LogDate  = @LogDate", new { LogDate = date }, false).ToList();
            _dataService.Insert(data);
        }

        public async Task<IList<Log<T>>> ReadAsync<T>(DateTime date) where T : class
        {
            var _dataService = new DataService<Log<T>>(_sqlConnection);
            var result = (await _dataService.FindAsync("LogDate  = @LogDate", new { LogDate = date }, false)).ToList();
            return result;
        }

        public async Task<IList<Log<T>>> Read<T>(DateTime date, string level) where T : class
        {
            var data = await ReadAsync<T>(date);
            var res = data.Where(c => c.Level == level).ToList();
            return res;
        }

        public async Task<IList<Log<T>>> Read<T>(string category, DateTime date) where T : class
        {
            var data = await ReadAsync<T>(date);
            var res = data.Where(c => c.Category == category).ToList();
            return res;
        }

        public async Task<IList<Log<T>>> Read<T>(DateTime fromDate, DateTime toDate) where T : class
        {
            var res = await Read<T>(fromDate, toDate);
            return res.ToList();
        }

        public async Task<IList<Log<T>>> Read<T>(DateTime fromDate, DateTime toDate, string level) where T : class
        {
            var res = await Read<T>(fromDate, toDate);
            var result = res.Where(p => p.Level == level).ToList();
            return result;
        }

        public async Task Write<T>(ILog<T> log) where T : class
        {
            var _dataService = new DataService<Log>(_sqlConnection);
            var message = JsonConvert.SerializeObject(log.Message);
            var data = new Log(log.Level.LevelParser(), log.Category)
            {
                LogDate = log.LogDate,
                Message = message,
                SectionId = log.SectionId,
                ServiceId = log.ServiceId,
                TraceId = log.TraceId
            };

            ///  var result = _dataService.Find("LogDate  = @LogDate", new { LogDate = date }, false).ToList();
            await _dataService.InsertAsync(data);
        }


    }

}

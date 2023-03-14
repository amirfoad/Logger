using Framework.Core;
using Framework.DataAccess.Dapper;
using Framework.Logging.Common;
using Microsoft.Data.SqlClient;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
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

        public async Task<IList<Log<T>>> Read<T>(DateTime date) where T : class
        {
            var _dataService = new DataService<Log<T>>(_sqlConnection);
            var result =  _dataService.Find("LogDate  = @LogDate", new { LogDate = date }, false).ToList();
            return result;
        }

        public async  Task<IList<Log<T>>> Read<T>(DateTime date, string level) where T : class
        {
            return Read<T>(date).Result.Where(c => c.Level == level).ToList();
        }

        public async Task<IList<Log<T>>> Read<T>(string category, DateTime date) where T : class
        {
            return Read<T>(date).Result.Where(c => c.Category == category).ToList();
        }

        public async Task<IList<Log<T>>> Read<T>(DateTime fromDate, DateTime toDate) where T : class
        {
            return Read<T>(fromDate, toDate).Result.ToList();
        }

        public async Task<IList<Log<T>>> Read<T>(DateTime fromDate, DateTime toDate, string level) where T : class
        {
            return Read<T>(fromDate, toDate).Result .Where(p => p.Level == level).ToList();
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
            _dataService.Insert(data);
        }
    }

}

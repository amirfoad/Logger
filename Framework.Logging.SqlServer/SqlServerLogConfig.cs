using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Framework.Logging.SqlServer
{
    public class SqlServerLogConfig
    {
        /// <summary>
        /// Connection string for Connect to database
        /// </summary>
        public string SqlConnection { get; set; }

        public LogLevel? Level { get; set; } = null;
    }
}

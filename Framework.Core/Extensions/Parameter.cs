using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Framework.Core.Extensions
{
    public class Parameter
    {
        public Parameter(string key, object value, DbType? type = null)
        {
            Key = key;
            Value = value;
            Type = type ?? GetValueType(value);
        }

        public string Key { get; }
        public object Value { get; }
        public DbType? Type { get; }

        private static DbType? GetValueType(object value)
        {
            if (value is string)
            {
                return DbType.AnsiString;
            }

            return null; // Use SqlMapper DefaultTypes mapping
        }
    }
}

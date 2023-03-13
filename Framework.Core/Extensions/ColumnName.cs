using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Framework.Core.Extensions
{
    public interface IAttributeName
    {
        string GetName();
    }
    [AttributeUsage(AttributeTargets.Property)]
    public class ColumnName : Attribute, IAttributeName
    {
        public ColumnName(string name)
        {
            Name = name;
        }

        public string Name { get; }

        public string GetName()
        {
            return Name;
        }
    }
    [AttributeUsage(AttributeTargets.Class)]
    public class TableName : Attribute, IAttributeName
    {
        private string name;

        public TableName(string name)
        {
            this.name = name;
        }

        public string GetName()
        {
            return name;
        }
    }
}

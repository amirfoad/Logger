using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Framework.Core.Extensions
{
    public interface IEntityMap
    {
        void SetTableName(string tableName);

        Type Type();

        string GetTableName();

        string Name();
    }

    public interface IPropertyMap
    {
        void SetColumnName(string columnName);

        PropertyInfo Type();

        string GetColumnName();
    }
    public class Configuration
    {
        private static Configuration instance;
        private List<IEntityMap> entitiesMaps;
        private List<IPropertyMap> propertiesMaps;

        public static Configuration GetInstance()
        {
            return instance ?? (instance = new Configuration());
        }

        internal List<IEntityMap> Entities()
        {
            return entitiesMaps;
        }

        internal List<IPropertyMap> Properties()
        {
            return propertiesMaps;
        }

        internal void AddEntity(IEntityMap entity)
        {
            if (entitiesMaps == null)
                entitiesMaps = new List<IEntityMap>();
            entitiesMaps.Add(entity);
        }

        internal void AddProperty(IPropertyMap propertyMap)
        {
            if (propertiesMaps == null)
                propertiesMaps = new List<IPropertyMap>();
            propertiesMaps.Add(propertyMap);
        }
    }
}

using Framework.Core.Common;
using System.Reflection;

namespace Framework.Core.Extensions
{
    public static class ReflectionExtensions
    {
        public static IEnumerable<string> GetPublicPropertiesNames(this Type type, Func<PropertyInfo, bool> filterBy = null)
        {
            var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                                 .Where(x => type.Name.Contains("AnonymousType") ? x.CanRead : x.CanWrite && x.CanRead)
                                 .AsEnumerable();

            if (filterBy != null)
                properties = properties.Where(filterBy);

            return properties.Select(x => x.Name)
                             .OrderBy(x => x);
        }

        public static IEnumerable<PropertyInfo> GetPublicProperties(this Type type, Func<PropertyInfo, bool> filterBy = null)
        {
            var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                                 .Where(x => type.Name.Contains("AnonymousType") ? x.CanRead : x.CanWrite && x.CanRead)
                                 .AsEnumerable();

            if (filterBy != null)
                properties = properties.Where(filterBy);

            return properties.Select(x => x)
                             .OrderBy(x => x.Name);
        }

        public static IEnumerable<PropertyInfo> GetStringTypeProperties(this Type type, Func<PropertyInfo, bool> filterBy = null)
        {
            var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                                             .Where(x => type.Name.Contains("AnonymousType") ? x.CanRead && x.PropertyType == typeof(string) : x.CanWrite && x.CanRead && x.PropertyType == typeof(string))
                                             .AsEnumerable();

            if (filterBy != null)
                properties = properties.Where(filterBy);

            return properties.Select(x => x)
                             .OrderBy(x => x.Name);
        }

        public static object ProtectFarsiYeKeCorrection(this object obj)
        {
            var t = obj.GetType();
            if (t.IsGenericType &&
                (t.GetGenericTypeDefinition() == typeof(IList<>)
                || t.GetGenericTypeDefinition() == typeof(IEnumerable<>)
                || t.GetGenericTypeDefinition() == typeof(List<>)))
            {
                var result = ((IEnumerable<object>)obj).Select(x => x.ProtectFarsiYeKeCorrection());
                return result;
            }
            else
            {
                var propertyInfos = obj.GetType().GetStringTypeProperties();
                var propertyReflector = new PropertyReflector();

                //اعمال یکپارچگی نهایی
                foreach (var propertyInfo in propertyInfos)
                {
                    var propName = propertyInfo.Name;
                    var value = propertyReflector.GetValue(obj, propName);
                    if (value != null)
                    {
                        var strValue = value.ToString();
                        var newVal = strValue.ApplyFarsiYeKeCorrection();
                        if (newVal == strValue)
                        {
                            continue;
                        }
                        propertyReflector.SetValue(obj, propName, newVal);
                    }
                }
                return obj;
            }
        }

        public static Type GetBaseTypeEx(this Type type)
        {
            Argument.IsNotNull("type", type);
            return type.BaseType;
        }

        public static Type[] GetInterfacesEx(this Type type)
        {
            Argument.IsNotNull("type", type);
            return type.GetTypeInfo().ImplementedInterfaces.ToArray();
        }

        public static bool IsCOMObjectEx(this Type type)
        {
            Argument.IsNotNull("type", type);
            return type.IsCOMObject;
        }

        public static bool IsAssignableFromEx(this Type type, Type typeToCheck)
        {
            Argument.IsNotNull("type", type);
            Argument.IsNotNull("typeToCheck", typeToCheck);

            return type.GetTypeInfo().IsAssignableFrom(typeToCheck.GetTypeInfo());
        }
    }
}
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;

namespace PKHeX.Core
{
    public static class ReflectUtil
    {
        public static bool GetValueEquals(object obj, string propertyName, object value)
        {
            PropertyInfo pi = obj.GetType().GetTypeInfo().GetDeclaredProperty(propertyName);
            var v = pi.GetValue(obj, null);
            var c = ConvertValue(value, pi.PropertyType);
            return v.Equals(c);
        }
        public static void SetValue(object obj, string propertyName, object value)
        {
            PropertyInfo pi = obj.GetType().GetTypeInfo().GetDeclaredProperty(propertyName);
            pi.SetValue(obj, ConvertValue(value, pi.PropertyType), null);            
        }

        public static object GetValue(object obj, string propertyName)
        {
            PropertyInfo pi = obj.GetType().GetTypeInfo().GetDeclaredProperty(propertyName);
            return pi.GetValue(obj, null);
        }

        public static object GetValue(Type t, string propertyName) => t.GetTypeInfo().GetDeclaredProperty(propertyName).GetValue(null);
        public static void SetValue(Type t, string propertyName, object value) => t.GetTypeInfo().GetDeclaredProperty(propertyName).SetValue(null, value);

        public static IEnumerable<string> getPropertiesStartWithPrefix(Type type, string prefix)
        {
            return type.GetTypeInfo().DeclaredProperties
                .Where(p => p.Name.StartsWith(prefix, StringComparison.Ordinal))
                .Select(p => p.Name);
        }
        public static IEnumerable<string> getPropertiesCanWritePublic(Type type)
        {
            return type.GetTypeInfo().DeclaredProperties
                .Where(p => p.CanWrite && p.SetMethod.IsPublic)
                .Select(p => p.Name);
        }
        public static IEnumerable<string> getPropertiesCanWritePublicDeclared(Type type)
        {
            return getPropertiesCanWritePublic(type);
        }
        public static bool HasProperty(this Type type, string name)
        {
            return type.GetTypeInfo().GetDeclaredProperty(name) != null;
        }
        public static bool HasPropertyAll(this Type type, string name)
        {
            return type.GetTypeInfo().GetDeclaredProperty(name) != null;
        }

        private static object ConvertValue(object value, Type type)
        {
            if (type == typeof(DateTime?)) // Used for PKM.MetDate and other similar properties
            {
                DateTime dateValue;
                return DateTime.TryParseExact(value.ToString(), "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None, out dateValue) 
                    ? new DateTime?(dateValue) 
                    : null;
            }

            // Convert.ChangeType is suitable for most things
            return Convert.ChangeType(value, type);
        }
        public static bool? getBooleanState(object obj, string prop)
        {
            return obj.GetType().HasProperty(prop) ? GetValue(obj, prop) as bool? : null;
        }
    }
}

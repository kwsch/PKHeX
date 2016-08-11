using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;

namespace PKHeX
{
    public static partial class ReflectUtil
    {
        internal static bool GetValueEquals(object obj, string propertyName, object value)
        {
            PropertyInfo pi = obj.GetType().GetProperty(propertyName);
            var v = pi.GetValue(obj, null);
            var c = ConvertValue(value, pi.PropertyType);
            return v.Equals(c);
        }
        internal static void SetValue(object obj, string propertyName, object value)
        {
            PropertyInfo pi = obj.GetType().GetProperty(propertyName);
            pi.SetValue(obj, ConvertValue(value, pi.PropertyType), null);            
        }
        internal static object GetValue(object obj, string propertyName)
        {
            PropertyInfo pi = obj.GetType().GetProperty(propertyName);
            return pi.GetValue(obj, null);
        }
        internal static IEnumerable<string> getPropertiesStartWithPrefix(Type type, string prefix)
        {
            return type.GetProperties()
                .Where(p => p.Name.StartsWith(prefix))
                .Select(p => p.Name);
        }
        internal static IEnumerable<string> getPropertiesCanWritePublic(Type type)
        {
            return type.GetProperties().Where(p => p.CanWrite && p.GetSetMethod(nonPublic: true).IsPublic).Select(p => p.Name);
        }
        internal static bool HasProperty(this Type type, string name)
        {
            return type.GetProperties(BindingFlags.Public | BindingFlags.Instance).Any(p => p.Name == name);
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
    }
}

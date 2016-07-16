using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace PKHeX
{
    public static class ReflectUtil
    {
        internal static bool GetValueEquals(object obj, string propertyName, object value)
        {
            PropertyInfo pi = obj.GetType().GetProperty(propertyName);
            var v = pi.GetValue(obj, null);
            var c = Convert.ChangeType(value, pi.PropertyType);
            return v.Equals(c);
        }
        internal static void SetValue(object obj, string propertyName, object value)
        {
            PropertyInfo pi = obj.GetType().GetProperty(propertyName);
            pi.SetValue(obj, Convert.ChangeType(value, pi.PropertyType), null);
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
    }
}

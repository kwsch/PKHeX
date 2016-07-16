using System;
using System.Reflection;

namespace PKHeX
{
    public static class ReflectUtil
    {
        internal static bool GetValueEquals<T>(T obj, string propertyName, object value)
        {
            PropertyInfo pi = typeof(T).GetProperty(propertyName);
            var v = pi.GetValue(obj, null);
            var c = Convert.ChangeType(value, pi.PropertyType);
            return v.Equals(c);
        }
        internal static void SetValue<T>(T obj, string propertyName, object value)
        {
            PropertyInfo pi = typeof(T).GetProperty(propertyName);
            pi.SetValue(obj, Convert.ChangeType(value, pi.PropertyType), null);
        }
    }
}

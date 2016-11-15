using PKHeX.Reflection;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;

namespace PKHeX.Reflection
{
    public static partial class ReflectUtil
    {
        public static bool GetValueEquals(object obj, string propertyName, object value)
        {
            var pi = obj.GetType().GetProperty(propertyName, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
            var batchEditAttribute = GetAttributes(pi, typeof(BatchEditAttribute)).SingleOrDefault() as BatchEditAttribute;
            if (batchEditAttribute == null)
            {
                var v = pi.GetValue(obj, null);
                var c = ConvertValue(value, pi.PropertyType);
                return v.Equals(c);
            }
            else
            {
                return batchEditAttribute.IsValueEqual(obj, pi, value);
            }
        }
        public static void SetValue(object obj, string propertyName, object value)
        {
            var pi = obj.GetType().GetProperty(propertyName);
            var batchEditAttribute = GetAttributes(pi, typeof(BatchEditAttribute)).SingleOrDefault() as BatchEditAttribute;
            if (batchEditAttribute == null)
            {
                pi.SetValue(obj, ConvertValue(value, pi.PropertyType), null);
            }
            else
            {
                batchEditAttribute.SetValue(obj, pi, value);
            }
        }
        public static object GetValue(object obj, string propertyName)
        {
            var pi = obj.GetType().GetProperty(propertyName);
            var batchEditAttribute = GetAttributes(pi, typeof(BatchEditAttribute)).SingleOrDefault() as BatchEditAttribute;
            if (batchEditAttribute == null)
            {
                return pi.GetValue(obj, null);
            }
            else
            {
                return batchEditAttribute.GetValue(obj, pi);
            }
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
            return type.GetProperty(name, BindingFlags.Public | BindingFlags.Instance) != null;
        }
        internal static bool HasPropertyAll(this Type type, string name)
        {
            return type.GetProperty(name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance) != null;
        }

        internal static object ConvertValue(object value, Type type)
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
        internal static bool? getBooleanState(object obj, string prop)
        {
            return obj.GetType().HasProperty(prop) ? GetValue(obj, prop) as bool? : null;
        }
        
        internal static List<object> GetAttributes(PropertyInfo pi, Type attributeType)
        {
            var attributes = new List<object>();
            var parentType = pi.DeclaringType.BaseType;

            // Get the attributes for the current property
            attributes.AddRange(pi.GetCustomAttributes(attributeType, true));
            
            if (parentType != typeof(object))
            {
                var parentPI = parentType.GetProperty(pi.Name);
                if (parentPI != null)
                {
                    attributes.AddRange(GetAttributes(parentPI, attributeType));
                }                
            }

            return attributes;
        }
    }
}

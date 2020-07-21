using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;

namespace PKHeX.Core
{
    public static class ReflectUtil
    {
        public static bool IsValueEqual(this PropertyInfo pi, object obj, object value)
        {
            var v = pi.GetValue(obj, null);
            var c = ConvertValue(value, pi.PropertyType);
            return v.Equals(c);
        }

        public static void SetValue(PropertyInfo pi, object obj, object value)
        {
            var c = ConvertValue(value, pi.PropertyType);
            pi.SetValue(obj, c, null);
        }

        public static object? GetValue(object obj, string name) => GetPropertyInfo(obj.GetType().GetTypeInfo(), name)?.GetValue(obj);
        public static void SetValue(object obj, string name, object value) => GetPropertyInfo(obj.GetType().GetTypeInfo(), name)?.SetValue(obj, value, null);
        public static object GetValue(Type t, string propertyName) => t.GetTypeInfo().GetDeclaredProperty(propertyName).GetValue(null);
        public static void SetValue(Type t, string propertyName, object value) => t.GetTypeInfo().GetDeclaredProperty(propertyName).SetValue(null, value);

        public static IEnumerable<string> GetPropertiesStartWithPrefix(Type type, string prefix)
        {
            return type.GetTypeInfo().GetAllTypeInfo().SelectMany(GetAllProperties)
                .Where(p => p.Name.StartsWith(prefix, StringComparison.Ordinal))
                .Select(p => p.Name)
                .Distinct()
                ;
        }

        public static IEnumerable<string> GetPropertiesCanWritePublic(Type type)
        {
            return GetAllPropertyInfoCanWritePublic(type).Select(p => p.Name)
                    .Distinct()
                ;
        }

        public static IEnumerable<PropertyInfo> GetAllPropertyInfoCanWritePublic(Type type)
        {
            return type.GetTypeInfo().GetAllTypeInfo().SelectMany(GetAllProperties)
                            .Where(p => p.CanWrite && p.SetMethod.IsPublic);
        }

        public static IEnumerable<PropertyInfo> GetAllPropertyInfoPublic(Type type)
        {
            return type.GetTypeInfo().GetAllTypeInfo().SelectMany(GetAllProperties)
                .Where(p => (p.CanRead && p.GetMethod.IsPublic) || (p.CanWrite && p.SetMethod.IsPublic));
        }

        public static IEnumerable<string> GetPropertiesPublic(Type type)
        {
            return GetAllPropertyInfoPublic(type).Select(p => p.Name)
                    .Distinct()
                ;
        }

        public static IEnumerable<string> GetPropertiesCanWritePublicDeclared(Type type)
        {
            return type.GetTypeInfo().GetAllProperties()
                .Where(p => p.CanWrite && p.SetMethod.IsPublic)
                .Select(p => p.Name)
                    .Distinct()
                ;
        }

        private static object? ConvertValue(object value, Type type)
        {
            if (type == typeof(DateTime?)) // Used for PKM.MetDate and other similar properties
            {
                return DateTime.TryParseExact(value.ToString(), "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dateValue)
                    ? new DateTime?(dateValue)
                    : null;
            }

            // Convert.ChangeType is suitable for most things
            return Convert.ChangeType(value, type);
        }

        public static IEnumerable<ConstructorInfo> GetAllConstructors(this TypeInfo typeInfo)
            => GetAll(typeInfo, ti => ti.DeclaredConstructors);

        public static IEnumerable<EventInfo> GetAllEvents(this TypeInfo typeInfo)
            => GetAll(typeInfo, ti => ti.DeclaredEvents);

        public static IEnumerable<FieldInfo> GetAllFields(this TypeInfo typeInfo)
            => GetAll(typeInfo, ti => ti.DeclaredFields);

        public static IEnumerable<MemberInfo> GetAllMembers(this TypeInfo typeInfo)
            => GetAll(typeInfo, ti => ti.DeclaredMembers);

        public static IEnumerable<MethodInfo> GetAllMethods(this TypeInfo typeInfo)
            => GetAll(typeInfo, ti => ti.DeclaredMethods);

        public static IEnumerable<TypeInfo> GetAllNestedTypes(this TypeInfo typeInfo)
            => GetAll(typeInfo, ti => ti.DeclaredNestedTypes);

        public static IEnumerable<PropertyInfo> GetAllProperties(this TypeInfo typeInfo)
            => GetAll(typeInfo, ti => ti.DeclaredProperties);

        public static IEnumerable<TypeInfo> GetAllTypeInfo(this TypeInfo? typeInfo)
        {
            while (typeInfo != null)
            {
                yield return typeInfo;
                typeInfo = typeInfo.BaseType?.GetTypeInfo();
            }
        }

        public static bool HasProperty(object obj, string name, out PropertyInfo? pi) => (pi = GetPropertyInfo(obj.GetType().GetTypeInfo(), name)) != null;

        public static PropertyInfo? GetPropertyInfo(this TypeInfo typeInfo, string name)
        {
            return typeInfo.GetAllTypeInfo().Select(t => t.GetDeclaredProperty(name)).FirstOrDefault(pi => pi != null);
        }

        private static IEnumerable<T> GetAll<T>(this TypeInfo typeInfo, Func<TypeInfo, IEnumerable<T>> accessor)
        {
            return GetAllTypeInfo(typeInfo).SelectMany(_ => accessor(typeInfo));
        }

        public static Dictionary<T, string> GetAllConstantsOfType<T>(this Type type)
        {
            var fields = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.FlattenHierarchy);
            var consts = fields.Where(fi => fi.IsLiteral && !fi.IsInitOnly && fi.FieldType == typeof(T));
            return consts.ToDictionary(x => (T)x.GetRawConstantValue(), z => z.Name);
        }

        public static Dictionary<T, string> GetAllPropertiesOfType<T>(this Type type, object obj)
        {
            var props = type.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            var ofType = props.Where(fi => typeof(T).IsAssignableFrom(fi.PropertyType));
            return ofType.ToDictionary(x => (T)x.GetValue(obj), z => z.Name);
        }
    }
}

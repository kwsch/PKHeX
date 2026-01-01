using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Reflection;

namespace PKHeX.Core;

/// <summary>
/// Utility class for reflection.
/// </summary>
public static class ReflectUtil
{
    /// <summary>
    /// Fetches the requested property from <see cref="obj"/>, and compares it to <see cref="value"/>.
    /// </summary>
    /// <param name="pi">Property to fetch</param>
    /// <param name="obj">Object to fetch property from</param>
    /// <param name="value">Value to compare to</param>
    /// <returns>Comparison result</returns>
    public static int CompareTo(this PropertyInfo pi, object obj, object value)
    {
        var v = pi.GetValue(obj, null);
        var c = ConvertValue(value, pi.PropertyType);
        if (v is null)
            return 0;
        if (c is IComparable c1 && v is IComparable c2)
            return c2.CompareTo(c1);
        return 0;
    }

    public static void SetValue(PropertyInfo pi, object obj, object value)
    {
        var c = ConvertValue(value, pi.PropertyType);
        pi.SetValue(obj, c, null);
    }

    public static object? GetValue(object obj, string name)
    {
        if (obj.GetType().GetTypeInfo().TryGetPropertyInfo(name, out var pi))
            return pi.GetValue(obj, null);
        return null;
    }

    public static bool SetValue(object obj, string name, object value)
    {
        if (!obj.GetType().GetTypeInfo().TryGetPropertyInfo(name, out var pi))
            return false;
        if (!pi.CanWrite)
            return false;
        pi.SetValue(obj, value);
        return true;
    }

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
            .Where(CanWritePublic);
    }

    public static IEnumerable<PropertyInfo> GetAllPropertyInfoPublic(Type type)
    {
        return type.GetTypeInfo().GetAllTypeInfo().SelectMany(GetAllProperties)
            .Where(p => p.CanReadPublic() || p.CanWritePublic());
    }

    extension(PropertyInfo p)
    {
        private bool CanReadPublic() => p.CanRead && (p.GetMethod?.IsPublic ?? false);
        private bool CanWritePublic() => p.CanWrite && (p.SetMethod?.IsPublic ?? false);
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
                .Where(CanWritePublic)
                .Select(p => p.Name)
                .Distinct()
            ;
    }

    private static object? ConvertValue(object value, Type type)
    {
        if (type == typeof(DateOnly?)) // Used for PKM.MetDate and other similar properties
        {
            return DateOnly.TryParseExact(value.ToString(), "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateOnly dateValue)
                ? new DateOnly?(dateValue)
                : null;
        }

        if (type.IsEnum)
        {
            var str = value.ToString() ?? string.Empty;
            if (Enum.IsDefined(type, str) && int.TryParse(str, out var integer))
                return Convert.ChangeType(integer, type);
            return Enum.Parse(type, str, true);
        }
        // Convert.ChangeType is suitable for most things
        return Convert.ChangeType(value, type);
    }

    extension(TypeInfo typeInfo)
    {
        public IEnumerable<ConstructorInfo> GetAllConstructors() => typeInfo.GetAll(ti => ti.DeclaredConstructors);
        public IEnumerable<EventInfo> GetAllEvents() => typeInfo.GetAll(ti => ti.DeclaredEvents);
        public IEnumerable<FieldInfo> GetAllFields() => typeInfo.GetAll(ti => ti.DeclaredFields);
        public IEnumerable<MemberInfo> GetAllMembers() => typeInfo.GetAll(ti => ti.DeclaredMembers);
        public IEnumerable<MethodInfo> GetAllMethods() => typeInfo.GetAll(ti => ti.DeclaredMethods);
        public IEnumerable<TypeInfo> GetAllNestedTypes() => typeInfo.GetAll(ti => ti.DeclaredNestedTypes);
        public IEnumerable<PropertyInfo> GetAllProperties() => typeInfo.GetAll(ti => ti.DeclaredProperties);
    }

    public static IEnumerable<TypeInfo> GetAllTypeInfo(this TypeInfo? typeInfo)
    {
        while (typeInfo is not null)
        {
            yield return typeInfo;
            typeInfo = typeInfo.BaseType?.GetTypeInfo();
        }
    }

    /// <summary>
    /// Checks if the <see cref="obj"/> has the requested property <see cref="name"/>.
    /// </summary>
    /// <param name="obj">Object to check for property existence.</param>
    /// <param name="name">Name of the property.</param>
    /// <param name="pi">Reference to the property info for the object, if it exists.</param>
    /// <returns>True if it has property, and false if it does not have property. <see cref="pi"/> is null when returning false.</returns>
    public static bool HasProperty(object obj, string name, [NotNullWhen(true)] out PropertyInfo? pi)
    {
        var type = obj.GetType();
        return type.GetTypeInfo().TryGetPropertyInfo(name, out pi);
    }

    extension(TypeInfo typeInfo)
    {
        public bool TryGetPropertyInfo(string name, [NotNullWhen(true)] out PropertyInfo? pi)
        {
            foreach (var t in typeInfo.GetAllTypeInfo())
            {
                pi = t.GetDeclaredProperty(name);
                if (pi is not null)
                    return true;
                foreach (var i in t.ImplementedInterfaces)
                {
                    pi = i.GetTypeInfo().GetDeclaredProperty(name);
                    if (pi is not null)
                        return true;
                }
            }
            pi = null;
            return false;
        }

        private IEnumerable<T> GetAll<T>(Func<TypeInfo, IEnumerable<T>> accessor)
        {
            return typeInfo.GetAllTypeInfo().SelectMany(_ => accessor(typeInfo));
        }
    }

    extension(Type type)
    {
        public Dictionary<T, string> GetAllConstantsOfType<T>() where T : struct
        {
            var fields = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.FlattenHierarchy);
            var consts = fields.Where(fi => fi is { IsLiteral: true, IsInitOnly: false } && fi.FieldType == typeof(T));
            return consts.ToDictionary(z => (T)(z.GetRawConstantValue() ?? throw new NullReferenceException(nameof(z.Name))), z => z.Name);
        }

        public Dictionary<string, T> GetAllPropertiesOfType<T>(object obj) where T : class
        {
            var props = type.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            var result = new Dictionary<string, T>(props.Length);
            var requestType = typeof(T);
            foreach (var pi in props)
            {
                if (!requestType.IsAssignableFrom(pi.PropertyType))
                    continue;

                var name = pi.Name;
                var value = pi.GetValue(obj);
                if (value is not T t)
                    continue;
                result.TryAdd(name, t);
            }
            return result;
        }
    }
}

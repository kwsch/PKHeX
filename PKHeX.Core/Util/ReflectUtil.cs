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
        if (TryGetPropertyInfo(obj.GetType().GetTypeInfo(), name, out var pi))
            return pi.GetValue(obj, null);
        return default;
    }

    public static bool SetValue(object obj, string name, object value)
    {
        if (!TryGetPropertyInfo(obj.GetType().GetTypeInfo(), name, out var pi))
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

    private static bool CanReadPublic(this PropertyInfo p) => p.CanRead && (p.GetMethod?.IsPublic ?? false);
    private static bool CanWritePublic(this PropertyInfo p) => p.CanWrite && (p.SetMethod?.IsPublic ?? false);

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
        return TryGetPropertyInfo(type.GetTypeInfo(), name, out pi);
    }

    public static bool TryGetPropertyInfo(this TypeInfo typeInfo, string name, [NotNullWhen(true)] out PropertyInfo? pi)
    {
        foreach (var t in typeInfo.GetAllTypeInfo())
        {
            pi = t.GetDeclaredProperty(name);
            if (pi != null)
                return true;
            foreach (var i in t.ImplementedInterfaces)
            {
                pi = i.GetTypeInfo().GetDeclaredProperty(name);
                if (pi != null)
                    return true;
            }
        }
        pi = null;
        return false;
    }

    private static IEnumerable<T> GetAll<T>(this TypeInfo typeInfo, Func<TypeInfo, IEnumerable<T>> accessor)
    {
        return GetAllTypeInfo(typeInfo).SelectMany(_ => accessor(typeInfo));
    }

    public static Dictionary<T, string> GetAllConstantsOfType<T>(this Type type) where T : struct
    {
        var fields = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.FlattenHierarchy);
        var consts = fields.Where(fi => fi is { IsLiteral: true, IsInitOnly: false } && fi.FieldType == typeof(T));
        return consts.ToDictionary(z => (T)(z.GetRawConstantValue() ?? throw new NullReferenceException(nameof(z.Name))), z => z.Name);
    }

    public static Dictionary<string, T> GetAllPropertiesOfType<T>(this Type type, object obj) where T : class
    {
        var props = type.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly);
        var result = new Dictionary<string, T>(props.Length);
        foreach (var pi in props)
        {
            if (!typeof(T).IsAssignableFrom(pi.PropertyType))
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

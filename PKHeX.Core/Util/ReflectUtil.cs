using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Reflection;

namespace PKHeX.Core;

public static class ReflectUtil
{
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

    public static object? GetValue(object obj, string name) => GetPropertyInfo(obj.GetType().GetTypeInfo(), name)?.GetValue(obj);
    public static void SetValue(object obj, string name, object value) => GetPropertyInfo(obj.GetType().GetTypeInfo(), name)?.SetValue(obj, value, null);

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
            if (int.TryParse(str, out var integer))
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
    /// <returns>True if has property, and false if does not have property. <see cref="pi"/> is null when returning false.</returns>
    public static bool HasProperty(object obj, string name, [NotNullWhen(true)] out PropertyInfo? pi) => (pi = GetPropertyInfo(obj.GetType().GetTypeInfo(), name)) != null;

    public static PropertyInfo? GetPropertyInfo(this TypeInfo typeInfo, string name)
    {
        return typeInfo.GetAllTypeInfo().Select(t => t.GetDeclaredProperty(name)).FirstOrDefault(pi => pi != null);
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

    public static Dictionary<T, string> GetAllPropertiesOfType<T>(this Type type, object obj) where T : class
    {
        var props = type.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly);
        var ofType = props.Where(fi => typeof(T).IsAssignableFrom(fi.PropertyType));
        return ofType.ToDictionary(x => (T)(x.GetValue(obj) ?? throw new NullReferenceException(nameof(x.Name))), z => z.Name);
    }
}

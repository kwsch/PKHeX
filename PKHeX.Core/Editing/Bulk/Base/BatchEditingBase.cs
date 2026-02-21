using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Numerics;
using System.Reflection;

using static PKHeX.Core.BatchEditingUtil;

namespace PKHeX.Core;

/// <summary>
/// Base logic for editing entities with user provided <see cref="StringInstruction"/> list.
/// </summary>
/// <remarks>
/// Caches reflection results for the provided types, and provides utility methods for fetching properties and applying instructions.
/// </remarks>
public abstract class BatchEditingBase<TObject, TMeta> : IBatchEditor<TObject> where TObject : notnull
{
    private readonly Type[] _types;
    private readonly string[] _customProperties;
    private readonly Dictionary<string, PropertyInfo>.AlternateLookup<ReadOnlySpan<char>>[] _props;
    private readonly Lazy<string[][]> _properties;

    protected BatchEditingBase(Type[] types, string[] customProperties, int expectedMax)
    {
        _types = types;
        _customProperties = customProperties;
        _props = GetPropertyDictionaries(types, expectedMax);
        _properties = new Lazy<string[][]>(() => GetPropArray(_props, customProperties));
    }

    /// <summary>
    /// Property names, indexed by <see cref="Types"/>.
    /// </summary>
    public string[][] Properties => _properties.Value;

    /// <summary>
    /// Gets the list of supported entity types.
    /// </summary>
    public IReadOnlyList<Type> Types => _types;

    protected abstract TMeta CreateMeta(TObject entity);

    protected abstract bool ShouldModify(TObject entity);

    protected abstract bool TryHandleSetOperation(StringInstruction cmd, TMeta info, TObject entity, out ModifyResult result);

    protected abstract bool TryHandleFilter(StringInstruction cmd, TMeta info, TObject entity, out bool isMatch);

    /// <summary>
    /// Tries to fetch the entity property from the cache of available properties.
    /// </summary>
    public bool TryGetHasProperty(TObject entity, ReadOnlySpan<char> name, [NotNullWhen(true)] out PropertyInfo? pi)
        => TryGetHasProperty(entity.GetType(), name, out pi);

    /// <summary>
    /// Tries to fetch the entity property from the cache of available properties.
    /// </summary>
    public bool TryGetHasProperty(Type type, ReadOnlySpan<char> name, [NotNullWhen(true)] out PropertyInfo? pi)
    {
        var index = _types.IndexOf(type);
        if (index < 0)
        {
            pi = null;
            return false;
        }
        var localProps = _props[index];
        return localProps.TryGetValue(name, out pi);
    }

    /// <summary>
    /// Gets a list of entity types that implement the requested property.
    /// </summary>
    public IEnumerable<string> GetTypesImplementing(string property)
    {
        for (int i = 0; i < _types.Length; i++)
        {
            var type = _types[i];
            var localProps = _props[i];
            if (!localProps.TryGetValue(property, out var pi))
                continue;
            yield return $"{type.Name}: {pi.PropertyType.Name}";
        }
    }

    /// <summary>
    /// Gets the type of the entity property using the saved cache of properties.
    /// </summary>
    public bool TryGetPropertyType(string propertyName, [NotNullWhen(true)] out string? result, int typeIndex = 0)
    {
        if (_customProperties.Contains(propertyName))
        {
            result = "Custom";
            return true;
        }

        result = null;
        if (typeIndex == 0)
        {
            foreach (var p in _props)
            {
                if (!p.TryGetValue(propertyName, out var pi))
                    continue;
                result = pi.PropertyType.Name;
                return true;
            }
            return false;
        }

        int index = typeIndex - 1;
        if ((uint)index >= _props.Length)
            index = 0;
        var pr = _props[index];
        if (!pr.TryGetValue(propertyName, out var info))
            return false;
        result = info.PropertyType.Name;
        return true;
    }

    /// <summary>
    /// Checks if the entity is filtered by the provided filters.
    /// </summary>
    public bool IsFilterMatch(IEnumerable<StringInstruction> filters, TObject entity)
    {
        var info = CreateMeta(entity);
        var localProps = GetProps(entity);
        foreach (var filter in filters)
        {
            if (!IsFilterMatch(filter, info, entity, localProps))
                return false;
        }
        return true;
    }

    /// <summary>
    /// Tries to modify the entity.
    /// </summary>
    public bool TryModifyIsSuccess(TObject entity, IEnumerable<StringInstruction> filters, IEnumerable<StringInstruction> modifications, Func<TObject, bool>? modifier = null)
        => TryModify(entity, filters, modifications, modifier) is ModifyResult.Modified;

    /// <summary>
    /// Tries to modify the entity using instructions and a custom modifier delegate.
    /// </summary>
    public ModifyResult TryModify(TObject entity, IEnumerable<StringInstruction> filters, IEnumerable<StringInstruction> modifications, Func<TObject, bool>? modifier = null)
    {
        if (!ShouldModify(entity))
            return ModifyResult.Skipped;

        var info = CreateMeta(entity);
        var localProps = GetProps(entity);

        foreach (var cmd in filters)
        {
            try
            {
                if (!IsFilterMatch(cmd, info, entity, localProps))
                    return ModifyResult.Filtered;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return ModifyResult.Error;
            }
        }

        var error = false;
        var result = ModifyResult.Skipped;

        if (modifier is { } func)
        {
            try
            {
                if (!func(entity))
                    return ModifyResult.Skipped;
                result = ModifyResult.Modified;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return ModifyResult.Error;
            }
        }

        foreach (var cmd in modifications)
        {
            try
            {
                var tmp = SetProperty(cmd, entity, info, localProps);
                if (tmp == ModifyResult.Error)
                    error = true;
                else if (tmp != ModifyResult.Skipped)
                    result = tmp;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                error = true;
            }
        }
        if (error)
            result |= ModifyResult.Error;
        return result;
    }

    private static Dictionary<string, PropertyInfo>.AlternateLookup<ReadOnlySpan<char>>[] GetPropertyDictionaries(IReadOnlyList<Type> types, int expectedMax)
    {
        var result = new Dictionary<string, PropertyInfo>.AlternateLookup<ReadOnlySpan<char>>[types.Count];
        for (int i = 0; i < types.Count; i++)
            result[i] = GetPropertyDictionary(types[i], ReflectUtil.GetAllPropertyInfoPublic, expectedMax).GetAlternateLookup<ReadOnlySpan<char>>();
        return result;
    }

    private static Dictionary<string, PropertyInfo> GetPropertyDictionary(Type type, Func<Type, IEnumerable<PropertyInfo>> selector, int expectedMax)
    {
        var dict = new Dictionary<string, PropertyInfo>(expectedMax);
        var localProps = selector(type);
        foreach (var p in localProps)
            dict.TryAdd(p.Name, p);
        return dict;
    }

    private static string[][] GetPropArray<T>(Dictionary<string, T>.AlternateLookup<ReadOnlySpan<char>>[] types, ReadOnlySpan<string> extra)
    {
        var result = new string[types.Length + 2][];
        var p = result.AsSpan(1, types.Length);

        for (int i = 0; i < p.Length; i++)
        {
            var type = types[i].Dictionary;
            string[] combine = [..type.Keys, ..extra];
            combine.Sort();
            p[i] = combine;
        }

        var first = p[0];
        var any = new HashSet<string>(first);
        var all = new HashSet<string>(first);
        foreach (var set in p[1..])
        {
            any.UnionWith(set);
            all.IntersectWith(set);
        }

        var arrAny = any.ToArray();
        arrAny.Sort();
        result[0] = arrAny;

        var arrAll = all.ToArray();
        arrAll.Sort();
        result[^1] = arrAll;

        return result;
    }

    private Dictionary<string, PropertyInfo>.AlternateLookup<ReadOnlySpan<char>> GetProps(TObject entity)
    {
        var type = entity.GetType();
        var typeIndex = _types.IndexOf(type);
        return _props[typeIndex];
    }

    private bool IsFilterMatch(StringInstruction cmd, TMeta info, TObject entity, Dictionary<string, PropertyInfo>.AlternateLookup<ReadOnlySpan<char>> localProps)
    {
        if (TryHandleFilter(cmd, info, entity, out var isMatch))
            return isMatch;
        return IsPropertyFiltered(cmd, entity, localProps);
    }

    private static bool IsPropertyFiltered(StringInstruction cmd, TObject entity, Dictionary<string, PropertyInfo>.AlternateLookup<ReadOnlySpan<char>> localProps)
    {
        if (!localProps.TryGetValue(cmd.PropertyName, out var pi))
            return false;
        if (!pi.CanRead)
            return false;

        var val = cmd.PropertyValue;
        if (val.StartsWith(PointerToken) && localProps.TryGetValue(val.AsSpan(1), out var opi))
        {
            var result = opi.GetValue(entity) ?? throw new NullReferenceException();
            return cmd.Comparer.IsCompareOperator(pi.CompareTo(entity, result));
        }
        return cmd.Comparer.IsCompareOperator(pi.CompareTo(entity, val));
    }

    private ModifyResult SetProperty(StringInstruction cmd, TObject entity, TMeta info, Dictionary<string, PropertyInfo>.AlternateLookup<ReadOnlySpan<char>> localProps)
    {
        if (cmd.Operation == InstructionOperation.Set && TryHandleSetOperation(cmd, info, entity, out var result))
            return result;

        if (!localProps.TryGetValue(cmd.PropertyName, out var pi))
            return ModifyResult.Error;

        if (!pi.CanWrite)
            return ModifyResult.Error;

        if (cmd.Operation != InstructionOperation.Set)
            return ApplyNumericOperation(entity, cmd, pi, localProps);

        if (!TryResolveOperandValue(cmd, entity, localProps, out var value))
            return ModifyResult.Error;

        ReflectUtil.SetValue(pi, entity, value);
        return ModifyResult.Modified;
    }

    private static ModifyResult ApplyNumericOperation(TObject entity, StringInstruction cmd, PropertyInfo pi, Dictionary<string, PropertyInfo>.AlternateLookup<ReadOnlySpan<char>> localProps)
    {
        if (!pi.CanRead)
            return ModifyResult.Error;

        if (!TryGetNumericType(pi.PropertyType, out var numericType))
            return ModifyResult.Error;

        var currentValue = pi.GetValue(entity);
        if (currentValue is null)
            return ModifyResult.Error;

        if (!TryResolveOperandValue(cmd, entity, localProps, out var operandValue))
            return ModifyResult.Error;

        if (!TryApplyNumericOperation(numericType, cmd.Operation, currentValue, operandValue, out var value))
            return ModifyResult.Error;

        ReflectUtil.SetValue(pi, entity, value);
        return ModifyResult.Modified;
    }

    private static bool TryResolveOperandValue(StringInstruction cmd, TObject entity, Dictionary<string, PropertyInfo>.AlternateLookup<ReadOnlySpan<char>> localProps, [NotNullWhen(true)] out object? value)
    {
        if (cmd.Random)
        {
            value = cmd.RandomValue;
            return true;
        }

        var propertyValue = cmd.PropertyValue;
        if (propertyValue.StartsWith(PointerToken) && localProps.TryGetValue(propertyValue.AsSpan(1), out var opi))
        {
            value = opi.GetValue(entity);
            return value is not null;
        }

        value = propertyValue;
        return true;
    }

    private static bool TryGetNumericType(Type type, out Type numericType)
    {
        numericType = Nullable.GetUnderlyingType(type) ?? type;
        // bool isNullable = type != numericType;
        return numericType.GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(INumber<>));
    }

    private static bool TryApplyNumericOperation(Type numericType, InstructionOperation operation, object currentValue, object operandValue, [NotNullWhen(true)] out object? result)
    {
        result = null;
        if (numericType == typeof(byte))
            return ApplyBinaryInteger<byte>(currentValue, operandValue, operation, out result);
        if (numericType == typeof(sbyte))
            return ApplyBinaryInteger<sbyte>(currentValue, operandValue, operation, out result);
        if (numericType == typeof(short))
            return ApplyBinaryInteger<short>(currentValue, operandValue, operation, out result);
        if (numericType == typeof(ushort))
            return ApplyBinaryInteger<ushort>(currentValue, operandValue, operation, out result);
        if (numericType == typeof(int))
            return ApplyBinaryInteger<int>(currentValue, operandValue, operation, out result);
        if (numericType == typeof(uint))
            return ApplyBinaryInteger<uint>(currentValue, operandValue, operation, out result);
        if (numericType == typeof(long))
            return ApplyBinaryInteger<long>(currentValue, operandValue, operation, out result);
        if (numericType == typeof(ulong))
            return ApplyBinaryInteger<ulong>(currentValue, operandValue, operation, out result);
        if (numericType == typeof(nint))
            return ApplyBinaryInteger<nint>(currentValue, operandValue, operation, out result);
        if (numericType == typeof(nuint))
            return ApplyBinaryInteger<nuint>(currentValue, operandValue, operation, out result);
        if (numericType == typeof(BigInteger))
            return ApplyBinaryInteger<BigInteger>(currentValue, operandValue, operation, out result);
        if (numericType == typeof(float))
            return ApplyNumeric<float>(currentValue, operandValue, operation, out result);
        if (numericType == typeof(double))
            return ApplyNumeric<double>(currentValue, operandValue, operation, out result);
        if (numericType == typeof(decimal))
            return ApplyNumeric<decimal>(currentValue, operandValue, operation, out result);
        return false;
    }

    private static bool ApplyNumeric<T>(object currentValue, object operandValue, InstructionOperation operation, [NotNullWhen(true)] out object? result)
        where T : INumber<T>
    {
        if (operation.IsBitwise)
        {
            result = null;
            return false;
        }

        var success = TryApplyNumericOperationCore<T>(operation, currentValue, operandValue, out var typed);
        result = typed;
        return success;
    }

    private static bool ApplyBinaryInteger<T>(object currentValue, object operandValue, InstructionOperation operation, [NotNullWhen(true)] out object? result)
        where T : IBinaryInteger<T>
    {
        var success = operation.IsBitwise
            ? TryApplyBinaryIntegerOperationCore<T>(operation, currentValue, operandValue, out var typed)
            : TryApplyNumericOperationCore(operation, currentValue, operandValue, out typed);
        result = typed;
        return success;
    }

    private static bool TryApplyNumericOperationCore<T>(InstructionOperation operation, object currentValue, object operandValue, [NotNullWhen(true)] out T? result)
        where T : INumber<T>
    {
        if (!TryConvertNumeric<T>(currentValue, out var left) || !TryConvertNumeric<T>(operandValue, out var right))
        {
            result = default;
            return false;
        }

        return TryApplyNumericOperationCore(operation, left, right, out result);
    }

    private static bool TryApplyNumericOperationCore<T>(InstructionOperation operation, T left, T right, [NotNullWhen(true)] out T? result)
        where T : INumber<T>
    {
        try
        {
            result = operation switch
            {
                InstructionOperation.Add => left + right,
                InstructionOperation.Subtract => left - right,
                InstructionOperation.Multiply => left * right,
                InstructionOperation.Divide => left / right,
                InstructionOperation.Modulo => left % right,
                _ => right,
            };
            return true;
        }
        catch (DivideByZeroException)
        {
            result = default;
            return false;
        }
    }

    private static bool TryApplyBinaryIntegerOperationCore<T>(InstructionOperation operation, object currentValue, object operandValue, [NotNullWhen(true)] out T? result)
        where T : IBinaryInteger<T>
    {
        if (!TryConvertNumeric<T>(currentValue, out var left) || !TryConvertNumeric<T>(operandValue, out var right))
        {
            result = default;
            return false;
        }

        return TryApplyBinaryIntegerOperationCore(operation, left, right, out result);
    }

    private static bool TryApplyBinaryIntegerOperationCore<T>(InstructionOperation operation, T left, T right, [NotNullWhen(true)] out T? result)
        where T : IBinaryInteger<T>
    {
        try
        {
            switch (operation)
            {
                case InstructionOperation.BitwiseAnd:
                    result = left & right;
                    return true;
                case InstructionOperation.BitwiseOr:
                    result = left | right;
                    return true;
                case InstructionOperation.BitwiseXor:
                    result = left ^ right;
                    return true;
                case InstructionOperation.BitwiseShiftLeft:
                    result = left << int.CreateChecked(right);
                    return true;
                case InstructionOperation.BitwiseShiftRight:
                    result = left >> int.CreateChecked(right);
                    return true;
                default:
                    result = default;
                    return false;
            }
        }
        catch (OverflowException)
        {
            result = default;
            return false;
        }
    }

    private static bool TryConvertNumeric<T>(object value, [NotNullWhen(true)] out T? result) where T : INumber<T>
    {
        if (value is T typed)
        {
            result = typed;
            return true;
        }

        if (value is string text)
        {
            if (T.TryParse(text, CultureInfo.InvariantCulture, out var parsed))
            {
                result = parsed;
                return true;
            }
            result = default;
            return false;
        }

        if (value is IConvertible)
        {
            try
            {
                var converted = Convert.ChangeType(value, typeof(T), CultureInfo.InvariantCulture);
                if (converted is T convertedValue)
                {
                    result = convertedValue;
                    return true;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        result = default;
        return false;
    }
}

using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace PKHeX.Core;

public static class BatchEditingUtil
{
    public const string PROP_TYPENAME = "ObjectType";
    public const char PointerToken = '*';

    /// <summary>
    /// Checks if the object is filtered by the provided <see cref="filters"/>.
    /// </summary>
    /// <remarks>
    /// Does not use cached reflection; less performant than a cached <see cref="BatchEditingBase{TObject,TMeta}"/> implementation.
    /// </remarks>
    /// <param name="filters">Filters which must be satisfied.</param>
    /// <param name="obj">Object to check.</param>
    /// <returns>True if <see cref="obj"/> matches all filters.</returns>
    public static bool IsFilterMatch<T>(IEnumerable<StringInstruction> filters, T obj) where T : notnull
    {
        foreach (var cmd in filters)
        {
            var name = cmd.PropertyName;
            var value = cmd.PropertyValue;
            if (name is PROP_TYPENAME)
            {
                var type = obj.GetType();
                var typeName = type.Name;
                if (!cmd.Comparer.IsCompareEquivalence(value == typeName))
                    return false;
                continue;
            }

            if (!ReflectUtil.HasProperty(obj, name, out var pi))
                return false;
            try
            {
                if (cmd.Comparer.IsCompareOperator(pi.CompareTo(obj, value)))
                    continue;
            }
            // User provided inputs can mismatch the type's required value format, and fail to be compared.
            catch (Exception e)
            {
                Debug.WriteLine($"Unable to compare {name} to {value}.");
                Debug.WriteLine(e.Message);
            }
            return false;
        }
        return true;
    }
}

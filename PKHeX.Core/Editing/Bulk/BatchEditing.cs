using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;

using static PKHeX.Core.MessageStrings;
using static PKHeX.Core.BatchModifications;

namespace PKHeX.Core;

/// <summary>
/// Logic for editing many <see cref="PKM"/> with user provided <see cref="StringInstruction"/> list.
/// </summary>
public static class BatchEditing
{
    public static readonly Type[] Types =
    [
        typeof (PK9),
        typeof (PK8), typeof (PA8), typeof (PB8),
        typeof (PB7),
        typeof (PK7), typeof (PK6), typeof (PK5), typeof (PK4), typeof(BK4), typeof(RK4),
        typeof (PK3), typeof (XK3), typeof (CK3),
        typeof (PK2), typeof (SK2), typeof (PK1),
    ];

    /// <summary>
    /// Extra properties to show in the list of selectable properties (GUI)
    /// </summary>
    public static readonly List<string> CustomProperties =
    [
        PROP_LEGAL, PROP_TYPENAME, PROP_RIBBONS, PROP_CONTESTSTATS, PROP_MOVEMASTERY,
        PROP_TYPE1, PROP_TYPE2, PROP_TYPEEITHER,
        IdentifierContains, nameof(ISlotInfo.Slot), nameof(SlotInfoBox.Box),
    ];

    /// <summary>
    /// Property names, indexed by <see cref="Types"/>.
    /// </summary>
    public static string[][] Properties => GetProperties.Value;

    private static readonly Lazy<string[][]> GetProperties = new(() => GetPropArray(Types, CustomProperties));

    private static readonly Dictionary<string, PropertyInfo>[] Props = GetPropertyDictionaries(Types);

    private static Dictionary<string, PropertyInfo>[] GetPropertyDictionaries(IReadOnlyList<Type> types)
    {
        var result = new Dictionary<string, PropertyInfo>[types.Count];
        for (int i = 0; i < types.Count; i++)
            result[i] = GetPropertyDictionary(types[i], ReflectUtil.GetAllPropertyInfoPublic);
        return result;
    }

    private static Dictionary<string, PropertyInfo> GetPropertyDictionary(Type type, Func<Type, IEnumerable<PropertyInfo>> selector)
    {
        const int expectedMax = 0x200; // currently 0x160 as of 2022
        var dict = new Dictionary<string, PropertyInfo>(expectedMax);
        var props = selector(type);
        foreach (var p in props)
            dict.TryAdd(p.Name, p);
        return dict;
    }

    internal const string CONST_RAND = "$rand";
    internal const string CONST_SHINY = "$shiny";
    internal const string CONST_SUGGEST = "$suggest";
    private const string CONST_BYTES = "$[]";
    private const char CONST_POINTER = '*';
    internal const char CONST_SPECIAL = '$';

    internal const string PROP_LEGAL = "Legal";
    internal const string PROP_TYPENAME = "ObjectType";
    internal const string PROP_TYPEEITHER = "HasType";
    internal const string PROP_TYPE1 = "PersonalType1";
    internal const string PROP_TYPE2 = "PersonalType2";
    internal const string PROP_RIBBONS = "Ribbons";
    internal const string PROP_EVS = "EVs";
    internal const string PROP_CONTESTSTATS = "ContestStats";
    internal const string PROP_MOVEMASTERY = "MoveMastery";
    internal const string IdentifierContains = nameof(IdentifierContains);

    private static string[][] GetPropArray(IReadOnlyList<Type> types, IReadOnlyList<string> extra)
    {
        var result = new string[types.Count + 2][];
        var p = result.AsSpan(1, types.Count);

        for (int i = 0; i < p.Length; i++)
        {
            var type = types[i];
            var props = ReflectUtil.GetPropertiesPublic(type);
            var combine = props.Concat(extra).ToArray();
            Array.Sort(combine);
            p[i] = combine;
        }

        // Properties for any PKM
        // Properties shared by all PKM
        var first = p[0];
        var any = new HashSet<string>(first);
        var all = new HashSet<string>(first);
        foreach (var set in p[1..])
        {
            any.UnionWith(set);
            all.IntersectWith(set);
        }

        var arrAny = any.ToArray();
        Array.Sort(arrAny);
        result[0] = arrAny;

        var arrAll = all.ToArray();
        Array.Sort(arrAll);
        result[^1] = arrAll;

        return result;
    }

    /// <summary>
    /// Tries to fetch the <see cref="PKM"/> property from the cache of available properties.
    /// </summary>
    /// <param name="pk">Pokémon to check</param>
    /// <param name="name">Property Name to check</param>
    /// <param name="pi">Property Info retrieved (if any).</param>
    /// <returns>True if it has property, false if it does not.</returns>
    public static bool TryGetHasProperty(PKM pk, string name, [NotNullWhen(true)] out PropertyInfo? pi)
    {
        var type = pk.GetType();
        return TryGetHasProperty(type, name, out pi);
    }

    /// <summary>
    /// Tries to fetch the <see cref="PKM"/> property from the cache of available properties.
    /// </summary>
    /// <param name="type">Type to check</param>
    /// <param name="name">Property Name to check</param>
    /// <param name="pi">Property Info retrieved (if any).</param>
    /// <returns>True if it has property, false if it does not.</returns>
    public static bool TryGetHasProperty(Type type, string name, [NotNullWhen(true)] out PropertyInfo? pi)
    {
        var index = Array.IndexOf(Types, type);
        if (index < 0)
        {
            pi = null;
            return false;
        }
        var props = Props[index];
        return props.TryGetValue(name, out pi);
    }

    /// <summary>
    /// Gets a list of <see cref="PKM"/> types that implement the requested <see cref="property"/>.
    /// </summary>
    public static IEnumerable<string> GetTypesImplementing(string property)
    {
        for (int i = 0; i < Types.Length; i++)
        {
            var type = Types[i];
            var props = Props[i];
            if (!props.TryGetValue(property, out var pi))
                continue;
            yield return $"{type.Name}: {pi.PropertyType.Name}";
        }
    }

    /// <summary>
    /// Gets the type of the <see cref="PKM"/> property using the saved cache of properties.
    /// </summary>
    /// <param name="propertyName">Property Name to fetch the type for</param>
    /// <param name="typeIndex">Type index (within <see cref="Types"/>). Leave empty (0) for a nonspecific format.</param>
    /// <returns>Short name of the property's type.</returns>
    public static string? GetPropertyType(string propertyName, int typeIndex = 0)
    {
        if (CustomProperties.Contains(propertyName))
            return "Custom";

        if (typeIndex == 0) // Any
        {
            foreach (var p in Props)
            {
                if (p.TryGetValue(propertyName, out var pi))
                    return pi.PropertyType.Name;
            }
            return null;
        }

        int index = typeIndex - 1;
        if ((uint)index >= Props.Length)
            index = 0; // All vs Specific
        var pr = Props[index];
        if (!pr.TryGetValue(propertyName, out var info))
            return null;
        return info.PropertyType.Name;
    }

    /// <summary>
    /// Initializes the <see cref="StringInstruction"/> list with a context-sensitive value. If the provided value is a string, it will attempt to convert that string to its corresponding index.
    /// </summary>
    /// <param name="il">Instructions to initialize.</param>
    public static void ScreenStrings(IEnumerable<StringInstruction> il)
    {
        foreach (var i in il)
        {
            var pv = i.PropertyValue;
            if (pv.All(char.IsDigit))
                continue;

            if (pv.StartsWith(CONST_SPECIAL) && !pv.StartsWith(CONST_BYTES, StringComparison.Ordinal))
            {
                var str = pv.AsSpan(1);
                if (StringInstruction.IsRandomRange(str))
                {
                    i.SetRandomRange(str);
                    continue;
                }
            }

            SetInstructionScreenedValue(i);
        }
    }

    /// <summary>
    /// Initializes the <see cref="StringInstruction"/> with a context-sensitive value. If the provided value is a string, it will attempt to convert that string to its corresponding index.
    /// </summary>
    /// <param name="i">Instruction to initialize.</param>
    private static void SetInstructionScreenedValue(StringInstruction i)
    {
        switch (i.PropertyName)
        {
            case nameof(PKM.Species): i.SetScreenedValue(GameInfo.Strings.specieslist); return;
            case nameof(PKM.HeldItem): i.SetScreenedValue(GameInfo.Strings.itemlist); return;
            case nameof(PKM.Ability): i.SetScreenedValue(GameInfo.Strings.abilitylist); return;
            case nameof(PKM.Nature): i.SetScreenedValue(GameInfo.Strings.natures); return;
            case nameof(PKM.Ball): i.SetScreenedValue(GameInfo.Strings.balllist); return;

            case nameof(PKM.Move1) or nameof(PKM.Move2) or nameof(PKM.Move3) or nameof(PKM.Move4):
            case nameof(PKM.RelearnMove1) or nameof(PKM.RelearnMove2) or nameof(PKM.RelearnMove3) or nameof(PKM.RelearnMove4):
                i.SetScreenedValue(GameInfo.Strings.movelist); return;
        }
    }

    private static Dictionary<string, PropertyInfo> GetProps(PKM pk)
    {
        var type = pk.GetType();
        var typeIndex = Array.IndexOf(Types, type);
        return Props[typeIndex];
    }

    /// <summary>
    /// Checks if the object is filtered by the provided <see cref="filters"/>.
    /// </summary>
    /// <param name="filters">Filters which must be satisfied.</param>
    /// <param name="pk">Object to check.</param>
    /// <returns>True if <see cref="pk"/> matches all filters.</returns>
    public static bool IsFilterMatch(IEnumerable<StringInstruction> filters, PKM pk)
    {
        var props = GetProps(pk);
        foreach (var filter in filters)
        {
            if (!IsFilterMatch(filter, pk, props))
                return false;
        }
        return true;
    }

    /// <summary>
    /// Checks if the object is filtered by the provided <see cref="filters"/>.
    /// </summary>
    /// <param name="filters">Filters which must be satisfied.</param>
    /// <param name="pk">Object to check.</param>
    /// <returns>True if <see cref="pk"/> matches all filters.</returns>
    public static bool IsFilterMatchMeta(IEnumerable<StringInstruction> filters, SlotCache pk)
    {
        foreach (var i in filters)
        {
            foreach (var filter in BatchFilters.FilterMeta)
            {
                if (!filter.IsMatch(i.PropertyName))
                    continue;

                if (!filter.IsFiltered(pk, i))
                    return false;

                break;
            }
        }
        return true;
    }

    /// <summary>
    /// Checks if the object is filtered by the provided <see cref="filters"/>.
    /// </summary>
    /// <param name="filters">Filters which must be satisfied.</param>
    /// <param name="obj">Object to check.</param>
    /// <returns>True if <see cref="obj"/> matches all filters.</returns>
    public static bool IsFilterMatch(IEnumerable<StringInstruction> filters, object obj)
    {
        foreach (var cmd in filters)
        {
            if (cmd.PropertyName is PROP_TYPENAME)
            {
                var type = obj.GetType();
                var typeName = type.Name;
                if (!cmd.Comparer.IsCompareEquivalence(cmd.PropertyValue == typeName))
                    return false;
                continue;
            }

            if (!ReflectUtil.HasProperty(obj, cmd.PropertyName, out var pi))
                return false;
            try
            {
                if (cmd.Comparer.IsCompareOperator(pi.CompareTo(obj, cmd.PropertyValue)))
                    continue;
            }
            // User provided inputs can mismatch the type's required value format, and fail to be compared.
            catch (Exception e)
            {
                Debug.WriteLine($"Unable to compare {cmd.PropertyName} to {cmd.PropertyValue}.");
                Debug.WriteLine(e.Message);
            }
            return false;
        }
        return true;
    }

    /// <summary>
    /// Tries to modify the <see cref="PKM"/>.
    /// </summary>
    /// <param name="pk">Object to modify.</param>
    /// <param name="filters">Filters which must be satisfied prior to any modifications being made.</param>
    /// <param name="modifications">Modifications to perform on the <see cref="pk"/>.</param>
    /// <returns>Result of the attempted modification.</returns>
    public static bool TryModify(PKM pk, IEnumerable<StringInstruction> filters, IEnumerable<StringInstruction> modifications)
    {
        var result = TryModifyPKM(pk, filters, modifications);
        return result == ModifyResult.Modified;
    }

    /// <summary>
    /// Tries to modify the <see cref="BatchInfo"/>.
    /// </summary>
    /// <param name="pk">Command Filter</param>
    /// <param name="filters">Filters which must be satisfied prior to any modifications being made.</param>
    /// <param name="modifications">Modifications to perform on the <see cref="pk"/>.</param>
    /// <returns>Result of the attempted modification.</returns>
    internal static ModifyResult TryModifyPKM(PKM pk, IEnumerable<StringInstruction> filters, IEnumerable<StringInstruction> modifications)
    {
        if (!pk.ChecksumValid || pk.Species == 0)
            return ModifyResult.Invalid;

        var info = new BatchInfo(pk);
        var props = GetProps(pk);
        foreach (var cmd in filters)
        {
            try
            {
                if (!IsFilterMatch(cmd, info, props))
                    return ModifyResult.Filtered;
            }
            // Swallow any error because this can be malformed user input.
            catch (Exception ex)
            {
                Debug.WriteLine(MsgBEModifyFailCompare + " " + ex.Message, cmd.PropertyName, cmd.PropertyValue);
                return ModifyResult.Error;
            }
        }

        ModifyResult result = ModifyResult.Modified;
        foreach (var cmd in modifications)
        {
            try
            {
                var tmp = SetPKMProperty(cmd, info, props);
                if (tmp != ModifyResult.Modified)
                    result = tmp;
            }
            // Swallow any error because this can be malformed user input.
            catch (Exception ex)
            {
                Debug.WriteLine(MsgBEModifyFail + " " + ex.Message, cmd.PropertyName, cmd.PropertyValue);
                result = ModifyResult.Error;
            }
        }
        return result;
    }

    /// <summary>
    /// Sets the property if the <see cref="BatchInfo"/> should be filtered due to the <see cref="StringInstruction"/> provided.
    /// </summary>
    /// <param name="cmd">Command Filter</param>
    /// <param name="info">Pokémon to check.</param>
    /// <param name="props">PropertyInfo cache (optional)</param>
    /// <returns>True if filtered, else false.</returns>
    private static ModifyResult SetPKMProperty(StringInstruction cmd, BatchInfo info, Dictionary<string, PropertyInfo> props)
    {
        var pk = info.Entity;
        if (cmd.PropertyValue.StartsWith(CONST_BYTES, StringComparison.Ordinal))
            return SetByteArrayProperty(pk, cmd);

        if (cmd.PropertyValue.StartsWith(CONST_SUGGEST, StringComparison.OrdinalIgnoreCase))
            return SetSuggestedPKMProperty(cmd.PropertyName, info, cmd.PropertyValue);
        if (cmd is { PropertyValue: CONST_RAND, PropertyName: nameof(PKM.Moves) })
            return SetSuggestedMoveset(info, true);

        if (SetComplexProperty(pk, cmd))
            return ModifyResult.Modified;

        if (!props.TryGetValue(cmd.PropertyName, out var pi))
            return ModifyResult.Error;

        if (!pi.CanWrite)
            return ModifyResult.Error;

        object val;
        if (cmd.Random)
            val = cmd.RandomValue;
        else if (cmd.PropertyValue.StartsWith(CONST_POINTER) && props.TryGetValue(cmd.PropertyValue[1..], out var opi))
            val = opi.GetValue(pk) ?? throw new NullReferenceException();
        else
            val = cmd.PropertyValue;

        ReflectUtil.SetValue(pi, pk, val);
        return ModifyResult.Modified;
    }

    /// <summary>
    /// Checks if the <see cref="BatchInfo"/> should be filtered due to the <see cref="StringInstruction"/> provided.
    /// </summary>
    /// <param name="cmd">Command Filter</param>
    /// <param name="info">Pokémon to check.</param>
    /// <param name="props">PropertyInfo cache (optional)</param>
    /// <returns>True if filter matches, else false.</returns>
    private static bool IsFilterMatch(StringInstruction cmd, BatchInfo info, IReadOnlyDictionary<string, PropertyInfo> props)
    {
        var match = BatchFilters.FilterMods.Find(z => z.IsMatch(cmd.PropertyName));
        if (match != null)
            return match.IsFiltered(info, cmd);
        return IsPropertyFiltered(cmd, info.Entity, props);
    }

    /// <summary>
    /// Checks if the <see cref="PKM"/> should be filtered due to the <see cref="StringInstruction"/> provided.
    /// </summary>
    /// <param name="cmd">Command Filter</param>
    /// <param name="pk">Pokémon to check.</param>
    /// <param name="props">PropertyInfo cache (optional)</param>
    /// <returns>True if filter matches, else false.</returns>
    private static bool IsFilterMatch(StringInstruction cmd, PKM pk, IReadOnlyDictionary<string, PropertyInfo> props)
    {
        var match = BatchFilters.FilterMods.Find(z => z.IsMatch(cmd.PropertyName));
        if (match != null)
            return match.IsFiltered(pk, cmd);
        return IsPropertyFiltered(cmd, pk, props);
    }

    /// <summary>
    /// Checks if the <see cref="PKM"/> should be filtered due to the <see cref="StringInstruction"/> provided.
    /// </summary>
    /// <param name="cmd">Command Filter</param>
    /// <param name="pk">Pokémon to check.</param>
    /// <param name="props">PropertyInfo cache</param>
    /// <returns>True if filtered, else false.</returns>
    private static bool IsPropertyFiltered(StringInstruction cmd, PKM pk, IReadOnlyDictionary<string, PropertyInfo> props)
    {
        if (!props.TryGetValue(cmd.PropertyName, out var pi))
            return false;
        if (!pi.CanRead)
            return false;

        string val = cmd.PropertyValue;
        if (val.StartsWith(CONST_POINTER) && props.TryGetValue(val[1..], out var opi))
        {
            var result = opi.GetValue(pk) ?? throw new NullReferenceException();
            return cmd.Comparer.IsCompareOperator(pi.CompareTo(pk, result));
        }
        return cmd.Comparer.IsCompareOperator(pi.CompareTo(pk, val));
    }

    /// <summary>
    /// Sets the <see cref="PKM"/> data with a suggested value based on its <see cref="LegalityAnalysis"/>.
    /// </summary>
    /// <param name="name">Property to modify.</param>
    /// <param name="info">Cached info storing Legal data.</param>
    /// <param name="propValue">Suggestion string which starts with <see cref="CONST_SUGGEST"/></param>
    private static ModifyResult SetSuggestedPKMProperty(string name, BatchInfo info, string propValue)
    {
        var first = BatchMods.SuggestionMods.Find(z => z.IsMatch(name, propValue, info));
        if (first != null)
            return first.Modify(name, propValue, info);
        return ModifyResult.Error;
    }

    /// <summary>
    /// Sets the <see cref="PKM"/> byte array property to a specified value.
    /// </summary>
    /// <param name="pk">Pokémon to modify.</param>
    /// <param name="cmd">Modification</param>
    private static ModifyResult SetByteArrayProperty(PKM pk, StringInstruction cmd)
    {
        switch (cmd.PropertyName)
        {
            case nameof(PKM.NicknameTrash): StringUtil.LoadHexBytesTo(cmd.PropertyValue.AsSpan(CONST_BYTES.Length), pk.NicknameTrash, 3); return ModifyResult.Modified;
            case nameof(PKM.OriginalTrainerTrash):       StringUtil.LoadHexBytesTo(cmd.PropertyValue.AsSpan(CONST_BYTES.Length), pk.OriginalTrainerTrash, 3);       return ModifyResult.Modified;
            case nameof(PKM.HandlingTrainerTrash):       StringUtil.LoadHexBytesTo(cmd.PropertyValue.AsSpan(CONST_BYTES.Length), pk.HandlingTrainerTrash, 3);       return ModifyResult.Modified;
            default:
                return ModifyResult.Error;
        }
    }

    /// <summary>
    /// Sets the <see cref="PKM"/> property to a non-specific smart value.
    /// </summary>
    /// <param name="pk">Pokémon to modify.</param>
    /// <param name="cmd">Modification</param>
    /// <returns>True if modified, false if no modifications done.</returns>
    private static bool SetComplexProperty(PKM pk, StringInstruction cmd)
    {
        if (cmd.PropertyName.StartsWith("IV", StringComparison.Ordinal) && cmd.PropertyValue == CONST_RAND)
        {
            SetRandomIVs(pk, cmd);
            return true;
        }

        var match = BatchMods.ComplexMods.Find(z => z.IsMatch(cmd.PropertyName, cmd.PropertyValue));
        if (match == null)
            return false;

        match.Modify(pk, cmd);
        return true;
    }

    /// <summary>
    /// Sets the <see cref="PKM"/> IV(s) to a random value.
    /// </summary>
    /// <param name="pk">Pokémon to modify.</param>
    /// <param name="cmd">Modification</param>
    private static void SetRandomIVs(PKM pk, StringInstruction cmd)
    {
        if (cmd.PropertyName == nameof(PKM.IVs))
        {
            var la = new LegalityAnalysis(pk);
            var enc = la.EncounterMatch;
            if (enc is IFlawlessIVCount { FlawlessIVCount: not 0 } fc)
                pk.SetRandomIVs(fc.FlawlessIVCount);
            else if (enc is IFixedIVSet { IVs: {IsSpecified: true} iv})
                pk.SetRandomIVs(iv);
            else if (enc is IFlawlessIVCountConditional c && c.GetFlawlessIVCount(pk) is { Max: not 0 } x)
                pk.SetRandomIVs(Util.Rand.Next(x.Min, x.Max + 1));
            else
                pk.SetRandomIVs();
            return;
        }

        if (TryGetHasProperty(pk, cmd.PropertyName, out var pi))
        {
            if (cmd.PropertyName == nameof(PK9.IV32))
            {
                var value = (uint)Util.Rand.Next(0x3FFFFFFF + 1);
                if (pk is BK4 bk) // Big Endian, reverse IV ordering
                {
                    value <<= 2; // flags are the lowest bits, and our random value is still fine.
                    value |= bk.IV32 & 3; // preserve the flags
                }
                else
                {
                    var exist = ReflectUtil.GetValue(pk, cmd.PropertyName);
                    value |= exist switch
                    {
                        uint iv => iv & (3u << 30), // preserve the flags
                        _ => 0,
                    };
                }
                ReflectUtil.SetValue(pi, pk, value);
            }
            else
            {
                var value = Util.Rand.Next(pk.MaxIV + 1);
                ReflectUtil.SetValue(pi, pk, value);
            }
        }
    }
}

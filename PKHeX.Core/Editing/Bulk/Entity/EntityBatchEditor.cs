using System;
using System.Collections.Generic;
using System.Linq;
using static PKHeX.Core.BatchModifications;

namespace PKHeX.Core;

/// <summary>
/// Logic for editing many <see cref="PKM"/> with user provided <see cref="StringInstruction"/> list.
/// </summary>
public sealed class EntityBatchEditor() : BatchEditingBase<PKM, BatchInfo>(EntityTypes, EntityCustomProperties, expectedMax: 0x200)
{
    private static readonly Type[] EntityTypes =
    [
        typeof (PK9), typeof (PA9),
        typeof (PK8), typeof (PA8), typeof (PB8),
        typeof (PB7),
        typeof (PK7), typeof (PK6), typeof (PK5), typeof (PK4), typeof(BK4), typeof(RK4),
        typeof (PK3), typeof (XK3), typeof (CK3),
        typeof (PK2), typeof (SK2), typeof (PK1),
    ];

    /// <summary>
    /// Extra properties to show in the list of selectable properties (GUI) with special handling.
    /// </summary>
    /// <remarks>
    /// These are not necessarily properties of the <see cref="PKM"/> themselves,
    /// but can be any context-sensitive value related to the <see cref="PKM"/> or its legality,
    /// such as "Legal" or "HasType". The handling of these properties must be implemented in the <see cref="TryHandleSetOperation"/> and <see cref="TryHandleFilter"/> methods.
    /// </remarks>
    private static readonly string[] EntityCustomProperties =
    [
        // General
        BatchEditingUtil.PROP_TYPENAME,

        // Entity/PersonalInfo
        PROP_LEGAL, PROP_RIBBONS, PROP_EVS, PROP_CONTESTSTATS, PROP_MOVEMASTERY, PROP_MOVEPLUS,
        PROP_TYPE1, PROP_TYPE2, PROP_TYPEEITHER,

        // SlotCache
        IdentifierContains, nameof(ISlotInfo.Slot), nameof(SlotInfoBox.Box),
    ];

    public static EntityBatchEditor Instance { get; } = new();

    // Custom Identifiers for special handling.
    private const string CONST_BYTES = "$[]"; // Define a byte array with separated hex byte values, e.g. "$[]FF,02,03" or "$[]A0 02 0A FF"

    // Custom Values to apply.
    internal const string CONST_RAND = "$rand";
    internal const string CONST_SHINY = "$shiny";
    internal const string CONST_SUGGEST = "$suggest";
    internal const char CONST_SPECIAL = '$';

    // Custom Properties to change.
    internal const string PROP_LEGAL = "Legal";
    internal const string PROP_TYPEEITHER = "HasType";
    internal const string PROP_TYPE1 = "PersonalType1";
    internal const string PROP_TYPE2 = "PersonalType2";
    internal const string PROP_RIBBONS = "Ribbons";
    internal const string PROP_EVS = "EVs";
    internal const string PROP_CONTESTSTATS = "ContestStats";
    internal const string PROP_MOVEMASTERY = "MoveMastery";
    internal const string PROP_MOVEPLUS = "PlusMoves";
    internal const string IdentifierContains = nameof(IdentifierContains);

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
        ReadOnlySpan<string> set;
        switch (i.PropertyName)
        {
            case nameof(PKM.Species):  set = GameInfo.Strings.specieslist; break;
            case nameof(PKM.HeldItem): set = GameInfo.Strings.itemlist;    break;
            case nameof(PKM.Ability):  set = GameInfo.Strings.abilitylist; break;
            case nameof(PKM.Nature):   set = GameInfo.Strings.natures;  break;
            case nameof(PKM.Ball):     set = GameInfo.Strings.balllist; break;

            case nameof(PKM.Move1) or nameof(PKM.Move2) or nameof(PKM.Move3) or nameof(PKM.Move4):
            case nameof(PKM.RelearnMove1) or nameof(PKM.RelearnMove2) or nameof(PKM.RelearnMove3) or nameof(PKM.RelearnMove4):
                set = GameInfo.Strings.movelist; break;
            default:
                return;
        }
        i.SetScreenedValue(set);
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

    protected override BatchInfo CreateMeta(PKM entity) => new(entity);

    protected override bool ShouldModify(PKM entity) => entity.ChecksumValid && entity.Species != 0;

    protected override bool TryHandleSetOperation(StringInstruction cmd, BatchInfo info, PKM entity, out ModifyResult result)
    {
        if (cmd.PropertyValue.StartsWith(CONST_BYTES, StringComparison.Ordinal))
        {
            result = SetByteArrayProperty(entity, cmd);
            return true;
        }

        if (cmd.PropertyValue.StartsWith(CONST_SUGGEST, StringComparison.OrdinalIgnoreCase))
        {
            result = SetSuggestedProperty(cmd.PropertyName, info, cmd.PropertyValue);
            return true;
        }

        if (cmd is { PropertyValue: CONST_RAND, PropertyName: nameof(PKM.Moves) })
        {
            result = SetSuggestedMoveset(info, true);
            return true;
        }

        if (SetComplexProperty(info, cmd))
        {
            result = ModifyResult.Modified;
            return true;
        }

        result = ModifyResult.Skipped;
        return false;
    }

    protected override bool TryHandleFilter(StringInstruction cmd, BatchInfo info, PKM entity, out bool isMatch)
    {
        var match = BatchFilters.FilterMods.Find(z => z.IsMatch(cmd.PropertyName));
        if (match is null)
        {
            isMatch = false;
            return false;
        }

        isMatch = match.IsFiltered(info, cmd);
        return true;
    }

    /// <summary>
    /// Sets the <see cref="PKM"/> data with a suggested value based on its <see cref="LegalityAnalysis"/>.
    /// </summary>
    /// <param name="name">Property to modify.</param>
    /// <param name="info">Cached info storing Legal data.</param>
    /// <param name="propValue">Suggestion string which starts with <see cref="CONST_SUGGEST"/></param>
    private static ModifyResult SetSuggestedProperty(ReadOnlySpan<char> name, BatchInfo info, ReadOnlySpan<char> propValue)
    {
        foreach (var mod in BatchMods.SuggestionMods)
        {
            if (mod.IsMatch(name, propValue, info))
                return mod.Modify(name, propValue, info);
        }
        return ModifyResult.Error;
    }

    /// <summary>
    /// Sets the <see cref="PKM"/> byte array property to a specified value.
    /// </summary>
    /// <param name="pk">Pokémon to modify.</param>
    /// <param name="cmd">Modification</param>
    private static ModifyResult SetByteArrayProperty(PKM pk, StringInstruction cmd)
    {
        Span<byte> dest;
        switch (cmd.PropertyName)
        {
            case nameof(PKM.NicknameTrash) or nameof(PKM.Nickname): dest = pk.NicknameTrash; break;
            case nameof(PKM.OriginalTrainerTrash): dest = pk.OriginalTrainerTrash; break;
            case nameof(PKM.HandlingTrainerTrash): dest = pk.HandlingTrainerTrash; break;
            default:
                return ModifyResult.Error;
        }
        var src = cmd.PropertyValue.AsSpan(CONST_BYTES.Length); // skip prefix
        StringUtil.LoadHexBytesTo(src, dest, 3);
        return ModifyResult.Modified;
    }

    /// <summary>
    /// Sets the <see cref="PKM"/> property to a non-specific smart value.
    /// </summary>
    /// <param name="info">Pokémon to modify.</param>
    /// <param name="cmd">Modification</param>
    /// <returns>True if modified, false if no modifications done.</returns>
    private bool SetComplexProperty(BatchInfo info, StringInstruction cmd)
    {
        ReadOnlySpan<char> name = cmd.PropertyName;
        ReadOnlySpan<char> value = cmd.PropertyValue;

        if (name.StartsWith("IV") && value is CONST_RAND)
        {
            SetRandomIVs(info, name);
            return true;
        }

        foreach (var mod in BatchMods.ComplexMods)
        {
            if (!mod.IsMatch(name, value))
                continue;
            mod.Modify(info.Entity, cmd);
            return true;
        }
        return false;
    }

    /// <summary>
    /// Sets the <see cref="PKM"/> IV(s) to a random value.
    /// </summary>
    /// <param name="info">Pokémon to modify.</param>
    /// <param name="propertyName">Property to modify</param>
    private void SetRandomIVs(BatchInfo info, ReadOnlySpan<char> propertyName)
    {
        var pk = info.Entity;
        if (propertyName is nameof(PKM.IVs))
        {
            var la = info.Legality;
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

        if (TryGetHasProperty(pk, propertyName, out var pi))
        {
            const string IV32 = nameof(PK9.IV32);
            if (propertyName is IV32)
            {
                var value = (uint)Util.Rand.Next(0x3FFFFFFF + 1);
                if (pk is BK4 bk) // Big Endian, reverse IV ordering
                {
                    value <<= 2; // flags are the lowest bits, and our random value is still fine.
                    value |= bk.IV32 & 3; // preserve the flags
                    bk.IV32 = value;
                    return;
                }

                var exist = ReflectUtil.GetValue(pk, IV32);
                value |= exist switch
                {
                    uint iv => iv & (3u << 30), // preserve the flags
                    _ => 0,
                };
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

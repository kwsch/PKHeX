using System;
using static PKHeX.Core.BattleTemplateParseErrorType;

namespace PKHeX.Core;

public enum BattleTemplateParseErrorType : byte
{
    None = 0,
    LineLength,
    TokenUnknown,
    TokenFailParse,
    MoveCountTooMany,
    MoveSlotAlreadyUsed,
    MoveDuplicate,
    MoveUnrecognized,

    ItemUnrecognized,
    AbilityDeclaration,
    AbilityUnrecognized,
    AbilityAlreadySpecified,
    NatureUnrecognized,
    NatureAlreadySpecified,

    HiddenPowerUnknownType,
    HiddenPowerIncompatibleIVs,

    NatureEffortAmpDeclaration,
    NatureEffortAmpUnknown,
    NatureEffortAmpAlreadySpecified,
    NatureEffortAmpConflictNature,
    NatureAmpNoPlus,
    NatureAmpNoMinus,
}

public static class BattleTemplateParseErrorExtensions
{
    /// <summary>
    /// Returns the localized string for the provided <paramref name="type"/>.
    /// Falls back to the enum name if no mapping exists.
    /// </summary>
    public static string Humanize(this BattleTemplateParseErrorType type, BattleTemplateParseErrorLocalization localization, string value)
    {
        var template = GetTemplate(type, localization);
        if (value.Length == 0)
            return template;
        return string.Format(template, value);
    }

    private static string GetTemplate(BattleTemplateParseErrorType type, BattleTemplateParseErrorLocalization localization) => type switch
    {
        None => "",
        LineLength => localization.LineLength,
        TokenUnknown => localization.TokenUnknown,
        TokenFailParse => localization.TokenFailParse,
        MoveCountTooMany => localization.MoveCountTooMany,
        MoveSlotAlreadyUsed => localization.MoveSlotAlreadyUsed,
        MoveDuplicate => localization.MoveDuplicate,
        MoveUnrecognized => localization.MoveUnrecognized,
        ItemUnrecognized => localization.ItemUnrecognized,
        AbilityDeclaration => localization.AbilityDeclaration,
        AbilityUnrecognized => localization.AbilityUnrecognized,
        AbilityAlreadySpecified => localization.AbilityAlreadySpecified,
        NatureUnrecognized => localization.NatureUnrecognized,
        NatureAlreadySpecified => localization.NatureAlreadySpecified,
        HiddenPowerUnknownType => localization.HiddenPowerUnknownType,
        HiddenPowerIncompatibleIVs => localization.HiddenPowerIncompatibleIVs,
        NatureEffortAmpDeclaration => localization.NatureEffortAmpDeclaration,
        NatureEffortAmpUnknown => localization.NatureEffortAmpUnknown,
        NatureEffortAmpAlreadySpecified => localization.NatureEffortAmpAlreadySpecified,
        NatureEffortAmpConflictNature => localization.NatureEffortAmpConflictNature,
        NatureAmpNoPlus => localization.NatureAmpNoPlus,
        NatureAmpNoMinus => localization.NatureAmpNoMinus,
        _ => throw new ArgumentOutOfRangeException(nameof(type), type, null),
    };
}

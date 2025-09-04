using System;

namespace PKHeX.Core;

public readonly record struct BattleTemplateParseError(BattleTemplateParseErrorType Type, string Value)
{
    public string Humanize(BattleTemplateParseErrorLocalization localization) => Type.Humanize(localization, Value);
}

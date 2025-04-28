using System;

namespace PKHeX.Core;

public readonly ref struct BattleTemplateExportSettings
{
    public ReadOnlySpan<BattleTemplateToken> Order { get; init; }
    public BattleTemplateLocalization Localization { get; }

    public StatDisplayStyle Stats { get; init; }
    public MoveDisplayStyle Moves { get; init; }

    public static BattleTemplateExportSettings Showdown => new(BattleTemplateConfig.Showdown);
    public static BattleTemplateExportSettings CommunityStandard => new(BattleTemplateConfig.CommunityStandard);

    public BattleTemplateExportSettings(string language) : this(BattleTemplateConfig.Showdown, language) { }
    public BattleTemplateExportSettings(LanguageID language) : this(BattleTemplateConfig.Showdown, language) { }

    public BattleTemplateExportSettings(ReadOnlySpan<BattleTemplateToken> order, string language = BattleTemplateLocalization.DefaultLanguage)
    {
        Localization = BattleTemplateLocalization.GetLocalization(language);
        Order = order;
    }

    public BattleTemplateExportSettings(ReadOnlySpan<BattleTemplateToken> order, LanguageID language)
    {
        Localization = BattleTemplateLocalization.GetLocalization(language);
        Order = order;
    }

    public bool IsTokenInExport(BattleTemplateToken token)
    {
        foreach (var t in Order)
        {
            if (t == token)
                return true;
        }
        return false;
    }

    public int GetTokenIndex(BattleTemplateToken token)
    {
        for (int i = 0; i < Order.Length; i++)
        {
            if (Order[i] == token)
                return i;
        }
        return -1;
    }

    public bool IsTokenInExport(BattleTemplateToken token, ReadOnlySpan<BattleTemplateToken> tokens)
    {
        foreach (var t in tokens)
        {
            if (t == token)
                return true;
        }
        return false;
    }
}

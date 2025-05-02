using System;

namespace PKHeX.Core;

/// <summary>
/// Settings for exporting a battle template.
/// </summary>
public readonly ref struct BattleTemplateExportSettings
{
    /// <summary>
    /// Order of the tokens in the export.
    /// </summary>
    public ReadOnlySpan<BattleTemplateToken> Order { get; init; }

    /// <summary>
    /// Localization for the battle template.
    /// </summary>
    public BattleTemplateLocalization Localization { get; }

    /// <summary>
    /// Display style for the EVs.
    /// </summary>
    public StatDisplayStyle StatsEVs { get; init; }

    /// <summary>
    /// Display style for the IVs.
    /// </summary>
    public StatDisplayStyle StatsIVs { get; init; }

    public StatDisplayStyle StatsOther { get; init; }

    /// <summary>
    /// Display style for the moves.
    /// </summary>
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

    /// <summary>
    /// Checks if the token is in the export.
    /// </summary>
    public bool IsTokenInExport(BattleTemplateToken token)
    {
        foreach (var t in Order)
        {
            if (t == token)
                return true;
        }
        return false;
    }

    /// <summary>
    /// Gets the index of the token in the export.
    /// </summary>
    public int GetTokenIndex(BattleTemplateToken token)
    {
        for (int i = 0; i < Order.Length; i++)
        {
            if (Order[i] == token)
                return i;
        }
        return -1;
    }

    /// <summary>
    /// Checks if the token is in the export.
    /// </summary>
    /// <remarks>Should be a static method, but is not because it feels better this way.</remarks>
    /// <param name="token">Token to check</param>
    /// <param name="tokens">Tokens to check against</param>
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

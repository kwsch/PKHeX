using System;
using System.ComponentModel;

namespace PKHeX.Core;

public sealed class BattleTemplateSettings
{
    [LocalizedDescription("Settings for showing details when hovering a slot.")]
    public BattleTemplateTypeSetting Hover { get; set; } = new(BattleTemplateDisplayStyle.Preview, LanguageID.None, MoveDisplayStyle.Directional);

    [LocalizedDescription("Settings for showing details when exporting a slot.")]
    public BattleTemplateTypeSetting Export { get; set; } = new(BattleTemplateDisplayStyle.Showdown, LanguageID.English);
}

[TypeConverter(typeof(ExpandableObjectConverter))]
public sealed class BattleTemplateTypeSetting
{

    [LocalizedDescription("Language to use when exporting a battle template.")]
    public LanguageID Language { get; set; }
    public StatDisplayStyle StyleStat { get; set; }
    public MoveDisplayStyle StyleMove { get; set; }

    [LocalizedDescription("Display format to use when exporting a battle template from the program.")]
    public BattleTemplateDisplayStyle TokenOrder { get; set; }

    [LocalizedDescription("Custom ordering for exporting a set, if chosen via export display style.")]
    public BattleTemplateToken[] TokenOrderCustom { get; set; } = BattleTemplateConfig.Showdown.ToArray();

    public BattleTemplateTypeSetting() { }
    public BattleTemplateTypeSetting(BattleTemplateDisplayStyle style, LanguageID lang, MoveDisplayStyle move = MoveDisplayStyle.Fill)
    {
        TokenOrder = style;
        Language = lang;
        StyleMove = move;
    }
    private LanguageID GetLanguageExport(LanguageID program) => GetLanguage(Language, program);

    public BattleTemplateExportSettings GetSettings(LanguageID programLanguage, EntityContext context) => new(GetOrder(TokenOrder, TokenOrderCustom), GetLanguageExport(programLanguage))
    {
        Stats = StyleStat,
        Moves = GetMoveDisplayStyle(StyleMove, context),
    };

    private static LanguageID GetLanguage(LanguageID choice, LanguageID program)
    {
        if (choice != LanguageID.None)
            return choice;
        if (program == LanguageID.None)
            return LanguageID.English;
        return program;
    }

    private static ReadOnlySpan<BattleTemplateToken> GetOrder(BattleTemplateDisplayStyle style, ReadOnlySpan<BattleTemplateToken> custom) => style switch
    {
        BattleTemplateDisplayStyle.Legacy => BattleTemplateConfig.CommunityStandard,
        BattleTemplateDisplayStyle.Preview => BattleTemplateConfig.DefaultHover,
        BattleTemplateDisplayStyle.Custom => custom,
        _ => BattleTemplateConfig.Showdown,
    };

    private static MoveDisplayStyle GetMoveDisplayStyle(MoveDisplayStyle style, EntityContext context) => style switch
    {
        //MoveDisplayStyle.Directional when context is EntityContext.Gen9a => MoveDisplayStyle.Directional, TODO ZA
        _ => MoveDisplayStyle.Fill,
    };
}

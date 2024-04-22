using System;
using static PKHeX.Core.RibbonIndex4;

namespace PKHeX.Core;

/// <summary>
/// Ribbons from Generation 4 that were not carried forward to future formats.
/// </summary>
public enum RibbonIndex4 : byte
{
    // Battle: Gen4
    Ability,
    AbilityGreat,
    AbilityDouble,
    AbilityMulti,
    AbilityPair,
    AbilityWorld,

    // Contest: Gen4
    Cool,
    CoolGreat,
    CoolUltra,
    CoolMaster,
    Beauty,
    BeautyGreat,
    BeautyUltra,
    BeautyMaster,
    Cute,
    CuteGreat,
    CuteUltra,
    CuteMaster,
    Smart,
    SmartGreat,
    SmartUltra,
    SmartMaster,
    Tough,
    ToughGreat,
    ToughUltra,
    ToughMaster,

    MAX_COUNT,
}

public static class RibbonIndex4Extensions
{
    public static void Fix(this RibbonIndex4 r, in RibbonVerifierArguments args, bool state)
    {
        var pk = args.Entity;
        if (pk is not IRibbonSetUnique4 u4)
            return;

        _ = r switch
        {
            RibbonIndex4.Ability => u4.RibbonAbility = state,
            AbilityGreat => u4.RibbonAbilityGreat = state,
            AbilityDouble => u4.RibbonAbilityDouble = state,
            AbilityMulti => u4.RibbonAbilityMulti = state,
            AbilityPair => u4.RibbonAbilityPair = state,
            AbilityWorld => u4.RibbonAbilityWorld = state,
            Cool => u4.RibbonG4Cool = state,
            CoolGreat => u4.RibbonG4CoolGreat = state,
            CoolUltra => u4.RibbonG4CoolUltra = state,
            CoolMaster => u4.RibbonG4CoolMaster = state,
            Beauty => u4.RibbonG4Beauty = state,
            BeautyGreat => u4.RibbonG4BeautyGreat = state,
            BeautyUltra => u4.RibbonG4BeautyUltra = state,
            BeautyMaster => u4.RibbonG4BeautyMaster = state,
            Cute => u4.RibbonG4Cute = state,
            CuteGreat => u4.RibbonG4CuteGreat = state,
            CuteUltra => u4.RibbonG4CuteUltra = state,
            CuteMaster => u4.RibbonG4CuteMaster = state,
            Smart => u4.RibbonG4Smart = state,
            SmartGreat => u4.RibbonG4SmartGreat = state,
            SmartUltra => u4.RibbonG4SmartUltra = state,
            SmartMaster => u4.RibbonG4SmartMaster = state,
            Tough => u4.RibbonG4Tough = state,
            ToughGreat => u4.RibbonG4ToughGreat = state,
            ToughUltra => u4.RibbonG4ToughUltra = state,
            ToughMaster => u4.RibbonG4ToughMaster = state,
            _ => throw new ArgumentOutOfRangeException(nameof(r), r, null),
        };
    }

    public static string GetPropertyName(this RibbonIndex4 r) => r switch
    {
        RibbonIndex4.Ability => nameof(IRibbonSetUnique4.RibbonAbility),
        AbilityGreat => nameof(IRibbonSetUnique4.RibbonAbilityGreat),
        AbilityDouble => nameof(IRibbonSetUnique4.RibbonAbilityDouble),
        AbilityMulti => nameof(IRibbonSetUnique4.RibbonAbilityMulti),
        AbilityPair => nameof(IRibbonSetUnique4.RibbonAbilityPair),
        AbilityWorld => nameof(IRibbonSetUnique4.RibbonAbilityWorld),
        Cool => nameof(IRibbonSetUnique4.RibbonG4Cool),
        CoolGreat => nameof(IRibbonSetUnique4.RibbonG4CoolGreat),
        CoolUltra => nameof(IRibbonSetUnique4.RibbonG4CoolUltra),
        CoolMaster => nameof(IRibbonSetUnique4.RibbonG4CoolMaster),
        Beauty => nameof(IRibbonSetUnique4.RibbonG4Beauty),
        BeautyGreat => nameof(IRibbonSetUnique4.RibbonG4BeautyGreat),
        BeautyUltra => nameof(IRibbonSetUnique4.RibbonG4BeautyUltra),
        BeautyMaster => nameof(IRibbonSetUnique4.RibbonG4BeautyMaster),
        Cute => nameof(IRibbonSetUnique4.RibbonG4Cute),
        CuteGreat => nameof(IRibbonSetUnique4.RibbonG4CuteGreat),
        CuteUltra => nameof(IRibbonSetUnique4.RibbonG4CuteUltra),
        CuteMaster => nameof(IRibbonSetUnique4.RibbonG4CuteMaster),
        Smart => nameof(IRibbonSetUnique4.RibbonG4Smart),
        SmartGreat => nameof(IRibbonSetUnique4.RibbonG4SmartGreat),
        SmartUltra => nameof(IRibbonSetUnique4.RibbonG4SmartUltra),
        SmartMaster => nameof(IRibbonSetUnique4.RibbonG4SmartMaster),
        Tough => nameof(IRibbonSetUnique4.RibbonG4Tough),
        ToughGreat => nameof(IRibbonSetUnique4.RibbonG4ToughGreat),
        ToughUltra => nameof(IRibbonSetUnique4.RibbonG4ToughUltra),
        ToughMaster => nameof(IRibbonSetUnique4.RibbonG4ToughMaster),
        _ => throw new ArgumentOutOfRangeException(nameof(r), r, null),
    };
}

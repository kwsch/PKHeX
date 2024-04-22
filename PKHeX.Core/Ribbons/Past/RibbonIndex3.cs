using System;
using static PKHeX.Core.RibbonIndex3;

namespace PKHeX.Core;

/// <summary>
/// Ribbons from Generation 3 that were not carried forward to future formats.
/// </summary>
public enum RibbonIndex3 : byte
{
    // Battle: Gen3
    Winning,
    Victory,

    // Contest: Gen3
    Cool,
    CoolSuper,
    CoolHyper,
    CoolMaster,
    Beauty,
    BeautySuper,
    BeautyHyper,
    BeautyMaster,
    Cute,
    CuteSuper,
    CuteHyper,
    CuteMaster,
    Smart,
    SmartSuper,
    SmartHyper,
    SmartMaster,
    Tough,
    ToughSuper,
    ToughHyper,
    ToughMaster,

    MAX_COUNT,
}

public static class RibbonIndex3Extensions
{
    public static void Fix(this RibbonIndex3 r, in RibbonVerifierArguments args, bool state)
    {
        var pk = args.Entity;
        if (r is Victory or Winning)
        {
            if (pk is not IRibbonSetUnique3 u3)
                return;
            if (r is Victory)
                u3.RibbonVictory = state;
            else
                u3.RibbonWinning = state;
            return;
        }

        if (pk is IRibbonSetOnly3 o3)
        {
            const byte max = 4;
            const byte min = 0;
            byte value = state ? max : min;
            if (r is Cool)
                o3.RibbonCountG3Cool = value;
            else if (r is Beauty)
                o3.RibbonCountG3Beauty = value;
            else if (r is Cute)
                o3.RibbonCountG3Cute = value;
            else if (r is Smart)
                o3.RibbonCountG3Smart = value;
            else if (r is Tough)
                o3.RibbonCountG3Tough = value;
            return;
        }

        if (pk is not IRibbonSetUnique4 u4)
            return;

        _ = r switch
        {
            Cool => u4.RibbonG3Cool = state,
            CoolSuper => u4.RibbonG3CoolSuper = state,
            CoolHyper => u4.RibbonG3CoolHyper = state,
            CoolMaster => u4.RibbonG3CoolMaster = state,
            Beauty => u4.RibbonG3Beauty = state,
            BeautySuper => u4.RibbonG3BeautySuper = state,
            BeautyHyper => u4.RibbonG3BeautyHyper = state,
            BeautyMaster => u4.RibbonG3BeautyMaster = state,
            Cute => u4.RibbonG3Cute = state,
            CuteSuper => u4.RibbonG3CuteSuper = state,
            CuteHyper => u4.RibbonG3CuteHyper = state,
            CuteMaster => u4.RibbonG3CuteMaster = state,
            Smart => u4.RibbonG3Smart = state,
            SmartSuper => u4.RibbonG3SmartSuper = state,
            SmartHyper => u4.RibbonG3SmartHyper = state,
            SmartMaster => u4.RibbonG3SmartMaster = state,
            Tough => u4.RibbonG3Tough = state,
            ToughSuper => u4.RibbonG3ToughSuper = state,
            ToughHyper => u4.RibbonG3ToughHyper = state,
            ToughMaster => u4.RibbonG3ToughMaster = state,
            _ => throw new ArgumentOutOfRangeException(nameof(r), r, null),
        };
    }

    public static string GetPropertyName(this RibbonIndex3 r) => r switch
    {
        Winning => nameof(IRibbonSetUnique3.RibbonWinning),
        Victory => nameof(IRibbonSetUnique3.RibbonVictory),
        Cool => nameof(IRibbonSetUnique4.RibbonG3Cool),
        CoolSuper => nameof(IRibbonSetUnique4.RibbonG3CoolSuper),
        CoolHyper => nameof(IRibbonSetUnique4.RibbonG3CoolHyper),
        CoolMaster => nameof(IRibbonSetUnique4.RibbonG3CoolMaster),
        Beauty => nameof(IRibbonSetUnique4.RibbonG3Beauty),
        BeautySuper => nameof(IRibbonSetUnique4.RibbonG3BeautySuper),
        BeautyHyper => nameof(IRibbonSetUnique4.RibbonG3BeautyHyper),
        BeautyMaster => nameof(IRibbonSetUnique4.RibbonG3BeautyMaster),
        Cute => nameof(IRibbonSetUnique4.RibbonG3Cute),
        CuteSuper => nameof(IRibbonSetUnique4.RibbonG3CuteSuper),
        CuteHyper => nameof(IRibbonSetUnique4.RibbonG3CuteHyper),
        CuteMaster => nameof(IRibbonSetUnique4.RibbonG3CuteMaster),
        Smart => nameof(IRibbonSetUnique4.RibbonG3Smart),
        SmartSuper => nameof(IRibbonSetUnique4.RibbonG3SmartSuper),
        SmartHyper => nameof(IRibbonSetUnique4.RibbonG3SmartHyper),
        SmartMaster => nameof(IRibbonSetUnique4.RibbonG3SmartMaster),
        Tough => nameof(IRibbonSetUnique4.RibbonG3Tough),
        ToughSuper => nameof(IRibbonSetUnique4.RibbonG3ToughSuper),
        ToughHyper => nameof(IRibbonSetUnique4.RibbonG3ToughHyper),
        ToughMaster => nameof(IRibbonSetUnique4.RibbonG3ToughMaster),
        _ => throw new ArgumentOutOfRangeException(nameof(r), r, null),
    };
}

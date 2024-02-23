using System;
using static PKHeX.Core.LearnMethod;
using static PKHeX.Core.LegalityCheckStrings;

namespace PKHeX.Core;

/// <summary>
/// Egg Moveset Building Order for Generation 2
/// </summary>
public enum EggSource2 : byte
{
    None,
    /// <summary> Initial moveset for the egg's level. </summary>
    Base,
    /// <summary> Egg move inherited from the Father. </summary>
    FatherEgg,
    /// <summary> Technical Machine move inherited from the Father. </summary>
    FatherTM,
    /// <summary> Level Up move inherited from a parent. </summary>
    ParentLevelUp,
    /// <summary> Tutor move (Elemental Beam) inherited from a parent. </summary>
    Tutor,

    Max,
}

/// <summary>
/// Egg Moveset Building Order for Generation 3 &amp; 4
/// </summary>
public enum EggSource34 : byte
{
    None,
    /// <summary> Initial moveset for the egg's level. </summary>
    Base,
    /// <summary> Egg move inherited from the Father. </summary>
    FatherEgg,
    /// <summary> Technical Machine move inherited from the Father. </summary>
    FatherTM,
    /// <summary> Level Up move inherited from a parent. </summary>
    ParentLevelUp,

    Max,

    /// <summary> Special Move applied at the end if certain conditions are satisfied. </summary>
    VoltTackle,
}

/// <summary>
/// Egg Moveset Building Order for Generation 5
/// </summary>
public enum EggSource5 : byte
{
    None,
    /// <summary> Initial moveset for the egg's level. </summary>
    Base,
    /// <summary> Egg move inherited from the Father. </summary>
    FatherEgg,
    /// <summary> Technical Machine move inherited from the Father. </summary>
    ParentLevelUp,
    /// <summary> Technical Machine move inherited from the Father. </summary>
    /// <remarks> After level up, unlike Gen3/4! </remarks>
    FatherTM,

    Max,

    /// <summary> Special Move applied at the end if certain conditions are satisfied. </summary>
    VoltTackle,
}

/// <summary>
/// Egg Moveset Building Order for Generation 6+
/// </summary>
public enum EggSource6 : byte
{
    None,
    /// <summary> Initial moveset for the egg's level. </summary>
    Base,
    /// <summary> Level Up move inherited from a parent. </summary>
    ParentLevelUp,
    /// <summary> Egg move inherited from a parent. </summary>
    ParentEgg,

    Max,

    /// <summary> Special Move applied at the end if certain conditions are satisfied. </summary>
    VoltTackle,
}

/// <summary>
/// Utility logic for converting a <see cref="MoveBreed"/> move result into a user-friendly string.
/// </summary>
public static class EggSourceUtil
{
    /// <summary>
    /// Unboxes the parse result and returns a user-friendly string for the move result.
    /// </summary>
    public static string GetSourceString(Array parse, byte generation, int index)
    {
        if (index >= parse.Length)
            return LMoveSourceEmpty;

        return generation switch
        {
            2      => ((EggSource2[])parse)[index].GetSourceString(),
            3 or 4 => ((EggSource34[])parse)[index].GetSourceString(),
            5      => ((EggSource5[])parse)[index].GetSourceString(),
            >= 6   => ((EggSource6[])parse)[index].GetSourceString(),
            _      => LMoveSourceEmpty,
        };
    }

    private static string GetSourceString(this EggSource2 source) => source switch
    {
        EggSource2.Base => LMoveRelearnEgg,
        EggSource2.FatherEgg => LMoveEggInherited,
        EggSource2.FatherTM => LMoveEggTMHM,
        EggSource2.ParentLevelUp => LMoveEggLevelUp,
        EggSource2.Tutor => LMoveEggInheritedTutor,
        EggSource2.Max => "Any",
        _ => LMoveEggInvalid,
    };

    private static string GetSourceString(this EggSource34 source) => source switch
    {
        EggSource34.Base => LMoveRelearnEgg,
        EggSource34.FatherEgg => LMoveEggInherited,
        EggSource34.FatherTM => LMoveEggTMHM,
        EggSource34.ParentLevelUp => LMoveEggLevelUp,
        EggSource34.Max => "Any",
        EggSource34.VoltTackle => LMoveSourceSpecial,
        _ => LMoveEggInvalid,
    };

    private static string GetSourceString(this EggSource5 source) => source switch
    {
        EggSource5.Base => LMoveRelearnEgg,
        EggSource5.FatherEgg => LMoveEggInherited,
        EggSource5.ParentLevelUp => LMoveEggLevelUp,
        EggSource5.FatherTM => LMoveEggTMHM,
        EggSource5.Max => "Any",
        EggSource5.VoltTackle => LMoveSourceSpecial,
        _ => LMoveEggInvalid,
    };

    private static string GetSourceString(this EggSource6 source) => source switch
    {
        EggSource6.Base => LMoveRelearnEgg,
        EggSource6.ParentLevelUp => LMoveEggLevelUp,
        EggSource6.ParentEgg => LMoveEggInherited,
        EggSource6.Max => "Any",
        EggSource6.VoltTackle => LMoveSourceSpecial,
        _ => LMoveEggInvalid,
    };

    /// <summary>
    /// Converts the parse result and returns a user-friendly string for the move result.
    /// </summary>
    public static LearnMethod GetSource(byte value, byte generation) => generation switch
    {
        2      => ((EggSource2)value).GetSource(),
        3 or 4 => ((EggSource34)value).GetSource(),
        5      => ((EggSource5)value).GetSource(),
        >= 6   => ((EggSource6)value).GetSource(),
        _ => None,
    };

    private static LearnMethod GetSource(this EggSource2 source) => source switch
    {
        EggSource2.Base => Initial,
        EggSource2.FatherEgg => EggMove,
        EggSource2.FatherTM => TMHM,
        EggSource2.ParentLevelUp => InheritLevelUp,
        EggSource2.Tutor => Tutor,
        _ => None,
    };

    private static LearnMethod GetSource(this EggSource34 source) => source switch
    {
        EggSource34.Base => Initial,
        EggSource34.FatherEgg => EggMove,
        EggSource34.FatherTM => TMHM,
        EggSource34.ParentLevelUp => InheritLevelUp,
        EggSource34.VoltTackle => SpecialEgg,
        _ => None,
    };

    private static LearnMethod GetSource(this EggSource5 source) => source switch
    {
        EggSource5.Base => Initial,
        EggSource5.FatherEgg => EggMove,
        EggSource5.ParentLevelUp => InheritLevelUp,
        EggSource5.FatherTM => TMHM,
        EggSource5.VoltTackle => SpecialEgg,
        _ => None,
    };

    private static LearnMethod GetSource(this EggSource6 source) => source switch
    {
        EggSource6.Base => Initial,
        EggSource6.ParentLevelUp => InheritLevelUp,
        EggSource6.ParentEgg => EggMove,
        EggSource6.VoltTackle => SpecialEgg,
        _ => None,
    };
}

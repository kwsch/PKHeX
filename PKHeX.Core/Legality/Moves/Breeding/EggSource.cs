using System;
using static PKHeX.Core.LearnMethod;
using static PKHeX.Core.LegalityCheckLocalization;

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
    public static string GetSourceString(Array parse, byte generation, int index, MoveSourceLocalization loc)
    {
        if (index >= parse.Length)
            return loc.SourceEmpty;

        return generation switch
        {
            2      => ((EggSource2[])parse)[index].GetSourceString(loc),
            3 or 4 => ((EggSource34[])parse)[index].GetSourceString(loc),
            5      => ((EggSource5[])parse)[index].GetSourceString(loc),
            >= 6   => ((EggSource6[])parse)[index].GetSourceString(loc),
            _      => loc.SourceEmpty,
        };
    }

    private static string GetSourceString(this EggSource2 source, MoveSourceLocalization loc) => source switch
    {
        EggSource2.Base => loc.RelearnEgg,
        EggSource2.FatherEgg => loc.EggInherited,
        EggSource2.FatherTM => loc.EggTMHM,
        EggSource2.ParentLevelUp => loc.EggLevelUp,
        EggSource2.Tutor => loc.EggInheritedTutor,
        EggSource2.Max => "Any",
        _ => loc.EggInvalid,
    };

    private static string GetSourceString(this EggSource34 source, MoveSourceLocalization loc) => source switch
    {
        EggSource34.Base => loc.RelearnEgg,
        EggSource34.FatherEgg => loc.EggInherited,
        EggSource34.FatherTM => loc.EggTMHM,
        EggSource34.ParentLevelUp => loc.EggLevelUp,
        EggSource34.Max => "Any",
        EggSource34.VoltTackle => loc.SourceSpecial,
        _ => loc.EggInvalid,
    };

    private static string GetSourceString(this EggSource5 source, MoveSourceLocalization loc) => source switch
    {
        EggSource5.Base => loc.RelearnEgg,
        EggSource5.FatherEgg => loc.EggInherited,
        EggSource5.ParentLevelUp => loc.EggLevelUp,
        EggSource5.FatherTM => loc.EggTMHM,
        EggSource5.Max => "Any",
        EggSource5.VoltTackle => loc.SourceSpecial,
        _ => loc.EggInvalid,
    };

    private static string GetSourceString(this EggSource6 source, MoveSourceLocalization loc) => source switch
    {
        EggSource6.Base => loc.RelearnEgg,
        EggSource6.ParentLevelUp => loc.EggLevelUp,
        EggSource6.ParentEgg => loc.EggInherited,
        EggSource6.Max => "Any",
        EggSource6.VoltTackle => loc.SourceSpecial,
        _ => loc.EggInvalid,
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

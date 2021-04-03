using System;
using static PKHeX.Core.LegalityCheckStrings;

namespace PKHeX.Core
{
    public enum EggSource2 : byte
    {
        None,
        Base,
        FatherEgg,
        FatherTM,
        ParentLevelUp,
        Tutor,

        Max,
    }

    public enum EggSource34 : byte
    {
        None,
        Base,
        FatherEgg,
        FatherTM,
        ParentLevelUp,

        Max,

        VoltTackle,
    }

    public enum EggSource5 : byte
    {
        None,
        Base,
        FatherEgg,
        ParentLevelUp,
        FatherTM, // after level up, unlike Gen3/4!

        Max,

        VoltTackle,
    }

    public enum EggSource6 : byte
    {
        None,
        Base,
        ParentLevelUp,
        ParentEgg,

        Max,

        VoltTackle,
    }

    public static class EggSourceExtensions
    {
        public static string GetSource(this EggSource2 source) => source switch
        {
            EggSource2.None => LMoveSourceEmpty,
            EggSource2.Base => LMoveEggLevelUp,
            EggSource2.FatherEgg => LMoveRelearnEgg,
            EggSource2.FatherTM => LMoveEggTMHM,
            EggSource2.ParentLevelUp => LMoveEggInherited,
            EggSource2.Tutor => LMoveEggInheritedTutor,
            EggSource2.Max => "Any",
            _ => throw new ArgumentOutOfRangeException(nameof(source), source, null)
        };

        public static string GetSource(this EggSource34 source) => source switch
        {
            EggSource34.None => LMoveSourceEmpty,
            EggSource34.Base => LMoveEggLevelUp,
            EggSource34.FatherEgg => LMoveRelearnEgg,
            EggSource34.FatherTM => LMoveEggTMHM,
            EggSource34.ParentLevelUp => LMoveEggInherited,
            EggSource34.VoltTackle => LMoveSourceSpecial,
            EggSource34.Max => "Any",
            _ => throw new ArgumentOutOfRangeException(nameof(source), source, null)
        };

        public static string GetSource(this EggSource5 source) => source switch
        {
            EggSource5.None => LMoveSourceEmpty,
            EggSource5.Base => LMoveEggLevelUp,
            EggSource5.FatherEgg => LMoveRelearnEgg,
            EggSource5.FatherTM => LMoveEggTMHM,
            EggSource5.ParentLevelUp => LMoveEggInherited,
            EggSource5.VoltTackle => LMoveSourceSpecial,
            EggSource5.Max => "Any",
            _ => throw new ArgumentOutOfRangeException(nameof(source), source, null)
        };

        public static string GetSource(this EggSource6 source) => source switch
        {
            EggSource6.None => LMoveSourceEmpty,
            EggSource6.Base => LMoveEggLevelUp,
            EggSource6.ParentEgg => LMoveRelearnEgg,
            EggSource6.ParentLevelUp => LMoveEggInherited,
            EggSource6.VoltTackle => LMoveSourceSpecial,
            EggSource6.Max => "Any",
            _ => throw new ArgumentOutOfRangeException(nameof(source), source, null)
        };
    }
}

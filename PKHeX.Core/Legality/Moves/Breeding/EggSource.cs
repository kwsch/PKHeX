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
#pragma warning disable RCS1224 // Make method an extension method.
        public static string GetSource(object parse, int index) => parse switch
#pragma warning restore RCS1224 // Make method an extension method.
        {
            EggSource2[] x when index < x.Length => x[index].GetSource(),
           EggSource34[] x when index < x.Length => x[index].GetSource(),
            EggSource5[] x when index < x.Length => x[index].GetSource(),
            EggSource6[] x when index < x.Length => x[index].GetSource(),
            _ => LMoveSourceEmpty,
        };

        public static string GetSource(this EggSource2 source) => source switch
        {
            EggSource2.Base => LMoveRelearnEgg,
            EggSource2.FatherEgg => LMoveEggLevelUp,
            EggSource2.FatherTM => LMoveEggTMHM,
            EggSource2.ParentLevelUp => LMoveEggInherited,
            EggSource2.Tutor => LMoveEggInheritedTutor,
            EggSource2.Max => "Any",
            _ => LMoveEggInvalid,
        };

        public static string GetSource(this EggSource34 source) => source switch
        {
            EggSource34.Base => LMoveRelearnEgg,
            EggSource34.FatherEgg => LMoveEggLevelUp,
            EggSource34.FatherTM => LMoveEggTMHM,
            EggSource34.ParentLevelUp => LMoveEggInherited,
            EggSource34.VoltTackle => LMoveSourceSpecial,
            EggSource34.Max => "Any",
            _ => LMoveEggInvalid,
        };

        public static string GetSource(this EggSource5 source) => source switch
        {
            EggSource5.Base => LMoveRelearnEgg,
            EggSource5.FatherEgg => LMoveEggLevelUp,
            EggSource5.FatherTM => LMoveEggTMHM,
            EggSource5.ParentLevelUp => LMoveEggInherited,
            EggSource5.VoltTackle => LMoveSourceSpecial,
            EggSource5.Max => "Any",
            _ => LMoveEggInvalid,
        };

        public static string GetSource(this EggSource6 source) => source switch
        {
            EggSource6.Base => LMoveRelearnEgg,
            EggSource6.ParentEgg => LMoveEggLevelUp,
            EggSource6.ParentLevelUp => LMoveEggInherited,
            EggSource6.VoltTackle => LMoveSourceSpecial,
            EggSource6.Max => "Any",
            _ => LMoveEggInvalid,
        };
    }
}

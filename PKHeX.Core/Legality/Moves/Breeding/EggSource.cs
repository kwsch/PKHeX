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

    public static class EggSourceUtil
    {
#pragma warning disable RCS1224 // Make method an extension method.
        public static string GetSource(object parse, int gen, int index)
#pragma warning restore RCS1224 // Make method an extension method.
        {
            static string GetLine<T>(T[] arr, Func<T, string> act, int i)
            {
                if (i >= arr.Length)
                    return LMoveSourceEmpty;
                return act(arr[i]);
            }

            return gen switch
            {
                2      => GetLine((EggSource2[]) parse, GetSource, index),
                3 or 4 => GetLine((EggSource34[])parse, GetSource, index),
                5      => GetLine((EggSource5[]) parse, GetSource, index),
                >= 6   => GetLine((EggSource6[]) parse, GetSource, index),
                _      => LMoveSourceEmpty,
            };
        }

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

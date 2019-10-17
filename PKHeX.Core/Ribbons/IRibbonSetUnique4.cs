namespace PKHeX.Core
{
    /// <summary> Ribbons introduced in Generation 4 and were transferred to future Generations (4 and 5 only). </summary>
    public interface IRibbonSetUnique4
    {
        bool RibbonAbility { get; set; }
        bool RibbonAbilityGreat { get; set; }
        bool RibbonAbilityDouble { get; set; }
        bool RibbonAbilityMulti { get; set; }
        bool RibbonAbilityPair { get; set; }
        bool RibbonAbilityWorld { get; set; }

        bool RibbonG3Cool { get; set; }
        bool RibbonG3CoolSuper { get; set; }
        bool RibbonG3CoolHyper { get; set; }
        bool RibbonG3CoolMaster { get; set; }
        bool RibbonG3Beauty { get; set; }
        bool RibbonG3BeautySuper { get; set; }
        bool RibbonG3BeautyHyper { get; set; }
        bool RibbonG3BeautyMaster { get; set; }
        bool RibbonG3Cute { get; set; }
        bool RibbonG3CuteSuper { get; set; }
        bool RibbonG3CuteHyper { get; set; }
        bool RibbonG3CuteMaster { get; set; }
        bool RibbonG3Smart { get; set; }
        bool RibbonG3SmartSuper { get; set; }
        bool RibbonG3SmartHyper { get; set; }
        bool RibbonG3SmartMaster { get; set; }
        bool RibbonG3Tough { get; set; }
        bool RibbonG3ToughSuper { get; set; }
        bool RibbonG3ToughHyper { get; set; }
        bool RibbonG3ToughMaster { get; set; }

        bool RibbonG4Cool { get; set; }
        bool RibbonG4CoolGreat { get; set; }
        bool RibbonG4CoolUltra { get; set; }
        bool RibbonG4CoolMaster { get; set; }
        bool RibbonG4Beauty { get; set; }
        bool RibbonG4BeautyGreat { get; set; }
        bool RibbonG4BeautyUltra { get; set; }
        bool RibbonG4BeautyMaster { get; set; }
        bool RibbonG4Cute { get; set; }
        bool RibbonG4CuteGreat { get; set; }
        bool RibbonG4CuteUltra { get; set; }
        bool RibbonG4CuteMaster { get; set; }
        bool RibbonG4Smart { get; set; }
        bool RibbonG4SmartGreat { get; set; }
        bool RibbonG4SmartUltra { get; set; }
        bool RibbonG4SmartMaster { get; set; }
        bool RibbonG4Tough { get; set; }
        bool RibbonG4ToughGreat { get; set; }
        bool RibbonG4ToughUltra { get; set; }
        bool RibbonG4ToughMaster { get; set; }
    }

    internal static partial class RibbonExtensions
    {
        private static readonly string[] RibbonSetNamesUnique4Ability =
        {
            nameof(IRibbonSetUnique4.RibbonAbility),
            nameof(IRibbonSetUnique4.RibbonAbilityGreat),
            nameof(IRibbonSetUnique4.RibbonAbilityDouble),
            nameof(IRibbonSetUnique4.RibbonAbilityMulti),
            nameof(IRibbonSetUnique4.RibbonAbilityPair),
            nameof(IRibbonSetUnique4.RibbonAbilityWorld),
        };

        private static readonly string[] RibbonSetNamesUnique4Contest3 =
        {
            nameof(IRibbonSetUnique4.RibbonG3Cool),
            nameof(IRibbonSetUnique4.RibbonG3CoolSuper),
            nameof(IRibbonSetUnique4.RibbonG3CoolHyper),
            nameof(IRibbonSetUnique4.RibbonG3CoolMaster),
            nameof(IRibbonSetUnique4.RibbonG3Beauty),
            nameof(IRibbonSetUnique4.RibbonG3BeautySuper),
            nameof(IRibbonSetUnique4.RibbonG3BeautyHyper),
            nameof(IRibbonSetUnique4.RibbonG3BeautyMaster),
            nameof(IRibbonSetUnique4.RibbonG3Cute),
            nameof(IRibbonSetUnique4.RibbonG3CuteSuper),
            nameof(IRibbonSetUnique4.RibbonG3CuteHyper),
            nameof(IRibbonSetUnique4.RibbonG3CuteMaster),
            nameof(IRibbonSetUnique4.RibbonG3Smart),
            nameof(IRibbonSetUnique4.RibbonG3SmartSuper),
            nameof(IRibbonSetUnique4.RibbonG3SmartHyper),
            nameof(IRibbonSetUnique4.RibbonG3SmartMaster),
            nameof(IRibbonSetUnique4.RibbonG3Tough),
            nameof(IRibbonSetUnique4.RibbonG3ToughSuper),
            nameof(IRibbonSetUnique4.RibbonG3ToughHyper),
            nameof(IRibbonSetUnique4.RibbonG3ToughMaster),
        };

        private static readonly string[] RibbonSetNamesUnique4Contest4 =
        {
            nameof(IRibbonSetUnique4.RibbonG4Cool),
            nameof(IRibbonSetUnique4.RibbonG4CoolGreat),
            nameof(IRibbonSetUnique4.RibbonG4CoolUltra),
            nameof(IRibbonSetUnique4.RibbonG4CoolMaster),
            nameof(IRibbonSetUnique4.RibbonG4Beauty),
            nameof(IRibbonSetUnique4.RibbonG4BeautyGreat),
            nameof(IRibbonSetUnique4.RibbonG4BeautyUltra),
            nameof(IRibbonSetUnique4.RibbonG4BeautyMaster),
            nameof(IRibbonSetUnique4.RibbonG4Cute),
            nameof(IRibbonSetUnique4.RibbonG4CuteGreat),
            nameof(IRibbonSetUnique4.RibbonG4CuteUltra),
            nameof(IRibbonSetUnique4.RibbonG4CuteMaster),
            nameof(IRibbonSetUnique4.RibbonG4Smart),
            nameof(IRibbonSetUnique4.RibbonG4SmartGreat),
            nameof(IRibbonSetUnique4.RibbonG4SmartUltra),
            nameof(IRibbonSetUnique4.RibbonG4SmartMaster),
            nameof(IRibbonSetUnique4.RibbonG4Tough),
            nameof(IRibbonSetUnique4.RibbonG4ToughGreat),
            nameof(IRibbonSetUnique4.RibbonG4ToughUltra),
            nameof(IRibbonSetUnique4.RibbonG4ToughMaster),
        };

        internal static bool[] RibbonBitsAbility(this IRibbonSetUnique4 set)
        {
            return new[]
            {
                set.RibbonAbility,
                set.RibbonAbilityGreat,
                set.RibbonAbilityDouble,
                set.RibbonAbilityMulti,
                set.RibbonAbilityPair,
                set.RibbonAbilityWorld,
            };
        }

        internal static bool[] RibbonBitsContest3(this IRibbonSetUnique4 set)
        {
            return new[]
            {
                set.RibbonG3Cool,
                set.RibbonG3CoolSuper,
                set.RibbonG3CoolHyper,
                set.RibbonG3CoolMaster,

                set.RibbonG3Beauty,
                set.RibbonG3BeautySuper,
                set.RibbonG3BeautyHyper,
                set.RibbonG3BeautyMaster,

                set.RibbonG3Cute,
                set.RibbonG3CuteSuper,
                set.RibbonG3CuteHyper,
                set.RibbonG3CuteMaster,

                set.RibbonG3Smart,
                set.RibbonG3SmartSuper,
                set.RibbonG3SmartHyper,
                set.RibbonG3SmartMaster,

                set.RibbonG3Tough,
                set.RibbonG3ToughSuper,
                set.RibbonG3ToughHyper,
                set.RibbonG3ToughMaster,
            };
        }

        internal static bool[] RibbonBitsContest4(this IRibbonSetUnique4 set)
        {
            return new[]
            {
                set.RibbonG4Cool,
                set.RibbonG4CoolGreat,
                set.RibbonG4CoolUltra,
                set.RibbonG4CoolMaster,

                set.RibbonG4Beauty,
                set.RibbonG4BeautyGreat,
                set.RibbonG4BeautyUltra,
                set.RibbonG4BeautyMaster,

                set.RibbonG4Cute,
                set.RibbonG4CuteGreat,
                set.RibbonG4CuteUltra,
                set.RibbonG4CuteMaster,

                set.RibbonG4Smart,
                set.RibbonG4SmartGreat,
                set.RibbonG4SmartUltra,
                set.RibbonG4SmartMaster,

                set.RibbonG4Tough,
                set.RibbonG4ToughGreat,
                set.RibbonG4ToughUltra,
                set.RibbonG4ToughMaster,
            };
        }

        internal static string[] RibbonNamesAbility(this IRibbonSetUnique4 _) => RibbonSetNamesUnique4Ability;
        internal static string[] RibbonNamesContest3(this IRibbonSetUnique4 _) => RibbonSetNamesUnique4Contest3;
        internal static string[] RibbonNamesContest4(this IRibbonSetUnique4 _) => RibbonSetNamesUnique4Contest4;
    }
}

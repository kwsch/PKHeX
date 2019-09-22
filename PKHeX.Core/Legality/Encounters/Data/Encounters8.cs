using static PKHeX.Core.EncounterUtil;

namespace PKHeX.Core
{
    internal static class Encounters8
    {
        internal static readonly EncounterArea8[] SlotsSW = GetEncounterTables<EncounterArea8>("gg", "gp");
        internal static readonly EncounterArea8[] SlotsSH = GetEncounterTables<EncounterArea8>("gg", "ge");
        internal static readonly EncounterStatic[] StaticSW, StaticSH;

        static Encounters8()
        {
            StaticSW = GetStaticEncounters(Encounter_SWSH, GameVersion.SW);
            StaticSH = GetStaticEncounters(Encounter_SWSH, GameVersion.SH);

            SlotsSW.SetVersion(GameVersion.SW);
            SlotsSH.SetVersion(GameVersion.SH);
            Encounter_SWSH.SetVersion(GameVersion.SWSH);
            TradeGift_SWSH.SetVersion(GameVersion.SWSH);
            MarkEncountersGeneration(8, SlotsSW, SlotsSH);
            MarkEncountersGeneration(8, StaticSW, StaticSH, TradeGift_SWSH);
        }

        private static readonly EncounterStatic[] Encounter_SWSH =
        {
            // encounters

            // gifts
        };

        internal static readonly EncounterTrade[] TradeGift_SWSH =
        {

        };
    }
}

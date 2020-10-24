using static PKHeX.Core.Encounters8Nest;

namespace PKHeX.Core
{
    /// <summary>
    /// Generation 8 Nest Encounter (Max Raid) Underground
    /// </summary>
    public sealed class EncounterStatic8U : EncounterStatic8Nest<EncounterStatic8U>
    {
        public override int Location { get => MaxLair; set { } }

        public EncounterStatic8U(int species, int form, int level, GameVersion game = GameVersion.SWSH)
        {
            Species = species;
            Form = form;
            Level = level;
            DynamaxLevel = 8;
            FlawlessIVCount = 4;
            Version = game;
        }

        public override bool IsMatch(PKM pkm, DexLevel evo)
        {
            if (pkm.FlawlessIVCount < FlawlessIVCount)
                return false;

            return base.IsMatch(pkm, evo);
        }
    }
}

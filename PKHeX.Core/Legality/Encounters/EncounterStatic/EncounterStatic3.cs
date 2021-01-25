using System;
using System.Linq;

namespace PKHeX.Core
{
    /// <summary>
    /// Generation 3 Static Encounter
    /// </summary>
    /// <inheritdoc cref="EncounterStatic"/>
    public sealed record EncounterStatic3 : EncounterStatic
    {
        public override int Generation => 3;
        public bool Roaming { get; init; }

        public EncounterStatic3(int species, int level, GameVersion game) : base(game)
        {
            Species = species;
            Level = level;
        }

        protected override bool IsMatchEggLocation(PKM pkm)
        {
            if (pkm.Format == 3)
                return !pkm.IsEgg || EggLocation == 0 || EggLocation == pkm.Met_Location;
            return pkm.Egg_Location == 0;
        }

        protected override bool IsMatchLevel(PKM pkm, DexLevel evo)
        {
            if (pkm.Format != 3) // Met Level lost on PK3=>PK4
                return Level <= evo.Level;

            if (EggEncounter)
                return pkm.Met_Level == 0 && pkm.CurrentLevel >= 5; // met level 0, origin level 5

            return pkm.Met_Level == Level;
        }

        protected override bool IsMatchLocation(PKM pkm)
        {
            if (EggEncounter)
                return true;
            if (pkm.Format != 3)
                return true; // transfer location verified later

            var met = pkm.Met_Location;
            if (!Roaming)
                return Location == met;

            var table = Version <= GameVersion.E ? Roaming_MetLocation_RSE : Roaming_MetLocation_FRLG;
            return table.Contains(met);
        }

        protected override bool IsMatchPartial(PKM pkm)
        {
            if (Gift && pkm.Ball != Ball)
                return true;
            return base.IsMatchPartial(pkm);
        }

        protected override void SetMetData(PKM pk, int level, DateTime today)
        {
            pk.Met_Level = level;
            pk.Met_Location = !Roaming ? Location : (Version <= GameVersion.E ? Roaming_MetLocation_RSE : Roaming_MetLocation_FRLG)[0];
        }

        private static readonly int[] Roaming_MetLocation_FRLG =
        {
            // Route 1-25 encounter is possible either in grass or on water
            101,102,103,104,105,106,107,108,109,110,
            111,112,113,114,115,116,117,118,119,120,
            121,122,123,124,125
        };

        private static readonly int[] Roaming_MetLocation_RSE =
        {
            // Roaming encounter is possible in tall grass and on water
            // Route 101-138
            16, 17, 18, 19, 20, 21, 22, 23, 24, 25,
            26, 27, 28, 29, 30, 31, 32, 33, 34, 35,
            36, 37, 38, 39, 40, 41, 42, 43, 44, 45,
            46, 47, 48, 49,
        };
    }
}

using System;
using System.Linq;

namespace PKHeX.Core
{
    public class EncounterStatic2 : EncounterStatic
    {
        public sealed override int Level { get; set; }

        public EncounterStatic2(int species, int level)
        {
            Species = species;
            Level = level;
        }

        protected override bool IsMatchEggLocation(PKM pkm)
        {
            if (pkm.Format > 2)
                return true;

            if (pkm.IsEgg)
            {
                if (pkm.Met_Location != 0 && pkm.Met_Level != 0)
                    return false;
                if (pkm.OT_Friendship > EggCycles) // Dizzy Punch eggs start with below-normal hatch counters.
                    return false;
            }
            else
            {
                switch (pkm.Met_Level)
                {
                    case 0 when pkm.Met_Location != 0:
                        return false;
                    case 1 when pkm.Met_Location == 0:
                        return false;
                    default:
                        if (pkm.Met_Location == 0 && pkm.Met_Level != 0)
                            return false;
                        break;
                }
            }

            return true;
        }

        protected override void SetMetData(PKM pk, int level, DateTime today)
        {
            pk.Met_Location = Location;
            pk.Met_Level = level;
            if (Version == GameVersion.C && pk is PK2 pk2)
                pk2.Met_TimeOfDay = EncounterTime.Any.RandomValidTime();
        }

        protected override bool IsMatchLevel(PKM pkm, DexLevel evo)
        {
            if (pkm is PK2 pk2 && pk2.CaughtData != 0)
                return pkm.Met_Level == (EggEncounter ? 1 : Level);

            return Level <= evo.Level;
        }

        protected override bool IsMatchLocation(PKM pkm)
        {
            if (EggEncounter)
                return true;
            if (Location == 0)
                return true;
            if (pkm is PK2 pk2 && pk2.CaughtData != 0)
                return Location == pkm.Met_Location;
            return true;
        }
    }

    public sealed class EncounterStatic2Odd : EncounterStatic2
    {
        private const int Dizzy = 146;
        private static readonly int[] _dizzy = { Dizzy };

        public EncounterStatic2Odd(int species) : base(species, 5)
        {
            Version = GameVersion.C;
            Moves = _dizzy;
            EggLocation = 256;
            EggCycles = 20;
        }

        public override bool IsMatch(PKM pkm, DexLevel evo)
        {
            // Let it get picked up as regular EncounterEgg under other conditions.
            if (pkm.Format > 2)
                return false;
            if (pkm.Move1 != Dizzy && pkm.Move2 != Dizzy && pkm.Move3 != Dizzy && pkm.Move4 != Dizzy)
                return false;
            if (pkm.IsEgg && pkm.EXP != 125)
                return false;
            return base.IsMatch(pkm, evo);
        }
    }

    public sealed class EncounterStatic2Roam : EncounterStatic2
    {
        private static readonly int[] Roaming_MetLocation_GSC_Grass =
        {
            // Routes 29, 30-31, 33, 34, 35, 36-37, 38-39, 42, 43, 44, 45-46 can be encountered in grass
            2, 4, 5, 8, 11, 15, 18, 20, 21,
            25, 26, 34, 37, 39, 43, 45,
        };

        public override int Location => Roaming_MetLocation_GSC_Grass[0];

        public EncounterStatic2Roam(int species, int level) : base(species, level) { }

        protected override bool IsMatchLocation(PKM pkm)
        {
            if (!pkm.HasOriginalMetLocation)
                return true;
            return Roaming_MetLocation_GSC_Grass.Contains(Location);
        }
    }
}

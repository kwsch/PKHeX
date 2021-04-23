using System;

namespace PKHeX.Core
{
    /// <summary>
    /// Generation 2 Static Encounter
    /// </summary>
    /// <inheritdoc cref="EncounterStatic"/>
    public record EncounterStatic2 : EncounterStatic
    {
        public sealed override int Generation => 2;
        public sealed override int Level { get; init; }

        public EncounterStatic2(int species, int level, GameVersion game) : base(game)
        {
            Species = species;
            Level = level;
        }

        public override bool IsMatchExact(PKM pkm, DexLevel evo)
        {
            if (Shiny == Shiny.Always && !pkm.IsShiny)
                return false;
            return base.IsMatchExact(pkm, evo);
        }

        protected override bool IsMatchEggLocation(PKM pkm)
        {
            if (pkm.Format > 2)
                return true;

            if (pkm.IsEgg)
            {
                if (!EggEncounter)
                    return false;
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

        protected override bool IsMatchLevel(PKM pkm, DexLevel evo)
        {
            if (pkm is ICaughtData2 {CaughtData: not 0})
                return pkm.Met_Level == (EggEncounter ? 1 : Level);

            return Level <= evo.Level;
        }

        protected override bool IsMatchLocation(PKM pkm)
        {
            if (EggEncounter)
                return true;
            if (Location == 0)
                return true;
            if (pkm is ICaughtData2 {CaughtData: not 0})
                return Location == pkm.Met_Location;
            return true;
        }

        protected override bool IsMatchPartial(PKM pkm) => false;

        protected override void ApplyDetails(ITrainerInfo sav, EncounterCriteria criteria, PKM pk)
        {
            base.ApplyDetails(sav, criteria, pk);
            var pk2 = (PK2)pk;
            if (Shiny == Shiny.Always)
                pk2.SetShiny();
        }

        protected override void SetMetData(PKM pk, int level, DateTime today)
        {
            if (Version != GameVersion.C && pk.OT_Gender != 1)
                return;
            var pk2 = (PK2)pk;
            pk2.Met_Location = Location;
            pk2.Met_Level = level;
            pk2.Met_TimeOfDay = EncounterTime.Any.RandomValidTime();
        }
    }

    public sealed record EncounterStatic2Odd : EncounterStatic2
    {
        private const int Dizzy = 146;
        private static readonly int[] _dizzy = { Dizzy };

        public EncounterStatic2Odd(int species) : base(species, 5, GameVersion.C)
        {
            Moves = _dizzy;
            EggLocation = 256;
            EggCycles = 20;
        }

        public override bool IsMatchExact(PKM pkm, DexLevel evo)
        {
            // Let it get picked up as regular EncounterEgg under other conditions.
            if (pkm.Format > 2)
                return false;
            if (!pkm.HasMove(Dizzy))
                return false;
            if (pkm.IsEgg && pkm.EXP != 125)
                return false;
            return base.IsMatchExact(pkm, evo);
        }
    }

    public sealed record EncounterStatic2Roam : EncounterStatic2
    {
        // Routes 29-46, except 40 & 41; total 16.
        // 02, 04, 05, 08, 11, 15, 18, 20,
        // 21, 25, 26, 34, 37, 39, 43, 45,
        private const ulong RoamLocations = 0b10_1000_1010_0100_0000_0110_0011_0100_1000_1001_0011_0100;
        public override int Location => 2;

        public EncounterStatic2Roam(int species, int level, GameVersion ver) : base(species, level, ver) { }

        protected override bool IsMatchLocation(PKM pkm)
        {
            if (!pkm.HasOriginalMetLocation)
                return true;
            // Gen2 met location is always u8
            var loc = pkm.Met_Location;
            return loc <= 45 && ((RoamLocations & (1UL << loc)) != 0);
        }
    }
}

using System.Collections.Generic;

namespace PKHeX.Core
{
    public abstract record EncounterTrade4 : EncounterTrade
    {
        public sealed override int Generation => 4;
    }

    public sealed record EncounterTrade4PID : EncounterTrade4, IContestStats, IContestStatsRecord
    {
        /// <summary>
        /// Fixed <see cref="PKM.PID"/> value the encounter must have.
        /// </summary>
        public readonly uint PID;

        public EncounterTrade4PID(uint pid, int species, int level)
        {
            PID = pid;
            Shiny = Shiny.FixedValue;
            Species = species;
            Level = level;
        }

        public int CNT_Cool { get; init; }
        public int CNT_Beauty { get; init; }
        public int CNT_Cute { get; init; }
        public int CNT_Smart { get; init; }
        public int CNT_Tough { get; init; }
        public int CNT_Sheen { get; init; }

        public IReadOnlyList<int> Contest
        {
            init
            {
                CNT_Cool = value[0];
                CNT_Beauty = value[1];
                CNT_Cute = value[2];
                CNT_Smart = value[3];
                CNT_Tough = value[4];
                CNT_Sheen = value[5];
            }
        }

        public override bool IsMatch(PKM pkm, DexLevel evo)
        {
            if (!base.IsMatch(pkm, evo))
                return false;

            if (pkm is IContestStats s && s.IsContestBelow(this))
                return false;

            return true;
        }

        protected override void ApplyDetails(ITrainerInfo sav, EncounterCriteria criteria, PKM pk)
        {
            base.ApplyDetails(sav, criteria, pk);
            this.CopyContestStatsTo((PK4)pk);
        }

        protected override void SetPINGA(PKM pk, EncounterCriteria criteria)
        {
            pk.PID = PID;
            pk.Nature = (int)(PID % 25);
            pk.Gender = Gender;
            pk.RefreshAbility(Ability >> 1);
            SetIVs(pk);
        }

        protected override bool IsMatchNatureGenderShiny(PKM pkm)
        {
            return PID == pkm.EncryptionConstant;
        }
    }

    public sealed record EncounterTrade4RanchGift : EncounterTrade4
    {
        /// <summary>
        /// Fixed <see cref="PKM.PID"/> value the encounter must have.
        /// </summary>
        public readonly uint PID;

        public EncounterTrade4RanchGift(uint pid, int species, int level)
        {
            PID = pid;
            Shiny = Shiny.FixedValue;
            Species = species;
            Level = level;
            Version = GameVersion.D;
        }

        protected override bool IsMatchNatureGenderShiny(PKM pkm)
        {
            return PID == pkm.EncryptionConstant;
        }

        protected override void SetPINGA(PKM pk, EncounterCriteria criteria)
        {
            pk.PID = PID;
            pk.Nature = (int)(PID % 25);
            pk.Gender = Gender;
            pk.RefreshAbility(Ability >> 1);
            SetIVs(pk);
        }
    }

    public sealed record EncounterTrade4RanchSpecial : EncounterTrade4
    {
        public EncounterTrade4RanchSpecial(int species, int level)
        {
            Species = species;
            Level = level;
            Ball = 0x10;
            Version = GameVersion.D;
            OTGender = 1;
            Location = 3000;
        }

        protected override void ApplyDetails(ITrainerInfo sav, EncounterCriteria criteria, PKM pk)
        {
            base.ApplyDetails(sav, criteria, pk);
            pk.FatefulEncounter = true;
        }
    }
}

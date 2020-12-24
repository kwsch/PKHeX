using System.Collections.Generic;

namespace PKHeX.Core
{
    public sealed record EncounterTrade4PID : EncounterTrade, IContestStats, IContestStatsRecord
    {
        public override int Generation => 4;

        /// <summary>
        /// Fixed <see cref="PKM.PID"/> value the encounter must have.
        /// </summary>
        public readonly uint PID;

        public EncounterTrade4PID(uint pid)
        {
            PID = pid;
            Shiny = Shiny.FixedValue;
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
            var pi = pk.PersonalInfo;
            int gender = criteria.GetGender(PKX.GetGenderFromPID(Species, PID), pi);
            int nature = (int)criteria.GetNature(Nature);
            int ability = criteria.GetAbilityFromNumber(Ability, pi);

            pk.PID = PID;
            pk.Nature = nature;
            pk.Gender = gender;
            pk.RefreshAbility(ability);

            SetIVs(pk);
        }

        protected override bool IsMatchNatureGenderShiny(PKM pkm)
        {
            return PID == pkm.EncryptionConstant;
        }
    }

    public sealed record EncounterTrade4Ranch : EncounterTrade
    {
        public override int Generation => 4;

        public EncounterTrade4Ranch(int species, int level)
        {
            Species = species;
            Level = level;
            Ball = 0x10;
        }

        protected override void ApplyDetails(ITrainerInfo sav, EncounterCriteria criteria, PKM pk)
        {
            base.ApplyDetails(sav, criteria, pk);
            pk.FatefulEncounter = true;
        }
    }
}

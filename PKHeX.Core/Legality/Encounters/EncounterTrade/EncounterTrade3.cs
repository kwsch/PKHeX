using System.Collections.Generic;

namespace PKHeX.Core
{
    public sealed record EncounterTrade3 : EncounterTrade, IContestStats
    {
        public override int Generation => 3;
        public override int Location => Locations.LinkTrade3NPC;

        /// <summary>
        /// Fixed <see cref="PKM.PID"/> value the encounter must have.
        /// </summary>
        public readonly uint PID;

        public byte CNT_Cool   { get; private init; }
        public byte CNT_Beauty { get; private init; }
        public byte CNT_Cute   { get; private init; }
        public byte CNT_Smart  { get; private init; }
        public byte CNT_Tough  { get; private init; }
        public byte CNT_Sheen  { get; private init; }

        public IReadOnlyList<byte> Contest
        {
            init
            {
                CNT_Cool   = value[0];
                CNT_Beauty = value[1];
                CNT_Cute   = value[2];
                CNT_Smart  = value[3];
                CNT_Tough  = value[4];
                CNT_Sheen  = value[5];
            }
        }

        public EncounterTrade3(GameVersion game, uint pid, int species, int level) : base(game)
        {
            PID = pid;
            Shiny = Shiny.FixedValue;
            Species = species;
            Level = level;
        }

        public override bool IsMatchExact(PKM pkm, DexLevel evo)
        {
            if (!base.IsMatchExact(pkm, evo))
                return false;

            if (pkm is IContestStats s && s.IsContestBelow(this))
                return false;

            return true;
        }

        protected override void ApplyDetails(ITrainerInfo sav, EncounterCriteria criteria, PKM pk)
        {
            base.ApplyDetails(sav, criteria, pk);
            var pk3 = (PK3) pk;

            // Italian LG Jynx untranslated from English name
            if (Species == (int)Core.Species.Jynx && pk3.Version == (int)GameVersion.LG && pk3.Language == (int)LanguageID.Italian)
            {
                pk3.OT_Name = GetOT((int)LanguageID.English);
                pk3.SetNickname(GetNickname((int)LanguageID.English));
            }

            this.CopyContestStatsTo((PK3)pk);
        }

        protected override void SetPINGA(PKM pk, EncounterCriteria criteria)
        {
            var pi = pk.PersonalInfo;
            int gender = criteria.GetGender(PKX.GetGenderFromPID(Species, PID), pi);
            int nature = (int)criteria.GetNature(Nature);
            int ability = criteria.GetAbilityFromNumber(Ability);

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
}

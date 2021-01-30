using static PKHeX.Core.Species;

namespace PKHeX.Core
{
    public sealed record EncounterTrade2 : EncounterTradeGB
    {
        public override int Generation => 2;

        public EncounterTrade2(int species, int level, int tid) : base(species, level, GameVersion.GSC)
        {
            TID = tid;
        }

        public override bool IsMatchExact(PKM pkm, DexLevel dl)
        {
            if (Level > pkm.CurrentLevel) // minimum required level
                return false;
            if (TID != pkm.TID)
                return false;

            if (pkm.Format <= 2)
            {
                if (Gender >= 0 && Gender != pkm.Gender)
                    return false;
                if (IVs.Count != 0 && !Legal.GetIsFixedIVSequenceValidNoRand(IVs, pkm))
                    return false;
                if (pkm.Format == 2 && pkm.Met_Location is not (0 or 126))
                    return false;
            }

            if (!IsValidTradeOTGender(pkm))
                return false;
            return IsValidTradeOTName(pkm);
        }

        private bool IsValidTradeOTGender(PKM pkm)
        {
            if (OTGender == 1)
            {
                // Female, can be cleared if traded to RBY (clears met location)
                if (pkm.Format <= 2)
                    return pkm.OT_Gender == (pkm.Met_Location != 0 ? 1 : 0);
                return pkm.OT_Gender == 0 || !pkm.VC1; // require male except if transferred from GSC
            }
            return pkm.OT_Gender == 0;
        }

        private bool IsValidTradeOTName(PKM pkm)
        {
            var OT = pkm.OT_Name;
            if (pkm.Japanese)
                return GetOT((int)LanguageID.Japanese) == OT;
            if (pkm.Korean)
                return GetOT((int)LanguageID.Korean) == OT;

            var lang = GetInternationalLanguageID(OT);
            if (pkm.Format < 7)
                return lang != -1;

            switch (Species)
            {
                case (int)Voltorb when pkm.Language == (int)LanguageID.French:
                    if (lang == (int)LanguageID.Spanish)
                        return false;
                    if (lang != -1)
                        return true;
                    return OT == "FALCçN"; // FALCÁN

                case (int)Shuckle when pkm.Language == (int)LanguageID.French:
                    if (lang == (int)LanguageID.Spanish)
                        return false;
                    if (lang != -1)
                        return true;
                    return OT == "MANôA"; // MANÍA

                default: return lang != -1;
            }
        }

        private int GetInternationalLanguageID(string OT)
        {
            const int start = (int)LanguageID.English;
            const int end = (int)LanguageID.Spanish;

            var tr = TrainerNames;
            for (int i = start; i <= end; i++)
            {
                if (tr[i] == OT)
                    return i;
            }
            return -1;
        }
    }
}

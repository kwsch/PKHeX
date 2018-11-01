using System;
using System.Linq;

namespace PKHeX.Core
{
    public abstract class Zukan
    {
        protected SaveFile SAV { get; set; }
        protected int PokeDex { get; set; }
        protected int PokeDexLanguageFlags { get; set; }

        protected abstract int OFS_SEEN { get; }
        protected abstract int OFS_CAUGHT { get; }
        protected abstract int BitSeenSize { get; }
        protected abstract int DexLangFlagByteCount { get; }
        protected abstract int DexLangIDCount { get; }
        protected abstract int GetDexLangFlag(int lang);

        protected Func<int, int, int, int> DexFormIndexFetcher { get; set; }

        protected abstract bool GetSaneFormsToIterate(int species, out int formStart, out int formEnd, int formIn);
        protected abstract void SetSpindaDexData(PKM pkm, bool alreadySeen);
        protected abstract void SetAllDexFlagsLanguage(int bit, int lang, bool value = true);
        protected abstract void SetAllDexSeenFlags(int baseBit, int altform, int gender, bool isShiny, bool value = true);

        public bool GetFlag(int ofs, int bitIndex) => SAV.GetFlag(PokeDex + ofs, bitIndex);
        public void SetFlag(int ofs, int bitIndex, bool value = true) => SAV.SetFlag(PokeDex + ofs, bitIndex, value);

        public bool GetCaught(int species) => GetFlag(OFS_CAUGHT, species - 1);
        public void SetCaught(int species, bool value = true) => SetFlag(OFS_CAUGHT, species - 1, value);

        public int SeenCount => Enumerable.Range(1, SAV.MaxSpeciesID).Count(GetSeen);
        public int CaughtCount => Enumerable.Range(1, SAV.MaxSpeciesID).Count(GetCaught);
        public decimal PercentSeen => (decimal)SeenCount / SAV.MaxSpeciesID;
        public decimal PercentCaught => (decimal)CaughtCount / SAV.MaxSpeciesID;

        public bool GetSeen(int species)
        {
            // check all 4 seen flags (gender/shiny)
            for (int i = 0; i < 4; i++)
            {
                if (GetFlag(OFS_SEEN + (i * BitSeenSize), species - 1))
                    return true;
            }
            return false;
        }

        public void SetSeen(int species, bool value = true)
        {
            if (!value)
            {
                ClearSeen(species);
                return;
            }

            // check all 4 seen flags (gender/shiny)
            for (int i = 0; i < 4; i++)
            {
                if (GetFlag(OFS_SEEN + (i * BitSeenSize), species - 1))
                    return;
            }
            var gender = SAV.Personal[species].RandomGender & 1;
            SetAllDexSeenFlags(species - 1, 0, gender, false);
        }

        private void ClearSeen(int species)
        {
            SetCaught(species, false);
            for (int i = 0; i < 4; i++)
                SetFlag(OFS_SEEN + (i * BitSeenSize), species - 1, false);
        }

        public virtual void SetDex(PKM pkm)
        {
            if (PokeDex < 0 || SAV.Version == GameVersion.Invalid) // sanity
                return;
            if (pkm.Species == 0 || pkm.Species > SAV.MaxSpeciesID) // out of range
                return;
            if (pkm.IsEgg) // do not add
                return;

            int bit = pkm.Species - 1;
            SetCaught(pkm.Species); // Set the Owned Flag
            if (pkm.Species == 327) // Spinda
                SetSpindaDexData(pkm, GetSeen(pkm.Species));
            SetAllDexSeenFlags(bit, pkm.AltForm, pkm.Gender & 1, pkm.IsShiny); // genderless -> male
            SetAllDexFlagsLanguage(bit, pkm.Language);
        }

        protected void SetDexFlags(int baseBit, int formBit, int gender, int shiny, bool value = true)
        {
            int shift = gender | (shiny << 1);

            // Set the [Species/Gender/Shiny] Seen Flag
            SetFlag(OFS_SEEN + (shift * BitSeenSize), baseBit, value);

            // Set the Display flag if none are set
            bool displayed = GetIsSpeciesFormAnyDisplayed(baseBit, formBit);
            if (!displayed || !value)
                SetFlag(OFS_SEEN + ((4 + shift) * BitSeenSize), formBit, value);
        }

        private bool GetIsSpeciesFormAnyDisplayed(int baseBit, int formBit)
        {
            // Check Displayed Status for base form
            for (int i = 4; i < 8; i++)
            {
                if (GetFlag(OFS_SEEN + (i * BitSeenSize), baseBit))
                    return true;
            }
            if (baseBit == formBit)
                return false;

            // If form is not base form, check form too
            for (int i = 4; i < 8; i++)
            {
                if (GetFlag(OFS_SEEN + (i * BitSeenSize), formBit))
                    return true;
            }
            return false;
        }

        // Bulk Manipulation

        public void SetDexEntriesAll(bool value = true, int max = -1)
        {
            if (max <= 0)
                max = SAV.MaxSpeciesID;
            for (int i = 1; i <= max; i++)
                SetDexEntriesSingle(i, value);
        }

        public void SetDexEntriesSingle(int species, bool value = true)
        {
            SetCaught(species, value);
            SetSeen(species, value);

            var entry = SAV.Personal[species];
            int baseBit = species - 1;
            int fc = entry.FormeCount;
            for (int f = 0; f < fc; f++)
            {
                if (!entry.OnlyFemale)
                {
                    SetAllDexSeenFlags(baseBit, f, 0, false, value);
                    SetAllDexSeenFlags(baseBit, f, 0, true, value);
                }
                if (!entry.OnlyMale && !entry.Genderless)
                {
                    SetAllDexSeenFlags(baseBit, f, 1, false, value);
                    SetAllDexSeenFlags(baseBit, f, 1, true, value);
                }
            }
            SetAllDexFlagsLanguage(baseBit, value);
        }

        protected void SetAllDexFlagsLanguage(int bit, bool value = true)
        {
            for (int i = 1; i <= DexLangIDCount + 1; i++)
                SetAllDexFlagsLanguage(bit, i, value);
        }
    }
}
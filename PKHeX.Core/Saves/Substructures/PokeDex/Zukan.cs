using System;
using System.Linq;

namespace PKHeX.Core
{
    /// <summary>
    /// Base class for Pokédex logic operations.
    /// </summary>
    public abstract class ZukanBase
    {
        protected readonly SaveFile SAV;
        public readonly int PokeDex;

        protected ZukanBase(SaveFile sav, int dex)
        {
            SAV = sav;
            PokeDex = dex;
        }

        #region Overall Info
        /// <summary> Count of unique Species Seen </summary>
        public int SeenCount => Enumerable.Range(1, SAV.MaxSpeciesID).Count(GetSeen);
        /// <summary> Count of unique Species Caught (Owned) </summary>
        public int CaughtCount => Enumerable.Range(1, SAV.MaxSpeciesID).Count(GetCaught);

        public decimal PercentSeen => (decimal)SeenCount / SAV.MaxSpeciesID;
        public decimal PercentCaught => (decimal)CaughtCount / SAV.MaxSpeciesID;
        #endregion

        /// <summary> Gets if the Species has been Seen by the player. </summary>
        public abstract bool GetSeen(int species);
        /// <summary> Gets if the Species has been Caught (Owned) by the player. </summary>
        public abstract bool GetCaught(int species);

        /// <summary> Adds the Pokémon's information to the Pokédex. </summary>
        public abstract void SetDex(PKM pkm);

        #region Overall Manipulation
        public abstract void SeenNone();
        public abstract void CaughtNone();

        public abstract void SeenAll(bool shinyToo = false);
        public abstract void CompleteDex(bool shinyToo = false);
        public abstract void CaughtAll(bool shinyToo = false);
        public abstract void SetAllSeen(bool value = true, bool shinyToo = false);

        public abstract void SetDexEntryAll(int species, bool shinyToo = false);
        public abstract void ClearDexEntryAll(int species);
        #endregion
    }

    /// <summary>
    /// Base class for Pokédex operations, exposing the shared structure features used by Generations 5, 6, and 7.
    /// </summary>
    public abstract class Zukan : ZukanBase
    {
        protected readonly int PokeDexLanguageFlags;

        protected Zukan(SaveFile sav, int dex, int langflag) : base(sav, dex)
        {
            PokeDexLanguageFlags = langflag;
            if (langflag > dex)
                throw new ArgumentException(nameof(langflag));
        }

        protected abstract int OFS_SEEN { get; }
        protected abstract int OFS_CAUGHT { get; }
        protected abstract int BitSeenSize { get; }
        protected abstract int DexLangFlagByteCount { get; }
        protected abstract int DexLangIDCount { get; }
        protected abstract int GetDexLangFlag(int lang);

        protected abstract bool GetSaneFormsToIterate(int species, out int formStart, out int formEnd, int formIn);
        protected virtual void SetSpindaDexData(PKM pkm, bool alreadySeen) { }
        protected abstract void SetAllDexFlagsLanguage(int bit, int lang, bool value = true);
        protected abstract void SetAllDexSeenFlags(int baseBit, int form, int gender, bool isShiny, bool value = true);

        protected bool GetFlag(int ofs, int bitIndex) => SAV.GetFlag(PokeDex + ofs + (bitIndex >> 3), bitIndex);
        protected void SetFlag(int ofs, int bitIndex, bool value = true) => SAV.SetFlag(PokeDex + ofs + (bitIndex >> 3), bitIndex, value);

        public override bool GetCaught(int species) => GetFlag(OFS_CAUGHT, species - 1);
        public virtual void SetCaught(int species, bool value = true) => SetFlag(OFS_CAUGHT, species - 1, value);

        public override bool GetSeen(int species)
        {
            // check all 4 seen flags (gender/shiny)
            for (int i = 0; i < 4; i++)
            {
                if (GetSeen(species, i))
                    return true;
            }
            return false;
        }

        public bool GetSeen(int species, int i) => GetFlag(OFS_SEEN + (i * BitSeenSize), species - 1);
        public void SetSeen(int species, int i, bool value) => SetFlag(OFS_SEEN + (i * BitSeenSize), species - 1, value);

        public bool GetDisplayed(int bit, int i) => GetFlag(OFS_SEEN + ((i + 4) * BitSeenSize), bit);
        public void SetDisplayed(int bit, int i, bool value) => SetFlag(OFS_SEEN + ((i + 4) * BitSeenSize), bit, value);

        public bool GetLanguageFlag(int bit, int lang) => GetFlag(PokeDexLanguageFlags, (bit * DexLangIDCount) + lang);
        public void SetLanguageFlag(int bit, int lang, bool value) => SetFlag(PokeDexLanguageFlags, (bit * DexLangIDCount) + lang, value);

        public virtual void SetSeen(int species, bool value = true)
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
            var gender = SAV.Personal[species].RandomGender() & 1;
            SetAllDexSeenFlags(species - 1, 0, gender, false);
        }

        private void ClearSeen(int species)
        {
            SetCaught(species, false);
            for (int i = 0; i < 4; i++)
                SetFlag(OFS_SEEN + (i * BitSeenSize), species - 1, false);
        }

        public override void SetDex(PKM pkm)
        {
            if ((uint)(pkm.Species - 1) >= (uint)SAV.MaxSpeciesID) // out of range
                return;
            if (pkm.IsEgg) // do not add
                return;

            int species = pkm.Species;
            if (species == (int)Species.Spinda)
                SetSpindaDexData(pkm, GetSeen(species));

            int bit = pkm.Species - 1;
            int form = pkm.Form;
            int gender = pkm.Gender & 1;
            bool shiny = pkm.IsShiny;
            int lang = pkm.Language;
            SetDex(species, bit, form, gender, shiny, lang);
        }

        protected virtual void SetDex(int species, int bit, int form, int gender, bool shiny, int lang)
        {
            SetCaught(species); // Set the Owned Flag
            SetAllDexSeenFlags(bit, form, gender, shiny); // genderless -> male
            SetAllDexFlagsLanguage(bit, lang);
        }

        protected void SetDexFlags(int baseBit, int formBit, int gender, int shiny, bool value = true)
        {
            int shift = (gender & 1) | (shiny << 1);

            // Set the [Species/Gender/Shiny] Seen Flag
            SetFlag(OFS_SEEN + (shift * BitSeenSize), baseBit, value);

            // Set the Display flag if none are set
            SetDisplayedFlag(baseBit, formBit, value, shift);
        }

        protected virtual void SetDisplayedFlag(int baseBit, int formBit, bool value, int shift)
        {
            var bit = formBit >= 0 ? formBit : baseBit;
            if (!value)
            {
                SetDisplayed(bit, shift, false);
                return;
            }

            bool displayed = GetIsSpeciesFormAnyDisplayed(baseBit, formBit);
            if (displayed)
                return; // no need to set another bit

            SetDisplayed(bit, shift, true);
        }

        private bool GetIsSpeciesFormAnyDisplayed(int baseBit, int formBit)
        {
            // Check Displayed Status for base form
            for (int i = 0; i < 4; i++)
            {
                if (GetDisplayed(baseBit, i))
                    return true;
            }
            if (baseBit == formBit)
                return false;

            // If form is not base form, check form too
            for (int i = 0; i < 4; i++)
            {
                if (GetDisplayed(formBit, i))
                    return true;
            }
            return false;
        }

        // Bulk Manipulation
        public override void SeenNone() => SetDexEntriesAll(false, shinyToo: true);
        public override void CaughtNone() => SetAllCaught(false, true);
        public override void SeenAll(bool shinyToo = false) => SetAllSeen(shinyToo);
        public override void CompleteDex(bool shinyToo = false) => SetDexEntriesAll(shinyToo: shinyToo);

        public override void CaughtAll(bool shinyToo = false)
        {
            SetAllSeen(true, shinyToo);
            SetAllCaught(true, shinyToo);
        }

        public void SetAllCaught(bool value = true, bool shinyToo = false)
        {
            for (int i = 0; i < SAV.MaxSpeciesID; i++)
            {
                int species = i + 1;
                SetCaught(species, value); // Set the Owned Flag
                SetSeenSingle(i + 1, value, shinyToo);
            }
        }

        public override void SetAllSeen(bool value = true, bool shinyToo = false)
        {
            for (int i = 0; i < SAV.MaxSpeciesID; i++)
                SetSeenSingle(i + 1, value, shinyToo);
        }

        public override void SetDexEntryAll(int species, bool shinyToo = false)
        {
            SetSeenSingle(species, true, shinyToo);
            SetCaughtSingle(species);
        }

        public override void ClearDexEntryAll(int species)
        {
            SetSeenSingle(species, false);
            SetCaughtSingle(species, false);
        }

        public void SetDexEntriesAll(bool value = true, int max = -1, bool shinyToo = false)
        {
            if (max <= 0)
                max = SAV.MaxSpeciesID;

            for (int i = 1; i <= max; i++)
            {
                SetSeenSingle(i, value, shinyToo);
                SetCaughtSingle(i, value);
            }
        }

        public void SetCaughtSingle(int species, bool value = true)
        {
            SetCaught(species, value);
            int baseBit = species - 1;
            SetAllDexFlagsLanguage(baseBit, value);
        }

        public void SetSeenSingle(int species, bool seen = true, bool shinyToo = false)
        {
            SetSeen(species, seen);

            var entry = SAV.Personal[species];
            int baseBit = species - 1;
            int fc = entry.FormCount;
            for (int f = 0; f < fc; f++)
            {
                if (!entry.OnlyFemale)
                {
                    SetAllDexSeenFlags(baseBit, f, 0, false, seen);
                    if (shinyToo)
                        SetAllDexSeenFlags(baseBit, f, 0, true, seen);
                }
                if (!entry.OnlyMale && !entry.Genderless)
                {
                    SetAllDexSeenFlags(baseBit, f, 1, false, seen);
                    if (shinyToo)
                        SetAllDexSeenFlags(baseBit, f, 1, true, seen);
                }
            }
        }

        protected void SetAllDexFlagsLanguage(int bit, bool value = true)
        {
            for (int i = 1; i <= DexLangIDCount + 1; i++)
                SetAllDexFlagsLanguage(bit, i, value);
        }
    }
}
using System;

namespace PKHeX.Core
{
    public class Zukan6 : Zukan
    {
        protected override int OFS_SEEN => OFS_CAUGHT + BitSeenSize;
        protected override int OFS_CAUGHT => 0x8;
        protected override int BitSeenSize => 0x60;
        protected override int DexLangFlagByteCount => 7;
        protected override int DexLangIDCount => 7;

        public Zukan6(SaveFile sav, int dex, int langflag)
        {
            SAV = sav;
            PokeDex = dex;
            PokeDexLanguageFlags = langflag;
            var wrap = SAV.ORAS ? DexFormUtil.GetDexFormIndexORAS : (Func<int,int,int>)DexFormUtil.GetDexFormIndexXY;
            DexFormIndexFetcher = (spec, form, _) => wrap(spec, form);
        }

        protected override int GetDexLangFlag(int lang)
        {
            lang--;
            if (lang > 5)
                lang--; // 0-6 language vals
            if ((uint)lang > 5)
                return -1;
            return lang;
        }

        protected override bool GetSaneFormsToIterate(int species, out int formStart, out int formEnd, int formIn)
        {
            formStart = 0;
            formEnd = 0;
            return false;
        }

        protected override void SetSpindaDexData(PKM pkm, bool alreadySeen)
        {

        }

        protected override void SetAllDexFlagsLanguage(int bit, int lang, bool value = true)
        {
            lang = GetDexLangFlag(lang);
            if (lang < 0)
                return;

            int lbit = (bit * DexLangIDCount) + lang;
            if (lbit < DexLangFlagByteCount << 3) // Sanity check for max length of region
                SetFlag(PokeDexLanguageFlags, lbit, value);
        }

        protected override void SetAllDexSeenFlags(int baseBit, int altform, int gender, bool isShiny, bool value = true)
        {
            var shiny = isShiny ? 1 : 0;
            SetDexFlags(baseBit, 0, gender, shiny);
            SetFormFlags(baseBit + 1, altform, shiny, value);
        }

        public override void SetDex(PKM pkm)
        {
            if (PokeDex < 0)
                return;
            if (pkm.Species == 0)
                return;
            if (pkm.Species > SAV.MaxSpeciesID)
                return;
            if (SAV.Version == GameVersion.Invalid)
                return;

            int bit = pkm.Species - 1;
            SetCaughtFlag(bit, pkm.Version);

            // Set the [Species/Gender/Shiny] Seen Flag
            SetAllDexSeenFlags(pkm.Species - 1, pkm.AltForm, pkm.Gender, pkm.IsShiny);
            SetAllDexFlagsLanguage(bit, pkm.Language);
            SetFormFlags(pkm);
        }

        private void SetCaughtFlag(int bit, int origin)
        {
            // Owned quality flag
            if (SAV.ORAS)
            {
                SetFlag(OFS_CAUGHT, bit);
                // Set DexNav count (only if not encountered previously)
                var count = ((SAV6)SAV).GetEncounterCount(bit);
                if (count == 0)
                    ((SAV6)SAV).SetEncounterCount(bit, 1);
            }
            else
            {
                // Species: 1-649 for X/Y, and not for ORAS; Set the Foreign Owned Flag
                if (origin < 24 && bit < 649)
                    SetFlag(OFS_CAUGHT + 0x644, bit);
                else
                    SetFlag(OFS_CAUGHT, bit);
            }
        }

        private int FormLen => SAV.ORAS ? 0x26 : 0x18;
        private int FormDex => PokeDex + 0x8 + (BitSeenSize * 9);

        private void SetFormFlags(PKM pkm)
        {
            int species = pkm.Species;
            int form = pkm.AltForm;
            var shiny = pkm.IsShiny ? 1 : 0;
            SetFormFlags(species, form, shiny);
        }

        private void SetFormFlags(int species, int form, int shiny, bool value = true)
        {
            int fc = SAV.Personal[species].FormeCount;
            int f = DexFormIndexFetcher(species, fc, SAV.MaxSpeciesID - 1);
            if (f < 0)
                return;

            var bit = f + form;

            // Set Form Seen Flag
            SetFlag(FormDex + (FormLen * shiny), bit, value);

            // Set Displayed Flag if necessary, check all flags
            if (!value || !GetIsFormDisplayed(f, fc))
                SetFlag(FormDex + (FormLen * (2 + shiny)), bit, value);
        }

        private bool GetIsFormDisplayed(int f, int fc)
        {
            for (int i = 0; i < fc; i++)
            {
                var bit2 = f + i;
                if (GetFlag(FormDex + (FormLen * 2), bit2)) // Nonshiny
                    return true; // already set
                if (GetFlag(FormDex + (FormLen * 3), bit2)) // Shiny
                    return true; // already set
            }
            return false;
        }

        public bool[] GetLanguageBitflags(int species)
        {
            var result = new bool[DexLangIDCount];
            int bit = species - 1;
            for (int i = 0; i < DexLangIDCount; i++)
            {
                int lbit = (bit * DexLangIDCount) + i;
                result[i] = GetFlag(PokeDexLanguageFlags, lbit);
            }
            return result;
        }

        public void SetLanguageBitflags(int species, bool[] value)
        {
            int bit = species - 1;
            for (int i = 0; i < DexLangIDCount; i++)
            {
                int lbit = (bit * DexLangIDCount) + i;
                SetFlag(PokeDexLanguageFlags, lbit, value[i]);
            }
        }

        public void ToggleLanguageFlagsAll(bool value)
        {
            var arr = GetBlankLanguageBits(value);
            for (int i = 1; i <= SAV.MaxSpeciesID; i++)
                SetLanguageBitflags(i, arr);
        }

        public void ToggleLanguageFlagsSingle(int species, bool value)
        {
            var arr = GetBlankLanguageBits(value);
            SetLanguageBitflags(species, arr);
        }

        private bool[] GetBlankLanguageBits(bool value)
        {
            var result = new bool[DexLangIDCount];
            for (int i = 0; i < DexLangIDCount; i++)
                result[i] = value;
            return result;
        }
    }
}
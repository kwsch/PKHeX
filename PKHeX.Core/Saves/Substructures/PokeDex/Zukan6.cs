using System;
using System.Diagnostics;

namespace PKHeX.Core
{
    public abstract class Zukan6 : Zukan
    {
        protected override int OFS_SEEN => OFS_CAUGHT + BitSeenSize;
        protected override int OFS_CAUGHT => 0x8;
        protected override int BitSeenSize => 0x60;
        protected override int DexLangFlagByteCount => 631; // 721 * 7, rounded up
        protected override int DexLangIDCount => 7;
        protected int SpindaOffset { get; init; }

        protected Zukan6(SAV6XY sav, int dex, int langflag) : base(sav, dex, langflag)
        {
            DexFormIndexFetcher = DexFormUtil.GetDexFormIndexXY;
        }

        private Func<int, int, int> DexFormIndexFetcher { get; }

        protected Zukan6(SAV6AO sav, int dex, int langflag) : base(sav, dex, langflag)
        {
            DexFormIndexFetcher = DexFormUtil.GetDexFormIndexORAS;
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

        protected override void SetAllDexSeenFlags(int baseBit, int form, int gender, bool isShiny, bool value = true)
        {
            var shiny = isShiny ? 1 : 0;
            SetDexFlags(baseBit, baseBit, gender, shiny);
            SetFormFlags(baseBit + 1, form, shiny, value);
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
            SetAllDexSeenFlags(pkm.Species - 1, pkm.Form, pkm.Gender, pkm.IsShiny);
            SetAllDexFlagsLanguage(bit, pkm.Language);
            SetFormFlags(pkm);
        }

        protected abstract void SetCaughtFlag(int bit, int origin);

        private int FormLen => SAV is SAV6XY ? 0x18 : 0x26;
        private int FormDex => 0x8 + (BitSeenSize * 9);
        public bool GetFormFlag(int formIndex, int flagRegion) => GetFlag(FormDex + (FormLen * flagRegion), formIndex);
        public void SetFormFlag(int formIndex, int flagRegion, bool value = true) => SetFlag(FormDex + (FormLen * flagRegion), formIndex, value);

        private void SetFormFlags(PKM pkm)
        {
            int species = pkm.Species;
            int form = pkm.Form;
            var shiny = pkm.IsShiny ? 1 : 0;
            SetFormFlags(species, form, shiny);
        }

        private void SetFormFlags(int species, int form, int shiny, bool value = true)
        {
            int fc = SAV.Personal[species].FormCount;
            int f = DexFormIndexFetcher(species, fc);
            if (f < 0)
                return;

            var bit = f + form;

            // Set Form Seen Flag
            SetFormFlag(bit, shiny, value);

            // Set Displayed Flag if necessary, check all flags
            if (!value || !GetIsFormDisplayed(f, fc))
                SetFormFlag(bit, 2 + shiny, value);
        }

        private bool GetIsFormDisplayed(int f, int fc)
        {
            for (int i = 0; i < fc; i++)
            {
                var index = f + i;
                if (GetFormFlag(index, 2)) // Nonshiny
                    return true; // already set
                if (GetFormFlag(index, 3)) // Shiny
                    return true; // already set
            }
            return false;
        }

        public uint SpindaPID
        {
            get => BitConverter.ToUInt32(SAV.Data, PokeDex + SpindaOffset);
            set => SAV.SetData(BitConverter.GetBytes(value), PokeDex + SpindaOffset);
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

    public sealed class Zukan6AO : Zukan6
    {
        public Zukan6AO(SAV6AO sav, int dex, int langflag) : base(sav, dex, langflag)
        {
            SpindaOffset = 0x680;
        }

        protected override void SetCaughtFlag(int bit, int origin)
        {
            SetFlag(OFS_CAUGHT, bit);
            if (GetEncounterCount(bit) == 0)
                SetEncounterCount(bit, 1);
        }

        public ushort GetEncounterCount(int index)
        {
            var ofs = PokeDex + 0x686 + (index * 2);
            return BitConverter.ToUInt16(SAV.Data, ofs);
        }

        public void SetEncounterCount(int index, ushort value)
        {
            var ofs = PokeDex + 0x686 + (index * 2);
            var data = BitConverter.GetBytes(value);
            SAV.SetData(data, ofs);
        }
    }

    public sealed class Zukan6XY : Zukan6
    {
        public Zukan6XY(SAV6XY sav, int dex, int langflag) : base(sav, dex, langflag)
        {
            SpindaOffset = 0x648;
        }

        protected override void SetCaughtFlag(int bit, int origin)
        {
            // Species: 1-649 for X/Y, and not for ORAS; Set the Foreign Owned Flag
            if (origin < (int)GameVersion.X && bit < (int)Species.Genesect)
                SetForeignFlag(bit);
            else
                SetFlag(OFS_CAUGHT, bit);
        }

        public bool GetForeignFlag(int bit)
        {
            Debug.Assert(bit < (int)Species.Genesect);
            return GetFlag(0x64C, bit);
        }

        public void SetForeignFlag(int bit, bool value = true)
        {
            Debug.Assert(bit < (int)Species.Genesect);
            SetFlag(0x64C, bit, value);
        }
    }
}
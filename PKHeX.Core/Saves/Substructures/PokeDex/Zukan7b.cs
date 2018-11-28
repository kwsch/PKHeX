using System;

namespace PKHeX.Core
{
    /// <summary>
    /// Beluga specific Dex manipulator, slightly modified from Gen7.
    /// </summary>
    public class Zukan7b : Zukan7
    {
        private const int SIZE_MAGIC = 4; // 0x2F120F17 magic
        private const int SIZE_FLAGS = 4;
        private const int SIZE_MISC = 0x80; // Misc Data (1024 bits)
        private const int SIZE_CAUGHT = 0x68; // 832 bits

        protected override int OFS_CAUGHT => SIZE_MAGIC + SIZE_FLAGS + SIZE_MISC;
        protected override int OFS_SEEN => OFS_CAUGHT + SIZE_CAUGHT;

        protected override int BitSeenSize => 0x8C; // 1120 bits
        protected override int DexLangFlagByteCount => 920; // 0x398 = 817*9, top off the savedata block.
        protected override int DexLangIDCount => 9; // CHT, skipping langID 6 (unused)

        public Zukan7b(SaveFile sav, int dex, int langflag)
        {
            SAV = sav;
            PokeDex = dex;
            PokeDexLanguageFlags = langflag;
            DexFormIndexFetcher = SaveUtil.GetDexFormIndexGG;
            LoadDexList();
        }

        public override void SetDex(PKM pkm)
        {
            if (!TryGetIndex(pkm.AltForm, pkm.Species, out _))
                return;
            SetSizeData((PB7)pkm);
            base.SetDex(pkm);
        }

        protected override void SetDex(int species, int bit, int form, int gender, bool shiny, int lang)
        {
            if (IsBuddy(species, form))
                form = 0;
            base.SetDex(species, bit, form, gender, shiny, lang);
        }

        private static bool IsBuddy(int species, int form) => (species == 25 && form == 8) || (species == 133 && form == 1);

        private void SetSizeData(PB7 pkm)
        {
            int species = pkm.Species;
            int form = pkm.AltForm;
            if (!TryGetIndex(form, species, out int index))
                return;

            if (Math.Round(pkm.HeightAbsolute) < pkm.PersonalInfo.Height) // possible minimum height
            {
                int ofs = GetDexSizeOffset(0, index);
                var minHeight = BitConverter.ToUInt16(SAV.Data, ofs) >> 1;
                var calcHeight = PB7.GetHeightAbsolute(pkm.PersonalInfo, minHeight);
                if (Math.Round(pkm.HeightAbsolute) < Math.Round(calcHeight) || BitConverter.ToUInt32(SAV.Data, ofs) == 0x007F00FE) // unset
                    UpdateSizeIndex(pkm, 0);
            }
            else if (Math.Round(pkm.HeightAbsolute) > pkm.PersonalInfo.Height) // possible maximum height
            {
                int ofs = GetDexSizeOffset(1, index);
                var maxHeight = BitConverter.ToUInt16(SAV.Data, ofs) >> 1;
                var calcHeight = PB7.GetHeightAbsolute(pkm.PersonalInfo, maxHeight);
                if (Math.Round(pkm.HeightAbsolute) > Math.Round(calcHeight) || BitConverter.ToUInt32(SAV.Data, ofs) == 0x007F00FE) // unset
                    UpdateSizeIndex(pkm, 1);
            }

            if (Math.Round(pkm.WeightAbsolute) < pkm.PersonalInfo.Weight) // possible minimum weight
            {
                int ofs = GetDexSizeOffset(2, index);
                var minWeight = BitConverter.ToUInt16(SAV.Data, ofs + 2);
                var minHeight = BitConverter.ToUInt16(SAV.Data, ofs) >> 1;
                var calcWeight = PB7.GetWeightAbsolute(pkm.PersonalInfo, minHeight, minWeight);
                if (Math.Round(pkm.WeightAbsolute) < Math.Round(calcWeight) || BitConverter.ToUInt32(SAV.Data, ofs) == 0x007F00FE) // unset
                    UpdateSizeIndex(pkm, 2);
            }
            else if (Math.Round(pkm.WeightAbsolute) > pkm.PersonalInfo.Weight) // possible maximum weight
            {
                int ofs = GetDexSizeOffset(3, index);
                var maxWeight = BitConverter.ToUInt16(SAV.Data, ofs + 2);
                var maxHeight = BitConverter.ToUInt16(SAV.Data, ofs) >> 1;
                var calcWeight = PB7.GetWeightAbsolute(pkm.PersonalInfo, maxHeight, maxWeight);
                if (Math.Round(pkm.WeightAbsolute) > Math.Round(calcWeight) || BitConverter.ToUInt32(SAV.Data, ofs) == 0x007F00FE) // unset
                    UpdateSizeIndex(pkm, 3);
            }
        }

        private int GetDexSizeOffset(int group, int index) => 0x3978 + (index * 6) + (group * 0x45C); // blockofs + 0xF78 + ([186*6]*n) + x*6

        private void UpdateSizeIndex(PB7 pkm, int group)
        {
            var tree = EvolutionTree.GetEvolutionTree(pkm, 7);
            int species = pkm.Species;
            int form = pkm.AltForm;

            // update for all species in potential lineage
            var allspec = tree.GetEvolutionsAndPreEvolutions(species, form);
            foreach (var s in allspec)
            {
                if (!TryGetIndex(form, s, out var index))
                    continue; // shouldn't hit this

                var ofs = GetDexSizeOffset(group, index);
                BitConverter.GetBytes((ushort)(pkm.HeightScalar << 1)).CopyTo(SAV.Data, ofs);
                BitConverter.GetBytes((ushort)(pkm.WeightScalar)).CopyTo(SAV.Data, ofs + 2);
            }
        }

        private static bool TryGetIndex(int form, int species, out int index)
        {
            index = -1;
            if (form == 0 && species <= 151)
            {
                index = species - 1;
                return true;
            }
            for (int i = 0; i < SizeDexInfoTable.Length; i += 3)
            {
                if (SizeDexInfoTable[i] != species)
                    continue;
                if (SizeDexInfoTable[i + 1] != form)
                    continue;
                index = SizeDexInfoTable[i + 2];
                return true;
            }
            return false;
        }

        public static readonly ushort[] SizeDexInfoTable =
        {
            // spec, form, index
            808, 0, 151,
            809, 0, 152,

            003, 1, 153,
            006, 1, 154,
            006, 2, 155,
            009, 1, 156,
            015, 1, 157,
            018, 1, 158,
            019, 1, 159,
            020, 1, 160,
            026, 1, 161,
            027, 1, 162,
            028, 1, 163,
            037, 1, 164,
            038, 1, 165,
            050, 1, 166,
            051, 1, 167,
            052, 1, 168,
            053, 1, 169,
            065, 1, 170,
            074, 1, 171,
            075, 1, 172,
            076, 1, 173,
            080, 1, 174,
            088, 1, 175,
            089, 1, 176,
            094, 1, 177,
            103, 1, 178,
            105, 1, 179,
            115, 1, 180,
            127, 1, 181,
            130, 1, 182,
            142, 1, 183,
            150, 1, 184,
            150, 2, 185,
        };

        protected override bool GetSaneFormsToIterate(int species, out int formStart, out int formEnd, int formIn)
        {
            switch (species)
            {
                case 020: // Raticate
                case 105: // Marowak
                    formStart = 0;
                    formEnd = 1;
                    return true;

                default:
                    int count = SaveUtil.GetDexFormCountGG(species);
                    formStart = formEnd = 0;
                    return count < formIn;
            }
        }
    }
}
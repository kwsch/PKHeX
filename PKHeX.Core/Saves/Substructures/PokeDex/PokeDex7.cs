using System;
using System.Collections.Generic;
using PKHeX.Core;

namespace PKHeX.WinForms
{
    public sealed class PokeDex7 : PokeDex
    {
        private readonly SAV7 Parent;

        private const int MiscLen = 0x80;
        private const int OwnedLen = 0x68;
        private const int SeenDispLen = 0x8C;
        private const int LanguageLen = 0x398;

        public readonly bool[][] Seen = new bool[4][];
        public readonly bool[][] Displayed = new bool[4][];
        public readonly bool[] LanguageFlags;

        public PokeDex7(SAV7 SAV)
        {
            Parent = SAV;
            if (Parent.Generation != 7)
                return;

            int ofs = Parent.PokeDex + 0x8 + MiscLen;
            Owned = SetBits(Parent.Data, ofs, OwnedLen);

            ofs += OwnedLen;
            for (int i = 0; i < 4; i++)
            {
                Seen[i] = SetBits(Parent.Data, ofs, SeenDispLen);
                ofs += SeenDispLen;
            }
            for (int i = 0; i < 4; i++)
            {
                Displayed[i] = SetBits(Parent.Data, ofs, SeenDispLen);
                ofs += SeenDispLen;
            }
            LanguageFlags = SetBits(Parent.Data, Parent.PokeDexLanguageFlags, LanguageLen);
        }

        public void Write()
        {
            if (Parent.Generation != 7)
                return;

            int ofs = Parent.PokeDex + 0x8 + MiscLen;
            SetBits(Owned).CopyTo(Parent.Data, ofs);

            ofs += OwnedLen;
            for (int i = 0; i < 4; i++)
            {
                SetBits(Seen[i]).CopyTo(Parent.Data, ofs);
                ofs += SeenDispLen;
            }
            for (int i = 0; i < 4; i++)
            {
                SetBits(Displayed[i]).CopyTo(Parent.Data, ofs);
                ofs += SeenDispLen;
            }
            SetBits(LanguageFlags).CopyTo(Parent.Data, Parent.PokeDexLanguageFlags);
        }

        public IEnumerable<int> GetAllFormEntries(int spec)
        {
            var fc = Parent.Personal[spec].FormeCount;
            for (int j = 1; j < fc; j++)
            {
                int start = j;
                int end = j;
                if (SAV7.SanitizeFormsToIterate(spec, out int s, out int n, j, Parent.USUM))
                {
                    start = s;
                    end = n;
                }
                start = Math.Max(1, start);
                for (int f = start; f <= end; f++)
                {
                    int x = GetDexFormIndex(spec, fc, f);
                    if (x >= 0)
                        yield return x;
                }
            }
        }

        public int GetDexFormIndex(int spec, int fc, int f)
        {
            var index = Parent.USUM
                ? SaveUtil.GetDexFormIndexUSUM(spec, fc, f)
                : SaveUtil.GetDexFormIndexSM(spec, fc, f);
            if (index < 0)
                return index;
            return index + Parent.MaxSpeciesID - 1;
        }

        public int GetDexFormStart(int spec, int fc)
        {
            return Parent.USUM
                ? SaveUtil.GetDexFormIndexUSUM(spec, fc, Parent.MaxSpeciesID - 1)
                : SaveUtil.GetDexFormIndexSM(spec, fc, Parent.MaxSpeciesID - 1);
        }
    }
}
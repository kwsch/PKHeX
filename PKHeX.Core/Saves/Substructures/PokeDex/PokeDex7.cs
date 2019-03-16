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

        private readonly IList<int> FormBaseSpecies;

        public PokeDex7(SAV7 SAV)
        {
            Parent = SAV;
            if (Parent.Generation != 7)
                return;

            FormBaseSpecies = GetFormIndexBaseSpeciesList();

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
                ? DexFormUtil.GetDexFormIndexUSUM(spec, fc, f)
                : DexFormUtil.GetDexFormIndexSM(spec, fc, f);
            if (index < 0)
                return index;
            return index + Parent.MaxSpeciesID - 1;
        }

        public int GetDexFormStart(int spec, int fc)
        {
            return Parent.USUM
                ? DexFormUtil.GetDexFormIndexUSUM(spec, fc, Parent.MaxSpeciesID - 1)
                : DexFormUtil.GetDexFormIndexSM(spec, fc, Parent.MaxSpeciesID - 1);
        }

        public IEnumerable<string> GetEntryNames(IReadOnlyList<string> Species)
        {
            var names = new List<string>();
            for (int i = 1; i <= Parent.MaxSpeciesID; i++)
                names.Add($"{i:000} - {Species[i]}");

            // Add Formes
            int ctr = Parent.MaxSpeciesID;
            for (int spec = 1; spec <= Parent.MaxSpeciesID; spec++)
            {
                int c = Parent.Personal[spec].FormeCount;
                for (int f = 1; f < c; f++)
                {
                    int x = GetDexFormIndex(spec, c, f);
                    if (x >= 0)
                        names.Add($"{++ctr:000} - {Species[spec]}-{f}");
                }
            }
            return names;
        }

        /// <summary>
        /// Gets a list of Species IDs that a given dex-forme index corresponds to.
        /// </summary>
        /// <returns></returns>
        private List<int> GetFormIndexBaseSpeciesList()
        {
            var baseSpecies = new List<int>();
            for (int spec = 1; spec <= Parent.MaxSpeciesID; spec++)
            {
                int c = Parent.Personal[spec].FormeCount;
                for (int f = 1; f < c; f++)
                {
                    int x = GetDexFormIndex(spec, c, f);
                    if (x >= 0)
                        baseSpecies.Add(spec);
                }
            }
            return baseSpecies;
        }

        public int GetBaseSpeciesGenderValue(int index)
        {
            // meowstic special handling
            const int meow = 678;
            if (index == meow - 1 || (index >= Parent.MaxSpeciesID && FormBaseSpecies[index - Parent.MaxSpeciesID] == meow))
                return index < Parent.MaxSpeciesID ? 0 : 254; // M : F

            if (index < Parent.MaxSpeciesID)
                return Parent.Personal[index + 1].Gender;

            index -= Parent.MaxSpeciesID;
            int spec = FormBaseSpecies[index];
            return Parent.Personal[spec].Gender;
        }

        public int GetBaseSpecies(int index)
        {
            if (index <= Parent.MaxSpeciesID)
                return index;

            return FormBaseSpecies[index - Parent.MaxSpeciesID - 1];
        }
    }
}
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace PKHeX.Core
{
    /// <summary>
    /// Storage for all <see cref="Poffin4"/> the trainer has.
    /// </summary>
    public class PoffinCase4
    {
        private readonly SAV4 SAV;
        private readonly int Offset;
        public Poffin4[] Poffins { get; set; }

        private const int Count = 100;

        public PoffinCase4(SAV4 sav)
        {
            SAV = sav;
            if (SAV.HGSS)
                throw new ArgumentException(nameof(SAV));

            Offset = SAV.DP ? 0x5050 : 0x52E8;
            Poffins = ReadPoffins(SAV, Offset);
        }

        public void Save() => WritePoffins(SAV, Offset, Poffins);

        private static Poffin4[] ReadPoffins(SaveFile sav, int offset)
        {
            var Poffins = new Poffin4[Count];
            for (int i = 0; i < Poffins.Length; i++)
                Poffins[i] = new Poffin4(sav.Data, offset + (i * Poffin4.SIZE));
            return Poffins;
        }

        private static void WritePoffins(SaveFile sav, int offset, IReadOnlyList<Poffin4> poffins)
        {
            Debug.Assert(poffins.Count == Count);
            for (int i = 0; i < poffins.Count; i++)
                sav.SetData(poffins[i].Data, offset + (i * Poffin4.SIZE));
        }

        public void FillCase()
        {
            foreach (var p in Poffins)
                p.SetAll();
        }

        public void DeleteAll()
        {
            foreach (var p in Poffins)
                p.Delete();
        }
    }
}
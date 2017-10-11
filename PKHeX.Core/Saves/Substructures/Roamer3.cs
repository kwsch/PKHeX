using System;

namespace PKHeX.Core
{
    public class Roamer3
    {
        private readonly SaveFile SAV;
        private readonly int Offset;
        public bool IsGlitched { get; }
        
        public Roamer3(SAV3 sav)
        {
            SAV = sav;
            Offset = sav.GetBlockOffset(4);
            if (GameVersion.FRLG.Contains(SAV.Version))
                Offset += 0x250; // 0x250 - FRLG
            else
                Offset += 0x35C; // 0x35C - RSE
            IsGlitched = SAV.Version != GameVersion.E;
        }

        private uint IV32
        {
            get => BitConverter.ToUInt32(SAV.Data, Offset);
            set => SAV.SetData(BitConverter.GetBytes(value), Offset);
        }
        public uint PID
        {
            get => BitConverter.ToUInt32(SAV.Data, Offset + 4);
            set => SAV.SetData(BitConverter.GetBytes(value), Offset + 4);
        }
        public int Species
        {
            get => SpeciesConverter.GetG4Species(BitConverter.ToInt16(SAV.Data, Offset + 8));
            set => SAV.SetData(BitConverter.GetBytes((ushort)SpeciesConverter.GetG3Species(value)), Offset + 8);
        }

        // Derived Properties
        private int IV_HP { get => (int)(IV32 >> 00) & 0x1F; set => IV32 = (uint)((IV32 & ~(0x1F << 00)) | (uint)((value > 31 ? 31 : value) << 00)); }
        private int IV_ATK { get => (int)(IV32 >> 05) & 0x1F; set => IV32 = (uint)((IV32 & ~(0x1F << 05)) | (uint)((value > 31 ? 31 : value) << 05)); }
        private int IV_DEF { get => (int)(IV32 >> 10) & 0x1F; set => IV32 = (uint)((IV32 & ~(0x1F << 10)) | (uint)((value > 31 ? 31 : value) << 10)); }
        private int IV_SPE { get => (int)(IV32 >> 15) & 0x1F; set => IV32 = (uint)((IV32 & ~(0x1F << 15)) | (uint)((value > 31 ? 31 : value) << 15)); }
        private int IV_SPA { get => (int)(IV32 >> 20) & 0x1F; set => IV32 = (uint)((IV32 & ~(0x1F << 20)) | (uint)((value > 31 ? 31 : value) << 20)); }
        private int IV_SPD { get => (int)(IV32 >> 25) & 0x1F; set => IV32 = (uint)((IV32 & ~(0x1F << 25)) | (uint)((value > 31 ? 31 : value) << 25)); }

        /// <summary>
        /// Roamer's IVs.
        /// </summary>
        public int[] IVs
        {
            get => new[] { IV_HP, IV_ATK, IV_DEF, IV_SPE, IV_SPA, IV_SPD };
            set
            {
                if (value?.Length != 6) return;
                IV_HP = value[0]; IV_ATK = value[1]; IV_DEF = value[2];
                IV_SPE = value[3]; IV_SPA = value[4]; IV_SPD = value[5];
            }
        }

        /// <summary>
        /// Indicates if the Roamer is shiny with the <see cref="SAV"/>'s Trainer Details.
        /// </summary>
        /// <param name="pid">PID to check for</param>
        /// <returns>Indication if the PID is shiny for the trainer.</returns>
        public bool IsShiny(uint pid)
        {
            var val = (ushort)(SAV.SID ^ SAV.TID ^ (pid >> 16) ^ pid);
            return val < 8;
        }

        /// <summary>
        /// Gets the glitched Roamer IVs, where only 1 byte of IV data is loaded when encountered.
        /// </summary>
        public int[] IVsGlitch
        {
            get
            {
                var ivs = IV32; // store for restoration later
                IV32 &= 0xFF; // only 1 byte is loaded to the encounter
                var glitch = IVs; // get glitched IVs
                IV32 = ivs; // restore unglitched IVs
                return glitch;
            }
        }
    }
}

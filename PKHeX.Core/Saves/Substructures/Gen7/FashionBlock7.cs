using System;
using System.Linq;

namespace PKHeX.Core
{
    public sealed class FashionBlock7 : SaveBlock
    {
        public FashionBlock7(SAV7SM sav, int offset) : base(sav) => Offset = offset;
        public FashionBlock7(SAV7USUM sav, int offset) : base(sav) => Offset = offset;

        private const int FashionLength = 0x1A08;

        public FashionItem7[] Wardrobe
        {
            get
            {
                var data = SAV.GetData(Offset, 0x5A8);
                return data.Select(b => new FashionItem7(b)).ToArray();
            }
            set
            {
                if (value.Length != 0x5A8)
                    throw new ArgumentOutOfRangeException($"Unexpected size: 0x{value.Length:X}");
                SAV.SetData(value.Select(z => z.Value).ToArray(), Offset);
            }
        }

        public void Clear() => Array.Clear(Data, Offset, FashionLength);

        /// <summary>
        /// Resets the fashion unlocks to default values.
        /// </summary>
        public void Reset()
        {
            var offsetList = SAV is SAV7USUM
                ? (SAV.Gender == 0
                    ? new[] { 0x03A, 0x109, 0x1DA, 0x305, 0x3D9, 0x4B1, 0x584 }   // M
                    : new[] { 0x05E, 0x208, 0x264, 0x395, 0x3B4, 0x4F9, 0x5A8 })  // F
                : (SAV.Gender == 0
                    ? new[] { 0x000, 0x0FB, 0x124, 0x28F, 0x3B4, 0x452, 0x517 }   // M
                    : new[] { 0x000, 0x100, 0x223, 0x288, 0x3B4, 0x452, 0x517 }); // F

            foreach (var ofs in offsetList)
                SAV.Data[Offset + ofs] = 3; // owned | new
        }
    }

    // Every fashion item is 2 bits, New Flag (high) & Owned Flag (low)
    public sealed class FashionItem7
    {
        public bool IsOwned { get; set; }
        public bool IsNew { get; set; }

        public FashionItem7(byte b)
        {
            IsOwned = (b & 1) != 0;
            IsNew = (b & 2) != 0;
        }

        public byte Value => (byte)((IsOwned ? 1 : 0) | (IsNew ? 2 : 0));
    }
}
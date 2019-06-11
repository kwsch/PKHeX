using System;
using System.Linq;

namespace PKHeX.Core
{
    public sealed class FashionBlock7 : SaveBlock
    {
        public FashionBlock7(SAV7 sav, int offset) : base(sav) => Offset = offset;

        private const int FashionLength = 0x1A08;

        public sealed class FashionItem
        {
            public bool IsOwned { get; set; }
            public bool IsNew { get; set; }
        }

        public FashionItem[] Wardrobe
        {
            get
            {
                var data = SAV.GetData(Offset, 0x5A8);
                return data.Select(b => new FashionItem { IsOwned = (b & 1) != 0, IsNew = (b & 2) != 0 }).ToArray();
            }
            set
            {
                if (value.Length != 0x5A8)
                    throw new ArgumentOutOfRangeException($"Unexpected size: 0x{value.Length:X}");
                SAV.SetData(value.Select(t => (byte)((t.IsOwned ? 1 : 0) | (t.IsNew ? 2 : 0))).ToArray(), Offset);
            }
        }

        public void Clear() => Array.Clear(Data, Offset, FashionLength);
    }
}
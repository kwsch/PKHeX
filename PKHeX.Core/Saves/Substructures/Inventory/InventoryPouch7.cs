using System;
using System.Collections.Generic;
using System.Linq;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core
{
    public sealed class InventoryPouch7 : InventoryPouch
    {
        public bool SetNew { get; set; } = false;
        private InventoryItem7[] OriginalItems = Array.Empty<InventoryItem7>();

        public InventoryPouch7(InventoryType type, ushort[] legal, int maxCount, int offset)
            : base(type, legal, maxCount, offset) { }

        public override InventoryItem GetEmpty(int itemID = 0, int count = 0) => new InventoryItem7 { Index = itemID, Count = count };

        public override void GetPouch(ReadOnlySpan<byte> data)
        {
            var span = data[Offset..];
            var items = new InventoryItem7[PouchDataSize];
            for (int i = 0; i < items.Length; i++)
            {
                var item = span.Slice(i * 4, 4);
                uint val = ReadUInt32LittleEndian(item);
                items[i] = InventoryItem7.GetValue(val);
            }
            Items = items;
            OriginalItems = items.Select(i => i.Clone()).ToArray();
        }

        public override void SetPouch(Span<byte> data)
        {
            if (Items.Length != PouchDataSize)
                throw new ArgumentException("Item array length does not match original pouch size.");

            var span = data[Offset..];
            var items = (InventoryItem7[])Items;
            for (int i = 0; i < Items.Length; i++)
            {
                var item = span.Slice(i * 4, 4);
                var val = items[i].GetValue(SetNew, OriginalItems);
                WriteUInt32LittleEndian(item, val);
            }
        }
    }

    public sealed class InventoryItem7 : InventoryItem, IItemFreeSpaceIndex, IItemNew
    {
        public uint FreeSpaceIndex { get; set; }
        public bool IsNew { get; set; }

        /// <summary> Creates a copy of the object. </summary>
        public new InventoryItem7 Clone() => (InventoryItem7)MemberwiseClone();

        public static InventoryItem7 GetValue(uint value) => new()
        {
            // 10bit itemID
            // 10bit count
            // 10bit freespace index
            // 1 bit new flag
            // 1 bit reserved
            Index = (int)(value & 0x3FF),
            Count = (int)(value >> 10 & 0x3FF),
            IsNew = (value & 0x40000000) != 0, // 30th bit is "NEW"
            FreeSpaceIndex = (value >> 20 & 0x3FF), // "FREE SPACE" sortIndex
        };

        public uint GetValue(bool setNew, IReadOnlyList<InventoryItem> original)
        {
            // Build Item Value
            uint value = 0;
            value |= (uint)(Index & 0x3FF);
            value |= (uint)(Count & 0x3FF) << 10;

            bool isNew = IsNew || (setNew && original.All(z => z.Index != Index));
            if (isNew)
                value |= 0x40000000;
            value |= (FreeSpaceIndex & 0x3FFu) << 20;
            return value;
        }
    }
}

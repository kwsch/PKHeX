using System;
using System.Linq;

namespace PKHeX.Core
{
    /// <summary>
    /// Inventory Pouch used by <see cref="GameVersion.SWSH"/>
    /// </summary>
    public sealed class InventoryPouch8 : InventoryPouch
    {
        public bool SetNew { get; set; }
        private InventoryItem8[] OriginalItems = Array.Empty<InventoryItem8>();

        public InventoryPouch8(InventoryType type, ushort[] legal, int maxCount, int offset, int size, Func<ushort, bool>? isLegal = null)
            : base(type, legal, maxCount, offset, size, isLegal) { }

        public override InventoryItem GetEmpty(int itemID = 0, int count = 0) => new InventoryItem8 { Index = itemID, Count = count };

        public override void GetPouch(byte[] data)
        {
            var items = new InventoryItem8[PouchDataSize];
            for (int i = 0; i < items.Length; i++)
            {
                uint value = BitConverter.ToUInt32(data, Offset + (i * 4));
                items[i] = InventoryItem8.GetValue(value);
            }
            Items = items;
            OriginalItems = items.Select(i => i.Clone()).ToArray();
        }

        public override void SetPouch(byte[] data)
        {
            if (Items.Length != PouchDataSize)
                throw new ArgumentException("Item array length does not match original pouch size.");

            var items = (InventoryItem8[])Items;
            for (int i = 0; i < items.Length; i++)
            {
                uint val = items[i].GetValue(SetNew, OriginalItems);
                BitConverter.GetBytes(val).CopyTo(data, Offset + (i * 4));
            }
        }

        /// <summary>
        /// Checks pouch contents for bad count values.
        /// </summary>
        /// <remarks>
        /// Certain pouches contain a mix of count-limited items and uncapped regular items.
        /// </remarks>
        internal void SanitizeCounts()
        {
            foreach (var item in Items)
                item.Count = GetSuggestedCount(Type, item.Index, item.Count);
        }

        public static int GetSuggestedCount(InventoryType t, int item, int requestVal) => t switch
        {
            // TMs are clamped to 1, let TRs be whatever
            InventoryType.TMHMs => item is >= 1130 and <= 1229 ? requestVal : 1,
            _ => requestVal,
        };
    }
}

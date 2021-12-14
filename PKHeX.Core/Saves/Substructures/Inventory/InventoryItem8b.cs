using System;

namespace PKHeX.Core
{
    public sealed class InventoryItem8b : InventoryItem, IItemFavorite, IItemNew
    {
        public const int SIZE = 0x10;

        public bool IsFavorite { get; set; }
        public bool IsNew { get; set; }
        public ushort SortOrder { get; set; }

        public override string ToString() => $"{SortOrder:00} - {Index:000} x{Count}{(IsNew ? "*" : "")}{(IsFavorite ? "F" : "")}";

        /// <summary> Creates a copy of the object. </summary>
        public new InventoryItem8b Clone() => (InventoryItem8b)MemberwiseClone();

        /// <summary>
        /// Indicates if the item has been acquired by the player.
        /// </summary>
        public bool IsValidSaveSortNumberCount => SortOrder != 0;

        public static InventoryItem8b Read(ushort index, byte[] data, int offset) => new()
        {
            Index = index,
            Count = BitConverter.ToInt32(data, offset),
            IsNew = BitConverter.ToInt32(data, offset + 4) == 0,
            IsFavorite = BitConverter.ToInt32(data, offset + 0x8) == 1,
            SortOrder = BitConverter.ToUInt16(data, offset + 0xC),
            // 0xE alignment
        };

        public void Write(byte[] data, int offset)
        {
            // Index is not saved.
            BitConverter.GetBytes((uint)Count).CopyTo(data, offset);
            BitConverter.GetBytes(IsNew ? 0u : 1u).CopyTo(data, offset + 0x4);
            BitConverter.GetBytes(IsFavorite ? 1u : 0u).CopyTo(data, offset + 0x8);
            BitConverter.GetBytes(SortOrder).CopyTo(data, offset + 0xC);
            BitConverter.GetBytes((ushort)0).CopyTo(data, offset + 0xE);
        }

        public static void Clear(byte[] data, int offset) => Array.Clear(data, offset, SIZE);

        public override void SetNewDetails(int count)
        {
            base.SetNewDetails(count);
            if (IsValidSaveSortNumberCount)
                return;
            IsNew = true;
            IsFavorite = false;
        }

        /// <summary>
        /// Item has been matched to a previously existing item. Copy over the misc details.
        /// </summary>
        public override void MergeOverwrite<T>(T other)
        {
            base.MergeOverwrite(other);
            if (other is not InventoryItem8b item)
                return;
            SortOrder = item.SortOrder;
            IsNew = item.IsNew;
            IsFavorite = item.IsFavorite;
        }
    }
}

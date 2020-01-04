using System;

namespace PKHeX.Core
{
    /// <summary>
    /// Storage for the two in-game daycare structures.
    /// </summary>
    public sealed class Daycare8 : SaveBlock
    {
        public Daycare8(SaveFile sav, SCBlock block) : base(sav, block.Data) { }

        // BLOCK STRUCTURE:
        // bool8 present
        // pk8 entry1
        // bool8 present
        // pk8 entry2
        // daycare metadata (0x26)
        // bool1 present
        // pk8 entry1
        // bool1 present
        // pk8 entry2
        // daycare metadata (0x26)

        // daycare metadata:
        // u16 ??
        // u32 ?step count since last update?
        // u64 RNG seed
        // 0x18 unk

        /// <summary>
        /// Size of each PKM data stored (bool, pk8)
        /// </summary>
        private const int STRUCT_SIZE = 1 + PokeCrypto.SIZE_8STORED;

        /// <summary>
        /// Size of each daycare (both entries &amp; metadata)
        /// </summary>
        private const int DAYCARE_SIZE = (2 * STRUCT_SIZE) + 0x26;

        private const int META_1 = (2 * STRUCT_SIZE);
        private const int META_2 = DAYCARE_SIZE + META_1;

        public bool GetDaycare1SlotOccupied(int slot)
        {
            if ((uint) slot >= 2)
                throw new IndexOutOfRangeException(nameof(slot));

            return Data[GetDaycare1StructOffset(slot)] == 1;
        }

        public bool GetDaycare2SlotOccupied(int slot)
        {
            if ((uint)slot >= 2)
                throw new IndexOutOfRangeException(nameof(slot));

            return Data[GetDaycare2StructOffset(slot)] == 1;
        }

        public static int GetDaycare1StructOffset(int slot)
        {
            if ((uint)slot >= 2)
                throw new IndexOutOfRangeException(nameof(slot));

            return 0 + (slot * STRUCT_SIZE);
        }

        public static int GetDaycare2StructOffset(int slot)
        {
            if ((uint)slot >= 2)
                throw new IndexOutOfRangeException(nameof(slot));

            return DAYCARE_SIZE + (slot * STRUCT_SIZE);
        }

        public static int GetDaycareSlotOffset(int daycare, int slot)
        {
            return daycare switch
            {
                0 => (1 + GetDaycare1StructOffset(slot)),
                1 => (1 + GetDaycare2StructOffset(slot)),
                _ => throw new IndexOutOfRangeException(nameof(daycare))
            };
        }

        public static int GetDaycareMetadataOffset(int daycare)
        {
            return daycare switch
            {
                0 => META_1,
                1 => META_2,
                _ => throw new IndexOutOfRangeException(nameof(daycare))
            };
        }

        public ulong GetDaycareSeed(int daycare) => BitConverter.ToUInt64(Data, GetDaycareMetadataOffset(daycare) + 6);
        public void SetDaycareSeed(int daycare, ulong value) => SAV.SetData(Data, BitConverter.GetBytes(value), GetDaycareMetadataOffset(daycare) + 6);
    }
}
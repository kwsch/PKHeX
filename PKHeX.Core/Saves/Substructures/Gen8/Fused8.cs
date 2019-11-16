using System;

namespace PKHeX.Core
{
    public sealed class Fused8 : SaveBlock
    {
        public Fused8(SaveFile sav, SCBlock block) : base(sav, block.Data) { }

        public static int GetFusedSlotOffset(int slot)
        {
            if ((uint)slot >= 3)
                return -1;
            return PKX.SIZE_8PARTY * slot;
        }
    }

    public sealed class Daycare8 : SaveBlock
    {
        public Daycare8(SaveFile sav, SCBlock block) : base(sav, block.Data) { }

        private const int STRUCT_SIZE = 4 + PKX.SIZE_8STORED;
        private const int DAYCARE_SIZE = (2 * STRUCT_SIZE) + 0x26;

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
                0 => (4 + GetDaycare1StructOffset(slot)),
                1 => (4 + GetDaycare2StructOffset(slot)),
                _ => throw new IndexOutOfRangeException(nameof(daycare))
            };
        }
    }
}
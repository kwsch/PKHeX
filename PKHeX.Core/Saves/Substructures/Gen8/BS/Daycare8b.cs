using System;
using System.ComponentModel;

namespace PKHeX.Core
{
    /// <summary>
    /// Storage for the in-game daycare structure.
    /// </summary>
    /// <remarks>size: 0x2C0</remarks>
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public sealed class Daycare8b : SaveBlock
    {
        public Daycare8b(SaveFile sav, int offset) : base(sav) => Offset = offset;

        // BLOCK STRUCTURE
        // PB8[2] Parents;
        // bool32 eggExist;
        // ulong eggSeed; -- setter puts only 32 bits!
        // int32 eggStepCount;

        private const int SlotCount = 2;
        private const int ExtraDataOffset = PokeCrypto.SIZE_8PARTY * SlotCount;

        public bool GetDaycareSlotOccupied(int slot) => GetSlot(slot).Species != 0;

        public int GetParentSlotOffset(int slot)
        {
            if ((uint)slot >= SlotCount)
                throw new IndexOutOfRangeException(nameof(slot));

            return Offset + (slot * PokeCrypto.SIZE_8PARTY);
        }

        public PB8 GetSlot(int slot)
        {
            var offset = GetParentSlotOffset(slot);
            var data = Data.AsSpan(offset, PokeCrypto.SIZE_8PARTY).ToArray();
            return new PB8(data);
        }

        public bool IsEggAvailable
        {
            get => BitConverter.ToUInt32(Data, Offset + ExtraDataOffset) == 1;
            set => BitConverter.GetBytes(value ? 1u : 0).CopyTo(Data, Offset + ExtraDataOffset);
        }

        public ulong DaycareSeed
        {
            get => BitConverter.ToUInt64(Data, Offset + ExtraDataOffset + 4);
            set => SAV.SetData(Data, BitConverter.GetBytes(value), Offset + ExtraDataOffset + 4);
        }

        public int EggStepCount
        {
            get => BitConverter.ToInt32(Data, Offset + ExtraDataOffset + 4 + 8);
            set => BitConverter.GetBytes(value).CopyTo(Data, Offset + ExtraDataOffset + 4 + 8);
        }
    }
}

using System;

namespace PKHeX.Core
{
    public class Daycare5 : SaveBlock
    {
        private const int SlotSize = 4 + PKX.SIZE_5STORED + 4; // occupied u32 flag, pk5, exp
        public const int DaycareSeedSize = 16; // 8 bytes, b2w2 only

        public Daycare5(SaveFile sav, int offset) : base(sav) => Offset = offset;

        public ulong? GetSeed()
        {
            if (SAV.Version != GameVersion.B2W2)
                return null;
            return BitConverter.ToUInt64(Data, Offset + 0x1CC);
        }

        public void SetSeed(string value)
        {
            if (value == null)
                return;
            var data = Util.GetBytesFromHexString(value);
            SAV.SetData(data, Offset + 0x1CC);
        }


        private int SlotOffset(int slot) => Offset + (SlotSize * slot);
        private int DaycareEXPOffset(int slot) => SlotOffset(slot) + 0xE0;

        public bool? IsOccupied(int slot) => BitConverter.ToUInt32(Data, SlotOffset(slot)) == 1;
        public void SetOccupied(int slot, bool occupied) => SAV.SetData(BitConverter.GetBytes((uint)(occupied ? 1 : 0)), SlotOffset(slot));

        public int GetOffset(int slot) => SlotOffset(slot) + 4;

        public uint? GetEXP(int slot) => BitConverter.ToUInt32(Data, DaycareEXPOffset(slot));
        public void SetEXP(int slot, uint EXP) => SAV.SetData(BitConverter.GetBytes(EXP), SlotOffset(slot) + 0xE0);
    }
}
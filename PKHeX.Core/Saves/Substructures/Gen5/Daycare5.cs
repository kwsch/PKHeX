using System;

namespace PKHeX.Core
{
    public sealed class Daycare5 : SaveBlock
    {
        // struct daycareSlot 
        // bool32 occupied
        // pk5party pkm
        // u32 expGained
        private const int SlotSize = 4 + PokeCrypto.SIZE_5PARTY + 4; // occupied u32 flag, pk5, exp

        // struct daycare
        // daycareSlot[2]
        // ???->end ???

        public const int DaycareSeedSize = 16; // 8 bytes, b2w2 only

        public Daycare5(SaveFile sav, int offset) : base(sav) => Offset = offset;

        public ulong? GetSeed()
        {
            if (SAV is not SAV5B2W2)
                return null;
            return BitConverter.ToUInt64(Data, Offset + 0x1CC);
        }

        public void SetSeed(string value)
        {
            if (SAV is not SAV5B2W2)
                return;
            var data = Util.GetBytesFromHexString(value);
            SAV.SetData(data, Offset + 0x1CC);
        }

        private int GetDaycareSlotOffset(int slot) => Offset + (SlotSize * slot);
        public int GetPKMOffset(int slot) => GetDaycareSlotOffset(slot) + 4;
        private int GetDaycareEXPOffset(int slot) => GetDaycareSlotOffset(slot) + 4 + PokeCrypto.SIZE_5PARTY;

        public bool? IsOccupied(int slot) => BitConverter.ToUInt32(Data, GetDaycareSlotOffset(slot)) == 1;
        public void SetOccupied(int slot, bool occupied) => SAV.SetData(BitConverter.GetBytes((uint)(occupied ? 1 : 0)), GetDaycareSlotOffset(slot));

        public uint? GetEXP(int slot) => BitConverter.ToUInt32(Data, GetDaycareEXPOffset(slot));
        public void SetEXP(int slot, uint EXP) => SAV.SetData(BitConverter.GetBytes(EXP), GetDaycareEXPOffset(slot));
    }
}
using System;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

public sealed class Daycare5(SAV5 sav, Memory<byte> raw) : SaveBlock<SAV5>(sav, raw)
{
    // struct daycareSlot
    // bool32 occupied
    // pk5 (party sized) pk
    // u32 expGained
    private const int SlotSize = 4 + PokeCrypto.SIZE_5PARTY + 4; // occupied u32 flag, pk5, exp

    // struct daycare
    // daycareSlot[2]
    // ???->end ???

    public const int DaycareSeedSize = 16; // 8 bytes, B2/W2 only

    public ulong? GetSeed()
    {
        if (SAV is not SAV5B2W2)
            return null;
        return ReadUInt64LittleEndian(Data[0x1CC..]);
    }

    public void SetSeed(ReadOnlySpan<char> value)
    {
        if (SAV is not SAV5B2W2)
            return;
        var data = Util.GetBytesFromHexString(value);
        SAV.SetData(data, 0x1CC);
    }

    private static int GetDaycareSlotOffset(int slot) => (SlotSize * slot);
    public int GetPKMOffset(int slot) => GetDaycareSlotOffset(slot) + 4;
    private int GetDaycareEXPOffset(int slot) => GetDaycareSlotOffset(slot) + 4 + PokeCrypto.SIZE_5PARTY;

    public bool? IsOccupied(int slot) => ReadUInt32LittleEndian(Data[GetDaycareSlotOffset(slot)..]) == 1;
    public void SetOccupied(int slot, bool occupied) => WriteUInt32LittleEndian(Data[GetDaycareSlotOffset(slot)..], occupied ? 1u : 0);

    public uint? GetEXP(int slot) => ReadUInt32LittleEndian(Data[GetDaycareEXPOffset(slot)..]);
    public void SetEXP(int slot, uint EXP) => WriteUInt32LittleEndian(Data[GetDaycareEXPOffset(slot)..], EXP);
}

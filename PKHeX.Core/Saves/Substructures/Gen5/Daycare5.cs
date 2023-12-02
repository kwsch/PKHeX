using System;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

public sealed class Daycare5 : SaveBlock<SAV5>
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

    public Daycare5(SAV5 sav, int offset) : base(sav) => Offset = offset;

    public ulong? GetSeed()
    {
        if (SAV is not SAV5B2W2)
            return null;
        return ReadUInt64LittleEndian(Data.AsSpan(Offset + 0x1CC));
    }

    public void SetSeed(ReadOnlySpan<char> value)
    {
        if (SAV is not SAV5B2W2)
            return;
        var data = Util.GetBytesFromHexString(value);
        SAV.SetData(data, Offset + 0x1CC);
    }

    private int GetDaycareSlotOffset(int slot) => Offset + (SlotSize * slot);
    public int GetPKMOffset(int slot) => GetDaycareSlotOffset(slot) + 4;
    private int GetDaycareEXPOffset(int slot) => GetDaycareSlotOffset(slot) + 4 + PokeCrypto.SIZE_5PARTY;

    public bool? IsOccupied(int slot) => ReadUInt32LittleEndian(Data.AsSpan(GetDaycareSlotOffset(slot))) == 1;
    public void SetOccupied(int slot, bool occupied) => WriteUInt32LittleEndian(Data.AsSpan(GetDaycareSlotOffset(slot)), occupied ? 1u : 0);

    public uint? GetEXP(int slot) => ReadUInt32LittleEndian(Data.AsSpan(GetDaycareEXPOffset(slot)));
    public void SetEXP(int slot, uint EXP) => WriteUInt32LittleEndian(Data.AsSpan(GetDaycareEXPOffset(slot)), EXP);
}

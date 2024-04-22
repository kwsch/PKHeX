using System;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

public sealed class Daycare5(SAV5 sav, Memory<byte> raw) : SaveBlock<SAV5>(sav, raw), IDaycareStorage, IDaycareRandomState<ulong>, IDaycareExperience
{
    // struct daycareSlot
    // bool32 occupied
    // pk5 (party sized) pk
    // u32 expGained
    private const int SlotSize = 4 + PokeCrypto.SIZE_5PARTY + 4; // occupied u32 flag, pk5, exp

    // struct daycare
    // daycareSlot[2]
    // ???->end ???

    public int DaycareSlotCount => 2;

    private static int GetDaycareSlotOffset(int index) => SlotSize * index;
    public bool IsDaycareOccupied(int index) => ReadUInt32LittleEndian(Data[GetDaycareSlotOffset(index)..]) == 1;
    public void SetDaycareOccupied(int index, bool occupied) => WriteUInt32LittleEndian(Data[GetDaycareSlotOffset(index)..], occupied ? 1u : 0);

    private static int GetPKMOffset(int index) => GetDaycareSlotOffset(index) + 4;
    public Memory<byte> GetDaycareSlot(int index) => Raw.Slice(GetPKMOffset(index), PokeCrypto.SIZE_5PARTY);

    private static int GetDaycareEXPOffset(int slot) => GetDaycareSlotOffset(slot) + 4 + PokeCrypto.SIZE_5PARTY;
    public uint GetDaycareEXP(int index) => ReadUInt32LittleEndian(Data[GetDaycareEXPOffset(index)..]);
    public void SetDaycareEXP(int index, uint value) => WriteUInt32LittleEndian(Data[GetDaycareEXPOffset(index)..], value);

    // 0x1C8

    public bool IsEggAvailable
    {
        get => (Data[0x1C8] & 1) != 0;
        set => Data[0x1C8] = (byte)(value ? (Data[0x1C8] | 1) : (Data[0x1C8] & ~1));
    }

    // 8 bytes, B2/W2 only
    public ulong Seed
    {
        get
        {
            if (SAV is not SAV5B2W2)
                return 0;
            return ReadUInt64LittleEndian(Data[0x1CC..]);
        }
        set
        {
            if (SAV is not SAV5B2W2)
                return;
            WriteUInt64LittleEndian(Data[0x1CC..], value);
        }
    }
}

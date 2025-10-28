using System;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

public sealed class Daycare6XY(SAV6XY sav, Memory<byte> raw) : SaveBlock<SAV6XY>(sav, raw), IDaycareStorage, IDaycareRandomState<ulong>, IDaycareEggState, IDaycareExperience
{
    private const int SIZE_STORED = PokeCrypto.SIZE_6STORED;
    private const int SlotSize = 4 + SIZE_STORED + 4; // occupied, exp, stored

    public int DaycareSlotCount => 2;

    public bool IsDaycareOccupied(int slot) => Data[SlotSize * slot] == 1;
    public void SetDaycareOccupied(int slot, bool occupied) => Data[SlotSize * slot] = occupied ? (byte)1 : (byte)0;

    public uint GetDaycareEXP(int slot) => ReadUInt32LittleEndian(Data[(4 + (SlotSize * slot))..]);
    public void SetDaycareEXP(int slot, uint EXP) => WriteUInt32LittleEndian(Data[(4 + (SlotSize * slot))..], EXP);

    public Memory<byte> GetDaycareSlot(int slot) => Raw.Slice((SlotSize * slot) + 8, SIZE_STORED);

    public bool IsEggAvailable
    {
        get => Data[0x1E0] == 1;
        set => Data[0x1E0] = value ? (byte)1 : (byte)0;
    }

    public ulong Seed
    {
        get => ReadUInt64LittleEndian(Data[0x1E8..]);
        set => WriteUInt64LittleEndian(Data[0x1E8..], value);
    }
}

public sealed class Daycare6AO(SAV6AO sav, Memory<byte> raw) : SaveBlock<SAV6AO>(sav, raw), IDaycareMulti
{
    public readonly Daycare6Couple Primary = new(raw[..Daycare6Couple.SIZE]);
    public readonly Daycare6Couple Secondary = new(raw.Slice(Daycare6Couple.SIZE, Daycare6Couple.SIZE));

    public int DaycareCount => 2;
    public IDaycareStorage this[int index] => index == 0 ? Primary : Secondary;
}

public sealed class Daycare6Couple(Memory<byte> Raw) : IDaycareStorage, IDaycareRandomState<ulong>, IDaycareEggState, IDaycareExperience
{
    public const int SIZE = 0x1F0;
    private const int SIZE_STORED = PokeCrypto.SIZE_6STORED;
    private const int SlotSize = 4 + SIZE_STORED + 4; // occupied, exp, stored

    private Span<byte> Data => Raw.Span;

    public int DaycareSlotCount => 2;

    public bool IsDaycareOccupied(int slot) => Data[SlotSize * slot] == 1;
    public void SetDaycareOccupied(int slot, bool occupied) => Data[SlotSize * slot] = occupied ? (byte)1 : (byte)0;

    public uint GetDaycareEXP(int slot) => ReadUInt32LittleEndian(Data[(4 + (SlotSize * slot))..]);
    public void SetDaycareEXP(int slot, uint EXP) => WriteUInt32LittleEndian(Data[(4 + (SlotSize * slot))..], EXP);

    public Memory<byte> GetDaycareSlot(int slot) => Raw.Slice((SlotSize * slot) + 8, SIZE_STORED);

    public bool IsEggAvailable
    {
        get => Data[0x1E0] == 1;
        set => Data[0x1E0] = value ? (byte)1 : (byte)0;
    }

    public ulong Seed
    {
        get => ReadUInt64LittleEndian(Data[0x1E8..]);
        set => WriteUInt64LittleEndian(Data[0x1E8..], value);
    }
}

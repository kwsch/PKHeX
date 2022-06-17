using System;
using System.ComponentModel;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

/// <summary>
/// Storage for the in-game daycare structure.
/// </summary>
/// <remarks>size: 0x2C0</remarks>
[TypeConverter(typeof(ExpandableObjectConverter))]
public sealed class Daycare8b : SaveBlock<SAV8BS>
{
    public Daycare8b(SAV8BS sav, int offset) : base(sav) => Offset = offset;

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
            throw new ArgumentOutOfRangeException(nameof(slot));

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
        get => ReadUInt32LittleEndian(Data.AsSpan(Offset + ExtraDataOffset)) == 1;
        set => WriteUInt32LittleEndian(Data.AsSpan(Offset + ExtraDataOffset), value ? 1u : 0u);
    }

    public ulong DaycareSeed
    {
        get => ReadUInt64LittleEndian(Data.AsSpan(Offset + ExtraDataOffset + 4));
        set => WriteUInt64LittleEndian(Data.AsSpan(Offset + ExtraDataOffset + 4), value);
    }

    public int EggStepCount
    {
        get => ReadInt32LittleEndian(Data.AsSpan(Offset + ExtraDataOffset + 4 + 8));
        set => WriteInt32LittleEndian(Data.AsSpan(Offset + ExtraDataOffset + 4 + 8), value);
    }
}

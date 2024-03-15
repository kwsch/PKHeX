using System;
using System.ComponentModel;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

/// <summary>
/// Storage for the in-game daycare structure.
/// </summary>
/// <remarks>size: 0x2C0</remarks>
[TypeConverter(typeof(ExpandableObjectConverter))]
public sealed class Daycare8b(SAV8BS sav, Memory<byte> raw) : SaveBlock<SAV8BS>(sav, raw), IDaycareStorage, IDaycareEggState, IDaycareRandomState<ulong>
{
    // BLOCK STRUCTURE
    // PB8[2] Parents;
    // bool32 eggExist;
    // ulong eggSeed; -- setter puts only 32 bits!
    // int32 eggStepCount;

    private const int SlotCount = 2;
    private const int ExtraDataOffset = PokeCrypto.SIZE_8PARTY * SlotCount;
    public const int SIZE = ExtraDataOffset + 16; // 0x2C0

    public int DaycareSlotCount => 2;

    public bool IsDaycareOccupied(int slot) => GetSlot(slot).Species != 0;

    public void SetDaycareOccupied(int slot, bool occupied)
    {
        if (occupied)
            return;
        GetDaycareSlot(slot).Span.Clear();
    }

    public static int GetParentSlotOffset(int slot)
    {
        if ((uint)slot >= SlotCount)
            throw new ArgumentOutOfRangeException(nameof(slot));
        return slot * PokeCrypto.SIZE_8PARTY;
    }

    public Memory<byte> GetDaycareSlot(int index) => Raw.Slice(GetParentSlotOffset(index), PokeCrypto.SIZE_8PARTY);

    public PB8 GetSlot(int slot)
    {
        var offset = GetParentSlotOffset(slot);
        var data = Data.Slice(offset, PokeCrypto.SIZE_8PARTY).ToArray();
        return new PB8(data);
    }

    private Span<byte> ExtraData => Data[ExtraDataOffset..];

    public bool IsEggAvailable
    {
        get => ReadUInt32LittleEndian(ExtraData) == 1;
        set => WriteUInt32LittleEndian(ExtraData, value ? 1u : 0u);
    }

    public ulong Seed
    {
        get => ReadUInt64LittleEndian(ExtraData[4..]);
        set => WriteUInt64LittleEndian(ExtraData[4..], value);
    }

    public int EggStepCount
    {
        get => ReadInt32LittleEndian(ExtraData[8..]);
        set => WriteInt32LittleEndian(ExtraData[8..], value);
    }
}

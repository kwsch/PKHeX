using System;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

public sealed class Daycare7(SAV7 sav, Memory<byte> raw) : SaveBlock<SAV7>(sav, raw), IDaycareStorage, IDaycareEggState, IDaycareRandomState<UInt128>
{
    private const int SlotSize = PokeCrypto.SIZE_6STORED + 1;

    public int DaycareSlotCount => 2;

    public bool IsDaycareOccupied(int slot) => Data[SlotSize * slot] != 0;
    public void SetDaycareOccupied(int slot, bool occupied) => Data[SlotSize * slot] = occupied ? (byte)1 : (byte)0;

    public Memory<byte> GetDaycareSlot(int slot) => Raw.Slice(1 + (slot * SlotSize), PokeCrypto.SIZE_6STORED);

    public bool IsEggAvailable
    {
        get => Data[0x1D8] == 1;
        set => Data[0x1D8] = value ? (byte)1 : (byte)0;
    }

    public UInt128 Seed
    {
        get => ReadUInt128LittleEndian(Data[0x1DC..]);
        set => WriteUInt128LittleEndian(Data[0x1DC..], value);
    }
}

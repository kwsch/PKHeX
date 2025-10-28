using System;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

public sealed class CaptureRecords(SAV7b sav, Memory<byte> raw) : SaveBlock<SAV7b>(sav, raw)
{
    private const int ENTRY_COUNT = 153;
    private const int MAX_COUNT_ENTRY_CAPTURE = 9_999;
    private const int MAX_COUNT_ENTRY_TRANSFER = 999_999_999;
    private const int MAX_COUNT_TOTAL = 999_999_999;

    // 0x468A8 to 0x46B0B contains 153 entries (u32) denoting how many of said Species you've caught (each cap out at 9,999).
    private const int CapturedOffset = 0x2A8;
    // 0x46B0C to 0x46D6F contains 153 entries (u32) denoting how many of said Species you've transferred to Professor Oak (each cap out at 999,999,999).
    private const int TransferredOffset = CapturedOffset + (ENTRY_COUNT * sizeof(uint)); // 0x50C

    // 0x770 ??

    // 0x46D94: u32 stores how many total Pokémon you've caught (caps out at 999,999,999).
    private const int TotalCapturedOffset = 0x794;

    // 0x46DA8: u32 that stores how many Pokémon you've transferred to Professor Oak.
    // This value is equal to the sum of all individual transferred Species, but caps out at 999,999,999 even if the sum of all individual Species exceeds this.
    private const int TotalTransferredOffset = 0x7A8;

    // Calling into these directly, you should be sure that you're less than ENTRY_COUNT.
    private static int GetCapturedOffset(int index) => CapturedOffset + (index * 4);
    private static int GetTransferredOffset(int index) => TransferredOffset + (index * 4);
    public uint GetCapturedCountIndex(int index) => ReadUInt32LittleEndian(Data[GetCapturedOffset(index)..]);
    public uint GetTransferredCountIndex(int index) => ReadUInt32LittleEndian(Data[GetTransferredOffset(index)..]);
    public void SetCapturedCountIndex(int index, uint value) => WriteUInt32LittleEndian(Data[GetCapturedOffset(index)..], Math.Min(MAX_COUNT_ENTRY_CAPTURE, value));
    public void SetTransferredCountIndex(int index, uint value) => WriteUInt32LittleEndian(Data[GetTransferredOffset(index)..], Math.Min(MAX_COUNT_ENTRY_TRANSFER, value));

    public const ushort MaxIndex = 152;

    public static ushort GetSpeciesIndex(ushort species) => species switch
    {
        <= (int) Species.Mew => (ushort)(species - 1),
        (int) Species.Meltan or (int) Species.Melmetal => (ushort)(species - 657), // 151, 152
        _ => ushort.MaxValue,
    };

    public static ushort GetIndexSpecies(ushort index)
    {
        if (index < (int) Species.Mew)
            return (ushort)(index + 1);
        return (ushort)(index + 657);
    }

    public uint GetCapturedCount(ushort species)
    {
        int index = GetSpeciesIndex(species);
        if (index > MaxIndex)
            throw new ArgumentOutOfRangeException(nameof(species));
        return GetCapturedCountIndex(index);
    }

    public void SetCapturedCount(ushort species, uint value)
    {
        int index = GetSpeciesIndex(species);
        if (index > MaxIndex)
            throw new ArgumentOutOfRangeException(nameof(species));
        SetCapturedCountIndex(index, value);
    }

    public uint GetTransferredCount(ushort species)
    {
        int index = GetSpeciesIndex(species);
        if (index > MaxIndex)
            throw new ArgumentOutOfRangeException(nameof(species));
        return GetTransferredCountIndex(index);
    }

    public void SetTransferredCount(ushort species, uint value)
    {
        int index = GetSpeciesIndex(species);
        if (index > MaxIndex)
            throw new ArgumentOutOfRangeException(nameof(species));
        SetTransferredCountIndex(index, value);
    }

    public uint TotalCaptured
    {
        get => ReadUInt32LittleEndian(Data[TotalCapturedOffset..]);
        set => WriteUInt32LittleEndian(Data[TotalCapturedOffset..], Math.Min(MAX_COUNT_TOTAL, value));
    }

    public uint TotalTransferred
    {
        get => ReadUInt32LittleEndian(Data[TotalTransferredOffset..]);
        set => WriteUInt32LittleEndian(Data[TotalTransferredOffset..], Math.Min(MAX_COUNT_TOTAL, value));
    }

    public uint CalculateTotalCaptured()
    {
        uint total = 0;
        for (int i = 0; i < ENTRY_COUNT; i++)
            total += GetCapturedCountIndex(i);
        return Math.Min(total, MAX_COUNT_TOTAL);
    }

    public uint CalculateTotalTransferred()
    {
        ulong total = 0;
        for (int i = 0; i < ENTRY_COUNT; i++)
            total += GetTransferredCountIndex(i);
        return (uint)Math.Min(total, MAX_COUNT_TOTAL);
    }

    public void SetAllCaptured(uint count = MAX_COUNT_ENTRY_CAPTURE, Zukan7b? dex = null)
    {
        uint total = 0;
        count = Math.Min(count, MAX_COUNT_ENTRY_CAPTURE);
        for (ushort i = 0; i < ENTRY_COUNT; i++)
        {
            ushort species = GetIndexSpecies(i);
            if (count != 0 && dex?.GetCaught(species) == false)
            {
                total += GetCapturedCountIndex(i);
                continue;
            }
            SetCapturedCountIndex(i, count);
            total += count;
        }
        if (total < TotalCaptured)
            TotalCaptured = total;
    }

    public void SetAllTransferred(uint count = MAX_COUNT_ENTRY_TRANSFER, Zukan7b? dex = null)
    {
        uint total = 0;
        count = Math.Min(count, MAX_COUNT_ENTRY_TRANSFER);
        for (ushort i = 0; i < ENTRY_COUNT; i++)
        {
            ushort species = GetIndexSpecies(i);
            if (count != 0 && dex?.GetCaught(species) == false)
            {
                total += GetTransferredCountIndex(i);
                continue;
            }
            SetTransferredCountIndex(i, count);
            total += count;
        }
        if (total < TotalTransferred)
            TotalTransferred = total;
    }
}

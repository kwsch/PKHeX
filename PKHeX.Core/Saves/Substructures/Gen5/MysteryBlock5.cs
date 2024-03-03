using System;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

public sealed class MysteryBlock5(SAV5 sav, Memory<byte> raw) : SaveBlock<SAV5>(sav, raw), IMysteryGiftStorage, IMysteryGiftFlags
{
    private const int FlagStart = 0;
    private const int MaxReceivedFlag = 2048;
    private const int MaxCardsPresent = 12;
    private const int FlagRegionSize = (MaxReceivedFlag / 8); // 0x100
    private const int CardStart = FlagStart + FlagRegionSize;

    private const int DataSize = 0xA90;

    // Everything is stored encrypted, and only decrypted on demand. Only crypt on object fetch...
    private uint AlbumSeed
    {
        get => ReadUInt32LittleEndian(Data[DataSize..]);
        set => WriteUInt32LittleEndian(Data[DataSize..], value);
    }

    private Span<byte> DataRegion => Data[..^4]; // 0xA90
    private Span<byte> FlagRegion => Data[..CardStart]; // 0x100

    private bool IsDecrypted;
    public void EndAccess() => EnsureDecrypted(false);
    private void EnsureDecrypted(bool state = true)
    {
        if (IsDecrypted == state)
            return;
        PokeCrypto.CryptArray(DataRegion, AlbumSeed);
        IsDecrypted = state;
    }

    public void ClearReceivedFlags()
    {
        EnsureDecrypted();
        FlagRegion.Clear();
    }

    private static int GetGiftOffset(int index)
    {
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual((uint)index, (uint)MaxCardsPresent);
        return CardStart + (index * PGF.Size);
    }

    private Span<byte> GetCardSpan(int index)
    {
        var offset = GetGiftOffset(index);
        EnsureDecrypted();
        return Data.Slice(offset, PGF.Size);
    }

    public PGF GetMysteryGift(int index) => new(GetCardSpan(index).ToArray());

    public void SetMysteryGift(int index, PGF pgf)
    {
        if ((uint)index > MaxCardsPresent)
            throw new ArgumentOutOfRangeException(nameof(index));
        if (pgf.Data.Length != PGF.Size)
            throw new InvalidCastException(nameof(pgf));
        SAV.SetData(GetCardSpan(index), pgf.Data);
    }

    public int MysteryGiftReceivedFlagMax => MaxReceivedFlag;
    public bool GetMysteryGiftReceivedFlag(int index)
    {
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual((uint)index, (uint)MaxReceivedFlag);
        EnsureDecrypted();
        return FlagUtil.GetFlag(Data, index); // offset 0
    }

    public void SetMysteryGiftReceivedFlag(int index, bool value)
    {
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual((uint)index, (uint)MaxReceivedFlag);
        EnsureDecrypted();
        FlagUtil.SetFlag(Data, index, value); // offset 0
    }

    public int GiftCountMax => MaxCardsPresent;
    DataMysteryGift IMysteryGiftStorage.GetMysteryGift(int index) => GetMysteryGift(index);
    void IMysteryGiftStorage.SetMysteryGift(int index, DataMysteryGift gift) => SetMysteryGift(index, (PGF)gift);
}

public sealed class GTS5(SAV5 sav, Memory<byte> raw) : SaveBlock<SAV5>(sav, raw)
{
    // 0x08: Stored Upload
    private const int SizeStored = PokeCrypto.SIZE_5STORED;

    public Memory<byte> Upload => Raw[..SizeStored];
}

public sealed class GlobalLink5(SAV5 sav, Memory<byte> raw) : SaveBlock<SAV5>(sav, raw)
{
    // 0x08: Stored Upload
    private const int SizeStored = PokeCrypto.SIZE_5STORED;

    public Memory<byte> Upload => Raw.Slice(8, SizeStored);
}

public sealed class AdventureInfo5(SAV5 sav, Memory<byte> raw) : SaveBlock<SAV5>(sav, raw)
{
    public uint SecondsToStart { get => ReadUInt32LittleEndian(Data[0x34..]); set => WriteUInt32LittleEndian(Data[0x34..], value); }
    public uint SecondsToFame { get => ReadUInt32LittleEndian(Data[0x3C..]); set => WriteUInt32LittleEndian(Data[0x3C..], value); }
}

using System;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

public class Record5(SAV5 SAV, Memory<byte> raw) : SaveBlock<SAV5>(SAV, raw)
{
    private Span<byte> DataRegion => Data[4..^4]; // 4..0x1DC

    private uint CryptoSeed // 0x1DC
    {
        get => ReadUInt32LittleEndian(Data[^4..]);
        set => WriteUInt32LittleEndian(Data[^4..], value);
    }

    private bool IsDecrypted;
    public void EndAccess() => EnsureDecrypted(false);
    private void EnsureDecrypted(bool state = true)
    {
        if (IsDecrypted == state)
            return;
        PokeCrypto.CryptArray(DataRegion, CryptoSeed);
        IsDecrypted = state;
    }

    public uint Revision // 0x00
    {
        get => ReadUInt32LittleEndian(Data);
        set => WriteUInt32LittleEndian(Data, value);
    }

    public const int Record32 = 68;
    public const int Record16 = 100;
    private const int Partition2 = Record32 * sizeof(uint);
    private Span<byte> Record32Data => DataRegion[..Partition2];
    private Span<byte> Record16Data => DataRegion[Partition2..];

    private const uint Max32 = 999_999_999;
    private const ushort Max16 = 9999;

    public uint GetRecord32(int index)
    {
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual<uint>((uint)index, Record32);
        EnsureDecrypted();
        return ReadUInt32LittleEndian(Record32Data[(index * 4)..]);
    }

    public void SetRecord32(int index, uint value)
    {
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual<uint>((uint)index, Record32);
        EnsureDecrypted();
        WriteUInt32LittleEndian(Record32Data[(index * 4)..], Math.Min(Max32, value));
    }

    public ushort GetRecord16(int index)
    {
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual<uint>((uint)index, Record16);
        EnsureDecrypted();
        return ReadUInt16LittleEndian(Record16Data[(index * 2)..]);
    }

    public void SetRecord16(int index, ushort value)
    {
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual<uint>((uint)index, Record16);
        EnsureDecrypted();
        WriteUInt16LittleEndian(Record16Data[(index * 2)..], Math.Min(Max16, value));
    }

    public enum Record5Index
    {
        TimesSaved = 0,
        StepsTaken = 1,
        UsedBicycle = 2,
        TotalBattles = 3,
        WildBattles = 4,
        TrainerBattles = 5,
        Captured = 6,
        CapturedFishing = 7,
        EggsHatched = 8,
        PokemonEvolved = 9,
        TimesHealedPokeCenter = 10,

        // ???

        LinkTrades = 21,
        LinkBattles = 22,
        LinkBattleWins = 23,
        LinkBattleLosses = 24,

        // 00 - 0x110: start of u16 records
        // 46 - 0x16C: Feeling Checks
        // 47 - 0x16E: Musical
        // 56 - 0x180: Battle Tests Attempted
        // 57 - 0x182: Battle Test High Score
        // 60 - 0x188: Customers
        // 64 - 0x190: Movie Shoots
        FirstU16             = Record32 + 00,
        FeelingsChecked      = Record32 + 46,
        Musical              = Record32 + 47,
        BattleTestsAttempted = Record32 + 56,
        BattleTestHighScore  = Record32 + 57,
        Customers            = Record32 + 60,
        MovieShoots          = Record32 + 64,
    }
}

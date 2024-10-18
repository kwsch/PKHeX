using System;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

public static class Record5(SAV5 SAV, Memory<byte> raw) : SaveBlock<SAV5>(SAV, raw)
{
    private Span<byte> DataRegion => Data[4..^4]; // 0..0x1DC

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

    public static class Records
{
    private const byte LargeRecordCount = 68; // int32
    private const byte SmallRecordCount = 100; // int16
    private const byte Count = LargeRecordCount + SmallRecordCount;

    /// <summary>
    /// Gets the maximum value for the specified record using the provided maximum list.
    /// </summary>
    /// <param name="recordID">Record ID to retrieve the maximum for</param>
    /// <param name="maxes">Maximum enum values for each record</param>
    /// <returns>Maximum the record can be</returns>
    public static int GetMax(int recordID, ReadOnlySpan<byte> maxes)
    {
        if ((byte)recordID >= Count)
            return 0;
        return MaxByType[maxes[recordID]];
        
    }

    public static int GetOffset(int recordID) => recordID switch
    {
        < LargeRecordCount => (recordID * sizeof(int)),
        < Count => (LargeRecordCount * sizeof(int)) + ((recordID - LargeRecordCount) * sizeof(ushort)),
        _ => -1,
    };

    private const uint Max32 = 999_999_999;
    private const ushort Max16 = 65535;

    public static class RecordLists
{
    public static readonly Dictionary<int, string> RecordList_5 = new()
    {
        {000, "Steps Taken"},
        {001, "Times Saved"},
        {002, "Storyline Completed Time"},
        {003, "Times Bicycled"},
        {004, "Total Battles"},
        {005, "Wild Pokémon Battles"},
        {006, "Trainer Battles"},
        {007, "Pokemon Caught"},
        {008, "Times fished"},
        {009, "Eggs Hatched"},
        {010, "Pokémon Evolved"},
        {011, "Times Healed at Pokémon Centers"},
        {012, "Link Trades"},
        {013, "Link Battles"},
        {014, "Link Battle Wins"},
        {015, "Link Battle Losses"},
        {016, "WiFi Trades"},
        {017, "WiFi Battles"},
        {018, "WiFi Battle Wins"},
        {019, "WiFi Battle Losses"},
        {020, "Times Shopped"},
        {021, "Money Spent"},
        {022, "TVs Watched"},
        {023, "Pokemon deposited in Daycare"},
        {024, "Pokemon Defeated"},
        {025, "EXP Earned (highest)"},
        {026, "EXP Earned (today)"},
        {027, "GTS Used"},
        {028, "Mail Sent"},
        {029, "Nicknames Given"},
        {030, "Premier Balls earned"},
        {031, "Nimbasa Stadium Battles"},
        {032, "BP Earned"},
        {033, "BP Spent"},
        {034, "???"},
        {035, "IR Trades"},
        
        {036, "IR Battles"},
        {037, "IR Wins"},
        {038, "IR Losses"},
        {039, "???"},
        {040, "???"},
        {041, "Times used Fly"},
        {042, "Trash Cans Checked"},
        {043, "Hidden Items Found"},
        {044, "Pass Powers Used"},
        {045, "Pokemon Caught in Entralink"},
        {046, "Super Effective Moves Used"},
        {047, "Times Challenged Battle Subway"},
        {048, "Tower/Treehollow Trainers Defeated"},
        {049, "Balloon Game Tottal Points"},
        {050, "Highest Box Office Gross"},
        {051, "Total Box Office Gross"},
        
        {052, "???"},
        {053, "???"},
        {054, "???"},
        {055, "???"},
        {056, "???"},
        {057, "???"},
        {058, "???"},
        {059, "???"},
        {060, "???"},
        {061, "???"},
        {062, "???"},
        {063, "???"},
        {064, "???"},
        {065, "???"},
        {066, "???"},
        {067, "???"},
        {068, "???"},
        {069, "???"},
        {070, "???"},
        
        {071, "Champion Beaten"},
        {072, "Healed with Mom"},
        {073, "Used Splash"},
        {074, "Used Struggle"},
        {075, "Noneffective move used on you"},
        {076, "Own Pokemon Attacked"},
        {077, "Own Pokemon Fainted"},
        {078, "Failed to Run"},
        {079, "Pokemon Fled"},
        {080, "Failed Fishing"},
        {081, "Pokemon defeated (highest)"},
        {082, "Pokemon defeated (today)"},
        {083, "Trainers defeated (highest)"},
        {084, "Trainers defeated (today)"},
        {085, "Pokemon evolved (highest)"},
        {086, "Pokemon evolved (today)"},
        {087, "Fossils Restored"},
        {088, "Spin Trade"},
        
        {089, "???"},
        {090, "???"},
        {091, "???"},
        {092, "???"},
        {093, "???"},
        {094, "???"},
        {095, "???"},
        {096, "???"},
        {097, "???"},
        {098, "???"},
        {099, "???"},
        {100, "???"},
        {101, "???"},
        {102, "???"},
        {103, "???"},
        {104, "???"},
        {105, "???"},
        {106, "???"},
        {107, "???"},
        {108, "???"},
        {109, "???"},
        
        {110, "Disturbed Tile Encounters"},
        {111, "Feeling Check"},
        {112, "Musicals Participated In"},
        {113, "Musicals Won"},
        {114, "Musicals with Friends"},
        {115, "Musicals with Friends Won"},
        {116, "Musical Fame Score"},
        {117, "Pokemon Tucked In"}, 
        {118, "Poketransfer Minigame Played"}, 
        {119, "Battle Institute Attempts"},
        {120, "???"},
        {121, "Battle Institute Max Rank reached"},
        {122, "Battle Test High Score"},
        {123, "Vending Machines Used"},
        {124, "Rode Royal Unova"},
        {125, "Passers-by Guided"}, 
        {126, "Shops Created"},
        {127, "Xtransciever minigames Played"},
        {128, "Souvenirs Collected"},
        {129, "Movie Shoots"},

    };

        // 00 - 0x110: start of u16 records
        // 00 - 0x11C: Champion Beaten
        // 46 - 0x16C: Feeling Checks
        // 47 - 0x16E: Musical
        // 56 - 0x180: Battle Tests Attempted
        // 57 - 0x182: Battle Test High Score
        // 60 - 0x188: Customers
        // 64 - 0x190: Movie Shoots
        FirstU16             = Record16 + 00,
        FeelingsChecked      = Record16 + 46,
        Musical              = Record16 + 47,
        BattleTestsAttempted = Record16 + 56,
        BattleTestHighScore  = Record16 + 57,
        Customers            = Record16 + 60,
        MovieShoots          = Record16 + 64,
    }
}

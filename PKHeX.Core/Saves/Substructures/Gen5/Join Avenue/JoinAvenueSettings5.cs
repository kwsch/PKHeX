using System;
using System.Runtime.InteropServices;
using static System.Buffers.Binary.BinaryPrimitives;
using static PKHeX.Core.StringConverter5;

namespace PKHeX.Core;

public sealed class JoinAvenueSettings5(Memory<byte> data, SAV5B2W2 sav) // 12F4 within the block, 24EF4 in SAV
{
    public const int SIZE = 0xEC;
    private Span<byte> Data => data.Span;

    public string Name // 20 chars + terminator
    {
        get => GetString(Data[..0x2A]);
        set => SetString(Data[..0x2A], value, 20, sav.Language);
    }

    public string PlayerTitle // 20 chars + terminator
    {
        get => GetString(Data[0x2A..0x54]);
        set => SetString(Data[0x2A..0x54], value, 20, sav.Language);
    }

    // 32 visiting player trainer IDs remembered, reset daily. Store 0xFFFFFFFF for empty.
    public const ushort CountVisitingPlayersRemembered = 32;
    public const uint VisitingPlayerDefaultNone = uint.MaxValue;

    public Span<uint> VisitingPlayerDatabase => MemoryMarshal.Cast<byte, uint>(Data.Slice(0x54, CountVisitingPlayersRemembered * sizeof(uint)));

    public uint GetVisitingPlayerTrainerID(int index)
    {
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(index, CountVisitingPlayersRemembered);
        return ReadUInt32LittleEndian(Data[(0x54 + (index * sizeof(uint)))..]);
    }

    public void SetVisitingPlayerTrainerID(int index, uint value)
    {
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(index, CountVisitingPlayersRemembered);
        WriteUInt32LittleEndian(Data[(0x54 + (index * sizeof(uint)))..], value);
    }

    public uint Experience { get => ReadUInt32LittleEndian(Data[0xD4..]); set => WriteUInt32LittleEndian(Data[0xD4..], value); }
    public ushort Rank { get => ReadUInt16LittleEndian(Data[0xD8..]); set => WriteUInt16LittleEndian(Data[0xD8..], Math.Min(value, MaxAvenueRank)); }

    public const ushort MaxAvenueRank = 9999;

    public JoinAvenueCeilingColor5 CeilingColor
    {
        get => (JoinAvenueCeilingColor5)ReadUInt16LittleEndian(Data[0xDA..]);
        set => WriteUInt16LittleEndian(Data[0xDA..], (ushort)value);
    }

    public uint Flags { get => ReadUInt32LittleEndian(Data[0xDC..]); set => WriteUInt32LittleEndian(Data[0xDC..], value); }
    // ???

    /// <summary>
    /// <see cref="GetVisitingPlayerTrainerID"/> is a database of trainer IDs that have been connected to the Join Avenue, managed as a circular buffer.
    /// </summary>
    public ushort VisitingPlayerDatabaseCount { get => ReadUInt16LittleEndian(Data[0xE0..]); set => WriteUInt16LittleEndian(Data[0xE0..], Math.Min(value, CountVisitingPlayersRemembered)); }
    public ushort VistiingPlayerDatabaseInsertIndex { get => ReadUInt16LittleEndian(Data[0xE2..]); set => WriteUInt16LittleEndian(Data[0xE2..], (ushort)(value % CountVisitingPlayersRemembered)); }

    public uint Seed { get => ReadUInt32LittleEndian(Data[0xE4..]); set => WriteUInt32LittleEndian(Data[0xE4..], value); }

    public ushort PromotionDaysElapsed { get => ReadUInt16LittleEndian(Data[0xE8..]); set => WriteUInt16LittleEndian(Data[0xE8..], value); }
    public bool IsPromotionActive { get => ReadUInt16LittleEndian(Data[0xEA..]) == 1; set => WriteUInt16LittleEndian(Data[0xEA..], value ? (ushort)1 : (ushort)0); }

    /// <summary>
    /// A promotion being active only lasts a full week [0..6], and then expires on the 7th day, which is when the counter resets and the promotion must be activated again.
    /// </summary>
    public const ushort PromotionDaysMax = 7;

    /// <summary>
    /// Adds the trainer ID to the database of players that have visited today.
    /// </summary>
    /// <param name="id32">Player trainer ID to add.</param>
    /// <returns>Index at which the trainer ID was added. If the database is full, it will overwrite the oldest entry.</returns>
    public int AddPlayerVisitor(uint id32)
    {
        int index = VistiingPlayerDatabaseInsertIndex;
        SetVisitingPlayerTrainerID(index, id32);

        // setters will auto-clamp the count and wrap the index, so we can just increment and let it handle the rest
        VisitingPlayerDatabaseCount = (ushort)(index + 1);
        VisitingPlayerDatabaseCount++;
        return index;
    }

    /// <summary>
    /// Daily reset to allow a player to visit again.
    /// </summary>
    public void ResetPlayerVisitList()
    {
        VisitingPlayerDatabase.Fill(VisitingPlayerDefaultNone);
        VisitingPlayerDatabaseCount = 0;
        VistiingPlayerDatabaseInsertIndex = 0;
    }

    /// <summary>
    /// Checks if the player has visited today.
    /// </summary>
    /// <param name="id32">Player trainer ID to check for.</param>
    public bool HasPlayerVisitedToday(uint id32)
    {
        if (!BitConverter.IsLittleEndian)
            id32 = ReverseEndianness(id32);
        return VisitingPlayerDatabase[..VisitingPlayerDatabaseCount].Contains(id32);
    }
}

using System;
using System.Collections.Generic;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

/// <summary>
/// Stores 12 different sets of record data, with the earliest entry being called the "head" record index.
/// </summary>
/// <remarks>size: 0x5A0 (12 * 4*30)</remarks>
public sealed class Record8b(SAV8BS sav, Memory<byte> raw) : SaveBlock<SAV8BS>(sav, raw), IRecordStatStorage
{
    public const int RecordIndexCount = 12; // There's a total of 12 uint[30] record entries. The head one is used, not sure about the others.
    public const int RecordCount = 30;
    public const int RecordMaxValue = 999_999;

    public static int GetMax(int recordID) => MaxValue_BDSP[recordID];

    private static int ClampRecord(int recordID, int value)
    {
        var max = MaxValue_BDSP[recordID];
        return Math.Min(max, value);
    }

    public static int GetRecordOffset(int recordID)
    {
        if ((uint)recordID >= RecordCount)
            throw new ArgumentOutOfRangeException(nameof(recordID));
        return (sizeof(int) * recordID);
    }

    public int GetRecord(int recordID)
    {
        var value = ReadInt32LittleEndian(Data[GetRecordOffset(recordID)..]);
        if (recordID != 0)
            value = ClampRecord(recordID, value);
        return value;
    }

    public void SetRecord(int recordID, int value)
    {
        if (recordID != 0)
            value = Math.Min(RecordMaxValue, value);
        WriteInt32LittleEndian(Data[GetRecordOffset(recordID)..], value);
    }

    public void AddRecord(int recordID, int count = 1) => SetRecord(recordID, GetRecord(recordID) + count);

    public static ReadOnlySpan<int> MaxValue_BDSP =>
    [
        int.MaxValue, // CLEAR_TIME
        9_999, // DENDOU_CNT
        999_999, // CAPTURE_POKE
        999_999, // FISHING_SUCCESS
        999_999, // TAMAGO_HATCHING
        999_999, // BEAT_DOWN_POKE
        9_999, // RENSHOU_SINGLE
        9_999, // RENSHOU_SINGLE_NOW
        9_999, // RENSHOU_DOUBLE
        9_999, // RENSHOU_DOUBLE_NOW
        9_999, // RENSHOU_MASTER_SINGLE
        9_999, // RENSHOU_MASTER_SINGLE_NOW
        9_999, // RENSHOU_MASTER_DOUBLE
        9_999, // RENSHOU_MASTER_DOUBLE_NOW
        7, // BTL_TOWER_AVERAGE
        5, // CONTEST_STYLE_RANK
        5, // CONTEST_BEATIFUL_RANK
        5, // CONTEST_CUTE_RANK
        5, // CONTEST_CLEVER_RANK
        5, // CONTEST_STRONG_RANK
        9_999, // CONTEST_PLAY_SINGLE
        9_999, // CONTEST_PLAY_LOCAL
        9_999, // CONTEST_PLAY_NETWORK
        9_999, // CONTEST_WIN_SINGLE
        9_999, // CONTEST_WIN_LOCAL
        9_999, // CONTEST_WIN_NETWORK
        100,  // CONTEST_RATE_SINGLE
        100,  // CONTEST_RATE_LOCAL
        100,  // CONTEST_RATE_NETWORK
        65_536,// CONTEST_GET_RIBBON
    ];

    public static readonly Dictionary<int, string> RecordList_8b = new()
    {
        { 00, "CLEAR_TIME" },
        { 01, "DENDOU_CNT" },
        { 02, "CAPTURE_POKE" },
        { 03, "FISHING_SUCCESS" },
        { 04, "TAMAGO_HATCHING" },
        { 05, "BEAT_DOWN_POKE" },
        { 06, "RENSHOU_SINGLE" },
        { 07, "RENSHOU_SINGLE_NOW" },
        { 08, "RENSHOU_DOUBLE" },
        { 09, "RENSHOU_DOUBLE_NOW" },
        { 10, "RENSHOU_MASTER_SINGLE" },
        { 11, "RENSHOU_MASTER_SINGLE_NOW" },
        { 12, "RENSHOU_MASTER_DOUBLE" },
        { 13, "RENSHOU_MASTER_DOUBLE_NOW" },
        { 14, "BTL_TOWER_AVERAGE" },
        { 15, "CONTEST_STYLE_RANK" },
        { 16, "CONTEST_BEATIFUL_RANK" },
        { 17, "CONTEST_CUTE_RANK" },
        { 18, "CONTEST_CLEVER_RANK" },
        { 19, "CONTEST_STRONG_RANK" },
        { 20, "CONTEST_PLAY_SINGLE" },
        { 21, "CONTEST_PLAY_LOCAL" },
        { 22, "CONTEST_PLAY_NETWORK" },
        { 23, "CONTEST_WIN_SINGLE" },
        { 24, "CONTEST_WIN_LOCAL" },
        { 25, "CONTEST_WIN_NETWORK" },
        { 26, "CONTEST_RATE_SINGLE" },
        { 27, "CONTEST_RATE_LOCAL" },
        { 28, "CONTEST_RATE_NETWORK" },
        { 29, "CONTEST_GET_RIBBON" },
    };
}

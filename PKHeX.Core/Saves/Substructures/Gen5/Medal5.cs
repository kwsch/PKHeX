using System;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

public sealed class Medal5(Memory<byte> Data)
{
    public const int SIZE = 4;
    private Span<byte> Span => Data.Span;

    // Structure:
    // ushort Date:7
    // ushort Month:4
    // ushort Day:5

    // byte State:3
    // byte IsUnread:1
    // byte unused:4

    // byte unused

    private const int EpochYear = 2000;

    public ushort RawDate
    {
        get => ReadUInt16LittleEndian(Span);
        set => WriteUInt16LittleEndian(Span, value);
    }

    public int Year
    {
        get => (RawDate & 0x007F) + EpochYear;
        set => RawDate = (ushort)((RawDate & 0xFF80) | ((value - EpochYear) & 0x007F));
    }

    public int Month
    {
        get => (RawDate & 0x0780) >> 7;
        set => RawDate = (ushort)((RawDate & 0xF87F) | ((value & 0x0F) << 7));
    }

    public int Day
    {
        get => RawDate >> 11;
        set => RawDate = (ushort)((RawDate & 0x07FF) | ((value & 0x1F) << 11));
    }

    public Medal5State State
    {
        get => (Medal5State)(Span[2] & 0b0111);
        set => Span[2] = (byte)((Span[2] & 0b1000) | ((int)value & 0b0111));
    }

    public bool IsUnread
    {
        get => FlagUtil.GetFlag(Span, 2, 3);
        set => FlagUtil.SetFlag(Span, 2, 3, value);
    }

    public bool CanHaveDate => State switch
    {
        Medal5State.HintObtained => true,
        Medal5State.Obtained => true,
        Medal5State.ObtainReady => HasDate,
        _ => false,
    };

    public bool HasDate => RawDate != 0;
    public bool IsObtained => State == Medal5State.Obtained;
    public void Clear() => Span.Clear();

    public DateOnly Date { get => GetDate(RawDate); set => RawDate = GetDate(value); }

    private static ushort GetDate(DateOnly date)
    {
        int year = date.Year - EpochYear;
        int month = date.Month;
        int day = date.Day;
        return (ushort)((year & 0x007F) | ((month & 0x0F) << 7) | ((day & 0x1F) << 11));
    }

    public static DateOnly GetDate(ushort date)
    {
        int year = (date & 0x007F) + EpochYear;
        int month = (date & 0x0780) >> 7;
        int day = date >> 11;
        return new DateOnly(year, month, day);
    }

    public void Obtain(DateOnly time, bool unread = true)
    {
        RawDate = GetDate(time);
        State = Medal5State.Obtained;
        IsUnread = unread;
    }
}

public enum Medal5State
{
    Unobtained = 0,
    HintReady = 1,
    HintObtained = 2,
    ObtainReady = 3,
    Obtained = 4,
}

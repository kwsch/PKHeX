using System;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

/// <summary>
/// Simple 4-byte block storing time played in a save file.
/// </summary>
public abstract class PlayTime<TSave>(TSave sav, Memory<byte> raw) : SaveBlock<TSave>(sav, raw)
    where TSave : SaveFile
{
    public int PlayedHours
    {
        get => ReadUInt16LittleEndian(Data);
        set => WriteUInt16LittleEndian(Data, (ushort)value);
    }

    public int PlayedMinutes
    {
        get => Data[2];
        set => Data[2] = (byte)value;
    }

    public int PlayedSeconds
    {
        get => Data[3];
        set => Data[3] = (byte)value;
    }

    public string PlayedTime => $"{PlayedHours:0000}ː{PlayedMinutes:00}ː{PlayedSeconds:00}"; // not :
}

/// <summary>
/// Object storing the playtime of a save file as well as the last saved date.
/// </summary>
/// <typeparam name="TSave">Type of Save File</typeparam>
/// <typeparam name="TEpoch">Type of Epoch for the <see cref="LastSaved"/> timestamp.</typeparam>
public abstract class PlayTimeLastSaved<TSave, TEpoch>(TSave sav, Memory<byte> raw) : PlayTime<TSave>(sav, raw)
    where TSave : SaveFile
    where TEpoch : EpochDateTime
{
    protected abstract TEpoch LastSaved { get; }
    public string LastSavedTime => $"{LastSaved.Year:0000}-{LastSaved.Month:00}-{LastSaved.Day:00} {LastSaved.Hour:00}ː{LastSaved.Minute:00}"; // not :

    public DateTime? LastSavedDate
    {
        get => !DateUtil.IsDateValid(LastSaved.Year, LastSaved.Month, LastSaved.Day)
            ? null
            : LastSaved.Timestamp;
        set
        {
            // Only update the properties if a value is provided.
            if (value is { } dt)
            {
                LastSaved.Timestamp = dt;
            }
            else // Clear the date.
            {
                // If code tries to access MetDate again, null will be returned.
                LastSaved.Year = 0;
                LastSaved.Month = 0;
                LastSaved.Day = 0;
                LastSaved.Hour = 0;
                LastSaved.Minute = 0;
            }
        }
    }
}

/// <summary>
/// PlayTime object with a zero-epoch Last Saved timestamp.
/// </summary>
public sealed class PlayTime6 : PlayTimeLastSaved<SaveFile, Epoch0000DateTime>
{
    public PlayTime6(SAV6 sav, Memory<byte> raw) : base(sav, raw) { }
    public PlayTime6(SAV7 sav, Memory<byte> raw) : base(sav, raw) { }

    protected override Epoch0000DateTime LastSaved => new(Raw.Slice(0x4, 4));
}

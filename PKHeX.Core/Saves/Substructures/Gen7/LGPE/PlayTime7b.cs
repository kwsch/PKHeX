using System;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

public sealed class PlayTime7b : SaveBlock<SaveFile>
{
    public PlayTime7b(SAV7b sav, Memory<byte> raw) : base(sav, raw) { }
    public PlayTime7b(SAV8SWSH sav, SCBlock block) : base(sav, block.Data) { }

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

    private Epoch1900DateTimeValue LastSaved => new(Raw.Slice(0x4, 4));
    public string LastSavedTime => LastSaved.DisplayValue;

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

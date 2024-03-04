using System;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

public sealed class PlayTime7b(SAV7b sav, Memory<byte> raw) : SaveBlock<SAV7b>(sav, raw)
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

    private Epoch1900DateTimeValue LastSaved => new(Raw.Slice(0x4, 4));
    public string LastSavedTime => $"{LastSaved.Year:0000}-{LastSaved.Month:00}-{LastSaved.Day:00} {LastSaved.Hour:00}ː{LastSaved.Minute:00}"; // not :

    public DateTime? LastSavedDate
    {
        get => !DateUtil.IsDateValid(LastSaved.Year, LastSaved.Month, LastSaved.Day)
            ? null
            : new DateTime(LastSaved.Year, LastSaved.Month, LastSaved.Day, LastSaved.Hour, LastSaved.Minute, 0);
        set
        {
            // Only update the properties if a value is provided.
            if (value.HasValue)
            {
                var dt = value.Value;
                LastSaved.Year = dt.Year;
                LastSaved.Month = dt.Month;
                LastSaved.Day = dt.Day;
                LastSaved.Hour = dt.Hour;
                LastSaved.Minute = dt.Minute;
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

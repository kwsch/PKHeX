using System;
using System.ComponentModel;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

/// <summary>
/// Stores the <see cref="LastSavedDate"/> of the player.
/// </summary>
/// <remarks>
/// Year value is offset by -1900.
/// Month value is offset by -1.
/// </remarks>
[TypeConverter(typeof(ExpandableObjectConverter))]
public sealed class LastSaved8a(SAV8LA sav, SCBlock block) : SaveBlock<SAV8LA>(sav, block.Data)
{
    private uint LastSaved { get => ReadUInt32LittleEndian(Data.AsSpan(Offset + 0x0)); set => WriteUInt32LittleEndian(Data.AsSpan(Offset + 0x0), value); }
    private int LastSavedYear { get => (int)(LastSaved & 0xFFF); set => LastSaved = (LastSaved & 0xFFFFF000) | (uint)value; }
    private int LastSavedMonth { get => (int)((LastSaved >> 12) & 0xF); set => LastSaved = (LastSaved & 0xFFFF0FFF) | (((uint)value & 0xF) << 12); }
    private int LastSavedDay { get => (int)((LastSaved >> 16) & 0x1F); set => LastSaved = (LastSaved & 0xFFE0FFFF) | (((uint)value & 0x1F) << 16); }
    private int LastSavedHour { get => (int)((LastSaved >> 21) & 0x1F); set => LastSaved = (LastSaved & 0xFC1FFFFF) | (((uint)value & 0x1F) << 21); }
    private int LastSavedMinute { get => (int)((LastSaved >> 26) & 0x3F); set => LastSaved = (LastSaved & 0x03FFFFFF) | (((uint)value & 0x3F) << 26); }
    public string LastSavedTime => $"{LastSavedYear+1900:0000}-{LastSavedMonth+1:00}-{LastSavedDay:00} {LastSavedHour:00}ː{LastSavedMinute:00}"; // not :

    public DateTime? LastSavedDate
    {
        get => !DateUtil.IsDateValid(LastSavedYear + 1900, LastSavedMonth + 1, LastSavedDay)
            ? null
            : new DateTime(LastSavedYear + 1900, LastSavedMonth + 1, LastSavedDay, LastSavedHour, LastSavedMinute, 0);
        set
        {
            // Only update the properties if a value is provided.
            if (value.HasValue)
            {
                var dt = value.Value;
                LastSavedYear = dt.Year - 1900;
                LastSavedMonth = dt.Month - 1;
                LastSavedDay = dt.Day;
                LastSavedHour = dt.Hour;
                LastSavedMinute = dt.Minute;
            }
            else // Clear the date.
            {
                // If code tries to access MetDate again, null will be returned.
                LastSavedYear = 0;
                LastSavedMonth = 0;
                LastSavedDay = 0;
                LastSavedHour = 0;
                LastSavedMinute = 0;
            }
        }
    }
}

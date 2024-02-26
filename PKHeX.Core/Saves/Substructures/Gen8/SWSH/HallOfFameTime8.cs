using System;
using System.Diagnostics;
using System.ComponentModel;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

public sealed class HallOfFameTime8 : SaveBlock<SAV8SWSH>
{
    public HallOfFameTime8(SAV8SWSH sav, SCBlock block) : base(sav, block.Data)
    {
        Debug.Assert(Data.Length == 8);
    }

    private const string General = nameof(General);

    [Category(General), Description("Raw amount of seconds since 1970 (Unix Timestamp)")]
    public long TimeStamp
    {
        get => ReadInt64LittleEndian(Data);
        set => WriteInt64LittleEndian(Data, value);
    }

    [Category(General), Description("Date and Time (UTC) the player entered the Hall Of Fame")]
    public DateTime Date
    {
        get
        {
            var epoch = new DateTime(1970, 1, 1);
            var date = epoch.AddSeconds(TimeStamp);
            return date;
        }
        set
        {
            var epoch = new DateTime(1970, 1, 1);
            var delta = value.Subtract(epoch);
            TimeStamp = (long)delta.TotalSeconds;
        }
    }
}

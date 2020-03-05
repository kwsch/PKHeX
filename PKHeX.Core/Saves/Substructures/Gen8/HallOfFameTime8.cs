using System;
using System.Diagnostics;
using System.ComponentModel;

namespace PKHeX.Core
{
    public sealed class HallOfFameTime8 : SaveBlock
    {
        public HallOfFameTime8(SAV8SWSH sav, SCBlock block) : base(sav, block.Data)
        {
            Debug.Assert(Data.Length == 8);
        }

        private const string General = nameof(General);

        [Category(General), Description("Raw amount of seconds since 1970 (Unix Timestamp)")]
        public long TimeStamp
        {
            get => BitConverter.ToInt64(Data, Offset + 0);
            set => SAV.SetData(Data, BitConverter.GetBytes(value), Offset + 0);
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
}
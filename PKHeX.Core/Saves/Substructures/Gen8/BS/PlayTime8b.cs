using System;
using System.ComponentModel;

namespace PKHeX.Core
{
    /// <summary>
    /// Playtime storage
    /// </summary>
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public sealed class PlayTime8b : SaveBlock
    {
        public PlayTime8b(SAV8BS sav, int offset) : base(sav) => Offset = offset;

        public ushort PlayedHours
        {
            get => BitConverter.ToUInt16(Data, Offset);
            set => BitConverter.GetBytes(value).CopyTo(Data, Offset);
        }

        public byte PlayedMinutes { get => Data[Offset + 2]; set => Data[Offset + 2] = value; }
        public byte PlayedSeconds { get => Data[Offset + 3]; set => Data[Offset + 3] = value; }
        public string LastSavedTime => $"{PlayedHours:0000}ː{PlayedMinutes:00}ː{PlayedSeconds:00}"; // not :
    }
}

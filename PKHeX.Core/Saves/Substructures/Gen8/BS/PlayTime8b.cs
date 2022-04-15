using System;
using System.ComponentModel;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core
{
    /// <summary>
    /// Playtime storage
    /// </summary>
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public sealed class PlayTime8b : SaveBlock<SAV8BS>
    {
        public PlayTime8b(SAV8BS sav, int offset) : base(sav) => Offset = offset;

        public ushort PlayedHours
        {
            get => ReadUInt16LittleEndian(Data.AsSpan(Offset));
            set => WriteUInt16LittleEndian(Data.AsSpan(Offset), value);
        }

        public byte PlayedMinutes { get => Data[Offset + 2]; set => Data[Offset + 2] = value; }
        public byte PlayedSeconds { get => Data[Offset + 3]; set => Data[Offset + 3] = value; }
        public string LastSavedTime => $"{PlayedHours:0000}ː{PlayedMinutes:00}ː{PlayedSeconds:00}"; // not :
    }
}

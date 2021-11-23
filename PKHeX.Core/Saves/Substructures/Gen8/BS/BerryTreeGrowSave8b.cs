using System;
using System.ComponentModel;

namespace PKHeX.Core
{
    /// <summary>
    /// Details about berry tree plots.
    /// </summary>
    /// <remarks>size: 0x808</remarks>
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public sealed class BerryTreeGrowSave8b : SaveBlock
    {
        public BerryTreeGrowSave8b(SAV8BS sav, int offset) : base(sav) => Offset = offset;

        public const int KinomiGrowsCount = 128;

        public const int KinomiSize = 0x10;
        // structure:
        // KinomiGrow[] kinomiGrows; // 0x0
        // long LastUpdateMinutes; // 0x8

        public long LastUpdateMinutes { get => BitConverter.ToInt64(Data, Offset + 0x800); set => BitConverter.GetBytes(value).CopyTo(Data, Offset + 0x800); }
    }
}

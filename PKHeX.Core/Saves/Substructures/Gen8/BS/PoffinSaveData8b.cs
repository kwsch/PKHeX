using System;
using System.ComponentModel;

namespace PKHeX.Core
{
    /// <summary>
    /// Details about the Poketch corner app.
    /// </summary>
    /// <remarks>size: 0x644</remarks>
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public sealed class PoffinSaveData8b : SaveBlock
    {
        public PoffinSaveData8b(SAV8BS sav, int offset) : base(sav) => Offset = offset;

        // structure:
        // PoffinData[] Poffins;
        // int CookingCount;

        // 0x640 bytes of data is for poffins
        public const int COUNT_POFFIN = 50;
        public const int SIZE_POFFIN = 0x20;

        public int CookingCount { get => BitConverter.ToInt32(Data, Offset + 0x640); set => BitConverter.GetBytes(value).CopyTo(Data, Offset + 0x640); }
    }
}

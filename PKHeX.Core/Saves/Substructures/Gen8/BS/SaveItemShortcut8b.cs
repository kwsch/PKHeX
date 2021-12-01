using System;
using System.ComponentModel;

namespace PKHeX.Core
{
    /// <summary>
    /// Tracks the 4 select bound item slots. Size: 0x8 (4 * u16)
    /// </summary>
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public sealed class SaveItemShortcut8b : SaveBlock
    {
        public SaveItemShortcut8b(SAV8BS sav, int offset) : base(sav) => Offset = offset;

        public int Item0 { get => BitConverter.ToInt32(Data, Offset + 0x00); set => BitConverter.GetBytes(value).CopyTo(Data, Offset + 0x00); }
        public int Item1 { get => BitConverter.ToInt32(Data, Offset + 0x02); set => BitConverter.GetBytes(value).CopyTo(Data, Offset + 0x02); }
        public int Item2 { get => BitConverter.ToInt32(Data, Offset + 0x04); set => BitConverter.GetBytes(value).CopyTo(Data, Offset + 0x04); }
        public int Item3 { get => BitConverter.ToInt32(Data, Offset + 0x06); set => BitConverter.GetBytes(value).CopyTo(Data, Offset + 0x06); }
    }
}

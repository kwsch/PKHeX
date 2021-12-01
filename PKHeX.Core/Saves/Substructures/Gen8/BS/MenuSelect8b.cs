using System;
using System.ComponentModel;

namespace PKHeX.Core
{
    /// <summary>
    /// Tracks the main menu items. Size: 0x44
    /// </summary>
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public sealed class MenuSelect8b : SaveBlock
    {
        // (TopMenuItemTypeInt32, bool IsNew)[8], TopMenuItemTypeInt32 LastSelected
        private const int COUNT_ITEMS = 8;
        private const int SIZE_TUPLE = 4 + 4; // int,bool32
        public MenuSelect8b(SAV8BS sav, int offset) : base(sav) => Offset = offset;

        public int GetMenuItem(int index)
        {
            int ofs = GetOffset(index);
            return BitConverter.ToInt32(Data, Offset + ofs);
        }

        public void SetMenuItem(int index, int value)
        {
            int ofs = GetOffset(index);
            BitConverter.GetBytes(value).CopyTo(Data, Offset + ofs);
        }

        public bool GetMenuItemIsNew(int index)
        {
            int ofs = GetOffset(index);
            return BitConverter.ToInt32(Data, Offset + ofs + 4) == 1;
        }

        public void SetMenuItemIsNew(int index, bool value)
        {
            int ofs = GetOffset(index);
            BitConverter.GetBytes(value).CopyTo(Data, Offset + ofs + 4);
        }

        private static int GetOffset(int index)
        {
            if ((uint)index >= COUNT_ITEMS)
                throw new ArgumentOutOfRangeException(nameof(index));
            return index * SIZE_TUPLE;
        }

        public int LastSelectedMenu { get => BitConverter.ToInt32(Data, Offset + 0x40); set => BitConverter.GetBytes(value).CopyTo(Data, Offset + 0x40); }
    }
}

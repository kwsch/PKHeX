using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace PKHeX.Core
{
    /// <summary>
    /// Stores 1000 field objects to spawn into the map.
    /// </summary>
    /// <remarks>size: 0x109A0 (1000 * 4*17)</remarks>
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public sealed class FieldObjectSave8b : SaveBlock
    {
        private const int COUNT_OBJECTS = 1_000;

        public FieldObjectSave8b(SAV8BS sav, int offset) : base(sav) => Offset = offset;

#pragma warning disable CA1819 // Properties should not return arrays
        public FieldObject8b[] AllObjects
        {
            get => GetObjects();
            set => SetObjects(value);
        }
#pragma warning restore CA1819 // Properties should not return arrays

        private FieldObject8b[] GetObjects()
        {
            var result = new FieldObject8b[COUNT_OBJECTS];
            for (int i = 0; i < result.Length; i++)
                result[i] = new FieldObject8b(Data, Offset + (i * FieldObject8b.SIZE));
            return result;
        }

        private static void SetObjects(IReadOnlyList<FieldObject8b> value)
        {
            if (value.Count != COUNT_OBJECTS)
                throw new ArgumentOutOfRangeException(nameof(value.Count));
            // data is already hard-referencing the original byte array. This is mostly a hack for Property Grid displays.
        }
    }

    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class FieldObject8b
    {
        public const int SIZE = 4 * 17;

        private readonly byte[] Data = new byte[SIZE];

        public override string ToString() => $"{NameHash:X8} @ ({GridX:000},{GridY:000}) - {(Active ? "✓" : "✕")}";

        public FieldObject8b(byte[] data, int offset)
        {
            data.AsSpan(offset, SIZE).CopyTo(Data);
        }

        public byte Count // cnt
        {
            get => Data[0] ;
            set => Data[0] = value;
        }

        public int NameHash { get => BitConverter.ToInt32(Data, 0x04); set => BitConverter.GetBytes(value).CopyTo(Data, 0x04); }
        public int GridX    { get => BitConverter.ToInt32(Data, 0x08); set => BitConverter.GetBytes(value).CopyTo(Data, 0x08); }
        public int GridY    { get => BitConverter.ToInt32(Data, 0x0C); set => BitConverter.GetBytes(value).CopyTo(Data, 0x0C); }
        public int Height   { get => BitConverter.ToInt32(Data, 0x10); set => BitConverter.GetBytes(value).CopyTo(Data, 0x10); }
        public int Angle    { get => BitConverter.ToInt32(Data, 0x14); set => BitConverter.GetBytes(value).CopyTo(Data, 0x14); }
        public bool Active  { get => BitConverter.ToInt32(Data, 0x18) == 1; set => BitConverter.GetBytes(value ? 1u : 0u).CopyTo(Data, 0x18); }
        public int MoveCode { get => BitConverter.ToInt32(Data, 0x1C); set => BitConverter.GetBytes(value).CopyTo(Data, 0x1C); }
        public int DirHead  { get => BitConverter.ToInt32(Data, 0x20); set => BitConverter.GetBytes(value).CopyTo(Data, 0x20); }
        public int MvParam0 { get => BitConverter.ToInt32(Data, 0x24); set => BitConverter.GetBytes(value).CopyTo(Data, 0x24); }
        public int MvParam1 { get => BitConverter.ToInt32(Data, 0x28); set => BitConverter.GetBytes(value).CopyTo(Data, 0x28); }
        public int MvParam2 { get => BitConverter.ToInt32(Data, 0x2C); set => BitConverter.GetBytes(value).CopyTo(Data, 0x2C); }
        public int LimitX   { get => BitConverter.ToInt32(Data, 0x30); set => BitConverter.GetBytes(value).CopyTo(Data, 0x30); }
        public int LimitZ   { get => BitConverter.ToInt32(Data, 0x34); set => BitConverter.GetBytes(value).CopyTo(Data, 0x34); }
        public int EvType   { get => BitConverter.ToInt32(Data, 0x38); set => BitConverter.GetBytes(value).CopyTo(Data, 0x38); }
        public int MvOldDir { get => BitConverter.ToInt32(Data, 0x3C); set => BitConverter.GetBytes(value).CopyTo(Data, 0x3C); }
        public int MvDir    { get => BitConverter.ToInt32(Data, 0x40); set => BitConverter.GetBytes(value).CopyTo(Data, 0x40); }
    }
}

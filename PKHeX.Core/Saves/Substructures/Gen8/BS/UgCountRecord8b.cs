using System;
using System.ComponentModel;

namespace PKHeX.Core
{
    /// <summary>
    /// Underground player metadata counts.
    /// </summary>
    /// <remarks>size 0x20, struct_name UgCountRecord</remarks>
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public sealed class UgCountRecord8b : SaveBlock
    {
        public UgCountRecord8b(SAV8BS sav, int offset) : base(sav) => Offset = offset;

        public short DigFossilPlayCount
        {
            get => BitConverter.ToInt16(Data, Offset + 0x00);
            set => BitConverter.GetBytes(value).CopyTo(Data, Offset + 0x00);
        }
        public short NumStatueBroadcastOnTV
        {
            get => BitConverter.ToInt16(Data, Offset + 0x02);
            set => BitConverter.GetBytes(value).CopyTo(Data, Offset + 0x02);
        }
        public int NumTimesSecretBaseBroadcastOnTVWereLiked
        {
            get => BitConverter.ToInt32(Data, Offset + 0x04);
            set => BitConverter.GetBytes(value).CopyTo(Data, Offset + 0x04);
        }
        public int SomeoneSecretBaseLikeCount
        {
            get => BitConverter.ToInt32(Data, Offset + 0x08);
            set => BitConverter.GetBytes(value).CopyTo(Data, Offset + 0x08);
        }
        public int NumSuccessfulLightStoneSearches
        {
            get => BitConverter.ToInt32(Data, Offset + 0x0C);
            set => BitConverter.GetBytes(value).CopyTo(Data, Offset + 0x0C);
        }
        public long Reserved1
        {
            get => BitConverter.ToInt64(Data, Offset + 0x10);
            set => BitConverter.GetBytes(value).CopyTo(Data, Offset + 0x10);
        }
        public long Reserved2
        {
            get => BitConverter.ToInt64(Data, Offset + 0x18);
            set => BitConverter.GetBytes(value).CopyTo(Data, Offset + 0x18);
        }
    }
}

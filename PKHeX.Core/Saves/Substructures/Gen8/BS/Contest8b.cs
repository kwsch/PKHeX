using System;
using System.ComponentModel;

namespace PKHeX.Core
{
    /// <summary>
    /// Contest photo and Rank Point storage.
    /// </summary>
    /// <remarks>size 0x720, struct_name CONTEST_DATA</remarks>
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public sealed class Contest8b : SaveBlock
    {
        public const int SIZE_CONTEST_PHOTO = 0x16C;
        public const int PHOTO_MAX = 5;

        public Contest8b(SAV8BS sav, int offset) : base(sav) => Offset = offset;

        private const int OFS_RANK = SIZE_CONTEST_PHOTO * PHOTO_MAX; // 0x71C;

        public uint ContestRankPoint { get => BitConverter.ToUInt32(Data, Offset + OFS_RANK); set => BitConverter.GetBytes(value).CopyTo(Data, Offset + OFS_RANK); }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;

namespace PKHeX.Core
{
    /// <summary>
    /// Footer checksum block layout <see cref="SaveFile"/>, marked with "BEEF" magic number.
    /// </summary>
    /// <remarks>Shared logic is used by Gen6 and Gen7 save files.</remarks>
    public abstract class SAV_BEEF : SaveFile, ISecureValueStorage
    {
        protected SAV_BEEF(byte[] data, int biOffset) : base(data)
        {
            BlockInfoOffset = biOffset;
        }

        protected SAV_BEEF(int size, int biOffset) : base(size)
        {
            BlockInfoOffset = biOffset;
        }

        public abstract IReadOnlyList<BlockInfo> AllBlocks { get; }
        protected override void SetChecksums() => AllBlocks.SetChecksums(Data);
        public override bool ChecksumsValid => AllBlocks.GetChecksumsValid(Data);
        public override string ChecksumInfo => AllBlocks.GetChecksumInfo(Data);
        public override string MiscSaveInfo() => string.Join(Environment.NewLine, AllBlocks.Select(b => b.Summary));

        protected readonly int BlockInfoOffset;

        public ulong TimeStampCurrent
        {
            get => BitConverter.ToUInt64(Data, BlockInfoOffset);
            set => BitConverter.GetBytes(value).CopyTo(Data, BlockInfoOffset);
        }

        public ulong TimeStampPrevious
        {
            get => BitConverter.ToUInt64(Data, BlockInfoOffset);
            set => BitConverter.GetBytes(value).CopyTo(Data, BlockInfoOffset);
        }
    }
}
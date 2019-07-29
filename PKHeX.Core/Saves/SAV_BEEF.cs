using System;
using System.Linq;

namespace PKHeX.Core
{
    /// <summary>
    /// Footer checksum block layout <see cref="SaveFile"/>, marked with "BEEF" magic number.
    /// </summary>
    /// <remarks>Shared logic is used by Gen6 and Gen7 save files.</remarks>
    public abstract class SAV_BEEF : SaveFile, ISecureValueStorage
    {
        protected SAV_BEEF(byte[] data, BlockInfo[] blocks, int biOffset) : base(data)
        {
            Blocks = blocks;
            BlockInfoOffset = biOffset;
        }

        protected SAV_BEEF(int size, BlockInfo[] blocks, int biOffset) : base(size)
        {
            Blocks = blocks;
            BlockInfoOffset = biOffset;
        }

        protected override void SetChecksums() => Blocks.SetChecksums(Data);
        public override bool ChecksumsValid => Blocks.GetChecksumsValid(Data);
        public override string ChecksumInfo => Blocks.GetChecksumInfo(Data);
        public override string MiscSaveInfo() => string.Join(Environment.NewLine, Blocks.Select(b => b.Summary));

        protected readonly int BlockInfoOffset;
        protected readonly BlockInfo[] Blocks;

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
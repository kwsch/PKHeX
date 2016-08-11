namespace PKHeX
{
    public sealed class BlockInfo
    {
        // General
        public int Offset;
        public int Length;

        // Gen6
        public ushort ID;
        public ushort Checksum;
        public BlockInfo() { }

        // Gen4/5
        public readonly int ChecksumOffset;
        public readonly int ChecksumMirror;
        public BlockInfo(int offset, int length, int chkOffset, int chkMirror)
        {
            Offset = offset;
            Length = length;
            ChecksumOffset = chkOffset;
            ChecksumMirror = chkMirror;
        }
    }
}

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

    public class RSBOX_Block
    {
        private ushort CHK_0;
        private ushort CHK_1;

        public readonly uint BlockNumber;
        public readonly uint SaveCount;
        public readonly byte[] Data;

        public readonly int Offset;

        public RSBOX_Block(byte[] data, int offset)
        {
            Data = (byte[])data.Clone();
            Offset = offset;
            // Values stored in Big Endian format
            CHK_0 =         (ushort)((Data[0x0] << 8) | (Data[0x1] << 0));
            CHK_1 =         (ushort)((Data[0x2] << 8) | (Data[0x3] << 0));
            BlockNumber =     (uint)((Data[0x4] << 8) | (Data[0x5] << 8) | (Data[0x6] << 8) | (Data[0x7] << 0));
            SaveCount =       (uint)((Data[0x8] << 8) | (Data[0x9] << 8) | (Data[0xA] << 8) | (Data[0xB] << 0));
        }

        public bool ChecksumsValid
        {
            get
            {
                ushort[] chks = getCHK(Data);
                return chks[0] == CHK_0 && chks[1] == CHK_1;
            }
        }
        public void SetChecksums()
        {
            ushort[] chks = getCHK(Data);
            CHK_0 = chks[0];
            CHK_1 = chks[1];
            Data[0] = (byte)(CHK_0 >> 8);
            Data[1] = (byte)(CHK_0 & 0xFF);
            Data[2] = (byte)(CHK_1 >> 8);
            Data[3] = (byte)(CHK_1 & 0xFF);
        }

        private static ushort[] getCHK(byte[] data)
        {
            int chk = 0; // initial value
            for (int j = 0x4; j < 0x1FFC; j += 2)
            {
                chk += data[j] << 8;
                chk += data[j + 1];
            }
            ushort chk0 = (ushort)chk;
            ushort chk1 = (ushort)(0xF004 - chk0);

            return new[] { chk0, chk1 };
        }
    }
}

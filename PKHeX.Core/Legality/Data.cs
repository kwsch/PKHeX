using System;

namespace PKHeX.Core
{
    public static class Data
    {
        public static byte[][] UnpackMini(byte[] fileData, string identifier)
        {
            if (fileData == null || fileData.Length < 4)
                return null;

            if (identifier[0] != fileData[0] || identifier[1] != fileData[1])
                return null;
            
            int count = BitConverter.ToUInt16(fileData, 2); int ctr = 4;
            int start = BitConverter.ToInt32(fileData, ctr); ctr += 4;
            byte[][] returnData = new byte[count][];
            for (int i = 0; i < count; i++)
            {
                int end = BitConverter.ToInt32(fileData, ctr); ctr += 4;
                int len = end - start;
                byte[] data = new byte[len];
                Buffer.BlockCopy(fileData, start, data, 0, len);
                returnData[i] = data;
                start = end;
            }
            return returnData;
        }
    }
}

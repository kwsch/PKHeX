using System;

namespace PKHeX.Core
{
    public static class BinLinker
    {
        /// <summary>
        /// Unpacks a BinLinkerAccessor generated file container into individual arrays.
        /// </summary>
        /// <param name="fileData">Packed data</param>
        /// <param name="identifier">Signature expected in the first two bytes (ASCII)</param>
        /// <returns>Unpacked array containing all files that were packed.</returns>
        public static byte[][] Unpack(byte[] fileData, string identifier)
        {
            if (fileData.Length < 4)
                throw new ArgumentException(nameof(fileData));

            if (identifier[0] != fileData[0] || identifier[1] != fileData[1])
                throw new ArgumentException(nameof(identifier));

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

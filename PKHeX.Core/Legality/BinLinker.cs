using System;
using System.Runtime.InteropServices;

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
        public static byte[][] Unpack(ReadOnlySpan<byte> fileData, string identifier)
        {
#if DEBUG
            System.Diagnostics.Debug.Assert(fileData.Length > 4);
            System.Diagnostics.Debug.Assert(identifier[0] == fileData[0] && identifier[1] == fileData[1]);
#endif
            MemoryMarshal.TryRead(fileData[4..], out int start);
            MemoryMarshal.TryRead(fileData[2..], out ushort count);
            var offsetBytes = fileData[8..(8 + (count * sizeof(int)))];
            var offsets = MemoryMarshal.Cast<byte, int>(offsetBytes);

            byte[][] returnData = new byte[count][];
            for (int i = 0; i < offsets.Length; i++)
            {
                int end = offsets[i];
                returnData[i] = fileData[start..end].ToArray();
                start = end;
            }
            return returnData;
        }
    }
}

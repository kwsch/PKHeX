using System;
using System.IO;

namespace PKHeX
{
    internal static class Data
    {
        internal static byte[][] unpackMini(byte[] fileData, string identifier)
        {
            if (fileData == null || fileData.Length < 4)
                return null;

            using (var s = new MemoryStream(fileData))
            using (var br = new BinaryReader(s))
            {
                if (identifier != new string(br.ReadChars(2)))
                    return null;

                ushort count = br.ReadUInt16();
                byte[][] returnData = new byte[count][];

                uint[] offsets = new uint[count + 1];
                for (int i = 0; i < count; i++)
                    offsets[i] = br.ReadUInt32();

                uint length = br.ReadUInt32();
                offsets[offsets.Length - 1] = length;

                for (int i = 0; i < count; i++)
                {
                    br.BaseStream.Seek(offsets[i], SeekOrigin.Begin);
                    using (MemoryStream dataout = new MemoryStream())
                    {
                        byte[] data = new byte[0];
                        s.CopyTo(dataout, (int)offsets[i]);
                        int len = (int)offsets[i + 1] - (int)offsets[i];

                        if (len != 0)
                        {
                            data = dataout.ToArray();
                            Array.Resize(ref data, len);
                        }
                        returnData[i] = data;
                    }
                }
                return returnData;
            }
        }
    }
}

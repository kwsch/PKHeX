#if DEBUG
using System;
using System.IO;

namespace PKHeX.Core;

/// <summary>
/// Utility class for writing BinLinker files.
/// </summary>
public static class BinLinkerWriter
{
    /// <inheritdoc cref="Compress(BinLinkerAccessor, Func{byte[], byte[]}, int)"/>
    public static byte[] Compress<T>(T[] arr, ReadOnlySpan<byte> ident, Func<T, byte[]> sel, int padTo = 0)
    {
        // Convert th
        var result = new byte[arr.Length][];
        for (int i = 0; i < arr.Length; i++)
            result[i] = sel(arr[i]);
        return Write16(result, ident, padTo);
    }

    /// <summary>
    /// Compresses a <see cref="BinLinkerAccessor"/> into a <see cref="BinLinkerAccessor16"/>
    /// </summary>
    /// <param name="arr">Container to compress</param>
    /// <param name="convert">Optional conversion function to apply to each file</param>
    /// <param name="padTo">Per-file padding size</param>
    /// <returns>Newly compressed <see cref="BinLinkerAccessor16"/></returns>
    public static byte[] Compress(BinLinkerAccessor arr, Func<byte[], byte[]>? convert = null, int padTo = 0)
    {
        var ident = arr.IdentifierSpan; // use existing identifier
        var temp = arr.Unpack();
        return Compress(temp, ident, convert ?? (z => z), padTo);
    }

    /// <summary>
    /// Unpacks a <see cref="BinLinkerAccessor"/> into individual file arrays.
    /// </summary>
    public static byte[][] Unpack(this BinLinkerAccessor arr)
    {
        var temp = new byte[arr.Length][];
        for (int i = 0; i < temp.Length; i++)
            temp[i] = arr[i].ToArray();
        return temp;
    }

    /// <inheritdoc cref="Unpack(BinLinkerAccessor)"/>
    public static byte[][] Unpack(this BinLinkerAccessor16 arr)
    {
        var temp = new byte[arr.Length][];
        for (int i = 0; i < temp.Length; i++)
            temp[i] = arr[i].ToArray();
        return temp;
    }

    /// <summary>
    /// Writes a <see cref="BinLinkerAccessor"/> to a new file with the given identifier.
    /// </summary>
    /// <param name="data">Data to write</param>
    /// <param name="identifier">Identifier for the file</param>
    /// <param name="padTo">Padding size for each file</param>
    /// <returns>Writeable byte array</returns>
    public static byte[] Write32(byte[][] data, ReadOnlySpan<byte> identifier, int padTo = 4)
    {
        using var ms = new MemoryStream(4096);
        using var bw = new BinaryWriter(ms);

        bw.Write(identifier[..2]);
        bw.Write((ushort)data.Length);
        const int start = 0;

        // Preallocate the offset map
        int count = data.Length;
        int dataOffset = 4 + ((count + 1) * sizeof(uint));
        for (int i = 0; i < count; i++)
            bw.Write((uint)0);
        bw.Write((uint)0);

        // Write each file, then update the offset map
        for (int i = 0; i < count; i++)
        {
            // Write File Offset
            var fileOffset = bw.BaseStream.Length - start;
            bw.Seek(start + 4 + (i * sizeof(uint)), SeekOrigin.Begin);
            bw.Write((uint)fileOffset);
            // Write File to Stream
            bw.Seek(0, SeekOrigin.End);
            bw.Write(data[i]);
            if (padTo != 0)
            {
                while ((ms.Position - start) % padTo != 0)
                    bw.Write((byte)0);
            }
        }

        // Cap the File
        {
            var fileOffset = bw.BaseStream.Length - start;
            bw.Seek(start + 4 + (count * sizeof(uint)), SeekOrigin.Begin);
            bw.Write((uint)fileOffset);
        }

        // Return the byte array
        return ms.ToArray();
    }

    /// <inheritdoc cref="Write32(byte[][], ReadOnlySpan{byte}, int)"/>
    public static byte[] Write16(byte[][] data, ReadOnlySpan<byte> identifier, int padTo = 2)
    {
        using var ms = new MemoryStream(4096);
        using var bw = new BinaryWriter(ms);
        const int start = 0;

        bw.Write(identifier[..2]);
        bw.Write((ushort)data.Length);

        // Preallocate the offset map
        int count = data.Length;
        int dataOffset = 4 + ((count + 1) * sizeof(ushort));
        for (int i = 0; i < count; i++)
            bw.Write((ushort)0);
        bw.Write((ushort)0);

        // Write each file, then update the offset map
        for (int i = 0; i < count; i++)
        {
            // Write File Offset
            var fileOffset = bw.BaseStream.Length - start;
            bw.Seek(start + 4 + (i * sizeof(ushort)), SeekOrigin.Begin);
            bw.Write((ushort)fileOffset);
            // Write File to Stream
            bw.Seek(0, SeekOrigin.End);
            bw.Write(data[i]);
            if (padTo != 0)
            {
                while ((ms.Position - start) % padTo != 0)
                    bw.Write((byte)0);
            }
        }

        // Cap the File
        {
            var fileOffset = bw.BaseStream.Length - start;
            bw.Seek(start + 4 + (count * sizeof(ushort)), SeekOrigin.Begin);
            bw.Write((ushort)fileOffset);
        }

        // Return the byte array
        return ms.ToArray();
    }
}
#endif

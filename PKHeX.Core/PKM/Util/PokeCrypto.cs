using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

/// <summary>
/// Logic related to Encrypting and Decrypting Pokémon entity data.
/// </summary>
public static class PokeCrypto
{
    internal const int SIZE_1ULIST = 69;
    internal const int SIZE_1JLIST = 59;
    internal const int SIZE_1PARTY = 44;
    internal const int SIZE_1STORED = 33;

    internal const int SIZE_2ULIST = 73;
    internal const int SIZE_2JLIST = 63;
    internal const int SIZE_2PARTY = 48;
    internal const int SIZE_2STORED = 32;
    internal const int SIZE_2STADIUM = 60;

    internal const int SIZE_3CSTORED = 312;
    internal const int SIZE_3XSTORED = 196;
    internal const int SIZE_3PARTY = 100;
    internal const int SIZE_3STORED = 80;
    private const int SIZE_3HEADER = 32;
    private const int SIZE_3BLOCK = 12;

    internal const int SIZE_4PARTY = 236;
    internal const int SIZE_4STORED = 136;
    internal const int SIZE_4RSTORED = 164; // 4STORED + 0x1C bytes of extra data
    private const int SIZE_4BLOCK = 32;

    internal const int SIZE_5PARTY = 220;
    internal const int SIZE_5STORED = 136;
    //private const int SIZE_5BLOCK = SIZE_4BLOCK;

    internal const int SIZE_6PARTY = 0x104;
    internal const int SIZE_6STORED = 0xE8;
    private const int SIZE_6BLOCK = 56;

    // Gen7 Format is the same size as Gen6.

    internal const int SIZE_8STORED = 8 + (4 * SIZE_8BLOCK); // 0x148
    internal const int SIZE_8PARTY = SIZE_8STORED + 0x10; // 0x158
    private const int SIZE_8BLOCK = 80; // 0x50

    internal const int SIZE_8ASTORED = 8 + (4 * SIZE_8ABLOCK); // 0x168
    internal const int SIZE_8APARTY = SIZE_8ASTORED + 0x10; // 0x178
    private const int SIZE_8ABLOCK = 88; // 0x58

    internal const int SIZE_9STORED = SIZE_8STORED;
    internal const int SIZE_9PARTY = SIZE_8PARTY;
    private const int SIZE_9BLOCK = SIZE_8BLOCK;

    /// <summary>
    /// Positions for shuffling.
    /// </summary>
    private static ReadOnlySpan<byte> BlockPosition =>
    [
        0, 1, 2, 3,
        0, 1, 3, 2,
        0, 2, 1, 3,
        0, 3, 1, 2,
        0, 2, 3, 1,
        0, 3, 2, 1,
        1, 0, 2, 3,
        1, 0, 3, 2,
        2, 0, 1, 3,
        3, 0, 1, 2,
        2, 0, 3, 1,
        3, 0, 2, 1,
        1, 2, 0, 3,
        1, 3, 0, 2,
        2, 1, 0, 3,
        3, 1, 0, 2,
        2, 3, 0, 1,
        3, 2, 0, 1,
        1, 2, 3, 0,
        1, 3, 2, 0,
        2, 1, 3, 0,
        3, 1, 2, 0,
        2, 3, 1, 0,
        3, 2, 1, 0,

        // duplicates of 0-7 to eliminate modulus
        0, 1, 2, 3,
        0, 1, 3, 2,
        0, 2, 1, 3,
        0, 3, 1, 2,
        0, 2, 3, 1,
        0, 3, 2, 1,
        1, 0, 2, 3,
        1, 0, 3, 2,
    ];

    /// <summary>
    /// Positions for un-shuffling.
    /// </summary>
    private static ReadOnlySpan<byte> BlockPositionInvert =>
    [
        0, 1, 2, 4, 3, 5, 6, 7, 12, 18, 13, 19, 8, 10, 14, 20, 16, 22, 9, 11, 15, 21, 17, 23,
        0, 1, 2, 4, 3, 5, 6, 7, // duplicates of 0-7 to eliminate modulus
    ];

    /// <summary>
    /// Shuffles a 232 byte array containing Pokémon data.
    /// </summary>
    /// <param name="data">Data to shuffle</param>
    /// <param name="sv">Block Shuffle order</param>
    /// <param name="blockSize">Size of shuffling chunks</param>
    /// <returns>Shuffled byte array</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static byte[] ShuffleArray(ReadOnlySpan<byte> data, uint sv, [ConstantExpected(Min = 0)] int blockSize)
    {
        byte[] sdata = new byte[data.Length];
        ShuffleArray(data, sdata, sv, blockSize);
        return sdata;
    }

    private static void ShuffleArray(ReadOnlySpan<byte> data, Span<byte> result, uint sv, [ConstantExpected(Min = 0)] int blockSize)
    {
        int index = (int)sv * 4;
        const int start = 8;
        data[..start].CopyTo(result[..start]);
        var end = start + (blockSize * 4);
        data[end..].CopyTo(result[end..]);
        for (int block = 3; block >= 0; block--)
        {
            var dest = result.Slice(start + (blockSize * block), blockSize);
            int ofs = BlockPosition[index + block];
            var src = data.Slice(start + (blockSize * ofs), blockSize);
            src.CopyTo(dest);
        }
    }

    /// <summary>
    /// Decrypts a Gen8 pk byte array.
    /// </summary>
    /// <param name="ekm">Encrypted Pokémon data.</param>
    /// <returns>Decrypted Pokémon data.</returns>
    /// <returns>Encrypted Pokémon data.</returns>
    public static byte[] DecryptArray8(Span<byte> ekm)
    {
        uint pv = ReadUInt32LittleEndian(ekm);
        uint sv = (pv >> 13) & 31;

        CryptPKM(ekm, pv, SIZE_8BLOCK);
        return ShuffleArray(ekm, sv, SIZE_8BLOCK);
    }

    /// <summary>
    /// Decrypts a Gen8 pk byte array.
    /// </summary>
    /// <param name="ekm">Encrypted Pokémon data.</param>
    /// <returns>Decrypted Pokémon data.</returns>
    /// <returns>Encrypted Pokémon data.</returns>
    public static byte[] DecryptArray8A(Span<byte> ekm)
    {
        uint pv = ReadUInt32LittleEndian(ekm);
        uint sv = (pv >> 13) & 31;

        CryptPKM(ekm, pv, SIZE_8ABLOCK);
        return ShuffleArray(ekm, sv, SIZE_8ABLOCK);
    }

    /// <summary>
    /// Decrypts a Gen9 pk byte array.
    /// </summary>
    /// <param name="ekm">Encrypted Pokémon data.</param>
    /// <returns>Decrypted Pokémon data.</returns>
    /// <returns>Encrypted Pokémon data.</returns>
    public static byte[] DecryptArray9(Span<byte> ekm)
    {
        uint pv = ReadUInt32LittleEndian(ekm);
        uint sv = (pv >> 13) & 31;

        CryptPKM(ekm, pv, SIZE_9BLOCK);
        return ShuffleArray(ekm, sv, SIZE_9BLOCK);
    }

    /// <summary>
    /// Encrypts a Gen8 pk byte array.
    /// </summary>
    /// <param name="pk">Decrypted Pokémon data.</param>
    public static byte[] EncryptArray8(ReadOnlySpan<byte> pk)
    {
        uint pv = ReadUInt32LittleEndian(pk);
        uint sv = (pv >> 13) & 31;

        byte[] ekm = ShuffleArray(pk, BlockPositionInvert[(int)sv], SIZE_8BLOCK);
        CryptPKM(ekm, pv, SIZE_8BLOCK);
        return ekm;
    }

    /// <summary>
    /// Encrypts a Gen8 pk byte array.
    /// </summary>
    /// <param name="pk">Decrypted Pokémon data.</param>
    public static byte[] EncryptArray8A(ReadOnlySpan<byte> pk)
    {
        uint pv = ReadUInt32LittleEndian(pk);
        uint sv = (pv >> 13) & 31;

        byte[] ekm = ShuffleArray(pk, BlockPositionInvert[(int)sv], SIZE_8ABLOCK);
        CryptPKM(ekm, pv, SIZE_8ABLOCK);
        return ekm;
    }

    /// <summary>
    /// Encrypts a Gen9 pk byte array.
    /// </summary>
    /// <param name="pk">Decrypted Pokémon data.</param>
    public static byte[] EncryptArray9(ReadOnlySpan<byte> pk)
    {
        uint pv = ReadUInt32LittleEndian(pk);
        uint sv = (pv >> 13) & 31;

        byte[] ekm = ShuffleArray(pk, BlockPositionInvert[(int)sv], SIZE_9BLOCK);
        CryptPKM(ekm, pv, SIZE_9BLOCK);
        return ekm;
    }

    /// <summary>
    /// Decrypts a 232 byte + party stat byte array.
    /// </summary>
    /// <param name="ekm">Encrypted Pokémon data.</param>
    /// <returns>Decrypted Pokémon data.</returns>
    /// <returns>Encrypted Pokémon data.</returns>
    public static byte[] DecryptArray6(Span<byte> ekm)
    {
        uint pv = ReadUInt32LittleEndian(ekm);
        uint sv = (pv >> 13) & 31;

        CryptPKM(ekm, pv, SIZE_6BLOCK);
        return ShuffleArray(ekm, sv, SIZE_6BLOCK);
    }

    /// <summary>
    /// Encrypts a 232 byte + party stat byte array.
    /// </summary>
    /// <param name="pk">Decrypted Pokémon data.</param>
    public static byte[] EncryptArray6(ReadOnlySpan<byte> pk)
    {
        uint pv = ReadUInt32LittleEndian(pk);
        uint sv = (pv >> 13) & 31;

        byte[] ekm = ShuffleArray(pk, BlockPositionInvert[(int)sv], SIZE_6BLOCK);
        CryptPKM(ekm, pv, SIZE_6BLOCK);
        return ekm;
    }

    /// <summary>
    /// Decrypts a 136 byte + party stat byte array.
    /// </summary>
    /// <param name="ekm">Encrypted Pokémon data.</param>
    /// <returns>Decrypted Pokémon data.</returns>
    public static byte[] DecryptArray45(Span<byte> ekm)
    {
        uint pv = ReadUInt32LittleEndian(ekm);
        uint chk = ReadUInt16LittleEndian(ekm[6..]);
        uint sv = (pv >> 13) & 31;

        CryptPKM45(ekm, pv, chk, SIZE_4BLOCK);
        return ShuffleArray(ekm, sv, SIZE_4BLOCK);
    }

    /// <summary>
    /// Encrypts a 136 byte + party stat byte array.
    /// </summary>
    /// <param name="pk">Decrypted Pokémon data.</param>
    /// <returns>Encrypted Pokémon data.</returns>
    public static byte[] EncryptArray45(ReadOnlySpan<byte> pk)
    {
        uint pv = ReadUInt32LittleEndian(pk);
        uint chk = ReadUInt16LittleEndian(pk[6..]);
        uint sv = (pv >> 13) & 31;

        byte[] ekm = ShuffleArray(pk, BlockPositionInvert[(int)sv], SIZE_4BLOCK);
        CryptPKM45(ekm, pv, chk, SIZE_4BLOCK);
        return ekm;
    }

    /// <summary>
    /// Decrypts a 136 byte + party stat byte array.
    /// </summary>
    /// <param name="ekm">Encrypted Pokémon data.</param>
    /// <returns>Decrypted Pokémon data.</returns>
    public static byte[] DecryptArray4BE(ReadOnlySpan<byte> ekm)
    {
        uint pv = ReadUInt32BigEndian(ekm);
        uint sv = (pv >> 13) & 31;
        return ShuffleArray(ekm, sv, SIZE_4BLOCK);
    }

    /// <summary>
    /// Encrypts a 136 byte + party stat byte array.
    /// </summary>
    /// <param name="pk">Decrypted Pokémon data.</param>
    /// <returns>Encrypted Pokémon data.</returns>
    public static byte[] EncryptArray4BE(ReadOnlySpan<byte> pk)
    {
        uint pv = ReadUInt32BigEndian(pk);
        uint sv = (pv >> 13) & 31;
        return ShuffleArray(pk, BlockPositionInvert[(int)sv], SIZE_4BLOCK);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void CryptPKM(Span<byte> data, uint pv, [ConstantExpected(Min = 0)] int blockSize)
    {
        const int start = 8;
        int end = (4 * blockSize) + start;
        CryptArray(data[start..end], pv); // Blocks
        if (data.Length > end)
            CryptArray(data[end..], pv); // Party Stats
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void CryptPKM45(Span<byte> data, uint pv, uint chk, [ConstantExpected(Min = 0)] int blockSize)
    {
        const int start = 8;
        int end = (4 * blockSize) + start;
        CryptArray(data[start..end], chk); // Blocks
        if (data.Length > end)
            CryptArray(data[end..], pv); // Party Stats
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void CryptArray(Span<byte> data, uint seed)
    {
        foreach (ref var u32 in MemoryMarshal.Cast<byte, ushort>(data))
        {
            seed = (0x41C64E6D * seed) + 0x00006073;
            var xor = (ushort)(seed >> 16);
            if (!BitConverter.IsLittleEndian)
                xor = ReverseEndianness(xor);
            u32 ^= xor;
        }
    }

    /// <summary>
    /// Decrypts an 80 byte format Generation 3 Pokémon byte array.
    /// </summary>
    /// <param name="ekm">Encrypted data.</param>
    /// <returns>Decrypted data.</returns>
    public static byte[] DecryptArray3(Span<byte> ekm)
    {
        Debug.Assert(ekm.Length is SIZE_3PARTY or SIZE_3STORED);

        uint PID = ReadUInt32LittleEndian(ekm);
        uint OID = ReadUInt32LittleEndian(ekm[4..]);
        uint seed = PID ^ OID;
        CryptArray3(ekm, seed);
        return ShuffleArray3(ekm, PID % 24);
    }

    private static void CryptArray3(Span<byte> ekm, uint seed)
    {
        if (!BitConverter.IsLittleEndian)
            seed = ReverseEndianness(seed);
        var toEncrypt = ekm[SIZE_3HEADER..SIZE_3STORED];
        foreach (ref var u32 in MemoryMarshal.Cast<byte, uint>(toEncrypt))
            u32 ^= seed;
    }

    /// <summary>
    /// Shuffles an 80 byte format Generation 3 Pokémon byte array.
    /// </summary>
    /// <param name="data">Un-shuffled data.</param>
    /// <param name="sv">Block order shuffle value</param>
    /// <returns>Un-shuffled  data.</returns>
    private static byte[] ShuffleArray3(ReadOnlySpan<byte> data, uint sv)
    {
        byte[] sdata = new byte[data.Length];
        ShuffleArray3(data, sdata, sv);
        return sdata;
    }

    private static void ShuffleArray3(ReadOnlySpan<byte> data, Span<byte> result, uint sv)
    {
        int index = (int)sv * 4;
        data[..SIZE_3HEADER].CopyTo(result[..SIZE_3HEADER]);
        data[SIZE_3STORED..].CopyTo(result[SIZE_3STORED..]);
        for (int block = 3; block >= 0; block--)
        {
            var dest = result.Slice(SIZE_3HEADER + (SIZE_3BLOCK * block), SIZE_3BLOCK);
            int ofs = BlockPosition[index + block];
            var src = data.Slice(SIZE_3HEADER + (SIZE_3BLOCK * ofs), SIZE_3BLOCK);
            src.CopyTo(dest);
        }
    }

    /// <summary>
    /// Encrypts an 80 byte format Generation 3 Pokémon byte array.
    /// </summary>
    /// <param name="pk">Decrypted data.</param>
    /// <returns>Encrypted data.</returns>
    public static byte[] EncryptArray3(ReadOnlySpan<byte> pk)
    {
        Debug.Assert(pk.Length is SIZE_3PARTY or SIZE_3STORED);

        uint PID = ReadUInt32LittleEndian(pk);
        uint OID = ReadUInt32LittleEndian(pk[4..]);
        uint seed = PID ^ OID;

        byte[] ekm = ShuffleArray3(pk, BlockPositionInvert[(int)(PID % 24)]);
        CryptArray3(ekm, seed);
        return ekm;
    }

    /// <summary>
    /// Decrypts the input <see cref="pk"/> data into a new array if it is encrypted, and updates the reference.
    /// </summary>
    /// <remarks>Generation 3 Format encryption check which verifies the checksum</remarks>
    public static void DecryptIfEncrypted3(ref byte[] pk)
    {
        ushort chk = Checksums.Add16(pk.AsSpan(0x20, 4 * SIZE_3BLOCK));
        if (chk != ReadUInt16LittleEndian(pk.AsSpan(0x1C)))
            pk = DecryptArray3(pk);
    }

    /// <summary>
    /// Decrypts the input <see cref="pk"/> data into a new array if it is encrypted, and updates the reference.
    /// </summary>
    /// <remarks>Generation 4 &amp; 5 Format encryption check which checks for the unused bytes</remarks>
    public static void DecryptIfEncrypted45(ref byte[] pk)
    {
        var span = pk.AsSpan();
        if (ReadUInt32LittleEndian(span[0x64..]) != 0)
            pk = DecryptArray45(span);
    }

    /// <summary>
    /// Decrypts the input <see cref="pk"/> data into a new array if it is encrypted, and updates the reference.
    /// </summary>
    /// <remarks>Generation 6 &amp; 7 Format encryption check</remarks>
    public static void DecryptIfEncrypted67(ref byte[] pk)
    {
        var span = pk.AsSpan();
        if (ReadUInt16LittleEndian(span[0xC8..]) != 0 || ReadUInt16LittleEndian(span[0x58..]) != 0)
            pk = DecryptArray6(span);
    }

    /// <summary>
    /// Decrypts the input <see cref="pk"/> data into a new array if it is encrypted, and updates the reference.
    /// </summary>
    /// <remarks>Generation 8 Format encryption check</remarks>
    public static void DecryptIfEncrypted8(ref byte[] pk)
    {
        var span = pk.AsSpan();
        if (ReadUInt16LittleEndian(span[0x70..]) != 0 || ReadUInt16LittleEndian(span[0x110..]) != 0)
            pk = DecryptArray8(span);
    }

    /// <summary>
    /// Decrypts the input <see cref="pk"/> data into a new array if it is encrypted, and updates the reference.
    /// </summary>
    /// <remarks>Generation 8 Format encryption check</remarks>
    public static void DecryptIfEncrypted8A(ref byte[] pk)
    {
        var span = pk.AsSpan();
        if (ReadUInt16LittleEndian(span[0x78..]) != 0 || ReadUInt16LittleEndian(span[0x128..]) != 0)
            pk = DecryptArray8A(span);
    }

    /// <summary>
    /// Decrypts the input <see cref="pk"/> data into a new array if it is encrypted, and updates the reference.
    /// </summary>
    /// <remarks>Generation 9 Format encryption check</remarks>
    public static void DecryptIfEncrypted9(ref byte[] pk)
    {
        var span = pk.AsSpan();
        if (ReadUInt16LittleEndian(span[0x70..]) != 0 || ReadUInt16LittleEndian(span[0x110..]) != 0)
            pk = DecryptArray9(span);
    }
}

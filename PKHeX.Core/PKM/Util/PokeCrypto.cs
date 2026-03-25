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

    internal const int SIZE_8STORED = 8 + (BlockCount * SIZE_8BLOCK); // 0x148
    internal const int SIZE_8PARTY = SIZE_8STORED + 0x10; // 0x158
    private const int SIZE_8BLOCK = 80; // 0x50

    internal const int SIZE_8ASTORED = 8 + (BlockCount * SIZE_8ABLOCK); // 0x168
    internal const int SIZE_8APARTY = SIZE_8ASTORED + 0x10; // 0x178
    private const int SIZE_8ABLOCK = 88; // 0x58

    private const int BlockCount = 4;

    /// <summary>
    /// Positions for shuffling.
    /// </summary>
    private static ReadOnlySpan<byte> BlockPosition =>
    [
        0, 1, 2, 3,  0, 1, 3, 2,  0, 2, 1, 3,  0, 3, 1, 2,
        0, 2, 3, 1,  0, 3, 2, 1,  1, 0, 2, 3,  1, 0, 3, 2,
        2, 0, 1, 3,  3, 0, 1, 2,  2, 0, 3, 1,  3, 0, 2, 1,
        1, 2, 0, 3,  1, 3, 0, 2,  2, 1, 0, 3,  3, 1, 0, 2,
        2, 3, 0, 1,  3, 2, 0, 1,  1, 2, 3, 0,  1, 3, 2, 0,
        2, 1, 3, 0,  3, 1, 2, 0,  2, 3, 1, 0,  3, 2, 1, 0,

        // duplicates of 0-7 to eliminate modulus (32 => 24)
        0, 1, 2, 3,  0, 1, 3, 2,  0, 2, 1, 3,  0, 3, 1, 2,
        0, 2, 3, 1,  0, 3, 2, 1,  1, 0, 2, 3,  1, 0, 3, 2,
    ];

    /// <summary>
    /// Positions for un-shuffling.
    /// </summary>
    private static ReadOnlySpan<byte> BlockPositionInvert =>
    [
        00, 01, 02, 04,
        03, 05, 06, 07,
        12, 18, 13, 19,
        08, 10, 14, 20,
        16, 22, 09, 11,
        15, 21, 17, 23,

        // duplicates of 0-7 to eliminate modulus (32 => 24)
        00, 01, 02, 04,
        03, 05, 06, 07,
    ];

    /// <summary>
    /// Decrypts an 80 byte format Generation 3 Pokémon byte array.
    /// </summary>
    /// <param name="data">Encrypted data.</param>
    /// <returns>Decrypted data.</returns>
    public static void Decrypt3(Span<byte> data)
    {
        uint PID = ReadUInt32LittleEndian(data);
        uint OID = ReadUInt32LittleEndian(data[4..]);
        uint seed = PID ^ OID;
        uint sv = PID % 24;

        var shuffle = data[SIZE_3HEADER..SIZE_3STORED];
        CryptArray3(shuffle, seed);
        Shuffle3(shuffle, sv);
    }

    /// <summary>
    /// Encrypts an 80 byte format Generation 3 Pokémon byte array.
    /// </summary>
    /// <param name="data">Decrypted data.</param>
    /// <returns>Encrypted data.</returns>
    public static void Encrypt3(Span<byte> data)
    {
        uint PID = ReadUInt32LittleEndian(data);
        uint OID = ReadUInt32LittleEndian(data[4..]);
        uint seed = PID ^ OID;
        uint sv = PID % 24;
        sv = BlockPositionInvert[(int)sv];

        var shuffle = data[SIZE_3HEADER..SIZE_3STORED];
        Shuffle3(shuffle, sv);
        CryptArray3(shuffle, seed);
    }

    /// <summary>
    /// Decrypts a 136 byte array from Battle Revolution (Gen4).
    /// </summary>
    /// <param name="data">Encrypted Pokémon data.</param>
    /// <returns>Decrypted Pokémon data.</returns>
    public static void Decrypt4BE(Span<byte> data)
    {
        Debug.Assert(data.Length is SIZE_4STORED);
        uint pv = ReadUInt32BigEndian(data);
        uint sv = (pv >> 13) & 31;

        var shuffle = data[8..SIZE_4STORED];
        // No encryption applied at rest.
        Shuffle45(shuffle, sv);
    }

    /// <summary>
    /// Encrypts a 136 byte array from Battle Revolution (Gen4).
    /// </summary>
    /// <param name="data">Decrypted Pokémon data.</param>
    /// <returns>Encrypted Pokémon data.</returns>
    public static void Encrypt4BE(Span<byte> data)
    {
        Debug.Assert(data.Length is SIZE_4STORED);
        uint pv = ReadUInt32BigEndian(data);
        uint sv = (pv >> 13) & 31;
        sv = BlockPositionInvert[(int)sv];

        var shuffle = data[8..SIZE_4STORED];
        // No encryption applied at rest.
        Shuffle45(shuffle, sv);
    }

    /// <summary>
    /// Decrypts a 136 byte + party stat byte array.
    /// </summary>
    /// <param name="data">Encrypted Pokémon data.</param>
    /// <returns>Decrypted Pokémon data.</returns>
    public static void Decrypt45(Span<byte> data)
    {
        Debug.Assert(data.Length is SIZE_4STORED or SIZE_4PARTY or SIZE_5PARTY);

        uint pv = ReadUInt32LittleEndian(data);
        uint chk = ReadUInt16LittleEndian(data[6..]);
        uint sv = (pv >> 13) & 31;

        var shuffle = data[8..SIZE_4STORED];
        CryptArray(shuffle, chk);
        if (data.Length > SIZE_4STORED) // Party Stats
            CryptArray(data[SIZE_4STORED..], pv);
        Shuffle45(shuffle, sv);
    }

    /// <summary>
    /// Encrypts a 136 byte + party stat byte array.
    /// </summary>
    /// <param name="data">Decrypted Pokémon data.</param>
    /// <returns>Encrypted Pokémon data.</returns>
    public static void Encrypt45(Span<byte> data)
    {
        Debug.Assert(data.Length is SIZE_4STORED or SIZE_4PARTY or SIZE_5PARTY);

        uint pv = ReadUInt32LittleEndian(data);
        uint chk = ReadUInt16LittleEndian(data[6..]);
        uint sv = (pv >> 13) & 31;
        sv = BlockPositionInvert[(int)sv];

        var shuffle = data[8..SIZE_4STORED];
        Shuffle45(shuffle, sv);
        CryptArray(shuffle, chk);
        if (data.Length > SIZE_4STORED) // Party Stats
            CryptArray(data[SIZE_4STORED..], pv);
    }

    /// <summary>
    /// Decrypts a 232 byte + party stat byte array.
    /// </summary>
    /// <param name="data">Encrypted Pokémon data.</param>
    /// <returns>Decrypted Pokémon data.</returns>
    public static void Decrypt67(Span<byte> data)
    {
        Debug.Assert(data.Length is SIZE_6STORED or SIZE_6PARTY);

        uint pv = ReadUInt32LittleEndian(data);
        uint sv = (pv >> 13) & 31;

        var shuffle = data[8..SIZE_6STORED];
        CryptArray(shuffle, pv);
        if (data.Length > SIZE_6STORED) // Party Stats
            CryptArray(data[SIZE_6STORED..], pv);
        Shuffle67(shuffle, sv);
    }

    /// <summary>
    /// Encrypts a 232 byte + party stat byte array.
    /// </summary>
    /// <param name="data">Decrypted Pokémon data.</param>
    public static void Encrypt67(Span<byte> data)
    {
        Debug.Assert(data.Length is SIZE_6STORED or SIZE_6PARTY);

        uint pv = ReadUInt32LittleEndian(data);
        uint sv = (pv >> 13) & 31;
        sv = BlockPositionInvert[(int)sv];

        var shuffle = data[8..SIZE_6STORED];
        Shuffle67(shuffle, sv);
        CryptArray(shuffle, pv);
        if (data.Length > SIZE_6STORED) // Party Stats
            CryptArray(data[SIZE_6STORED..], pv);
    }

    /// <summary>
    /// Decrypts a Gen8 pk byte array.
    /// </summary>
    /// <param name="data">Encrypted Pokémon data.</param>
    /// <returns>Decrypted Pokémon data.</returns>
    public static void Decrypt8(Span<byte> data)
    {
        Debug.Assert(data.Length is SIZE_8STORED or SIZE_8PARTY);

        uint pv = ReadUInt32LittleEndian(data);
        uint sv = (pv >> 13) & 31;

        var shuffle = data[8..SIZE_8STORED];
        CryptArray(shuffle, pv);
        if (data.Length > SIZE_8STORED) // Party Stats
            CryptArray(data[SIZE_8STORED..], pv); // Party Stats
        Shuffle8(shuffle, sv);
    }

    /// <summary>
    /// Encrypts a Gen8 pk byte array.
    /// </summary>
    /// <param name="data">Decrypted Pokémon data.</param>
    public static void Encrypt8(Span<byte> data)
    {
        Debug.Assert(data.Length is SIZE_8STORED or SIZE_8PARTY);

        uint pv = ReadUInt32LittleEndian(data);
        uint sv = (pv >> 13) & 31;
        sv = BlockPositionInvert[(int)sv];

        var shuffle = data[8..SIZE_8STORED];
        Shuffle8(shuffle, sv);
        CryptArray(shuffle, pv);
        if (data.Length > SIZE_8STORED) // Party Stats
            CryptArray(data[SIZE_8STORED..], pv); // Party Stats
    }

    /// <summary>
    /// Decrypts a PA8 byte array.
    /// </summary>
    /// <param name="data">Encrypted Pokémon data.</param>
    /// <returns>Decrypted Pokémon data.</returns>
    public static void Decrypt8A(Span<byte> data)
    {
        Debug.Assert(data.Length is SIZE_8ASTORED or SIZE_8APARTY);

        uint pv = ReadUInt32LittleEndian(data);
        uint sv = (pv >> 13) & 31;

        var shuffle = data[8..SIZE_8ASTORED];
        CryptArray(shuffle, pv);
        if (data.Length > SIZE_8ASTORED)
            CryptArray(data[SIZE_8ASTORED..], pv); // Party Stats
        Shuffle8A(shuffle, sv);
    }

    /// <summary>
    /// Encrypts a PA8 byte array.
    /// </summary>
    /// <param name="data">Decrypted Pokémon data.</param>
    public static void Encrypt8A(Span<byte> data)
    {
        Debug.Assert(data.Length is SIZE_8ASTORED or SIZE_8APARTY);

        uint pv = ReadUInt32LittleEndian(data);
        uint sv = (pv >> 13) & 31;
        sv = BlockPositionInvert[(int)sv];

        var shuffle = data[8..SIZE_8ASTORED];
        Shuffle8A(shuffle, sv);
        CryptArray(shuffle, pv);
        if (data.Length > SIZE_8ASTORED) // Party Stats
            CryptArray(data[SIZE_8ASTORED..], pv);
    }

    /// <summary>
    /// Encrypts or Decrypts the input <see cref="data"/> using the provided <see cref="seed"/>.
    /// </summary>
    /// <param name="data">The byte array to be encrypted or decrypted. Must be a multiple of 2 bytes in length.</param>
    /// <param name="seed">
    /// The seed used for encryption or decryption.
    /// For Gen3 entities, this is the XOR of PID and OID.
    /// For Gen4/5 entities, this is the checksum for the main data, and the PV for the party stats encryption as well.
    /// For Gen6+ entities, this is the Personality Value (PV) used for both the main data and party stats encryption.
    /// </param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void CryptArray(Span<byte> data, uint seed)
    {
        foreach (ref var u16 in MemoryMarshal.Cast<byte, ushort>(data))
        {
            seed = (0x41C64E6D * seed) + 0x00006073;
            var xor = (ushort)(seed >> 16);
            if (!BitConverter.IsLittleEndian)
                xor = ReverseEndianness(xor);
            u16 ^= xor;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void CryptArray3(Span<byte> data, uint seed)
    {
        if (!BitConverter.IsLittleEndian)
            seed = ReverseEndianness(seed);
        foreach (ref var u32 in MemoryMarshal.Cast<byte, uint>(data))
            u32 ^= seed;
    }

    private static void Shuffle3(Span<byte> data, uint sv) => Shuffle<uint>(data, sv, SIZE_3BLOCK);
    private static void Shuffle45(Span<byte> data, uint sv) => Shuffle<ulong>(data, sv, SIZE_4BLOCK);
    private static void Shuffle67(Span<byte> data, uint sv) => Shuffle<ulong>(data, sv, SIZE_6BLOCK);
    private static void Shuffle8(Span<byte> data, uint sv) => Shuffle<ulong>(data, sv, SIZE_8BLOCK);
    private static void Shuffle8A(Span<byte> data, uint sv) => Shuffle<ulong>(data, sv, SIZE_8ABLOCK);

    private static void Shuffle<T>(Span<byte> data, uint sv, [ConstantExpected(Min = 0)] int blockSizeBytes) where T : unmanaged
    {
        if (sv == 0)
            return;

        var size = Unsafe.SizeOf<T>(); // JIT constant-folded
        var count = blockSizeBytes / size; // number of T-elements per block

        Span<byte> perm = stackalloc byte[BlockCount];
        Span<byte> slotOf = stackalloc byte[BlockCount];

        // build current layout and reverse map (identity)
        for (byte s = 0; s < BlockCount; s++)
            perm[s] = slotOf[s] = s;

        var shuffle = BlockPosition.Slice((int)sv * BlockCount, BlockCount);

        // Perform shuffle, let JIT unroll.
        var u = MemoryMarshal.Cast<byte, T>(data);
        for (byte i = 0; i < BlockCount - 1; i++)
        {
            var desired = shuffle[i];
            var j = slotOf[desired]; // O(1)
            if (j == i)
                continue;
            SwapBlocks(u, i * count, j * count, count);

            // reflect permutation, update reverse map
            // no need to update processed indexes - they won't be used in future loops.
            // we don't care to book-keep the full state of the perm, just the location of unprocessed blocks
            var blockAtI = perm[i];
            // perm[i] = desired;
            perm[j] = blockAtI;
            // slotOf[desired] = i;
            slotOf[blockAtI] = j;
        }
    }

    private static void SwapBlocks<T>(Span<T> u, int a, int b, int count) where T : unmanaged
    {
        for (int i = 0; i < count; i++)
        {
            ref var ra = ref u[a + i];
            ref var rb = ref u[b + i];
            (ra, rb) = (rb, ra);
        }
    }

    /// <summary>
    /// Checks if the Gen3 format entity is encrypted, when the checksum of the data does not match the expected value.
    /// </summary>
    public static bool IsEncrypted3(ReadOnlySpan<byte> data)
    {
        var calc = Checksums.Add16(data[SIZE_3HEADER..SIZE_3STORED]);
        var expect = ReadUInt16LittleEndian(data[0x1C..]);
        return calc != expect;
    }

    /// <summary>
    /// Checks if the Gen4/5 format entity is encrypted, when the unused ribbon bits are not 0.
    /// </summary>
    public static bool IsEncrypted45(ReadOnlySpan<byte> data) => ReadUInt32LittleEndian(data[0x64..]) != 0;

    /// <summary>
    /// Checks if the Gen6/7 format entity is encrypted, when the final terminators of the text strings are not 0.
    /// </summary>
    /// <remarks> Checks Nickname and Original Trainer. </remarks>
    private static bool IsEncrypted67(ReadOnlySpan<byte> data) => ReadUInt16LittleEndian(data[0xC8..]) != 0 || ReadUInt16LittleEndian(data[0x58..]) != 0;

    /// <summary>
    /// Checks if the Gen8 format entity is encrypted, when the final terminators of the text strings are not 0.
    /// </summary>
    /// <remarks> Checks Nickname and Original Trainer. </remarks>
    public static bool IsEncrypted8(ReadOnlySpan<byte> data) => ReadUInt16LittleEndian(data[0x70..]) != 0 || ReadUInt16LittleEndian(data[0x110..]) != 0;

    /// <summary>
    /// Checks if the Gen8a format entity is encrypted, when the final terminators of the text strings are not 0.
    /// </summary>
    /// <remarks> Checks Nickname and Original Trainer. </remarks>
    public static bool IsEncrypted8A(ReadOnlySpan<byte> data) => ReadUInt16LittleEndian(data[0x78..]) != 0 || ReadUInt16LittleEndian(data[0x128..]) != 0;

    /// <summary>
    /// Decrypts the input <see cref="data"/> if it is encrypted.
    /// </summary>
    /// <remarks>Generation 3 Format encryption check which verifies the checksum</remarks>
    public static void DecryptIfEncrypted3(Span<byte> data)
    {
        if (IsEncrypted3(data))
            Decrypt3(data);
    }

    /// <summary>
    /// Decrypts the input <see cref="data"/> if it is encrypted.
    /// </summary>
    /// <remarks>Generation 4 &amp; 5 Format encryption check which checks for the unused bytes</remarks>
    public static void DecryptIfEncrypted45(Span<byte> data)
    {
        if (IsEncrypted45(data))
            Decrypt45(data);
    }

    /// <summary>
    /// Decrypts the input <see cref="data"/> if it is encrypted.
    /// </summary>
    /// <remarks>Generation 6 &amp; 7 Format encryption check</remarks>
    public static void DecryptIfEncrypted67(Span<byte> data)
    {
        if (IsEncrypted67(data))
            Decrypt67(data);
    }

    /// <summary>
    /// Decrypts the input <see cref="data"/> if it is encrypted.
    /// </summary>
    /// <remarks>Generation 8 Format encryption check</remarks>
    public static void DecryptIfEncrypted8(Span<byte> data)
    {
        if (IsEncrypted8(data))
            Decrypt8(data);
    }

    /// <summary>
    /// Decrypts the input <see cref="data"/> if it is encrypted.
    /// </summary>
    /// <remarks>Generation 8 Format encryption check</remarks>
    public static void DecryptIfEncrypted8A(Span<byte> data)
    {
        if (IsEncrypted8A(data))
            Decrypt8A(data);
    }
}

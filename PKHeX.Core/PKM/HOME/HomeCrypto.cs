using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

/// <summary>
/// Logic related to Encrypting and Decrypting Pokémon Home entity data.
/// </summary>
public static class HomeCrypto
{
    internal const int Version1 = 1;

    internal const int SIZE_1HEADER = 0x10; // 16

    internal const int SIZE_1CORE = 0xC8; // 200

    internal const int SIZE_1GAME_PB7 = 0x3B; // 59
    internal const int SIZE_1GAME_PK8 = 0x44; // 68
    internal const int SIZE_1GAME_PA8 = 0x3C; // 60
    internal const int SIZE_1GAME_PB8 = 0x2B; // 43
    internal const int SIZE_1GAME_PK9 = 0x39; // todo sv
    internal const int SIZE_1STORED = 0x1EE; // 494

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void GetFormat1EncryptionKey(Span<byte> key, ulong seed)
    {
        WriteUInt64BigEndian(key, seed ^ 0x6B7B5966193DB88B);
        WriteUInt64BigEndian(key.Slice(8, 8), seed & 0x937EC53BF8856E87);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void GetFormat1EncryptionIv(Span<byte> iv, ulong seed)
    {
        WriteUInt64BigEndian(iv, seed ^ 0x5F4ED4E84975D976);
        WriteUInt64BigEndian(iv.Slice(8, 8), seed | 0xE3CDA917EA9E489C);
    }

    /// <summary>
    /// Encryption and Decryption are asymmetrical operations, but we reuse the same method and pivot off the inputs.
    /// </summary>
    /// <param name="data">Data to crypt, not in place.</param>
    /// <param name="decrypt">Encryption or Decryption mode</param>
    /// <returns>New array with result data.</returns>
    /// <exception cref="ArgumentException"> if the format is not supported.</exception>
    public static byte[] Crypt1(ReadOnlySpan<byte> data, bool decrypt = true)
    {
        var format = ReadUInt16LittleEndian(data);
        if (format != Version1)
            throw new ArgumentException($"Unrecognized format: {format}");

        ulong seed = ReadUInt64LittleEndian(data.Slice(2, 8));

        var key = new byte[0x10];
        GetFormat1EncryptionKey(key, seed);
        var iv  = new byte[0x10];
        GetFormat1EncryptionIv(iv, seed);

        var dataSize = ReadUInt16LittleEndian(data[0xE..0x10]);
        var result = new byte[SIZE_1HEADER + dataSize];
        data[..SIZE_1HEADER].CopyTo(result); // header
        Crypt1(data, key, iv, result, dataSize, decrypt);

        return result;
    }

    private static void Crypt1(ReadOnlySpan<byte> data, byte[] key, byte[] iv, byte[] result, ushort dataSize, bool decrypt)
    {
        using var aes = Aes.Create();
        aes.Mode = CipherMode.CBC;
        aes.Padding = PaddingMode.None; // Handle PKCS7 manually.

        var tmp = data[SIZE_1HEADER..].ToArray();
        using var ms = new MemoryStream(tmp);
        using var transform = decrypt ? aes.CreateDecryptor(key, iv) : aes.CreateEncryptor(key, iv);
        using var cs = new CryptoStream(ms, transform, CryptoStreamMode.Read);

        var size = cs.Read(result, SIZE_1HEADER, dataSize);
        System.Diagnostics.Debug.Assert(SIZE_1HEADER + size == data.Length);
    }

    /// <summary>
    /// Decrypts the input <see cref="data"/> data into a new array if it is encrypted, and updates the reference.
    /// </summary>
    /// <remarks>Format encryption check</remarks>
    public static void DecryptIfEncrypted(ref byte[] data)
    {
        var span = data.AsSpan();
        var format = ReadUInt16LittleEndian(span);
        if (format == Version1)
        {
            if (GetIsEncrypted1(span))
                data = Crypt1(span);
        }
        else
        {
            throw new ArgumentException($"Unrecognized format: {format}");
        }
    }

    /// <summary>
    /// Converts the input <see cref="pk"/> data into their encrypted state.
    /// </summary>
    public static byte[] Encrypt(ReadOnlySpan<byte> pk)
    {
        var result = Crypt1(pk, false);
        RefreshChecksum(result, result);
        return result;
    }

    private static void RefreshChecksum(ReadOnlySpan<byte> encrypted, Span<byte> dest)
    {
        var chk = GetChecksum1(encrypted);
        WriteUInt32LittleEndian(dest[0xA..0xE], chk);
    }

    /// <summary>
    /// Calculates the checksum of format 1 data.
    /// </summary>
    public static uint GetChecksum1(ReadOnlySpan<byte> encrypted) => GetCHK(encrypted[SIZE_1HEADER..]);

    /// <summary>
    /// Checks if the format 1 data is encrypted.
    /// </summary>
    /// <returns>True if encrypted.</returns>
    public static bool GetIsEncrypted1(ReadOnlySpan<byte> data)
    {
        if (ReadUInt16LittleEndian(data[SIZE_1HEADER..]) != SIZE_1CORE)
            return true; // Core length should be constant if decrypted.

        var core = data.Slice(SIZE_1HEADER + 2, SIZE_1CORE);
        if (ReadUInt16LittleEndian(core[0xB5..]) != 0)
            return true; // OT_Name final terminator should be 0 if decrypted.
        if (ReadUInt16LittleEndian(core[0x60..]) != 0)
            return true; // Nickname final terminator should be 0 if decrypted.
        if (ReadUInt16LittleEndian(core[0x88..]) != 0)
            return true; // HT_Name final terminator should be 0 if decrypted.

        //// Fall back to checksum.
        //return ReadUInt32LittleEndian(data[0xA..0xE]) == GetChecksum1(data);
        return false; // 64 bits checked is enough to feel safe about this check.
    }

    /// <summary>
    /// Gets the checksum of an Pokémon's AES-encrypted data.
    /// </summary>
    /// <param name="data">AES-Encrypted Pokémon data.</param>
    public static uint GetCHK(ReadOnlySpan<byte> data)
    {
        uint chk = 0;
        for (var i = 0; i < data.Length; i += 100)
        {
            var chunkSize = Math.Min(data.Length - i, 100);
            var span = data.Slice(i, chunkSize);
            chk ^= Checksums.CRC32Invert(span);
        }
        return chk;
    }
}

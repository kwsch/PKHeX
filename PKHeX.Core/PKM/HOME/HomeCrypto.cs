using System;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

/// <summary>
/// Logic related to Encrypting and Decrypting Pokémon Home entity data.
/// </summary>
public static class HomeCrypto
{
    public const int Version1 = 1;
    public const int Version2 = 2;
    public const int Version3 = 3;

    public const int SIZE_1HEADER = 0x10; // 16

    public const int SIZE_1CORE = 0xC8; // 200
    public const int SIZE_1GAME_PB7 = 0x3B; // 59
    public const int SIZE_1GAME_PK8 = 0x44; // 68
    public const int SIZE_1GAME_PA8 = 0x3C; // 60
    public const int SIZE_1GAME_PB8 = 0x2B; // 43
    public const int SIZE_1STORED = 0x1EE; // 494

    public const int SIZE_2CORE = 0xC4; // 196
    public const int SIZE_2GAME_PB7 = 0x3F; // 63
    public const int SIZE_2GAME_PK8 = 0x48; // 72
    public const int SIZE_2GAME_PA8 = 0x40; // 64
    public const int SIZE_2GAME_PB8 = 0x2F; // 47
    public const int SIZE_2GAME_PK9 = 0x3D; // 61
    public const int SIZE_2STORED = 0x23A; // 570

    public const int SIZE_3GAME_PK9 = 0x3D + 0xD; // 61
    public const int SIZE_3STORED = 0x247; // 583

    public const int SIZE_STORED = SIZE_3STORED;
    public const int SIZE_CORE = SIZE_2CORE;
    public const int VersionLatest = Version3;

    public static bool IsKnownVersion(ushort version) => version is Version1 or Version2 or Version3;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void SetEncryptionKey(Span<byte> key, ulong seed)
    {
        WriteUInt64BigEndian(key, seed ^ 0x6B7B5966193DB88B);
        WriteUInt64BigEndian(key.Slice(8, 8), seed & 0x937EC53BF8856E87);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void SetEncryptionIv(Span<byte> iv, ulong seed)
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
    public static byte[] Crypt(ReadOnlySpan<byte> data, bool decrypt = true)
    {
        var format = ReadUInt16LittleEndian(data);
        if (!IsKnownVersion(format))
            throw new ArgumentException($"Unrecognized format: {format}");

        ulong seed = ReadUInt64LittleEndian(data.Slice(2, 8));

        var key = new byte[0x10];
        SetEncryptionKey(key, seed);
        var iv  = new byte[0x10];
        SetEncryptionIv(iv, seed);

        var dataSize = ReadUInt16LittleEndian(data[0xE..0x10]);
        var result = new byte[SIZE_1HEADER + dataSize];
        data[..SIZE_1HEADER].CopyTo(result); // header

        var input = data.Slice(SIZE_1HEADER, dataSize);
        var output = result.AsSpan(SIZE_1HEADER, dataSize);
        Crypt(input, output, key, iv, decrypt);

        return result;
    }

    private static void Crypt(ReadOnlySpan<byte> data, Span<byte> result, byte[] key, byte[] iv, bool decrypt)
    {
        // Handle PKCS7 manually.
        using var aes = RuntimeCryptographyProvider.Aes.Create(key, CipherMode.CBC, PaddingMode.None, iv);
        if (decrypt)
            aes.DecryptCbc(data, result);
        else
            aes.EncryptCbc(data, result);
    }

    /// <summary>
    /// Decrypts the input <see cref="data"/> data into a new array if it is encrypted, and updates the reference.
    /// </summary>
    /// <remarks>Format encryption check</remarks>
    public static void DecryptIfEncrypted(ref byte[] data)
    {
        var span = data.AsSpan();
        var format = ReadUInt16LittleEndian(span);
        if (IsKnownVersion(format))
        {
            if (GetIsEncrypted(span, format))
                data = Crypt(span);
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
        var result = Crypt(pk, false);
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
    public static bool GetIsEncrypted(ReadOnlySpan<byte> data, ushort format) => format switch
    {
        Version1 => IsEncryptedCore1(data),
        Version2 => IsEncryptedCore2(data),
        Version3 => IsEncryptedCore3(data),
        _ => throw new ArgumentException($"Unrecognized format: {format}"),
    };

    private static bool IsEncryptedCore1(ReadOnlySpan<byte> data)
    {
        var core = data.Slice(SIZE_1HEADER + 2, SIZE_1CORE);

        // Strings should be \0000 terminated if decrypted.
        // Any non-zero value is a sign of encryption.
        if (ReadUInt16LittleEndian(core[0xB5..]) != 0) // OT
            return true; // OriginalTrainerName final terminator should be 0 if decrypted.
        if (ReadUInt16LittleEndian(core[0x60..]) != 0) // Nick
            return true; // Nickname final terminator should be 0 if decrypted.
        if (ReadUInt16LittleEndian(core[0x88..]) != 0) // HT
            return true; // HandlingTrainerName final terminator should be 0 if decrypted.

        //// Fall back to checksum.
        //return ReadUInt32LittleEndian(data[0xA..0xE]) == GetChecksum1(data);
        return false; // 64 bits checked is enough to feel safe about this check.
    }

    private static bool IsEncryptedCore2(ReadOnlySpan<byte> data)
    {
        var core = data.Slice(SIZE_1HEADER + 2, SIZE_2CORE);
        if (ReadUInt16LittleEndian(core[0xB1..]) != 0)
            return true; // OriginalTrainerName final terminator should be 0 if decrypted.
        if (ReadUInt16LittleEndian(core[0x5C..]) != 0)
            return true; // Nickname final terminator should be 0 if decrypted.
        if (ReadUInt16LittleEndian(core[0x84..]) != 0)
            return true; // HandlingTrainerName final terminator should be 0 if decrypted.

        //// Fall back to checksum.
        //return ReadUInt32LittleEndian(data[0xA..0xE]) == GetChecksum1(data);
        return false; // 64 bits checked is enough to feel safe about this check.
    }

    private static bool IsEncryptedCore3(ReadOnlySpan<byte> data) => IsEncryptedCore2(data); // Same struct as Core version 2.

    /// <summary>
    /// Gets the checksum of a Pokémon's AES-encrypted data.
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

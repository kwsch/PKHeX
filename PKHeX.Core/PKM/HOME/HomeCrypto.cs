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
    internal const int Version = 1;

    // Format 1 Core Data has size 200 bytes
    internal const int SIZE_1CORE = 200;

    internal const int SIZE_1GAME_PB7 = 59;
    internal const int SIZE_1GAME_PK8 = 68;
    internal const int SIZE_1GAME_PA8 = 60;
    internal const int SIZE_1GAME_PB8 = 43;
    internal const int SIZE_1STORED = 494;

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

    public static byte[] DecryptFormat1(Span<byte> ekm)
    {
        ushort format = ReadUInt16LittleEndian(ekm);
        if (format != 1)
            throw new ArgumentException($"Invalid format {format} != 1");

        ulong seed = ReadUInt64LittleEndian(ekm.Slice(2, 8));

        var key = new byte[0x10];
        GetFormat1EncryptionKey(key, seed);

        var iv  = new byte[0x10];
        GetFormat1EncryptionIv(iv, seed);

        var dataSize = ReadUInt16LittleEndian(ekm[0xE..0x10]);

        using var ms = new MemoryStream(dataSize);
        ms.Write(ekm[0x10..].ToArray(), 0, dataSize);
        ms.Seek(0, SeekOrigin.Begin);

        using var aes = Aes.Create();
        aes.Mode = CipherMode.CBC;
        aes.Padding = PaddingMode.PKCS7;

        using var cs = new CryptoStream(ms, aes.CreateDecryptor(key, iv), CryptoStreamMode.Read);

        var dec = new byte[0x10 + dataSize];
        ekm[..0x10].CopyTo(dec);

        var decSize = cs.Read(dec, 0x10, dataSize);
        Array.Resize(ref dec, 0x10 + decSize);

        return dec;
    }

    /// <summary>
    /// Decrypts the input <see cref="pkm"/> data into a new array if it is encrypted, and updates the reference.
    /// </summary>
    /// <remarks>Generation 8 Format encryption check</remarks>
    public static void DecryptIfEncrypted(ref byte[] pkm)
    {
        var span = pkm.AsSpan();

        // TODO: This can fail if by chance first two bytes are SIZE_1CORE
        // How can this detect encryption when there are no padding bytes?
        if (ReadUInt16LittleEndian(span) == 1 && ReadUInt16LittleEndian(span[0x10..]) != SIZE_1CORE)
            pkm = DecryptFormat1(span);
    }

    /// <summary>
    /// Gets the checksum of an Pokémon's AES-encrypted data.
    /// </summary>
    /// <param name="data">AES-Encrypted Pokémon data.</param>
    public static uint GetCHK(ReadOnlySpan<byte> data)
    {
        uint chk = 0;
        for (var i = 0; i < data.Length; i += 100)
            chk ^= Checksums.CRC32Invert(data[i..(i + 100)]);
        return chk;
    }
}

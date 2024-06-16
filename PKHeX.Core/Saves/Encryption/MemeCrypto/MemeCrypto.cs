using System;
using System.Security.Cryptography;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

/// <summary>
/// MemeCrypto V1 - The Original Series
/// </summary>
/// <remarks>
/// A variant of <see cref="SaveFile"/> encryption and obfuscation used in <see cref="GameVersion.Gen7"/>.
/// <br> The save file stores a dedicated block to contain a hash of the savedata, computed when the block is zeroed. </br>
/// <br> This signing logic is reused for other authentication; refer to <see cref="MemeKeyIndex"/>. </br>
/// <br> The save file first computes a SHA256 Hash over the block checksum region.
/// The logic then applies a SHA1 hash over the SHA256 hash result, encrypts it with a <see cref="MemeKey"/>, and signs it with an RSA private key in a non-straightforward manner. </br>
/// </remarks>
public static class MemeCrypto
{
    private const uint POKE = 0x454B4F50;

    public static bool VerifyMemePOKE(ReadOnlySpan<byte> input, out byte[] output)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(input.Length, MemeKey.SignatureLength);
        var memeLen = input.Length - 8;
        var memeIndex = MemeKeyIndex.PokedexAndSaveFile;
        for (var i = input.Length - 8; i >= 0; i--)
        {
            if (ReadUInt32LittleEndian(input[i..]) != POKE)
                continue;

            var keyIndex = ReadInt32LittleEndian(input[(i+4)..]);
            if (!MemeKey.IsValidPokeKeyIndex(keyIndex))
                continue;

            memeLen = i;
            memeIndex = (MemeKeyIndex)keyIndex;
            break;
        }

        foreach (var len in new[] { memeLen, memeLen - 2 }) // Account for Pokédex QR Edge case
        {
            if (VerifyMemeData(input[..len], out output, memeIndex))
                return true;

            if (VerifyMemeData(input[..len], out output, MemeKeyIndex.PokedexAndSaveFile))
                return true;
        }

        output = [];
        return false;
    }

    public static bool VerifyMemeData(ReadOnlySpan<byte> input, out byte[] output)
    {
        foreach (MemeKeyIndex keyIndex in Enum.GetValues<MemeKeyIndex>())
        {
            if (VerifyMemeData(input, out output, keyIndex))
                return true;
        }
        output = [];
        return false;
    }

    public static bool VerifyMemeData(ReadOnlySpan<byte> input, out byte[] output, MemeKeyIndex keyIndex)
    {
        if (input.Length < MemeKey.SignatureLength)
        {
            output = [];
            return false;
        }
        var key = new MemeKey(keyIndex);
        Span<byte> sigBuffer = stackalloc byte[MemeKey.SignatureLength];
        var inputSig = input[^MemeKey.SignatureLength..];
        key.RsaPublic(inputSig, sigBuffer);

        output = input.ToArray();
        if (DecryptCompare(output, sigBuffer, key))
            return true;

        sigBuffer[0x0] |= 0x80;

        output = input.ToArray();
        if (DecryptCompare(output, sigBuffer, key))
            return true;

        output = [];
        return false;
    }

    private static bool DecryptCompare(Span<byte> output, ReadOnlySpan<byte> sigBuffer, MemeKey key)
    {
        sigBuffer.CopyTo(output[^MemeKey.SignatureLength..]);
        key.AesDecrypt(output);
        // Check for 8-byte equality.
        Span<byte> hash = stackalloc byte[SHA1.HashSizeInBytes];
        SHA1.HashData(output[..^8], hash);
        return hash[..8].SequenceEqual(output[^8..]);
    }

    public static byte[] SignMemeData(ReadOnlySpan<byte> input, MemeKeyIndex keyIndex = MemeKeyIndex.PokedexAndSaveFile)
    {
        var output = input.ToArray();
        SignMemeDataInPlace(output, keyIndex);
        return output;
    }

    private static void SignMemeDataInPlace(Span<byte> data, MemeKeyIndex keyIndex = MemeKeyIndex.PokedexAndSaveFile)
    {
        // Validate Input
        ArgumentOutOfRangeException.ThrowIfLessThan(data.Length, MemeKey.SignatureLength);
        var key = new MemeKey(keyIndex);
        if (!key.CanResign)
            throw new ArgumentException("Cannot sign with the specified key!");

        // SHA1 the entire payload except for the last 8 bytes; store the first 8 bytes of hash at the end of the input.
        var payload = data[..^8];
        var hash = data[^8..];
        Span<byte> tmp = stackalloc byte[SHA1.HashSizeInBytes];
        SHA1.HashData(payload, tmp);

        // Copy in the SHA1 signature
        tmp[..hash.Length].CopyTo(hash);

        // Perform AES operations
        key.AesEncrypt(data);
        var sigBuffer = data[^MemeKey.SignatureLength..];
        sigBuffer[0] &= 0x7F; // chop off sign bit
        key.RsaPrivate(sigBuffer, sigBuffer);
    }

    public const int SaveFileSignatureOffset = 0x100;
    public const int SaveFileSignatureLength = 0x80;

    /// <summary>
    /// Resigns save data.
    /// </summary>
    /// <param name="span">Save file data to resign</param>
    /// <returns>The resigned save data. Invalid input returns null.</returns>
    public static void SignInPlace(Span<byte> span)
    {
        if (span.Length is not (SaveUtil.SIZE_G7SM or SaveUtil.SIZE_G7USUM))
            throw new ArgumentException("Should not be using this for unsupported saves.");

        var isUSUM = span.Length == SaveUtil.SIZE_G7USUM;
        var ChecksumTableOffset = span.Length - 0x200;
        var ChecksumSignatureLength = isUSUM ? 0x150 : 0x140;
        var MemeCryptoOffset = (isUSUM ? 0x6C000 : 0x6BA00) + SaveFileSignatureOffset;

        // data[0x80]. Region is initially zero when exporting (nothing set).
        // Store a SHA256[0x20] hash of checksums at [..0x20].
        // Compute the signature over this 0x80 region, store at [0x20..]
        var sigSpan = span.Slice(MemeCryptoOffset, SaveFileSignatureLength);
        var chkBlockSpan = span.Slice(ChecksumTableOffset, ChecksumSignatureLength);

        SignInPlace(sigSpan, chkBlockSpan);
    }

    public static void SignInPlace(Span<byte> sigSpan, ReadOnlySpan<byte> chkBlockSpan)
    {
        SHA256.HashData(chkBlockSpan, sigSpan);
        SignMemeDataInPlace(sigSpan);
    }
}

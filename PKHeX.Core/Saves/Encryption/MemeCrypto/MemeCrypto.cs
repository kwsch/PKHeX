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
        if (input.Length < 0x60)
            throw new ArgumentException("Invalid POKE buffer!");
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
            if (VerifyMemeData(input, out output, 0, len, memeIndex))
                return true;

            if (VerifyMemeData(input, out output, 0, len, MemeKeyIndex.PokedexAndSaveFile))
                return true;
        }

        output = Array.Empty<byte>();
        return false;
    }

    public static bool VerifyMemeData(ReadOnlySpan<byte> input, out byte[] output)
    {
        foreach (MemeKeyIndex keyIndex in Enum.GetValues(typeof(MemeKeyIndex)))
        {
            if (VerifyMemeData(input, out output, keyIndex))
                return true;
        }
        output = Array.Empty<byte>();
        return false;
    }

    public static bool VerifyMemeData(ReadOnlySpan<byte> input, out byte[] output, MemeKeyIndex keyIndex)
    {
        if (input.Length < 0x60)
        {
            output = Array.Empty<byte>();
            return false;
        }
        var key = new MemeKey(keyIndex);
        output = input.ToArray();

        var sigBuffer = key.RsaPublic(input[^0x60..]);
        if (DecryptCompare(output, sigBuffer, key))
            return true;
        sigBuffer[0x0] |= 0x80;
        if (DecryptCompare(output, sigBuffer, key))
            return true;

        output = Array.Empty<byte>();
        return false;
    }

    private static bool DecryptCompare(byte[] output, ReadOnlySpan<byte> sigBuffer, MemeKey key)
    {
        sigBuffer.CopyTo(output.AsSpan(output.Length - 0x60));
        key.AesDecrypt(output).CopyTo(output);
        // Check for 8-byte equality.
        var hash = SHA1.HashData(output.AsSpan(0, output.Length - 0x8));
        var computed = ReadUInt64LittleEndian(hash.AsSpan());
        var existing = ReadUInt64LittleEndian(output.AsSpan(output.Length - 0x8));
        return computed == existing;
    }

    public static bool VerifyMemeData(ReadOnlySpan<byte> input, out byte[] output, int offset, int length)
    {
        var data = input.Slice(offset, length).ToArray();
        if (VerifyMemeData(data, out output))
        {
            var newOutput = input.ToArray();
            output.CopyTo(newOutput, offset);
            output = newOutput;
            return true;
        }
        output = Array.Empty<byte>();
        return false;
    }

    public static bool VerifyMemeData(ReadOnlySpan<byte> input, out byte[] output, int offset, int length, MemeKeyIndex keyIndex)
    {
        var data = input.Slice(offset, length);
        if (VerifyMemeData(data, out output, keyIndex))
        {
            var newOutput = input.ToArray();
            output.CopyTo(newOutput, offset);
            output = newOutput;
            return true;
        }
        output = Array.Empty<byte>();
        return false;
    }

    public static byte[] SignMemeData(ReadOnlySpan<byte> input, MemeKeyIndex keyIndex = MemeKeyIndex.PokedexAndSaveFile)
    {
        // Validate Input
        if (input.Length < 0x60)
            throw new ArgumentException("Cannot memesign a buffer less than 0x60 bytes in size!");
        var key = new MemeKey(keyIndex);
        if (!key.CanResign)
            throw new ArgumentException("Cannot sign with the specified memekey!");

        var output = input.ToArray();

        // Copy in the SHA1 signature
        var payload = output.AsSpan(0, output.Length - 8);
        var hash = output.AsSpan(output.Length - 8, 8);
        Span<byte> tmp = stackalloc byte[0x20];
        SHA1.HashData(payload, tmp);
        tmp[..hash.Length].CopyTo(hash);

        // Perform AES operations
        output = key.AesEncrypt(output);
        var sigBuffer = output.AsSpan(output.Length - 0x60, 0x60);
        sigBuffer[0] &= 0x7F;
        var signed = key.RsaPrivate(sigBuffer);
        signed.CopyTo(sigBuffer);
        return output;
    }

    /// <summary>
    /// Resigns save data.
    /// </summary>
    /// <param name="sav7">Save file data to resign</param>
    /// <returns>The resigned save data. Invalid input returns null.</returns>
    public static byte[] Resign7(ReadOnlySpan<byte> sav7)
    {
        if (sav7.Length is not (SaveUtil.SIZE_G7SM or SaveUtil.SIZE_G7USUM))
            throw new ArgumentException("Should not be using this for unsupported saves.");

        // Save Chunks are 0x200 bytes each; Memecrypto signature is 0x100 bytes into the 2nd to last chunk.
        var isUSUM = sav7.Length == SaveUtil.SIZE_G7USUM;
        var ChecksumTableOffset = sav7.Length - 0x200;
        var MemeCryptoOffset = isUSUM ? 0x6C100 : 0x6BB00;
        var ChecksumSignatureLength = isUSUM ? 0x150 : 0x140;
        const int MemeCryptoSignatureLength = 0x80;

        var result = sav7.ToArray();

        // Store current signature
        var oldSig = sav7.Slice(MemeCryptoOffset, MemeCryptoSignatureLength).ToArray();
        Span<byte> sigSpan = stackalloc byte[MemeCryptoSignatureLength];
        SHA256.HashData(result.AsSpan(ChecksumTableOffset, ChecksumSignatureLength), sigSpan);

        if (VerifyMemeData(oldSig, out var memeSig, MemeKeyIndex.PokedexAndSaveFile))
            memeSig.AsSpan()[0x20..0x80].CopyTo(sigSpan[0x20..]);

        SignMemeData(sigSpan).CopyTo(result, MemeCryptoOffset);
        return result;
    }
}

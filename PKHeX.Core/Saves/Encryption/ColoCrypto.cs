using System;
using System.Security.Cryptography;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

/// <summary>
/// Encryption logic used by Pokémon Colosseum
/// </summary>
public static class ColoCrypto
{
    private const int sha1HashSize = 20;
    public const int SLOT_SIZE = 0x1E000;
    public const int SLOT_START = 0x6000;
    public const int SLOT_COUNT = 3;
    public const int SAVE_SIZE = SLOT_START + (SLOT_SIZE * SLOT_COUNT);

    public static byte[] GetSlot(ReadOnlySpan<byte> fullEncrypted, int slotIndex)
    {
        ArgumentOutOfRangeException.ThrowIfNotEqual(fullEncrypted.Length, SAVE_SIZE);
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual<uint>((uint)slotIndex, SLOT_COUNT);

        var result = fullEncrypted.Slice(SLOT_START + (slotIndex * SLOT_SIZE), SLOT_SIZE).ToArray();
        DecryptInPlace(result);
        return result;
    }

    public static Memory<byte> GetSlot(Memory<byte> fullEncrypted, int slotIndex)
    {
        ArgumentOutOfRangeException.ThrowIfNotEqual(fullEncrypted.Length, SAVE_SIZE);
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual<uint>((uint)slotIndex, SLOT_COUNT);

        var result = fullEncrypted.Slice(SLOT_START + (slotIndex * SLOT_SIZE), SLOT_SIZE);
        DecryptInPlace(result.Span);
        return result;
    }

    public static void GetSlot(ReadOnlySpan<byte> fullEncrypted, int slotIndex, Span<byte> slotData)
    {
        ArgumentOutOfRangeException.ThrowIfNotEqual(slotData.Length, SLOT_SIZE);
        ArgumentOutOfRangeException.ThrowIfNotEqual(fullEncrypted.Length, SAVE_SIZE);
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual<uint>((uint)slotIndex, SLOT_COUNT);

        var data = fullEncrypted.Slice(SLOT_START + (slotIndex * SLOT_SIZE), SLOT_SIZE);
        data.CopyTo(slotData);
        DecryptInPlace(slotData);
    }

    public static void SetSlot(Span<byte> fullEncrypted, int slotIndex, ReadOnlySpan<byte> slotData)
    {
        ArgumentOutOfRangeException.ThrowIfNotEqual(slotData.Length, SLOT_SIZE);
        ArgumentOutOfRangeException.ThrowIfNotEqual(fullEncrypted.Length, SAVE_SIZE);
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual<uint>((uint)slotIndex, SLOT_COUNT);

        var data = fullEncrypted.Slice(SLOT_START + (slotIndex * SLOT_SIZE), SLOT_SIZE);
        slotData.CopyTo(data);
        EncryptInPlace(data);
    }

    public static void EncryptInPlace(Span<byte> slot)
    {
        ArgumentOutOfRangeException.ThrowIfNotEqual(slot.Length, SLOT_SIZE);

        // Get updated save slot data
        Span<byte> digest = stackalloc byte[sha1HashSize];
        slot[^sha1HashSize..].CopyTo(digest);

        EncryptColosseum(slot, digest);
    }

    public static void DecryptInPlace(Span<byte> slot)
    {
        ArgumentOutOfRangeException.ThrowIfNotEqual(slot.Length, SLOT_SIZE);

        // Get updated save slot data
        Span<byte> digest = stackalloc byte[sha1HashSize];
        slot[^sha1HashSize..].CopyTo(digest);

        DecryptColosseum(slot, digest);
    }

    // Encryption Algorithm:
    // Use a 20-byte hash as the seed for an iterative xor & hash-decrypted routine.
    // Decrypt: Hash encrypted bytes for next loop, then un-xor to the decrypted state.
    // Encrypt: xor to the encrypted state, then hash for the next loop.

    private static void EncryptColosseum(Span<byte> data, Span<byte> digest)
    {
        // At-rest key is NOT the initial key :)
        for (int i = 0; i < digest.Length; i++)
            digest[i] = (byte)~digest[i];

        var span = data[0x18..^sha1HashSize];
        while (span.Length >= sha1HashSize)
        {
            // Could cast to u32 but juggling that is not worth it.
            for (int j = 0; j < digest.Length; j++)
                span[j] ^= digest[j];

            // Hash the encrypted bytes for the next loop
            SHA1.HashData(span[..sha1HashSize], digest);

            span = span[sha1HashSize..];
        }
    }

    // The second to last 20 bytes of the slot doesn't appear to be used for anything.
    // When we do our hash over 0x18..0x1DFD7, we spill over 0x10 bytes due to (length % 20) not being 0.
    // Wonder what the correct behavior is here? :(

    private static void DecryptColosseum(Span<byte> data, Span<byte> digest)
    {
        // At-rest key is NOT the initial key :)
        for (int i = 0; i < digest.Length; i++)
            digest[i] = (byte)~digest[i];

        Span<byte> hash = stackalloc byte[sha1HashSize];
        var span = data[0x18..^sha1HashSize];
        while (span.Length >= sha1HashSize)
        {
            // Hash the encrypted bytes for the next loop
            SHA1.HashData(span[..sha1HashSize], hash);

            // Could cast to u32 but juggling that is not worth it.
            for (int j = 0; j < digest.Length; j++)
                span[j] ^= digest[j];

            // for use in next loop
            hash.CopyTo(digest);

            span = span[sha1HashSize..];
        }
    }

    private static int ComputeHeaderChecksum(Span<byte> header, Span<byte> hash)
    {
        int result = 0;
        for (int i = 0; i < 0x18; i += 4)
            result -= ReadInt32BigEndian(header[i..]);
        result -= ReadInt32BigEndian(header[0x18..]) ^ ~ReadInt32BigEndian(hash);
        result -= ReadInt32BigEndian(header[0x1C..]) ^ ~ReadInt32BigEndian(hash[4..]);
        return result;
    }

    public static void SetChecksums(Span<byte> data)
    {
        ArgumentOutOfRangeException.ThrowIfNotEqual(data.Length, SLOT_SIZE);

        var header = data[..0x20];
        var payload = data[..^(2 * sha1HashSize)];
        var hash = data[^sha1HashSize..];
        var headerCHK = data[0x0C..];

        // Clear Header Checksum
        WriteInt32BigEndian(headerCHK, 0);
        // Compute checksum of data
        SHA1.HashData(payload, hash);

        // Compute new header checksum
        int newHC = ComputeHeaderChecksum(header, hash);

        // Set Header Checksum
        WriteInt32BigEndian(headerCHK, newHC);
    }

    /// <summary>
    /// Checks if the checksums are valid. Needs to mutate the header, only temporarily (no changes when returned).
    /// </summary>
    public static (bool IsHeaderValid, bool IsBodyValid) IsChecksumValid(Span<byte> data)
    {
        ArgumentOutOfRangeException.ThrowIfNotEqual(data.Length, SLOT_SIZE);

        var header = data[..0x20];
        var payload = data[..^(2 * sha1HashSize)];
        var storedHash = data[^sha1HashSize..];
        var hc = header[0x0C..];

        int oldHC = ReadInt32BigEndian(hc);
        // Clear Header Checksum
        WriteInt32BigEndian(hc, 0);
        Span<byte> currentHash = stackalloc byte[sha1HashSize];
        SHA1.HashData(payload, currentHash);

        // Compute new header checksum
        int newHC = ComputeHeaderChecksum(header, currentHash);

        // Restore old header checksum
        WriteInt32BigEndian(hc, oldHC);
        bool isHeaderValid = newHC == oldHC;
        bool isBodyValid = storedHash.SequenceEqual(currentHash);
        return (isHeaderValid, isBodyValid);
    }

    public static (int Index, int Count) DetectLatest(ReadOnlySpan<byte> data)
    {
        // Scan all 3 save slots for the highest counter
        ArgumentOutOfRangeException.ThrowIfNotEqual(data.Length, SaveUtil.SIZE_G3COLO);
        int counter = -1, index = -1;
        for (int i = 0; i < SLOT_COUNT; i++)
        {
            int slotOffset = SLOT_START + (i * SLOT_SIZE);
            int SaveCounter = ReadInt32BigEndian(data[(slotOffset + 4)..]);
            if (SaveCounter <= counter)
                continue;

            counter = SaveCounter;
            index = i;
        }
        return (index, counter);
    }
}

using System;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

/// <summary>
/// Encryption logic used by Pokémon XD
/// </summary>
public static class XDCrypto
{
    private const int SLOT_SIZE = 0x28000;
    private const int SLOT_START = 0x6000;
    private const int SLOT_COUNT = 2;
    public const int SAVE_SIZE = SLOT_START + (SLOT_SIZE * SLOT_COUNT);
    // 0x04 - 0x08: Save Counter (big-endian)
    // 0x08 - 0x10: Encryption Keys
    // 0x10 - 0x27FD8: Encrypted Data

    // 0x10 - 0x20: Checksum Data (4 x ushort, big-endian)

    public static Memory<byte> GetSlot(Memory<byte> raw, int slotIndex)
    {
        ArgumentOutOfRangeException.ThrowIfNotEqual(raw.Length, SAVE_SIZE);
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual<uint>((uint)slotIndex, SLOT_COUNT);

        return raw.Slice(SLOT_START + (slotIndex * SLOT_SIZE), SLOT_SIZE);
    }

    public static (int Index, int Count) DetectLatest(ReadOnlySpan<byte> data)
    {
        ArgumentOutOfRangeException.ThrowIfNotEqual(data.Length, SAVE_SIZE);

        // Scan all 3 save slots for the highest counter
        int counter = -1, index = -1;
        for (int i = 0; i < SLOT_COUNT; i++)
        {
            int slotOffset = SLOT_START + (i * SLOT_SIZE);
            int ctr = ReadInt32BigEndian(data[(slotOffset + 4)..]);
            if (ctr <= counter)
                continue;

            counter = ctr;
            index = i;
        }
        return (index, counter);
    }

    public static void DecryptSlot(Span<byte> slot) => GeniusCrypto.Decrypt(slot, new Range(0x08, 0x10), new Range(0x10, 0x27FD8));
    public static void EncryptSlot(Span<byte> slot) => GeniusCrypto.Encrypt(slot, new Range(0x08, 0x10), new Range(0x10, 0x27FD8));
    public static void SetChecksums(Span<byte> data, int subOffset0)
    {
        ArgumentOutOfRangeException.ThrowIfNotEqual(data.Length, SLOT_SIZE);

        const int start = 0xA8; // 0x88 + 0x20

        // Header Checksum
        int newHC = 0;
        for (int i = 0; i < 8; i++)
            newHC += data[i];
        WriteInt32BigEndian(data[(start + subOffset0 + 0x38)..], newHC);

        // Body Checksum
        data[0x10..0x20].Clear(); // Clear old Checksum Data
        Span<uint> checksum = stackalloc uint[4];
        int dt = 8;
        for (int i = 0; i < checksum.Length; i++)
        {
            uint val = 0;
            var end = dt + 0x9FF4;
            for (int j = dt; j < end; j += 2)
                val += ReadUInt16BigEndian(data[j..]);
            dt = end;
            checksum[i] = val;
        }

        Span<ushort> newchks = stackalloc ushort[8];
        for (int i = 0; i < 4; i++)
        {
            newchks[i * 2] = (ushort)(checksum[i] >> 16);
            newchks[(i * 2) + 1] = (ushort)checksum[i];
        }

        for (int i = 0; i < newchks.Length; i++)
        {
            var dest = data[(0x10 + (2 * i))..];
            var chk = newchks[newchks.Length - 1 - i];
            WriteUInt16BigEndian(dest, chk);
        }
    }
}

using System;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;
// anatomy of a QR7:
// u32 magic; // POKE
// u32 _0xFF;
// u32 box;
// u32 slot;
// u32 num_copies;
// u8  reserved[0x1C];
// u8  ek7[0x104];
// u8  dex_data[0x60];
// u16 crc16
// sizeof(QR7) == 0x1A2

/// <summary>
/// Generation 7 QR format (readable by the in-game QR Scanner feature)
/// </summary>
public static class QR7
{
    public const int SIZE = 0x1A2;

    private static ReadOnlySpan<byte> GenderDifferences =>
    [
        0x08, 0x10, 0x18, 0x06, 0x00, 0x36, 0x00, 0x00, 0x03, 0x00,
        0x30, 0x00, 0x02, 0x80, 0xC1, 0x08, 0x06, 0x00, 0x00, 0x04,
        0x60, 0x00, 0x04, 0x46, 0x4C, 0x8C, 0xD1, 0x22, 0x21, 0x01,
        0x00, 0x80, 0x03, 0x28, 0x0D, 0x00, 0x00, 0x00, 0x18, 0x38,
        0x0C, 0x10, 0x00, 0x40, 0x00, 0x00, 0x02, 0x00, 0x00, 0xF0,
        0xBF, 0x80, 0x0E, 0x01, 0x00, 0x38, 0x66, 0x3B, 0x03, 0x02,
        0x00, 0x00, 0x00, 0x00, 0x00, 0x02, 0x00, 0x00, 0x00, 0x00,
        0x00, 0x00, 0x00, 0x00, 0x03, 0x00, 0x00, 0x00, 0x00, 0x00,
        0x00, 0x00, 0x00, 0x10, 0x40,
    ];

    private static bool IsGenderDifferent(ushort species)
    {
        var index = species >> 3;
        var table = GenderDifferences;
        if (index >= table.Length)
            return false;
        return (table[index] & (1 << (species & 7))) != 0;
    }

    private static void GetRawQR(Span<byte> dest, ushort species, byte form, bool shiny, byte gender)
    {
        dest[..6].Fill(0xFF);
        WriteUInt16LittleEndian(dest[0x28..], species);

        var pi = PersonalTable.USUM.GetFormEntry(species, form);
        bool biGender = false;
        if (pi.OnlyMale)
            gender = 0;
        else if (pi.OnlyFemale)
            gender = 1;
        else if (pi.Genderless)
            gender = 2;
        else
            biGender = !IsGenderDifferent(species);

        dest[0x2A] = form;
        dest[0x2B] = gender;
        dest[0x2C] = shiny ? (byte)1 : (byte)0;
        dest[0x2D] = biGender ? (byte)1 : (byte)0;
    }

    public static byte[] GenerateQRData(PK7 pk7, int box = 0, int slot = 0, int num_copies = 1)
    {
        byte[] data = new byte[SIZE];
        SetQRData(pk7, data, box, slot, num_copies);
        return data;
    }

    public static void SetQRData(PK7 pk7, Span<byte> span, int box = 0, int slot = 0, int num_copies = 1)
    {
        box = Math.Clamp(box, 0, 31);
        slot = Math.Clamp(slot, 0, 29);
        num_copies = Math.Min(num_copies, 1);
        ArgumentOutOfRangeException.ThrowIfLessThan(span.Length, SIZE);

        WriteUInt32LittleEndian(span, 0x454B4F50); // POKE magic
        span[0x4] = 0xFF; // QR Type
        WriteInt32LittleEndian(span[0x08..], box);
        WriteInt32LittleEndian(span[0x0C..], slot);
        WriteInt32LittleEndian(span[0x10..], num_copies); // No need to check max num_copies, payload parser handles it on-console.

        pk7.EncryptedPartyData.CopyTo(span[0x30..]); // Copy in pokemon data
        GetRawQR(span[0x140..], pk7.Species, pk7.Form, pk7.IsShiny, pk7.Gender);

        var chk = Checksums.CRC16Invert(span[..0x1A0]);
        WriteUInt16LittleEndian(span[0x1A0..], chk);
    }
}

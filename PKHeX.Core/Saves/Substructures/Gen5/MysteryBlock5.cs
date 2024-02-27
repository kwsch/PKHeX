using System;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

public sealed class MysteryBlock5(SAV5 sav, int offset) : SaveBlock<SAV5>(sav, offset)
{
    private const int FlagStart = 0;
    private const int MaxReceivedFlag = 2048;
    private const int MaxCardsPresent = 12;
    private const int FlagRegionSize = (MaxReceivedFlag / 8); // 0x100
    private const int CardStart = FlagStart + FlagRegionSize;

    private const int DataSize = 0xA90;
    private int SeedOffset => Offset + DataSize;

    // Everything is stored encrypted, and only decrypted on demand. Only crypt on object fetch...

    public EncryptedMysteryGiftAlbum GiftAlbum
    {
        get
        {
            uint seed = ReadUInt32LittleEndian(Data.AsSpan(SeedOffset));
            byte[] wcData = SAV.Data.AsSpan(Offset + FlagStart, 0xA90).ToArray(); // Encrypted, Decrypt
            return GetAlbum(seed, wcData);
        }
        set
        {
            var wcData = SetAlbum(value);
            // Write Back
            wcData.CopyTo(Data, Offset + FlagStart);
            WriteUInt32LittleEndian(Data.AsSpan(SeedOffset), value.Seed);
        }
    }

    private static EncryptedMysteryGiftAlbum GetAlbum(uint seed, byte[] wcData)
    {
        PokeCrypto.CryptArray(wcData, seed);

        var flags = new bool[MaxReceivedFlag];
        var gifts = new DataMysteryGift[MaxCardsPresent];
        var Info = new EncryptedMysteryGiftAlbum(gifts, flags, seed);

        // 0x100 Bytes for Used Flags
        for (int i = 0; i < Info.Flags.Length; i++)
            Info.Flags[i] = ((wcData[i / 8] >> (i % 8)) & 0x1) == 1;
        // 12 PGFs
        for (int i = 0; i < Info.Gifts.Length; i++)
        {
            var data = wcData.AsSpan(CardStart + (i * PGF.Size), PGF.Size).ToArray();
            Info.Gifts[i] = new PGF(data);
        }

        return Info;
    }

    private static byte[] SetAlbum(EncryptedMysteryGiftAlbum value)
    {
        byte[] wcData = new byte[0xA90];

        // Toss back into byte[]
        for (int i = 0; i < value.Flags.Length; i++)
        {
            if (value.Flags[i])
                wcData[i / 8] |= (byte) (1 << (i & 7));
        }

        var span = wcData.AsSpan(CardStart);
        for (int i = 0; i < value.Gifts.Length; i++)
            value.Gifts[i].Data.CopyTo(span[(i * PGF.Size)..]);

        // Decrypted, Encrypt
        PokeCrypto.CryptArray(wcData, value.Seed);
        return wcData;
    }
}

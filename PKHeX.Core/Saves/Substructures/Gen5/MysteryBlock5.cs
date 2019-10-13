using System;

namespace PKHeX.Core
{
    public sealed class MysteryBlock5 : SaveBlock
    {
        private const int FlagStart = 0;
        private const int MaxReceivedFlag = 2048;
        private const int MaxCardsPresent = 12;
        private const int FlagRegionSize = (MaxReceivedFlag / 8); // 0x100
        private const int CardStart = FlagStart + (MaxReceivedFlag / 8);

        private const int DataSize = 0xA90;
        private int SeedOffset => Offset + DataSize;

        // Everything is stored encrypted, and only decrypted on demand. Only crypt on object fetch...
        public MysteryBlock5(SAV5BW sav, int offset) : base(sav) => Offset = offset;
        public MysteryBlock5(SAV5B2W2 sav, int offset) : base(sav) => Offset = offset;

        public EncryptedMysteryGiftAlbum GiftAlbum
        {
            get
            {
                uint seed = BitConverter.ToUInt32(Data, SeedOffset);
                byte[] wcData = SAV.GetData(Offset + FlagStart, 0xA90); // Encrypted, Decrypt
                return GetAlbum(seed, wcData);
            }
            set
            {
                var wcData = SetAlbum(value);
                // Write Back
                wcData.CopyTo(Data, Offset + FlagStart);
                BitConverter.GetBytes(value.Seed).CopyTo(Data, SeedOffset);
            }
        }

        private static EncryptedMysteryGiftAlbum GetAlbum(uint seed, byte[] wcData)
        {
            PKX.CryptArray(wcData, seed);

            var flags = new bool[MaxReceivedFlag];
            var gifts = new DataMysteryGift[MaxCardsPresent];
            var Info = new EncryptedMysteryGiftAlbum(gifts, flags, seed);

            // 0x100 Bytes for Used Flags
            for (int i = 0; i < Info.Flags.Length; i++)
                Info.Flags[i] = (wcData[i / 8] >> i % 8 & 0x1) == 1;
            // 12 PGFs
            for (int i = 0; i < Info.Gifts.Length; i++)
            {
                var data = new byte[PGF.Size];
                Array.Copy(wcData, FlagRegionSize + (i * PGF.Size), data, 0, PGF.Size);
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

            for (int i = 0; i < value.Gifts.Length; i++)
                value.Gifts[i].Data.CopyTo(wcData, 0x100 + (i * PGF.Size));

            // Decrypted, Encrypt
            PKX.CryptArray(wcData, value.Seed);
            return wcData;
        }
    }
}
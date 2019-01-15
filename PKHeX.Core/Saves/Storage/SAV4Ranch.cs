using System;
using System.Security.Cryptography;
using System.Text;

namespace PKHeX.Core
{
    public sealed class SAV4Ranch : BulkStorage
    {
        private const int SIZE_MII = 0x28;
        private const int SIZE_MIILINK = 0x2C;

        public override int SIZE_STORED => 0x88 + 0x1C;
        protected override int SIZE_PARTY => SIZE_STORED;

        public override int BoxCount { get; }
        public override int SlotCount { get; }

        public override string PlayTimeString => SaveUtil.CRC16(Data, 0, Data.Length).ToString("X4");
        protected override string BAKText => $"{OT} {PlayTimeString}";
        public override string Extension => ".bin";
        public override string Filter { get; } = "Ranch G4 Storage|*.bin*";

        public override PKM GetPKM(byte[] data) => new PK4(data);
        public override byte[] DecryptPKM(byte[] data) => PKX.DecryptArray45(data);
        public override StorageSlotFlag GetSlotFlags(int index) => index >= SlotCount ? StorageSlotFlag.Locked : StorageSlotFlag.None;
        protected override bool IsSlotSwapProtected(int box, int slot) => IsSlotOverwriteProtected(box, slot);

        public SAV4Ranch(byte[] data) : base(data, typeof(PK4), 0)
        {
            Personal = PersonalTable.Pt;
            Version = GameVersion.DPPt;
            HeldItems = Legal.HeldItems_Pt;

            OT = GetString(0x770, 0x12);

            // Unpack the binary a little:
            // count, Mii data[count]
            // count, Mii Link data[count]
            // count, Pokemon (PK4 + metadata)[count]

            /* ====Metadata====
             * uint8_t poke_type;// 01 trainer, 04 hayley, 05 traded
             * uint8_t tradeable;// 02 is tradeable, normal 00
             * uint16_t tid;
             * uint16_t sid;
             * uint32_t name1;
             * uint32_t name2;
             * uint32_t name3;
             * uint32_t name4;
             */

            const int miiCountOffset = 0x22AC;
            var miiCount = BigEndian.ToInt32(Data, miiCountOffset);
            var miiLinkCountOffset = miiCountOffset + 4 + (SIZE_MII * miiCount) + 4;
            var miiLinkCount = BigEndian.ToInt32(Data, miiLinkCountOffset);
            var pkCountOffset = miiLinkCountOffset + 4 + (SIZE_MIILINK * miiLinkCount) + 4;

            SlotCount = BigEndian.ToInt32(Data, pkCountOffset);
            BoxCount = (int)Math.Ceiling((decimal)SlotCount / SlotsPerBox);

            Box = pkCountOffset + 4;

            FinalCountOffset = pkCountOffset + 4 + (SIZE_STORED * SlotCount);
            FinalCount = BigEndian.ToInt32(Data, FinalCountOffset);
        }

        private readonly int FinalCount;
        private readonly int FinalCountOffset;

        protected override void SetChecksums()
        {
            BigEndian.GetBytes(FinalCount).CopyTo(Data, FinalCountOffset); // ensure the final data is written if the user screws stuff up
            var goodlen = (FinalCountOffset + 4);
            Array.Clear(Data, goodlen, Data.Length - goodlen);

            // 20 byte SHA checksum at the top of the file, which covers all data that follows.
            using (var hash = SHA1.Create())
            {
                var result = hash.ComputeHash(Data, 20, Data.Length - 20);
                SetData(result, 0);
            }
        }

        public override string GetString(byte[] data, int offset, int length) => Util.TrimFromZero(Encoding.BigEndianUnicode.GetString(data, offset, length));

        public override byte[] SetString(string value, int maxLength, int PadToSize = 0, ushort PadWith = 0)
        {
            if (value.Length > maxLength)
                value = value.Substring(0, maxLength); // Hard cap
            string temp = value
                .PadRight(value.Length + 1, (char)0) // Null Terminator
                .PadRight(PadToSize, (char)PadWith); // Padding
            return Encoding.BigEndianUnicode.GetBytes(temp);
        }
    }
}
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace PKHeX.Core
{
    /// <summary>
    /// Generation 4 <see cref="SaveFile"/> object for My Pokémon Ranch saves.
    /// </summary>
    public sealed class SAV4Ranch : BulkStorage, ISaveFileRevision
    {
        protected override int SIZE_STORED => 0x88 + 0x1C;
        protected override int SIZE_PARTY => SIZE_STORED;

        public int SaveRevision => Version == GameVersion.DP ? 0 : 1;
        public string SaveRevisionString => Version == GameVersion.DP ? "-DP" : "-Pt";

        public override int BoxCount { get; }
        public override int SlotCount { get; }

        public override PersonalTable Personal => PersonalTable.Pt;
        public override IReadOnlyList<ushort> HeldItems => Legal.HeldItems_Pt;
        protected override SaveFile CloneInternal() => new SAV4Ranch((byte[])Data.Clone());
        public override string PlayTimeString => Checksums.CRC16(Data, 0, Data.Length).ToString("X4");
        protected internal override string ShortSummary => $"{OT} {PlayTimeString}";
        public override string Extension => ".bin";

        protected override PKM GetPKM(byte[] data) => new PK4(data);
        protected override byte[] DecryptPKM(byte[] data) => PokeCrypto.DecryptArray45(data);
        public override StorageSlotFlag GetSlotFlags(int index) => index >= SlotCount ? StorageSlotFlag.Locked : StorageSlotFlag.None;
        protected override bool IsSlotSwapProtected(int box, int slot) => IsSlotOverwriteProtected(box, slot);

        public SAV4Ranch(byte[] data) : base(data, typeof(PK4), 0)
        {
            Version = Data.Length == SaveUtil.SIZE_G4RANCH_PLAT ? GameVersion.Pt : GameVersion.DP;

            OT = GetString(0x770, 0x12);

            // 0x18 starts the header table
            // Block 00, Offset = ???
            // Block 01, Offset = Mii Data
            // Block 02, Offset = Mii Link Data
            // Block 03, Offset = Pokemon Data
            // Block 04, Offset = ??

            // Unpack the binary a little:
            // size, count, Mii data[count]
            // size, count, Mii Link data[count]
            // size, count, Pokemon (PK4 + metadata)[count]
            // size, count, ???

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

            var pkCountOffset = BigEndian.ToInt32(Data, 0x34) + 4;
            SlotCount = BigEndian.ToInt32(Data, pkCountOffset);
            BoxCount = (int)Math.Ceiling((decimal)SlotCount / SlotsPerBox);

            Box = pkCountOffset + 4;

            FinalCountOffset = BigEndian.ToInt32(Data, 0x3C);
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
            using var hash = SHA1.Create();
            var result = hash.ComputeHash(Data, 20, Data.Length - 20);
            SetData(result, 0);
        }

        public override string GetString(byte[] data, int offset, int length) => StringConverter4.GetBEString4(data, offset, length);

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
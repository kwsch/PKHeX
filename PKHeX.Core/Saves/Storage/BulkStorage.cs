using System;

namespace PKHeX.Core
{
    /// <summary>
    /// Simple Storage Binary wrapper for a concatenated list of <see cref="PKM"/> data.
    /// </summary>
    public abstract class BulkStorage : SaveFile
    {
        protected BulkStorage(byte[] data, Type t, int start, int slotsPerBox = 30) : base(data)
        {
            Box = start;
            SlotsPerBox = slotsPerBox;

            blank = PKMConverter.GetBlank(t);
            var slots = (Data.Length - Box) / blank.SIZE_STORED;
            BoxCount = slots / SlotsPerBox;

            GetIsPKMPresent = PKX.GetFuncIsPKMPresent(blank);
        }

        protected readonly int SlotsPerBox;

        protected override string BAKText => $"{Checksums.CRC16(Data, Box, Data.Length - Box):X4}";
        public override string Filter { get; } = "All Files|*.*";
        public override string Extension { get; } = ".bin";
        public override bool ChecksumsValid { get; } = true;
        public override string ChecksumInfo { get; } = "No Info.";

        private readonly PKM blank;
        public override Type PKMType => blank.GetType();
        public override PKM BlankPKM => blank.Clone();

        protected override PKM GetPKM(byte[] data) => PKMConverter.GetPKMfromBytes(data, prefer: Generation) ?? blank;
        protected override byte[] DecryptPKM(byte[] data) => GetPKM(data).Data;

        public override int SIZE_STORED => blank.SIZE_STORED;
        protected override int SIZE_PARTY => blank.SIZE_PARTY;
        public override int MaxEV => blank.MaxEV;
        public override int Generation => blank.Format;
        public override int MaxMoveID => blank.MaxMoveID;
        public override int MaxSpeciesID => blank.MaxSpeciesID;
        public override int MaxAbilityID => blank.MaxAbilityID;
        public override int MaxItemID => blank.MaxItemID;
        public override int MaxBallID => blank.MaxBallID;
        public override int MaxGameID => blank.MaxGameID;
        public override int OTLength => blank.OTLength;
        public override int NickLength => blank.NickLength;
        public bool IsBigEndian => blank is BK4 || blank is XK3 || blank is CK3;

        private readonly Func<byte[], int, bool> GetIsPKMPresent;
        public override bool IsPKMPresent(byte[] data, int offset) => GetIsPKMPresent(data, offset);

        public override int BoxCount { get; }
        protected override void SetChecksums() { }

        public override int GetBoxOffset(int box) => Box + (box * (SlotsPerBox * SIZE_STORED));
        public override string GetBoxName(int box) => $"Box {box + 1:d2}";
        public override void SetBoxName(int box, string value) { }
        public override int GetPartyOffset(int slot) => int.MinValue;

        public override string GetString(byte[] data, int offset, int length)
            => StringConverter.GetString(data, Generation, blank.Japanese, IsBigEndian, length, offset);

        public override byte[] SetString(string value, int maxLength, int PadToSize = 0, ushort PadWith = 0)
            => StringConverter.SetString(value, Generation, blank.Japanese, IsBigEndian, maxLength, padTo: PadToSize, padWith: PadWith);
    }
}

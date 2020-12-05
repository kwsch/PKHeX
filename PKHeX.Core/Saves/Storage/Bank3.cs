using System.Collections.Generic;

namespace PKHeX.Core
{
    /// <summary>
    /// Generation 3 <see cref="SaveFile"/> object that reads exported data for Generation 3 PokeStock .gst dumps.
    /// </summary>
    public sealed class Bank3 : BulkStorage
    {
        public Bank3(byte[] data) : base(data, typeof(PK3), 0) => Version = GameVersion.RS;

        public override PersonalTable Personal => PersonalTable.RS;
        public override IReadOnlyList<ushort> HeldItems => Legal.HeldItems_RS;
        protected override SaveFile CloneInternal() => new Bank3((byte[])Data.Clone());
        public override string PlayTimeString => Checksums.CRC16(Data, 0, Data.Length).ToString("X4");
        protected internal override string ShortSummary => PlayTimeString;
        public override string Extension => ".gst";

        public override int BoxCount => 64;
        private const int BoxNameSize = 9;

        private int BoxDataSize => SlotsPerBox * SIZE_STORED;
        public override int GetBoxOffset(int box) => Box + (BoxDataSize * box);
        public override string GetBoxName(int box) => GetString(GetBoxNameOffset(box), BoxNameSize);
        private int GetBoxNameOffset(int box) => 0x25800 + (9 * box);
    }
}
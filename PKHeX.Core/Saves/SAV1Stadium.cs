using System;
using System.Collections.Generic;

namespace PKHeX.Core
{
    public sealed class SAV1Stadium : SaveFile, ILangDeviantSave
    {
        protected override string BAKText => $"{OT} ({Version}) - {PlayTimeString}";
        public override string Filter => "SAV File|*.sav|All Files|*.*";
        public override string Extension => ".sav";

        public int SaveRevision => Japanese ? 0 : 1;
        public string SaveRevisionString => Japanese ? "J" : "U";
        public bool Japanese { get; }
        public bool Korean => false;

        public override PersonalTable Personal => PersonalTable.Y;
        public override int MaxEV => ushort.MaxValue;
        public override IReadOnlyList<ushort> HeldItems => Array.Empty<ushort>();
        public override GameVersion Version { get; protected set; } = GameVersion.Stadium;

        public override SaveFile Clone() => new SAV1Stadium((byte[])Data.Clone(), Japanese);

        public override bool ChecksumsValid => true;
        public override string ChecksumInfo => string.Empty;
        public override int Generation => 1;

        public override string GetString(byte[] data, int offset, int length) => StringConverter12.GetString1(data, offset, length, Japanese);

        public override byte[] SetString(string value, int maxLength, int PadToSize = 0, ushort PadWith = 0)
        {
            if (PadToSize == 0)
                PadToSize = maxLength + 1;
            return StringConverter12.SetString1(value, maxLength, Japanese, PadToSize, PadWith);
        }

        private int StringLength => Japanese ? 5 : 10;
        public override int OTLength => StringLength;
        public override int NickLength => StringLength;
        public override int BoxCount => 84;
        public override int BoxSlotCount => 6;

        public override int MaxMoveID => Legal.MaxMoveID_1;
        public override int MaxSpeciesID => Legal.MaxSpeciesID_1;
        public override int MaxAbilityID => Legal.MaxAbilityID_1;
        public override int MaxItemID => Legal.MaxItemID_1;
        public override int MaxBallID => 0; // unused
        public override int MaxGameID => 99; // unused
        public override int MaxMoney => 999999;
        public override int MaxCoins => 9999;

        public override int GetPartyOffset(int slot) => -1;
        protected override void SetChecksums() { } // todo

        public override Type PKMType => typeof(PK1);

        protected override PKM GetPKM(byte[] data)
        {
            int len = StringLength;
            var nick = data.Slice(0x21, len);
            var ot = data.Slice(0x21 + len, len);
            return new PK1(data, Japanese) {OT_Trash = ot, Nickname_Trash = nick};
        }

        protected override byte[] DecryptPKM(byte[] data) => data;

        public override PKM BlankPKM => new PK1(Japanese);
        protected override int SIZE_STORED => Japanese ? 0x2D : 0x37;
        protected override int SIZE_PARTY => Japanese ? 0x2D : 0x37;

        public SAV1Stadium(byte[] data) : this(data, IsStadiumJ(data)) { }

        public SAV1Stadium(byte[] data, bool japanese) : base(data, false)
        {
            Japanese = japanese;
            Box = 0;
        }

        public SAV1Stadium(bool japanese = false) : base(SaveUtil.SIZE_G1STAD)
        {
            Japanese = japanese;
            Box = 0;
            ClearBoxes();
        }

        private int TeamSize => Japanese ? TeamSizeJ : TeamSizeU;
        private const int TeamSizeU = 0x160;
        private const int TeamSizeJ = 0x128;
        public override int GetBoxOffset(int box) => (box * TeamSize) + 0x10;
        public override string GetBoxName(int box) => $"Team {box + 1}";
        public override void SetBoxName(int box, string value) { }

        public override void WriteSlotFormatStored(PKM pkm, byte[] data, int offset)
        {
            // pkm that have never been boxed have yet to save the 'current level' for box indication
            // set this value at this time
            ((PK1)pkm).Stat_LevelBox = pkm.CurrentLevel;
            base.WriteSlotFormatStored(pkm, Data, offset);
        }

        public override void WriteBoxSlot(PKM pkm, byte[] data, int offset)
        {
            // pkm that have never been boxed have yet to save the 'current level' for box indication
            // set this value at this time
            ((PK1)pkm).Stat_LevelBox = pkm.CurrentLevel;
            base.WriteBoxSlot(pkm, Data, offset);
        }

        public static bool IsStadiumU(byte[] data)
        {
            if (data.Length != SaveUtil.SIZE_G1STAD)
                return false;

            for (int i = 0; i < 10; i++)
            {
                if (BitConverter.ToUInt32(data, 0x15A + (i * TeamSizeU)) != 0x454B4F50) // POKE
                    return false;
            }
            return true;
        }

        public static bool IsStadiumJ(byte[] data)
        {
            if (data.Length != SaveUtil.SIZE_G1STAD)
                return false;

            for (int i = 0; i < 10; i++)
            {
                if (BitConverter.ToUInt32(data, 0x122 + (i * TeamSizeJ)) != 0x454B4F50) // POKE
                    return false;
            }
            return true;
        }
    }
}

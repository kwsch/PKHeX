using System;
using System.Collections.Generic;

namespace PKHeX.Core
{
    /// <summary>
    /// Pocket Monsters Stadium
    /// </summary>
    public class SAV1StadiumJ : SaveFile
    {
        protected override string BAKText => $"{OT} ({Version}) - {PlayTimeString}";
        public override string Filter => "SAV File|*.sav|All Files|*.*";
        public override string Extension => ".sav";

        public override PersonalTable Personal => PersonalTable.Y;
        public override int MaxEV => ushort.MaxValue;
        public override IReadOnlyList<ushort> HeldItems => Array.Empty<ushort>();
        public override GameVersion Version { get; protected set; } = GameVersion.StadiumJ;

        public override SaveFile Clone() => new SAV1StadiumJ((byte[])Data.Clone());

        public override bool ChecksumsValid => true;
        public override string ChecksumInfo => string.Empty;
        public override int Generation => 1;

        public override string GetString(byte[] data, int offset, int length) => StringConverter12.GetString1(data, offset, length, true);

        public override byte[] SetString(string value, int maxLength, int PadToSize = 0, ushort PadWith = 0)
        {
            if (PadToSize == 0)
                PadToSize = maxLength + 1;
            return StringConverter12.SetString1(value, maxLength, true, PadToSize, PadWith);
        }

        private int StringLength => 5;
        public override int OTLength => StringLength;
        public override int NickLength => StringLength;
        public override int BoxCount => 8;
        public override int BoxSlotCount => 30;

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
            return new PK1(data, true) { OT_Trash = ot, Nickname_Trash = nick };
        }

        protected override byte[] DecryptPKM(byte[] data) => data;

        public override PKM BlankPKM => new PK1(true);
        protected override int SIZE_STORED => 0x2D;
        protected override int SIZE_PARTY => 0x2D;

        public SAV1StadiumJ(byte[] data) : base(data, false)
        {
            Box = 0x2500;
        }

        public SAV1StadiumJ() : base(SaveUtil.SIZE_G1STAD)
        {
            Box = 0x2500;
            ClearBoxes();
        }

        private const int TeamSizeJ = 0x128;
        private const int BoxSizeJ = 0x560;
        public override int GetBoxOffset(int box) => (box * BoxSizeJ) + 0x14;
        public override string GetBoxName(int box) => $"Box {box + 1}";
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

        public static bool IsStadiumJ(byte[] data)
        {
            if (data.Length != SaveUtil.SIZE_G1STAD)
                return false;

            for (int i = 0; i < 10; i++)
            {
                if (BitConverter.ToUInt32(data, 0x11A + (i * TeamSizeJ)) != 0x454B4F50) // POKE
                    return false;
            }
            return true;
        }
    }
}

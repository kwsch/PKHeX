using System;
using System.Collections.Generic;

namespace PKHeX.Core
{
    /// <summary>
    /// Pocket Monsters Stadium
    /// </summary>
    public class SAV1StadiumJ : SaveFile, ILangDeviantSave
    {
        protected override string BAKText => $"{OT} ({Version})";
        public override string Filter => "SAV File|*.sav|All Files|*.*";
        public override string Extension => ".sav";

        // Required since PK1 logic comparing a save file assumes the save file can be U/J
        public int SaveRevision => 0;
        public string SaveRevisionString => "0"; // so we're different from Japanese SAV1Stadium naming...
        public bool Japanese => true;
        public bool Korean => false;

        public override PersonalTable Personal => PersonalTable.Y;
        public override int MaxEV => ushort.MaxValue;
        public override IReadOnlyList<ushort> HeldItems => Array.Empty<ushort>();
        public override GameVersion Version { get; protected set; } = GameVersion.StadiumJ;

        public override SaveFile Clone() => new SAV1StadiumJ((byte[])Data.Clone());

        public override string ChecksumInfo => ChecksumsValid ? "Checksum valid." : "Checksum invalid";
        public override int Generation => 1;

        public override string GetString(byte[] data, int offset, int length) => StringConverter12.GetString1(data, offset, length, true);

        public override byte[] SetString(string value, int maxLength, int PadToSize = 0, ushort PadWith = 0)
        {
            if (PadToSize == 0)
                PadToSize = maxLength + 1;
            return StringConverter12.SetString1(value, maxLength, true, PadToSize, PadWith);
        }

        private const int StringLength = 6; // Japanese Only
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

        private readonly bool IsPairSwapped;

        protected override byte[] GetFinalData()
        {
            var result = base.GetFinalData();
            if (IsPairSwapped)
                BigEndian.SwapBytes32(result = (byte[])result.Clone());
            return result;
        }

        public override bool ChecksumsValid => GetBoxChecksumsValid();
        protected override void SetChecksums() => SetBoxChecksums();

        private bool GetBoxChecksumsValid()
        {
            for (int i = 0; i < BoxCount; i++)
            {
                var boxOfs = GetBoxOffset(i) - ListHeaderSize;
                const int size = BoxSizeJ - 2;
                var chk = Checksums.CheckSum16(Data, boxOfs, size);
                var actual = BigEndian.ToUInt16(Data, boxOfs + size);
                if (chk != actual)
                    return false;
            }
            return true;
        }

        private void SetBoxChecksums()
        {
            for (int i = 0; i < BoxCount; i++)
            {
                var boxOfs = GetBoxOffset(i) - ListHeaderSize;
                const int size = BoxSizeJ - 2;
                var chk = Checksums.CheckSum16(Data, boxOfs, size);
                BigEndian.GetBytes(chk).CopyTo(Data, boxOfs + size);
            }
        }

        public override Type PKMType => typeof(PK1);

        protected override PKM GetPKM(byte[] data)
        {
            const int len = StringLength;
            var nick = data.Slice(0x21, len);
            var ot = data.Slice(0x21 + len, len);
            data = data.Slice(0, 0x21);
            return new PK1(data, true) { OT_Trash = ot, Nickname_Trash = nick };
        }

        protected override byte[] DecryptPKM(byte[] data) => data;

        public override PKM BlankPKM => new PK1(true);
        private const int SIZE_PK1J = PokeCrypto.SIZE_1STORED + (2 * StringLength); // 0x2D
        protected override int SIZE_STORED => SIZE_PK1J;
        protected override int SIZE_PARTY => SIZE_PK1J;

        public SAV1StadiumJ(byte[] data) : base(data)
        {
            var swap = StadiumUtil.IsMagicPresentSwap(data, TeamSizeJ, MAGIC_POKE);
            if (swap)
            {
                BigEndian.SwapBytes32(Data);
                IsPairSwapped = true;
            }
            Box = 0x2500;
        }

        public SAV1StadiumJ() : base(SaveUtil.SIZE_G1STAD)
        {
            Box = 0x2500;
            ClearBoxes();
        }

        private const int ListHeaderSize = 0x14;
        private const int ListFooterSize = 6; // POKE + 2byte checksum

        private const int TeamCount = 86; // todo
        private const int TeamSizeJ = 0x14 + (SIZE_PK1J * 6) + ListFooterSize; // 0x128
        public static int GetTeamOffset(int team) => 0 + ListHeaderSize + (team * TeamSizeJ);
        public static string GetTeamName(int team) => $"Team {team + 1}";

        public BattleTeam<PK1> GetTeam(int team)
        {
            if ((uint)team >= TeamCount)
                throw new ArgumentOutOfRangeException(nameof(team));

            var name = GetTeamName(team);
            var members = new PK1[6];
            var ofs = GetTeamOffset(team);
            for (int i = 0; i < 6; i++)
            {
                var rel = ofs + (i * SIZE_STORED);
                members[i] = (PK1)GetStoredSlot(Data, rel);
            }
            return new BattleTeam<PK1>(name, members);
        }

        private const int BoxSizeJ = 0x560;
        public override int GetBoxOffset(int box) => Box + ListHeaderSize + (box * BoxSizeJ);
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

        private const uint MAGIC_POKE = 0x454B4F50;

        public static bool IsStadiumJ(byte[] data)
        {
            if (data.Length != SaveUtil.SIZE_G1STADJ)
                return false;
            return StadiumUtil.IsMagicPresentEither(data, TeamSizeJ, MAGIC_POKE);
        }
    }
}

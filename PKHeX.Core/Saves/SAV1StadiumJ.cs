using System;
using System.Collections.Generic;

namespace PKHeX.Core
{
    /// <summary>
    /// Pocket Monsters Stadium
    /// </summary>
    public sealed class SAV1StadiumJ : SAV_STADIUM
    {
        // Required since PK1 logic comparing a save file assumes the save file can be U/J
        public override int SaveRevision => 0;
        public override string SaveRevisionString => "0"; // so we're different from Japanese SAV1Stadium naming...

        public override PersonalTable Personal => PersonalTable.Y;
        public override int MaxEV => ushort.MaxValue;
        public override IReadOnlyList<ushort> HeldItems => Array.Empty<ushort>();
        public override GameVersion Version { get; protected set; } = GameVersion.StadiumJ;

        protected override SaveFile CloneInternal() => new SAV1StadiumJ((byte[])Data.Clone());

        public override int Generation => 1;
        private const int StringLength = 6; // Japanese Only
        public override int OTLength => StringLength;
        public override int NickLength => StringLength;
        public override int BoxCount => 4; // 8 boxes stored sequentially; latter 4 are backups
        public override int BoxSlotCount => 30;

        public override int MaxMoveID => Legal.MaxMoveID_1;
        public override int MaxSpeciesID => Legal.MaxSpeciesID_1;
        public override int MaxAbilityID => Legal.MaxAbilityID_1;
        public override int MaxItemID => Legal.MaxItemID_1;
        private const int SIZE_PK1J = PokeCrypto.SIZE_1STORED + (2 * StringLength); // 0x2D
        protected override int SIZE_STORED => SIZE_PK1J;
        protected override int SIZE_PARTY => SIZE_PK1J;

        public override Type PKMType => typeof(PK1);
        public override PKM BlankPKM => new PK1(true);

        private const int ListHeaderSize = 0x14;
        private const int ListFooterSize = 6; // POKE + 2byte checksum
        private const uint FOOTER_MAGIC = 0x454B4F50; // POKE

        protected override int TeamCount => 16; // 32 teams stored sequentially; latter 16 are backups
        private const int TeamSizeJ = 0x14 + (SIZE_PK1J * 6) + ListFooterSize; // 0x128
        private const int BoxSizeJ = 0x560;

        public SAV1StadiumJ(byte[] data) : base(data, true, StadiumUtil.IsMagicPresentSwap(data, TeamSizeJ, FOOTER_MAGIC))
        {
            Box = 0x2500;
        }

        public SAV1StadiumJ() : base(true, SaveUtil.SIZE_G1STAD)
        {
            Box = 0x2500;
            ClearBoxes();
        }

        protected override bool GetIsBoxChecksumValid(int i)
        {
            var boxOfs = GetBoxOffset(i) - ListHeaderSize;
            const int size = BoxSizeJ - 2;
            var chk = Checksums.CheckSum16(new ReadOnlySpan<byte>(Data, boxOfs, size));
            var actual = BigEndian.ToUInt16(Data, boxOfs + size);
            return chk == actual;
        }

        protected override void SetBoxChecksum(int i)
        {
            var boxOfs = GetBoxOffset(i) - ListHeaderSize;
            const int size = BoxSizeJ - 2;
            var chk = Checksums.CheckSum16(new ReadOnlySpan<byte>(Data, boxOfs, size));
            BigEndian.GetBytes(chk).CopyTo(Data, boxOfs + size);
        }

        protected override void SetBoxMetadata(int i)
        {
            // Not implemented
        }

        protected override PKM GetPKM(byte[] data)
        {
            const int len = StringLength;
            var nick = data.Slice(0x21, len);
            var ot = data.Slice(0x21 + len, len);
            data = data.Slice(0, 0x21);
            return new PK1(data, true) { OT_Trash = ot, Nickname_Trash = nick };
        }

        public override byte[] GetDataForFormatStored(PKM pkm)
        {
            byte[] result = new byte[SIZE_STORED];
            var gb = (PK1)pkm;

            var data = pkm.Data;
            const int len = StringLength;
            data.CopyTo(result, 0);
            gb.RawNickname.CopyTo(result, PokeCrypto.SIZE_1STORED);
            gb.RawOT.CopyTo(result, PokeCrypto.SIZE_1STORED + len);
            return result;
        }

        public override byte[] GetDataForFormatParty(PKM pkm) => GetDataForFormatStored(pkm);
        public override byte[] GetDataForParty(PKM pkm) => GetDataForFormatStored(pkm);
        public override byte[] GetDataForBox(PKM pkm) => GetDataForFormatStored(pkm);

        public override int GetBoxOffset(int box) => Box + ListHeaderSize + (box * BoxSizeJ);
        public static int GetTeamOffset(int team) => 0 + (team * 2 * TeamSizeJ); // backups are after each team

        public string GetTeamName(int team)
        {
            var name = $"Team {team + 1}";

            var ofs = GetTeamOffset(team);
            var str = GetString(ofs + 2, 5);
            if (string.IsNullOrWhiteSpace(str))
                return name;
            var id = BigEndian.ToUInt16(Data, ofs + 8);
            return $"{name} [{id:D5}:{str}]";
        }

        public override SlotGroup GetTeam(int team)
        {
            if ((uint)team >= TeamCount)
                throw new ArgumentOutOfRangeException(nameof(team));

            var name = GetTeamName(team);
            var members = new PK1[6];
            var ofs = GetTeamOffset(team);
            for (int i = 0; i < 6; i++)
            {
                var rel = ofs + ListHeaderSize + (i * SIZE_STORED);
                members[i] = (PK1)GetStoredSlot(Data, rel);
            }
            return new SlotGroup(name, members);
        }

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

        public static bool IsStadium(byte[] data)
        {
            if (data.Length != SaveUtil.SIZE_G1STADJ)
                return false;
            return StadiumUtil.IsMagicPresentEither(data, TeamSizeJ, FOOTER_MAGIC);
        }
    }
}

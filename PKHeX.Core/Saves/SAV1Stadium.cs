using System;
using System.Collections.Generic;

namespace PKHeX.Core
{
    public sealed class SAV1Stadium : SaveFile, ILangDeviantSave
    {
        protected override string BAKText => $"{OT} ({Version})";
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

        public override string ChecksumInfo => ChecksumsValid ? "Checksum valid." : "Checksum invalid";
        public override int Generation => 1;

        public override string GetString(byte[] data, int offset, int length) => StringConverter12.GetString1(data, offset, length, Japanese);

        public override byte[] SetString(string value, int maxLength, int PadToSize = 0, ushort PadWith = 0)
        {
            if (PadToSize == 0)
                PadToSize = maxLength + 1;
            return StringConverter12.SetString1(value, maxLength, Japanese, PadToSize, PadWith);
        }

        private int StringLength => Japanese ? StringLengthJ : StringLengthU;
        private const int StringLengthJ = 6;
        private const int StringLengthU = 11;
        public override int OTLength => StringLength;
        public override int NickLength => StringLength;
        public override int BoxCount => Japanese ? 8 : 12;
        public override int BoxSlotCount => Japanese ? 30 : 20;

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
                var size = BoxSize - 2;
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
                var size = BoxSize - 2;
                var chk = Checksums.CheckSum16(Data, boxOfs, size);
                BigEndian.GetBytes(chk).CopyTo(Data, boxOfs + size);
            }
        }

        public override Type PKMType => typeof(PK1);

        protected override PKM GetPKM(byte[] data)
        {
            int len = StringLength;
            var nick = data.Slice(PokeCrypto.SIZE_1STORED, len);
            var ot = data.Slice(PokeCrypto.SIZE_1STORED + len, len);
            data = data.Slice(0, PokeCrypto.SIZE_1STORED);
            return new PK1(data, Japanese) {OT_Trash = ot, Nickname_Trash = nick};
        }

        protected override byte[] DecryptPKM(byte[] data) => data;

        public override PKM BlankPKM => new PK1(Japanese);
        private const int SIZE_PK1J = PokeCrypto.SIZE_1STORED + (2 * StringLengthJ); // 0x2D
        private const int SIZE_PK1U = PokeCrypto.SIZE_1STORED + (2 * StringLengthU); // 0x37
        protected override int SIZE_STORED => Japanese ? SIZE_PK1J : SIZE_PK1U;
        protected override int SIZE_PARTY => Japanese ? SIZE_PK1J : SIZE_PK1U;

        public SAV1Stadium(byte[] data) : this(data, IsStadiumJ(data)) { }

        public SAV1Stadium(byte[] data, bool japanese) : base(data)
        {
            Japanese = japanese;
            var swap = StadiumUtil.IsMagicPresentSwap(data, TeamSize, MAGIC_POKE);
            if (swap)
            {
                BigEndian.SwapBytes32(Data);
                IsPairSwapped = true;
            }
            Box = 0xC000;
        }

        public SAV1Stadium(bool japanese = false) : base(SaveUtil.SIZE_G1STAD)
        {
            Japanese = japanese;
            Box = 0xC000;
            ClearBoxes();
        }

        private int ListHeaderSize => Japanese ? 0x0C : 0x10;
        private const int ListFooterSize = 6; // POKE + 2byte checksum

        private const int TeamCountU = 10;
        private const int TeamCountJ = 12;
        private const int TeamCountTypeU = 6;
        private const int TeamCountTypeJ = 8;
        private int TeamCount => Japanese ? TeamCountJ * TeamCountTypeJ : TeamCountU * TeamCountTypeU;
        private int TeamSize => Japanese ? TeamSizeJ : TeamSizeU;
        private const int TeamSizeJ = 0x0C + (SIZE_PK1J * 6) + ListFooterSize; // 0x120
        private const int TeamSizeU = 0x10 + (SIZE_PK1U * 6) + ListFooterSize; // 0x160

        public int GetTeamOffset(int team) => Japanese ? GetTeamOffsetJ(team) : GetTeamOffsetU(team);

        private int GetTeamOffsetJ(int team)
        {
            if ((uint) team > TeamCount)
                throw new ArgumentOutOfRangeException(nameof(team));
            return GetTeamTypeOffsetJ(team / TeamCountJ) + (TeamSizeJ * (team % TeamCountJ));
        }

        private int GetTeamOffsetU(int team)
        {
            if ((uint)team > TeamCount)
                throw new ArgumentOutOfRangeException(nameof(team));
            return GetTeamTypeOffsetU(team / TeamCountU) + (TeamSizeU * (team % TeamCountU));
        }

        private static int GetTeamTypeOffsetJ(int team) => team switch
        {
            0 => 0x0000, // Anything Goes
            1 => 0x0D80, // Nintendo Cup '97
            2 => 0x1B00, // Nintendo Cup '98
            3 => 0x2880, // Nintendo Cup '99
            4 => 0x4000, // Petit Cup
            5 => 0x4D80, // Pika Cup
            6 => 0x5B00, // Prime Cup
            7 => 0x6880, // Gym Leader Castle
            8 => 0x8000, // Vs. Mewtwo
            _ => throw new ArgumentOutOfRangeException(nameof(team)),
        };

        private static int GetTeamTypeOffsetU(int team) => team switch
        {
            0 => 0x0000, // Anything Goes
            1 => 0x0D80, // Unused
            2 => 0x1B00, // Unused
            3 => 0x2880, // Poke Cup
            4 => 0x4000, // Petit Cup
            5 => 0x4D80, // Pika Cup
            6 => 0x5B00, // Prime Cup
            7 => 0x6880, // Gym Leader Castle
            8 => 0x8000, // Vs. Mewtwo
            _ => throw new ArgumentOutOfRangeException(nameof(team)),
        };

        public int GetTeamOffset(Stadium2TeamType type, int team)
        {
            if (Japanese)
               return GetTeamTypeOffsetJ((int)type) + (TeamSizeJ * team);
            return GetTeamTypeOffsetU((int)type) + (TeamSizeU * team);
        }

        public string GetTeamName(int team)
        {
            if ((uint)team >= TeamCount)
                throw new ArgumentOutOfRangeException(nameof(team));

            var teamsPerType = Japanese ? TeamCountJ : TeamCountU;
            var type = team / teamsPerType;
            var index = team % teamsPerType;
            return $"{GetTeamTypeName(type)} {index + 1}";
        }

        private string GetTeamTypeName(int type)
        {
            if (Japanese)
                return ((Stadium1TeamType) type).ToString();
            return type switch
            {
                1 => "Unused1",
                2 => "Unused2",
                3 => "PokeCup",
                _ => ((Stadium1TeamType)type).ToString(),
            };
        }

        public BattleTeam<PK1>[] GetRegisteredTeams()
        {
            var result = new BattleTeam<PK1>[TeamCount];
            for (int i = 0; i < result.Length; i++)
                result[i] = GetTeam(i);
            return result;
        }

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

        private int BoxSize => Japanese ? BoxSizeJ : BoxSizeU;
        private const int BoxSizeJ = 0x0C + (SIZE_PK1J * 30) + ListFooterSize; // 0x558
        private const int BoxSizeU = 0x10 + (SIZE_PK1U * 20) + 6 + ListFooterSize; // 0x468 (6 bytes alignment)
        public override int GetBoxOffset(int box) => Box + ListHeaderSize + (box * BoxSize);
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

        public static bool IsStadiumU(byte[] data)
        {
            if (data.Length != SaveUtil.SIZE_G1STAD)
                return false;
            return StadiumUtil.IsMagicPresentEither(data, TeamSizeU, MAGIC_POKE);
        }

        public static bool IsStadiumJ(byte[] data)
        {
            if (data.Length != SaveUtil.SIZE_G1STAD)
                return false;
            return StadiumUtil.IsMagicPresentEither(data, TeamSizeJ, MAGIC_POKE);
        }
    }

    public enum Stadium1TeamType
    {
        AnythingGoes = 0,
        NintendoCup97 = 1, // unused in non-JP
        NintendoCup98 = 2, // unused in non-JP
        NintendoCup99 = 3, // Poke Cup in non-JP
        PetitCup = 4,
        PikaCup = 5,
        PrimeCup = 6,
        GymLeaderCastle = 7,
        VsMewtwo = 8,
    }

    public class BattleTeam<T> where T : PKM
    {
        public readonly string TeamName;
        public readonly T[] Team;

        public BattleTeam(string name, T[] team)
        {
            TeamName = name;
            Team = team;
        }
    }
}

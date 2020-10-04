using System;
using System.Collections.Generic;

namespace PKHeX.Core
{
    public sealed class SAV2Stadium : SaveFile, ILangDeviantSave
    {
        protected override string BAKText => $"{OT} ({Version})";
        public override string Filter => "SAV File|*.sav|All Files|*.*";
        public override string Extension => ".sav";

        public int SaveRevision => Japanese ? 0 : 1;
        public string SaveRevisionString => Japanese ? "J" : "U";
        public bool Japanese { get; }
        public bool Korean => false;

        public override PersonalTable Personal => PersonalTable.C;
        public override int MaxEV => ushort.MaxValue;
        public override IReadOnlyList<ushort> HeldItems => Legal.HeldItems_GSC;
        public override GameVersion Version { get; protected set; } = GameVersion.Stadium2;

        public override SaveFile Clone() => new SAV1Stadium((byte[])Data.Clone(), Japanese);

        public override string ChecksumInfo => ChecksumsValid ? "Checksum valid." : "Checksum invalid";
        public override int Generation => 2;

        public override string GetString(byte[] data, int offset, int length) => StringConverter12.GetString1(data, offset, length, Japanese);

        public override byte[] SetString(string value, int maxLength, int PadToSize = 0, ushort PadWith = 0)
        {
            if (PadToSize == 0)
                PadToSize = maxLength + 1;
            return StringConverter12.SetString1(value, maxLength, Japanese, PadToSize, PadWith);
        }

        private const int StringLength = 12;
        public override int OTLength => StringLength;
        public override int NickLength => StringLength;
        public override int BoxCount => Japanese ? 9 : 14;
        public override int BoxSlotCount => Japanese ? 30 : 20;

        public override int MaxMoveID => Legal.MaxMoveID_2;
        public override int MaxSpeciesID => Legal.MaxSpeciesID_2;
        public override int MaxAbilityID => Legal.MaxAbilityID_2;
        public override int MaxItemID => Legal.MaxItemID_2;
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
                var boxOfs = GetBoxOffset(i) - ListHeaderSizeBox;
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
                var boxOfs = GetBoxOffset(i) - ListHeaderSizeBox;
                var size = BoxSize - 2;
                var chk = Checksums.CheckSum16(Data, boxOfs, size);
                BigEndian.GetBytes(chk).CopyTo(Data, boxOfs + size);
            }
        }

        public override Type PKMType => typeof(SK2);
        protected override PKM GetPKM(byte[] data) => new SK2(data, Japanese);
        protected override byte[] DecryptPKM(byte[] data) => data;

        public override PKM BlankPKM => new SK2(Japanese);
        private const int SIZE_SK2 = PokeCrypto.SIZE_2STADIUM; // 60
        protected override int SIZE_STORED => SIZE_SK2;
        protected override int SIZE_PARTY => SIZE_SK2;

        public SAV2Stadium(byte[] data) : this(data, IsJapanese(data)) { }

        public SAV2Stadium(byte[] data, bool japanese) : base(data)
        {
            var swap = StadiumUtil.IsMagicPresentSwap(data, TeamSize, MAGIC_POKE);
            if (swap)
            {
                BigEndian.SwapBytes32(Data);
                IsPairSwapped = true;
            }
            Japanese = japanese;
            Box = BoxStart;
        }

        public SAV2Stadium(bool japanese = false) : base(SaveUtil.SIZE_G1STAD)
        {
            Japanese = japanese;
            Box = BoxStart;
            ClearBoxes();
        }

        private const int ListHeaderSizeTeam = 0x10;
        private const int ListHeaderSizeBox = 0x20;
        private const int ListFooterSize = 6; // POKE + 2byte checksum

        private const int TeamCount = 60;
        private const int TeamCountType = 10;
        private const int TeamSize = ListHeaderSizeTeam + (SIZE_SK2 * 6) + 2 + ListFooterSize; // 0x180

        public static int GetTeamOffset(Stadium2TeamType type, int team)
        {
            if ((uint)team >= TeamCountType)
                throw new ArgumentOutOfRangeException(nameof(team));

            var index = (TeamCountType * (int)type) + team;
            return GetTeamOffset(index);
        }

        public static int GetTeamOffset(int team)
        {
            if (team < 40)
                return 0 + (team * TeamSize);
            // Teams 41-60 are in a separate chunk
            return 0x4000 + ((team - 40) * TeamSize);
        }

        public static string GetTeamName(int team) => $"{((Stadium2TeamType)(team / TeamCountType)).ToString().Replace('_', ' ')} {(team % 10) + 1}";

        public SlotGroup[] GetRegisteredTeams()
        {
            var result = new SlotGroup[TeamCount];
            for (int i = 0; i < result.Length; i++)
                result[i] = GetTeam(i);
            return result;
        }

        public SlotGroup GetTeam(int team)
        {
            if ((uint)team >= TeamCount)
                throw new ArgumentOutOfRangeException(nameof(team));

            var name = GetTeamName(team);
            var members = new SK2[6];
            var ofs = GetTeamOffset(team);
            for (int i = 0; i < 6; i++)
            {
                var rel = ofs + ListHeaderSizeTeam + (i * SIZE_STORED);
                members[i] = (SK2)GetStoredSlot(Data, rel);
            }
            return new SlotGroup(name, members);
        }

        private int BoxSize => Japanese ? BoxSizeJ : BoxSizeU;
        private const int BoxSizeJ = ListHeaderSizeBox + (SIZE_SK2 * 30) + 2 + ListFooterSize; // 0x730
        private const int BoxSizeU = ListHeaderSizeBox + (SIZE_SK2 * 20) + 2 + ListFooterSize; // 0x4D8

        // Box 1 is stored separately from the remainder of the boxes.
        private const int BoxStart = 0x5E00; // Box 1
        private const int BoxContinue = 0x8000; // Box 2+

        public override int GetBoxOffset(int box)
        {
            if (box == 0)
                return BoxStart + ListHeaderSizeBox;
            return BoxContinue + ListHeaderSizeBox + ((box - 1) * BoxSize);
        }

        public override string GetBoxName(int box) => $"Box {box + 1}";
        public override void SetBoxName(int box, string value) { }

        private const uint MAGIC_POKE = 0x30763350; // P3v0

        public static bool IsStadium(byte[] data)
        {
            if (data.Length != SaveUtil.SIZE_G1STAD)
                return false;
            return StadiumUtil.IsMagicPresentEither(data, TeamSize, MAGIC_POKE);
        }

        // Check Box 1's footer magic.
        private static bool IsJapanese(byte[] data) => StadiumUtil.IsMagicPresentAbsolute(data, BoxStart + BoxSizeJ - ListFooterSize, MAGIC_POKE);
    }

    public enum Stadium2TeamType
    {
        Anything_Goes = 0,
        Little_Cup = 1,
        Poké_Cup = 2,
        Prime_Cup = 3,
        GymLeader_Castle = 4,
        Vs_Rival = 5,
    }
}

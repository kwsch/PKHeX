namespace PKHeX.Core
{
    /// <summary>
    /// Generation 8 <see cref="SaveFile"/> object for <see cref="GameVersion.SWSH"/> games.
    /// </summary>
    public sealed class SAV8SWSH : SAV8
    {
        public SAV8SWSH(byte[] data) : base(data, BlocksSWSH, boSWSH) => Initialize();
        public SAV8SWSH() : base(SaveUtil.SIZE_G8SWSH, BlocksSWSH, boSWSH) => Initialize();

        public override SaveFile Clone() => new SAV8SWSH((byte[])Data.Clone());
        public override int MaxMoveID => Legal.MaxMoveID_8;
        public override int MaxSpeciesID => Legal.MaxSpeciesID_8;
        public override int MaxItemID => Legal.MaxItemID_8;
        public override int MaxBallID => Legal.MaxBallID_8;
        public override int MaxGameID => Legal.MaxGameID_8;
        public override int MaxAbilityID => Legal.MaxAbilityID_8;

        private const int boSWSH = -1;

        private static readonly BlockInfo[] BlocksSWSH =
        {
            new BlockInfo7b(boSWSH, 00, 0x00000, 0x00000),
            new BlockInfo7b(boSWSH, 01, 0x00000, 0x00000),
            new BlockInfo7b(boSWSH, 02, 0x00000, 0x00000),
            new BlockInfo7b(boSWSH, 03, 0x00000, 0x00000),
            new BlockInfo7b(boSWSH, 04, 0x00000, 0x00000),
            new BlockInfo7b(boSWSH, 04, 0x00000, 0x00000),
            new BlockInfo7b(boSWSH, 05, 0x00000, 0x00000),
            new BlockInfo7b(boSWSH, 06, 0x00000, 0x00000),
            new BlockInfo7b(boSWSH, 07, 0x00000, 0x00000),
            new BlockInfo7b(boSWSH, 08, 0x00000, 0x00000),
            new BlockInfo7b(boSWSH, 09, 0x00000, 0x00000),
            new BlockInfo7b(boSWSH, 10, 0x00000, 0x00000),
            new BlockInfo7b(boSWSH, 11, 0x00000, 0x00000),
            new BlockInfo7b(boSWSH, 12, 0x00000, 0x00000),
            new BlockInfo7b(boSWSH, 13, 0x00000, 0x00000),
            new BlockInfo7b(boSWSH, 14, 0x00000, 0x00000),
            new BlockInfo7b(boSWSH, 14, 0x00000, 0x00000),
            new BlockInfo7b(boSWSH, 15, 0x00000, 0x00000),
            new BlockInfo7b(boSWSH, 16, 0x00000, 0x00000),
            new BlockInfo7b(boSWSH, 17, 0x00000, 0x00000),
            new BlockInfo7b(boSWSH, 18, 0x00000, 0x00000),
            new BlockInfo7b(boSWSH, 19, 0x00000, 0x00000),
            new BlockInfo7b(boSWSH, 20, 0x00000, 0x00000),
        };

        private void Initialize()
        {
            Personal = PersonalTable.SWSH;
        }
    }
}
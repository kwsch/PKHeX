using System;
using static PKHeX.Core.GameVersion;

namespace PKHeX.Core
{
    public static class GameData
    {
        public static Learnset[] GetLearnsets(GameVersion game) => Learnsets(game);
        public static PersonalTable GetPersonal(GameVersion game) => Personal(game);

        public static Learnset GetLearnset(GameVersion game, int species, int form)
        {
            var pt = Personal(game);
            var index = pt.GetFormIndex(species, form);
            var sets = Learnsets(game);
            return sets[index];
        }

        private static Learnset[] Learnsets(GameVersion game) => game switch
        {
            RD or GN or BU or RB => Legal.LevelUpRB,
            YW or RBY => Legal.LevelUpY,
            GD or SV or GS => Legal.LevelUpGS,
            C or GSC => Legal.LevelUpC,

            R or S or RS or RSE => Legal.LevelUpRS,
            E or COLO or XD or FRLG or CXD => Legal.LevelUpE,
            FR => Legal.LevelUpFR,
            LG => Legal.LevelUpLG,

            D or P or DP => Legal.LevelUpDP,
            Pt or DPPt => Legal.LevelUpPt,
            HG or SS or HGSS => Legal.LevelUpHGSS,

            B or W or BW => Legal.LevelUpBW,
            B2 or W2 or B2W2 => Legal.LevelUpB2W2,

            X or Y or XY => Legal.LevelUpXY,
            AS or OR or ORAS => Legal.LevelUpAO,

            SN or MN or SM => Legal.LevelUpSM,
            US or UM or USUM => Legal.LevelUpUSUM,
            GO or GP or GE or GG => Legal.LevelUpGG,

            SW or SH or SWSH => Legal.LevelUpSWSH,

            Gen1 => Legal.LevelUpY,
            Gen2 => Legal.LevelUpC,
            Gen3 => Legal.LevelUpE,
            Gen4 => Legal.LevelUpHGSS,
            Gen5 => Legal.LevelUpB2W2,
            Gen6 => Legal.LevelUpAO,
            Gen7 => Legal.LevelUpSM,
            Gen7b => Legal.LevelUpGG,
            Gen8 => Legal.LevelUpSWSH,

            Stadium => Legal.LevelUpY,
            Stadium2 => Legal.LevelUpGS,
            _ => throw new ArgumentOutOfRangeException(nameof(game))
        };

        private static PersonalTable Personal(GameVersion game) => game switch
        {
            RD or GN or BU or RB => PersonalTable.RB,
            YW or RBY => PersonalTable.Y,
            GD or SV or GS => PersonalTable.GS,
            C or GSC => PersonalTable.C,

            R or S or RS or RSE => PersonalTable.RS,
            E or COLO or XD or FRLG or CXD => PersonalTable.E,
            FR => PersonalTable.FR,
            LG => PersonalTable.LG,

            D or P or DP => PersonalTable.DP,
            Pt or DPPt => PersonalTable.Pt,
            HG or SS or HGSS => PersonalTable.HGSS,

            B or W or BW => PersonalTable.BW,
            B2 or W2 or B2W2 => PersonalTable.B2W2,

            X or Y or XY => PersonalTable.XY,
            AS or OR or ORAS => PersonalTable.AO,

            SN or MN or SM => PersonalTable.SM,
            US or UM or USUM => PersonalTable.USUM,
            GO or GP or GE or GG => PersonalTable.GG,

            SW or SH or SWSH => PersonalTable.SWSH,

            Gen1 => PersonalTable.Y,
            Gen2 => PersonalTable.C,
            Gen3 => PersonalTable.E,
            Gen4 => PersonalTable.HGSS,
            Gen5 => PersonalTable.B2W2,
            Gen6 => PersonalTable.AO,
            Gen7 => PersonalTable.USUM,
            Gen7b => PersonalTable.GG,
            Gen8 => PersonalTable.SWSH,

            Stadium => PersonalTable.Y,
            Stadium2 => PersonalTable.GS,

            _ => throw new ArgumentOutOfRangeException(nameof(game))
        };
    }
}

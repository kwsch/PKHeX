using System;
using System.Collections.Generic;

using static PKHeX.Core.Legal;
using static PKHeX.Core.GameVersion;

namespace PKHeX.Core;

public static class MoveLevelUp
{
    private static readonly LearnLookup
        LearnLA   = new(PersonalTable.LA,   LevelUpLA),
        LearnBDSP = new(PersonalTable.BDSP, LevelUpBDSP),
        LearnSWSH = new(PersonalTable.SWSH, LevelUpSWSH),
        LearnSM   = new(PersonalTable.SM,   LevelUpSM),
        LearnUSUM = new(PersonalTable.USUM, LevelUpUSUM),
        LearnGG   = new(PersonalTable.GG,   LevelUpGG),
        LearnXY   = new(PersonalTable.XY,   LevelUpXY),
        LearnAO   = new(PersonalTable.AO,   LevelUpAO),
        LearnBW   = new(PersonalTable.BW,   LevelUpBW),
        LearnB2W2 = new(PersonalTable.B2W2, LevelUpB2W2),
        LearnDP   = new(PersonalTable.DP,   LevelUpDP),
        LearnPt   = new(PersonalTable.Pt,   LevelUpPt),
        LearnHGSS = new(PersonalTable.HGSS, LevelUpHGSS),
        LearnRSE  = new(PersonalTable.RS,   LevelUpRS),
        LearnFRLG = new(PersonalTable.LG,   LevelUpLG),
        LearnGS   = new(PersonalTable.GS,   LevelUpGS),
        LearnC    = new(PersonalTable.C,    LevelUpC),
        LearnRB   = new(PersonalTable.RB,   LevelUpRB),
        LearnY    = new(PersonalTable.Y,    LevelUpY);

    private static Learnset? GetDeoxysLearn3(int form, GameVersion ver = Any)
    {
        const int index = (int)Species.Deoxys;
        if (ver != Any && Gen3.Contains(ver))
            return GameData.GetLearnsets(ver)[index];

        return form switch
        {
            0 => LevelUpRS[index], // Normal
            1 => LevelUpFR[index], // Attack
            2 => LevelUpLG[index], // Defense
            3 => LevelUpE[index], // Speed
            _ => null,
        };
    }

    public static IEnumerable<int> GetMovesLevelUp(PKM pk, int species, int form, int maxLevel, int minlvlG1, int minlvlG2, GameVersion version, bool MoveReminder, int generation)
    {
        var restrict = pk.IsMovesetRestricted();
        if (restrict.IsRestricted)
            version = restrict.Game;

        return generation switch
        {
            1 => GetMovesLevelUp1(species, form, maxLevel, minlvlG1, version),
            2 => GetMovesLevelUp2(species, form, maxLevel, minlvlG2, pk.Korean, pk.LearnMovesNew2Disallowed(), version),
            3 => GetMovesLevelUp3(species, form, maxLevel, version),
            4 => GetMovesLevelUp4(species, form, maxLevel, version),
            5 => GetMovesLevelUp5(species, form, maxLevel, version),
            6 => GetMovesLevelUp6(species, form, maxLevel, version),
            7 => GetMovesLevelUp7(species, form, maxLevel, MoveReminder, pk.LGPE || pk.GO ? (GameVersion)pk.Version : version),
            8 => GetMovesLevelUp8(species, form, maxLevel, version),
            _ => Array.Empty<int>(),
        };
    }

    private static bool LearnMovesNew2Disallowed(this PKM pk) => pk.Format == 1 || (pk.Format >= 7 && pk.VC1);

    internal static List<int> GetMovesLevelUp1(int species, int form, int maxLevel, int minLevel, GameVersion ver = Any)
    {
        return AddMovesLevelUp1(new List<int>(), ver, species, form, maxLevel, minLevel);
    }

    private static List<int> GetMovesLevelUp2(int species, int form, int maxLevel, int minLevel, bool korean, bool removeNewGSCMoves, GameVersion ver = Any)
    {
        var moves = AddMovesLevelUp2(new List<int>(), ver, species, form, maxLevel, minLevel, korean);
        if (removeNewGSCMoves)
            moves.RemoveAll(m => m > MaxMoveID_1);
        return moves;
    }

    private static List<int> GetMovesLevelUp3(int species, int form, int maxLevel, GameVersion ver = Any)
    {
        return AddMovesLevelUp3(new List<int>(), ver, species, form, maxLevel);
    }

    private static List<int> GetMovesLevelUp4(int species, int form, int maxLevel, GameVersion ver = Any)
    {
        return AddMovesLevelUp4(new List<int>(), ver, species, form, maxLevel);
    }

    private static List<int> GetMovesLevelUp5(int species, int form, int maxLevel, GameVersion ver = Any)
    {
        return AddMovesLevelUp5(new List<int>(), ver, species, form, maxLevel);
    }

    private static List<int> GetMovesLevelUp6(int species, int form, int maxLevel, GameVersion ver = Any)
    {
        return AddMovesLevelUp6(new List<int>(), ver, species, form, maxLevel);
    }

    private static List<int> GetMovesLevelUp7(int species, int form, int maxLevel, bool MoveReminder, GameVersion ver = Any)
    {
        return AddMovesLevelUp7(new List<int>(), ver, species, form, maxLevel, MoveReminder);
    }

    private static List<int> GetMovesLevelUp8(int species, int form, int maxLevel, GameVersion ver = Any)
    {
        return AddMovesLevelUp8(new List<int>(), ver, species, form, maxLevel);
    }

    private static List<int> AddMovesLevelUp1(List<int> moves, GameVersion ver, int species, int form, int max, int min)
    {
        switch (ver)
        {
            case Any: case RBY:
                LearnRB.AddMoves1(moves, species, form, max, min);
                return LearnY.AddMoves1(moves, species, form, max, min);

            case RD or BU or GN or RB:
                return LearnRB.AddMoves1(moves, species, form, max, min);
            case YW:
                return LearnY.AddMoves1(moves, species, form, max, min);
        }
        return moves;
    }

    private static List<int> AddMovesLevelUp2(List<int> moves, GameVersion ver, int species, int form, int max, int min, bool korean)
    {
        switch (ver)
        {
            case Any: case GSC:
                LearnGS.AddMoves(moves, species, form, max, min);
                if (korean)
                    return moves;
                return LearnC.AddMoves(moves, species, form, max, min);

            case GD or SI or GS:
                return LearnGS.AddMoves(moves, species, form, max, min);
            case C when !korean:
                return LearnC.AddMoves(moves, species, form, max, min);
        }
        return moves;
    }

    private static List<int> AddMovesLevelUp3(List<int> moves, GameVersion ver, int species, int form, int maxLevel)
    {
        if (species == (int)Species.Deoxys)
        {
            var learn = GetDeoxysLearn3(form, ver);
            if (learn != null)
                moves.AddRange(learn.GetMoves(maxLevel));
            return moves;
        }

        // Emerald level up tables are equal to R/S level up tables
        switch (ver)
        {
            case Any:
                LearnRSE.AddMoves(moves, species, form, maxLevel);
                return LearnFRLG.AddMoves(moves, species, form, maxLevel);

            case R or S or E or RS or RSE:
                return LearnRSE.AddMoves(moves, species, form, maxLevel);
            case FR or LG or FRLG:
                return LearnFRLG.AddMoves(moves, species, form, maxLevel);
        }
        return moves;
    }

    private static List<int> AddMovesLevelUp4(List<int> moves, GameVersion ver, int species, int form, int maxLevel)
    {
        switch (ver)
        {
            case Any: case DPPt:
                LearnDP.AddMoves(moves, species, form, maxLevel);
                LearnPt.AddMoves(moves, species, form, maxLevel);
                if (ver == DPPt) // stop here
                    return moves;
                return LearnHGSS.AddMoves(moves, species, form, maxLevel);

            case D or P or DP:
                return LearnDP.AddMoves(moves, species, form, maxLevel);
            case Pt:
                return LearnPt.AddMoves(moves, species, form, maxLevel);
            case HG or SS or HGSS:
                return LearnHGSS.AddMoves(moves, species, form, maxLevel);
        }
        return moves;
    }

    private static List<int> AddMovesLevelUp5(List<int> moves, GameVersion ver, int species, int form, int maxLevel)
    {
        switch (ver)
        {
            case Any:
                if (species != 646) // Kyurem moves are same for both versions, but form movepool not present.
                    LearnBW.AddMoves(moves, species, form, maxLevel);
                return LearnB2W2.AddMoves(moves, species, form, maxLevel);

            case B or W or BW:
                return LearnBW.AddMoves(moves, species, form, maxLevel);
            case B2 or W2 or B2W2:
                return LearnB2W2.AddMoves(moves, species, form, maxLevel);
        }
        return moves;
    }

    private static List<int> AddMovesLevelUp6(List<int> moves, GameVersion ver, int species, int form, int maxLevel)
    {
        switch (ver)
        {
            case Any:
                LearnXY.AddMoves(moves, species, form, maxLevel);
                return LearnAO.AddMoves(moves, species, form, maxLevel);

            case X or Y or XY:
                return LearnXY.AddMoves(moves, species, form, maxLevel);
            case AS or OR or ORAS:
                return LearnAO.AddMoves(moves, species, form, maxLevel);
        }
        return moves;
    }

    private static List<int> AddMovesLevelUp7(List<int> moves, GameVersion ver, int species, int form, int maxLevel, bool reminder)
    {
        if (reminder)
            maxLevel = 100; // Move reminder can teach any level in movepool now!
        switch (ver)
        {
            case GP or GE or GG or GO:
                return LearnGG.AddMoves(moves, species, form, maxLevel);

            case Any:
                if (species > MaxSpeciesID_7_USUM)
                    return moves;
                LearnUSUM.AddMoves(moves, species, form, maxLevel);
                if (species > MaxSpeciesID_7)
                    return moves;
                return LearnSM.AddMoves(moves, species, form, maxLevel);

            case SN or MN or SM:
                if (species > MaxSpeciesID_7)
                    return moves;
                return LearnSM.AddMoves(moves, species, form, maxLevel);

            case US or UM or USUM:
                if (species > MaxSpeciesID_7_USUM)
                    return moves;
                LearnUSUM.AddMoves(moves, species, form, maxLevel);
                break;
        }
        return moves;
    }

    private static List<int> AddMovesLevelUp8(List<int> moves, GameVersion ver, int species, int form, int maxLevel)
    {
        // Move reminder can NOT teach any level like Gen7
        switch (ver)
        {
            case Any:
            case GO:
            case SW or SH or SWSH:
                if (species > MaxSpeciesID_8)
                    return moves;
                return LearnSWSH.AddMoves(moves, species, form, maxLevel);

            case PLA:
                if (species > MaxSpeciesID_8a)
                    return moves;
                return LearnLA.AddMoves(moves, species, form, maxLevel);

            case BD or SP or BDSP:
                if (species > MaxSpeciesID_8b)
                    return moves;
                return LearnBDSP.AddMoves(moves, species, form, maxLevel);
        }
        return moves;
    }

    public static int[] GetEncounterMoves(PKM pk, int level, GameVersion version)
    {
        if (version <= 0)
            version = (GameVersion)pk.Version;
        return GetEncounterMoves(pk.Species, pk.Form, level, version);
    }

    private static int[] GetEncounterMoves1(int species, int level, GameVersion version)
    {
        var learn = GameData.GetLearnsets(version);
        var table = GameData.GetPersonal(version);
        var index = table.GetFormIndex(species, 0);
        var lvl0 = (int[])((PersonalInfoG1) table[index]).Moves.Clone();
        int start = Math.Max(0, Array.IndexOf(lvl0, 0));

        learn[index].SetEncounterMoves(level, lvl0, start);
        return lvl0;
    }

    private static int[] GetEncounterMoves2(int species, int level, GameVersion version)
    {
        var learn = GameData.GetLearnsets(version);
        var table = GameData.GetPersonal(version);
        var index = table.GetFormIndex(species, 0);
        var lvl0 = learn[species].GetEncounterMoves(1);
        int start = Math.Max(0, Array.IndexOf(lvl0, 0));

        learn[index].SetEncounterMoves(level, lvl0, start);
        return lvl0;
    }

    public static int[] GetEncounterMoves(int species, int form, int level, GameVersion version)
    {
        if (RBY.Contains(version))
            return GetEncounterMoves1(species, level, version);
        if (GSC.Contains(version))
            return GetEncounterMoves2(species, level, version);
        var learn = GameData.GetLearnsets(version);
        var table = GameData.GetPersonal(version);
        var index = table.GetFormIndex(species, form);
        return learn[index].GetEncounterMoves(level);
    }
}

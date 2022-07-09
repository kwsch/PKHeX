using System;
using System.Collections.Generic;
using System.Linq;
using static PKHeX.Core.Legal;
using static PKHeX.Core.GameVersion;

namespace PKHeX.Core;

/// <summary>
/// Logic for obtaining a list of moves.
/// </summary>
internal static class MoveList
{
    internal static IEnumerable<int> GetValidRelearn(PKM pk, int species, int form, bool inheritlvlmoves, GameVersion version = Any)
    {
        int generation = pk.Generation;
        if (generation < 6)
            return Array.Empty<int>();

        var r = new List<int>();
        r.AddRange(MoveEgg.GetRelearnLVLMoves(pk, species, form, 1, version));

        if (pk.Format == 6 && pk.Species != (int)Species.Meowstic)
            form = 0;

        r.AddRange(MoveEgg.GetEggMoves(species, form, version, Math.Max(2, generation)));
        if (inheritlvlmoves)
            r.AddRange(MoveEgg.GetRelearnLVLMoves(pk, species, form, 100, version));
        return r.Distinct();
    }

    internal static int[] GetShedinjaEvolveMoves(PKM pk, int generation, int lvl)
    {
        if (pk.Species != (int)Species.Shedinja || lvl < 20)
            return Array.Empty<int>();

        // If Nincada evolves into Ninjask and learns a move after evolution from Ninjask's LevelUp data, Shedinja would appear with that move.
        // Only one move above level 20 is allowed; check the count of Ninjask moves elsewhere.
        return generation switch
        {
            3 when pk.InhabitedGeneration(3) => LevelUpE[(int)Species.Ninjask].GetMoves(lvl, 20), // Same LevelUp data in all Gen3 games
            4 when pk.InhabitedGeneration(4) => LevelUpPt[(int)Species.Ninjask].GetMoves(lvl, 20), // Same LevelUp data in all Gen4 games
            _ => Array.Empty<int>(),
        };
    }

    internal static int GetShedinjaMoveLevel(int species, int move, int generation)
    {
        var src = generation == 4 ? LevelUpPt : LevelUpE;
        var moves = src[species];
        return moves.GetLevelLearnMove(move);
    }

    internal static int[] GetBaseEggMoves(PKM pk, int species, int form, GameVersion gameSource, int lvl)
    {
        if (gameSource == Any)
            gameSource = (GameVersion)pk.Version;

        switch (gameSource)
        {
            case GSC or GS:
                // If checking back-transfer specimen (GSC->RBY), remove moves that must be deleted prior to transfer
                static int[] getRBYCompatibleMoves(int format, int[] moves) => format == 1 ? Array.FindAll(moves, m => m <= MaxMoveID_1) : moves;
                if (pk.InhabitedGeneration(2))
                    return getRBYCompatibleMoves(pk.Format, LevelUpGS[species].GetMoves(lvl));
                break;
            case C:
                if (pk.InhabitedGeneration(2))
                    return getRBYCompatibleMoves(pk.Format, LevelUpC[species].GetMoves(lvl));
                break;

            case R or S or RS:
                if (pk.InhabitedGeneration(3))
                    return LevelUpRS[species].GetMoves(lvl);
                break;
            case E:
                if (pk.InhabitedGeneration(3))
                    return LevelUpE[species].GetMoves(lvl);
                break;
            case FR or LG or FRLG:
                // The only difference in FR/LG is Deoxys, which doesn't breed.
                if (pk.InhabitedGeneration(3))
                    return LevelUpFR[species].GetMoves(lvl);
                break;

            case D or P or DP:
                if (pk.InhabitedGeneration(4))
                    return LevelUpDP[species].GetMoves(lvl);
                break;
            case Pt:
                if (pk.InhabitedGeneration(4))
                    return LevelUpPt[species].GetMoves(lvl);
                break;
            case HG or SS or HGSS:
                if (pk.InhabitedGeneration(4))
                    return LevelUpHGSS[species].GetMoves(lvl);
                break;

            case B or W or BW:
                if (pk.InhabitedGeneration(5))
                    return LevelUpBW[species].GetMoves(lvl);
                break;

            case B2 or W2 or B2W2:
                if (pk.InhabitedGeneration(5))
                    return LevelUpB2W2[species].GetMoves(lvl);
                break;

            case X or Y or XY:
                if (pk.InhabitedGeneration(6))
                    return LevelUpXY[species].GetMoves(lvl);
                break;

            case AS or OR or ORAS:
                if (pk.InhabitedGeneration(6))
                    return LevelUpAO[species].GetMoves(lvl);
                break;

            case SN or MN or SM:
                if (species > MaxSpeciesID_7)
                    break;
                if (pk.InhabitedGeneration(7))
                {
                    int index = PersonalTable.SM.GetFormIndex(species, form);
                    return LevelUpSM[index].GetMoves(lvl);
                }
                break;

            case US or UM or USUM:
                if (pk.InhabitedGeneration(7))
                {
                    int index = PersonalTable.USUM.GetFormIndex(species, form);
                    return LevelUpUSUM[index].GetMoves(lvl);
                }
                break;

            case SW or SH or SWSH:
                if (pk.InhabitedGeneration(8))
                {
                    int index = PersonalTable.SWSH.GetFormIndex(species, form);
                    return LevelUpSWSH[index].GetMoves(lvl);
                }
                break;

            case PLA:
                if (pk.InhabitedGeneration(8))
                {
                    int index = PersonalTable.LA.GetFormIndex(species, form);
                    return LevelUpLA[index].GetMoves(lvl);
                }
                break;

            case BD or SP or BDSP:
                if (pk.InhabitedGeneration(8))
                {
                    int index = PersonalTable.BDSP.GetFormIndex(species, form);
                    return LevelUpBDSP[index].GetMoves(lvl);
                }
                break;
        }
        return Array.Empty<int>();
    }

    internal static IReadOnlyList<int>[] GetValidMovesAllGens(PKM pk, EvolutionHistory evoChains, MoveSourceType types = MoveSourceType.ExternalSources, bool RemoveTransferHM = true)
    {
        var result = new IReadOnlyList<int>[evoChains.Length];
        for (int i = 0; i < result.Length; i++)
            result[i] = Array.Empty<int>();

        var min = pk is IBattleVersion b ? Math.Max(0, b.GetMinGeneration()) : 1;
        for (int i = min; i < evoChains.Length; i++)
        {
            var evos = evoChains[i];
            if (evos.Length == 0)
                continue;

            result[i] = GetValidMoves(pk, evos, i, types, RemoveTransferHM).ToList();
        }
        return result;
    }

    internal static IEnumerable<int> GetValidMoves(PKM pk, EvoCriteria[] evoChain, int generation, MoveSourceType types = MoveSourceType.ExternalSources, bool RemoveTransferHM = true)
    {
        var (_, version) = pk.IsMovesetRestricted();
        return GetValidMoves(pk, version, evoChain, generation, types: types, RemoveTransferHM: RemoveTransferHM);
    }

    internal static IEnumerable<int> GetValidRelearn(PKM pk, int species, int form, GameVersion version = Any)
    {
        return GetValidRelearn(pk, species, form, Breeding.GetCanInheritMoves(species), version);
    }

    /// <summary>
    /// ONLY CALL FOR GEN2 EGGS
    /// </summary>
    internal static IEnumerable<int> GetExclusivePreEvolutionMoves(PKM pk, int Species, EvoCriteria[] evoChain, int generation, GameVersion Version)
    {
        var preevomoves = new List<int>();
        var evomoves = new List<int>();
        var index = Array.FindIndex(evoChain, z => z.Species == Species);
        for (int i = 0; i < evoChain.Length; i++)
        {
            int minLvLG2;
            var evo = evoChain[i];
            if (ParseSettings.AllowGen2MoveReminder(pk))
                minLvLG2 = 0;
            else if (i == evoChain.Length - 1) // minimum level, otherwise next learnable level
                minLvLG2 = 5;
            else if (evo.RequiresLvlUp)
                minLvLG2 = evo.LevelMax + 1;
            else
                minLvLG2 = evo.LevelMax;

            var moves = GetMoves(pk, evo.Species, evo.Form, evo.LevelMax, 0, minLvLG2, Version: Version, types: MoveSourceType.ExternalSources, RemoveTransferHM: false, generation: generation);
            var list = i >= index ? preevomoves : evomoves;
            list.AddRange(moves);
        }
        return preevomoves.Except(evomoves).Distinct();
    }

    internal static IEnumerable<int> GetValidMoves(PKM pk, GameVersion version, EvoCriteria[] chain, int generation, MoveSourceType types = MoveSourceType.Reminder, bool RemoveTransferHM = true)
    {
        var r = new List<int> { 0 };
        int species = pk.Species;

        if (FormChangeMovesRetain.Contains(species)) // Deoxys & Shaymin & Giratina (others don't have extra but whatever)
            return GetValidMovesAllForms(pk, chain, version, generation, types, RemoveTransferHM, species, r);

        // Generation 1 & 2 do not always have move relearning capability, so the bottom bound for learnable indexes needs to be determined.
        var minLvLG1 = 0;
        var minLvLG2 = 0;
        for (var i = 0; i < chain.Length; i++)
        {
            var evo = chain[i];
            bool encounteredEvo = i == chain.Length - 1;

            if (generation <= 2)
            {
                if (encounteredEvo) // minimum level, otherwise next learnable level
                    minLvLG1 = evo.LevelMin + 1;
                else // learns level up moves immediately after evolving
                    minLvLG1 = evo.LevelMin;

                if (!ParseSettings.AllowGen2MoveReminder(pk))
                    minLvLG2 = minLvLG1;
            }

            var maxLevel = evo.LevelMax;
            if (!encounteredEvo) // evolution
                ++maxLevel; // allow lvlmoves from the level it evolved to the next species
            var moves = GetMoves(pk, evo.Species, evo.Form, maxLevel, minLvLG1, minLvLG2, version, types, RemoveTransferHM, generation);
            r.AddRange(moves);
        }

        if (pk.Format <= 3)
            return r.Distinct();

        if (types.HasFlagFast(MoveSourceType.LevelUp))
            MoveTutor.AddSpecialFormChangeMoves(r, pk, generation, species);
        if (types.HasFlagFast(MoveSourceType.SpecialTutor))
            MoveTutor.AddSpecialTutorMoves(r, pk, generation, species);
        if (types.HasFlagFast(MoveSourceType.RelearnMoves) && generation >= 6)
            r.AddRange(pk.RelearnMoves);
        return r.Distinct();
    }

    internal static IEnumerable<int> GetValidMovesAllForms(PKM pk, EvoCriteria[] chain, GameVersion version, int generation, MoveSourceType types, bool RemoveTransferHM, int species, List<int> r)
    {
        // These don't evolve, so don't bother iterating for all entries in the evolution chain (should always be count==1).
        int formCount;

        // In gen 3 deoxys has different forms depending on the current game, in the PersonalInfo there is no alternate form info
        if (pk.Format == 3 && species == (int) Species.Deoxys)
            formCount = 4;
        else
            formCount = pk.PersonalInfo.FormCount;

        for (int form = 0; form < formCount; form++)
            r.AddRange(GetMoves(pk, species, form, chain[0].LevelMax, 0, 0, version, types, RemoveTransferHM, generation));
        if (types.HasFlagFast(MoveSourceType.RelearnMoves))
            r.AddRange(pk.RelearnMoves);
        return r.Distinct();
    }

    private static IEnumerable<int> GetMoves(PKM pk, int species, int form, int maxLevel, int minlvlG1, int minlvlG2, GameVersion Version, MoveSourceType types, bool RemoveTransferHM, int generation)
    {
        var r = new List<int>();
        if (types.HasFlagFast(MoveSourceType.LevelUp))
            r.AddRange(MoveLevelUp.GetMovesLevelUp(pk, species, form, maxLevel, minlvlG1, minlvlG2, Version, types.HasFlagFast(MoveSourceType.Reminder), generation));
        if (types.HasFlagFast(MoveSourceType.Machine))
            r.AddRange(MoveTechnicalMachine.GetTMHM(pk, species, form, generation, Version, RemoveTransferHM));
        if (types.HasFlagFast(MoveSourceType.TechnicalRecord))
            r.AddRange(MoveTechnicalMachine.GetRecords(pk, species, form, generation));
        if (types.HasFlagFast(MoveSourceType.AllTutors))
            r.AddRange(MoveTutor.GetTutorMoves(pk, species, form, types.HasFlagFast(MoveSourceType.SpecialTutor), generation));
        return r.Distinct();
    }
}

[Flags]
#pragma warning disable RCS1154 // Sort enum members.
public enum MoveSourceType
#pragma warning restore RCS1154 // Sort enum members.
{
    None,
    LevelUp         = 1 << 0,
    RelearnMoves    = 1 << 1,
    Machine         = 1 << 2,
    TypeTutor       = 1 << 3,
    SpecialTutor    = 1 << 4,
    EnhancedTutor   = 1 << 5,
    SharedEggMove   = 1 << 6,
    TechnicalRecord = 1 << 7,

    AllTutors = TypeTutor | SpecialTutor | EnhancedTutor,
    AllMachines = Machine | TechnicalRecord,

    Reminder = LevelUp | RelearnMoves | TechnicalRecord,
    Encounter = LevelUp | RelearnMoves,
    ExternalSources = Reminder | AllMachines | AllTutors,
    All = ExternalSources | SharedEggMove | RelearnMoves,
}

public static class MoveSourceTypeExtensions
{
    public static bool HasFlagFast(this MoveSourceType value, MoveSourceType flag) => (value & flag) != 0;
    public static MoveSourceType ClearNonEggSources(this MoveSourceType value) => value & MoveSourceType.Encounter;
}

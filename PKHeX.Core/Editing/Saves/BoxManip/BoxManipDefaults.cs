using System.Collections.Generic;
using PKHeX.Core.Searching;
using static PKHeX.Core.BoxManipType;

namespace PKHeX.Core;

/// <summary>
/// Curated lists of <see cref="BoxManipBase"/> implementations to present as options to the user.
/// </summary>
public static class BoxManipDefaults
{
    /// <summary>
    /// Common sorting actions.
    /// </summary>
    public static readonly IReadOnlyList<BoxManipBase> SortCommon = new List<BoxManipBase>
    {
        new BoxManipSort(SortSpecies, EntitySorting.OrderBySpecies),
        new BoxManipSort(SortSpeciesReverse, EntitySorting.OrderByDescendingSpecies),
        new BoxManipSort(SortLevel, EntitySorting.OrderByLevel),
        new BoxManipSort(SortLevelReverse, EntitySorting.OrderByDescendingLevel),
        new BoxManipSort(SortDate, EntitySorting.OrderByDateObtained, s => s.Generation >= 4),
        new BoxManipSort(SortName, list => list.OrderBySpeciesName(GameInfo.Strings.Species)),
        new BoxManipSort(SortFavorite, list => list.OrderByCustom(pk => pk is IFavorite {Favorite: true}), s => s.BlankPKM is IFavorite),
        new BoxManipSortComplex(SortParty, (list, sav, start) => list.BubbleUp(sav, i => ((SAV7b)sav).Blocks.Storage.IsParty(i), start), s => s is SAV7b),
        new BoxManipSort(SortShiny, list => list.OrderByCustom(pk => !pk.IsShiny)),
        new BoxManipSort(SortRandom, list => list.OrderByCustom(_ => Util.Rand32())),
    };

    /// <summary>
    /// Advanced sorting actions.
    /// </summary>
    public static readonly IReadOnlyList<BoxManipBase> SortAdvanced = new List<BoxManipBase>
    {
        new BoxManipSort(SortUsage, EntitySorting.OrderByUsage, s => s.Generation >= 3),
        new BoxManipSort(SortPotential, list => list.OrderByCustom(pk => (pk.MaxIV * 6) - pk.IVTotal)),
        new BoxManipSort(SortTraining, list => list.OrderByCustom(pk => (pk.MaxEV * 6) - pk.EVTotal)),
        new BoxManipSortComplex(SortOwner, (list, sav) => list.OrderByOwnership(sav)),
        new BoxManipSort(SortType, list => list.OrderByCustom(pk => pk.PersonalInfo.Type1, pk => pk.PersonalInfo.Type2)),
        new BoxManipSort(SortVersion, list => list.OrderByCustom(pk => pk.Generation, pk => pk.Version, pk => pk.Met_Location), s => s.Generation >= 3),
        new BoxManipSort(SortBST, list => list.OrderByCustom(pk => pk.PersonalInfo.GetBaseStatTotal())),
        new BoxManipSort(SortCP, list => list.OrderByCustom(pk => pk is PB7 pb7 ? pb7.Stat_CP : 0), s => s is SAV7b),
        new BoxManipSort(SortLegal, list => list.OrderByCustom(pk => !new LegalityAnalysis(pk).Valid)),
        new BoxManipSort(SortEncounterType, list => list.OrderByCustom(pk => new LegalityAnalysis(pk).Info.EncounterMatch.LongName)),
    };

    /// <summary>
    /// Common deletion actions.
    /// </summary>
    public static readonly IReadOnlyList<BoxManipBase> ClearCommon = new List<BoxManipBase>
    {
        new BoxManipClear(DeleteAll, _ => true),
        new BoxManipClear(DeleteEggs, pk => pk.IsEgg, s => s.Generation >= 2 && s is not SAV8LA),
        new BoxManipClearComplex(DeletePastGen, (pk, sav) => pk.Generation != sav.Generation, s => s.Generation >= 4),
        new BoxManipClearComplex(DeleteForeign, (pk, sav) => !sav.IsOriginalHandler(pk, pk.Format > 2)),
        new BoxManipClear(DeleteUntrained, pk => pk.EVTotal == 0, s => s is not SAV8LA),
        new BoxManipClear(DeleteUntrained, pk => !((PA8)pk).IsGanbaruValuesMax(pk), s => s is SAV8LA),
        new BoxManipClear(DeleteItemless, pk => pk.HeldItem == 0, s => s is not SAV8LA),
        new BoxManipClear(DeleteIllegal, pk => !new LegalityAnalysis(pk).Valid),
        new BoxManipClearDuplicate<string>(DeleteClones, pk => SearchUtil.GetCloneDetectMethod(CloneDetectionMethod.HashDetails)(pk)),
    };

    /// <summary>
    /// Common modifying actions.
    /// </summary>
    public static readonly IReadOnlyList<BoxManipModify> ModifyCommon = new List<BoxManipModify>
    {
        new(ModifyHatchEggs, pk => pk.ForceHatchPKM(), s => s.Generation >= 2 && s is not SAV8LA),
        new(ModifyMaxFriendship, pk => pk.MaximizeFriendship()),
        new(ModifyMaxLevel, pk => pk.MaximizeLevel()),
        new(ModifyResetMoves, pk => pk.SetMoves(pk.GetMoveSet()), s => s.Generation >= 3),
        new(ModifyRandomMoves, pk => pk.SetMoves(pk.GetMoveSet(true))),
        new(ModifyHyperTrain,pk => pk.SetSuggestedHyperTrainingData(), s => s.Generation >= 7 && s is not SAV8LA),
        new(ModifyGanbaru,pk => ((IGanbaru)pk).SetSuggestedGanbaruValues(pk), s => s is SAV8LA),
        new(ModifyRemoveNicknames, pk => pk.SetDefaultNickname()),
        new(ModifyRemoveItem, pk => pk.HeldItem = 0, s => s.Generation >= 2),
        new(ModifyHeal, pk => pk.Heal(), s => s.Generation >= 6), // HP stored in box, or official code has bugged transfer PP the user would like to rectify.
    };
}

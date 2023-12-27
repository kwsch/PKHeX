namespace PKHeX.Core;

/// <summary>
/// Action to perform when manipulating boxes.
/// </summary>
public enum BoxManipType
{
    DeleteAll,
    DeleteEggs,
    DeletePastGen,
    DeleteForeign,
    DeleteUntrained,
    DeleteItemless,
    DeleteIllegal,
    DeleteClones,

    SortSpecies,
    SortSpeciesReverse,
    SortLevel,
    SortLevelReverse,
    SortDate,
    SortName,
    SortFavorite,
    SortParty,
    SortShiny,
    SortRandom,

    SortUsage,
    SortPotential,
    SortTraining,
    SortOwner,
    SortType,
    SortTypeTera,
    SortVersion,
    SortBST,
    SortCP,
    SortScale,
    SortRibbons,
    SortMarks,
    SortLegal,
    SortEncounterType,

    ModifyHatchEggs,
    ModifyMaxFriendship,
    ModifyMaxLevel,
    ModifyResetMoves,
    ModifyRandomMoves,
    ModifyHyperTrain,
    ModifyGanbaru,
    ModifyRemoveNicknames,
    ModifyRemoveItem,
    ModifyHeal,
}

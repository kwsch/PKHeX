namespace PKHeX.Core
{
    public enum BoxManipType
    {
        DeleteAll,
        DeleteEggs,
        DeletePastGen,
        DeleteForeign,
        DeleteUntrained,
        DeleteItemless,
        DeleteIllegal,

        SortSpecies,
        SortSpeciesReverse,
        SortLevel,
        SortLevelReverse,
        SortDate,
        SortName,
        SortShiny,
        SortRandom,

        SortUsage,
        SortPotential,
        SortTraining,
        SortOwner,
        SortType,
        SortVersion,
        SortBST,
        SortLegal,

        ModifyHatchEggs,
        ModifyMaxFriendship,
        ModifyMaxLevel,
        ModifyResetMoves,
        ModifyRandomMoves,
        ModifyHyperTrain,
        ModifyRemoveNicknames,
        ModifyRemoveItem,
    }
}
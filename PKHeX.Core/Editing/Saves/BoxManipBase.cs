using System;
using System.Collections.Generic;
using PKHeX.Core.Searching;

namespace PKHeX.Core
{
    public abstract class BoxManipBase : IBoxManip
    {
        protected BoxManipBase(BoxManipType type, Func<SaveFile, bool> usable)
        {
            Type = type;
            Usable = usable;
        }

        public BoxManipType Type { get; }
        public Func<SaveFile, bool> Usable { get; }

        public abstract string GetPrompt(bool all);
        public abstract string GetFail(bool all);
        public abstract string GetSuccess(bool all);
        public abstract int Execute(SaveFile SAV, BoxManipParam param);

        public static readonly IReadOnlyList<BoxManipBase> SortCommon = new List<BoxManipBase>
        {
            new BoxManipSort(BoxManipType.SortSpecies, PKMSorting.OrderBySpecies),
            new BoxManipSort(BoxManipType.SortSpeciesReverse, PKMSorting.OrderByDescendingSpecies),
            new BoxManipSort(BoxManipType.SortLevel, PKMSorting.OrderByLevel),
            new BoxManipSort(BoxManipType.SortLevelReverse, PKMSorting.OrderByDescendingLevel),
            new BoxManipSort(BoxManipType.SortDate, PKMSorting.OrderByDateObtained, s => s.Generation >= 4),
            new BoxManipSort(BoxManipType.SortName, list => list.OrderBySpeciesName(GameInfo.Strings.Species)),
            new BoxManipSort(BoxManipType.SortFavorite, list => list.OrderByCustom(pk => pk is PB7 pb7 && pb7.Favorite), s => s is SAV7b),
            new BoxManipSortComplex(BoxManipType.SortParty, (list, sav) => list.OrderByCustom(pk => ((SAV7b)sav).Blocks.Storage.GetPartyIndex(pk.Box - 1, pk.Slot - 1)), s => s is SAV7b),
            new BoxManipSort(BoxManipType.SortShiny, list => list.OrderByCustom(pk => !pk.IsShiny)),
            new BoxManipSort(BoxManipType.SortRandom, list => list.OrderByCustom(_ => Util.Rand32())),
        };

        public static readonly IReadOnlyList<BoxManipBase> SortAdvanced = new List<BoxManipBase>
        {
            new BoxManipSort(BoxManipType.SortUsage, PKMSorting.OrderByUsage, s => s.Generation >= 3),
            new BoxManipSort(BoxManipType.SortPotential, list => list.OrderByCustom(pk => (pk.MaxIV * 6) - pk.IVTotal)),
            new BoxManipSort(BoxManipType.SortTraining, list => list.OrderByCustom(pk => (pk.MaxEV * 6) - pk.EVTotal)),
            new BoxManipSortComplex(BoxManipType.SortOwner, (list, sav) => list.OrderByOwnership(sav)),
            new BoxManipSort(BoxManipType.SortType, list => list.OrderByCustom(pk => pk.PersonalInfo.Type1, pk => pk.PersonalInfo.Type2)),
            new BoxManipSort(BoxManipType.SortVersion, list => list.OrderByCustom(pk => pk.GenNumber, pk => pk.Version, pk => pk.Met_Location), s => s.Generation >= 3),
            new BoxManipSort(BoxManipType.SortBST, list => list.OrderByCustom(pk => pk.PersonalInfo.BST)),
            new BoxManipSort(BoxManipType.SortCP, list => list.OrderByCustom(pk => pk is PB7 pb7 ? pb7.Stat_CP : 0), s => s is SAV7b),
            new BoxManipSort(BoxManipType.SortLegal, list => list.OrderByCustom(pk => !new LegalityAnalysis(pk).Valid)),
            new BoxManipSort(BoxManipType.SortEncounterType, list => list.OrderByCustom(pk => new LegalityAnalysis(pk).Info.EncounterMatch.LongName)),
        };

        public static readonly IReadOnlyList<BoxManipBase> ClearCommon = new List<BoxManipBase>
        {
            new BoxManipClear(BoxManipType.DeleteAll, _ => true),
            new BoxManipClear(BoxManipType.DeleteEggs, pk => pk.IsEgg, s => s.Generation >= 2),
            new BoxManipClearComplex(BoxManipType.DeletePastGen, (pk, sav) => pk.GenNumber != sav.Generation, s => s.Generation >= 4),
            new BoxManipClearComplex(BoxManipType.DeleteForeign, (pk, sav) => !sav.IsOriginalHandler(pk, pk.Format > 2)),
            new BoxManipClear(BoxManipType.DeleteUntrained, pk => pk.EVTotal == 0),
            new BoxManipClear(BoxManipType.DeleteItemless, pk => pk.HeldItem == 0),
            new BoxManipClear(BoxManipType.DeleteIllegal, pk => !new LegalityAnalysis(pk).Valid),
            new BoxManipClearDuplicate<string>(BoxManipType.DeleteClones, pk => SearchUtil.GetCloneDetectMethod(CloneDetectionMethod.HashDetails)(pk)),
        };

        public static readonly IReadOnlyList<BoxManipModify> ModifyCommon = new List<BoxManipModify>
        {
            new BoxManipModify(BoxManipType.ModifyHatchEggs, pk => pk.ForceHatchPKM(), s => s.Generation >= 2),
            new BoxManipModify(BoxManipType.ModifyMaxFriendship, pk => pk.MaximizeFriendship()),
            new BoxManipModify(BoxManipType.ModifyMaxLevel, pk => pk.MaximizeLevel()),
            new BoxManipModify(BoxManipType.ModifyResetMoves, pk => pk.SetMoves(pk.GetMoveSet()), s => s.Generation >= 3),
            new BoxManipModify(BoxManipType.ModifyRandomMoves, pk => pk.SetMoves(pk.GetMoveSet(true))),
            new BoxManipModify(BoxManipType.ModifyHyperTrain,pk => pk.SetSuggestedHyperTrainingData(), s => s.Generation >= 7),
            new BoxManipModify(BoxManipType.ModifyRemoveNicknames, pk => pk.SetDefaultNickname()),
            new BoxManipModify(BoxManipType.ModifyRemoveItem, pk => pk.HeldItem = 0, s => s.Generation >= 2),
            new BoxManipModify(BoxManipType.ModifyHeal, pk => pk.Heal(), s => s.Generation >= 8 || s is SAV7b),
        };
    }
}
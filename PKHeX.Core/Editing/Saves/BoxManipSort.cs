using System;
using System.Collections.Generic;

namespace PKHeX.Core
{
    public sealed class BoxManipSort : IBoxManip
    {
        public BoxManipType Type { get; }
        public Func<SaveFile, bool> Usable { get; set; }

        public string GetPrompt(bool all) => all ? MessageStrings.MsgSaveBoxSortAll : MessageStrings.MsgSaveBoxSortCurrent;
        public string GetFail(bool all) => all ? MessageStrings.MsgSaveBoxSortAllFailBattle: MessageStrings.MsgSaveBoxSortCurrentFailBattle;
        public string GetSuccess(bool all) => all ? MessageStrings.MsgSaveBoxSortAllSuccess : MessageStrings.MsgSaveBoxSortCurrentSuccess;

        private readonly Func<IEnumerable<PKM>, IEnumerable<PKM>> SorterSimple;
        private readonly Func<IEnumerable<PKM>, SaveFile, IEnumerable<PKM>> SorterComplex;

        public int Execute(SaveFile SAV, BoxManipParam param)
        {
            IEnumerable<PKM> Method(IEnumerable<PKM> p) => SorterSimple != null ? SorterSimple(p) : SorterComplex(p, SAV);
            return SAV.SortBoxes(param.Start, param.Stop, Method, param.Reverse);
        }

        private BoxManipSort(BoxManipType type, Func<IEnumerable<PKM>, IEnumerable<PKM>> sorter, Func<SaveFile, bool> usable = null)
        {
            Type = type;
            SorterSimple = sorter;
            Usable = usable;
        }

        private BoxManipSort(BoxManipType type, Func<IEnumerable<PKM>, SaveFile, IEnumerable<PKM>> sorter, Func<SaveFile, bool> usable = null)
        {
            Type = type;
            SorterComplex = sorter;
            Usable = usable;
        }

        public static readonly IReadOnlyList<BoxManipSort> Common = new List<BoxManipSort>
        {
            new BoxManipSort(BoxManipType.SortSpecies, PKMSorting.OrderBySpecies),
            new BoxManipSort(BoxManipType.SortSpeciesReverse, PKMSorting.OrderByDescendingSpecies),
            new BoxManipSort(BoxManipType.SortLevel, PKMSorting.OrderByLevel),
            new BoxManipSort(BoxManipType.SortLevelReverse, PKMSorting.OrderByDescendingLevel),
            new BoxManipSort(BoxManipType.SortDate, PKMSorting.OrderByDateObtained, s => s.Generation >= 4),
            new BoxManipSort(BoxManipType.SortName, list => list.OrderBySpeciesName(GameInfo.Strings.Species)),
            new BoxManipSort(BoxManipType.SortFavorite, list => list.OrderByCustom(pk => !(pk as PB7)?.Favorite), s => s is SAV7b),
            new BoxManipSort(BoxManipType.SortParty, (list, sav) => list.OrderByCustom(pk => ((SAV7b)sav).Storage.GetPartyIndex(pk.Box - 1, pk.Slot - 1)), s => s is SAV7b),
            new BoxManipSort(BoxManipType.SortShiny, list => list.OrderByCustom(pk => !pk.IsShiny)),
            new BoxManipSort(BoxManipType.SortRandom, list => list.OrderByCustom(_ => Util.Rand32())),
        };

        public static readonly IReadOnlyList<BoxManipSort> Advanced = new List<BoxManipSort>
        {
            new BoxManipSort(BoxManipType.SortUsage, PKMSorting.OrderByUsage, s => s.Generation >= 3),
            new BoxManipSort(BoxManipType.SortPotential, list => list.OrderByCustom(pk => (pk.MaxIV * 6) - pk.IVTotal)),
            new BoxManipSort(BoxManipType.SortTraining, list => list.OrderByCustom(pk => (pk.MaxEV * 6) - pk.EVTotal)),
            new BoxManipSort(BoxManipType.SortOwner, (list, sav) => list.OrderByOwnership(sav)),
            new BoxManipSort(BoxManipType.SortType, list => list.OrderByCustom(pk => pk.PersonalInfo.Type1, pk => pk.PersonalInfo.Type2)),
            new BoxManipSort(BoxManipType.SortVersion, list => list.OrderByCustom(pk => pk.GenNumber, pk => pk.Version, pk => pk.Met_Location), s => s.Generation >= 3),
            new BoxManipSort(BoxManipType.SortBST, list => list.OrderByCustom(pk => pk.PersonalInfo.BST)),
            new BoxManipSort(BoxManipType.SortCP, list => list.OrderByCustom(pk => (pk as PB7)?.Stat_CP), s => s is SAV7b),
            new BoxManipSort(BoxManipType.SortLegal, list => list.OrderByCustom(pk => !new LegalityAnalysis(pk).Valid)),
            new BoxManipSort(BoxManipType.SortEncounterType, list => list.OrderByCustom(pk => new LegalityAnalysis(pk).Info?.EncounterMatch.GetEncounterTypeName())),
        };
    }
}
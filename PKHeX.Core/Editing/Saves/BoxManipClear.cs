using System;
using System.Collections.Generic;

namespace PKHeX.Core
{
    public sealed class BoxManipClear : IBoxManip
    {
        public BoxManipType Type { get; }
        public Func<SaveFile, bool> Usable { get; set; }

        public string GetPrompt(bool all) => all ? MessageStrings.MsgSaveBoxClearAll : MessageStrings.MsgSaveBoxClearCurrent;
        public string GetFail(bool all) => all ? MessageStrings.MsgSaveBoxClearAllFailBattle : MessageStrings.MsgSaveBoxClearCurrentFailBattle;
        public string GetSuccess(bool all) => all ? MessageStrings.MsgSaveBoxClearAllSuccess : MessageStrings.MsgSaveBoxClearCurrentSuccess;

        private readonly Func<PKM, bool> CriteriaSimple;
        private readonly Func<PKM, SaveFile, bool> CriteriaSAV;

        public bool Execute(SaveFile SAV, BoxManipParam param)
        {
            bool Method(PKM p) => param.Reverse ^ (CriteriaSAV?.Invoke(p, SAV) ?? CriteriaSimple?.Invoke(p) ?? true);
            SAV.ClearBoxes(param.Start, param.Stop, Method);
            return true;
        }

        private BoxManipClear(BoxManipType type, Func<PKM, bool> criteria, Func<SaveFile, bool> usable = null)
        {
            Type = type;
            CriteriaSimple = criteria;
            Usable = usable;
        }

        private BoxManipClear(BoxManipType type, Func<PKM, SaveFile, bool> criteria, Func<SaveFile, bool> usable = null)
        {
            Type = type;
            CriteriaSAV = criteria;
            Usable = usable;
        }

        public static readonly IReadOnlyList<BoxManipClear> Common = new List<BoxManipClear>
        {
            new BoxManipClear(BoxManipType.DeleteAll, _ => true),
            new BoxManipClear(BoxManipType.DeleteEggs, pk => pk.IsEgg, s => s.Generation >= 2),
            new BoxManipClear(BoxManipType.DeletePastGen, (pk, sav) => pk.GenNumber != sav.Generation, s => s.Generation >= 4),
            new BoxManipClear(BoxManipType.DeleteForeign, (pk, sav) => !sav.IsOriginalHandler(pk, pk.Format > 2)),
            new BoxManipClear(BoxManipType.DeleteUntrained, pk => pk.EVTotal == 0),
            new BoxManipClear(BoxManipType.DeleteItemless, pk => pk.HeldItem == 0),
            new BoxManipClear(BoxManipType.DeleteIllegal, pk => !new LegalityAnalysis(pk).Valid),
        };
    }
}
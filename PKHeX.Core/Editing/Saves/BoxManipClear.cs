using System;
using System.Collections.Generic;
using PKHeX.Core.Searching;

namespace PKHeX.Core
{
    public class BoxManipClear : IBoxManip
    {
        public BoxManipType Type { get; protected set; }
        public Func<SaveFile, bool> Usable { get; set; }

        public string GetPrompt(bool all) => all ? MessageStrings.MsgSaveBoxClearAll : MessageStrings.MsgSaveBoxClearCurrent;
        public string GetFail(bool all) => all ? MessageStrings.MsgSaveBoxClearAllFailBattle : MessageStrings.MsgSaveBoxClearCurrentFailBattle;
        public string GetSuccess(bool all) => all ? MessageStrings.MsgSaveBoxClearAllSuccess : MessageStrings.MsgSaveBoxClearCurrentSuccess;

        protected Func<PKM, bool> CriteriaSimple { private get; set; }
        protected Func<PKM, SaveFile, bool> CriteriaSAV { private get; set; }

        public virtual int Execute(SaveFile SAV, BoxManipParam param)
        {
            bool Method(PKM p) => param.Reverse ^ (CriteriaSAV?.Invoke(p, SAV) ?? CriteriaSimple?.Invoke(p) ?? true);
            return SAV.ClearBoxes(param.Start, param.Stop, Method);
        }

        protected BoxManipClear() { }

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
            new BoxManipClearDuplicate<string>(BoxManipType.DeleteClones, pk => SearchUtil.GetCloneDetectMethod(CloneDetectionMethod.HashDetails)(pk)),
        };
    }

    public sealed class BoxManipClearDuplicate<T> : BoxManipClear
    {
        private readonly HashSet<T> HashSet = new HashSet<T>();

        public override int Execute(SaveFile SAV, BoxManipParam param)
        {
            HashSet.Clear();
            return base.Execute(SAV, param);
        }

        public BoxManipClearDuplicate(BoxManipType type, Func<PKM, T> criteria, Func<SaveFile, bool> usable = null)
        {
            Type = type;
            Usable = usable;
            CriteriaSimple = pk =>
            {
                var result = criteria(pk);
                if (HashSet.Contains(result))
                    return true;
                HashSet.Add(result);
                return false;
            };
        }
    }
}
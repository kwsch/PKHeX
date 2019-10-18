using System;
using System.Collections.Generic;

namespace PKHeX.Core
{
    public sealed class BoxManipClearDuplicate<T> : BoxManipBase
    {
        private readonly HashSet<T> HashSet = new HashSet<T>();
        private readonly Func<PKM, bool> Criteria;
        public BoxManipClearDuplicate(BoxManipType type, Func<PKM, T> criteria) : this(type, criteria, _ => true) { }

        public BoxManipClearDuplicate(BoxManipType type, Func<PKM, T> criteria, Func<SaveFile, bool> usable) : base(type, usable)
        {
            Criteria = pk =>
            {
                var result = criteria(pk);
                if (HashSet.Contains(result))
                    return true;
                HashSet.Add(result);
                return false;
            };
        }

        public override string GetPrompt(bool all) => all ? MessageStrings.MsgSaveBoxClearAll : MessageStrings.MsgSaveBoxClearCurrent;
        public override string GetFail(bool all) => all ? MessageStrings.MsgSaveBoxClearAllFailBattle : MessageStrings.MsgSaveBoxClearCurrentFailBattle;
        public override string GetSuccess(bool all) => all ? MessageStrings.MsgSaveBoxClearAllSuccess : MessageStrings.MsgSaveBoxClearCurrentSuccess;

        public override int Execute(SaveFile SAV, BoxManipParam param)
        {
            HashSet.Clear();
            bool Method(PKM p) => param.Reverse ^ Criteria(p);
            return SAV.ClearBoxes(param.Start, param.Stop, Method);
        }
    }
}
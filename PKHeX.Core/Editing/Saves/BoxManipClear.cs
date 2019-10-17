using System;

namespace PKHeX.Core
{
    public sealed class BoxManipClear : BoxManipBase
    {
        private readonly Func<PKM, bool> Criteria;
        public BoxManipClear(BoxManipType type, Func<PKM, bool> criteria) : this(type, criteria, _ => true) { }
        public BoxManipClear(BoxManipType type, Func<PKM, bool> criteria, Func<SaveFile, bool> usable) : base(type, usable) => Criteria = criteria;

        public override string GetPrompt(bool all) => all ? MessageStrings.MsgSaveBoxClearAll : MessageStrings.MsgSaveBoxClearCurrent;
        public override string GetFail(bool all) => all ? MessageStrings.MsgSaveBoxClearAllFailBattle : MessageStrings.MsgSaveBoxClearCurrentFailBattle;
        public override string GetSuccess(bool all) => all ? MessageStrings.MsgSaveBoxClearAllSuccess : MessageStrings.MsgSaveBoxClearCurrentSuccess;

        public override int Execute(SaveFile SAV, BoxManipParam param)
        {
            bool Method(PKM p) => param.Reverse ^ Criteria(p);
            return SAV.ClearBoxes(param.Start, param.Stop, Method);
        }
    }
}
using System;

namespace PKHeX.Core
{
    public sealed class BoxManipModify : BoxManipBase
    {
        private readonly Action<PKM> Action;
        public BoxManipModify(BoxManipType type, Action<PKM> action) : this(type, action, _ => true) { }
        public BoxManipModify(BoxManipType type, Action<PKM> action, Func<SaveFile, bool> usable) : base(type, usable) => Action = action;

        public override string GetPrompt(bool all) => string.Empty;
        public override string GetFail(bool all) => string.Empty;
        public override string GetSuccess(bool all) => string.Empty;

        public override int Execute(SaveFile SAV, BoxManipParam param) => SAV.ModifyBoxes(Action, param.Start, param.Stop);
    }
}
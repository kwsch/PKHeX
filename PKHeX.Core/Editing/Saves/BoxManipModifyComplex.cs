using System;

namespace PKHeX.Core
{
    public sealed class BoxManipModifyComplex : BoxManipBase
    {
        private readonly Action<PKM, SaveFile> Action;
        public BoxManipModifyComplex(BoxManipType type, Action<PKM, SaveFile> action) : this(type, action, _ => true) { }
        public BoxManipModifyComplex(BoxManipType type, Action<PKM, SaveFile> action, Func<SaveFile, bool> usable) : base(type, usable) => Action = action;

        public override string GetPrompt(bool all) => string.Empty;
        public override string GetFail(bool all) => string.Empty;
        public override string GetSuccess(bool all) => string.Empty;

        public override int Execute(SaveFile SAV, BoxManipParam param) => SAV.ModifyBoxes(pk => Action(pk, SAV), param.Start, param.Stop);
    }
}
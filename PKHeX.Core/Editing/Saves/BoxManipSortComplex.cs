using System;
using System.Collections.Generic;

namespace PKHeX.Core
{
    public sealed class BoxManipSortComplex : BoxManipBase
    {
        private readonly Func<IEnumerable<PKM>, SaveFile, IEnumerable<PKM>> Sorter;
        public BoxManipSortComplex(BoxManipType type, Func<IEnumerable<PKM>, SaveFile, IEnumerable<PKM>> sorter) : this(type, sorter, _ => true) { }
        public BoxManipSortComplex(BoxManipType type, Func<IEnumerable<PKM>, SaveFile, IEnumerable<PKM>> sorter, Func<SaveFile, bool> usable) : base(type, usable) => Sorter = sorter;

        public override string GetPrompt(bool all) => all ? MessageStrings.MsgSaveBoxSortAll : MessageStrings.MsgSaveBoxSortCurrent;
        public override string GetFail(bool all) => all ? MessageStrings.MsgSaveBoxSortAllFailBattle : MessageStrings.MsgSaveBoxSortCurrentFailBattle;
        public override string GetSuccess(bool all) => all ? MessageStrings.MsgSaveBoxSortAllSuccess : MessageStrings.MsgSaveBoxSortCurrentSuccess;

        public override int Execute(SaveFile sav, BoxManipParam param)
        {
            IEnumerable<PKM> Method(IEnumerable<PKM> p) => Sorter(p, sav);
            return sav.SortBoxes(param.Start, param.Stop, Method, param.Reverse);
        }
    }
}
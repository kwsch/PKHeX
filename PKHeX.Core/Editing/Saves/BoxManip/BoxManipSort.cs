using System;
using System.Collections.Generic;

namespace PKHeX.Core;

/// <summary>
/// Sorts contents of boxes by using a Sorter to determine the order.
/// </summary>
public sealed class BoxManipSort(BoxManipType Type, Func<IEnumerable<PKM>, IEnumerable<PKM>> Sorter, Func<SaveFile, bool> Usable) : BoxManipBase(Type, Usable)
{
    public BoxManipSort(BoxManipType Type, Func<IEnumerable<PKM>, IEnumerable<PKM>> Sorter) : this(Type, Sorter, _ => true) { }

    public override string GetPrompt(bool all) => all ? MessageStrings.MsgSaveBoxSortAll : MessageStrings.MsgSaveBoxSortCurrent;
    public override string GetFail(bool all) => all ? MessageStrings.MsgSaveBoxSortAllFailBattle: MessageStrings.MsgSaveBoxSortCurrentFailBattle;
    public override string GetSuccess(bool all) => all ? MessageStrings.MsgSaveBoxSortAllSuccess : MessageStrings.MsgSaveBoxSortCurrentSuccess;

    public override int Execute(SaveFile sav, BoxManipParam param)
    {
        var (start, stop, reverse) = param;
        return sav.SortBoxes(start, stop, Method, reverse);

        IEnumerable<PKM> Method(IEnumerable<PKM> p, int index) => Sorter(p);
    }
}

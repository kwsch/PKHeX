using System;
using System.Collections.Generic;

namespace PKHeX.Core;

/// <summary>
/// Clears contents of boxes by deleting all but the first duplicate detected.
/// </summary>
/// <typeparam name="T">Base type of the "is duplicate" hash for the duplicate detection.</typeparam>
public sealed class BoxManipClearDuplicate<T> : BoxManipBase
{
    private readonly HashSet<T> HashSet = [];
    private readonly Func<PKM, bool> Criteria;
    public BoxManipClearDuplicate(BoxManipType type, Func<PKM, T> criteria) : this(type, criteria, _ => true) { }

    public BoxManipClearDuplicate(BoxManipType type, Func<PKM, T> criteria, Func<SaveFile, bool> usable) : base(type, usable)
    {
        Criteria = pk =>
        {
            var result = criteria(pk);
            return !HashSet.Add(result);
        };
    }

    public override string GetPrompt(bool all) => all ? MessageStrings.MsgSaveBoxClearAll : MessageStrings.MsgSaveBoxClearCurrent;
    public override string GetFail(bool all) => all ? MessageStrings.MsgSaveBoxClearAllFailBattle : MessageStrings.MsgSaveBoxClearCurrentFailBattle;
    public override string GetSuccess(bool all) => all ? MessageStrings.MsgSaveBoxClearAllSuccess : MessageStrings.MsgSaveBoxClearCurrentSuccess;

    public override int Execute(SaveFile sav, BoxManipParam param)
    {
        HashSet.Clear();
        var (start, stop, reverse) = param;
        return sav.ClearBoxes(start, stop, Method);

        bool Method(PKM p) => reverse ^ Criteria(p);
    }
}

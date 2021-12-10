using System;

namespace PKHeX.Core;

/// <summary>
/// Clears contents of boxes by deleting all that satisfy a <see cref="Criteria"/>.
/// </summary>
public sealed class BoxManipClear : BoxManipBase
{
    private readonly Func<PKM, bool> Criteria;
    public BoxManipClear(BoxManipType type, Func<PKM, bool> criteria) : this(type, criteria, _ => true) { }
    public BoxManipClear(BoxManipType type, Func<PKM, bool> criteria, Func<SaveFile, bool> usable) : base(type, usable) => Criteria = criteria;

    public override string GetPrompt(bool all) => all ? MessageStrings.MsgSaveBoxClearAll : MessageStrings.MsgSaveBoxClearCurrent;
    public override string GetFail(bool all) => all ? MessageStrings.MsgSaveBoxClearAllFailBattle : MessageStrings.MsgSaveBoxClearCurrentFailBattle;
    public override string GetSuccess(bool all) => all ? MessageStrings.MsgSaveBoxClearAllSuccess : MessageStrings.MsgSaveBoxClearCurrentSuccess;

    public override int Execute(SaveFile sav, BoxManipParam param)
    {
        var (start, stop, reverse) = param;
        bool Method(PKM p) => reverse ^ Criteria(p);
        return sav.ClearBoxes(start, stop, Method);
    }
}

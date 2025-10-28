using System;

namespace PKHeX.Core;

/// <summary>
/// Clears contents of boxes by deleting all that satisfy a criteria based on a <see cref="SaveFile"/>.
/// </summary>
public sealed class BoxManipClearComplex(BoxManipType Type, Func<PKM, SaveFile, bool> criteria, Func<SaveFile, bool> Usable) : BoxManipBase(Type, Usable)
{
    public BoxManipClearComplex(BoxManipType Type, Func<PKM, SaveFile, bool> Criteria) : this(Type, Criteria, _ => true) { }

    public override string GetPrompt(bool all) => all ? MessageStrings.MsgSaveBoxClearAll : MessageStrings.MsgSaveBoxClearCurrent;
    public override string GetFail(bool all) => all ? MessageStrings.MsgSaveBoxClearAllFailBattle : MessageStrings.MsgSaveBoxClearCurrentFailBattle;
    public override string GetSuccess(bool all) => all ? MessageStrings.MsgSaveBoxClearAllSuccess : MessageStrings.MsgSaveBoxClearCurrentSuccess;

    public override int Execute(SaveFile sav, BoxManipParam param)
    {
        var (start, stop, reverse) = param;
        return sav.ClearBoxes(start, stop, Method);

        bool Method(PKM p) => reverse ^ criteria(p, sav);
    }
}

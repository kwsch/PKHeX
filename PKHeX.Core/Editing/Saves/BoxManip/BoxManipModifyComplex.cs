using System;

namespace PKHeX.Core;

/// <summary>
/// Modifies contents of boxes by using an <see cref="Action"/> (referencing a Save File) to change data.
/// </summary>
public sealed class BoxManipModifyComplex(BoxManipType Type, Action<PKM, SaveFile> Action, Func<SaveFile, bool> Usable)
    : BoxManipBase(Type, Usable)
{
    public BoxManipModifyComplex(BoxManipType Type, Action<PKM, SaveFile> Action) : this(Type, Action, _ => true) { }

    public override string GetPrompt(bool all) => string.Empty;
    public override string GetFail(bool all) => string.Empty;
    public override string GetSuccess(bool all) => string.Empty;

    public override int Execute(SaveFile sav, BoxManipParam param)
    {
        var (start, stop, _) = param;
        return sav.ModifyBoxes(pk => Action(pk, sav), start, stop);
    }
}

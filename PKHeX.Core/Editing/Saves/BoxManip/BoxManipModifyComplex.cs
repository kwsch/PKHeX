using System;

namespace PKHeX.Core;

/// <summary>
/// Modifies contents of boxes by using an <see cref="Action"/> (referencing a Save File) to change data.
/// </summary>
public sealed class BoxManipModifyComplex : BoxManipBase
{
    private readonly Action<PKM, SaveFile> Action;
    public BoxManipModifyComplex(BoxManipType type, Action<PKM, SaveFile> action) : this(type, action, _ => true) { }
    public BoxManipModifyComplex(BoxManipType type, Action<PKM, SaveFile> action, Func<SaveFile, bool> usable) : base(type, usable) => Action = action;

    public override string GetPrompt(bool all) => string.Empty;
    public override string GetFail(bool all) => string.Empty;
    public override string GetSuccess(bool all) => string.Empty;

    public override int Execute(SaveFile sav, BoxManipParam param)
    {
        var (start, stop, _) = param;
        return sav.ModifyBoxes(pk => Action(pk, sav), start, stop);
    }
}

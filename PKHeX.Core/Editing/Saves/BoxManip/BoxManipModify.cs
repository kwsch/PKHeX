using System;

namespace PKHeX.Core;

/// <summary>
/// Modifies contents of boxes by using an <see cref="Action"/> to change data.
/// </summary>
public sealed class BoxManipModify : BoxManipBase
{
    private readonly Action<PKM> Action;
    public BoxManipModify(BoxManipType type, Action<PKM> action) : this(type, action, _ => true) { }
    public BoxManipModify(BoxManipType type, Action<PKM> action, Func<SaveFile, bool> usable) : base(type, usable) => Action = action;

    public override string GetPrompt(bool all) => string.Empty;
    public override string GetFail(bool all) => string.Empty;
    public override string GetSuccess(bool all) => string.Empty;

    public override int Execute(SaveFile sav, BoxManipParam param)
    {
        var (start, stop, _) = param;
        return sav.ModifyBoxes(Action, start, stop);
    }
}
using System;

namespace PKHeX.Core;

/// <summary>
/// Modifies contents of boxes by using an <see cref="Action"/> to change data.
/// </summary>
public sealed class BoxManipModify(BoxManipType type, Action<PKM> Action, Func<SaveFile, bool> Usable)
    : BoxManipBase(type, Usable)
{
    public BoxManipModify(BoxManipType type, Action<PKM> Action) : this(type, Action, _ => true) { }

    public override string GetPrompt(bool all) => string.Empty;
    public override string GetFail(bool all) => string.Empty;
    public override string GetSuccess(bool all) => string.Empty;

    public override int Execute(SaveFile sav, BoxManipParam param)
    {
        var (start, stop, _) = param;
        return sav.ModifyBoxes(Action, start, stop);
    }
}

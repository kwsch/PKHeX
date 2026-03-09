using System;

namespace PKHeX.Core;

/// <summary>
/// Base class for defining a manipulation of box data.
/// </summary>
public abstract record BoxManipBase(BoxManipType Type, Func<SaveFile, bool> Usable) : IBoxManip
{
    public abstract string GetPrompt(bool all);
    public abstract string GetFail(bool all);
    public abstract string GetSuccess(bool all);
    public abstract int Execute(SaveFile sav, BoxManipParam param);
}

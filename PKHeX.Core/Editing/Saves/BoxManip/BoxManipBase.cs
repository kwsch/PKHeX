using System;

namespace PKHeX.Core;

/// <summary>
/// Base class for defining a manipulation of box data.
/// </summary>
public abstract class BoxManipBase : IBoxManip
{
    public BoxManipType Type { get; }
    public Func<SaveFile, bool> Usable { get; }

    protected BoxManipBase(BoxManipType type, Func<SaveFile, bool> usable)
    {
        Type = type;
        Usable = usable;
    }

    public abstract string GetPrompt(bool all);
    public abstract string GetFail(bool all);
    public abstract string GetSuccess(bool all);
    public abstract int Execute(SaveFile sav, BoxManipParam param);
}

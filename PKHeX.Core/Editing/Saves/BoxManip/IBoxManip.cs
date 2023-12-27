using System;

namespace PKHeX.Core;

/// <summary>
/// Base interface for all box manipulation actions.
/// </summary>
public interface IBoxManip
{
    BoxManipType Type { get; }
    Func<SaveFile, bool> Usable { get; }

    string GetPrompt(bool all);
    string GetFail(bool all);
    string GetSuccess(bool all);

    int Execute(SaveFile sav, BoxManipParam param);
}

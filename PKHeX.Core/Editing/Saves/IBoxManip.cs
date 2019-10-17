using System;

namespace PKHeX.Core
{
    public interface IBoxManip
    {
        BoxManipType Type { get; }
        Func<SaveFile, bool> Usable { get; }

        string GetPrompt(bool all);
        string GetFail(bool all);
        string GetSuccess(bool all);

        int Execute(SaveFile SAV, BoxManipParam param);
    }
}
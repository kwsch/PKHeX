using System;
using PKHeX.Core;

namespace PKHeX.WinForms.Controls;

public interface IMainEditor : IPKMView
{
    PKM Entity { get; }
    SaveFile RequestSaveFile { get; }

    void UpdateIVsGB(bool skipForm);
    void ChangeNature(Nature newNature);
}

[Flags]
public enum UpdateLegalityArgs
{
    None,
    SkipMoveRepopulation = 1 << 0,
}

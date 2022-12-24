using System;

namespace PKHeX.Core;

/// <summary>
/// Provides details about box names within the save file.
/// </summary>
public interface IBoxDetailName
{
    public string GetBoxName(int box);
    public void SetBoxName(int box, ReadOnlySpan<char> value);
}

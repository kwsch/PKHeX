using System;

namespace PKHeX.Core;

/// <summary>
/// Contains information about initial Nickname Details
/// </summary>
public interface IFixedNickname
{
    /// <summary>
    /// Indicates if the Nickname is specified by the encounter template.
    /// </summary>
    bool IsFixedNickname { get; }

    /// <summary>
    /// Checks if the specified nickname matches the encounter template.
    /// </summary>
    /// <param name="pk">Entity to check.</param>
    /// <param name="nickname">Trainer name to check.</param>
    /// <param name="language">Language ID to check with.</param>
    /// <returns>True if matches.</returns>
    bool IsNicknameMatch(PKM pk, ReadOnlySpan<char> nickname, int language);

    /// <summary>
    /// Gets the nickname for the specified language.
    /// </summary>
    /// <param name="language">Language ID to check with.</param>
    /// <returns>Localized nickname.</returns>
    string GetNickname(int language);
}

using System;

namespace PKHeX.Core;

/// <summary>
/// Base interface for all box manipulation actions.
/// </summary>
public interface IBoxManip
{
    /// <summary>
    /// Type of box manipulation action.
    /// </summary>
    BoxManipType Type { get; }

    /// <summary>
    /// Function to determine if the action is usable on the current save file.
    /// </summary>
    Func<SaveFile, bool> Usable { get; }

    /// <summary>
    /// Message to display when asking the user to confirm the action.
    /// </summary>
    /// <param name="all">All boxes or just one?</param>
    string GetPrompt(bool all);

    /// <summary>
    /// Message to display when the action cannot be performed/failed.
    /// </summary>
    /// <param name="all">All boxes or just one?</param>
    string GetFail(bool all);

    /// <summary>
    /// Message to display when the action is successfully completed.
    /// </summary>
    /// <param name="all">All boxes or just one?</param>
    string GetSuccess(bool all);

    /// <summary>
    /// Executes the box manipulation action on the provided save file.
    /// </summary>
    /// <param name="sav">Save file to manipulate.</param>
    /// <param name="param">Parameters for the manipulation action.</param>
    /// <returns>Count of slots modified.</returns>
    int Execute(SaveFile sav, BoxManipParam param);
}

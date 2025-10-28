namespace PKHeX.Core;

/// <summary>
/// Manipulates boxes of a <see cref="SaveFile"/>.
/// </summary>
public abstract class BoxManipulator
{
    protected abstract SaveFile SAV { get; }

    /// <summary>
    /// Executes the provided <see cref="manip"/> with the provided parameters.
    /// </summary>
    /// <param name="manip">Manipulation to perform on the <see cref="SAV"/> box data.</param>
    /// <param name="box">Single box to modify; if <see cref="allBoxes"/> is set, this param is ignored.</param>
    /// <param name="allBoxes">Indicates if all boxes are to be manipulated, or just one box.</param>
    /// <param name="reverse">Manipulation action should be inverted (criteria) or reversed (sort).</param>
    /// <returns>True if operation succeeded, false if no changes made.</returns>
    public bool Execute(IBoxManip manip, int box, bool allBoxes, bool reverse = false)
    {
        bool usable = manip.Usable.Invoke(SAV);
        if (!usable)
            return false;

        var start = allBoxes ? 0 : box;
        var stop = allBoxes ? SAV.BoxCount - 1 : box;
        var param = new BoxManipParam(start, stop, reverse);

        var prompt = manip.GetPrompt(allBoxes);
        var fail = manip.GetFail(allBoxes);
        if (!CanManipulateRegion(param.Start, param.Stop, prompt, fail))
            return false;

        var result = manip.Execute(SAV, param);
        if (result <= 0)
            return false;
        var success = manip.GetSuccess(allBoxes);
        FinishBoxManipulation(success, allBoxes, result);
        return true;
    }

    /// <summary>
    /// Executes the provided <see cref="type"/> with the provided parameters.
    /// </summary>
    /// <param name="type">Manipulation to perform on the <see cref="SAV"/> box data.</param>
    /// <param name="box">Single box to modify; if <see cref="allBoxes"/> is set, this param is ignored.</param>
    /// <param name="allBoxes">Indicates if all boxes are to be manipulated, or just one box.</param>
    /// <param name="reverse">Manipulation action should be inverted (criteria) or reversed (sort).</param>
    /// <returns>True if operation succeeded, false if no changes made.</returns>
    public bool Execute(BoxManipType type, int box, bool allBoxes, bool reverse = false)
    {
        var manip = type.GetManip();
        return Execute(manip, box, allBoxes, reverse);
    }

    /// <summary>
    /// Sanity check for modifying the box data.
    /// </summary>
    /// <param name="start">Start box</param>
    /// <param name="end">End box</param>
    /// <param name="prompt">Message asking about the operation.</param>
    /// <param name="fail">Message indicating the operation cannot be performed.</param>
    protected virtual bool CanManipulateRegion(int start, int end, string prompt, string fail) => !SAV.IsAnySlotLockedInBox(start, end);

    /// <summary>
    /// Called if the <see cref="IBoxManip"/> operation modified any data.
    /// </summary>
    /// <param name="message">Optional message to show if applicable.</param>
    /// <param name="all">Indicates if all boxes were manipulated, or just one box.</param>
    /// <param name="count">Count of manipulated slots</param>
    protected abstract void FinishBoxManipulation(string message, bool all, int count);
}

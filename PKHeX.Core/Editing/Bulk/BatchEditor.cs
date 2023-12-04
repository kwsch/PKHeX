using System;
using System.Collections.Generic;
using System.Diagnostics;
using static PKHeX.Core.MessageStrings;

namespace PKHeX.Core;

/// <summary>
/// Carries out a batch edit and contains information summarizing the results.
/// </summary>
public sealed class BatchEditor
{
    private int Modified { get; set; }
    private int Iterated { get; set; }
    private int Failed { get; set; }

    /// <summary>
    /// Tries to modify the <see cref="PKM"/>.
    /// </summary>
    /// <param name="pk">Object to modify.</param>
    /// <param name="filters">Filters which must be satisfied prior to any modifications being made.</param>
    /// <param name="modifications">Modifications to perform on the <see cref="pk"/>.</param>
    /// <returns>Result of the attempted modification.</returns>
    public bool Process(PKM pk, IEnumerable<StringInstruction> filters, IEnumerable<StringInstruction> modifications)
    {
        if (pk.Species == 0)
            return false;
        if (!pk.Valid)
        {
            Iterated++;
            const string reason = "Not Valid.";
            Debug.WriteLine($"{MsgBEModifyFailBlocked} {reason}");
            return false;
        }

        var result = BatchEditing.TryModifyPKM(pk, filters, modifications);
        if (result != ModifyResult.Invalid)
            Iterated++;
        if (result == ModifyResult.Error)
            Failed++;
        if (result != ModifyResult.Modified)
            return false;

        pk.RefreshChecksum();
        Modified++;
        return true;
    }

    /// <summary>
    /// Gets a message indicating the overall result of all modifications performed across multiple Batch Edit jobs.
    /// </summary>
    /// <param name="sets">Collection of modifications.</param>
    /// <returns>Friendly (multi-line) string indicating the result of the batch edits.</returns>
    public string GetEditorResults(IReadOnlyCollection<StringInstructionSet> sets)
    {
        if (sets.Count == 0)
            return MsgBEInstructionNone;
        int ctr = Modified / sets.Count;
        int len = Iterated / sets.Count;
        string maybe = sets.Count == 1 ? string.Empty : "~";
        string result = string.Format(MsgBEModifySuccess, maybe, ctr, len);
        if (Failed > 0)
            result += Environment.NewLine + maybe + string.Format(MsgBEModifyFailError, Failed);
        return result;
    }

    /// <summary>
    /// Executes the batch instruction <see cref="lines"/> on the input <see cref="data"/>
    /// </summary>
    /// <param name="lines">Batch instruction line(s)</param>
    /// <param name="data">Entities to modify</param>
    /// <returns>Editor object if follow-up modifications are desired.</returns>
    public static BatchEditor Execute(ReadOnlySpan<string> lines, IEnumerable<PKM> data)
    {
        var editor = new BatchEditor();
        var sets = StringInstructionSet.GetBatchSets(lines);
        foreach (var pk in data)
        {
            foreach (var set in sets)
                editor.Process(pk, set.Filters, set.Instructions);
        }

        return editor;
    }

    public void AddSkipped()
    {
        ++Iterated;
    }
}

using System;
using System.Collections.Generic;
using System.Diagnostics;
using static PKHeX.Core.MessageStrings;

namespace PKHeX.Core;

/// <summary>
/// Carries out a batch edit and contains information summarizing the results.
/// </summary>
public sealed class EntityBatchProcessor
{
    private int Modified { get; set; }
    private int Iterated { get; set; }
    private int Failed { get; set; }

    private static EntityBatchEditor Editor => EntityBatchEditor.Instance;

    /// <summary>
    /// Tries to modify the <see cref="PKM"/> using instructions and a custom modifier delegate.
    /// </summary>
    /// <param name="pk">Object to modify.</param>
    /// <param name="filters">Filters which must be satisfied prior to any modifications being made.</param>
    /// <param name="modifications">Modifications to perform on the <see cref="pk"/>.</param>
    /// <param name="modifier">Custom modifier delegate.</param>
    /// <returns>Result of the attempted modification.</returns>
    public bool Process(PKM pk, IEnumerable<StringInstruction> filters, IEnumerable<StringInstruction> modifications, Func<PKM, bool>? modifier = null)
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

        var result = Editor.TryModify(pk, filters, modifications, modifier);
        if (result != ModifyResult.Skipped)
            Iterated++;
        if (result.HasFlag(ModifyResult.Error))
        {
            Failed++;
            result &= ~ModifyResult.Error;
        }
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
    /// <param name="modifier">Custom modifier delegate.</param>
    /// <returns>Editor object if follow-up modifications are desired.</returns>
    public static EntityBatchProcessor Execute(ReadOnlySpan<string> lines, IEnumerable<PKM> data, Func<PKM, bool>? modifier = null)
    {
        var editor = new EntityBatchProcessor();
        var sets = StringInstructionSet.GetBatchSets(lines);
        foreach (var pk in data)
        {
            foreach (var set in sets)
                editor.Process(pk, set.Filters, set.Instructions, modifier);
        }

        return editor;
    }

    public void AddSkipped()
    {
        ++Iterated;
    }
}

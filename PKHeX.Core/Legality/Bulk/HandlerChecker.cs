using System;
using static PKHeX.Core.CheckIdentifier;
using static PKHeX.Core.LegalityCheckResultCode;

namespace PKHeX.Core.Bulk;

/// <summary>
/// Checks for handler-related legality issues among Pokémon in a bulk legality analysis.
/// </summary>
public sealed class HandlerChecker : IBulkAnalyzer
{
    /// <summary>
    /// Analyzes the provided <see cref="BulkAnalysis"/> for handler-related legality issues.
    /// </summary>
    /// <param name="input">The bulk analysis data to check.</param>
    public void Analyze(BulkAnalysis input)
    {
        if (input.Trainer.Generation < 6 || !input.Settings.CheckActiveHandler)
            return; // no HT yet
        CheckHandlerFlag(input);
    }

    private static void CheckHandlerFlag(BulkAnalysis input)
    {
        for (var i = 0; i < input.AllData.Count; i++)
        {
            var la = input.AllAnalysis[i];
            if (!la.Valid)
                continue;
            var cs = input.AllData[i];
            var cr = new CombinedReference(cs, la, i);
            Verify(input, cr);
        }
    }

    private static void Verify(BulkAnalysis input, CombinedReference cr)
    {
        var cs = cr.Slot;
        var la = cr.Analysis;
        var pk = cs.Entity;
        var tr = cs.SAV;
        if (!tr.State.Exportable)
            return; // blank saves should be skipped for checking handler state

        var current = pk.CurrentHandler;

        var shouldBe0 = tr.IsFromTrainer(pk);
        byte expect = shouldBe0 ? (byte)0 : (byte)1;
        if (!HistoryVerifier.IsHandlerStateCorrect(la.EncounterOriginal, pk, current, expect))
            input.AddLine(cs, Trainer, cr.Index, TransferCurrentHandlerInvalid);

        if (current == 1)
            CheckHandlingTrainerEquals(input, cr);
    }

    /// <summary> <see cref="HistoryVerifier.CheckHandlingTrainerEquals"/> </summary>
    private static void CheckHandlingTrainerEquals(BulkAnalysis data, CombinedReference cr)
    {
        var cs = cr.Slot;
        var pk = cs.Entity;
        var tr = cs.SAV;
        Span<char> ht = stackalloc char[pk.TrashCharCountTrainer];
        var len = pk.LoadString(pk.HandlingTrainerTrash, ht);
        ht = ht[..len];

        if (!ht.SequenceEqual(tr.OT))
            data.AddLine(cs, Trainer, cr.Index, TransferHandlerMismatchName);
        if (pk.HandlingTrainerGender != tr.Gender)
            data.AddLine(cs, Trainer, cr.Index, TransferHandlerMismatchGender);

        // If the format exposes a language, check if it matches.
        // Can be mismatched as the game only checks OT/Gender equivalence -- if it matches, don't update everything else.
        // Statistically unlikely that players will play in different languages, but it's technically possible.
        if (pk is IHandlerLanguage h && h.HandlingTrainerLanguage != tr.Language)
            data.AddLine(cs, Trainer, cr.Index, TransferHandlerMismatchLanguage, s: Severity.Fishy);
    }
}

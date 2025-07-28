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
            Verify(input, cs, la);
        }
    }

    private static void Verify(BulkAnalysis input, SlotCache cs, LegalityAnalysis la)
    {
        var pk = cs.Entity;
        var tr = cs.SAV;
        if (!tr.State.Exportable)
            return; // blank saves should be skipped for checking handler state

        var current = pk.CurrentHandler;

        var shouldBe0 = tr.IsFromTrainer(pk);
        byte expect = shouldBe0 ? (byte)0 : (byte)1;
        if (!HistoryVerifier.IsHandlerStateCorrect(la.EncounterOriginal, pk, current, expect))
            input.AddLine(cs, TransferCurrentHandlerInvalid, Trainer);

        if (current == 1)
            CheckHandlingTrainerEquals(input, pk, tr, cs);
    }

    /// <summary> <see cref="HistoryVerifier.CheckHandlingTrainerEquals"/> </summary>
    private static void CheckHandlingTrainerEquals(BulkAnalysis data, PKM pk, SaveFile tr, SlotCache cs)
    {
        Span<char> ht = stackalloc char[pk.TrashCharCountTrainer];
        var len = pk.LoadString(pk.HandlingTrainerTrash, ht);
        ht = ht[..len];

        if (!ht.SequenceEqual(tr.OT))
            data.AddLine(cs, TransferHandlerMismatchName, Trainer);
        if (pk.HandlingTrainerGender != tr.Gender)
            data.AddLine(cs, TransferHandlerMismatchGender, Trainer);

        // If the format exposes a language, check if it matches.
        // Can be mismatched as the game only checks OT/Gender equivalence -- if it matches, don't update everything else.
        // Statistically unlikely that players will play in different languages, but it's technically possible.
        if (pk is IHandlerLanguage h && h.HandlingTrainerLanguage != tr.Language)
            data.AddLine(cs, TransferHandlerMismatchLanguage, Trainer, Severity.Fishy);
    }
}

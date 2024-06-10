using System;
using static PKHeX.Core.CheckIdentifier;
using static PKHeX.Core.LegalityCheckStrings;

namespace PKHeX.Core.Bulk;

public sealed class HandlerChecker : IBulkAnalyzer
{
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
        var current = pk.CurrentHandler;

        var shouldBe0 = tr.IsFromTrainer(pk);
        byte expect = shouldBe0 ? (byte)0 : (byte)1;
        if (!HistoryVerifier.IsHandlerStateCorrect(la.EncounterOriginal, pk, current, expect))
            input.AddLine(cs, LTransferCurrentHandlerInvalid, Trainer);

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
            data.AddLine(cs, LTransferHTMismatchName, Trainer);
        if (pk.HandlingTrainerGender != tr.Gender)
            data.AddLine(cs, LTransferHTMismatchGender, Trainer);

        // If the format exposes a language, check if it matches.
        // Can be mismatched as the game only checks OT/Gender equivalence -- if it matches, don't update everything else.
        // Statistically unlikely that players will play in different languages, but it's technically possible.
        if (pk is IHandlerLanguage h && h.HandlingTrainerLanguage != tr.Language)
            data.AddLine(cs, LTransferHTMismatchLanguage, Trainer, Severity.Fishy);
    }
}

using static PKHeX.Core.CheckIdentifier;

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
            if (!input.AllAnalysis[i].Valid)
                continue;
            var cs = input.AllData[i];
            Verify(input, cs);
        }
    }

    private static void Verify(BulkAnalysis input, SlotCache cs)
    {
        var pk = cs.Entity;
        var tr = cs.SAV;
        var withOT = tr.IsFromTrainer(pk);
        var flag = pk.CurrentHandler;
        var expect = withOT ? 0 : 1;
        if (flag != expect)
            input.AddLine(cs, LegalityCheckStrings.LTransferCurrentHandlerInvalid, Trainer);

        if (flag != 1)
            return;

        if (pk.HandlingTrainerName != tr.OT)
            input.AddLine(cs, LegalityCheckStrings.LTransferHTMismatchName, Trainer);
        if (pk is IHandlerLanguage h && h.HandlingTrainerLanguage != tr.Language)
            input.AddLine(cs, LegalityCheckStrings.LTransferHTMismatchLanguage, Trainer);
    }
}

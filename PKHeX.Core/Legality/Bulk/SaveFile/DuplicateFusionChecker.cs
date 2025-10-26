using System;

namespace PKHeX.Core.Bulk;

public sealed class DuplicateFusionChecker : IBulkAnalyzer
{
    /// <summary>
    /// <see cref="StorageSlotType"/>
    /// </summary>
    /// <param name="input"></param>
    public void Analyze(BulkAnalysis input)
    {
        if (input.Trainer is not SaveFile sav)
            return;

        // Rule: Only 1 Fusion of each of the stored fused species.
        Span<int> seen = stackalloc int[4]; // 0: Kyurem, 1: Necrozma-Dawn, 2: Necrozma-Dusk, 3: Calyrex-Ice/Ghost
        const int seenNotSet = -1;
        seen.Fill(seenNotSet);

        SearchForDuplicates(input, seen, seenNotSet);

        // Rule: The consumed species to obtain (species, form) must be present in the save file's fused storage.
        if (seen.ContainsAnyExcept(seenNotSet))
            CheckConsumedSlots(input, sav, seen, seenNotSet);
    }

    private static void SearchForDuplicates(BulkAnalysis input, Span<int> seen, int seenNotSet)
    {
        for (var i = 0; i < input.AllData.Count; i++)
        {
            var slot = input.AllData[i];
            var pk = slot.Entity;

            var (species, form) = (pk.Species, pk.Form);
            if (form == 0) // eager check -- fused forms always have form > 0
                continue;

            var index = GetIndexFusedStorage(species, form);
            if (index == -1)
                continue;

            if (seen[index] != seenNotSet) // Already given to another slot.
            {
                var other = seen[index];
                input.AddLine(slot, input.AllData[other], CheckIdentifier.Form, i, index2: other, LegalityCheckResultCode.BulkDuplicateFusionSlot, species);
                continue;
            }

            // First time seeing this Fusion, all good.
            seen[index] = i;
        }
    }

    private static void CheckConsumedSlots(BulkAnalysis input, SaveFile sav, Span<int> seen, int seenNotSet)
    {
        // Check that fused species is present in the fused source
        var extraSlots = sav.GetExtraSlots();
        for (int i = 0; i < seen.Length; i++)
        {
            var index = seen[i];
            if (index == seenNotSet)
                continue;

            var extra = extraSlots.Find(z => GetIndexFusedStorage(z.Type) == i);
            if (extra is null) // uhh? shouldn't be in this save file, will be flagged by regular legality check.
                continue;

            var exist = input.AllData[index];
            var pk = exist.Entity;
            var form = pk.Form;

            var source = extra.Read(sav);
            if (source.Form != 0 || !IsValidStoredBaseSpecies(i, source.Species, form))
                input.AddLine(exist, CheckIdentifier.Form, i, LegalityCheckResultCode.BulkFusionSourceInvalid, source.Species, source.Form);
        }
    }

    private static int GetIndexFusedStorage(StorageSlotType type) => type switch
    {
        StorageSlotType.FusedKyurem => 0,
        StorageSlotType.FusedNecrozmaS => 1,
        StorageSlotType.FusedNecrozmaM => 2,
        StorageSlotType.FusedCalyrex => 3,
        _ => -1,
    };

    // The games let you fuse two separate Necrozma with the box legends (N-Solarizer and N-Lunarizer).
    private static int GetIndexFusedStorage(ushort species, byte form) => (species, form) switch
    {
        ((ushort)Species.Kyurem, _) => 0, // DNA Splicers
        ((ushort)Species.Necrozma, 1) => 1, // N-Solarizer
        ((ushort)Species.Necrozma, 2) => 2, // N-Lunarizer
        ((ushort)Species.Calyrex, _) => 3, // Reins of Unity
        _ => -1,
    };

    private static bool IsValidStoredBaseSpecies(int index, ushort consumedSpecies, byte resultForm) => index switch
    {
        0 when resultForm == 1 => consumedSpecies is (ushort)Species.Reshiram,
        0 when resultForm == 2 => consumedSpecies is (ushort)Species.Zekrom,
        1 => consumedSpecies is (ushort)Species.Solgaleo,
        2 => consumedSpecies is (ushort)Species.Lunala,
        3 when resultForm == 1 => consumedSpecies == (ushort)Species.Glastrier,
        3 when resultForm == 2 => consumedSpecies == (ushort)Species.Spectrier,
        _ => false,
    };
}

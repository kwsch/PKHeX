using System.Collections.Generic;
using System.Linq;
using PKHeX.Core;

namespace PKHeX.Application.UseCases;

public readonly record struct BatchImportResult(int Placed, int Skipped);

/// <summary>
/// Places multiple dropped Pokémon files into the next empty slots of a box, in order, reporting
/// how many were placed vs. skipped. Reuses <see cref="FileUtil.GetSupportedFile(string, SaveFile?)"/>
/// and <see cref="SaveExtensions.GetCompatible"/> for detection/conversion (same path as folder import).
/// </summary>
public sealed class BatchImportEntityFilesUseCase
{
    /// <summary>Resolves file paths to entities and places them. Files that aren't Pokémon entities (including save files) are skipped.</summary>
    public BatchImportResult Execute(SaveFile sav, int box, IEnumerable<string> paths)
    {
        var pathList = paths as IReadOnlyCollection<string> ?? paths.ToList();

        var candidates = new List<PKM>();
        foreach (var path in pathList)
        {
            object? obj;
            try { obj = FileUtil.GetSupportedFile(path, sav); }
            catch { continue; }

            switch (obj)
            {
                case PKM pk:
                    candidates.Add(pk);
                    break;
                case MysteryGift { IsEntity: true } g:
                    candidates.Add(g.ConvertToPKM(sav));
                    break;
                case IEncounterInfo { Species: > 0 } g:
                    candidates.Add(g.ConvertToPKM(sav));
                    break;
            }
        }

        var placed = PlaceInBox(sav, box, candidates);
        return new BatchImportResult(placed, pathList.Count - placed);
    }

    /// <summary>
    /// Places already-resolved candidates into the next empty slots of a box, in order.
    /// Pure logic (no file I/O), kept separate so it's directly unit-testable.
    /// </summary>
    public int PlaceInBox(SaveFile sav, int box, IReadOnlyList<PKM> candidates)
    {
        if (candidates.Count == 0)
            return 0;

        var compatible = sav.GetCompatible(candidates);
        var boxSlotCount = sav.BoxSlotCount;
        var slot = 0;
        var placed = 0;

        foreach (var pk in compatible)
        {
            while (slot < boxSlotCount && sav.GetBoxSlotAtIndex(box, slot).Species != 0)
                slot++;
            if (slot >= boxSlotCount)
                break;

            sav.SetBoxSlotAtIndex(pk, box, slot);
            placed++;
            slot++;
        }

        return placed;
    }
}

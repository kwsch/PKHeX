using System;
using System.Collections.Generic;
using PKHeX.Application.Services;
using PKHeX.Core;

namespace PKHeX.Application.UseCases;

/// <summary>Outcome classification for <see cref="LivingDexPlacementUseCase.TryPlace"/>.</summary>
public enum LivingDexPlacementStatus
{
    /// <summary>Every generated Pokémon was written starting at the requested box.</summary>
    Success,

    /// <summary>
    /// There was not enough contiguous empty (unlocked, unprotected) space starting at the requested box
    /// to fit every generated Pokémon. Nothing was written.
    /// </summary>
    InsufficientSpace,
}

/// <summary>
/// Result of a <see cref="LivingDexPlacementUseCase.TryPlace"/> call.
/// </summary>
/// <param name="Status">Outcome classification.</param>
/// <param name="PlacedCount">Number of Pokémon written. Zero unless <see cref="Status"/> is <see cref="LivingDexPlacementStatus.Success"/>.</param>
/// <param name="RequiredSlots">Number of contiguous slots that were needed.</param>
/// <param name="AvailableSlots">Number of contiguous empty/writable slots actually found starting at the requested box.</param>
public sealed record LivingDexPlacementResult(LivingDexPlacementStatus Status, int PlacedCount, int RequiredSlots, int AvailableSlots)
{
    public static LivingDexPlacementResult Ok(int placedCount) =>
        new(LivingDexPlacementStatus.Success, placedCount, placedCount, placedCount);

    public static LivingDexPlacementResult Refuse(int requiredSlots, int availableSlots) =>
        new(LivingDexPlacementStatus.InsufficientSpace, 0, requiredSlots, availableSlots);
}

/// <summary>
/// Places a batch of generated Pokémon (e.g. from the Living Dex generator, issue #123) into a save
/// file's boxes, starting at a user-chosen box. Refuses cleanly, writing nothing, if there is not enough
/// *contiguous* empty space from that box onward — it does not scatter results into gaps further down the
/// boxes. When an <see cref="UndoRedoService"/> is supplied, the whole placement is recorded as a single
/// batched operation (see <see cref="UndoRedoService.BeginBatch"/>), so one Undo reverts every slot this
/// call wrote.
/// </summary>
public sealed class LivingDexPlacementUseCase
{
    public LivingDexPlacementResult TryPlace(SaveFile sav, IReadOnlyList<PKM> pokemon, int startBox, UndoRedoService? undoRedo = null)
    {
        ArgumentNullException.ThrowIfNull(sav);
        ArgumentNullException.ThrowIfNull(pokemon);
        if ((uint)startBox >= (uint)sav.BoxCount)
            throw new ArgumentOutOfRangeException(nameof(startBox));

        if (pokemon.Count == 0)
            return LivingDexPlacementResult.Ok(0);

        var slotsPerBox = sav.BoxSlotCount;
        var startIndex = startBox * slotsPerBox;
        var totalSlots = sav.BoxCount * slotsPerBox;

        // Count the contiguous run of empty, unlocked, unprotected slots starting exactly at startBox/slot
        // 0. The first locked/protected/occupied slot ends the run — we never skip ahead to scavenge
        // slots further down the boxes.
        var contiguous = 0;
        for (var index = startIndex; index < totalSlots && contiguous < pokemon.Count; index++)
        {
            var box = index / slotsPerBox;
            var slot = index % slotsPerBox;

            if (sav.IsBoxSlotLocked(box, slot) || sav.IsBoxSlotOverwriteProtected(box, slot))
                break;
            if (sav.GetBoxSlotAtIndex(box, slot).Species != 0)
                break;

            contiguous++;
        }

        if (contiguous < pokemon.Count)
            return LivingDexPlacementResult.Refuse(requiredSlots: pokemon.Count, availableSlots: contiguous);

        undoRedo?.BeginBatch();
        for (var i = 0; i < pokemon.Count; i++)
        {
            var index = startIndex + i;
            var box = index / slotsPerBox;
            var slot = index % slotsPerBox;

            undoRedo?.AddChange(new SlotInfoBox(box, slot, sav));
            sav.SetBoxSlotAtIndex(pokemon[i], box, slot);
        }
        undoRedo?.EndBatch();

        return LivingDexPlacementResult.Ok(pokemon.Count);
    }
}

namespace PKHeX.Web.State;

/// <summary>What a move or PP Ups edit did to the slot's PP, so the editor can say so (a move change shows its PP effects).</summary>
public enum MoveChangeKind
{
    /// <summary>A new move, given full PP for the PP Ups it kept.</summary>
    MoveSet,

    /// <summary>A new move PP Ups cannot be used on (such as Sketch): its PP Ups were removed and it was given full PP.</summary>
    PpUpsCleared,

    /// <summary>The slot was emptied: its PP and PP Ups are 0.</summary>
    Emptied,

    /// <summary>The slot is back to its stored move, with its stored PP and PP Ups.</summary>
    Restored,

    /// <summary>The PP Ups changed, and the PP with them.</summary>
    PpUpsChanged,
}

/// <summary>A move or PP Ups edit of one slot, and what it did to the slot's PP.</summary>
/// <param name="Slot">The move slot, 0–3.</param>
/// <param name="Kind">What the edit did.</param>
/// <param name="Before">The slot before the edit.</param>
/// <param name="After">The slot after the edit.</param>
public sealed record MoveChange(int Slot, MoveChangeKind Kind, MoveSlot Before, MoveSlot After)
{
    /// <summary>
    /// Describes the edit that turned <paramref name="before"/> into <paramref name="after"/>, or null when it changed neither the move nor
    /// the PP Ups (a PP edit, or the drafted move chosen again).
    /// </summary>
    /// <param name="slot">The move slot, 0–3.</param>
    /// <param name="before">The slot before the edit.</param>
    /// <param name="after">The slot after the edit.</param>
    /// <param name="stored">The slot as stored, before any edit of the draft.</param>
    public static MoveChange? Between(int slot, MoveSlot before, MoveSlot after, MoveSlot stored)
    {
        if (after.Move == before.Move)
        {
            return after.PpUps == before.PpUps ? null : new MoveChange(slot, MoveChangeKind.PpUpsChanged, before, after);
        }
        var kind = after switch
        {
            { IsEmpty: true } => MoveChangeKind.Emptied,
            _ when after == stored => MoveChangeKind.Restored,
            { CanTakePpUps: false } when before.PpUps != 0 => MoveChangeKind.PpUpsCleared,
            _ => MoveChangeKind.MoveSet,
        };
        return new MoveChange(slot, kind, before, after);
    }
}

using PKHeX.Core;

namespace PKHeX.Web.State;

/// <summary>
/// One storage position's decrypted bytes, as compared by <see cref="SlotImages"/>.
/// </summary>
/// <param name="Slot">The party position or box slot.</param>
/// <param name="Bytes">The position's decrypted bytes: party format for a party position, stored format for a box slot.</param>
internal readonly record struct SlotImage(SlotRef Slot, byte[] Bytes);

/// <summary>
/// Snapshots of every party position and box slot of a save, compared before an apply is swapped in, so a write that changed any
/// position other than the one it targeted is refused.
/// </summary>
/// <remarks>
/// <para>
/// Every party position is included, also those at or after <see cref="SaveFile.PartyCount"/>: in game they are empty, but their
/// bytes are part of the file. The comparison covers storage only; other blocks (Pokédex, records) are checked by the tests'
/// whole-file byte-range oracles instead.
/// </para>
/// <para>
/// Untargeted positions must keep every byte, so a readable entity there cannot become unreadable. The targeted position is not
/// compared here; <see cref="SaveSession.Apply(EditorDraft, LegalityVerdict)"/> checks that it reads back as exactly the drafted entity.
/// </para>
/// </remarks>
internal static class SlotImages
{
    /// <summary>Snapshots every party position, then every box slot in box/slot order.</summary>
    public static IReadOnlyList<SlotImage> Of(SaveFile save)
    {
        var images = new List<SlotImage>();
        if (save.HasParty)
        {
            for (var slot = 0; slot < SlotRef.PartyPositions; slot++)
            {
                images.Add(new(SlotRef.InParty(slot), save.GetPartySlotAtIndex(slot).Data.ToArray()));
            }
        }
        if (save.HasBox)
        {
            for (var box = 0; box < save.BoxCount; box++)
            {
                for (var slot = 0; slot < save.BoxSlotCount; slot++)
                {
                    images.Add(new(SlotRef.InBox(box, slot), save.GetBoxSlotAtIndex(box, slot).Data.ToArray()));
                }
            }
        }
        return images;
    }

    /// <summary>
    /// The first position, other than <paramref name="target"/>, whose bytes differ between <paramref name="before"/> and
    /// <paramref name="after"/>, or null when there is none.
    /// </summary>
    /// <remarks>A different list of positions counts as a change at the first position that does not line up.</remarks>
    public static SlotRef? FirstUntargetedChange(IReadOnlyList<SlotImage> before, IReadOnlyList<SlotImage> after, SlotRef target)
    {
        for (var i = 0; i < before.Count; i++)
        {
            var old = before[i];
            if (i >= after.Count || after[i].Slot != old.Slot)
            {
                return old.Slot;
            }
            if (old.Slot != target && !old.Bytes.AsSpan().SequenceEqual(after[i].Bytes))
            {
                return old.Slot;
            }
        }
        return after.Count > before.Count ? after[before.Count].Slot : null;
    }
}

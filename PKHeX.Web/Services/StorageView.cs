using PKHeX.Core;
using PKHeX.Web.State;

namespace PKHeX.Web.Services;

/// <summary>
/// What one party position or box slot holds, as Core reads it. Values are typed and carry no display text; the UI formats them.
/// </summary>
/// <param name="Ref">The position.</param>
/// <param name="Occupied">True when the position holds an entity.</param>
/// <param name="Readable">
/// True when the entity passes its checksum and sanity check (<see cref="PKM.Valid"/>), so it can be opened. An entity that fails is a
/// bad egg, as the game and WinForms show it. Always true for an empty position.
/// </param>
/// <param name="Species">The stored species, or 0 when empty or unreadable.</param>
/// <param name="Nickname">The nickname when the entity is nicknamed, otherwise null. Eggs carry the game's egg name here.</param>
/// <param name="IsEgg">True for an egg.</param>
/// <param name="IsShiny">True for a shiny entity.</param>
/// <param name="Form">The stored form, or 0 when empty or unreadable. Used only to choose the sprite.</param>
/// <param name="Gender">The stored gender, or 0 when empty or unreadable. Used only to choose the sprite.</param>
/// <param name="HoldsItem">True when the entity holds an item. Used only to choose how an egg's sprite is drawn.</param>
public sealed record SlotSummary(SlotRef Ref, bool Occupied, bool Readable, ushort Species, string? Nickname, bool IsEgg, bool IsShiny, byte Form = 0, byte Gender = 0, bool HoldsItem = false)
{
    /// <summary>True when selecting the position opens a draft: it is occupied and not a bad egg.</summary>
    public bool CanOpen => Occupied && Readable;
}

/// <summary>One box and its slots.</summary>
/// <param name="Index">The zero-based box index.</param>
/// <param name="StoredName">The name stored in the save, or null when it stores none (the UI then numbers the box).</param>
/// <param name="Slots">Every slot of the box, in order, sized by the save's <see cref="SaveFile.BoxSlotCount"/>.</param>
public sealed record BoxView(int Index, string? StoredName, IReadOnlyList<SlotSummary> Slots);

/// <summary>
/// Read-only views of a session's party and boxes, read from the current revision each time they are asked for.
/// </summary>
/// <remarks>
/// Only what is shown is read: the party and a single box. Nothing is cached between revisions, so an empty position can never
/// show, or open, an entity from an earlier state of the save. No legality analysis runs here; it is costly and is run only for
/// the selected entity.
/// </remarks>
public static class StorageView
{
    /// <summary>Number of boxes in the session's save.</summary>
    public static int BoxCount(SaveSession session) => session.Working.BoxCount;

    /// <summary>
    /// Every party position, occupied or not. Positions at or after <see cref="SaveFile.PartyCount"/> are empty.
    /// </summary>
    public static IReadOnlyList<SlotSummary> Party(SaveSession session)
    {
        var save = session.Working;
        return Enumerable.Range(0, SlotRef.PartyPositions).Select(i => Summarise(save, SlotRef.InParty(i))).ToArray();
    }

    /// <summary>
    /// Box <paramref name="box"/> with its stored name and every slot.
    /// </summary>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="box"/> is not a box of the save.</exception>
    public static BoxView Box(SaveSession session, int box)
    {
        var save = session.Working;
        ArgumentOutOfRangeException.ThrowIfNegative(box);
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(box, save.BoxCount);
        var slots = Enumerable.Range(0, save.BoxSlotCount).Select(i => Summarise(save, SlotRef.InBox(box, i))).ToArray();
        return new BoxView(box, BoxName(session, box), slots);
    }

    /// <summary>
    /// The name stored for box <paramref name="box"/>, or null when the save stores none or a blank one. Reads no slots.
    /// </summary>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="box"/> is not a box of the save.</exception>
    public static string? BoxName(SaveSession session, int box)
    {
        var save = session.Working;
        ArgumentOutOfRangeException.ThrowIfNegative(box);
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(box, save.BoxCount);
        var name = save is IBoxDetailNameRead names ? names.GetBoxName(box) : null;
        return string.IsNullOrWhiteSpace(name) ? null : name;
    }

    /// <summary>The in-game current box of the session's save, or 0 when the stored value is not a box.</summary>
    public static int InitialBox(SaveSession session)
    {
        var save = session.Working;
        return (uint)save.CurrentBox < (uint)save.BoxCount ? save.CurrentBox : 0;
    }

    private static SlotSummary Summarise(SaveFile save, SlotRef slot)
    {
        if (SaveSession.ReadOccupied(save, slot) is not { } entity)
        {
            return new SlotSummary(slot, false, true, 0, null, false, false);
        }
        if (!entity.Valid)
        {
            // A bad egg: nothing read from it can be trusted, so none of it is shown.
            return new SlotSummary(slot, true, false, 0, null, false, false);
        }
        return new SlotSummary(slot, true, true, entity.Species, entity.IsNicknamed ? entity.Nickname : null, entity.IsEgg, entity.IsShiny,
            entity.Form, entity.Gender, entity.SpriteItem != 0);
    }
}

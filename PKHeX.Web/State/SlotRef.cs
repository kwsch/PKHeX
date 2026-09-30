using PKHeX.Core;

namespace PKHeX.Web.State;

/// <summary>
/// A storage position in a save: a party position or a box slot. It names the position only, never what it holds.
/// </summary>
/// <remarks>
/// Positions are zero-based, as Core addresses them. Create values through <see cref="InParty"/> or <see cref="InBox"/>;
/// the default value is box 0, slot 0.
/// </remarks>
public readonly record struct SlotRef
{
    /// <summary>Number of party positions. Core stores six in every save with a party, but keeps its own constant private.</summary>
    public const int PartyPositions = 6;

    private SlotRef(bool isParty, int box, int slot)
    {
        IsParty = isParty;
        Box = box;
        Slot = slot;
    }

    /// <summary>True for a party position, false for a box slot.</summary>
    public bool IsParty { get; }

    /// <summary>The box index; 0 for a party position.</summary>
    public int Box { get; }

    /// <summary>The slot within the box, or the party position.</summary>
    public int Slot { get; }

    /// <summary>Party position <paramref name="index"/>.</summary>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> is negative.</exception>
    public static SlotRef InParty(int index)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(index);
        return new(true, 0, index);
    }

    /// <summary>Slot <paramref name="slot"/> of box <paramref name="box"/>.</summary>
    /// <exception cref="ArgumentOutOfRangeException">Either value is negative.</exception>
    public static SlotRef InBox(int box, int slot)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(box);
        ArgumentOutOfRangeException.ThrowIfNegative(slot);
        return new(false, box, slot);
    }

    /// <summary>True when the position exists in <paramref name="save"/>.</summary>
    public bool IsWithin(SaveFile save) => IsParty
        ? save.HasParty && Slot < PartyPositions
        : save.HasBox && Box < save.BoxCount && Slot < save.BoxSlotCount;

    /// <summary>Core's slot for this position on <paramref name="save"/>, used to read, write and type the entity there.</summary>
    internal ISlotInfo ToSlotInfo(SaveFile save) => IsParty
        ? new SlotInfoParty(Slot)
        : new SlotInfoBox(Box, Slot, save);
}

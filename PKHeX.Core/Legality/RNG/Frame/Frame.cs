using System.Diagnostics;

namespace PKHeX.Core;

/// <summary>
/// Represents an RNG seed and the conditions of which it occurs.
/// </summary>
/// <param name="Seed">Ending seed value for the frame (prior to nature call).</param>
/// <param name="FrameType"></param>
/// <param name="Lead"></param>
[DebuggerDisplay($"{{{nameof(FrameType)},nq}}[{{{nameof(Lead)},nq}}]")]
public sealed record Frame(uint Seed, FrameType FrameType, LeadRequired Lead)
{
    /// <summary>
    /// Starting seed for the frame (to generate the frame).
    /// </summary>
    public uint OriginSeed { get; set; }

    /// <summary>
    /// RNG Call Value for the Level Calc
    /// </summary>
    public ushort RandLevel { get; set; }

    /// <summary>
    /// RNG Call Value for the Encounter Slot Calc
    /// </summary>
    public ushort RandESV { get; set; }

    public bool LevelSlotModified => Lead.IsLevelOrSlotModified() || (Lead & LeadRequired.UsesLevelCall) != 0;

    /// <summary>
    /// Checks the Encounter Slot for RNG calls before the Nature loop.
    /// </summary>
    /// <param name="slot">Slot Data</param>
    /// <param name="pk">Ancillary pk data for determining how to check level.</param>
    /// <returns>Slot number for this frame &amp; lead value.</returns>
    public bool IsSlotCompatibile<T>(T slot, PKM pk) where T : IMagnetStatic, INumberedSlot, ISlotRNGType, ILevelRange
    {
        // The only level rand type slots are Honey Tree and National Park BCC
        // Gen3 always does level rand, but the level ranges are same min,max.
        if (FrameType != FrameType.MethodH)
        {
            bool hasLevelCall = slot.IsRandomLevel();
            if (Lead.NeedsLevelCall() != hasLevelCall)
                return false;
        }

        if (slot.Type is not (SlotType.HoneyTree))
        {
            int calcSlot = GetSlot(slot);
            if (calcSlot != slot.SlotNumber)
                return false;
        }

        // Check Level Now
        var lvl = SlotRange.GetLevel(slot, Lead, RandLevel);
        if (pk.HasOriginalMetLocation)
        {
            if (lvl != pk.Met_Level)
                return false;
        }
        else
        {
            if (lvl > pk.Met_Level)
                return false;
        }

        // Check if the slot is actually encounterable (considering Sweet Scent)
        bool encounterable = SlotRange.GetIsEncounterable(slot, FrameType, (int)(OriginSeed >> 16), Lead);
        return encounterable;
    }

    /// <summary>
    /// Gets the slot value for the input slot.
    /// </summary>
    /// <param name="slot">Slot Data</param>
    /// <returns>Slot number for this frame &amp; lead value.</returns>
    private int GetSlot<T>(T slot) where T : IMagnetStatic, INumberedSlot, ISlotRNGType
    {
        // Static and Magnet Pull do a slot search rather than slot mapping 0-99.
        return Lead != LeadRequired.StaticMagnet
            ? SlotRange.GetSlot(slot.Type, RandESV, FrameType)
            : SlotRange.GetSlotStaticMagnet(slot, RandESV);
    }

    /// <summary>
    /// Only use this for test methods.
    /// </summary>
    /// <param name="t"></param>
    public int GetSlot(SlotType t) => SlotRange.GetSlot(t, RandESV, FrameType);
}

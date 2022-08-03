namespace PKHeX.Core;

/// <summary>
/// Encounter Slot found in <see cref="GameVersion.Gen3"/>.
/// </summary>
/// <inheritdoc cref="EncounterSlot"/>
public record EncounterSlot3 : EncounterSlot, IMagnetStatic, INumberedSlot, ISlotRNGType
{
    public sealed override int Generation => 3;
    public override EntityContext Context => EntityContext.Gen3;

    public byte StaticIndex { get; }
    public byte MagnetPullIndex { get; }
    public byte StaticCount { get; }
    public byte MagnetPullCount { get; }
    public SlotType Type => Area.Type;

    public byte SlotNumber { get; }
    public override Ball FixedBall => Locations.IsSafariZoneLocation3(Location) ? Ball.Safari : Ball.None;

    public EncounterSlot3(EncounterArea3 area, ushort species, byte form, byte min, byte max, byte slot, byte mpi, byte mpc, byte sti, byte stc) : base(area, species, form, min, max)
    {
        SlotNumber = slot;

        MagnetPullIndex = mpi;
        MagnetPullCount = mpc;

        StaticIndex = sti;
        StaticCount = stc;
    }

    public override EncounterMatchRating GetMatchRating(PKM pk)
    {
        if (IsDeferredSafari3(pk.Ball == (int)Ball.Safari))
            return EncounterMatchRating.PartialMatch;
        return base.GetMatchRating(pk);
    }

    private bool IsDeferredSafari3(bool IsSafariBall) => IsSafariBall != Locations.IsSafariZoneLocation3(Location);
}

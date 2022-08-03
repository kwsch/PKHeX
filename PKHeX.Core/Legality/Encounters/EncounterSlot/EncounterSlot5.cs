namespace PKHeX.Core;

/// <summary>
/// Encounter Slot found in <see cref="GameVersion.Gen5"/>.
/// </summary>
/// <inheritdoc cref="EncounterSlot"/>
public sealed record EncounterSlot5 : EncounterSlot
{
    public override int Generation => 5;
    public override EntityContext Context => EntityContext.Gen5;

    public EncounterSlot5(EncounterArea5 area, ushort species, byte form, byte min, byte max) : base(area, species, form, min, max)
    {
    }

    public bool IsHiddenGrotto => Area.Type == SlotType.HiddenGrotto;

    protected override HiddenAbilityPermission IsHiddenAbilitySlot() => Area.Type == SlotType.HiddenGrotto ? HiddenAbilityPermission.Always : HiddenAbilityPermission.Never;
}

namespace PKHeX.Core;

/// <summary>
/// Encounter Slot found in <see cref="GameVersion.XY"/>.
/// </summary>
/// <inheritdoc cref="EncounterSlot"/>
public sealed record EncounterSlot6XY : EncounterSlot
{
    public override int Generation => 6;
    public override EntityContext Context => EntityContext.Gen6;
    public bool Pressure { get; init; }
    public bool IsFriendSafari => Area.Type == SlotType.FriendSafari;
    public bool IsHorde => Area.Type == SlotType.Horde;

    public EncounterSlot6XY(EncounterArea6XY area, int species, int form, byte min, byte max) : base(area, species, form, min, max)
    {
    }

    protected override void SetFormatSpecificData(PKM pk)
    {
        var pk6 = (PK6)pk;
        pk6.SetRandomMemory6();
        pk6.SetRandomEC();
    }

    public override string GetConditionString(out bool valid)
    {
        valid = true;
        return Pressure ? LegalityCheckStrings.LEncConditionLead : LegalityCheckStrings.LEncCondition;
    }

    public EncounterSlot6XY CreatePressureFormCopy(int form) => this with {Form = form, Pressure = true};

    protected override HiddenAbilityPermission IsHiddenAbilitySlot() => IsHorde || IsFriendSafari ? HiddenAbilityPermission.Possible : HiddenAbilityPermission.Never;
}

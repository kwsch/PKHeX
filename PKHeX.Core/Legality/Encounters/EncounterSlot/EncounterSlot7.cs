namespace PKHeX.Core;

/// <summary>
/// Encounter Slot found in <see cref="GameVersion.Gen7"/>.
/// </summary>
/// <inheritdoc cref="EncounterSlot"/>
public sealed record EncounterSlot7 : EncounterSlot
{
    public override int Generation => 7;
    public override EntityContext Context => EntityContext.Gen7;
    public bool IsSOS => Area.Type == SlotType.SOS;

    public EncounterSlot7(EncounterArea7 area, ushort species, byte form, byte min, byte max) : base(area, species, form, min, max)
    {
    }

    protected override void SetPINGA(PKM pk, EncounterCriteria criteria)
    {
        var pi = pk.PersonalInfo;
        pk.PID = Util.Rand32();
        pk.Nature = (int)criteria.GetNature(Nature.Random);
        pk.Gender = criteria.GetGender(-1, pi);
        criteria.SetRandomIVs(pk);

        var num = Ability;
        if (IsSOS && pk.FlawlessIVCount < 2)
            num = 0; // let's fake it as an insufficient chain, no HA possible.
        var ability = criteria.GetAbilityFromNumber(num);
        pk.RefreshAbility(ability);
        pk.SetRandomEC();
    }

    protected override HiddenAbilityPermission IsHiddenAbilitySlot() => IsSOS ? HiddenAbilityPermission.Possible : HiddenAbilityPermission.Never;

    public override Ball FixedBall => Location == Locations.Pelago7 ? Ball.Poke : Ball.None;
}

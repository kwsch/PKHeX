namespace PKHeX.Core;

public sealed class EntityEditorSettings
{
    [LocalizedDescription("When changing the Hidden Power type, automatically maximize the IVs to ensure the highest Base Power result. Otherwise, keep the IVs as close as possible to the original.")]
    public bool HiddenPowerOnChangeMaxPower { get; set; } = true;

    [LocalizedDescription("When showing the list of balls to select, show the legal balls before the illegal balls rather than sorting by Ball ID.")]
    public bool ShowLegalBallsFirst { get; set; } = true;

    [LocalizedDescription("When showing a Generation 1 format entity, show the gender it would have if transferred to other generations.")]
    public bool ShowGenderGen1 { get; set; }

    [LocalizedDescription("When showing an entity, show any stored Status Condition (Sleep/Burn/etc) it may have.")]
    public bool ShowStatusCondition { get; set; } = true;
}

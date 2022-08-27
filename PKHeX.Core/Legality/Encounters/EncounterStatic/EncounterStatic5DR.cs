namespace PKHeX.Core;

/// <summary>
/// Generation 5 Dream Radar gift encounters
/// </summary>
/// <inheritdoc cref="EncounterStatic"/>
public sealed record EncounterStatic5DR : EncounterStatic5
{
    public EncounterStatic5DR(ushort species, byte form, AbilityPermission ability = AbilityPermission.OnlyHidden) : base(GameVersion.B2W2)
    {
        Species = species;
        Form = form;
        Ability = ability;
        Location = 30015;
        Gift = true;
        Ball = 25;
        Level = 5; // to 40
        Shiny = Shiny.Never;
    }

    protected override bool IsMatchLevel(PKM pk, EvoCriteria evo)
    {
        // Level from 5->40 depends on the number of badges
        var met = pk.Met_Level;
        if (met % 5 != 0)
            return false;
        return (uint) (met - 5) <= 35; // 5 <= x <= 40
    }
}

namespace PKHeX.Core;

/// <summary>
/// Generation 4 Pokéwalker  Encounter
/// </summary>
/// <inheritdoc cref="EncounterStatic"/>
public sealed record EncounterStatic4Pokewalker : EncounterStatic
{
    public override int Generation => 4;
    public override EntityContext Context => EntityContext.Gen4;

    public EncounterStatic4Pokewalker(ushort species, sbyte gender, byte level) : base(GameVersion.HGSS)
    {
        Species = species;
        Gender = gender;
        Level = level;
        Gift = true;
        Location = Locations.PokeWalker4;
    }

    protected override bool IsMatchLocation(PKM pk)
    {
        if (pk.Format == 4)
            return Location == pk.Met_Location;
        return true; // transfer location verified later
    }

    protected override bool IsMatchLevel(PKM pk, EvoCriteria evo)
    {
        if (pk.Format != 4) // Met Level lost on PK4=>PK5
            return Level <= evo.LevelMax;

        return pk.Met_Level == Level;
    }

    protected override bool IsMatchPartial(PKM pk)
    {
        if (Gift && pk.Ball != Ball)
            return true;
        return base.IsMatchPartial(pk);
    }

    protected override void SetPINGA(PKM pk, EncounterCriteria criteria)
    {
        var pi = pk.PersonalInfo;
        int gender = criteria.GetGender(Gender, pi);
        int nature = (int)criteria.GetNature(Nature.Random);

        // Cannot force an ability; nature-gender-trainerID only yield fixed PIDs.
        // int ability = criteria.GetAbilityFromNumber(Ability, pi);

        PIDGenerator.SetRandomPIDPokewalker(pk, nature, gender);
        criteria.SetRandomIVs(pk);
    }
}

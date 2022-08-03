namespace PKHeX.Core;

/// <summary>
/// Generation 5 Trade Encounter
/// </summary>
/// <inheritdoc cref="EncounterTrade"/>
public sealed record EncounterTrade5(GameVersion Version) : EncounterTrade(Version)
{
    public override int Generation => 5;
    public override EntityContext Context => EntityContext.Gen5;
    public override int Location => Locations.LinkTrade5NPC;
}

/// <summary>Generation 5 Trade with Fixed PID</summary>
/// <param name="PID"> Fixed <see cref="PKM.PID"/> value the encounter must have.</param>
public sealed record EncounterTrade5PID(GameVersion Version, uint PID) : EncounterTrade(Version)
{
    public override int Generation => 5;
    public override EntityContext Context => EntityContext.Gen5;
    public override int Location => Locations.LinkTrade5NPC;

    public override Shiny Shiny => Shiny.FixedValue;

    protected override void ApplyDetails(ITrainerInfo sav, EncounterCriteria criteria, PKM pk)
    {
        base.ApplyDetails(sav, criteria, pk);

        // Trades for JPN games have language ID of 0, not 1.
        if (pk.Language == (int) LanguageID.Japanese)
            pk.Language = 0;
    }

    protected override void SetPINGA(PKM pk, EncounterCriteria criteria)
    {
        var pi = pk.PersonalInfo;
        int gender = criteria.GetGender(EntityGender.GetFromPID(Species, PID), pi);
        int nature = (int)criteria.GetNature(Nature);
        int ability = criteria.GetAbilityFromNumber(Ability);

        pk.PID = PID;
        pk.Nature = nature;
        pk.Gender = gender;
        pk.RefreshAbility(ability);

        SetIVs(pk);
    }

    protected override bool IsMatchNatureGenderShiny(PKM pk)
    {
        if (PID != pk.EncryptionConstant)
            return false;
        if (Nature != Nature.Random && (int)Nature != pk.Nature) // gen5 BW only
            return false;
        return true;
    }

    public static bool IsValidMissingLanguage(PKM pk)
    {
        return pk.Format == 5 && pk.BW;
    }
}

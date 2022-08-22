namespace PKHeX.Core;

/// <summary>
/// Generation 7 Static Encounter
/// </summary>
/// <inheritdoc cref="EncounterStatic"/>
public sealed record EncounterStatic7(GameVersion Version) : EncounterStatic(Version), IRelearn
{
    public override int Generation => 7;
    public override EntityContext Context => EntityContext.Gen7;
    public Moveset Relearn { get; init; }

    public bool IsTotem => FormInfo.IsTotemForm(Species, Form);
    public bool IsTotemNoTransfer => Legal.Totem_NoTransfer.Contains(Species);
    public int GetTotemBaseForm() => FormInfo.GetTotemBaseForm(Species, Form);

    protected override bool IsMatchLocation(PKM pk)
    {
        if (EggLocation == Locations.Daycare5 && !Relearn.HasMoves && pk.RelearnMove1 != 0) // Gift Eevee edge case
            return false;
        return base.IsMatchLocation(pk);
    }

    protected override bool IsMatchEggLocation(PKM pk)
    {
        if (!EggEncounter)
            return base.IsMatchEggLocation(pk);

        var eggloc = pk.Egg_Location;
        if (!pk.IsEgg) // hatched
            return eggloc == EggLocation || eggloc == Locations.LinkTrade6;

        // Unhatched:
        if (eggloc != EggLocation)
            return false;
        if (pk.Met_Location is not (0 or Locations.LinkTrade6))
            return false;
        return true;
    }

    protected override bool IsMatchForm(PKM pk, EvoCriteria evo)
    {
        if (IsTotem)
        {
            var expectForm = pk.Format == 7 ? Form : FormInfo.GetTotemBaseForm(Species, Form);
            return expectForm == evo.Form;
        }
        return base.IsMatchForm(pk, evo);
    }

    protected override void ApplyDetails(ITrainerInfo tr, EncounterCriteria criteria, PKM pk)
    {
        base.ApplyDetails(tr, criteria, pk);
        if (Species == (int)Core.Species.Magearna && pk is IRibbonSetEvent4 e4)
            e4.RibbonWishing = true;
        if (Form == FormVivillon && pk is PK7 pk7)
            pk.Form = Vivillon3DS.GetPattern(pk7.Country, pk7.Region);
        pk.SetRandomEC();
    }

    internal static EncounterStatic7 GetVC1(int species, byte metLevel)
    {
        bool mew = species == (int)Core.Species.Mew;
        return new EncounterStatic7(GameVersion.RBY)
        {
            Species = species,
            Gift = true, // Forces Poké Ball
            Ability = Legal.TransferSpeciesDefaultAbilityGen1(species) ? AbilityPermission.OnlyFirst : AbilityPermission.OnlyHidden, // Hidden by default, else first
            Shiny = mew ? Shiny.Never : Shiny.Random,
            Fateful = mew,
            Location = Locations.Transfer1,
            Level = metLevel,
            FlawlessIVCount = mew ? (byte)5 : (byte)3,
        };
    }

    internal static EncounterStatic7 GetVC2(int species, byte metLevel)
    {
        bool mew = species == (int)Core.Species.Mew;
        bool fateful = mew || species == (int)Core.Species.Celebi;
        return new EncounterStatic7(GameVersion.GSC)
        {
            Species = species,
            Gift = true, // Forces Poké Ball
            Ability = Legal.TransferSpeciesDefaultAbilityGen2(species) ? AbilityPermission.OnlyFirst : AbilityPermission.OnlyHidden, // Hidden by default, else first
            Shiny = mew ? Shiny.Never : Shiny.Random,
            Fateful = fateful,
            Location = Locations.Transfer2,
            Level = metLevel,
            FlawlessIVCount = fateful ? (byte)5 : (byte)3,
        };
    }
}

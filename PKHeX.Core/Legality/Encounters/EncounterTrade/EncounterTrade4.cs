namespace PKHeX.Core;

/// <summary>
/// Generation 4 Trade Encounter
/// </summary>
/// <inheritdoc cref="EncounterTradeGB"/>
public abstract record EncounterTrade4(GameVersion Version) : EncounterTrade(Version)
{
    public sealed override int Generation => 4;
    public override EntityContext Context => EntityContext.Gen4;

    protected static readonly string[] RanchOTNames = { string.Empty, "ユカリ", "Hayley", "EULALIE", "GIULIA", "EUKALIA", string.Empty, "Eulalia" };
}

/// <summary>
/// Generation 4 Trade Encounter with a fixed PID value.
/// </summary>
/// <inheritdoc cref="EncounterTrade4"/>
public sealed record EncounterTrade4PID : EncounterTrade4, IContestStats
{
    /// <summary>
    /// Fixed <see cref="PKM.PID"/> value the encounter must have.
    /// </summary>
    public readonly uint PID;

    public override Shiny Shiny => Shiny.FixedValue;

    public EncounterTrade4PID(GameVersion game, uint pid, ushort species, byte level) : base(game)
    {
        PID = pid;
        Species = species;
        Level = level;
    }

    public byte CNT_Cool { get; init; }
    public byte CNT_Beauty { get; init; }
    public byte CNT_Cute { get; init; }
    public byte CNT_Smart { get; init; }
    public byte CNT_Tough { get; init; }
    public byte CNT_Sheen { get; init; }

    public byte Contest
    {
        init
        {
            CNT_Cool = value;
            CNT_Beauty = value;
            CNT_Cute = value;
            CNT_Smart = value;
            CNT_Tough = value;
            //CNT_Sheen = value;
        }
    }

    public int MetLocation { get; init; }
    public override int Location => MetLocation == default ? Locations.LinkTrade4NPC : MetLocation;

    public override bool IsMatchExact(PKM pk, EvoCriteria evo)
    {
        if (!base.IsMatchExact(pk, evo))
            return false;

        if (pk is IContestStats s && s.IsContestBelow(this))
            return false;

        return true;
    }

    protected override void ApplyDetails(ITrainerInfo sav, EncounterCriteria criteria, PKM pk)
    {
        base.ApplyDetails(sav, criteria, pk);
        var pk4 = (PK4) pk;

        if (Version == GameVersion.DPPt)
        {
            // Has German Language ID for all except German origin, which is English
            if (Species == (int)Core.Species.Magikarp)
                pk4.Language = (int)(pk4.Language == (int)LanguageID.German ? LanguageID.English : LanguageID.German);
            // All other trades received (DP only): English games have a Japanese language ID instead of English.
            else if (pk4.Version is not (int)GameVersion.Pt && pk4.Language == (int)LanguageID.English)
                pk4.Language = (int)LanguageID.Japanese;
        }
        else // HGSS
        {
            // Has English Language ID for all except English origin, which is French
            if (Species == (int)Core.Species.Pikachu)
                pk4.Language = (int)(pk4.Language == (int)LanguageID.English ? LanguageID.French : LanguageID.English);
        }

        this.CopyContestStatsTo(pk4);
    }

    protected override void SetPINGA(PKM pk, EncounterCriteria criteria)
    {
        pk.PID = PID;
        pk.Nature = (int)(PID % 25);
        pk.Gender = Gender;
        pk.RefreshAbility(Ability.GetSingleValue());
        SetIVs(pk);
    }

    protected override bool IsMatchNatureGenderShiny(PKM pk)
    {
        return PID == pk.EncryptionConstant;
    }
}

/// <summary>
/// Generation 4 Trade Encounter with a fixed PID value, met location, and version.
/// </summary>
/// <inheritdoc cref="EncounterTradeGB"/>
public sealed record EncounterTrade4RanchGift : EncounterTrade4
{
    /// <summary>
    /// Fixed <see cref="PKM.PID"/> value the encounter must have.
    /// </summary>
    public readonly uint PID;

    public int MetLocation { private get; init; }
    public override int Location => MetLocation;
    public override Shiny Shiny => Shiny.FixedValue;

    public EncounterTrade4RanchGift(uint pid, ushort species, byte level) : base(GameVersion.D)
    {
        PID = pid;
        Species = species;
        Level = level;
        TrainerNames = RanchOTNames;
    }

    protected override bool IsMatchNatureGenderShiny(PKM pk)
    {
        return PID == pk.EncryptionConstant;
    }

    protected override void SetPINGA(PKM pk, EncounterCriteria criteria)
    {
        pk.PID = PID;
        pk.Nature = (int)(PID % 25);
        pk.Gender = Gender;
        pk.RefreshAbility(Ability.GetSingleValue());
        SetIVs(pk);
    }
}

/// <summary>
/// Generation 4 Trade Encounter with a fixed location and version, as well as special details.
/// </summary>
/// <inheritdoc cref="EncounterTrade4"/>
public sealed record EncounterTrade4RanchSpecial : EncounterTrade4
{
    public override int Location => 3000;

    public EncounterTrade4RanchSpecial(ushort species, byte level) : base(GameVersion.D)
    {
        Species = species;
        Level = level;
        Ball = 0x10;
        OTGender = 1;
        TrainerNames = RanchOTNames;
    }

    protected override void ApplyDetails(ITrainerInfo sav, EncounterCriteria criteria, PKM pk)
    {
        base.ApplyDetails(sav, criteria, pk);
        pk.FatefulEncounter = true;
    }
}

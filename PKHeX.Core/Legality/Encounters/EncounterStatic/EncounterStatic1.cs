namespace PKHeX.Core;

/// <summary>
/// Generation 1 Static Encounter
/// </summary>
/// <inheritdoc cref="EncounterStatic"/>
public record EncounterStatic1 : EncounterStatic
{
    public override int Generation => 1;
    public override EntityContext Context => EntityContext.Gen1;
    public sealed override byte Level { get; init; }

    private const int LightBallPikachuCatchRate = 0xA3; // 163

    public EncounterStatic1(byte species, byte level, GameVersion game) : base(game)
    {
        Species = species;
        Level = level;
    }

    protected override void ApplyDetails(ITrainerInfo tr, EncounterCriteria criteria, PKM pk)
    {
        base.ApplyDetails(tr, criteria, pk);

        var pk1 = (PK1) pk;
        if (Species == (int) Core.Species.Pikachu && Version == GameVersion.YW && Level == 5 && Moves.Count == 0)
        {
            pk1.Catch_Rate = LightBallPikachuCatchRate; // Light Ball
            return;
        }

        // Encounters can have different Catch Rates (RBG vs Y)
        var table = Version == GameVersion.YW ? PersonalTable.Y : PersonalTable.RB;
        pk1.Catch_Rate = (byte)table[Species].CatchRate;
    }

    protected override bool IsMatchLevel(PKM pk, EvoCriteria evo)
    {
        // Met Level is not stored in the PK1 format.
        // Check if it is at or above the encounter level.
        return Level <= evo.LevelMax;
    }

    protected override bool IsMatchLocation(PKM pk)
    {
        // Met Location is not stored in the PK1 format.
        return true;
    }

    public override bool IsMatchExact(PKM pk, EvoCriteria evo)
    {
        if (!base.IsMatchExact(pk, evo))
            return false;

        // Encounters with this version have to originate from the Japanese Blue game.
        if (!pk.Japanese && Version == GameVersion.BU)
            return false;

        return true;
    }

    protected override bool IsMatchPartial(PKM pk)
    {
        if (pk is not PK1 pk1)
            return false;
        if (ParseSettings.AllowGen1Tradeback && PK1.IsCatchRateHeldItem(pk1.Catch_Rate))
            return false;
        if (IsCatchRateValid(pk1))
            return false;
        return true;
    }

    private bool IsCatchRateValid(PK1 pk1)
    {
        var catch_rate = pk1.Catch_Rate;

        // Light Ball (Yellow) starter
        if (Version == GameVersion.YW && Species == (int)Core.Species.Pikachu && Level == 5)
        {
            return catch_rate == LightBallPikachuCatchRate;
        }
        if (Version == GameVersion.Stadium)
        {
            // Amnesia Psyduck has different catch rates depending on language
            if (Species == (int)Core.Species.Psyduck)
                return catch_rate == (pk1.Japanese ? 167 : 168);
            return catch_rate is 167 or 168;
        }

        // Encounters can have different Catch Rates (RBG vs Y)
        return GBRestrictions.RateMatchesEncounter(Species, Version, catch_rate);
    }
}

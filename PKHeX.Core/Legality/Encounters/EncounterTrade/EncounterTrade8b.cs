namespace PKHeX.Core;

/// <summary>
/// Generation 8 BD/SP Trade Encounter
/// </summary>
/// <inheritdoc cref="EncounterTrade"/>
public sealed record EncounterTrade8b : EncounterTrade, IContestStatsReadOnly, IScaledSizeReadOnly, IFixedOTFriendship
{
    public override int Generation => 8;
    public override EntityContext Context => EntityContext.Gen8b;
    public override int Location => Locations.LinkTrade6NPC;

    public EncounterTrade8b(GameVersion game) : base(game) => EggLocation = Locations.Default8bNone;
    public byte CNT_Cool => BaseContest;
    public byte CNT_Beauty => BaseContest;
    public byte CNT_Cute => BaseContest;
    public byte CNT_Smart => BaseContest;
    public byte CNT_Tough => BaseContest;
    public byte CNT_Sheen => 0;
    public byte HeightScalar { get; init; }
    public byte WeightScalar { get; init; }
    public byte OT_Friendship => Species == (int)Core.Species.Chatot ? (byte)35 : (byte)50;
    private byte BaseContest => Species == (int)Core.Species.Chatot ? (byte)20 : (byte)0;
    public uint PID { get; init; }
    public uint EncryptionConstant { get; init; }

    public override bool IsMatchExact(PKM pk, EvoCriteria evo)
    {
        if (pk.EncryptionConstant != EncryptionConstant)
            return false;
        if (pk.PID != PID)
            return false;
        if (pk is IContestStatsReadOnly s && s.IsContestBelow(this))
            return false;
        if (pk is IScaledSize h && h.HeightScalar != HeightScalar)
            return false;
        if (pk is IScaledSize w && w.WeightScalar != WeightScalar)
            return false;
        if (!IsMatchLocation(pk))
            return false;
        return base.IsMatchExact(pk, evo);
    }

    private bool IsMatchLocation(PKM pk)
    {
        var metState = LocationsHOME.GetRemapState(Context, pk.Context);
        if (metState == LocationRemapState.Original)
            return IsMatchLocationExact(pk);
        if (metState == LocationRemapState.Remapped)
            return IsMatchLocationRemapped(pk);
        return IsMatchLocationExact(pk) || IsMatchLocationRemapped(pk);
    }

    private bool IsMatchLocationExact(PKM pk) => pk.Met_Location == Location;

    private bool IsMatchLocationRemapped(PKM pk)
    {
        var met = (ushort)pk.Met_Location;
        var version = pk.Version;
        if (pk.Context == EntityContext.Gen8)
            return LocationsHOME.IsValidMetBDSP(met, version);
        return LocationsHOME.GetMetSWSH((ushort)Location, version) == met;
    }

    protected override bool IsMatchEggLocation(PKM pk)
    {
        var metState = LocationsHOME.GetRemapState(Context, pk.Context);
        if (metState == LocationRemapState.Original)
            return IsMatchEggLocationExact(pk);
        if (metState == LocationRemapState.Remapped)
            return IsMatchEggLocationRemapped(pk);
        // Either
        return IsMatchEggLocationExact(pk) || IsMatchEggLocationRemapped(pk);
    }

    private static bool IsMatchEggLocationRemapped(PKM pk) => pk.Egg_Location == 0;
    private bool IsMatchEggLocationExact(PKM pk) => pk.Egg_Location == EggLocation;

    protected override void ApplyDetails(ITrainerInfo sav, EncounterCriteria criteria, PKM pk)
    {
        base.ApplyDetails(sav, criteria, pk);
        var pb8 = (PB8)pk;
        pb8.EncryptionConstant = EncryptionConstant;
        pb8.PID = PID;

        // Has German Language ID for all except German origin, which is Japanese
        if (Species == (int)Core.Species.Magikarp)
            pb8.Language = (int)(pb8.Language == (int)LanguageID.German ? LanguageID.Japanese : LanguageID.German);

        this.CopyContestStatsTo(pb8);
        pb8.HT_Language = (byte)sav.Language;
        pb8.HeightScalar = HeightScalar;
        pb8.WeightScalar = WeightScalar;
    }
}

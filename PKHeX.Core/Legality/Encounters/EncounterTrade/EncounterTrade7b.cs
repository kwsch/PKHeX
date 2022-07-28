namespace PKHeX.Core;

/// <summary>
/// Generation 7 LGP/E Trade Encounter
/// </summary>
/// <inheritdoc cref="EncounterTrade"/>
public sealed record EncounterTrade7b : EncounterTrade
{
    public override int Generation => 7;
    public override EntityContext Context => EntityContext.Gen7b;
    public override int Location => Locations.LinkTrade6NPC;
    public override Shiny Shiny => Shiny.Random;

    public EncounterTrade7b(GameVersion game) : base(game) => IsNicknamed = false;

    protected override void ApplyDetails(ITrainerInfo sav, EncounterCriteria criteria, PKM pk)
    {
        base.ApplyDetails(sav, criteria, pk);
        pk.SetRandomEC();
        var pb = (PB7)pk;
        pb.ResetHeight();
        pb.ResetWeight();
        pb.ResetCP();
    }
}

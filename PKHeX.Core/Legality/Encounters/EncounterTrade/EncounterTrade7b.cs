namespace PKHeX.Core
{
    /// <summary>
    /// Generation 7 LGP/E Trade Encounter
    /// </summary>
    /// <inheritdoc cref="EncounterTrade"/>
    public sealed record EncounterTrade7b : EncounterTrade
    {
        public override int Generation => 7;
        public override int Location => Locations.LinkTrade6NPC;

        public EncounterTrade7b(GameVersion game) : base(game)
        {
            Shiny = Shiny.Random;
            IsNicknamed = false;
        }
    }
}

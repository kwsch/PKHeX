namespace PKHeX.Core
{
    /// <summary>
    /// Generation 7 Static Encounter (<see cref="GameVersion.GG"/>
    /// </summary>
    /// <inheritdoc cref="EncounterStatic"/>
    public sealed record EncounterStatic7b : EncounterStatic
    {
        public override int Generation => 7;

        public EncounterStatic7b(GameVersion game) : base(game) { }
    }
}

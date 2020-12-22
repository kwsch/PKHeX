namespace PKHeX.Core
{
    /// <summary>
    /// Generation 4 Pokéwalker  Encounter
    /// </summary>
    /// <inheritdoc cref="EncounterStatic"/>
    public sealed class EncounterStatic4Pokewalker : EncounterStatic4
    {
        public EncounterStatic4Pokewalker(int species, int gender, int level)
        {
            Gift = true;
            Location = 233;
            Version = GameVersion.HGSS;
        }
    }
}

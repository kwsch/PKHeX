namespace PKHeX.Core
{
    /// <summary>
    /// Generation 7 Static Encounter (<see cref="GameVersion.GG"/>
    /// </summary>
    /// <inheritdoc cref="EncounterStatic"/>
    public sealed record EncounterStatic7b(GameVersion Version) : EncounterStatic(Version)
    {
        public override int Generation => 7;

        protected override void ApplyDetails(ITrainerInfo sav, EncounterCriteria criteria, PKM pk)
        {
            base.ApplyDetails(sav, criteria, pk);
            if (pk is IScaledSizeValue v)
            {
                v.ResetHeight();
                v.ResetWeight();
            }
            pk.SetRandomEC();
        }
    }
}

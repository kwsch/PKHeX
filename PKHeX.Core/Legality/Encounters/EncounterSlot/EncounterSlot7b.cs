namespace PKHeX.Core
{
    /// <summary>
    /// Encounter Slot found in <see cref="GameVersion.GG"/>.
    /// </summary>
    /// <inheritdoc cref="EncounterSlot"/>
    public sealed record EncounterSlot7b : EncounterSlot
    {
        public override int Generation => 7;

        public EncounterSlot7b(EncounterArea7b area, int species, int min, int max) : base(area, species, 0, min, max)
        {
        }

        protected override void SetPINGA(PKM pk, EncounterCriteria criteria)
        {
            base.SetPINGA(pk, criteria);
            pk.SetRandomEC();
        }

        protected override void ApplyDetails(ITrainerInfo sav, EncounterCriteria criteria, PKM pk)
        {
            base.ApplyDetails(sav, criteria, pk);
            if (pk is IScaledSizeValue v)
            {
                v.ResetHeight();
                v.ResetWeight();
            }
        }
    }
}

using static PKHeX.Core.LegalityCheckStrings;

namespace PKHeX.Core
{
    /// <summary>
    /// Verifies the <see cref="PK4.EncounterType"/>.
    /// </summary>
    public sealed class EncounterTypeVerifier : Verifier
    {
        protected override CheckIdentifier Identifier => CheckIdentifier.Encounter;

        public override void Verify(LegalityAnalysis data)
        {
            var type = data.EncounterMatch is IEncounterTypeTile t ? t.TypeEncounter : EncounterType.None;
            var result = !type.Contains(data.pkm.EncounterType) ? GetInvalid(LEncTypeMismatch) : GetValid(LEncTypeMatch);
            data.AddLine(result);
        }
    }
}

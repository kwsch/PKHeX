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
            var type = GetEncounterTypeValue(data.pkm, data.EncounterMatch);
            var result = !type.Contains(data.pkm.EncounterType) ? GetInvalid(LEncTypeMismatch) : GetValid(LEncTypeMatch);
            data.AddLine(result);
        }

        private static EncounterType GetEncounterTypeValue(PKM pkm, IEncounterable enc)
        {
            // Encounter type data is only stored for gen 4 encounters
            // All eggs have encounter type none, even if they are from static encounters
            if (!pkm.Gen4 || pkm.Egg_Location != 0)
                return EncounterType.None;

            if (enc is EncounterSlot w)
                return w.TypeEncounter;
            if (enc is EncounterStaticTyped s)
                return s.TypeEncounter;

            return EncounterType.None;
        }
    }
}

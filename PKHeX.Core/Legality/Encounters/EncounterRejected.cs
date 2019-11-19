using System;

namespace PKHeX.Core
{
    /// <summary>
    /// Rejected Encounter Data containing a reason why the encounter was rejected (not compatible).
    /// </summary>
    public sealed class EncounterRejected : IEncounterable
    {
        public readonly IEncounterable Encounter;
        public readonly CheckResult Check;
        public string Reason => Check.Comment;

        public int Species => Encounter.Species;
        public int Form => Encounter.Form;
        public string Name => Encounter.Name;
        public string LongName => Encounter.LongName;
        public bool EggEncounter => Encounter.EggEncounter;
        public int LevelMin => Encounter.LevelMin;
        public int LevelMax => Encounter.LevelMax;

        public EncounterRejected(IEncounterable encounter, CheckResult check)
        {
            Encounter = encounter;
            Check = check;
        }

        public PKM ConvertToPKM(ITrainerInfo SAV) => ConvertToPKM(SAV, EncounterCriteria.Unrestricted);
        public PKM ConvertToPKM(ITrainerInfo SAV, EncounterCriteria criteria) => throw new ArgumentException($"Cannot convert an {nameof(EncounterRejected)} to PKM.");
    }
}
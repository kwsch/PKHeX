namespace PKHeX.Core
{
    public sealed class EncounterStatic2Odd : EncounterStatic
    {
        private const int Dizzy = 146;
        private static readonly int[] _dizzy = { Dizzy };

        public EncounterStatic2Odd(int species)
        {
            Species = species;
            Level = 5;
            Version = GameVersion.C;
            Moves = _dizzy;
            EggLocation = 256;
            EggCycles = 20;
        }

        public override bool IsMatch(PKM pkm, int lvl)
        {
            // Let it get picked up as regular EncounterEgg under other conditions.
            if (pkm.Format > 2)
                return false;
            if (pkm.Move1 != Dizzy && pkm.Move2 != Dizzy && pkm.Move3 != Dizzy && pkm.Move4 != Dizzy)
                return false;
            if (pkm.IsEgg && pkm.EXP != 125)
                return false;
            return base.IsMatch(pkm, lvl);
        }
    }
}
namespace PKHeX.Core
{
    /// <summary>
    /// Generation 8 Nest Encounter (Distributed Data)
    /// </summary>
    public sealed class EncounterStatic8ND : EncounterStatic, IGigantamax, IDynamaxLevel
    {
        public bool CanGigantamax { get; set; }
        public byte DynamaxLevel { get; set; }

        public override bool IsMatch(PKM pkm, int lvl)
        {
            if (Ability != -1 && pkm.AbilityNumber != 4)
                return false;
            if (pkm is IDynamaxLevel d && d.DynamaxLevel < DynamaxLevel)
                return false;

            return base.IsMatch(pkm, lvl);
        }
    }
}
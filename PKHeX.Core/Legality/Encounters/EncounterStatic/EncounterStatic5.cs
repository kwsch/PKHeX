namespace PKHeX.Core
{
    public class EncounterStatic5 : EncounterStatic
    {
        public override int Generation => 5;
        public bool Roaming { get; set; }

        public sealed override bool IsMatchDeferred(PKM pkm)
        {
            if (pkm.FatefulEncounter != Fateful)
                return true;
            if (Ability == 4 && pkm.AbilityNumber != 4) // BW/2 Jellicent collision with wild surf slot, resolved by duplicating the encounter with any abil
                return true;
            return false;
        }
    }
}

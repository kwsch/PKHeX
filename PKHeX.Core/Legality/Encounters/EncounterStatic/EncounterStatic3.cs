namespace PKHeX.Core
{
    public class EncounterStatic3 : EncounterStatic
    {
        protected override bool IsMatchEggLocation(PKM pkm)
        {
            if (pkm.Format == 3)
                return !pkm.IsEgg || EggLocation == 0 || EggLocation == pkm.Met_Location;
            return pkm.Egg_Location == 0;
        }

        protected override bool IsMatchLevel(PKM pkm, DexLevel evo)
        {
            if (pkm.Format != 3) // Met Level lost on PK3=>PK4
                return Level <= evo.Level;

            if (EggEncounter)
                return pkm.Met_Level == 0 && pkm.CurrentLevel >= 5; // met level 0, origin level 5

            return pkm.Met_Level == Level;
        }

        protected override bool IsMatchLocation(PKM pkm)
        {
            if (EggEncounter)
                return true;
            if (Location == 0)
                return true;
            if (pkm.Format == 3)
                return Location == pkm.Met_Location;
            return true; // transfer location verified later
        }
    }
}

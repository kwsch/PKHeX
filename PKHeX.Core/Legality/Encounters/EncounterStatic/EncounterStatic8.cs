namespace PKHeX.Core
{
    public class EncounterStatic8 : EncounterStatic
    {
        protected override bool IsMatchLevel(PKM pkm, DexLevel evo)
        {
            if (evo.Level == Level)
                return true;
            if (EncounterArea8.IsWildArea8(Location) || EncounterArea8.IsWildArea8Armor(Location))
                return evo.Level == 60;
            return false;
        }
    }
}
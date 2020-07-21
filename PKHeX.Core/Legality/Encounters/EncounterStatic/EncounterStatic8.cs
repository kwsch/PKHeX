namespace PKHeX.Core
{
    public class EncounterStatic8 : EncounterStatic
    {
        protected override bool IsMatchLevel(PKM pkm, int lvl)
        {
            if (lvl == Level)
                return true;
            if (EncounterArea8.IsWildArea8(Location) || EncounterArea8.IsWildArea8Armor(Location))
                return lvl == 60;
            return false;
        }
    }
}
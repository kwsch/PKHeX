namespace PKHeX.Core
{
    public class EncounterStatic4 : EncounterStatic
    {
        protected sealed override bool IsMatchEggLocation(PKM pkm)
        {
            if (pkm.Egg_Location == EggLocation)
            {
                if (EggLocation == 0)
                    return true;

                // Check the inverse scenario for 4->5 eggs
                if (!Locations.IsPtHGSSLocationEgg(EggLocation))
                    return true;
                return pkm.Format == 4;
            }

            if (pkm.IsEgg) // unhatched
            {
                if (EggLocation != pkm.Met_Location)
                    return false;
                return pkm.Egg_Location == 0;
            }

            // Only way to mismatch is to be a Link Traded egg, or traded to Pt/HG/SS and hatched there.
            if (pkm.Egg_Location == Locations.LinkTrade4)
                return true;

            // check Pt/HGSS data
            if (pkm.Format == 4)
                return false;

            if (!Locations.IsPtHGSSLocationEgg(EggLocation)) // non-Pt/HG/SS egg gift
                return false;

            // transferring 4->5 clears Pt/HG/SS location value and keeps Faraway Place
            return pkm.Egg_Location == Locations.Faraway4;
        }

        protected sealed override bool IsMatchLevel(PKM pkm, DexLevel evo)
        {
            if (pkm.Format != 4) // Met Level lost on PK3=>PK4
                return Level <= evo.Level;

            return pkm.Met_Level == (EggEncounter ? 0 : Level);
        }

        protected override bool IsMatchLocation(PKM pkm)
        {
            if (EggEncounter)
                return true;
            if (Location == 0)
                return true;
            if (pkm.Format == 4)
                return Location == pkm.Met_Location;
            return true; // transfer location verified later
        }
    }
}

namespace PKHeX.Core
{
    public class DexLevel
    {
        public int Species;
        public int Level;
        public int MinLevel;
        public bool RequiresLvlUp;
        public int Form = -1;
        public int Flag = -1;

        public DexLevel Copy(int lvl)
        {
            return new DexLevel {Species = Species, Level = lvl, MinLevel = MinLevel, RequiresLvlUp = RequiresLvlUp, Form = Form, Flag = -1};
        }
        public bool Matches(int species, int form)
        {
            if (species != Species)
                return false;
            if (Form > -1)
                return form == Form;
            return true;
        }
    }
}

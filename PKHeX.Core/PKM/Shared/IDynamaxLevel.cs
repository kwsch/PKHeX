using static PKHeX.Core.Species;

namespace PKHeX.Core
{
    public interface IDynamaxLevel
    {
        byte DynamaxLevel { get; set; }
    }

    public static class DynamaxLevelExtensions
    {
        public static bool CanHaveDynamaxLevel(this IDynamaxLevel _, PKM pkm)
        {
            if (pkm.IsEgg)
                return false;
            return CanHaveDynamaxLevel(pkm.Species);
        }

        private static bool CanHaveDynamaxLevel(int species)
        {
            if (species == (int)Zacian)
                return false;
            if (species == (int)Zamazenta)
                return false;
            if (species == (int)Eternatus)
                return false;
            return true;
        }
    }
}

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
            return species is not ((int)Zacian or (int)Zamazenta or (int)Eternatus);
        }
    }
}

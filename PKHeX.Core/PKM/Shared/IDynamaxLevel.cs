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
            var species = pkm.Species;
            if (species == (int)Zacian || species == (int)Zamazenta || species == (int)Eternatus)
                return false;
            return true;
        }
    }
}

namespace PKHeX.Core
{
    public interface ISpriteBuilder<T>
    {
        T GetSprite(int species, int form, int gender, uint formarg, int heldItem, bool isEgg, bool isShiny,
            int generation = -1,
            bool isBoxBGRed = false,
            bool isAltShiny = false);

        T GetSprite(T baseSprite, int species, int heldItem, bool isEgg, bool isShiny,
            int generation = -1,
            bool isBoxBGRed = false,
            bool isAltShiny = false);

        void Initialize(SaveFile sav);
    }
}

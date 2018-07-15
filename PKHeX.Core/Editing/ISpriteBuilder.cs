namespace PKHeX.Core
{
    public interface ISpriteBuilder<out T>
    {
        T GetSprite(int species, int form, int gender, int heldItem, bool isEgg, bool isShiny,
            int generation = -1,
            bool isBoxBGRed = false);

        void Initialize(SaveFile sav);
    }
}

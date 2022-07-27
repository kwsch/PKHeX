namespace PKHeX.Core;

public interface ISpriteBuilder<T>
{
    T GetSprite(int species, int form, int gender, uint formarg, int heldItem, bool isEgg, Shiny shiny,
        int generation = -1,
        SpriteBuilderTweak tweak = SpriteBuilderTweak.None);

    T GetSprite(T baseSprite, int species, int heldItem, bool isEgg, Shiny shiny,
        int generation = -1,
        SpriteBuilderTweak tweak = SpriteBuilderTweak.None);

    void Initialize(SaveFile sav);
}

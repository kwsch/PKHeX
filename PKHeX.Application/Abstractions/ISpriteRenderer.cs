using PKHeX.Core;

namespace PKHeX.Application.Abstractions;

/// <summary>
/// Renders PKM sprites to framework-free PNG bytes. The View layer materializes an Avalonia
/// <c>Bitmap</c> from these bytes via a value converter, keeping the UI framework out of the
/// Application/Presentation layers.
/// </summary>
public interface ISpriteRenderer
{
    byte[]? GetSprite(PKM pk, bool isEgg = false);
    byte[]? GetSprite(ushort species, byte form, byte gender, uint formarg, bool shiny, EntityContext context);
    byte[]? GetItemSprite(int itemId);
    byte[]? GetEmptySlot();
    void Initialize(SaveFile sav);
}

namespace PKHeX.Core;

/// <summary>
/// Provides interactions for Mystery Gift reading and writing.
/// </summary>
public interface IMysteryGiftStorage
{
    int GiftCountMax { get; }
    DataMysteryGift GetMysteryGift(int index);
    void SetMysteryGift(int index, DataMysteryGift gift);
}

namespace PKHeX.Core;

/// <summary>
/// Structure containing the Mystery Gift Data
/// </summary>
public class MysteryGiftAlbum(DataMysteryGift[] Gifts, bool[] Flags)
{
    /// <summary>
    /// Mystery Gift data received
    /// </summary>
    public readonly DataMysteryGift[] Gifts = Gifts;

    /// <summary>
    /// Received Flag list
    /// </summary>
    /// <remarks>
    /// this[index] == true iff index=<see cref="MysteryGift.CardID"/> has been received already.
    /// </remarks>
    public readonly bool[] Flags = Flags;
}

public sealed class EncryptedMysteryGiftAlbum(DataMysteryGift[] Gifts, bool[] Flags, uint Seed)
    : MysteryGiftAlbum(Gifts, Flags)
{
    /// <summary>
    /// Encryption Seed (only used in Generation 5 to encrypt the stored data)
    /// </summary>
    public readonly uint Seed = Seed;
}

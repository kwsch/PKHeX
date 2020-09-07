namespace PKHeX.Core
{
    /// <summary>
    /// Structure containing the Mystery Gift Data
    /// </summary>
    public class MysteryGiftAlbum
    {
        /// <summary>
        /// Mystery Gift data received
        /// </summary>
        public readonly DataMysteryGift[] Gifts;

        /// <summary>
        /// Received Flag list
        /// </summary>
        /// <remarks>
        /// this[index] == true iff index=<see cref="MysteryGift.CardID"/> has been received already.
        /// </remarks>
        public readonly bool[] Flags;

        public MysteryGiftAlbum(DataMysteryGift[] gifts, bool[] flags)
        {
            Flags = flags;
            Gifts = gifts;
        }
    }

    public sealed class EncryptedMysteryGiftAlbum : MysteryGiftAlbum
    {
        /// <summary>
        /// Encryption Seed (only used in Generation 5 to encrypt the stored data)
        /// </summary>
        public readonly uint Seed;

        public EncryptedMysteryGiftAlbum(DataMysteryGift[] gifts, bool[] flags, uint seed) : base(gifts, flags) => Seed = seed;
    }
}

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
        public MysteryGift[] Gifts;

        /// <summary>
        /// Received Flag list
        /// </summary>
        /// <remarks>
        /// this[index] == true iff index=<see cref="MysteryGift.CardID"/> has been received already.
        /// </remarks>
        public bool[] Flags;

        /// <summary>
        /// Encryption Seed (only used in Generation 4 to encrypt the stored data)
        /// </summary>
        public uint Seed;
    }
}

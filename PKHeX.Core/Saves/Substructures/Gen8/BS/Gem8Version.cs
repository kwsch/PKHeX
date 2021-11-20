namespace PKHeX.Core
{
    public enum Gem8Version
    {
        /// <summary>
        /// Initial cartridge version shipped.
        /// </summary>
        /// <remarks><see cref="SaveUtil.SIZE_G8BDSP"/></remarks>
        V1_0 = 0x25,

        /// <summary>
        /// Pre-release patch.
        /// </summary>
        /// <remarks><see cref="SaveUtil.SIZE_G8BDSP_1"/></remarks>
        V1_1 = 0x2C,
    }
}

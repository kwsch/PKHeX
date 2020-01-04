namespace PKHeX.Core
{
    /// <summary>
    /// Storage for the Fused species, <see cref="Species.Kyurem"/>, <see cref="Species.Solgaleo"/>, <see cref="Species.Lunala"/>
    /// </summary>
    public sealed class Fused8 : SaveBlock
    {
        public Fused8(SaveFile sav, SCBlock block) : base(sav, block.Data) { }

        public static int GetFusedSlotOffset(int slot)
        {
            if ((uint)slot >= 3)
                return -1;
            return PokeCrypto.SIZE_8PARTY * slot;
        }
    }
}
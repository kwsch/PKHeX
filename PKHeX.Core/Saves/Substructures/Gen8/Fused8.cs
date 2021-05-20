namespace PKHeX.Core
{
    /// <summary>
    /// Storage for the species that was fused into <see cref="Species.Kyurem"/> and <see cref="Species.Necrozma"/>.
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

        public PK8 Kyurem
        {
            get => (PK8) SAV.GetStoredSlot(Data, GetFusedSlotOffset(0));
            set => value.EncryptedBoxData.CopyTo(Data, GetFusedSlotOffset(0));
        }

        public PK8 NecrozmaSolgaleo
        {
            get => (PK8)SAV.GetStoredSlot(Data, GetFusedSlotOffset(1));
            set => value.EncryptedBoxData.CopyTo(Data, GetFusedSlotOffset(1));
        }

        public PK8 NecrozmaLunala
        {
            get => (PK8)SAV.GetStoredSlot(Data, GetFusedSlotOffset(2));
            set => value.EncryptedBoxData.CopyTo(Data, GetFusedSlotOffset(2));
        }
    }
}

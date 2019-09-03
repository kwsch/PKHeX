namespace PKHeX.Core
{
    /// <summary>
    /// <see cref="SaveFile"/> format which stores Box Data in a separate buffer from the game data.
    /// </summary>
    public abstract class DualBufferSAV : SaveFile
    {
        protected DualBufferSAV(byte[] data) : base(data) { }
        protected DualBufferSAV() { }

        // SaveData is chunked into two pieces.
        public byte[] Storage { get; protected set; }
        public byte[] General { get; protected set; }

        protected abstract int StorageSize { get; }
        protected abstract int GeneralSize { get; }
        protected abstract int StorageStart { get; }

        /// <inheritdoc />
        public override bool GetFlag(int offset, int bitIndex) => FlagUtil.GetFlag(General, offset, bitIndex);

        /// <inheritdoc />
        public override void SetFlag(int offset, int bitIndex, bool value) => FlagUtil.SetFlag(General, offset, bitIndex, value);

        public override PKM GetPartySlot(int offset) => GetDecryptedPKM(General.Slice(offset, SIZE_PARTY));

        protected override void WritePartySlot(PKM pkm, int offset) => SetData(General, pkm.EncryptedPartyData, offset);
        protected override void WriteStoredSlot(PKM pkm, int offset) => SetData(General, pkm.EncryptedBoxData, offset);
        protected override void WriteBoxSlot(PKM pkm, int offset) => SetData(Storage, pkm.EncryptedBoxData, offset);
    }
}
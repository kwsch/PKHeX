namespace PKHeX.Core
{
    /// <summary>
    /// Party data storage and metadata
    /// </summary>
    public sealed class Party8b : SaveBlock<SAV8BS>
    {
        public Party8b(SAV8BS sav, int offset) : base(sav) => Offset = offset;

        public int PartyCount
        {
            get => Data[Offset + (6 * PokeCrypto.SIZE_8PARTY)];
            set => Data[Offset + (6 * PokeCrypto.SIZE_8PARTY)] = (byte)value;
        }

        public int MarkingIndex
        {
            get => Data[Offset + (6 * PokeCrypto.SIZE_8PARTY) + 1];
            set => Data[Offset + (6 * PokeCrypto.SIZE_8PARTY) + 1] = (byte)value;
        }
    }
}

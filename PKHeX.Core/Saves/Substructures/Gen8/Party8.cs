namespace PKHeX.Core
{
    public sealed class Party8 : SaveBlock
    {
        public Party8(SaveFile sav, SCBlock block) : base(sav, block.Data) { }

        public int PartyCount
        {
            get => Data[6 * PokeCrypto.SIZE_8PARTY];
            set => Data[6 * PokeCrypto.SIZE_8PARTY] = (byte)value;
        }
    }
}
namespace PKHeX.Core
{
    public sealed class Party8 : SaveBlock<SAV8SWSH>
    {
        public Party8(SAV8SWSH sav, SCBlock block) : base(sav, block.Data) { }

        public int PartyCount
        {
            get => Data[6 * PokeCrypto.SIZE_8PARTY];
            set => Data[6 * PokeCrypto.SIZE_8PARTY] = (byte)value;
        }
    }
}

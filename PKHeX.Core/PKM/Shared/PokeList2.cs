namespace PKHeX.Core
{
    public sealed class PokeList2 : PokeListGB<PK2>
    {
        protected override byte GetSpeciesBoxIdentifier(PK2 pk) => pk.IsEgg ? (byte)0xFD : (byte)pk.Species;
        protected override PK2 GetEntry(byte[] dat, byte[] otname, byte[] nick, bool egg) => new PK2(dat) { otname = otname, nick = nick, IsEgg = egg };
        protected override int GetEntrySize() => GetEntrySize(IsFormatParty);

        public PokeList2(byte[] d, PokeListType c = PokeListType.Single, bool jp = false) : base(d, c, jp) { }
        public PokeList2(PokeListType c = PokeListType.Single, bool jp = false) : base(c, jp) { }
        public PokeList2(byte[] d) : base(d, PokeListType.Single, d.Length == PokeCrypto.SIZE_2JLIST) { }
        public PokeList2(PK2 pk) : base(pk) { }

        private static int GetEntrySize(bool party) => party ? PokeCrypto.SIZE_2PARTY : PokeCrypto.SIZE_2STORED;
        public static int GetDataLength(PokeListType c, bool jp) => GetDataSize(c, jp, GetEntrySize(IsCapacityPartyFormat(c)));
    }
}
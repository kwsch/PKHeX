using System;

namespace PKHeX.Core
{
    /// <summary>
    /// List of <see cref="PK1"/> prefixed by a count.
    /// </summary>
    public sealed class PokeList2 : PokeListGB<PK2>
    {
        protected override byte GetSpeciesBoxIdentifier(PK2 pk) => pk.IsEgg ? PK2.EggSpeciesValue : (byte)pk.Species;
        protected override PK2 GetEntry(byte[] dat, ReadOnlySpan<byte> otname, ReadOnlySpan<byte> nick, bool egg)
        {
            var result = new PK2(dat, Japanese) { IsEgg = egg };
            otname.CopyTo(result.OT_Trash);
            nick.CopyTo(result.Nickname_Trash);
            return result;
        }

        protected override int GetEntrySize() => GetEntrySize(IsFormatParty);

        public PokeList2(byte[] d, PokeListType c = PokeListType.Single, bool jp = false) : base(d, c, jp) { }
        public PokeList2(PokeListType c = PokeListType.Single, bool jp = false) : base(c, jp) { }
        public PokeList2(byte[] d) : base(d, PokeListType.Single, d.Length == PokeCrypto.SIZE_2JLIST) { }
        public PokeList2(PK2 pk) : base(pk) { }

        private static int GetEntrySize(bool party) => party ? PokeCrypto.SIZE_2PARTY : PokeCrypto.SIZE_2STORED;
        public static int GetDataLength(PokeListType c, bool jp) => GetDataSize(c, jp, GetEntrySize(IsCapacityPartyFormat(c)));
    }
}

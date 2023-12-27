using System;

namespace PKHeX.Core;

/// <summary>
/// List of <see cref="PK1"/> prefixed by a count.
/// </summary>
public sealed class PokeList1 : PokeListGB<PK1>
{
    protected override byte GetSpeciesBoxIdentifier(PK1 pk) => SpeciesConverter.GetInternal1(pk.Species);
    protected override PK1 GetEntry(byte[] dat, ReadOnlySpan<byte> otname, ReadOnlySpan<byte> nick, bool egg)
    {
        var result = new PK1(dat, Japanese); // no eggs
        otname.CopyTo(result.OT_Trash);
        nick.CopyTo(result.Nickname_Trash);
        return result;
    }

    protected override int GetEntrySize() => GetEntrySize(IsFormatParty);

    public PokeList1(byte[] d, PokeListType c = PokeListType.Single, bool jp = false) : base(d, c, jp) { }
    public PokeList1(PokeListType c = PokeListType.Single, bool jp = false) : base(c, jp) { }
    public PokeList1(byte[] d) : base(d, PokeListType.Single, d.Length == PokeCrypto.SIZE_1JLIST) { }
    public PokeList1(PK1 pk) : base(pk) { }

    private static int GetEntrySize(bool party) => party ? PokeCrypto.SIZE_1PARTY : PokeCrypto.SIZE_1STORED;
    public static int GetDataLength(PokeListType c, bool jp) => GetDataSize(c, jp, GetEntrySize(IsCapacityPartyFormat(c)));
}

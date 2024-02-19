namespace PKHeX.Core;

public sealed class BattleBox6 : SaveBlock<SAV6>
{
    public BattleBox6(SAV6XY SAV, int offset) : base(SAV, offset) { }
    public BattleBox6(SAV6AO SAV, int offset) : base(SAV, offset) { }

    private int LockedFlagOffset => Offset + (6 * PokeCrypto.SIZE_6STORED);

    public bool Locked
    {
        get => LockedWiFiTournament || LockedLiveTournament;
        set => LockedWiFiTournament = LockedLiveTournament = value;
    }

    public bool LockedWiFiTournament
    {
        get => (Data[LockedFlagOffset] & 1) != 0;
        set => Data[LockedFlagOffset] = (byte)((Data[Offset + LockedFlagOffset] & ~1) | (value ? 1 : 0));
    }

    public bool LockedLiveTournament
    {
        get => (Data[LockedFlagOffset] & 2) != 0;
        set => Data[LockedFlagOffset] = (byte)((Data[Offset + LockedFlagOffset] & ~2) | (value ? 2 : 0));
    }
}

using System;

namespace PKHeX.Core;

public sealed class BattleBox6 : SaveBlock<SAV6>
{
    public BattleBox6(SAV6XY SAV, Memory<byte> raw) : base(SAV, raw) { }
    public BattleBox6(SAV6AO SAV, Memory<byte> raw) : base(SAV, raw) { }

    public const int Count = 6;
    private const int SizeStored = PokeCrypto.SIZE_6STORED;
    private const int LockedFlagOffset = (Count * SizeStored);

    public Memory<byte> GetSlot(int index)
    {
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual<uint>((uint)index, Count);
        return Raw.Slice(index * SizeStored, SizeStored);
    }

    public Memory<byte> this[int index] => GetSlot(index);

    public bool Locked
    {
        get => LockedWiFiTournament || LockedLiveTournament;
        set => LockedWiFiTournament = LockedLiveTournament = value;
    }

    public bool LockedWiFiTournament
    {
        get => (Data[LockedFlagOffset] & 1) != 0;
        set => Data[LockedFlagOffset] = (byte)((Data[LockedFlagOffset] & ~1) | (value ? 1 : 0));
    }

    public bool LockedLiveTournament
    {
        get => (Data[LockedFlagOffset] & 2) != 0;
        set => Data[LockedFlagOffset] = (byte)((Data[LockedFlagOffset] & ~2) | (value ? 2 : 0));
    }
}

using System;

namespace PKHeX.Core;

public sealed class BattleBox5(SAV5 sav, Memory<byte> raw) : SaveBlock<SAV5>(sav, raw)
{
    private const int SizeStored = PokeCrypto.SIZE_5STORED;
    public const int Count = 6;

    public Memory<byte> GetSlot(int index)
    {
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual<uint>((uint)index, Count);
        return Raw.Slice(index * SizeStored, SizeStored);
    }

    public Memory<byte> this[int index] => GetSlot(index);

    public Span<byte> TeamTrash => Data.Slice(0x330, 0x28);

    public string Name // BOX 1
    {
        get => StringConverter5.GetString(TeamTrash);
        set => StringConverter5.SetString(TeamTrash, value, 20, 0);
    }

    public bool BattleBoxLockedWiFiTournament
    {
        get => (Data[0x358] & 1u) == 1;
        set => Data[0x358] = value ? (byte)(Data[0x358] | 1u) : (byte)(Data[0x358] & ~1u);
    }

    public bool BattleBoxLockedLiveTournament // For VGC IRL tournaments.
    {
        get => (Data[0x358] & 2u) == 2;
        set => Data[0x358] = value ? (byte)(Data[0x358] | 2u) : (byte)(Data[0x358] & ~2u);
    }
}

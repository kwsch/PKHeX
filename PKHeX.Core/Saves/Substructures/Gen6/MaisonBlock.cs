using System;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

public sealed class MaisonBlock : SaveBlock<SAV6>
{
    public MaisonBlock(SAV6XY sav, Memory<byte> raw) : base(sav, raw) { }
    public MaisonBlock(SAV6AO sav, Memory<byte> raw) : base(sav, raw) { }

    // 5 * [u16*4: normal,super,normalStreak,superStreak]
    public const int MaisonStatCount = 20;

    public ushort GetMaisonStat(int index) => ReadUInt16LittleEndian(Data[(0x1C0 + (2 * index))..]);
    public void SetMaisonStat(int index, ushort value) => WriteUInt16LittleEndian(Data[(0x1C0 + (2 * index))..], value);

    private static int GetMaisonStatIndex(BattleStyle6 type, bool streak, bool super) => ((int)type << 2) | (streak ? 2 : 0) | (super ? 1 : 0);
    public ushort GetMaisonStat(BattleStyle6 type, bool streak, bool super) => GetMaisonStat(GetMaisonStatIndex(type, streak, super));
    public void SetMaisonStat(BattleStyle6 type, bool streak, bool super, ushort value) => SetMaisonStat(GetMaisonStatIndex(type, streak, super), value);
}

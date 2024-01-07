using System;

namespace PKHeX.Core;

public sealed class BlueberrySupportBoard9(SAV9SV sav, SCBlock block) : SaveBlock<SAV9SV>(sav, block.Data)
{
    private Span<byte> Span => Data.AsSpan();

    public bool BaseballClub1Done { get => Span[0x04] == 1; set => Span[0x04] = value ? (byte)1 : (byte)0; }
    public bool BaseballClub1Unread { get => Span[0x05] == 1; set => Span[0x05] = value ? (byte)1 : (byte)0; }

    public bool BaseballClub2Done { get => Span[0x06] == 1; set => Span[0x06] = value ? (byte)1 : (byte)0; }
    public bool BaseballClub2Unread { get => Span[0x07] == 1; set => Span[0x07] = value ? (byte)1 : (byte)0; }

    public bool BaseballClub3Done { get => Span[0x08] == 1; set => Span[0x08] = value ? (byte)1 : (byte)0; }
    public bool BaseballClub3Unread { get => Span[0x09] == 1; set => Span[0x09] = value ? (byte)1 : (byte)0; }
}

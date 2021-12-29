using System;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

/// <summary>
/// Tracks <see cref="Species.Spinda"/> data for the game.
/// </summary>
/// <remarks>ZUKAN_PERSONAL_RND_DATA size: 0x64 (100)</remarks>
public sealed class ZukanSpinda8b : SaveBlock
{
    public ZukanSpinda8b(SAV8BS sav, int offset) : base(sav) => Offset = offset;

    public uint GetSeen(int gender, bool shiny)
    {
        var ofs = GetOffset(gender, shiny);
        return ReadUInt32LittleEndian(Data.AsSpan(Offset + ofs));
    }

    public uint GetCaught(int gender, bool shiny)
    {
        var ofs = GetOffset(gender, shiny);
        return ReadUInt32LittleEndian(Data.AsSpan(Offset + 0x10 + ofs));
    }

    public void SetSeen(int gender, bool shiny, uint value)
    {
        var ofs = GetOffset(gender, shiny);
        WriteUInt32LittleEndian(Data.AsSpan(Offset + ofs), value);
    }

    public void SetCaught(int gender, bool shiny, uint value)
    {
        var ofs = GetOffset(gender, shiny);
        WriteUInt32LittleEndian(Data.AsSpan(Offset + 0x10 + ofs), value);
    }

    private static int GetOffset(int gender, bool shiny) => 4 * ((gender & 1) + (shiny ? 2 : 0));

    public void SetDex(ZukanState8b state, uint ec, int gender, bool shiny)
    {
        if (state < ZukanState8b.Seen) // not seen yet
            SetSeen(gender, shiny, ec);
        if (state < ZukanState8b.Caught) // not caught yet
            SetCaught(gender, shiny, ec);
    }
}

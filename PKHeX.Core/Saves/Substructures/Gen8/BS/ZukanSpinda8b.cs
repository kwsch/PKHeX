using System;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

/// <summary>
/// Tracks <see cref="Species.Spinda"/> data for the game.
/// </summary>
/// <remarks>ZUKAN_PERSONAL_RND_DATA size: 0x64 (100)</remarks>
public sealed class ZukanSpinda8b(SAV8BS sav, Memory<byte> raw) : SaveBlock<SAV8BS>(sav, raw)
{
    public uint GetSeen(byte gender, bool shiny)
    {
        var ofs = GetOffset(gender, shiny);
        return ReadUInt32LittleEndian(Data[ofs..]);
    }

    public uint GetCaught(byte gender, bool shiny)
    {
        var ofs = GetOffset(gender, shiny);
        return ReadUInt32LittleEndian(Data[(0x10 + ofs)..]);
    }

    public void SetSeen(byte gender, bool shiny, uint value)
    {
        var ofs = GetOffset(gender, shiny);
        WriteUInt32LittleEndian(Data[ofs..], value);
    }

    public void SetCaught(byte gender, bool shiny, uint value)
    {
        var ofs = GetOffset(gender, shiny);
        WriteUInt32LittleEndian(Data[(0x10 + ofs)..], value);
    }

    private static int GetOffset(byte gender, bool shiny) => 4 * ((gender & 1) + (shiny ? 2 : 0));

    public void SetDex(ZukanState8b state, uint ec, byte gender, bool shiny)
    {
        if (state < ZukanState8b.Seen) // not seen yet
            SetSeen(gender, shiny, ec);
        if (state < ZukanState8b.Caught) // not caught yet
            SetCaught(gender, shiny, ec);
    }
}

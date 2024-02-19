using System.ComponentModel;

namespace PKHeX.Core;

/// <summary>
/// Base class for a savegame data reader.
/// </summary>
public abstract class SaveBlock<T>(T sav, byte[] data, int offset = 0) : IDataIndirect where T : SaveFile
{
    protected readonly T SAV = sav;

    [Browsable(false)] public byte[] Data { get; } = data;
    [Browsable(false)] public int Offset { get; protected init; } = offset;

    protected SaveBlock(T sav) : this(sav, sav.Data) { }

    protected SaveBlock(T sav, int offset) : this(sav, sav.Data, offset) { }
}

public interface IDataIndirect
{
    int Offset { get; }
    byte[] Data { get; }
}

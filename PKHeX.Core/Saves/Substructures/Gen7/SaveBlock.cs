using System.ComponentModel;

namespace PKHeX.Core;

/// <summary>
/// Base class for a savegame data reader.
/// </summary>
public abstract class SaveBlock<T> : IDataIndirect where T : SaveFile
{
    protected readonly T SAV;

    [Browsable(false)] public byte[] Data { get; }
    [Browsable(false)] public int Offset { get; protected init; }

    protected SaveBlock(T sav) : this(sav, sav.Data) { }

    protected SaveBlock(T sav, byte[] data)
    {
        SAV = sav;
        Data = data;
    }
}

public interface IDataIndirect
{
    int Offset { get; }
    byte[] Data { get; }
}

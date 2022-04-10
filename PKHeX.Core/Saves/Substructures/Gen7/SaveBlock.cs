using System.ComponentModel;

namespace PKHeX.Core;

/// <summary>
/// Base class for a savegame data reader.
/// </summary>
public abstract class SaveBlock
{
    [Browsable(false)]
    public int Offset { get; protected init; }

    public readonly byte[] Data;
    protected readonly SaveFile SAV;
    protected SaveBlock(SaveFile sav) => Data = (SAV = sav).Data;

    protected SaveBlock(SaveFile sav, byte[] data)
    {
        SAV = sav;
        Data = data;
    }
}

public abstract class SaveBlock<T> where T : SaveFile
{
    [Browsable(false)]
    public int Offset { get; protected init; }

    public readonly byte[] Data;
    protected readonly T SAV;
    protected SaveBlock(T sav) => Data = (SAV = sav).Data;

    protected SaveBlock(T sav, byte[] data)
    {
        SAV = sav;
        Data = data;
    }
}

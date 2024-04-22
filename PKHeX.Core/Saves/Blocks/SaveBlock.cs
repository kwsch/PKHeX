using System;
using System.ComponentModel;

namespace PKHeX.Core;

/// <summary>
/// Base class for a savegame data reader.
/// </summary>
public abstract class SaveBlock<T>(T sav, Memory<byte> raw) : IDataIndirect where T : SaveFile
{
    protected readonly T SAV = sav;

    [Browsable(false)] protected Memory<byte> Raw => raw;
    [Browsable(false)] public Span<byte> Data => raw.Span;

    public bool Equals(ReadOnlyMemory<byte> other) => other.Equals(raw);
}

public interface IDataIndirect
{
    Span<byte> Data { get; }
    bool Equals(ReadOnlyMemory<byte> other);
}

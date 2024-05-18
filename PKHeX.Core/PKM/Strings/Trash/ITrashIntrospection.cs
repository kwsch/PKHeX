using System;

namespace PKHeX.Core;

public interface ITrashIntrospection
{
    int GetStringTerminatorIndex(ReadOnlySpan<byte> data);
    int GetStringLength(ReadOnlySpan<byte> data);
    int GetBytesPerChar();
}

using System;

namespace PKHeX.Core;

public interface IStringConverter
{
    string GetString(ReadOnlySpan<byte> data);
    int LoadString(ReadOnlySpan<byte> data, Span<char> text);
    int SetString(Span<byte> data, ReadOnlySpan<char> text, int length, StringConverterOption option);
}

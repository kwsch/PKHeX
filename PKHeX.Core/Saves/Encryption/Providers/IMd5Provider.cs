using System;
using System.Security.Cryptography;

namespace PKHeX.Core;

public interface IMd5Provider
{
    void HashData(ReadOnlySpan<byte> source, Span<byte> destination);

    internal static readonly IMd5Provider Default = new DefaultMd5();

    private sealed class DefaultMd5 : IMd5Provider
    {
        public void HashData(ReadOnlySpan<byte> source, Span<byte> destination) => MD5.HashData(source, destination);
    }
}

using System;

namespace PKHeX.Core;

/// <summary>
/// Fowler–Noll–Vo non-cryptographic hash
/// </summary>
/// <remarks>https://en.wikipedia.org/wiki/Fowler%E2%80%93Noll%E2%80%93Vo_hash_function</remarks>
public static class FnvHash
{
    private const ulong kFnvPrime_64 = 0x00000100000001b3;
    private const ulong kOffsetBasis_64 = 0xCBF29CE484222645;

    /// <summary>
    /// Gets the hash code of the input sequence via the alternative Fnv1 method.
    /// </summary>
    /// <param name="input">Input sequence</param>
    /// <param name="hash">Initial hash value</param>
    /// <returns>Computed hash code</returns>
    public static ulong HashFnv1a_64(ReadOnlySpan<char> input, ulong hash = kOffsetBasis_64)
    {
        foreach (var c in input)
        {
            hash ^= c;
            hash *= kFnvPrime_64;
        }
        return hash;
    }

    /// <summary>
    /// Gets the hash code of the input sequence via the alternative Fnv1 method.
    /// </summary>
    /// <param name="input">Input sequence</param>
    /// <param name="hash">Initial hash value</param>
    /// <returns>Computed hash code</returns>
    public static ulong HashFnv1a_64(ReadOnlySpan<byte> input, ulong hash = kOffsetBasis_64)
    {
        foreach (var c in input)
        {
            hash ^= c;
            hash *= kFnvPrime_64;
        }
        return hash;
    }
}

using System.Collections.Generic;

namespace PKHeX.Core;

/// <summary>
/// Fowler–Noll–Vo non-cryptographic hash
/// </summary>
public static class FnvHash
{
    private const ulong kFnvPrime_64 = 0x00000100000001b3;
    private const ulong kOffsetBasis_64 = 0xCBF29CE484222645;

    /// <summary>
    /// Gets the hash code of the input sequence via the default Fnv1 method.
    /// </summary>
    /// <param name="input">Input sequence</param>
    /// <param name="hash">Initial hash value</param>
    /// <returns>Computed hash code</returns>
    public static ulong HashFnv1a_64(IEnumerable<char> input, ulong hash = kOffsetBasis_64)
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
    public static ulong HashFnv1a_64(IEnumerable<byte> input, ulong hash = kOffsetBasis_64)
    {
        foreach (var c in input)
        {
            hash ^= c;
            hash *= kFnvPrime_64;
        }
        return hash;
    }
}

using System;
using System.Collections.Generic;

namespace PKHeX.Core;

/// <summary>
/// <see cref="SwishCrypto"/> block accessor, where blocks are ordered by ascending <see cref="SCBlock.Key"/>.
/// </summary>
public abstract class SCBlockAccessor : ISaveBlockAccessor<SCBlock>
{
    public abstract IReadOnlyList<SCBlock> BlockInfo { get; }

    /// <inheritdoc cref="SCBlock.GetValue"/>
    public object GetBlockValue(uint key) => GetBlock(key).GetValue();

    /// <inheritdoc cref="SCBlock.SetValue"/>
    public void SetBlockValue(uint key, object value) => GetBlock(key).SetValue(value);

    /// <summary> Checks if there is any <see cref="SCBlock"/> with the requested <see cref="key"/>. </summary>
    public bool HasBlock(uint key) => FindIndex(BlockInfo, key) != -1;

    /// <summary> Returns the <see cref="SCBlock"/> reference with the corresponding <see cref="key"/>. </summary>
    public SCBlock GetBlock(uint key) => Find(BlockInfo, key);

    #region Ease of Use Overloads
    /// <inheritdoc cref="GetBlock(string)"/>
    /// <param name="name">Block name (un-hashed)</param>
    public SCBlock GetBlock(string name) => GetBlock(Hash(name.AsSpan()));

    /// <inheritdoc cref="GetBlock(string)"/>
    public SCBlock GetBlock(ReadOnlySpan<char> name) => GetBlock(Hash(name));
    private static uint Hash(ReadOnlySpan<char> name) => (uint)FnvHash.HashFnv1a_64(name);

    /// <inheritdoc cref="GetBlock(string)"/>
    public SCBlock GetBlock(ReadOnlySpan<byte> name) => GetBlock(Hash(name));
    private static uint Hash(ReadOnlySpan<byte> name) => (uint)FnvHash.HashFnv1a_64(name);
    #endregion

    /// <summary>
    /// Tries to grab the actual block, and returns a new dummy if the block does not exist.
    /// </summary>
    /// <param name="key">Block Key</param>
    /// <returns>Block if exists, dummy if not. Dummy key will not match requested key.</returns>
    public SCBlock GetBlockSafe(uint key) => FindOrDefault(BlockInfo, key);

    private static SCBlock Find(IReadOnlyList<SCBlock> array, uint key)
    {
        var index = FindIndex(array, key);
        if (index != -1)
            return array[index];
        throw new KeyNotFoundException(nameof(key));
    }

    private static SCBlock FindOrDefault(IReadOnlyList<SCBlock> array, uint key)
    {
        var index = FindIndex(array, key);
        if (index != -1)
            return array[index];
        return new SCBlock(0, SCTypeCode.None);
    }

    /// <summary>
    /// Finds a specified <see cref="key"/> within the <see cref="array"/>.
    /// </summary>
    /// <remarks>
    /// Rather than storing a dictionary of keys, we can abuse the fact that the <see cref="BlockInfo"/> is stored in order of ascending block key.
    /// <br></br>
    /// Binary Search doesn't require extra memory like a Dictionary would; also, we usually only need to find a few blocks.
    /// </remarks>
    /// <param name="array">Index-able collection</param>
    /// <param name="key"><see cref="SCBlock.Key"/> to find.</param>
    /// <returns>Returns -1 if no match found.</returns>
    private static int FindIndex(IReadOnlyList<SCBlock> array, uint key)
    {
        int min = 0;
        int max = array.Count - 1;
        do
        {
            int mid = min + ((max - min) >> 1);
            var entry = array[mid];
            var ek = entry.Key;
            if (key == ek)
                return mid;

            if (key < ek)
                max = mid - 1;
            else
                min = mid + 1;
        } while (min <= max);
        return -1;
    }
}

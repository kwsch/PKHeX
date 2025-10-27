using System;

namespace PKHeX.Core;

/// <summary>
/// Provides indexed and key-based access to event work values.
/// </summary>
/// <typeparam name="T">Unmanaged value type.</typeparam>
public interface IEventValueStorage<T> where T : struct, IEquatable<T>
{
    /// <summary>
    /// Total number of entry slots available in the storage.
    /// </summary>
    int Count { get; }

    /// <summary>
    /// Number of used entries from the start until the first empty slot.
    /// </summary>
    int CountUsed { get; }

    /// <summary>
    /// Reads the value component of the entry at the specified index.
    /// </summary>
    /// <param name="index">Entry index.</param>
    /// <returns>The stored value.</returns>
    T GetValue(int index);

    /// <summary>
    /// Writes the value component of the entry at the specified index.
    /// </summary>
    /// <param name="index">Entry index.</param>
    /// <param name="value">Value to write.</param>
    void SetValue(int index, T value);

    /// <summary>
    /// Writes the value for the entry identified by the 64-bit key. Adds a new entry if not present.
    /// </summary>
    /// <param name="hash">64-bit key.</param>
    /// <param name="value">Value to write.</param>
    void SetValue(ulong hash, T value);

    /// <summary>
    /// Writes the value for the entry identified by the provided name. Adds a new entry if not present.
    /// </summary>
    /// <param name="name">Key name. Implementations may hash this to a 64-bit key.</param>
    /// <param name="value">Value to write.</param>
    void SetValue(ReadOnlySpan<char> name, T value);

    /// <summary>
    /// Reads the 64-bit key component of the entry at the specified index.
    /// </summary>
    /// <param name="index">Entry index.</param>
    ulong GetKey(int index);

    /// <summary>
    /// Attempts to retrieve a value by the provided name.
    /// </summary>
    /// <param name="name">Key name. Implementations may hash this to a 64-bit key.</param>
    /// <param name="value">Resulting value if found; otherwise the default of <typeparamref name="T"/>.</param>
    /// <returns>true if the key exists; otherwise false.</returns>
    bool TryGetValue(ReadOnlySpan<char> name, out T value);

    /// <summary>
    /// Attempts to retrieve a value by its 64-bit key.
    /// </summary>
    /// <param name="hash">64-bit key.</param>
    /// <param name="value">Resulting value if found; otherwise the default of <typeparamref name="T"/>.</param>
    /// <returns>true if the key exists; otherwise false.</returns>
    bool TryGetValue(ulong hash, out T value);

    /// <summary>
    /// Gets the index of the entry matching the provided 64-bit key, or -1 if not present.
    /// </summary>
    /// <param name="hash">64-bit key.</param>
    /// <returns>Index of the entry if found; otherwise -1.</returns>
    int GetIndex(ulong hash);

    /// <summary>
    /// Clears the entry at the specified index.
    /// </summary>
    /// <param name="index">Index of the entry to clear.</param>
    void Clear(int index);

    /// <summary>
    /// Compacts the storage by shifting used entries toward the start, preserving order.
    /// </summary>
    void Compress();

    /// <summary>
    /// Sorts used entries by their key in ascending order.
    /// </summary>
    void Sort();

    /// <summary>
    /// Checks if there is a free slot available and returns the index of the first free slot.
    /// </summary>
    /// <param name="index">Index of the first free slot if available; otherwise undefined.</param>
    /// <returns>true if a free slot exists; otherwise false.</returns>
    bool HasSpace(out int index);

    /// <summary>
    /// Checks if there is a free slot available.
    /// </summary>
    /// <returns>true if a free slot exists; otherwise false.</returns>
    bool HasSpace();
}

using System;
using System.Collections.Generic;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

/// <summary>
/// Generic storage for event work values keyed by a fixed-size key and backed by a 64-bit value field.
/// </summary>
/// <typeparam name="TKey">Key type. Implementations serialize/deserialize to a fixed-size number of 64-bit chunks.</typeparam>
/// <typeparam name="TValue">Unmanaged value type stored alongside the key. Backed by a 64-bit field.</typeparam>
/// <param name="sav">Owning save file.</param>
/// <param name="block">Underlying block providing the backing bytes.</param>
public abstract class EventWorkStorage<TKey, TValue>(SaveFile sav, SCBlock block) : SaveBlock<SaveFile>(sav, block.Raw)
    where TKey : struct, IEquatable<TKey>
    where TValue : struct, IEquatable<TValue>
{
    // Structure:
    // entry[] of key(u64 * N), value(u64)
    // Empty entries use an implementation-defined empty key value.
    // Default state is all entries initialized to "empty".

    /// <summary> Number of 64-bit chunks used to serialize a key. </summary>
    protected abstract int KeyU64Count { get; }

    /// <summary> Gets the empty key sentinel. </summary>
    protected abstract TKey EmptyKey { get; }

    /// <summary> Reads the key at the specified index. </summary>
    protected abstract TKey ReadKey(int index);

    /// <summary> Writes the key at the specified index. </summary>
    protected abstract void WriteKey(int index, TKey key);

    /// <summary> Compares two keys for sorting. Return same semantics as <see cref="IComparer{T}.Compare(T, T)"/>. </summary>
    protected abstract int CompareKeys(TKey a, TKey b);

    /// <summary> Size in bytes of a single entry: key bytes (8 * KeyU64Count) + 8 bytes for the value. </summary>
    protected int EntrySize => (KeyU64Count * sizeof(ulong)) + sizeof(ulong);

    public int Count => Data.Length / EntrySize;

    public int CountUsed => ScanCountUsed(Data);

    private int ScanCountUsed(ReadOnlySpan<byte> data)
    {
        int count = 0;
        for (int i = 0; i < Count; i++)
        {
            var key = ReadKey(i);
            if (IsEmpty(key))
                break;
            count++;
        }
        return count;
    }

    protected int GetOffset(int index)
    {
        if ((uint)index > Count)
            throw new ArgumentOutOfRangeException(nameof(index), index, null);
        return index * EntrySize;
    }

    protected bool IsEmpty(TKey key) => EqualityComparer<TKey>.Default.Equals(key, EmptyKey);

    public bool HasSpace(out int index) => (index = CountUsed) < Count;
    public bool HasSpace() => HasSpace(out _);

    public int GetIndex(TKey key)
    {
        for (int i = 0; i < Count; i++)
        {
            var entryKey = ReadKey(i);
            if (entryKey.Equals(key))
                return i;
            if (IsEmpty(entryKey))
                break;
        }
        return -1;
    }

    public void Clear(int index)
    {
        // Write empty key and zero value
        WriteKey(index, EmptyKey);
        WriteUInt64LittleEndian(Data.Slice(GetOffset(index) + (KeyU64Count * sizeof(ulong)), sizeof(ulong)), 0);
    }

    public void Compress()
    {
        int count = Count;
        int writePtr = 0;
        for (int readPtr = 0; readPtr < count; readPtr++)
        {
            var entryKey = ReadKey(readPtr);
            if (IsEmpty(entryKey))
                continue; // skip empty
            if (writePtr != readPtr)
            {
                var writeOfs = GetOffset(writePtr);
                var readOfs = GetOffset(readPtr);
                // Move entry
                var dest = Data.Slice(writeOfs, EntrySize);
                var src = Data.Slice(readOfs, EntrySize);
                src.CopyTo(dest);
                // Clear old entry
                Clear(readPtr);
            }
            writePtr++;
        }
    }

    public void Sort()
    {
        Compress();
        var used = CountUsed;
        var entries = new (TKey Key, ulong Value)[used];

        // Gather used entries.
        for (int i = 0; i < entries.Length; i++)
        {
            var data = Data[GetOffset(i)..];
            var key = ReadKey(i);
            var value = ReadUInt64LittleEndian(data.Slice(KeyU64Count * sizeof(ulong), sizeof(ulong)));
            entries[i] = (key, value);
        }

        Array.Sort(entries, Comparer<(TKey Key, ulong Value)>.Create((a, b) => CompareKeys(a.Key, b.Key)));

        // Write back
        for (int i = 0; i < entries.Length; i++)
        {
            WriteKey(i, entries[i].Key);
            WriteUInt64LittleEndian(Data.Slice(GetOffset(i) + (KeyU64Count * sizeof(ulong)), sizeof(ulong)), entries[i].Value);
        }
    }

    public abstract TValue GetValue(int index);
    public abstract void SetValue(int index, TValue value);

    public bool TryGetValue(TKey key, out TValue value)
    {
        var index = GetIndex(key);
        if (index == -1)
        {
            value = default;
            return false;
        }
        value = GetValue(index);
        return true;
    }

    public void SetValue(TKey key, TValue value)
    {
        var index = GetIndex(key);
        if (index == -1 && !HasSpace(out index))
            throw new InvalidOperationException("No space available to add new entry.");
        // Only ensure the key exists; value writing is done via SetValue(index, ...)
        WriteKey(index, key);
    }
}

/// <summary>
/// Storage for event work values keyed by a 64-bit FNV-1a hash.
/// </summary>
/// <typeparam name="TValue">Unmanaged value type stored alongside the hash. Backed by a 64-bit field.</typeparam>
/// <param name="sav">Owning save file.</param>
/// <param name="block">Underlying block providing the backing bytes.</param>
public abstract class EventWorkStorage64<TValue>(SaveFile sav, SCBlock block) : EventWorkStorage<ulong, TValue>(sav, block),
    IEventValueStorage<TValue> where TValue : struct, IEquatable<TValue>
{
    /// <summary>
    /// Size in bytes of a single entry: 8 bytes for the key hash + 8 bytes for the value.
    /// </summary>
    public const int ENTRY_SIZE = 16;
    private const ulong HashEmpty = FnvHash.HashEmpty;

    protected override int KeyU64Count => 1;
    protected override ulong EmptyKey => HashEmpty;

    protected override int CompareKeys(ulong a, ulong b) => a.CompareTo(b);

    protected override ulong ReadKey(int index)
    {
        var ofs = GetOffset(index);
        return ReadUInt64LittleEndian(Data.Slice(ofs, sizeof(ulong)));
    }

    protected override void WriteKey(int index, ulong key)
    {
        var ofs = GetOffset(index);
        WriteUInt64LittleEndian(Data.Slice(ofs, sizeof(ulong)), key);
    }

    private static ulong Hash(ReadOnlySpan<char> name) => FnvHash.HashFnv1a_64(name);

    public bool TryGetValue(ReadOnlySpan<char> name, out TValue value) => TryGetValue(Hash(name), out value);

    public ulong GetKey(int index) => ReadKey(index);
    public void SetKey(int index, ulong key) => WriteKey(index, key);

    public TValue this[ReadOnlySpan<char> name]
    {
        get => TryGetValue(name, out var val) ? val : throw new KeyNotFoundException($"No entry found for name '{name.ToString()}'");
        set => SetValue(name, value);
    }

    public TValue this[ulong hash]
    {
        get => TryGetValue(hash, out var val) ? val : throw new KeyNotFoundException($"No entry found for hash '{hash:X16}'");
        set => SetValue(hash, value);
    }

    public void SetValue(ReadOnlySpan<char> name, TValue value) => SetValue(Hash(name), value);
}

/// <summary>
/// Storage for event work values keyed by a 128-bit composite FNV-1a hash (two 64-bit parts).
/// </summary>
/// <typeparam name="TValue">Unmanaged value type stored alongside the key. Backed by a 64-bit field.</typeparam>
/// <param name="sav">Owning save file.</param>
/// <param name="block">Underlying block providing the backing bytes.</param>
public abstract class EventWorkStorageKey128<TValue>(SaveFile sav, SCBlock block) : EventWorkStorage<(ulong A, ulong B), TValue>(sav, block)
    where TValue : struct, IEquatable<TValue>
{
    protected override int KeyU64Count => 2;
    protected override (ulong A, ulong B) EmptyKey => (FnvHash.HashEmpty, FnvHash.HashEmpty);

    protected override int CompareKeys((ulong A, ulong B) a, (ulong A, ulong B) b)
    {
        int cmp = a.A.CompareTo(b.A);
        return cmp != 0 ? cmp : a.B.CompareTo(b.B);
    }

    protected override (ulong A, ulong B) ReadKey(int index)
    {
        var ofs = GetOffset(index);
        var a = ReadUInt64LittleEndian(Data.Slice(ofs + (0 * sizeof(ulong)), sizeof(ulong)));
        var b = ReadUInt64LittleEndian(Data.Slice(ofs + (1 * sizeof(ulong)), sizeof(ulong)));
        return (a, b);
    }

    protected override void WriteKey(int index, (ulong A, ulong B) key)
    {
        var ofs = GetOffset(index);
        WriteUInt64LittleEndian(Data.Slice(ofs + (0 * sizeof(ulong)), sizeof(ulong)), key.A);
        WriteUInt64LittleEndian(Data.Slice(ofs + (1 * sizeof(ulong)), sizeof(ulong)), key.B);
    }

    private static ulong Hash(ReadOnlySpan<char> name) => FnvHash.HashFnv1a_64(name);

    // Helper methods for convenience
    public bool TryGetValue(ReadOnlySpan<char> a, ReadOnlySpan<char> b, out TValue value)
        => TryGetValue((Hash(a), Hash(b)), out value);

    public void SetValue(ReadOnlySpan<char> a, ReadOnlySpan<char> b, TValue value)
        => SetValue((Hash(a), Hash(b)), value);

    public (ulong A, ulong B) GetKey(int index) => ReadKey(index);
    public void SetKey(int index, (ulong A, ulong B) key) => WriteKey(index, key);
    public void SetKey(int index, ulong a, ulong b) => WriteKey(index, (a, b));
}

/// <summary>
/// Storage for event work values keyed by a 192-bit composite FNV-1a hash (three 64-bit parts).
/// </summary>
/// <typeparam name="TValue">Unmanaged value type stored alongside the key. Backed by a 64-bit field.</typeparam>
/// <param name="sav">Owning save file.</param>
/// <param name="block">Underlying block providing the backing bytes.</param>
public abstract class EventWorkStorageKey192<TValue>(SaveFile sav, SCBlock block) : EventWorkStorage<(ulong A, ulong B, ulong C), TValue>(sav, block)
    where TValue : struct, IEquatable<TValue>
{
    protected override int KeyU64Count => 3;
    protected override (ulong A, ulong B, ulong C) EmptyKey => (FnvHash.HashEmpty, FnvHash.HashEmpty, FnvHash.HashEmpty);

    protected override int CompareKeys((ulong A, ulong B, ulong C) a, (ulong A, ulong B, ulong C) b)
    {
        int cmp = a.A.CompareTo(b.A);
        if (cmp != 0) return cmp;
        cmp = a.B.CompareTo(b.B);
        return cmp != 0 ? cmp : a.C.CompareTo(b.C);
    }

    protected override (ulong A, ulong B, ulong C) ReadKey(int index)
    {
        var ofs = GetOffset(index);
        var a = ReadUInt64LittleEndian(Data.Slice(ofs + (0 * sizeof(ulong)), sizeof(ulong)));
        var b = ReadUInt64LittleEndian(Data.Slice(ofs + (1 * sizeof(ulong)), sizeof(ulong)));
        var c = ReadUInt64LittleEndian(Data.Slice(ofs + (2 * sizeof(ulong)), sizeof(ulong)));
        return (a, b, c);
    }

    protected override void WriteKey(int index, (ulong A, ulong B, ulong C) key)
    {
        var ofs = GetOffset(index);
        WriteUInt64LittleEndian(Data.Slice(ofs + (0 * sizeof(ulong)), sizeof(ulong)), key.A);
        WriteUInt64LittleEndian(Data.Slice(ofs + (1 * sizeof(ulong)), sizeof(ulong)), key.B);
        WriteUInt64LittleEndian(Data.Slice(ofs + (2 * sizeof(ulong)), sizeof(ulong)), key.C);
    }

    private static ulong Hash(ReadOnlySpan<char> name) => FnvHash.HashFnv1a_64(name);

    // Helper methods for convenience
    public bool TryGetValue(ReadOnlySpan<char> a, ReadOnlySpan<char> b, ReadOnlySpan<char> c, out TValue value)
        => TryGetValue((Hash(a), Hash(b), Hash(c)), out value);

    public void SetValue(ReadOnlySpan<char> a, ReadOnlySpan<char> b, ReadOnlySpan<char> c, TValue value)
        => SetValue((Hash(a), Hash(b), Hash(c)), value);

    public (ulong A, ulong B, ulong C) GetKey(int index) => ReadKey(index);
    public void SetKey(int index, (ulong A, ulong B, ulong C) key) => WriteKey(index, key);
    public void SetKey(int index, ulong a, ulong b, ulong c) => WriteKey(index, (a, b, c));
}

/// <summary>
/// Event work storage specialized for boolean flags. Stores 0 or 1 in the value field.
/// </summary>
/// <param name="sav">Owning save file.</param>
/// <param name="block">Underlying block providing the backing bytes.</param>
public sealed class EventWorkFlagStorage(SAV9ZA sav, SCBlock block) : EventWorkStorage64<bool>(sav, block)
{
    public override bool GetValue(int index)
    {
        var ofs = GetOffset(index); // key
        return ReadUInt64LittleEndian(Data.Slice(ofs + 8, 8)) != 0;
    }

    public override void SetValue(int index, bool value)
    {
        var ofs = GetOffset(index); // key
        WriteUInt64LittleEndian(Data.Slice(ofs + 8, 8), value ? 1UL : 0UL);
    }
}

/// <summary>
/// Event work storage specialized for 64-bit integer values.
/// </summary>
/// <param name="sav">Owning save file.</param>
/// <param name="block">Underlying block providing the backing bytes.</param>
public sealed class EventWorkValueStorage(SAV9ZA sav, SCBlock block) : EventWorkStorage64<ulong>(sav, block)
{
    public override ulong GetValue(int index)
    {
        var ofs = GetOffset(index); // key
        return ReadUInt64LittleEndian(Data.Slice(ofs + 8, 8));
    }

    public override void SetValue(int index, ulong value)
    {
        var ofs = GetOffset(index); // key
        WriteUInt64LittleEndian(Data.Slice(ofs + 8, 8), value);
    }
}

/// <summary>
/// Event work storage specialized for 64-bit integer values with 128-bit keys.
/// </summary>
/// <param name="sav">Owning save file.</param>
/// <param name="block">Underlying block providing the backing bytes.</param>
public sealed class EventWorkValueStorageKey128(SAV9ZA sav, SCBlock block) : EventWorkStorageKey128<ulong>(sav, block)
{
    public override ulong GetValue(int index)
    {
        var ofs = GetOffset(index);
        return ReadUInt64LittleEndian(Data.Slice(ofs + (KeyU64Count * sizeof(ulong)), sizeof(ulong)));
    }

    public override void SetValue(int index, ulong value)
    {
        var ofs = GetOffset(index);
        WriteUInt64LittleEndian(Data.Slice(ofs + (KeyU64Count * sizeof(ulong)), sizeof(ulong)), value);
    }
}

/// <summary>
/// Event work storage specialized for 64-bit integer values with 192-bit keys.
/// </summary>
/// <param name="sav">Owning save file.</param>
/// <param name="block">Underlying block providing the backing bytes.</param>
public sealed class EventWorkValueStorageKey192(SAV9ZA sav, SCBlock block) : EventWorkStorageKey192<ulong>(sav, block)
{
    public override ulong GetValue(int index)
    {
        var ofs = GetOffset(index);
        return ReadUInt64LittleEndian(Data.Slice(ofs + (KeyU64Count * sizeof(ulong)), sizeof(ulong)));
    }

    public override void SetValue(int index, ulong value)
    {
        var ofs = GetOffset(index);
        WriteUInt64LittleEndian(Data.Slice(ofs + (KeyU64Count * sizeof(ulong)), sizeof(ulong)), value);
    }
}


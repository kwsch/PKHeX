using System;
using System.Collections.Generic;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

/// <summary>
/// Storage for event work values keyed by a 64-bit FNV-1a hash.
/// </summary>
/// <typeparam name="T">Unmanaged value type stored alongside the hash. Backed by a 64-bit field.</typeparam>
/// <param name="sav">Owning save file.</param>
/// <param name="block">Underlying block providing the backing bytes.</param>
public abstract class EventWorkStorage64<T>(SaveFile sav, SCBlock block) : SaveBlock<SaveFile>(sav, block.Raw),
    IEventValueStorage<T> where T : struct, IEquatable<T>
{
    // Structure:
    // entry[] of u64 hash, T value
    // T is backed by a separate u64
    // Essentially, it is
    //      (u64, u64)[Count];
    // Empty entries use an empty hash value instead of 0.
    // Default state is all entries initialized to "empty".

    /// <summary>
    /// Size in bytes of a single entry: 8 bytes for the key hash + 8 bytes for the value.
    /// </summary>
    public const int ENTRY_SIZE = 16;
    private const ulong HashEmpty = FnvHash.HashEmpty;

    public int Count => Data.Length / ENTRY_SIZE;
    public int CountUsed => ScanCountUsed(Data);

    private int ScanCountUsed(ReadOnlySpan<byte> data)
    {
        int count = 0;
        for (int i = 0; i < Count; i++)
        {
            var hash = ReadUInt64LittleEndian(data[GetOffset(i)..]);
            if (hash == 0)
                break;
            count++;
        }
        return count;
    }

    private static ulong Hash(ReadOnlySpan<char> name) => FnvHash.HashFnv1a_64(name);

    public bool HasSpace(out int index) => (index = CountUsed) < Count;
    public bool HasSpace() => HasSpace(out _);
    public bool TryGetValue(ReadOnlySpan<char> name, out T value) => TryGetValue(Hash(name), out value);

    public bool TryGetValue(ulong hash, out T value)
    {
        var index = GetIndex(hash);
        if (index == -1)
        {
            value = default;
            return false;
        }
        value = GetValue(index);
        return true;
    }

    protected int GetOffset(int index)
    {
        if ((uint)index > Count)
            throw new ArgumentOutOfRangeException(nameof(index), index, null);
        return index * ENTRY_SIZE;
    }

    public int GetIndex(ulong hash)
    {
        for (int i = 0; i < Count; i++)
        {
            var ofs = i * ENTRY_SIZE;
            var entryHash = ReadUInt64LittleEndian(Data.Slice(ofs, 8));
            if (entryHash == hash)
                return i;
            if (entryHash == HashEmpty)
                break;
        }
        return -1;
    }

    public void Clear(int index) => WriteEntry(Data[GetOffset(index)..], HashEmpty, 0);

    private void WriteEntry(int index, ulong hash, ulong value) => WriteEntry(Data[GetOffset(index)..], hash, value);

    private static void WriteEntry(Span<byte> data, ulong hash, ulong value)
    {
        WriteUInt64LittleEndian(data, hash);
        WriteUInt64LittleEndian(data[8..], value);
    }

    public void Compress()
    {
        int count = Count;
        int writePtr = 0;
        for (int readPtr = 0; readPtr < count; readPtr++)
        {
            var readOfs = GetOffset(readPtr);
            var entryHash = ReadUInt64LittleEndian(Data.Slice(readOfs, 8));
            if (entryHash == HashEmpty)
                continue; // skip empty
            if (writePtr != readPtr)
            {
                var writeOfs = GetOffset(writePtr);
                // Move entry
                var dest = Data.Slice(writeOfs, ENTRY_SIZE);
                var src = Data.Slice(readOfs, ENTRY_SIZE);
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
        var entries = new (ulong Hash, ulong Value)[CountUsed];

        // Gather used entries.
        for (int i = 0; i < entries.Length; i++)
        {
            var data = Data[GetOffset(i)..];
            var entryHash = ReadUInt64LittleEndian(data);
            var entryValue = ReadUInt64LittleEndian(data[8..]);
            entries[i] = (entryHash, entryValue);
        }

        Array.Sort(entries, Comparer<(ulong Hash, ulong Value)>.Create((a, b) => a.Hash.CompareTo(b.Hash)));

        // Write back
        for (int i = 0; i < entries.Length; i++)
            WriteEntry(i, entries[i].Hash, entries[i].Value);
    }

    public ulong GetKey(int index) => ReadUInt64LittleEndian(Data.Slice(GetOffset(index), 8));
    public void SetKey(int index, ulong key) => WriteUInt64LittleEndian(Data.Slice(GetOffset(index), 8), key);
    public abstract T GetValue(int index);
    public abstract void SetValue(int index, T value);

    public T this[ReadOnlySpan<char> name]
    {
        get => TryGetValue(name, out var val) ? val : throw new KeyNotFoundException($"No entry found for name '{name.ToString()}'");
        set => SetValue(name, value);
    }

    public T this[ulong hash]
    {
        get => TryGetValue(hash, out var val) ? val : throw new KeyNotFoundException($"No entry found for hash '{hash:X16}'");
        set => SetValue(hash, value);
    }

    public void SetValue(ulong hash, T value)
    {
        var index = GetIndex(hash);
        if (index == -1 && !HasSpace(out index))
            throw new InvalidOperationException("No space available to add new entry.");
        WriteEntry(index, hash, 0);
    }

    public void SetValue(ReadOnlySpan<char> name, T value) => SetValue(Hash(name), value);
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


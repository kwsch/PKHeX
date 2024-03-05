using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace PKHeX.Core;

public sealed class SCBlockCompare
{
    private readonly List<string> AddedKeys = [];
    private readonly List<string> RemovedKeys = [];
    private readonly List<string> TypesChanged = [];
    private readonly List<string> ValueChanged = [];

    private readonly Dictionary<uint, string> KeyNames;
    private string GetKeyName(uint key) => KeyNames.TryGetValue(key, out var value) ? value : $"{key:X8}";

    public SCBlockCompare(SCBlockAccessor s1, SCBlockAccessor s2, IEnumerable<string> extraKeyNames)
    {
        var b1 = s1.BlockInfo;
        var b2 = s2.BlockInfo;
        KeyNames = GetKeyNames(s1, b1, b2);
        SCBlockMetadata.AddExtraKeyNames(KeyNames, extraKeyNames);

        var hs1 = new HashSet<uint>(b1.Select(z => z.Key));
        var hs2 = new HashSet<uint>(b2.Select(z => z.Key));

        LoadAddRemove(s1, s2, hs1, hs2);
        hs1.IntersectWith(hs2);
        LoadChanged(s1, s2, hs1);
    }

    private void LoadAddRemove(SCBlockAccessor s1, SCBlockAccessor s2, ICollection<uint> hs1, IEnumerable<uint> hs2)
    {
        var unique = new HashSet<uint>(hs1);
        unique.SymmetricExceptWith(hs2);
        foreach (var k in unique)
        {
            var name = GetKeyName(k);
            if (hs1.Contains(k))
            {
                var b = s1.GetBlock(k);
                RemovedKeys.Add($"{name} - {b.Type}");
            }
            else
            {
                var b = s2.GetBlock(k);
                AddedKeys.Add($"{name} - {b.Type} - 0x{b.Data.Length:X5} {b.Data.Length}");
            }
        }
    }

    private void LoadChanged(SCBlockAccessor s1, SCBlockAccessor s2, IEnumerable<uint> shared)
    {
        foreach (var k in shared)
        {
            var x1 = s1.GetBlock(k);
            var x2 = s2.GetBlock(k);
            var name = GetKeyName(x1.Key);
            if (x1.Type != x2.Type)
            {
                TypesChanged.Add($"{name} - {x1.Type} => {x2.Type}");
                continue;
            }
            if (x1.Data.Length != x2.Data.Length)
            {
                ValueChanged.Add($"{name} - Length: {x1.Data.Length} => {x2.Data.Length}");
                continue;
            }

            if (x1.Data.Length == 0)
                continue;

            if (x1.Type is SCTypeCode.Object or SCTypeCode.Array)
            {
                if (!x1.Data.SequenceEqual(x2.Data))
                    ValueChanged.Add($"Bytes Changed: Length: {x1.Data.Length} {name}");
                continue;
            }

            var val1 = x1.GetValue();
            var val2 = x2.GetValue();
            if (Equals(val1, val2))
                continue;
            if (val1 is ulong u1 && val2 is ulong u2)
                ValueChanged.Add($"{name} - {u1:X8} => {u2:x8}");
            else
                ValueChanged.Add($"{name} - {val1} => {val2}");
        }
    }

    private static Dictionary<uint, string> GetKeyNames(SCBlockAccessor s1, IEnumerable<SCBlock> b1, IEnumerable<SCBlock> b2)
    {
        var aType = s1.GetType();
        var b1n = aType.GetAllPropertiesOfType<IDataIndirect>(s1);
        var names = aType.GetAllConstantsOfType<uint>();
        ReplaceLabels(b1n, b1);
        ReplaceLabels(b1n, b2);

        // Replace all const name labels with explicit block property names if they exist.
        // Since our Block classes do not retain the u32 key they originated from, we need to compare the buffers to see if they match.
        // Could have just checked ContainsKey then indexed in, but I wanted to play with the higher performance API method to get the bucket and mutate directly.
        void ReplaceLabels(Dictionary<string, IDataIndirect> list, IEnumerable<SCBlock> blocks)
        {
            foreach (var b in blocks)
            {
                var match = list.FirstOrDefault(z => z.Value.Equals(b.Data));
                if (match.Key is not { } x)
                    continue;
                ref var exist = ref CollectionsMarshal.GetValueRefOrNullRef(names, b.Key);
                if (!Unsafe.IsNullRef(ref exist))
                    exist = x;
            }
        }
        return names;
    }

    public IReadOnlyList<string> Summary()
    {
        var result = new List<string>();
        AddIfPresent(result, AddedKeys, "Blocks Added:");
        AddIfPresent(result, RemovedKeys, "Blocks Removed:");
        AddIfPresent(result, TypesChanged, "BlockType Changed:");
        AddIfPresent(result, ValueChanged, "Value Changed:", true);

        return result;

        static void AddIfPresent(List<string> result, ICollection<string> list, string hdr, bool sort = false)
        {
            if (list.Count == 0)
                return;
            result.Add(hdr);
            result.AddRange(list);
            if (sort)
                result.Sort(result.Count - list.Count, list.Count, StringComparer.Ordinal);
            result.Add(string.Empty);
        }
    }
}

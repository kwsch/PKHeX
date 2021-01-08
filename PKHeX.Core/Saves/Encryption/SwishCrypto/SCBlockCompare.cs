using System.Collections.Generic;
using System.Linq;

namespace PKHeX.Core
{
    public sealed class SCBlockCompare
    {
        private readonly List<string> AddedKeys = new();
        private readonly List<string> RemovedKeys = new();
        private readonly List<string> TypesChanged = new();
        private readonly List<string> ValueChanged = new();

        private readonly Dictionary<uint, string> KeyNames;
        private string GetKeyName(uint key) => KeyNames.TryGetValue(key, out var val) ? val : $"{key:X8}";

        public SCBlockCompare(SCBlockAccessor s1, SCBlockAccessor s2)
        {
            var b1 = s1.BlockInfo;
            var b2 = s2.BlockInfo;
            KeyNames = GetKeyNames(s1, b1, b2);
            SCBlockMetadata.AddExtraKeyNames(KeyNames);

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
                    AddedKeys.Add($"{name} - {b.Type}");
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
                        ValueChanged.Add($"{name} - Bytes Changed");
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
            var b1n = aType.GetAllPropertiesOfType<SaveBlock>(s1);
            var names = aType.GetAllConstantsOfType<uint>();
            Add(b1n, b1);
            Add(b1n, b2);

            void Add(Dictionary<SaveBlock, string> list, IEnumerable<SCBlock> blocks)
            {
                foreach (var b in blocks)
                {
                    var match = list.FirstOrDefault(z => ReferenceEquals(z.Key.Data, b.Data));
                    if (match.Value != null && names.ContainsKey(b.Key))
                        names[b.Key] = match.Value;
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
            AddIfPresent(result, ValueChanged, "Value Changed:");

            return result;

            static void AddIfPresent(List<string> result, ICollection<string> list, string hdr)
            {
                if (list.Count == 0)
                    return;
                result.Add(hdr);
                result.AddRange(list);
                result.Add(string.Empty);
            }
        }
    }
}
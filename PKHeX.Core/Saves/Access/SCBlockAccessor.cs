using System.Collections.Generic;

namespace PKHeX.Core
{
    /// <summary>
    /// <see cref="SwishCrypto"/> block accessor, where blocks are ordered by ascending <see cref="SCBlock.Key"/>.
    /// </summary>
    public abstract class SCBlockAccessor : ISaveBlockAccessor<SCBlock>
    {
        public abstract IReadOnlyList<SCBlock> BlockInfo { get; }

        public object GetBlockValue(uint key) => GetBlock(key).GetValue();
        public void SetBlockValue(uint key, object value) => GetBlock(key).SetValue(value);

        // Rather than storing a dictionary of keys, we can abuse the fact that the SCBlock[] is stored in order of ascending block key.
        // Binary Search doesn't require extra memory like a Dictionary would; also, we only need to find a few blocks.
        public SCBlock GetBlock(uint key) => BinarySearch(BlockInfo, key);

        private static SCBlock BinarySearch(IReadOnlyList<SCBlock> arr, uint key)
        {
            int min = 0;
            int max = arr.Count - 1;
            do
            {
                int mid = (min + max) / 2;
                var entry = arr[mid];
                var ek = entry.Key;
                if (key == ek)
                    return entry;

                if (key < ek)
                    max = mid - 1;
                else
                    min = mid + 1;
            } while (min <= max);
            throw new KeyNotFoundException(nameof(key));
        }
    }
}
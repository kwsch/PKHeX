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

        /// <summary>
        /// Tries to grab the actual block, and returns a new dummy if the block does not exist.
        /// </summary>
        /// <param name="key">Block Key</param>
        /// <returns>Block if exists, dummy if not. Dummy key will not match requested key.</returns>
        public SCBlock GetBlockSafe(uint key)
        {
            try { return GetBlock(key); }
#pragma warning disable CA1031 // Do not catch general exception types
            catch (KeyNotFoundException) { return new SCBlock(0, SCTypeCode.None); }
#pragma warning restore CA1031 // Do not catch general exception types
        }

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

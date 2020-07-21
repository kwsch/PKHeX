using System.Collections.Generic;
using System.Linq;

namespace PKHeX.Core
{
    public sealed class FashionUnlock8 : SaveBlock
    {
        private const int SIZE_ENTRY = 0x80;
        private const int REGIONS = 15;

        public FashionUnlock8(SaveFile sav, SCBlock block) : base(sav, block.Data) { }

        public bool[] GetArrayOwned(int region) => ArrayUtil.GitBitFlagArray(Data, region * SIZE_ENTRY, SIZE_ENTRY * 8);
        public bool[] GetArrayNew(int region) => ArrayUtil.GitBitFlagArray(Data, (region + REGIONS) * SIZE_ENTRY, SIZE_ENTRY * 8);
        public int[] GetIndexesOwned(int region) => GetIndexes(GetArrayOwned(region));
        public int[] GetIndexesNew(int region) => GetIndexes(GetArrayNew(region));

        public void SetArrayOwned(int region, bool[] value) => ArrayUtil.SetBitFlagArray(Data, region * SIZE_ENTRY, value);
        public void SetArrayNew(int region, bool[] value) => ArrayUtil.SetBitFlagArray(Data, (region + REGIONS) * SIZE_ENTRY, value);
        public void SetIndexesOwned(int region, int[] value) => SetArrayOwned(region, SetIndexes(value));
        public void SetIndexesNew(int region, int[] value) => SetArrayNew(region, SetIndexes(value));

        public static int[] GetIndexes(bool[] arr)
        {
            var list = new List<int>();
            for (int i = 0; i < arr.Length; i++)
            {
                if (arr[i])
                    list.Add(i);
            }
            return list.ToArray();
        }

        public static bool[] SetIndexes(int[] arr)
        {
            var result = new bool[arr.Max()];
            foreach (var index in arr)
                result[index] = true;
            return result;
        }
    }
}
using System.Collections.Generic;

namespace PKHeX.Core
{
    public static class Records
    {
        private const int LargeRecordCount = 100;
        private const int SmallRecordCount = 100;
        private const int Count = LargeRecordCount + SmallRecordCount;

        /// <summary>
        /// Gets the maximum value for the specified record using the provided maximum list.
        /// </summary>
        /// <param name="recordID">Record ID to retrieve the maximum for</param>
        /// <param name="maxes">Maximum enum values for each record</param>
        /// <returns>Maximum the record can be</returns>
        public static int GetMax(int recordID, IReadOnlyList<int> maxes)
        {
            if (recordID >= Count)
                return 0;
            return MaxByType[maxes[recordID]];
        }

        public static int GetOffset(int baseOfs, int recordID)
        {
            if (recordID < LargeRecordCount)
                return baseOfs + (recordID * 4);
            if (recordID < Count)
                return baseOfs + (recordID * 2) + 200; // first 100 are 4bytes, so bias the difference
            return -1;
        }

        private static readonly int[] MaxByType = { 999999999, 9999999, 999999, 99999, 65535, 9999, 999 };

        public static readonly IReadOnlyList<int> MaxType_SM = new[]
        {
            0, 0, 0, 0, 0, 0, 2, 2, 2, 0,
            0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
            0, 0, 0, 0, 2, 2, 2, 0, 0, 0,
            0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
            0, 2, 2, 2, 0, 0, 0, 2, 2, 0,
            0, 2, 2, 2, 2, 2, 2, 2, 2, 2,
            2, 2, 2, 2, 2, 2, 1, 2, 2, 2,
            2, 2, 2, 2, 2, 2, 2, 2, 2, 2,
            2, 2, 2, 2, 2, 2, 2, 2, 2, 2,
            2, 2, 2, 2, 2, 2, 2, 2, 2, 2,

            5, 5, 5, 5, 5, 5, 5, 5, 5, 5,
            5, 5, 5, 5, 5, 5, 5, 5, 5, 5,
            5, 5, 5, 5, 5, 5, 6, 5, 5, 5,
            5, 5, 5, 5, 5, 5, 5, 5, 5, 5,
            5, 5, 5, 5, 5, 5, 5, 5, 5, 5,
            5, 5, 5, 5, 5, 5, 5, 5, 5, 5,
            5, 5, 5, 5, 5, 5, 5, 5, 5, 5,
            5, 5, 5, 5, 5, 5, 5, 5, 5, 5,
            5, 5, 5, 5, 5, 5, 5, 5, 5, 5,
            5, 5, 5, 5, 5, 5, 5, 5, 5, 5,
        };

        public static readonly IReadOnlyList<int> MaxType_USUM = new[]
        {
            0, 0, 0, 0, 0, 0, 2, 2, 2, 0,
            0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
            0, 0, 0, 0, 2, 2, 2, 0, 0, 0,
            0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
            0, 2, 2, 2, 0, 0, 0, 2, 2, 0,
            0, 2, 2, 2, 2, 2, 2, 2, 2, 2,
            2, 2, 2, 2, 2, 2, 1, 2, 2, 2,
            0, 0, 0, 0, 0, 2, 2, 2, 2, 2,
            2, 2, 2, 2, 2, 2, 2, 2, 2, 2,
            2, 2, 2, 2, 2, 2, 2, 2, 2, 2,

            5, 5, 5, 5, 5, 5, 5, 5, 5, 5,
            5, 5, 5, 5, 5, 5, 5, 5, 5, 5,
            5, 5, 5, 5, 5, 5, 6, 5, 5, 5,
            5, 5, 5, 5, 5, 5, 5, 5, 5, 5,
            5, 5, 5, 5, 5, 5, 5, 5, 5, 5,
            5, 5, 5, 5, 5, 5, 5, 5, 5, 5,
            5, 5, 5, 5, 5, 5, 5, 5, 5, 5,
            5, 5, 5, 5, 5, 5, 5, 5, 5, 5,
            5, 5, 5, 5, 5, 5, 5, 5, 5, 5,
            5, 5, 4, 4, 4, 5, 5, 4, 5, 5
        };
    }
}

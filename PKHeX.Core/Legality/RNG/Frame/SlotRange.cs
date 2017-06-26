namespace PKHeX.Core
{
    public static class SlotRange
    {
        private static readonly Range[] H_OldRod = GetRanges(70, 30);
        private static readonly Range[] H_GoodRod = GetRanges(60, 20, 20);
        private static readonly Range[] H_SuperRod = GetRanges(40, 30, 15, 10, 5);
        private static readonly Range[] H_Surf = GetRanges(60, 30, 5, 4, 1);
        private static readonly Range[] H_Regular = GetRanges(20, 20, 10, 10, 10, 10, 5, 5, 4, 4, 1, 1);

        private static readonly Range[] J_SuperRod = GetRanges(40, 40, 15, 4, 1);
        private static readonly Range[] K_BCC = Reverse(H_Regular);
        private static readonly Range[] K_Headbutt = GetRanges(50, 15, 15, 10, 5, 5);

        public static int GetSlot(SlotType type, uint rand, FrameType t)
        {
            switch (t)
            {
                case FrameType.MethodH:
                    return HSlot(type, rand);
                case FrameType.MethodJ:
                    return JSlot(type, rand);
                case FrameType.MethodK:
                    return KSlot(type, rand);
            }
            return -1;
        }

        private static int HSlot(SlotType type, uint rand)
        {
            var ESV = rand % 100;
            switch (type)
            {
                case SlotType.Old_Rod:
                    return CalcSlot(ESV, H_OldRod);
                case SlotType.Good_Rod:
                    return CalcSlot(ESV, H_GoodRod);
                case SlotType.Super_Rod:
                    return CalcSlot(ESV, H_SuperRod);
                case SlotType.Surf:
                    return CalcSlot(ESV, H_Surf);
                default:
                    return CalcSlot(ESV, H_Regular);
            }
        }
        private static int KSlot(SlotType type, uint rand)
        {
            var ESV = rand % 100;
            switch (type)
            {
                case SlotType.Surf:
                    return CalcSlot(ESV, H_Surf);
                case SlotType.Super_Rod:
                case SlotType.Good_Rod:
                case SlotType.Old_Rod:
                    return CalcSlot(ESV, H_SuperRod);
                case SlotType.BugContest:
                    return CalcSlot(ESV, K_BCC);
                case SlotType.Grass_Safari:
                case SlotType.Surf_Safari:
                case SlotType.Old_Rod_Safari:
                case SlotType.Good_Rod_Safari:
                case SlotType.Super_Rod_Safari:
                case SlotType.Rock_Smash_Safari:
                    return (int)(rand % 10);
                case SlotType.Headbutt:
                case SlotType.Headbutt_Special:
                    return CalcSlot(ESV, K_Headbutt);
                default:
                    return CalcSlot(ESV, H_Regular);
            }
        }
        private static int JSlot(SlotType type, uint rand)
        {
            uint ESV = rand / 656;
            switch (type)
            {
                case SlotType.Old_Rod:
                case SlotType.Surf:
                    return CalcSlot(ESV, H_Surf);
                case SlotType.Good_Rod:
                case SlotType.Super_Rod:
                    return CalcSlot(ESV, J_SuperRod);
                default:
                    return CalcSlot(ESV, H_Regular);
            }
        }

        private struct Range
        {
            internal Range(uint min, uint max)
            {
                Min = min;
                Max = max;
            }

            internal uint Min { get; }
            internal uint Max { get; }
        }

        private static Range[] GetRanges(params uint[] rates)
        {
            var len = rates.Length;
            var arr = new Range[len];
            uint sum = 0;
            for (int i = 0; i < len; ++i)
                arr[i] = new Range(sum, (sum += rates[i]) - 1);
            return arr;
        }

        private static int CalcSlot(uint esv, Range[] ranges)
        {
            for (int i = 0; i < ranges.Length; ++i)
                if (esv >= ranges[i].Min && esv <= ranges[i].Max)
                    return i;

            return -1;
        }

        private static Range[] Reverse(Range[] r)
        {
            var arr = new Range[r.Length];
            for (int i = 0; i < arr.Length; ++i)
                arr[i] = r[r.Length - 1 - i];
            return arr;
        }
    }
}

using System.Linq;

namespace PKHeX.Core
{
    public static class SlotRange
    {
        private static readonly Range[] H_OldRod = GetRanges(70, 30);
        private static readonly Range[] H_GoodRod = GetRanges(60, 20, 20);
        private static readonly Range[] H_SuperRod = GetRanges(40, 40, 15, 4, 1);
        private static readonly Range[] H_Surf = GetRanges(60, 30, 5, 4, 1);
        private static readonly Range[] H_Regular = GetRanges(20, 20, 10, 10, 10, 10, 5, 5, 4, 4, 1, 1);

        private static readonly Range[] J_SuperRod = GetRanges(40, 40, 15, 4, 1);
        private static readonly Range[] K_SuperRod = GetRanges(40, 30, 15, 10, 5);
        private static readonly Range[] K_BCC = GetRanges(5,5,5,5, 10,10,10,10, 20,20).Reverse().ToArray();
        private static readonly Range[] K_Headbutt = GetRanges(50, 15, 15, 10, 5, 5);

        public static int GetSlot(SlotType type, uint rand, FrameType t) => t switch
        {
            FrameType.MethodH => HSlot(type, rand),
            FrameType.MethodJ => JSlot(type, rand),
            FrameType.MethodK => KSlot(type, rand),
            _ => -1
        };

        private static int HSlot(SlotType type, uint rand)
        {
            var ESV = rand % 100;
            return type switch
            {
                SlotType.Old_Rod =>        CalcSlot(ESV, H_OldRod),
                SlotType.Good_Rod =>        CalcSlot(ESV, H_GoodRod),
                SlotType.Super_Rod =>        CalcSlot(ESV, H_SuperRod),
                SlotType.Rock_Smash =>        CalcSlot(ESV, H_Surf),
                SlotType.Surf =>        CalcSlot(ESV, H_Surf),
                SlotType.Swarm => (ESV < 50 ? 0 : -1),
                _ => CalcSlot(ESV, H_Regular)
            };
        }

        private static int KSlot(SlotType type, uint rand)
        {
            var ESV = rand % 100;
            return type switch
            {
                SlotType.Rock_Smash or SlotType.Surf => CalcSlot(ESV, H_Surf),
                SlotType.Super_Rod or SlotType.Good_Rod or SlotType.Old_Rod => CalcSlot(ESV, K_SuperRod),
                SlotType.BugContest => CalcSlot(ESV, K_BCC),
                SlotType.Headbutt => CalcSlot(ESV, K_Headbutt),
                _ => CalcSlot(ESV, H_Regular)
            };
        }

        private static int JSlot(SlotType type, uint rand)
        {
            uint ESV = rand / 656;
            return type switch
            {
                SlotType.Old_Rod or SlotType.Rock_Smash or SlotType.Surf => CalcSlot(ESV, H_Surf),
                SlotType.Good_Rod or SlotType.Super_Rod => CalcSlot(ESV, J_SuperRod),
                SlotType.HoneyTree => 0,
                _ => CalcSlot(ESV, H_Regular)
            };
        }

        private readonly struct Range
        {
            internal readonly uint Min;
            internal readonly uint Max;

            internal Range(uint min, uint max)
            {
                Min = min;
                Max = max;
            }
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
            {
                if (esv >= ranges[i].Min && esv <= ranges[i].Max)
                    return i;
            }

            return -1;
        }

        public static int GetLevel(EncounterSlot slot, LeadRequired lead, uint lvlrand)
        {
            if (lead == LeadRequired.PressureHustleSpirit)
                return slot.LevelMax;
            if (slot.LevelMin == slot.LevelMax)
                return slot.LevelMin;
            int delta = slot.LevelMax - slot.LevelMin + 1;
            var adjust = (int)(lvlrand % delta);

            return slot.LevelMin + adjust;
        }

#pragma warning disable IDE0060, RCS1163 // Unused parameter.
        public static bool GetIsEncounterable(EncounterSlot slot, FrameType frameType, int rand, LeadRequired lead)
#pragma warning restore IDE0060, RCS1163 // Unused parameter.
        {
            if (slot.Area.Type.IsSweetScentType())
                return true;
            return true; // todo
            //return GetCanEncounter(slot, frameType, rand, lead);
        }

        // ReSharper disable once UnusedMember.Global
        public static bool GetCanEncounter(EncounterSlot slot, FrameType frameType, int rand, LeadRequired lead)
        {
            int proc = frameType == FrameType.MethodJ ? rand / 656 : rand % 100;
            var stype = slot.Area.Type;
            if (stype == SlotType.Rock_Smash)
                return proc < 60;
            if (frameType == FrameType.MethodH)
                return true; // fishing encounters are disjointed by the hooked message.

            // fishing
            if (stype == SlotType.Old_Rod)
            {
                if (proc < 25)
                    return true;
                if (proc < 50)
                    return lead == LeadRequired.None;
            }
            else if (stype == SlotType.Good_Rod)
            {
                if (proc < 50)
                    return true;
                if (proc < 75)
                    return lead == LeadRequired.None;
            }
            else if (stype == SlotType.Super_Rod)
            {
                if (proc < 75)
                    return true;
                return lead == LeadRequired.None; // < 100 always true
            }
            return false; // shouldn't hit here
        }

        /// <summary>
        /// Checks both Static and Magnet Pull ability type selection encounters to see if the encounter can be selected.
        /// </summary>
        /// <param name="slot">Slot Data</param>
        /// <param name="ESV">Rand16 value for the call</param>
        /// <returns>Slot number from the slot data if the slot is selected on this frame, else an invalid slot value.</returns>
        internal static int GetSlotStaticMagnet<T>(T slot, uint ESV) where T : EncounterSlot, IMagnetStatic, INumberedSlot
        {
            if (slot.StaticCount > 0 && slot.StaticIndex >= 0)
            {
                var index = ESV % slot.StaticCount;
                if (index == slot.StaticIndex)
                    return slot.SlotNumber;
            }
            if (slot.MagnetPullCount > 0 && slot.MagnetPullIndex >= 0)
            {
                var index = ESV % slot.MagnetPullCount;
                if (index == slot.MagnetPullIndex)
                    return slot.SlotNumber;
            }
            return -1;
        }
    }
}

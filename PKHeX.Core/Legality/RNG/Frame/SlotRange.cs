using System.Linq;
using static PKHeX.Core.SlotType;

namespace PKHeX.Core;

/// <summary>
/// RNG Encounter Slot Ranges to convert the [0,100) value into a slot index.
/// </summary>
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

    private const int Invalid = -1; // all slots are [0,X], unsigned. This will always result in a non-match.

    /// <summary>
    /// Gets the <see cref="INumberedSlot.SlotNumber"/> from the raw 16bit <see cref="rand"/> seed half.
    /// </summary>
    public static int GetSlot(SlotType type, uint rand, FrameType t) => t switch
    {
        FrameType.MethodH => HSlot(type, rand),
        FrameType.MethodJ => JSlot(type, rand),
        FrameType.MethodK => KSlot(type, rand),
        _ => Invalid,
    };

    /// <summary>
    /// Gets the <see cref="INumberedSlot.SlotNumber"/> from the raw 16bit <see cref="rand"/> seed half.
    /// </summary>
    private static int HSlot(SlotType type, uint rand)
    {
        var ESV = rand % 100;
        if ((type & Swarm) != 0)
            return ESV < 50 ? 0 : Invalid;

        return type switch
        {
            Old_Rod    => CalcSlot(ESV, H_OldRod),
            Good_Rod   => CalcSlot(ESV, H_GoodRod),
            Super_Rod  => CalcSlot(ESV, H_SuperRod),
            Rock_Smash => CalcSlot(ESV, H_Surf),
            Surf       => CalcSlot(ESV, H_Surf),
            _          => CalcSlot(ESV, H_Regular),
        };
    }

    /// <summary>
    /// Gets the <see cref="INumberedSlot.SlotNumber"/> from the raw 16bit <see cref="rand"/> seed half.
    /// </summary>
    private static int KSlot(SlotType type, uint rand)
    {
        var ESV = rand % 100;
        return type switch
        {
            Rock_Smash or Surf               => CalcSlot(ESV, H_Surf),
            Old_Rod or Good_Rod or Super_Rod => CalcSlot(ESV, K_SuperRod),
            BugContest                       => CalcSlot(ESV, K_BCC),
            Headbutt or (Headbutt | Special) => CalcSlot(ESV, K_Headbutt),
            _ => CalcSlot(ESV, H_Regular),
        };
    }

    /// <summary>
    /// Gets the <see cref="INumberedSlot.SlotNumber"/> from the raw 16bit <see cref="rand"/> seed half.
    /// </summary>
    private static int JSlot(SlotType type, uint rand)
    {
        uint ESV = rand / 656;
        return type switch
        {
            Old_Rod or Rock_Smash or Surf => CalcSlot(ESV, H_Surf),
            Good_Rod or Super_Rod         => CalcSlot(ESV, J_SuperRod),
            HoneyTree                     => 0,
            _                             => CalcSlot(ESV, H_Regular),
        };
    }

    private readonly record struct Range(uint Min, uint Max);

    private static Range[] GetRanges(params byte[] rates)
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
            var (min, max) = ranges[i];
            if (esv >= min && esv <= max)
                return i;
        }
        return Invalid;
    }

    public static int GetLevel(EncounterSlot slot, LeadRequired lead, uint lvlrand)
    {
        if (lead == LeadRequired.PressureHustleSpirit)
            return slot.LevelMax;
        if (slot.IsFixedLevel)
            return slot.LevelMin;
        int delta = slot.LevelMax - slot.LevelMin + 1;
        var adjust = (int)(lvlrand % delta);

        return slot.LevelMin + adjust;
    }

#pragma warning disable IDE0060, RCS1163 // Unused parameter.
    public static bool GetIsEncounterable<T>(T slot, FrameType frameType, int rand, LeadRequired lead) where T : ISlotRNGType
#pragma warning restore IDE0060, RCS1163 // Unused parameter.
    {
        var type = slot.Type;
        if (type.IsSweetScentType())
            return true;
        if (type is HoneyTree)
            return true;
        return true; // todo
        //return GetCanEncounter(slot, frameType, rand, lead);
    }

    // ReSharper disable once UnusedMember.Global
    public static bool GetCanEncounter<T>(T slot, FrameType frameType, int rand, LeadRequired lead) where T : ISlotRNGType
    {
        int proc = frameType == FrameType.MethodJ ? rand / 656 : rand % 100;
        var stype = slot.Type;
        if (stype == Rock_Smash)
            return proc < 60;
        if (frameType == FrameType.MethodH)
            return true; // fishing encounters are disjointed by the hooked message.
        return GetCanEncounterFish(lead, stype, proc);
    }

    private static bool GetCanEncounterFish(LeadRequired lead, SlotType stype, int proc) => stype switch
    {
        // Lead:None => can be suction cups
        Old_Rod => proc switch
        {
            < 25 => true,
            < 50 => lead == LeadRequired.None,
            _ => false,
        },
        Good_Rod => proc switch
        {
            < 50 => true,
            < 75 => lead == LeadRequired.None,
            _ => false,
        },
        Super_Rod => proc < 75 || lead == LeadRequired.None,

        _ => false,
    };

    /// <summary>
    /// Checks both Static and Magnet Pull ability type selection encounters to see if the encounter can be selected.
    /// </summary>
    /// <param name="slot">Slot Data</param>
    /// <param name="esv">Rand16 value for the call</param>
    /// <returns>Slot number from the slot data if the slot is selected on this frame, else an invalid slot value.</returns>
    internal static int GetSlotStaticMagnet<T>(T slot, uint esv) where T : EncounterSlot, IMagnetStatic, INumberedSlot
    {
        if (slot.IsStaticSlot())
        {
            var index = esv % slot.StaticCount;
            if (index == slot.StaticIndex)
                return slot.SlotNumber;
        }
        if (slot.IsMagnetSlot())
        {
            var index = esv % slot.MagnetPullCount;
            if (index == slot.MagnetPullIndex)
                return slot.SlotNumber;
        }
        return Invalid;
    }
}

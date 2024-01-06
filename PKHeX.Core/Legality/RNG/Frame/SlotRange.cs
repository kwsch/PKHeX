using static PKHeX.Core.SlotType;
using static PKHeX.Core.SlotNumber;

namespace PKHeX.Core;

/// <summary>
/// RNG Encounter Slot Ranges to convert the [0,100) value into a slot index.
/// </summary>
public static class SlotRange
{
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
            Old_Rod    => GetHOldRod(ESV),
            Good_Rod   => GetHGoodRod(ESV),
            Super_Rod  => GetHSuperRod(ESV),
            Rock_Smash => GetHSurf(ESV),
            Surf       => GetHSurf(ESV),
            _          => GetHRegular(ESV),
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
            Rock_Smash or Surf               => GetHSurf(ESV),
            Old_Rod or Good_Rod or Super_Rod => GetKSuperRod(ESV),
            BugContest                       => GetKBCC(ESV),
            Headbutt or (Headbutt | Special) => GetKHeadbutt(ESV),
            _                                => GetHRegular(ESV),
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
            Old_Rod or Rock_Smash or Surf => GetHSurf(ESV),
            Good_Rod or Super_Rod         => GetJSuperRod(ESV),
            HoneyTree                     => 0,
            _                             => GetHRegular(ESV),
        };
    }

    /// <summary>
    /// Calculates the level for the given slot.
    /// </summary>
    /// <typeparam name="T">Slot type</typeparam>
    /// <param name="slot">Slot reference</param>
    /// <param name="lead">Player's lead Pokémon</param>
    /// <param name="lvlrand">Random value to use for level calculation</param>
    /// <returns>Actual level of the encounter</returns>
    public static int GetLevel<T>(T slot, LeadRequired lead, uint lvlrand) where T : ILevelRange
    {
        if ((lead & LeadRequired.PressureHustleSpiritFail) == LeadRequired.PressureHustleSpirit)
            return slot.LevelMax;
        if (slot.IsFixedLevel())
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
    internal static int GetSlotStaticMagnet<T>(T slot, uint esv) where T : IMagnetStatic, INumberedSlot
    {
        if (slot.IsStaticSlot)
        {
            var index = esv % slot.StaticCount;
            if (index == slot.StaticIndex)
                return slot.SlotNumber;
        }
        if (slot.IsMagnetSlot)
        {
            var index = esv % slot.MagnetPullCount;
            if (index == slot.MagnetPullIndex)
                return slot.SlotNumber;
        }
        return Invalid;
    }
}

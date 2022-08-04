using System;
using static PKHeX.Core.ContestStatGranting;

namespace PKHeX.Core;

/// <summary>
/// Logic for checking and applying <see cref="IContestStats"/>.
/// </summary>
public static class ContestStatInfo
{
    private const int LowestFeelBlock3 = 1; // quad Nutpea
    private const int LowestFeelPoffin4 = 11; // Beauty Haircut
    private const int LowestFeelPoffin8b = 7;
    private const int HighestFeelPoffin8b = 79;

    private const byte MaxContestStat = 255;

    /// <summary>
    /// By giving out all-Nutpea blocks in 3, you can have contest stats all maxed while feel is at 214 if the random stats of the foul blocks line up perfectly.
    /// </summary>
    private const int BestSheenStat3 = 214;

    /// <summary>
    /// Using optimal poffins in BD/SP, roughly 120-140; be generous.
    /// </summary>
    /// <remarks>
    /// Liechi-Pomeg-Qualot-Occa etc. (2 each), which puts sheen at 140 and will max all stats if the cook times are 47.43 or faster
    /// Leppa-Pomeg-Qualot-Occa etc. (3 each), which puts sheen at 135 and will max all stats if the cook times are 43.32 or faster
    /// Liechi-Leppa-Pomeg-Qualot etc. (2 each), which puts sheen at 120 and will max all stats if the cook times area 40.40 or faster
    /// </remarks>
    private const int BestSheenStat8b = 120;

    public static void SetSuggestedContestStats(this PKM pk, IEncounterTemplate enc, EvolutionHistory h)
    {
        if (pk is not IContestStatsMutable s)
            return;

        var restrict = GetContestStatRestriction(pk, pk.Generation, h);
        var baseStat = GetReferenceTemplate(enc);
        if (restrict == None || pk.Species is not (int)Species.Milotic)
            baseStat.CopyContestStatsTo(s); // reset
        else
            s.SetAllContestStatsTo(MaxContestStat, restrict == NoSheen ? baseStat.CNT_Sheen : MaxContestStat);
    }

    public static void SetMaxContestStats(this PKM pk, IEncounterTemplate enc, EvolutionHistory h)
    {
        if (pk is not IContestStatsMutable s)
            return;
        var restrict = GetContestStatRestriction(pk, enc.Generation, h);
        var baseStat = GetReferenceTemplate(enc);
        if (restrict == None)
            return;
        s.SetAllContestStatsTo(MaxContestStat, restrict == NoSheen ? baseStat.CNT_Sheen : MaxContestStat);
    }

    public static ContestStatGranting GetContestStatRestriction(PKM pk, int origin, EvolutionHistory h) => origin switch
    {
        3 => pk.Format < 6    ? CorrelateSheen : Mixed,
        4 => pk.Format < 6    ? CorrelateSheen : Mixed,

        5 => pk.Format < 6           ? None : !h.HasVisitedBDSP ? NoSheen : Mixed, // ORAS Contests
        6 => !pk.AO && pk.IsUntraded ? None : !h.HasVisitedBDSP ? NoSheen : Mixed,

        _ => h.HasVisitedBDSP ? CorrelateSheen : None, // BDSP Contests
    };

    public static int CalculateMaximumSheen(IContestStats s, int nature, IContestStats initial, bool pokeBlock3)
    {
        if (s.IsAnyContestStatMax())
            return MaxContestStat;

        if (s.IsContestEqual(initial))
            return initial.CNT_Sheen;

        if (pokeBlock3)
            return CalculateMaximumSheen3(s, nature, initial);

        var avg = GetAverageFeel(s, nature, initial);
        if (avg <= 0)
            return initial.CNT_Sheen;

		if (avg <= 2)
			return 59;

        // Can get trash poffins by burning and spilling on purpose.
        return Math.Min(MaxContestStat, avg * HighestFeelPoffin8b);
    }

    public static int CalculateMinimumSheen(IContestStats s, IContestStats initial, INature pk, ContestStatGrantingSheen method) => method switch
    {
        ContestStatGrantingSheen.Gen8b => CalculateMinimumSheen8b(s, pk.Nature, initial),
        ContestStatGrantingSheen.Gen3 => CalculateMinimumSheen3(s, pk.Nature, initial),
        ContestStatGrantingSheen.Gen4 => CalculateMinimumSheen4(s, pk.Nature, initial),
        _ => throw new ArgumentOutOfRangeException(nameof(method)),
    };

    // Slightly better stat:sheen ratio than Gen4; prefer if has visited.
    public static int CalculateMinimumSheen8b(IContestStats s, int nature, IContestStats initial)
    {
        if (s.IsContestEqual(initial))
            return initial.CNT_Sheen;

        var rawAvg = GetAverageFeel(s, 0, initial);
        if (rawAvg == MaxContestStat)
            return BestSheenStat8b;

        var avg = Math.Max(1, nature % 6 == 0 ? rawAvg : GetAverageFeel(s, nature, initial));
        avg = Math.Min(rawAvg, avg); // be generous
        avg = (BestSheenStat8b * avg) / MaxContestStat;

        return Math.Min(BestSheenStat8b, Math.Max(LowestFeelPoffin8b, avg));
    }

    public static int CalculateMinimumSheen3(IContestStats s, int nature, IContestStats initial)
    {
        if (s.IsContestEqual(initial))
            return initial.CNT_Sheen;

        var rawAvg = GetAverageFeel(s, 0, initial);
        if (rawAvg == MaxContestStat)
            return MaxContestStat;

        var avg = Math.Max(1, nature % 6 == 0 ? rawAvg : GetAverageFeel(s, nature, initial));
        avg = Math.Min(rawAvg, avg); // be generous

        avg = (BestSheenStat3 * avg) / MaxContestStat;
        return Math.Min(BestSheenStat3, Math.Max(LowestFeelBlock3, avg));
    }

    public static int CalculateMinimumSheen4(IContestStats s, int nature, IContestStats initial)
    {
        if (s.IsContestEqual(initial))
            return initial.CNT_Sheen;

        var rawAvg = GetAverageFeel(s, 0, initial);
        if (rawAvg == MaxContestStat)
            return MaxContestStat;

        var avg = Math.Max(1, nature % 6 == 0 ? rawAvg : GetAverageFeel(s, nature, initial));
        avg = Math.Min(rawAvg, avg); // be generous

        return Math.Min(MaxContestStat, Math.Max(LowestFeelPoffin4, avg));
    }

    private static int CalculateMaximumSheen3(IContestStats s, int nature, IContestStats initial)
    {
        // By using Enigma and Lansat and a 25 +1/-1, can get a +9/+19s at minimum RPM
        // By using Strib, Chilan, Niniku, or Topo, can get a black +2/2/2 & 83 block (6:83) at minimum RPM.
        // https://github.com/kwsch/PKHeX/issues/3517
        var sum = GetGainedSum(s, nature, initial);
        if (sum == 0)
            return 0;

        static int Gained2(byte a, byte b) => a - b >= 2 ? 1 : 0;
        int gained = Gained2(s.CNT_Cool,   initial.CNT_Cool)
                   + Gained2(s.CNT_Beauty, initial.CNT_Beauty)
                   + Gained2(s.CNT_Cute,   initial.CNT_Cute)
                   + Gained2(s.CNT_Smart,  initial.CNT_Smart)
                   + Gained2(s.CNT_Tough,  initial.CNT_Tough);
        bool has3 = gained >= 3;

        // Prefer the bad-black-block correlation if more than 3 stats have gains >= 2.
        var permit = has3 ? (sum * 83 / 6) : (sum * 19 / 9);
        return Math.Min(MaxContestStat, Math.Max(LowestFeelBlock3, permit));
    }

    private static int GetAverageFeel(IContestStats s, int nature, IContestStats initial)
    {
        var sum = GetGainedSum(s, nature, initial);
        return (int)Math.Ceiling(sum / 5f);
    }

    private static int GetGainedSum(IContestStats s, int nature, IContestStats initial)
    {
        ReadOnlySpan<sbyte> span = NatureAmpTable.AsSpan(5 * nature, 5);
        int sum = 0;
        sum += GetAmpedStat(span, 0, s.CNT_Cool - initial.CNT_Cool);
        sum += GetAmpedStat(span, 1, s.CNT_Beauty - initial.CNT_Beauty);
        sum += GetAmpedStat(span, 2, s.CNT_Cute - initial.CNT_Cute);
        sum += GetAmpedStat(span, 3, s.CNT_Smart - initial.CNT_Smart);
        sum += GetAmpedStat(span, 4, s.CNT_Tough - initial.CNT_Tough);
        return sum;
    }

    private static int GetAmpedStat(ReadOnlySpan<sbyte> amps, int index, int gain)
    {
        var amp = amps[index];
        if (amp == 0)
            return gain;
        return gain + GetStatAdjustment(gain, amp);
    }

    private static int GetStatAdjustment(int gain, sbyte amp)
    {
        // Undo the favor factor
        var undoFactor = amp == 1 ? 11 : 9;
        var boost = Boost(gain, undoFactor);
        return amp == -1 ? boost : -boost;

        static int Boost(int stat, int factor)
        {
            var remainder = stat % factor;
            var boost = stat / factor;

            if (remainder >= 5)
                ++boost;
            return boost;
        }
    }

    private static readonly DummyContestNone DummyNone = new();

    public static IContestStats GetReferenceTemplate(IEncounterTemplate initial) => initial as IContestStats ?? DummyNone;

    private sealed class DummyContestNone : IContestStats
    {
        public byte CNT_Cool => 0;
        public byte CNT_Beauty => 0;
        public byte CNT_Cute => 0;
        public byte CNT_Smart => 0;
        public byte CNT_Tough => 0;
        public byte CNT_Sheen => 0;
    }

    private static readonly sbyte[] NatureAmpTable =
    {
        // Spicy, Dry, Sweet, Bitter, Sour
        0, 0, 0, 0, 0, // Hardy
        1, 0, 0, 0,-1, // Lonely
        1, 0,-1, 0, 0, // Brave
        1,-1, 0, 0, 0, // Adamant
        1, 0, 0,-1, 0, // Naughty
       -1, 0, 0, 0, 1, // Bold
        0, 0, 0, 0, 0, // Docile
        0, 0,-1, 0, 1, // Relaxed
        0,-1, 0, 0, 1, // Impish
        0, 0, 0,-1, 1, // Lax
       -1, 0, 1, 0, 0, // Timid
        0, 0, 1, 0,-1, // Hasty
        0, 0, 0, 0, 0, // Serious
        0,-1, 1, 0, 0, // Jolly
        0, 0, 1,-1, 0, // Naive
       -1, 1, 0, 0, 0, // Modest
        0, 1, 0, 0,-1, // Mild
        0, 1,-1, 0, 0, // Quiet
        0, 0, 0, 0, 0, // Bashful
        0, 1, 0,-1, 0, // Rash
       -1, 0, 0, 1, 0, // Calm
        0, 0, 0, 1,-1, // Gentle
        0, 0,-1, 1, 0, // Sassy
        0,-1, 0, 1, 0, // Careful
        0, 0, 0, 0, 0, // Quirky
    };
}

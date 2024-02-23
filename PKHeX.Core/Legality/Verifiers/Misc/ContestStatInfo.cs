using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using static PKHeX.Core.ContestStatGranting;

namespace PKHeX.Core;

/// <summary>
/// Logic for checking and applying <see cref="IContestStatsReadOnly"/>.
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
        if (pk is not IContestStats s)
            return;

        var restrict = GetContestStatRestriction(pk, pk.Generation, h);
        var baseStat = GetReferenceTemplate(enc);
        if (restrict == None || pk.Species is not (int)Species.Milotic)
            baseStat.CopyContestStatsTo(s); // reset
        else
            s.SetAllContestStatsTo(MaxContestStat, restrict == NoSheen ? baseStat.ContestSheen : MaxContestStat);
    }

    public static void SetMaxContestStats(this PKM pk, IEncounterTemplate enc, EvolutionHistory h)
    {
        if (pk is not IContestStats s)
            return;
        var restrict = GetContestStatRestriction(pk, enc.Generation, h);
        var baseStat = GetReferenceTemplate(enc);
        if (restrict == None)
            return;
        s.SetAllContestStatsTo(MaxContestStat, restrict == NoSheen ? baseStat.ContestSheen : MaxContestStat);
    }

    public static ContestStatGranting GetContestStatRestriction(PKM pk, byte origin, EvolutionHistory h) => origin switch
    {
        3 => pk.Format < 6    ? CorrelateSheen : Mixed,
        4 => pk.Format < 6    ? CorrelateSheen : Mixed,

        5 => pk.Format < 6                         ? None : !h.HasVisitedBDSP ? NoSheen : Mixed, // OR/AS Contests
        6 => pk is { AO: false, IsUntraded: true } ? None : !h.HasVisitedBDSP ? NoSheen : Mixed,

        _ => h.HasVisitedBDSP ? CorrelateSheen : None, // BD/SP Contests
    };

    public static int CalculateMaximumSheen(IContestStatsReadOnly s, Nature nature, IContestStatsReadOnly initial, bool pokeBlock3)
    {
        if (s.IsAnyContestStatMax())
            return MaxContestStat;

        if (s.IsContestEqual(initial))
            return initial.ContestSheen;

        if (pokeBlock3)
            return CalculateMaximumSheen3(s, nature, initial);

        var avg = GetAverageFeel(s, nature, initial);
        if (avg <= 0)
            return initial.ContestSheen;

		if (avg <= 2)
			return 59;

        // Can get trash poffins by burning and spilling on purpose.
        return Math.Min(MaxContestStat, avg * HighestFeelPoffin8b);
    }

    public static int CalculateMinimumSheen(IContestStatsReadOnly s, IContestStatsReadOnly initial, INature pk, ContestStatGrantingSheen method) => method switch
    {
        ContestStatGrantingSheen.Gen8b => CalculateMinimumSheen8b(s, pk.Nature, initial),
        ContestStatGrantingSheen.Gen3 => CalculateMinimumSheen3(s, pk.Nature, initial),
        ContestStatGrantingSheen.Gen4 => CalculateMinimumSheen4(s, pk.Nature, initial),
        _ => throw new ArgumentOutOfRangeException(nameof(method)),
    };

    // BD/SP has a slightly better stat:sheen ratio than Gen4; prefer if it has visited.
    public static int CalculateMinimumSheen8b(IContestStatsReadOnly s, Nature nature, IContestStatsReadOnly initial)
    {
        if (s.IsContestEqual(initial))
            return initial.ContestSheen;

        var rawAvg = GetAverageFeel(s, 0, initial);
        if (rawAvg == MaxContestStat)
            return BestSheenStat8b;

        var avg = Math.Max(1, (byte)nature % 6 == 0 ? rawAvg : GetAverageFeel(s, nature, initial));
        avg = Math.Min(rawAvg, avg); // be generous
        avg = (BestSheenStat8b * avg) / MaxContestStat;

        return Math.Clamp(avg, LowestFeelPoffin8b, BestSheenStat8b);
    }

    public static int CalculateMinimumSheen3(IContestStatsReadOnly s, Nature nature, IContestStatsReadOnly initial)
    {
        if (s.IsContestEqual(initial))
            return initial.ContestSheen;

        var rawAvg = GetAverageFeel(s, 0, initial);
        if (rawAvg == MaxContestStat)
            return BestSheenStat3;

        var avg = Math.Max(1, (byte)nature % 6 == 0 ? rawAvg : GetAverageFeel(s, nature, initial));
        avg = Math.Min(rawAvg, avg); // be generous

        avg = (BestSheenStat3 * avg) / MaxContestStat;
        return Math.Clamp(avg, LowestFeelBlock3, BestSheenStat3);
    }

    public static int CalculateMinimumSheen4(IContestStatsReadOnly s, Nature nature, IContestStatsReadOnly initial)
    {
        if (s.IsContestEqual(initial))
            return initial.ContestSheen;

        var rawAvg = GetAverageFeel(s, 0, initial);
        if (rawAvg == MaxContestStat)
            return MaxContestStat;

        var avg = Math.Max(1, (byte)nature % 6 == 0 ? rawAvg : GetAverageFeel(s, nature, initial));
        avg = Math.Min(rawAvg, avg); // be generous

        return Math.Clamp(avg, LowestFeelPoffin4, MaxContestStat);
    }

    private static int CalculateMaximumSheen3(IContestStatsReadOnly s, Nature nature, IContestStatsReadOnly initial)
    {
        // By using Enigma and Lansat and a 25 +1/-1, can get a +9/+19s at minimum RPM
        // By using Strib, Chilan, Niniku, or Topo, can get a black +2/2/2 & 83 block (6:83) at minimum RPM.
        // https://github.com/kwsch/PKHeX/issues/3517
        var sum = GetGainedSum(s, nature, initial);
        if (sum == 0)
            return 0;

        static int Gained2(byte a, byte b) => a - b >= 2 ? 1 : 0;
        int gained = Gained2(s.ContestCool,   initial.ContestCool)
                   + Gained2(s.ContestBeauty, initial.ContestBeauty)
                   + Gained2(s.ContestCute,   initial.ContestCute)
                   + Gained2(s.ContestSmart,  initial.ContestSmart)
                   + Gained2(s.ContestTough,  initial.ContestTough);
        bool has3 = gained >= 3;

        // Prefer the bad-black-block correlation if more than 3 stats have gains >= 2.
        var permit = has3 ? (sum * 83 / 6) : (sum * 19 / 9);
        return Math.Clamp(permit, LowestFeelBlock3, MaxContestStat);
    }

    private static int GetAverageFeel(IContestStatsReadOnly s, Nature nature, IContestStatsReadOnly initial)
    {
        var sum = GetGainedSum(s, nature, initial);
        return (int)Math.Ceiling(sum / 5f);
    }

    // Indexes into the NatureAmpTable
    private const int AmpIndexCool   = 0; // Spicy
    private const int AmpIndexTough  = 1; // Sour
    private const int AmpIndexBeauty = 2; // Dry
    private const int AmpIndexSmart  = 3; // Bitter
    private const int AmpIndexCute   = 4; // Sweet

    private static int GetGainedSum(IContestStatsReadOnly s, Nature nature, IContestStatsReadOnly initial)
    {
        var span = NatureAmp.GetAmps(nature);
        int sum = 0;
        sum += GetAmpedStat(span, AmpIndexCool, s.ContestCool, initial.ContestCool);
        sum += GetAmpedStat(span, AmpIndexTough, s.ContestTough, initial.ContestTough);
        sum += GetAmpedStat(span, AmpIndexBeauty, s.ContestBeauty, initial.ContestBeauty);
        sum += GetAmpedStat(span, AmpIndexSmart, s.ContestSmart, initial.ContestSmart);
        sum += GetAmpedStat(span, AmpIndexCute, s.ContestCute, initial.ContestCute);
        return sum;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int GetAmpedStat(ReadOnlySpan<sbyte> amps, [ConstantExpected] int index, byte current, byte initial)
    {
        var gain = current - initial;
        if (gain <= 0)
            return 0;
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

    public static IContestStatsReadOnly GetReferenceTemplate(IEncounterTemplate initial) => initial as IContestStatsReadOnly ?? DummyNone;

    private sealed class DummyContestNone : IContestStatsReadOnly
    {
        public byte ContestCool => 0;
        public byte ContestBeauty => 0;
        public byte ContestCute => 0;
        public byte ContestSmart => 0;
        public byte ContestTough => 0;
        public byte ContestSheen => 0;
    }
}

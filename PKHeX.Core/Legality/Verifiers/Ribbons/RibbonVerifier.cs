using System;
using System.Text;
using static PKHeX.Core.LegalityCheckResultCode;

namespace PKHeX.Core;

/// <summary>
/// Verifies the <see cref="PKM"/> Ribbon values.
/// </summary>
public sealed class RibbonVerifier : Verifier
{
    protected override CheckIdentifier Identifier => CheckIdentifier.Ribbon;

    /// <summary>
    /// Maximum amount of ribbons to consider when allocating a span for parsing.
    /// </summary>
    /// <remarks>
    /// Minor optimization is to stackalloc as little as possible, without too much calculation.
    /// <see cref="RibbonIndex3.MAX_COUNT"/> + <see cref="RibbonIndex4.MAX_COUNT"/> is 48, but are not present after Gen5.
    /// <see cref="PK5"/> only has 80 ribbons implemented.
    /// Instead of using the sum of all 3 enums, we can use <see cref="RibbonIndex.MAX_COUNT"/> as the true maximum count.
    /// </remarks>
    public const int MaxRibbonCount = (int)RibbonIndex.MAX_COUNT;

    public override void Verify(LegalityAnalysis data)
    {
        Span<RibbonResult> result = stackalloc RibbonResult[MaxRibbonCount];
        var count = Parse(result, data);
        if (count == 0)
            data.AddLine(GetValid(RibbonAllValid));
        else // defer hint string creation unless we request the string. We'll re-do work, but this saves hot path allocation where the string is never needed to be humanized.
            data.AddLine(GetInvalid(RibbonFInvalid_0, (ushort)count));
    }

    public static string GetMessage(LegalityAnalysis data, RibbonStrings str, LegalityCheckLocalization localize)
    {
        // Calling this method assumes that one or more ribbons are invalid or missing.
        // The work was already done but forgotten, so we need to parse again.
        Span<RibbonResult> result = stackalloc RibbonResult[MaxRibbonCount];
        int count = Parse(result, data);
        return GetMessage(result[..count], str, localize);
    }

    private static int Parse(Span<RibbonResult> result, LegalityAnalysis data)
    {
        // Flag VC (Gen1/2) ribbons using Gen7 origin rules.
        var enc = data.EncounterMatch;
        var pk = data.Entity;

        // Check Unobtainable Ribbons
        var args = new RibbonVerifierArguments(pk, enc, data.Info.EvoChainsAllGens);
        return GetRibbonResults(args, result);
    }

    /// <summary>
    /// Checks if the <see cref="index"/> is not an invalid/missing ribbon in the result parse.
    /// </summary>
    /// <param name="index">Ribbon Index to check for</param>
    /// <param name="args">Inputs to analyze</param>
    /// <returns>True if not present in the flagged result span.</returns>
    public static bool IsValidExtra(RibbonIndex index, in RibbonVerifierArguments args)
    {
        Span<RibbonResult> result = stackalloc RibbonResult[MaxRibbonCount];
        int count = GetRibbonResults(args, result);
        if (count == 0)
            return true;

        var span = result[..count];
        foreach (var x in span)
        {
            if (x.Equals(index))
                return false;
        }
        return true;
    }

    /// <summary>
    /// Uses the input <see cref="args"/> and stores results in the <see cref="result"/> span.
    /// </summary>
    /// <param name="args">Inputs to analyze</param>
    /// <param name="result">Result storage</param>
    /// <returns>Count of elements filled in the <see cref="result"/> span.</returns>
    public static int GetRibbonResults(in RibbonVerifierArguments args, Span<RibbonResult> result)
    {
        var list = new RibbonResultList(result);
        return GetRibbonResults(args, ref list);
    }

    private static int GetRibbonResults(in RibbonVerifierArguments args, ref RibbonResultList list)
    {
        if (!args.Entity.IsEgg)
            Parse(args, ref list);
        else
            ParseEgg(args, ref list);

        return list.Count;
    }

    private static string GetMessage(ReadOnlySpan<RibbonResult> result, RibbonStrings str , LegalityCheckLocalization localize)
    {
        var total = result.Length;
        int missing = GetCountMissing(result);
        int invalid = total - missing;
        var sb = new StringBuilder(total * 20);
        if (missing != 0)
            AppendAll(result, sb, str, localize.RibbonMissing_0, true);
        if (invalid != 0)
        {
            if (missing != 0) // need to visually separate the message
                sb.Append(Environment.NewLine);
            AppendAll(result, sb, str, localize.RibbonFInvalid_0, false);
        }
        return sb.ToString();
    }

    private static int GetCountMissing(ReadOnlySpan<RibbonResult> result)
    {
        int count = 0;
        foreach (var x in result)
        {
            if (x.IsMissing)
                count++;
        }
        return count;
    }

    private const string MessageSplitNextRibbon = ", ";

    private static void AppendAll(ReadOnlySpan<RibbonResult> result, StringBuilder sb, RibbonStrings str, string startText, bool stateMissing)
    {
        int added = 0;
        sb.Append(startText);
        foreach (var x in result)
        {
            if (x.IsMissing != stateMissing)
                continue;
            if (added++ != 0)
                sb.Append(MessageSplitNextRibbon);
            var localized = str.GetName(x.PropertyName);
            sb.Append(localized);
        }
    }

    private static void Parse(in RibbonVerifierArguments args, ref RibbonResultList list)
    {
        var pk = args.Entity;
        if (pk is IRibbonSetOnly3 o3)
            o3.Parse(args, ref list);
        if (pk is IRibbonSetEvent3 e3)
            e3.Parse(args, ref list);
        if (pk is IRibbonSetEvent4 e4)
            e4.Parse(args, ref list);
        if (pk is IRibbonSetUnique3 u3)
            u3.Parse(args, ref list);
        if (pk is IRibbonSetCommon3 s3)
            s3.Parse(args, ref list);
        if (pk is IRibbonSetUnique4 u4)
            u4.Parse(args, ref list);
        if (pk is IRibbonSetCommon4 s4)
            s4.Parse(args, ref list);
        if (pk is IRibbonSetCommon6 s6)
            s6.Parse(args, ref list);
        if (pk is IRibbonSetCommon7 s7)
            s7.Parse(args, ref list);
        if (pk is IRibbonSetCommon8 s8)
            s8.Parse(args, ref list);
        if (pk is IRibbonSetCommon9 s9)
            s9.Parse(args, ref list);
        if (pk is IRibbonSetMark9 m9)
            m9.Parse(args, ref list);
    }

    private static void ParseEgg(in RibbonVerifierArguments args, ref RibbonResultList list)
    {
        var pk = args.Entity;
        if (pk is IRibbonSetOnly3 o3)
            o3.ParseEgg(ref list);
        if (pk is IRibbonSetEvent3 e3)
            e3.ParseEgg(ref list);
        if (pk is IRibbonSetEvent4 e4)
            e4.ParseEgg(ref list, args); // Some event eggs can have ribbons!
        if (pk is IRibbonSetUnique3 u3)
            u3.ParseEgg(ref list);
        if (pk is IRibbonSetCommon3 s3)
            s3.ParseEgg(ref list);
        if (pk is IRibbonSetUnique4 u4)
            u4.ParseEgg(ref list);
        if (pk is IRibbonSetCommon4 s4)
            s4.ParseEgg(ref list);
        if (pk is IRibbonSetCommon6 s6)
            s6.ParseEgg(ref list);
        if (pk is IRibbonSetCommon7 s7)
            s7.ParseEgg(ref list);
        if (pk is IRibbonSetCommon8 s8)
            s8.ParseEgg(ref list);
        if (pk is IRibbonSetCommon9 s9)
            s9.ParseEgg(ref list);
        if (pk is IRibbonSetMark9 m9)
            m9.ParseEgg(ref list);
    }
}

using System;
using System.Text;
using static PKHeX.Core.LegalityCheckStrings;

namespace PKHeX.Core;

/// <summary>
/// Verifies the <see cref="PKM"/> Ribbon values.
/// </summary>
public sealed class RibbonVerifier : Verifier
{
    protected override CheckIdentifier Identifier => CheckIdentifier.Ribbon;
    public const int MaxRibbonCount = (int)RibbonIndex.MAX_COUNT + (int)RibbonIndex3.MAX_COUNT + (int)RibbonIndex4.MAX_COUNT;

    public override void Verify(LegalityAnalysis data)
    {
        // Flag VC (Gen1/2) ribbons using Gen7 origin rules.
        var enc = data.EncounterMatch;
        var pk = data.Entity;

        // Check Unobtainable Ribbons
        var args = new RibbonVerifierArguments(pk, enc, data.Info.EvoChainsAllGens);
        Span<RibbonResult> result = stackalloc RibbonResult[MaxRibbonCount];
        int count = GetRibbonResults(args, result);
        if (count == 0)
        {
            data.AddLine(GetValid(LRibbonAllValid));
            return;
        }

        var msg = GetMessage(result[..count]);
        data.AddLine(GetInvalid(msg));
    }

    public static bool IsValidExtra(RibbonIndex index, RibbonVerifierArguments args)
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

    public static int GetRibbonResults(RibbonVerifierArguments args, Span<RibbonResult> result)
    {
        var list = new RibbonResultList(result);
        return GetRibbonResults(args, ref list);
    }

    private static int GetRibbonResults(RibbonVerifierArguments args, ref RibbonResultList list)
    {
        if (!args.Entity.IsEgg)
            Parse(args, ref list);
        else
            ParseEgg(args, ref list);

        return list.Count;
    }

    private static string GetMessage(Span<RibbonResult> result)
    {
        var count = result.Length;
        var sb = new StringBuilder(count * 20);

        // Count the amount of missing Ribbons.
        int required = 0;
        foreach (var x in result)
        {
            if (x.IsRequired)
                required++;
        }

        if (required != 0)
            AppendRequired(result, sb);
        if (count - required != 0)
            AppendInvalid(sb, result, required != 0);
        return sb.ToString();
    }

    private static void AppendInvalid(StringBuilder sb, Span<RibbonResult> result, bool hasRequired)
    {
        if (hasRequired)
            sb.Append(Environment.NewLine);

        var intro = LRibbonFInvalid_0;
        int added = 0;
        sb.Append(intro);
        foreach (var x in result)
        {
            if (x.IsRequired)
                continue;
            var name = x.Name;
            var localized = RibbonStrings.GetName(name);
            if (added != 0)
                sb.Append(", ");
            sb.Append(localized);
            added++;
        }
    }

    private static void AppendRequired(Span<RibbonResult> result, StringBuilder sb)
    {
        var intro = LRibbonFMissing_0;
        int added = 0;
        sb.Append(intro);
        foreach (var x in result)
        {
            if (!x.IsRequired)
                continue;
            var name = x.Name;
            var localized = RibbonStrings.GetName(name);
            if (added != 0)
                sb.Append(", ");
            sb.Append(localized);
            added++;
        }
    }

    private static void Parse(RibbonVerifierArguments args, ref RibbonResultList list)
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
    }

    private static void ParseEgg(RibbonVerifierArguments args, ref RibbonResultList list)
    {
        var pk = args.Entity;
        if (pk is IRibbonSetOnly3 o3)
            o3.ParseEgg(ref list);
        if (pk is IRibbonSetEvent3 e3)
            e3.ParseEgg(ref list);
        if (pk is IRibbonSetEvent4 e4)
            e4.ParseEgg(args, ref list); // Some event eggs can have ribbons!
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
    }
}

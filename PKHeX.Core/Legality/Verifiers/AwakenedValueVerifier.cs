using System;

namespace PKHeX.Core;

public sealed class AwakenedValueVerifier : Verifier
{
    protected override CheckIdentifier Identifier => CheckIdentifier.AVs;

    public override void Verify(LegalityAnalysis data)
    {
        if (data.Entity is not PB7 pb7)
            return;

        int sum = pb7.EVTotal;
        if (sum != 0)
            data.AddLine(GetInvalid(LegalityCheckStrings.LEffortShouldBeZero));

        if (!pb7.AwakeningAllValid())
            data.AddLine(GetInvalid(LegalityCheckStrings.LAwakenedCap));

        Span<byte> required = stackalloc byte[6];
        AwakeningUtil.GetExpectedMinimumAVs(required, pb7);
        ReadOnlySpan<byte> current = stackalloc byte[6]
        {
            pb7.GetAV(0),
            pb7.GetAV(1),
            pb7.GetAV(2),
            pb7.GetAV(4),
            pb7.GetAV(5),
            pb7.GetAV(3), // Speed last!
        };

        for (int i = 0; i < required.Length; i++)
        {
            if (current[i] >= required[i])
                continue;

            data.AddLine(GetInvalid(string.Format(LegalityCheckStrings.LAwakenedShouldBeValue, required[i])));
            break;
        }
    }
}

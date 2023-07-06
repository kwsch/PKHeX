using System;
using static PKHeX.Core.LegalityCheckStrings;

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
            data.AddLine(GetInvalid(LEffortShouldBeZero));

        if (!pb7.AwakeningAllValid())
            data.AddLine(GetInvalid(LAwakenedCap));

        Span<byte> required = stackalloc byte[6];
        AwakeningUtil.SetExpectedMinimumAVs(required, pb7);
        Span<byte> current = stackalloc byte[6];
        AwakeningUtil.AwakeningGetVisual(pb7, current);

        // For each index of current, the value should be >= the required value.
        if (current[0] < required[0])
            data.AddLine(GetInvalid(string.Format(LAwakenedShouldBeValue, required[0], nameof(IAwakened.AV_HP))));
        if (current[1] < required[1])
            data.AddLine(GetInvalid(string.Format(LAwakenedShouldBeValue, required[1], nameof(IAwakened.AV_ATK))));
        if (current[2] < required[2])
            data.AddLine(GetInvalid(string.Format(LAwakenedShouldBeValue, required[2], nameof(IAwakened.AV_DEF))));
        if (current[3] < required[3])
            data.AddLine(GetInvalid(string.Format(LAwakenedShouldBeValue, required[3], nameof(IAwakened.AV_SPA))));
        if (current[4] < required[4])
            data.AddLine(GetInvalid(string.Format(LAwakenedShouldBeValue, required[4], nameof(IAwakened.AV_SPD))));
        if (current[5] < required[5])
            data.AddLine(GetInvalid(string.Format(LAwakenedShouldBeValue, required[5], nameof(IAwakened.AV_SPE))));
    }
}

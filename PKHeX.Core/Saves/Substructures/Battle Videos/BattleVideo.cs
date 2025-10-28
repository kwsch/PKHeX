using System;

namespace PKHeX.Core;

public static class BattleVideo
{
    public static IBattleVideo? GetVariantBattleVideo(Memory<byte> data)
    {
        ReadOnlySpan<byte> span = data.Span;

        if (BattleVideo7.IsValid(span))
            return new BattleVideo7(data);
        if (BattleVideo6.IsValid(span))
            return new BattleVideo6(data);
        if (BattleVideo5.IsValid(span))
            return new BattleVideo5(data);
        if (BattleVideo4.IsValid(span))
            return new BattleVideo4(data);
        if (BattleVideo3.IsValid(span))
            return new BattleVideo3(data);
        return null;
    }

    public static bool IsValid(ReadOnlySpan<byte> data)
    {
        if (BattleVideo7.IsValid(data))
            return true;
        if (BattleVideo6.IsValid(data))
            return true;
        if (BattleVideo5.IsValid(data))
            return true;
        if (BattleVideo4.IsValid(data))
            return true;
        if (BattleVideo3.IsValid(data))
            return true;
        return false;
    }
}

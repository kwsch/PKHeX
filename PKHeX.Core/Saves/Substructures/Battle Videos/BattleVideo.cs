using System;

namespace PKHeX.Core;

public static class BattleVideo
{
    public static IBattleVideo? GetVariantBattleVideo(byte[] data)
    {
        if (BattleVideo6.IsValid(data))
            return new BattleVideo6(data);
        if (BattleVideo7.IsValid(data))
            return new BattleVideo7(data);
        if (BattleVideo4.IsValid(data))
            return new BattleVideo4(data);
        if (BattleVideo3.IsValid(data))
            return new BattleVideo3(data);
        return null;
    }

    public static bool IsValid(ReadOnlySpan<byte> data)
    {
        if (BattleVideo6.IsValid(data))
            return true;
        if (BattleVideo7.IsValid(data))
            return true;
        if (BattleVideo4.IsValid(data))
            return true;
        if (BattleVideo3.IsValid(data))
            return true;
        return false;
    }
}

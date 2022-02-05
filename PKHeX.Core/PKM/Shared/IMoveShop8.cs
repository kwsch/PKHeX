using System;

namespace PKHeX.Core;

public interface IMoveShop8
{
    ReadOnlySpan<bool> MoveShopPermitFlags { get; }
    ReadOnlySpan<int> MoveShopPermitIndexes { get; }
    bool GetPurchasedRecordFlag(int index);
    void SetPurchasedRecordFlag(int index, bool value);
    bool GetPurchasedRecordFlagAny();
}

public interface IMoveShop8Mastery : IMoveShop8
{
    bool GetMasteredRecordFlag(int index);
    void SetMasteredRecordFlag(int index, bool value);
    bool GetMasteredRecordFlagAny();
}

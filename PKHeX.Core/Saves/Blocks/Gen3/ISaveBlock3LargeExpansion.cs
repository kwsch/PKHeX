using System;

namespace PKHeX.Core;

public interface ISaveBlock3LargeExpansion : ISaveBlock3Large
{
    WonderNews3 GetWonderNews(bool isJapanese);
    void SetWonderNews(bool isJapanese, ReadOnlySpan<byte> data);
    WonderCard3 GetWonderCard(bool isJapanese);
    void SetWonderCard(bool isJapanese, ReadOnlySpan<byte> data);
    WonderCard3Extra GetWonderCardExtra(bool isJapanese);
    void SetWonderCardExtra(bool isJapanese, ReadOnlySpan<byte> data);
}

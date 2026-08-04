using PKHeX.Application.Abstractions.GiftRecords;
using PKHeX.Core;

namespace PKHeX.Infrastructure.GiftRecords;

/// <summary>
/// Maps Switch saves to their gift record stores. Gen 8/9 games no longer keep a wondercard
/// album in the save (redemption is server-side); they keep a received-gift record log instead.
/// SWSH/PLA/SV store it as a raw SCBlock whose key is private in Core, so the block keys are
/// re-declared here; BDSP exposes a typed block (<see cref="MysteryBlock8b"/>) directly.
/// </summary>
public sealed class GiftRecordProvider : IGiftRecordProvider
{
    // Core mirrors these as private consts (SaveBlockAccessor8SWSH / 8LA / 9SV: KMysteryGift).
    internal const uint KeyMysteryGiftSwsh = 0x112D5141;
    internal const uint KeyMysteryGiftSide = 0x99E1625E; // shared by PLA and SV

    public IGiftRecordStore? GetStore(SaveFile sav) => sav switch
    {
        SAV8SWSH swsh => Create(swsh.Blocks, KeyMysteryGiftSwsh, SwshGiftRecordStore.RequiredSize,
            b => new SwshGiftRecordStore(swsh, b)),
        SAV8LA la => Create(la.Blocks, KeyMysteryGiftSide, TrimmedCardGiftRecordStore.RequiredSize,
            b => new TrimmedCardGiftRecordStore(la, b, TrimmedCardGame.PLA)),
        SAV9SV sv => Create(sv.Blocks, KeyMysteryGiftSide, TrimmedCardGiftRecordStore.RequiredSize,
            b => new TrimmedCardGiftRecordStore(sv, b, TrimmedCardGame.SV)),
        SAV8BS bs when BdspGiftRecordStore.IsSupported(bs) => new BdspGiftRecordStore(bs),
        _ => null,
    };

    private static IGiftRecordStore? Create(SCBlockAccessor blocks, uint key, int requiredSize, Func<SCBlock, IGiftRecordStore> factory)
    {
        if (!blocks.TryGetBlock(key, out var block) || block is null)
            return null;
        if (block.Data.Length < requiredSize)
            return null;
        return factory(block);
    }
}

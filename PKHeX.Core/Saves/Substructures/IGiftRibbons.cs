using System;
using static PKHeX.Core.RibbonIndex;

namespace PKHeX.Core;

public interface IGiftRibbons
{
    public const int SIZE_3 = 11;
    public const int SIZE_4 = 14;

    public const int MAX_3 = 64;
    public const int MAX_4 = byte.MaxValue;

    public static ReadOnlySpan<RibbonIndex> Index => [
        ChampionBattle,
        ChampionRegional,
        ChampionNational,
        Country, // Unused in G4
        National, // Unused in G4
        Earth, // Unused in G4
        World, // Unused in G4
        ChampionWorld, // Unused in G3
        Birthday, // Unused in G3
        Special, // Unused in G3
        Souvenir, // Unused in G3
        Wishing,
        Classic,
        Premier,
    ];

    public Span<byte> GiftRibbons { get; }

    public void GiftRibbonsImport(ReadOnlySpan<byte> trade)
    {
        var self = GiftRibbons;
        var max = self.Length == SIZE_3 ? MAX_3 : MAX_4;
        for (int i = 0; i < self.Length; i++)
        {
            // ruby doesn't sanity check against 64, but emerald does.
            // just do it for all games to ensure "legal" values only import.
            if (self[i] == 0 && trade[i] != 0 && trade[i] < max)
                self[i] = trade[i];
        }
    }

    public void GiftRibbonSet(RibbonIndex ribbon, byte value)
    {
        var self = GiftRibbons;
        var max = self.Length == SIZE_3 ? MAX_3 : MAX_4;
        var i = Index.IndexOf(ribbon);
        if (i != -1 && i < self.Length && value <= max)
            self[i] = value;
    }

    public void GiftRibbonsClear() => GiftRibbons.Clear();
}

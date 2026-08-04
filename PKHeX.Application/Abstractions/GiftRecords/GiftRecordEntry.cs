using PKHeX.Core;

namespace PKHeX.Application.Abstractions.GiftRecords;

/// <summary>Category of a received-gift record.</summary>
public enum GiftRecordKind
{
    Unknown,
    Pokemon,
    Item,
    UndergroundItem,
    BattlePoints,
    Clothing,
    Money,
    OneDaySerial,
}

/// <summary>
/// One received Mystery Gift record, parsed into display-oriented fields.
/// A snapshot: mutating the save invalidates previously read entries.
/// </summary>
public sealed class GiftRecordEntry
{
    public required int Index { get; init; }
    public bool IsEmpty { get; init; }
    public DateTime? ReceivedAt { get; init; }
    public int CardId { get; init; }
    public GiftRecordKind Kind { get; init; }

    /// <summary>National dex species for Pokémon gifts; 0 otherwise.</summary>
    public ushort Species { get; init; }
    public byte Form { get; init; }
    public bool IsEgg { get; init; }
    public string OriginalTrainerName { get; init; } = string.Empty;

    /// <summary>Item id/count pairs for item gifts.</summary>
    public IReadOnlyList<GiftRecordItem> Items { get; init; } = [];
    /// <summary>Scalar value for BP or money gifts.</summary>
    public uint Amount { get; init; }

    /// <summary>
    /// Wondercard reconstructed from the record where the format allows it (SWSH/PLA/SV).
    /// Lossy: fields the record does not retain are left blank. Null for BDSP records.
    /// </summary>
    public DataMysteryGift? Card { get; init; }
}

/// <summary>An item id/count line of an item gift record.</summary>
public readonly record struct GiftRecordItem(int Id, int Count);

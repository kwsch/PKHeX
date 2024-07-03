using System;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

/// <summary>
/// Mystery Gift Template File
/// </summary>
public abstract class MysteryGift : IEncounterable, IMoveset, ITrainerID32, IFatefulEncounterReadOnly, IEncounterMatch
{
    /// <summary>
    /// Determines whether the given length of bytes is valid for a mystery gift.
    /// </summary>
    /// <param name="len">Length, in bytes, of the data of which to determine validity.</param>
    /// <returns>A boolean indicating whether the given length is valid for a mystery gift.</returns>
    public static bool IsMysteryGift(long len) => len is
        // WC9.Size or SAME AS WA8
        WA8.Size or WB8.Size or WC8.Size or
        WC6Full.Size or WC6.Size or
        PGF.Size or PGT.Size or
        PCD.Size
    ;

    /// <summary>
    /// Converts the given data to a <see cref="MysteryGift"/>.
    /// </summary>
    /// <param name="data">Raw data of the mystery gift.</param>
    /// <param name="ext">Extension of the file from which the <paramref name="data"/> was retrieved.</param>
    /// <returns>An instance of <see cref="MysteryGift"/> representing the given data, or null if <paramref name="data"/> or <paramref name="ext"/> is invalid.</returns>
    /// <remarks>This overload differs from <see cref="GetMysteryGift(byte[])"/> by checking the <paramref name="data"/>/<paramref name="ext"/> combo for validity.  If either is invalid, a null reference is returned.</remarks>
    public static DataMysteryGift? GetMysteryGift(byte[] data, ReadOnlySpan<char> ext) => data.Length switch
    {
        PGT.Size when Equals(ext, ".pgt") => new PGT(data),
        PCD.Size when Equals(ext, ".pcd", ".wc4") => new PCD(data),
        PGF.Size when Equals(ext, ".pgf") => new PGF(data),
        WC6.Size when Equals(ext, ".wc6") => new WC6(data),
        WC7.Size when Equals(ext, ".wc7") => new WC7(data),
        WR7.Size when Equals(ext, ".wr7") => new WR7(data),
        WB7.Size when Equals(ext, ".wb7", ".wb7full") => new WB7(data),
        WC8.Size when Equals(ext, ".wc8", ".wc8full") => new WC8(data),
        WB8.Size when Equals(ext, ".wb8") => new WB8(data),
        WA8.Size when Equals(ext, ".wa8") => new WA8(data),
        WC9.Size when Equals(ext, ".wc9") => new WC9(data),

        WC5Full.Size when Equals(ext, ".wc5full") => new WC5Full(data).Gift,
        WC6Full.Size when Equals(ext, ".wc6full") => new WC6Full(data).Gift,
        WC7Full.Size when Equals(ext, ".wc7full") => new WC7Full(data).Gift,
        _ => null,
    };

    private static bool Equals(ReadOnlySpan<char> c, ReadOnlySpan<char> cmp) => c.Equals(cmp, StringComparison.OrdinalIgnoreCase);
    private static bool Equals(ReadOnlySpan<char> c, ReadOnlySpan<char> cmp1, ReadOnlySpan<char> cmp2) => Equals(c, cmp1) || Equals(c, cmp2);

    /// <summary>
    /// Converts the given data to a <see cref="MysteryGift"/>.
    /// </summary>
    /// <param name="data">Raw data of the mystery gift.</param>
    /// <returns>An instance of <see cref="MysteryGift"/> representing the given data, or null if <paramref name="data"/> is invalid.</returns>
    public static DataMysteryGift? GetMysteryGift(byte[] data) => data.Length switch
    {
        PGT.Size => new PGT(data),
        PCD.Size => new PCD(data),
        PGF.Size => new PGF(data),
        WR7.Size => new WR7(data),
        WB8.Size => new WB8(data),

        // WC8/WC5Full: WC8 0x2CF always 0, WC5Full 0x2CF contains card checksum
        WC8.Size => data[0x2CF] == 0 ? new WC8(data) : new PGF(data),

        // WA8/WC9: WA8 CardType >0 for WA8, 0 for WC9.
        WA8.Size => data[0xF] > 0 ? new WA8(data) : new WC9(data),

        // WC6/WC7: Check year
        WC6.Size => ReadUInt32LittleEndian(data.AsSpan(0x4C)) / 10000 < 2000 ? new WC7(data) : new WC6(data),
        // WC6Full/WC7Full: 0x205 has 3 * 0x46 for Gen6, now only 2.
        WC6Full.Size => data[0x205] == 0 ? new WC7Full(data).Gift : new WC6Full(data).Gift,
        _ => null,
    };

    public string Extension => GetType().Name.ToLowerInvariant();
    public string FileName => $"{CardHeader}.{Extension}";
    public abstract byte Generation { get; }
    public abstract GameVersion Version { get; }
    public abstract EntityContext Context { get; }
    public abstract bool FatefulEncounter { get; }

    public PKM ConvertToPKM(ITrainerInfo tr) => ConvertToPKM(tr, EncounterCriteria.Unrestricted);
    public abstract PKM ConvertToPKM(ITrainerInfo tr, EncounterCriteria criteria);

    public abstract bool IsMatchExact(PKM pk, EvoCriteria evo);
    protected abstract bool IsMatchDeferred(PKM pk);
    protected abstract bool IsMatchPartial(PKM pk);

    public EncounterMatchRating GetMatchRating(PKM pk)
    {
        if (IsMatchPartial(pk))
            return EncounterMatchRating.PartialMatch;
        if (IsMatchDeferred(pk))
            return EncounterMatchRating.Deferred;
        return EncounterMatchRating.Match;
    }

    /// <summary>
    /// Creates a deep copy of the <see cref="MysteryGift"/> object data.
    /// </summary>
    public abstract MysteryGift Clone();

    /// <summary>
    /// Gets a friendly name for the underlying <see cref="MysteryGift"/> type.
    /// </summary>
    public string Type => GetType().Name;

    /// <summary>
    /// Gets a friendly name for the underlying <see cref="MysteryGift"/> type for the <see cref="IEncounterable"/> interface.
    /// </summary>
    public string Name => "Event Gift";

    /// <summary>
    /// Gets a friendly name for the underlying <see cref="MysteryGift"/> type for the <see cref="IEncounterable"/> interface.
    /// </summary>
    public string LongName => $"{Name} ({Type})";

    // Properties
    public abstract ushort Species { get; set; }
    public abstract AbilityPermission Ability { get; }
    public abstract bool GiftUsed { get; set; }
    public abstract string CardTitle { get; set; }
    public abstract int CardID { get; set; }

    public abstract bool IsItem { get; set; }
    public abstract int ItemID { get; set; }

    public abstract bool IsEntity { get; set; }
    public virtual int Quantity { get => 1; set { } }
    public virtual bool Empty => false;

    public virtual string CardHeader => (CardID > 0 ? $"Card #: {CardID:0000}" : "N/A") + $" - {CardTitle.Replace('\u3000',' ').Trim()}";

    // Search Properties
    public virtual Moveset Moves { get => default; set { } }
    public virtual bool HasFixedIVs => true;
    public virtual void GetIVs(Span<int> value) { }
    public virtual bool IsShiny => false;

    public virtual Shiny Shiny
    {
        get => Shiny.Never;
        init => throw new InvalidOperationException();
    }

    public virtual bool IsEgg { get => false; set { } }
    public virtual int HeldItem { get => -1; set { } }
    public virtual int AbilityType { get => -1; set { } }
    public abstract byte Gender { get; set; }
    public abstract byte Form { get; set; }
    public abstract uint ID32 { get; set; }
    public abstract ushort TID16 { get; set; }
    public abstract ushort SID16 { get; set; }
    public abstract string OriginalTrainerName { get; set; }
    public abstract ushort Location { get; set; }

    public abstract byte Level { get; set; }
    public byte LevelMin => Level;
    public byte LevelMax => Level;
    public abstract byte Ball { get; set; }
    public abstract ushort EggLocation { get; set; }

    protected virtual bool IsMatchEggLocation(PKM pk)
    {
        var expect = IsEgg ? EggLocation : pk is PB8 ? Locations.Default8bNone : 0;
        return pk.EggLocation == expect;
    }

    public Ball FixedBall => (Ball)Ball;

    public TrainerIDFormat TrainerIDDisplayFormat => this.GetTrainerIDFormat();
    public uint TrainerTID7 { get => this.GetTrainerTID7(); set => this.SetTrainerTID7(value); }
    public uint TrainerSID7 { get => this.GetTrainerSID7(); set => this.SetTrainerSID7(value); }
    public uint DisplayTID { get => this.GetDisplayTID(); set => this.SetDisplayTID(value); }
    public uint DisplaySID { get => this.GetDisplaySID(); set => this.SetDisplaySID(value); }

    /// <summary>
    /// Checks if the <see cref="PKM"/> has the <see cref="move"/> in its current move list.
    /// </summary>
    public bool HasMove(ushort move) => Moves.Contains(move);
}

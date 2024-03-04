using System;

namespace PKHeX.Core;

/// <summary>
/// Contains slot data and metadata indicating where the <see cref="PKM"/> originated from.
/// </summary>
public sealed class SlotCache : IComparable<SlotCache>
{
    /// <summary>
    /// Information regarding how the <see cref="Entity"/> was obtained.
    /// </summary>
    public readonly ISlotInfo Source;

    /// <summary>
    /// Save File reference that obtained the <see cref="Entity"/>.
    /// </summary>
    public readonly SaveFile SAV;

    /// <summary>
    /// Data that was loaded.
    /// </summary>
    public readonly PKM Entity;

    private static readonly FakeSaveFile NoSaveFile = new();

    public SlotCache(SlotInfoFile source, PKM entity)
    {
        Source = source;
        Entity = entity;
        SAV = NoSaveFile;
    }

    public SlotCache(ISlotInfo source, PKM entity, SaveFile sav)
    {
        Source = source;
        Entity = entity;
        SAV = sav;
    }

    private string GetBoxName(int box) => SAV is IBoxDetailNameRead r ? r.GetBoxName(box) : BoxDetailNameExtensions.GetDefaultBoxName(box);

    public string Identify() => GetFileName() + Source switch
    {
        SlotInfoBox box => $"[{box.Box + 1:00}] ({GetBoxName(box.Box)})-{box.Slot + 1:00}: {Entity.FileName}",
        SlotInfoFile file => $"File: {file.Path}",
        SlotInfoMisc misc => $"{misc.Type}-{misc.Slot}: {Entity.FileName}",
        SlotInfoParty party => $"Party: {party.Slot}: {Entity.FileName}",
        _ => throw new ArgumentOutOfRangeException(nameof(Source)),
    };

    private string GetFileName()
    {
        var fn = SAV.Metadata.FileName;
        if (fn is null)
            return string.Empty;
        return $"{fn} @ ";
    }

    public bool IsDataValid() => Entity.Species != 0 && Entity.Valid;

    public int CompareTo(SlotCache? other)
    {
        if (other is null)
            return -1;
        return string.CompareOrdinal(Identify(), other.Identify());
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(this, obj))
            return true;
        return obj is SlotCache c && c.Identify() == Identify();
    }

    public override int GetHashCode() => Identify().GetHashCode();
    public static bool operator ==(SlotCache left, SlotCache right) => left.Equals(right);
    public static bool operator !=(SlotCache left, SlotCache right) => !(left == right);
    public static bool operator <(SlotCache left, SlotCache right) => left.CompareTo(right) < 0;
    public static bool operator <=(SlotCache left, SlotCache right) => left.CompareTo(right) <= 0;
    public static bool operator >(SlotCache left, SlotCache right) => left.CompareTo(right) > 0;
    public static bool operator >=(SlotCache left, SlotCache right) => left.CompareTo(right) >= 0;

    public int CompareToSpeciesForm(SlotCache other)
    {
        var s1 = Entity;
        var s2 = other.Entity;
        var c1 = s1.Species.CompareTo(s2.Species);
        if (c1 != 0)
            return c1;
        var c2 = s1.Form.CompareTo(s2.Form);
        if (c2 != 0)
            return c2;
        return CompareTo(other);
    }
}

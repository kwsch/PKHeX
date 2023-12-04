using System;

namespace PKHeX.Core;

/// <summary>
/// Tuple containing data for a <see cref="Slot"/> and the originating <see cref="View"/>
/// </summary>
/// <typeparam name="T"></typeparam>
public sealed class SlotViewInfo<T>(ISlotInfo Slot, ISlotViewer<T> View) : IEquatable<T>
    where T : class
{
    public readonly ISlotInfo Slot = Slot;
    public readonly ISlotViewer<T> View = View;

    public PKM ReadCurrent() => Slot.Read(View.SAV);
    public bool CanWriteTo() => Slot.CanWriteTo(View.SAV);
    public bool IsEmpty() => Slot.IsEmpty(View.SAV);
    public WriteBlockedMessage CanWriteTo(PKM pk) => Slot.CanWriteTo(View.SAV, pk);

    private bool Equals(SlotViewInfo<T> other)
    {
        if (other.View.SAV != View.SAV)
            return false;
        if (other.View.ViewIndex != View.ViewIndex)
            return false;
        if (other.Slot.Slot != Slot.Slot)
            return false;
        return other.Slot.GetType() == Slot.GetType();
    }

    public override bool Equals(object? obj) => ReferenceEquals(this, obj) || (obj is SlotViewInfo<T> other && Equals(other));
    public override int GetHashCode() => (Slot.GetHashCode() * 397) ^ View.GetHashCode();
    bool IEquatable<T>.Equals(T? other) => other != null && Equals(other);
}

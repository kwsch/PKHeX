using System;

namespace PKHeX.Core
{
    /// <summary>
    /// Tuple containing data for a <see cref="Slot"/> and the originating <see cref="View"/>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public sealed class SlotViewInfo<T> : IEquatable<T>
    {
        public readonly ISlotInfo Slot;
        public readonly ISlotViewer<T> View;

        public PKM ReadCurrent() => Slot.Read(View.SAV);
        public bool CanWriteTo() => Slot.CanWriteTo(View.SAV);
        public WriteBlockedMessage CanWriteTo(PKM pkm) => Slot.CanWriteTo(View.SAV, pkm);

        public SlotViewInfo(ISlotInfo slot, ISlotViewer<T> view)
        {
            Slot = slot;
            View = view;
        }

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

        public override bool Equals(object obj) => ReferenceEquals(this, obj) || (obj is SlotViewInfo<T> other && Equals(other));
        public override int GetHashCode() => (Slot.GetHashCode() * 397) ^ View.GetHashCode();
        bool IEquatable<T>.Equals(T other) => other != null && Equals(other);
    }
}
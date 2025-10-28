using System.Collections.Generic;

namespace PKHeX.Core;

/// <summary>
/// Pushes slot update notifications out to all subscribers.
/// </summary>
public sealed class SlotPublisher<T>
{
    /// <summary>
    /// All <see cref="ISlotViewer{T}"/> instances that provide a view on individual <see cref="ISlotInfo"/> content.
    /// </summary>
    private List<ISlotViewer<T>> Subscribers { get; } = [];

    public ISlotInfo? Previous { get; private set; }
    public SlotTouchType PreviousType { get; private set; } = SlotTouchType.None;
    public PKM? PreviousEntity { get; private set; }

    /// <summary>
    /// Notifies all <see cref="Subscribers"/> with the latest slot change details.
    /// </summary>
    /// <param name="slot">Last interacted slot</param>
    /// <param name="type">Last interacted slot interaction type</param>
    /// <param name="pk">Last interacted slot interaction data</param>
    public void NotifySlotChanged(ISlotInfo slot, SlotTouchType type, PKM pk)
    {
        foreach (var sub in Subscribers)
            ResetView(sub, slot, type, pk);
        Previous = slot;
        PreviousType = type;
        PreviousEntity = pk;
    }

    private void ResetView(ISlotViewer<T> sub, ISlotInfo slot, SlotTouchType type, PKM pk)
    {
        if (Previous is not null)
            sub.NotifySlotOld(Previous);

        if (slot is not SlotInfoBox b || sub.ViewIndex == b.Box)
            sub.NotifySlotChanged(slot, type, pk);
    }

    public void ResetView(ISlotViewer<T> sub)
    {
        if (Previous is null || PreviousEntity is null)
            return;
        ResetView(sub, Previous, PreviousType, PreviousEntity);
    }

    public void Subscribe(ISlotViewer<T> sub) => Subscribers.Add(sub);
    public bool Unsubscribe(ISlotViewer<T> sub) => Subscribers.Remove(sub);
}

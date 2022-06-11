namespace PKHeX.Core;

public sealed record SlotGroup(string GroupName, PKM[] Slots)
{
#if DEBUG
    public override string ToString() => $"{GroupName}: {Slots.Length} {Slots.GetType().Name}";
#endif
}

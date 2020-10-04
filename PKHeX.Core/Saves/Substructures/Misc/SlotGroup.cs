namespace PKHeX.Core
{
    public class SlotGroup
    {
        public readonly string GroupName;
        public readonly PKM[] Slots;

        #if DEBUG
        public override string ToString() => $"{GroupName}: {Slots.Length} {Slots.GetType().Name}";
        #endif

        public SlotGroup(string name, PKM[] slots)
        {
            GroupName = name;
            Slots = slots;
        }
    }
}

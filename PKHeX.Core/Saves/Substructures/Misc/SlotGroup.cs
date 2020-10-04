namespace PKHeX.Core
{
    public class SlotGroup
    {
        public readonly string GroupName;
        public readonly PKM[] Slots;

        public SlotGroup(string name, PKM[] slots)
        {
            GroupName = name;
            Slots = slots;
        }
    }
}

using PKHeX.Core;

namespace PKHeX.WinForms
{
    public class SlotChange
    {
        /// <summary> Parent of the object that initiated the slot change. </summary>
        public object Parent { get; set; }

        /// <summary> Original Data of the slot. </summary>
        public byte[] OriginalData { get; set; }

        public int Offset { get; set; } = -1;
        public int Slot { get; set; } = -1;
        public int Box { get; set; } = -1;
        public PKM PKM { get; set; }

        public bool IsParty => 30 <= Slot && Slot < 36;
        public bool IsValid => Slot > -1 && (Box > -1 || IsParty);
    }
}

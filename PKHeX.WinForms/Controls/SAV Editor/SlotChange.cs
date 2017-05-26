using PKHeX.Core;

namespace PKHeX.WinForms
{
    public class SlotChange
    {
        public object Parent;

        public byte[] OriginalData;
        public int Offset = -1;
        public int Slot = -1;
        public int Box = -1;
        public PKM PKM;

        public bool IsParty => 30 <= Slot && Slot < 36;
        public bool IsValid => Slot > -1 && (Box > -1 || IsParty);
    }
}

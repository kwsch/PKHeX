namespace PKHeX.Core
{
    public enum SlotTouchType
    {
        None,
        Get,
        Set,
        Delete,
        Swap,
    }

    public static class SlotTouchTypeUtil
    {
        public static bool IsContentChange(this SlotTouchType t) => t > SlotTouchType.Get;
    }
}
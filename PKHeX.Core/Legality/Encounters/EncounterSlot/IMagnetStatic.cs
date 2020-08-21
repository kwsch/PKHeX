namespace PKHeX.Core
{
    public interface IMagnetStatic
    {
        int StaticIndex { get; set; }
        int MagnetPullIndex { get; set; }

        int StaticCount { get; set; }
        int MagnetPullCount { get; set; }
    }

    public static class MagnetStaticExtensions
    {
        public static bool IsMatchStatic(this IMagnetStatic slot,  int index, int count) => index == slot.StaticIndex && count == slot.StaticCount;
        public static bool IsMatchMagnet(this IMagnetStatic slot,  int index, int count) => index == slot.MagnetPullIndex && count == slot.MagnetPullCount;
    }
}

using System.Runtime.InteropServices;

namespace PKHeX.Core
{
    [StructLayout(LayoutKind.Sequential)]
    public struct DecorationInventory3
    {
        public const int SIZE = 150;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 10)]
        public readonly Decoration3[] Desk;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 10)]
        public readonly Decoration3[] Chair;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 10)]
        public readonly Decoration3[] Plant;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 30)]
        public readonly Decoration3[] Ornament;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 30)]
        public readonly Decoration3[] Mat;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 10)]
        public readonly Decoration3[] Poster;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 40)]
        public readonly Decoration3[] Doll;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 10)]
        public readonly Decoration3[] Cushion;
    }
}

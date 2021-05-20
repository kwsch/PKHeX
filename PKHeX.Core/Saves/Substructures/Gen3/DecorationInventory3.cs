using System.Runtime.InteropServices;

#pragma warning disable CA1815 // Override equals and operator equals on value types
namespace PKHeX.Core
{
    [StructLayout(LayoutKind.Sequential)]
    public readonly struct DecorationInventory3
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

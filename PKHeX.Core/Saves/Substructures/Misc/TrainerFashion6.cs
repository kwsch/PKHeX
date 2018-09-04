using System;

namespace PKHeX.Core
{
    public abstract class TrainerFashion6
    {
        protected uint data0;
        protected uint data1;
        protected uint data2;

        protected TrainerFashion6(byte[] data, int offset)
        {
            data0 = BitConverter.ToUInt32(data, 0 + offset);
            data1 = BitConverter.ToUInt32(data, 4 + offset);
            data2 = BitConverter.ToUInt32(data, 8 + offset);
        }

        public static TrainerFashion6 GetFashion(byte[] data, int offset, int gender)
        {
            if (gender == 0) // m
                return new Fashion6Male(data, offset);
            return new Fashion6Female(data, offset);
        }

        public void Write(byte[] data, int offset)
        {
            BitConverter.GetBytes(data0).CopyTo(data, 0 + offset);
            BitConverter.GetBytes(data1).CopyTo(data, 4 + offset);
            BitConverter.GetBytes(data2).CopyTo(data, 8 + offset);
        }

        protected static uint GetBits(uint value, int startPos, int bits)
        {
            uint mask = ((1u << bits) - 1) << startPos;
            return (value & mask) >> startPos;
        }

        protected static uint SetBits(uint value, int startPos, int bits, uint bitValue)
        {
            uint mask = ((1u << bits) - 1) << startPos;
            bitValue &= mask >> startPos;
            return (value & ~mask) | (bitValue << startPos);
        }
    }

    public class Fashion6Male : TrainerFashion6
    {
        public Fashion6Male(byte[] data, int offset)
            : base(data, offset) { }

        public uint Version  { get => GetBits(data0,  0, 3); set => data0 = SetBits(data0,  0, 3, value); }
        public uint Model    { get => GetBits(data0,  3, 3); set => data0 = SetBits(data0,  3, 3, value); }
        public uint Skin     { get => GetBits(data0,  6, 2); set => data0 = SetBits(data0,  6, 2, value); }
        public uint HairColor{ get => GetBits(data0,  8, 3); set => data0 = SetBits(data0,  8, 3, value); }
        public uint Hat      { get => GetBits(data0, 11, 5); set => data0 = SetBits(data0, 11, 5, value); }
        public uint Front    { get => GetBits(data0, 16, 3); set => data0 = SetBits(data0, 16, 3, value); }
        public uint Hair     { get => GetBits(data0, 19, 4); set => data0 = SetBits(data0, 19, 4, value); }
        public uint Face     { get => GetBits(data0, 23, 3); set => data0 = SetBits(data0, 23, 3, value); }
        public uint Arms     { get => GetBits(data0, 26, 2); set => data0 = SetBits(data0, 26, 2, value); }
        public uint _0       { get => GetBits(data0, 28, 2); set => data0 = SetBits(data0, 28, 2, value); }
        public uint Unused0  { get => GetBits(data0, 30, 2); set => data0 = SetBits(data0, 30, 2, value); }

        public uint Top      { get => GetBits(data1,  0, 6); set => data1 = SetBits(data1,  0, 6, value); }
        public uint Legs     { get => GetBits(data1,  6, 5); set => data1 = SetBits(data1,  6, 5, value); }
        public uint Socks    { get => GetBits(data1, 11, 3); set => data1 = SetBits(data1, 11, 3, value); }
        public uint Shoes    { get => GetBits(data1, 14, 5); set => data1 = SetBits(data1, 14, 5, value); }
        public uint Bag      { get => GetBits(data1, 19, 4); set => data1 = SetBits(data1, 19, 4, value); }
        public uint AHat     { get => GetBits(data1, 23, 4); set => data1 = SetBits(data1, 23, 4, value); }
        public uint _1       { get => GetBits(data1, 27, 2); set => data1 = SetBits(data1, 27, 2, value); }
        public uint Unused1  { get => GetBits(data1, 29, 3); set => data1 = SetBits(data1, 29, 3, value); }

        public bool Contacts      { get => GetBits(data2,  0, 1) == 1; set => data2 = SetBits(data2,  0, 1, value ? 1u : 0); }
        public uint FacialHair    { get => GetBits(data2,  1, 3);      set => data2 = SetBits(data2,  1, 3, value); }
        public uint ColorContacts { get => GetBits(data2,  4, 3);      set => data2 = SetBits(data2,  4, 3, value); }
        public uint FacialColor   { get => GetBits(data2,  7, 3);      set => data2 = SetBits(data2,  7, 3, value); }
        public uint PaintLeft     { get => GetBits(data2, 10, 4);      set => data2 = SetBits(data2, 10, 4, value); }
        public uint PaintRight    { get => GetBits(data2, 14, 4);      set => data2 = SetBits(data2, 14, 4, value); }
        public uint PaintLeftC    { get => GetBits(data2, 18, 3);      set => data2 = SetBits(data2, 18, 3, value); }
        public uint PaintRightC   { get => GetBits(data2, 21, 3);      set => data2 = SetBits(data2, 21, 3, value); }
        public uint Cheek         { get => GetBits(data2, 24, 2);      set => data2 = SetBits(data2, 24, 2, value); }
        public uint CheekColor    { get => GetBits(data2, 26, 3);      set => data2 = SetBits(data2, 26, 3, value); }
        public uint Unused2       { get => GetBits(data2, 29, 3);      set => data2 = SetBits(data2, 29, 3, value); }
    }

    public class Fashion6Female : TrainerFashion6
    {
        public Fashion6Female(byte[] data, int offset)
            : base(data, offset) { }

        public uint Version  { get => GetBits(data0,  0, 3); set => data0 = SetBits(data0,  0, 3, value); }
        public uint Model    { get => GetBits(data0,  3, 3); set => data0 = SetBits(data0,  3, 3, value); }
        public uint Skin     { get => GetBits(data0,  6, 2); set => data0 = SetBits(data0,  6, 2, value); }
        public uint HairColor{ get => GetBits(data0,  8, 3); set => data0 = SetBits(data0,  8, 3, value); }
        public uint Hat      { get => GetBits(data0, 11, 6); set => data0 = SetBits(data0, 11, 6, value); }
        public uint Front    { get => GetBits(data0, 17, 3); set => data0 = SetBits(data0, 17, 3, value); }
        public uint Hair     { get => GetBits(data0, 20, 4); set => data0 = SetBits(data0, 20, 4, value); }
        public uint Face     { get => GetBits(data0, 24, 3); set => data0 = SetBits(data0, 24, 3, value); }
        public uint Arms     { get => GetBits(data0, 27, 2); set => data0 = SetBits(data0, 27, 2, value); }
        public uint _0       { get => GetBits(data0, 29, 2); set => data0 = SetBits(data0, 29, 2, value); }
        public uint Unused0  { get => GetBits(data0, 31, 1); set => data0 = SetBits(data0, 31, 1, value); }

        public uint Top      { get => GetBits(data1,  0, 6); set => data1 = SetBits(data1,  0, 6, value); }
        public uint Legs     { get => GetBits(data1,  6, 7); set => data1 = SetBits(data1,  6, 7, value); }
        public uint OnePiece { get => GetBits(data1, 13, 4); set => data1 = SetBits(data1, 13, 4, value); }
        public uint Socks    { get => GetBits(data1, 17, 5); set => data1 = SetBits(data1, 17, 5, value); }
        public uint Shoes    { get => GetBits(data1, 22, 6); set => data1 = SetBits(data1, 22, 6, value); }
        public uint _1       { get => GetBits(data1, 28, 2); set => data1 = SetBits(data1, 28, 2, value); }
        public uint Unused1  { get => GetBits(data1, 30, 2); set => data1 = SetBits(data1, 30, 2, value); }

        public uint Bag           { get => GetBits(data2,  0, 5);      set => data2 = SetBits(data2,  0, 5, value); }
        public uint AHat          { get => GetBits(data2,  5, 5);      set => data2 = SetBits(data2,  5, 5, value); }
        public bool Contacts      { get => GetBits(data2, 10, 1) == 1; set => data2 = SetBits(data2, 10, 1, value ? 1u : 0); }
        public uint Mascara       { get => GetBits(data2, 11, 2);      set => data2 = SetBits(data2, 11, 2, value); }
        public uint EyeShadow     { get => GetBits(data2, 13, 2);      set => data2 = SetBits(data2, 13, 2, value); }
        public uint Cheek         { get => GetBits(data2, 15, 2);      set => data2 = SetBits(data2, 15, 2, value); }
        public uint Lips          { get => GetBits(data2, 17, 2);      set => data2 = SetBits(data2, 17, 2, value); }
        public uint ColorContacts { get => GetBits(data2, 19, 3);      set => data2 = SetBits(data2, 19, 3, value); }
        public uint ColorMascara  { get => GetBits(data2, 22, 3);      set => data2 = SetBits(data2, 22, 3, value); }
        public uint ColorEyeshadow{ get => GetBits(data2, 25, 3);      set => data2 = SetBits(data2, 25, 3, value); }
        public uint ColorCheek    { get => GetBits(data2, 28, 3);      set => data2 = SetBits(data2, 28, 3, value); }
        public uint Unused2       { get => GetBits(data2, 31, 1);      set => data2 = SetBits(data2, 31, 1, value); }
    }
}

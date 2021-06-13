using System;

namespace PKHeX.Core
{
    public sealed class SecretBase6Block : SaveBlock
    {
        public SecretBase6Block(SAV6AO sav, int offset) : base(sav) => Offset = offset;

        // structure: 0x7AD0 bytes total
        // [0000-031F] SecretBaseGoodStock[200] (800 bytes)
        // [0320-0321] u16 SecretBaseSelfLocation (-1 if not created)
        // [0322-0323] u16 alignment padding
        // [0324-0633] SecretBase6Self (0x310 bytes)
        // [0634-0637]   alignment for next field
        // [0638-7A77] SecretBase6Other[30] (0x7440 bytes, 0x3E0 each)
        //             each SecretBase6Other is a SecretBaseSelf followed by extra fields
        // [7A78-7AC7] u128[5] keys?
        // [7AC8..   ] u8 SecretBaseHasFlag

        public const int Count_Goods = 200;
        public const int Count_Goods_Used = 173;

        public SecretBase6GoodStock GetGood(int index) => new(Data, Offset + GetGoodOffset(index));
        public void SetGood(SecretBase6GoodStock good, int index) => good.Write(Data, Offset + GetGoodOffset(index));

        private static int GetGoodOffset(int index)
        {
            if ((uint) index >= Count_Goods)
                throw new ArgumentOutOfRangeException(nameof(index));
            return SecretBase6GoodStock.SIZE * index;
        }

        public void GiveAllGoods()
        {
            var bytes = BitConverter.GetBytes(25u | (1 << 16)); // count: 25, new flag.
            for (int i = 0; i < Count_Goods_Used; i++)
                bytes.CopyTo(Data, GetGoodOffset(i));
        }

        public ushort SecretBaseSelfLocation
        {
            get => BitConverter.ToUInt16(Data, Offset + 800);
            set => BitConverter.GetBytes(value).CopyTo(Data, Offset + 800);
        }

        public const int OtherSecretBaseCount = 30;
        private const int OtherSecretStart = 0x638;
        public SecretBase6 GetSecretBaseSelf() => new(Data, Offset + 0x324);
        public SecretBase6Other GetSecretBaseOther(int index) => new(Data, Offset + OtherSecretStart + GetSecretBaseOtherOffset(index));

        private static int GetSecretBaseOtherOffset(int index)
        {
            if ((uint) index >= OtherSecretBaseCount)
                throw new ArgumentOutOfRangeException(nameof(index));
            return SecretBase6Other.SIZE * index;
        }

        public bool SecretBaseHasFlag
        {
            get => Data[Offset + 0x7AC8] == 1;
            set => Data[Offset + 0x7AC8] = (byte) (value ? 1 : 0);
        }

        public void DeleteOther(int index)
        {
            int baseOffset = Offset + OtherSecretStart;
            const int maxBaseIndex = OtherSecretBaseCount - 1;
            const int size = SecretBase6Other.SIZE;
            int offset = baseOffset + GetSecretBaseOtherOffset(index);
            var arr = SAV.Data;
            if ((uint)index < OtherSecretBaseCount)
            {
                int shiftDownCount = maxBaseIndex - index;
                int shiftDownLength = size * shiftDownCount;
                Array.Copy(arr, offset + size, arr, offset, shiftDownLength);
            }

            // Ensure Last Entry is Cleared
            int lastBaseOffset = baseOffset + (size * maxBaseIndex);
            arr.AsSpan(lastBaseOffset, size).Fill(0);
        }
    }
}

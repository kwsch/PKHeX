using System;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

public sealed class SecretBase6Block(SAV6AO sav, Memory<byte> raw) : SaveBlock<SAV6AO>(sav, raw)
{
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

    public SecretBase6GoodStock GetGood(int index) => new(Raw.Slice(GetGoodOffset(index), SecretBase6GoodStock.SIZE));

    private static int GetGoodOffset(int index)
    {
        if ((uint) index >= Count_Goods)
            throw new ArgumentOutOfRangeException(nameof(index));
        return SecretBase6GoodStock.SIZE * index;
    }

    public void GiveAllGoods()
    {
        const uint value = 25u | (1 << 16); // count: 25, new flag.
        for (int i = 0; i < Count_Goods_Used; i++)
            WriteUInt32LittleEndian(Data[GetGoodOffset(i)..], value);
    }

    public ushort SecretBaseSelfLocation
    {
        get => ReadUInt16LittleEndian(Data[800..]);
        set => WriteUInt16LittleEndian(Data[800..], value);
    }

    public const int OtherSecretBaseCount = 30;
    private const int OtherSecretStart = 0x638;
    public SecretBase6 GetSecretBaseSelf() => new(Raw.Slice(0x324, SecretBase6.SIZE));
    public SecretBase6Other GetSecretBaseOther(int index) => new(Raw.Slice(OtherSecretStart + GetSecretBaseOtherOffset(index), SecretBase6Other.SIZE));

    private static int GetSecretBaseOtherOffset(int index)
    {
        if ((uint) index >= OtherSecretBaseCount)
            throw new ArgumentOutOfRangeException(nameof(index));
        return SecretBase6Other.SIZE * index;
    }

    public bool SecretBaseHasFlag
    {
        get => Data[0x7AC8] == 1;
        set => Data[0x7AC8] = (byte) (value ? 1 : 0);
    }

    public void DeleteOther(int index)
    {
        const int maxBaseIndex = OtherSecretBaseCount - 1;
        const int size = SecretBase6Other.SIZE;
        int offset = OtherSecretStart + GetSecretBaseOtherOffset(index);
        var arr = Data;
        if ((uint)index < OtherSecretBaseCount)
        {
            int shiftDownCount = maxBaseIndex - index;
            int shiftDownLength = size * shiftDownCount;

            var src = arr.Slice(offset + size, shiftDownLength);
            var dst = arr.Slice(offset, shiftDownLength);
            src.CopyTo(dst);
        }

        // Ensure Last Entry is Cleared
        const int lastBaseOffset = OtherSecretStart + (size * maxBaseIndex);
        arr.Slice(lastBaseOffset, size).Clear();
    }
}

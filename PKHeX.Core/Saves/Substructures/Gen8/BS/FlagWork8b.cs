using System;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

/// <summary>
/// Structure that manages the 3 event variable storage arrays.
/// </summary>
/// <remarks>Comprised of 3 fixed-sized arrays, we do our read/write in-place. Each element in an array is 4 bytes. Total size: 0x55F0 </remarks>
public sealed class FlagWork8b(SAV8BS sav, Memory<byte> raw) : SaveBlock<SAV8BS>(sav, raw),
    IEventFlag, ISystemFlag, IEventWork<int>
{
    public const int COUNT_WORK = 500;
    public const int COUNT_FLAG = 4000;
    public const int COUNT_SYSTEM = 1000;

    public const int OFS_WORK = 0;
    public const int OFS_FLAG = OFS_WORK + (COUNT_WORK * 4);
    public const int OFS_SYSTEM = OFS_FLAG + (COUNT_FLAG * 4);
    public const int SIZE = OFS_SYSTEM + (COUNT_SYSTEM * 4); // 0x55F0

    public const int FH_START = 0;
    public const int FH_END = 63;
    public const int FE_FLAG_START = 63;
    public const int FE_END = 314;
    public const int FV_FLAG_START = 314;
    public const int FV_END = 603;
    public const int FT_START = 603;
    public const int FT_END = 859;
    public const int FV_FLD_START = 859;
    public const int FV_FLD_END = 1115;
    public const int TMFLG_START = 1115;

    private const int BASE_VANISH = FV_FLAG_START;
    private const int END_VANISH = FV_END - 1;
    private const int COUNT_VANISH = END_VANISH - BASE_VANISH; // 0x120

    public int CountFlag => COUNT_FLAG;
    public int CountSystem => COUNT_SYSTEM;
    public int CountWork => COUNT_WORK;

    private static int GetOffsetFlag(int index)
    {
        if ((uint)index >= COUNT_FLAG)
            throw new ArgumentOutOfRangeException(nameof(index), $"Expected a number below {COUNT_FLAG}, not {index}.");
        return OFS_FLAG + (4 * index);
    }
    private static int GetOffsetSystem(int index)
    {
        if ((uint)index >= COUNT_SYSTEM)
            throw new ArgumentOutOfRangeException(nameof(index), $"Expected a number below {COUNT_SYSTEM}, not {index}.");
        return OFS_SYSTEM + (4 * index);
    }

    private static int GetOffsetWork(int index)
    {
        if ((uint)index >= COUNT_WORK)
            throw new ArgumentOutOfRangeException(nameof(index), $"Expected a number below {COUNT_WORK}, not {index}.");
        return OFS_WORK + (4 * index);
    }

    public bool GetFlag      (int index) => ReadInt32LittleEndian(Data[GetOffsetFlag(index)..]) == 1;
    public bool GetSystemFlag(int index) => ReadInt32LittleEndian(Data[GetOffsetSystem(index)..]) == 1;
    public int GetWork       (int index) => ReadInt32LittleEndian(Data[GetOffsetWork(index)..]);
    public float GetFloatWork(int index) => ReadSingleLittleEndian(Data[GetOffsetWork(index)..]);

    public void SetFlag      (int index,  bool value) => WriteUInt32LittleEndian(Data[GetOffsetFlag(index)..], value ? 1u : 0u);
    public void SetSystemFlag(int index,  bool value) => WriteUInt32LittleEndian(Data[GetOffsetSystem(index)..], value ? 1u : 0u);
    public void SetWork      (int index,   int value) => WriteInt32LittleEndian(Data[GetOffsetWork(index)..], value);
    public void SetFloatWork (int index, float value) => WriteSingleLittleEndian(Data[GetOffsetWork(index)..], value);

    public void ResetFlag      (int index) => SetFlag(index, false);
    public void ResetVanishFlag(int index) => SetVanishFlag(index, false);
    public void ResetSystemFlag(int index) => SetSystemFlag(index, false);
    public void ResetWork      (int index) => SetWork(index, 0);

    public bool GetVanishFlag(int index)
    {
        if ((uint)index >= COUNT_VANISH)
            throw new ArgumentOutOfRangeException(nameof(index), $"Expected a number below {COUNT_VANISH}, not {index}.");
        return GetFlag(BASE_VANISH + index);
    }

    public void SetVanishFlag(int index, bool value)
    {
        if ((uint)index >= COUNT_VANISH)
            throw new ArgumentOutOfRangeException(nameof(index), $"Expected a number below {COUNT_VANISH}, not {index}.");
        SetFlag(BASE_VANISH + index, value);
    }

    public int BadgeCount()
    {
        // system flags 124-131
        int ctr = 0;
        for (int i = 0; i < 8; i++)
            ctr += GetSystemFlag(124 + i) ? 1 : 0;
        return ctr;
    }
}

using System;
using System.Buffers.Binary;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace PKHeX.Core;

/// <summary>
/// Base class for GB Era Stadium files.
/// </summary>
public abstract class SAV_STADIUM : SaveFile, ILangDeviantSave
{
    protected internal sealed override string ShortSummary => $"{OT} ({Version}) {Checksums.CRC16_CCITT(Data):X4}";
    public sealed override string Extension => ".sav";

    public abstract int SaveRevision { get; }
    public abstract string SaveRevisionString { get; }
    public bool Japanese { get; }
    public bool Korean => false;

    public sealed override int MaxBallID => 0; // unused
    public sealed override GameVersion MaxGameID => GameVersion.Gen1; // unused
    public sealed override int MaxMoney => 999999;
    public sealed override int MaxCoins => 9999;

    /// <summary> If the original input data was swapped endianness. </summary>
    private readonly bool IsPairSwapped;

    protected abstract int TeamCount { get; }
    public sealed override string OT { get; set; }
    public sealed override int Language => Japanese ? 1 : 2;

    protected SAV_STADIUM(byte[] data, bool japanese, bool swap) : base(data)
    {
        Japanese = japanese;
        OT = SaveUtil.GetSafeTrainerName(this, (LanguageID)Language);

        if (!swap)
            return;
        ReverseEndianness(Data);
        IsPairSwapped = true;
    }

    protected SAV_STADIUM(bool japanese, [ConstantExpected] int size) : base(size)
    {
        Japanese = japanese;
        OT = SaveUtil.GetSafeTrainerName(this, (LanguageID)Language);
    }

    protected sealed override byte[] DecryptPKM(byte[] data) => data;
    public sealed override int GetPartyOffset(int slot) => -1;
    public sealed override bool ChecksumsValid => GetBoxChecksumsValid();
    public sealed override string ChecksumInfo => ChecksumsValid ? "Checksum valid." : "Checksum invalid";
    protected abstract void SetBoxChecksum(int box);
    protected abstract bool GetIsBoxChecksumValid(int box);
    protected sealed override void SetChecksums() => SetBoxChecksums();
    protected abstract void SetBoxMetadata(int box);

    protected void SetBoxChecksums()
    {
        for (int i = 0; i < BoxCount; i++)
        {
            SetBoxMetadata(i);
            SetBoxChecksum(i);
        }
    }

    private bool GetBoxChecksumsValid()
    {
        for (int i = 0; i < BoxCount; i++)
        {
            if (!GetIsBoxChecksumValid(i))
                return false;
        }
        return true;
    }

    protected sealed override byte[] GetFinalData()
    {
        var result = base.GetFinalData();
        if (IsPairSwapped)
            ReverseEndianness(result = [..result]);
        return result;
    }

    public abstract SlotGroup GetTeam(int team);

    public virtual SlotGroup[] GetRegisteredTeams()
    {
        var result = new SlotGroup[TeamCount];
        for (int i = 0; i < result.Length; i++)
            result[i] = GetTeam(i);
        return result;
    }

    public sealed override string GetString(ReadOnlySpan<byte> data)
        => StringConverter1.GetString(data, Japanese);
    public sealed override int LoadString(ReadOnlySpan<byte> data, Span<char> destBuffer)
        => StringConverter1.LoadString(data, destBuffer, Japanese);
    public sealed override int SetString(Span<byte> destBuffer, ReadOnlySpan<char> value, int maxLength, StringConverterOption option)
        => StringConverter1.SetString(destBuffer, value, maxLength, Japanese, option);

    /// <summary>
    /// Some emulators emit with system architecture endianness (Little Endian) instead of the original Big Endian ordering.
    /// This will efficiently swap 32-bit endianness for the entire span.
    /// </summary>
    /// <param name="data">Full savedata</param>
    private static void ReverseEndianness(Span<byte> data)
    {
        var uintArr = MemoryMarshal.Cast<byte, uint>(data);
        for (int i = 0; i < uintArr.Length; i++)
            uintArr[i] = BinaryPrimitives.ReverseEndianness(uintArr[i]);
    }
}

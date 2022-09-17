using System;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

public sealed class RanchTrainerMii
{
    public const int SIZE = 44;
    public readonly byte[] Data;

    public RanchTrainerMii(byte[] trainerMiiData)
    {
        this.Data = trainerMiiData;
        MiiIdOffset = 0x00;
        MiiIdSize = 0x04;
        SystemIdOffset = 0x04;
        SystemIdSize = 0x04;
        TrainerIdOffset = 0x0C;
        TrainerIdSize = 0x02;
        SecretIdOffset = 0x0E;
        SecretIdSize = 0x02;
        TrainerNameOffset = 0x10;
        TrainerNameSize = 0x0A;
    }

    public uint GetMiiId()
    {
        return ReadUInt32BigEndian(Data.AsSpan(MiiIdOffset, MiiIdSize));
    }

    public void SetMiiId(uint miiId)
    {
        WriteUInt32BigEndian(Data.AsSpan(MiiIdOffset, MiiIdSize), miiId);
    }

    public uint GetSystemId()
    {
        return ReadUInt32BigEndian(Data.AsSpan(SystemIdOffset, SystemIdSize));
    }

    public void SetSystemId(uint systemId)
    {
        WriteUInt32BigEndian(Data.AsSpan(SystemIdOffset, SystemIdSize), systemId);
    }

    public ushort GetTrainerId()
    {
        return ReadUInt16LittleEndian(Data.AsSpan(TrainerIdOffset, TrainerIdSize));
    }

    public void SetTrainerId(ushort trainerId)
    {
        WriteUInt16LittleEndian(Data.AsSpan(TrainerIdOffset, TrainerIdSize), trainerId);
    }

    public ushort GetSecretId()
    {
        return ReadUInt16LittleEndian(Data.AsSpan(SecretIdOffset, SecretIdSize));
    }

    public void SetSecretId(ushort secretId)
    {
        WriteUInt16LittleEndian(Data.AsSpan(SecretIdOffset, SecretIdSize), secretId);
    }

    public string GetTrainerName()
    {
        return StringConverter4.GetString(Data.Slice(TrainerNameOffset, TrainerNameSize));
    }

    public void SetTrainerName(string trainerName)
    {
        Span<byte> destBuffer = new Span<byte>(new byte[TrainerNameSize]);
        StringConverter4.SetString(destBuffer, trainerName.AsSpan(), trainerName.Length, StringConverterOption.ClearFF);
    }

    private int MiiIdOffset;
    private int MiiIdSize;
    private int SystemIdOffset;
    private int SystemIdSize;
    private int TrainerNameOffset;
    private int TrainerNameSize;
    private int TrainerIdOffset;
    private int TrainerIdSize;
    private int SecretIdOffset;
    private int SecretIdSize;
}

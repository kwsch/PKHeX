using System;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

public sealed class RanchToy
{
    public const int SIZE = 8;
    public readonly byte[] Data;

    public RanchToy(byte[] ranchToyData)
    {
        this.Data = ranchToyData;
        ToyTypeOffset = 0x00;
        ToyTypeSize = 4;
        ToyMetadataOffset = 0x04;
        ToyMetadataSize = 4;
    }

    public RanchToyType GetToyType()
    {
        uint toyTypeId = ReadUInt32BigEndian(Data.AsSpan(ToyTypeOffset, ToyTypeSize));
        RanchToyType toyType = (RanchToyType)toyTypeId;
        return toyType;
    }

    public void SetToyType(RanchToyType toyType)
    {
        WriteUInt32BigEndian(Data.AsSpan(ToyTypeOffset, ToyTypeSize), ((uint)toyType));
    }

    public uint GetToyMetadata()
    {
        return ReadUInt32BigEndian(Data.AsSpan(ToyMetadataOffset, ToyMetadataSize));
    }

    public void SetToyMetadata(uint toyMetadata)
    {
        WriteUInt32BigEndian(Data.AsSpan(ToyMetadataOffset, ToyMetadataSize), toyMetadata);
    }

    private int ToyTypeOffset;
    private int ToyTypeSize;
    private int ToyMetadataOffset;
    private int ToyMetadataSize;
}

using System;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

/// <summary>
/// Block type for a <see cref="SCBlock"/>.
/// </summary>
public enum SCTypeCode : byte
{
    None = 0,

    Bool1 = 1, // False?
    Bool2 = 2, // True?
    Bool3 = 3, // Either? (Array boolean type)

    Object = 4,

    Array = 5,

    Byte = 8,
    UInt16 = 9,
    UInt32 = 10,
    UInt64 = 11,
    SByte = 12,
    Int16 = 13,
    Int32 = 14,
    Int64 = 15,
    Single = 16,
    Double = 17,
}

public static class SCTypeCodeExtensions
{
    public static bool IsBoolean(this SCTypeCode type) => unchecked((uint)type - 1u) < 3;

    public static int GetTypeSize(this SCTypeCode type) => type switch
    {
        SCTypeCode.Bool3 => sizeof(bool),

        SCTypeCode.Byte => sizeof(byte),
        SCTypeCode.UInt16 => sizeof(ushort),
        SCTypeCode.UInt32 => sizeof(uint),
        SCTypeCode.UInt64 => sizeof(ulong),

        SCTypeCode.SByte => sizeof(sbyte),
        SCTypeCode.Int16 => sizeof(short),
        SCTypeCode.Int32 => sizeof(int),
        SCTypeCode.Int64 => sizeof(long),

        SCTypeCode.Single => sizeof(float),
        SCTypeCode.Double => sizeof(double),

        _ => throw new ArgumentOutOfRangeException(nameof(type), type.ToString()),
    };

    public static Type GetType(this SCTypeCode type) => type switch
    {
        SCTypeCode.Byte => typeof(byte),
        SCTypeCode.UInt16 => typeof(ushort),
        SCTypeCode.UInt32 => typeof(uint),
        SCTypeCode.UInt64 => typeof(ulong),

        SCTypeCode.SByte => typeof(sbyte),
        SCTypeCode.Int16 => typeof(short),
        SCTypeCode.Int32 => typeof(int),
        SCTypeCode.Int64 => typeof(long),

        SCTypeCode.Single => typeof(float),
        SCTypeCode.Double => typeof(double),

        _ => throw new ArgumentOutOfRangeException(nameof(type), type.ToString()),
    };

    public static object GetValue(this SCTypeCode type, ReadOnlySpan<byte> data)
    {
        // don't use a switch expression here, we want to box our underlying type rather than the last type (double)
        switch (type)
        {
            case SCTypeCode.Byte:   return data[0];
            case SCTypeCode.UInt16: return ReadUInt16LittleEndian(data);
            case SCTypeCode.UInt32: return ReadUInt32LittleEndian(data);
            case SCTypeCode.UInt64: return ReadUInt64LittleEndian(data);
            case SCTypeCode.SByte:  return (sbyte) data[0];
            case SCTypeCode.Int16:  return ReadInt16LittleEndian(data);
            case SCTypeCode.Int32:  return ReadInt32LittleEndian(data);
            case SCTypeCode.Int64:  return ReadInt64LittleEndian(data);
            case SCTypeCode.Single: return ReadSingleLittleEndian(data);
            case SCTypeCode.Double: return ReadDoubleLittleEndian(data);
            default:
                throw new ArgumentOutOfRangeException(nameof(type), type.ToString());
        }
    }

    public static void SetValue(this SCTypeCode type, Span<byte> data, object value)
    {
        switch (type)
        {
            case SCTypeCode.Byte: data[0] = (byte)value; break;
            case SCTypeCode.UInt16: WriteUInt16LittleEndian(data, (ushort)value); break;
            case SCTypeCode.UInt32: WriteUInt32LittleEndian(data, (uint)value); break;
            case SCTypeCode.UInt64: WriteUInt64LittleEndian(data, (ulong)value); break;

            case SCTypeCode.SByte: data[0] = (byte)(sbyte)value; break;
            case SCTypeCode.Int16: WriteInt16LittleEndian(data, (short)value); break;
            case SCTypeCode.Int32: WriteInt32LittleEndian(data, (int)value); break;
            case SCTypeCode.Int64: WriteInt64LittleEndian(data, (long)value); break;

            case SCTypeCode.Single: WriteSingleLittleEndian(data, (float)value); break;
            case SCTypeCode.Double: WriteDoubleLittleEndian(data, (double)value); break;

            default:
                throw new ArgumentOutOfRangeException(nameof(type), type.ToString());
        }
    }
}

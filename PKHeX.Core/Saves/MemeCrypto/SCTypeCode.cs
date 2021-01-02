using System;

namespace PKHeX.Core
{
    /// <summary>
    /// Block type for a <see cref="SCBlock"/>.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1027:Mark enums with FlagsAttribute", Justification = "NOT FLAGS")]
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
        public static bool IsBoolean(this SCTypeCode type) => (byte)type - 1 < 3;

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

            _ => throw new ArgumentException(type.ToString(), nameof(type))
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

            _ => throw new ArgumentException(type.ToString(), nameof(type))
        };

        public static object GetValue(this SCTypeCode type, byte[] data)
        {
            // don't use a switch expression here, we want to box our underlying type rather than the last type (double)
            switch (type)
            {
                case SCTypeCode.Byte:   return data[0];
                case SCTypeCode.UInt16: return BitConverter.ToUInt16(data, 0);
                case SCTypeCode.UInt32: return BitConverter.ToUInt32(data, 0);
                case SCTypeCode.UInt64: return BitConverter.ToUInt64(data, 0);
                case SCTypeCode.SByte:  return (sbyte) data[0];
                case SCTypeCode.Int16:  return BitConverter.ToInt16(data, 0);
                case SCTypeCode.Int32:  return BitConverter.ToInt32(data, 0);
                case SCTypeCode.Int64:  return BitConverter.ToInt64(data, 0);
                case SCTypeCode.Single: return BitConverter.ToSingle(data, 0);
                case SCTypeCode.Double: return BitConverter.ToDouble(data, 0);
                default:
                    throw new ArgumentException(type.ToString(), nameof(type));
            }
        }

        public static void SetValue(this SCTypeCode type, byte[] data, object value)
        {
            switch (type)
            {
                case SCTypeCode.Byte: data[0] = (byte)value; break;
                case SCTypeCode.UInt16: BitConverter.GetBytes((ushort)value).CopyTo(data, 0); break;
                case SCTypeCode.UInt32: BitConverter.GetBytes((uint)value).CopyTo(data, 0); break;
                case SCTypeCode.UInt64: BitConverter.GetBytes((ulong)value).CopyTo(data, 0); break;

                case SCTypeCode.SByte: data[0] = (byte)value; break;
                case SCTypeCode.Int16: BitConverter.GetBytes((short)value).CopyTo(data, 0); break;
                case SCTypeCode.Int32: BitConverter.GetBytes((int)value).CopyTo(data, 0); break;
                case SCTypeCode.Int64: BitConverter.GetBytes((long)value).CopyTo(data, 0); break;

                case SCTypeCode.Single: BitConverter.GetBytes((float)value).CopyTo(data, 0); break;
                case SCTypeCode.Double: BitConverter.GetBytes((double)value).CopyTo(data, 0); break;

                default: throw new ArgumentException(type.ToString(), nameof(type));
            }
        }
    }
}

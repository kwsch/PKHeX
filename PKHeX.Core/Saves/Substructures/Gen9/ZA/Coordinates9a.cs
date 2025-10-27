using System;
using System.Text;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

public sealed class Coordinates9a(SAV9ZA sav, SCBlock block) : SaveBlock<SAV9ZA>(sav, block.Raw)
{
    private Span<byte> MapName() => Data[..0x20];

    public string Map
    {
        get
        {
            var span = MapName();
            var trim = span.IndexOf<byte>(0);
            if (trim >= 0)
                span = span[..trim];
            return Encoding.ASCII.GetString(span);
        }
        set
        {
            var span = MapName();
            span.Clear();
            var toWrite = value.Length > span.Length ? span.Length : value.Length;
            Encoding.ASCII.GetBytes(value.AsSpan(0, toWrite), span);
        }
    }

    // Position
    public float X { get => ReadSingleLittleEndian(Data[0x20..]); set => WriteSingleLittleEndian(Data[0x20..], value); }
    public float Z { get => ReadSingleLittleEndian(Data[0x24..]); set => WriteSingleLittleEndian(Data[0x24..], value); }
    public float Y { get => ReadSingleLittleEndian(Data[0x28..]); set => WriteSingleLittleEndian(Data[0x28..], value); }

    // Rotation
    public float RX { get => ReadSingleLittleEndian(Data[0x30..]); set => WriteSingleLittleEndian(Data[0x30..], value); }
    public float RZ { get => ReadSingleLittleEndian(Data[0x34..]); set => WriteSingleLittleEndian(Data[0x34..], value); }
    public float RY { get => ReadSingleLittleEndian(Data[0x38..]); set => WriteSingleLittleEndian(Data[0x38..], value); }
    public float RW { get => ReadSingleLittleEndian(Data[0x3C..]); set => WriteSingleLittleEndian(Data[0x3C..], value); }
    public double Rotation => Math.Atan2(RZ, RW) * 360.0 / Math.PI;

    public void SetCoordinates(float x, float y, float z)
    {
        // Only set coordinates if epsilon is different enough
        const float epsilon = 0.0001f;
        if (Math.Abs(X - x) < epsilon && Math.Abs(Y - y) < epsilon && Math.Abs(Z - z) < epsilon)
            return;
        X = x;
        Y = y;
        Z = z;
    }

    public void SetPlayerRotation(double rotation)
    {
        var angle = rotation * Math.PI / 360.0d;
        SAV.Coordinates.SetPlayerRotation(0, (float)Math.Sin(angle), 0, (float)Math.Cos(angle));
    }

    public void SetPlayerRotation(float rx, float ry, float rz, float rw)
    {
        // Only set coordinates if epsilon is different enough
        const float epsilon = 0.0001f;
        if (Math.Abs(RX - rx) < epsilon && Math.Abs(RY - ry) < epsilon && Math.Abs(RZ - rz) < epsilon && Math.Abs(RW - rw) < epsilon)
            return;
        RX = rx;
        RY = ry;
        RZ = rz;
        RW = rw;
    }
}

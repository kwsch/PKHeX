using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

public sealed class ConfigSave9a(SAV9ZA sav, SCBlock block) : SaveBlock<SAV9ZA>(sav, block.Raw)
{
    public const int Size = 0x10;

    // Structure: u64 backing flags
    // u64:1 Rumble Disable (false = rumble enabled)
    // u64:2 Text Speed (3 = instant, not a selectable option but supported)
    // u64:1 Minimap Rotate (false = disabled)
    // u64:1 Camera Invert Vertical (false = inverted)
    // u64:1 Camera Invert Horizontal (false = inverted)
    // u64:3 Camera Sensitivity
    // u64:4 Volume Background Music
    // u64:4 Volume Sound Effects
    // u64:4 Volume Cries

    private ulong Flags
    {
        get => ReadUInt64LittleEndian(Data);
        set => WriteUInt64LittleEndian(Data, value);
    }

    public bool IsRumbleEnabled
    {
        get => (Flags & 0x1) == 0;
        set
        {
            if (!value)
                Flags |= 0x1;
            else
                Flags &= ~0x1UL;
        }
    }

    public TextSpeedOption TextSpeed
    {
        get => (TextSpeedOption)((Flags >> 1) & 0x3);
        set
        {
            Flags &= ~(3u << 1);
            Flags |= (((ulong)value & 3ul) << 1);
        }
    }

    public bool IsMinimapRotate
    {
        get => (Flags & 0x8u) != 0;
        set
        {
            if (value)
                Flags |= 0x8u;
            else
                Flags &= ~0x8u;
        }
    }

    public bool IsCameraInvertVertical
    {
        get => (Flags & 0x10u) == 0;
        set
        {
            if (!value)
                Flags |= 0x10u;
            else
                Flags &= ~0x10ul;
        }
    }

    public bool IsCameraInvertHorizontal
    {
        get => (Flags & 0x20u) == 0;
        set
        {
            if (!value)
                Flags |= 0x20u;
            else
                Flags &= ~0x20u;
        }
    }

    public byte CameraSensitivity
    {
        get => (byte)((Flags >> 6) & 0x7);
        set
        {
            Flags &= ~(0x7ul << 6);
            Flags |= ((value & 0x7u) << 6);
        }
    }

    public byte VolumeBackgroundMusic
    {
        get => (byte)((Flags >> 9) & 0xF);
        set
        {
            Flags &= ~(0xFul << 9);
            Flags |= ((value & 0xFu) << 9);
        }
    }

    public byte VolumeSoundEffects
    {
        get => (byte)((Flags >> 13) & 0xF);
        set
        {
            Flags &= ~(0xFul << 13);
            Flags |= ((value & 0xFu) << 13);
        }
    }

    public byte VolumeCries
    {
        get => (byte)((Flags >> 17) & 0xF);
        set
        {
            Flags &= ~(0xFul << 17);
            Flags |= ((value & 0xFu) << 17);
        }
    }
}


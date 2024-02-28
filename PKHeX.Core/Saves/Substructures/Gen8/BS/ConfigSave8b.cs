using System;
using System.ComponentModel;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

/// <summary>
/// Configuration settings for player preference.
/// </summary>
/// <remarks>size 0x40, struct_name CONFIG</remarks>
[TypeConverter(typeof(ExpandableObjectConverter))]
public sealed class ConfigSave8b(SAV8BS sav, Memory<byte> raw) : SaveBlock<SAV8BS>(sav, raw)
{
    public TextSpeedOption TextSpeed
    {
        get => (TextSpeedOption)ReadInt32LittleEndian(Data);
        set => WriteInt32LittleEndian(Data, (int)value);
    }

    public int Language
    {
        get => ReadInt32LittleEndian(Data[4..]);
        set => WriteInt32LittleEndian(Data[4..], value);
    }

    public bool IsKanji { get => Data[8] == 1; set => Data[8] = (byte)(value ? 1 : 0); }

    public int WindowType
    {
        get => ReadInt32LittleEndian(Data[0x0C..]);
        set => WriteInt32LittleEndian(Data[0x0C..], value);
    }

    public BattleAnimationSetting MoveAnimations
    {
        get => (BattleAnimationSetting)ReadInt32LittleEndian(Data[0x10..]);
        set => WriteInt32LittleEndian(Data[0x10..], (int)value);
    }

    public BattleStyleSetting BattleStyle
    {
        get => (BattleStyleSetting)ReadInt32LittleEndian(Data[0x14..]);
        set => WriteInt32LittleEndian(Data[0x14..], (int)value);
    }

    public PartyBoxSetting PartyBox
    {
        get => (PartyBoxSetting)ReadInt32LittleEndian(Data[0x18..]);
        set => WriteInt32LittleEndian(Data[0x18..], (int)value);
    }

    // 4 byte bool, nice
    public bool RegistNickname      { get => Data[0x1C] == 1; set => Data[0x1C] = (byte)(value ? 1 : 0); }
    public bool GyroSensor          { get => Data[0x20] == 1; set => Data[0x20] = (byte)(value ? 1 : 0); }
    public bool CameraShakeOfFossil { get => Data[0x24] == 1; set => Data[0x24] = (byte)(value ? 1 : 0); }

    public CameraInputMode CameraUpDown
    {
        get => (CameraInputMode)ReadInt32LittleEndian(Data[0x28..]);
        set => WriteInt32LittleEndian(Data[0x28..], (int)value);
    }

    public CameraInputMode CamerLeftRight
    {
        get => (CameraInputMode)ReadInt32LittleEndian(Data[0x2C..]);
        set => WriteInt32LittleEndian(Data[0x2C..], (int)value);
    }

    public bool AutoReport         { get => Data[0x30] == 1; set => Data[0x30] = (byte)(value ? 1 : 0); }

    public InputMode Input
    {
        get => (InputMode)ReadInt32LittleEndian(Data[0x34..]);
        set => WriteInt32LittleEndian(Data[0x34..], (int)value);
    }

    public bool ShowNicknames      { get => Data[0x38] == 1; set => Data[0x38] = (byte)(value ? 1 : 0); }
    public byte VolumeBGM          { get => Data[0x3C]; set => Data[0x3C] = value; }
    public byte VolumeSoundEffects { get => Data[0x3D]; set => Data[0x3D] = value; }
    public byte VolumeVoice        { get => Data[0x3E]; set => Data[0x3E] = value; }

    public enum CameraInputMode
    {
        Normal,
        Reverse,
    }
    public enum InputMode
    {
        Easy,
        Normal,
    }

    public enum BattleAnimationSetting
    {
        EffectsON,
        EffectsOFF,
    }

    public enum BattleStyleSetting
    {
        SWITCH,
        SET,
    }

    public enum PartyBoxSetting
    {
        Select,
        SendBox,
    }
}

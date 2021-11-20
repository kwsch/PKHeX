using System;
using System.ComponentModel;

namespace PKHeX.Core
{
    /// <summary>
    /// Configuration settings for player preference.
    /// </summary>
    /// <remarks>size 0x40, struct_name CONFIG</remarks>
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public sealed class ConfigSave8b : SaveBlock
    {
        public ConfigSave8b(SAV8BS sav, int offset) : base(sav) => Offset = offset;

        public TextSpeedOption TextSpeed { get => (TextSpeedOption)BitConverter.ToInt32(Data, Offset + 0); set => BitConverter.GetBytes((int)value).CopyTo(Data, Offset + 0); }
        public int Language { get => BitConverter.ToInt32(Data, Offset + 4); set => BitConverter.GetBytes(value).CopyTo(Data, Offset + 4); }
        public bool IsKanji { get => Data[Offset + 8] == 1; set => Data[Offset + 8] = (byte)(value ? 1 : 0); }

        public int WindowType
        {
            get => BitConverter.ToInt32(Data, Offset + 0x0C);
            set => BitConverter.GetBytes(value).CopyTo(Data, Offset + 0x0C);
        }

        public BattleAnimationSetting MoveAnimations
        {
            get => (BattleAnimationSetting)BitConverter.ToInt32(Data, Offset + 0x10);
            set => BitConverter.GetBytes((int)value).CopyTo(Data, Offset + 0x10);
        }

        public BattleStyleSetting BattleStyle
        {
            get => (BattleStyleSetting)BitConverter.ToInt32(Data, Offset + 0x14);
            set => BitConverter.GetBytes((int)value).CopyTo(Data, Offset + 0x14);
        }

        public PartyBoxSetting PartyBox
        {
            get => (PartyBoxSetting)BitConverter.ToInt32(Data, Offset + 0x18);
            set => BitConverter.GetBytes((int)value).CopyTo(Data, Offset + 0x18);
        }

        // 4 byte bool, nice
        public bool RegistNickname      { get => Data[Offset + 0x1C] == 1; set => Data[Offset + 0x1C] = (byte)(value ? 1 : 0); }
        public bool GyroSensor          { get => Data[Offset + 0x20] == 1; set => Data[Offset + 0x20] = (byte)(value ? 1 : 0); }
        public bool CameraShakeOfFossil { get => Data[Offset + 0x24] == 1; set => Data[Offset + 0x24] = (byte)(value ? 1 : 0); }

        public CameraInputMode CameraUpDown   { get => (CameraInputMode)BitConverter.ToInt32(Data, Offset + 0x28); set => BitConverter.GetBytes((int)value).CopyTo(Data, Offset + 0x28); }
        public CameraInputMode CamerLeftRight { get => (CameraInputMode)BitConverter.ToInt32(Data, Offset + 0x2C); set => BitConverter.GetBytes((int)value).CopyTo(Data, Offset + 0x2C); }
        public bool AutoReport         { get => Data[Offset + 0x30] == 1; set => Data[Offset + 0x30] = (byte)(value ? 1 : 0); }
        public InputMode Input         { get => (InputMode)BitConverter.ToInt32(Data, Offset + 0x34); set => BitConverter.GetBytes((int)value).CopyTo(Data, Offset + 0x34); }
        public bool ShowNicknames      { get => Data[Offset + 0x38] == 1; set => Data[Offset + 0x38] = (byte)(value ? 1 : 0); }
        public byte VolumeBGM          { get => Data[Offset + 0x3C]; set => Data[Offset + 0x3C] = value; }
        public byte VolumeSoundEffects { get => Data[Offset + 0x3D]; set => Data[Offset + 0x3D] = value; }
        public byte VolumeVoice        { get => Data[Offset + 0x3E]; set => Data[Offset + 0x3E] = value; }

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
}

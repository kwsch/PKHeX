using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

public sealed class ConfigSave9(SAV9SV sav, SCBlock block) : SaveBlock<SAV9SV>(sav, block.Data)
{
    // Structure: u32
    /* TalkingSpeed:2
     * SkipMoveLearning:1 | On = 0, Off = 1
     * SendToBoxes:1 | Manual = 0, Automatic = 1
     *
     * GiveNicknames:1 | On = 0, Off = 1
     * InvertCameraVertical:1 | Regular = 0, Inverted = 1
     * InvertCameraHorizontal:1 | Regular = 0, Inverted = 1
     * AutoSave:1 | On = 0, Off = 1
     *
     * ShowNicknames:1 | Show = 0, Don't = 1
     * SkipCutscenes:1 | On = 0, Off = 1
     * VolumeBGM:4
     * VolumeSE:4
     * VolumeCry:4
     *
     * Rumble | On = 0, Off = 1
     * Helping Functions | On = 0, Off = 1
     *
     * Last 8 bits unused?
     */

    public int ConfigValue
    {
        get => ReadInt32LittleEndian(Data);
        set => WriteInt32LittleEndian(Data, value);
    }

    private const int DefaultValue = 0x002AAA05;

    public void Reset() => ConfigValue = DefaultValue;

    public int TalkingSpeed                     { get => ConfigValue & 0b11;                       set => ConfigValue = (ConfigValue & ~0b11) | (value & 0b11); }
    public ConfigOption9 SkipMoveLearning       { get => (ConfigOption9)((ConfigValue >> 02) & 1); set => ConfigValue = (ConfigValue & ~(1 << 02)) | ((((int)value) & 1) << 02); }
    public ConfigOption9 PromptSendToBox        { get => (ConfigOption9)((ConfigValue >> 03) & 1); set => ConfigValue = (ConfigValue & ~(1 << 03)) | ((((int)value) & 1) << 03); }
    public ConfigOption9 PromptGiveNickname     { get => (ConfigOption9)((ConfigValue >> 04) & 1); set => ConfigValue = (ConfigValue & ~(1 << 04)) | ((((int)value) & 1) << 04); }
    public ConfigOption9 InvertCameraVertical   { get => (ConfigOption9)((ConfigValue >> 05) & 1); set => ConfigValue = (ConfigValue & ~(1 << 05)) | ((((int)value) & 1) << 05); }
    public ConfigOption9 InvertCameraHorizontal { get => (ConfigOption9)((ConfigValue >> 06) & 1); set => ConfigValue = (ConfigValue & ~(1 << 06)) | ((((int)value) & 1) << 06); }
    public ConfigOption9 EnableAutoSave         { get => (ConfigOption9)((ConfigValue >> 07) & 1); set => ConfigValue = (ConfigValue & ~(1 << 07)) | ((((int)value) & 1) << 07); }
    public ConfigOption9 ShowNicknames          { get => (ConfigOption9)((ConfigValue >> 08) & 1); set => ConfigValue = (ConfigValue & ~(1 << 08)) | ((((int)value) & 1) << 08); }
    public ConfigOption9 SkipCutscenes          { get => (ConfigOption9)((ConfigValue >> 09) & 1); set => ConfigValue = (ConfigValue & ~(1 << 09)) | ((((int)value) & 1) << 09); }
    public int VolumeBGM                        { get => (ConfigValue >> 10) & 0b1111;             set => ConfigValue = (ConfigValue & ~(0b1111 << 10)) | ((value & 0b1111) << 10); }
    public int VolumeSE                         { get => (ConfigValue >> 14) & 0b1111;             set => ConfigValue = (ConfigValue & ~(0b1111 << 14)) | ((value & 0b1111) << 14); }
    public int VolumeCry                        { get => (ConfigValue >> 18) & 0b1111;             set => ConfigValue = (ConfigValue & ~(0b1111 << 18)) | ((value & 0b1111) << 18); }
    public ConfigOption9 EnableRumble           { get => (ConfigOption9)((ConfigValue >> 22) & 1); set => ConfigValue = (ConfigValue & ~(1 << 22)) | ((((int)value) & 1) << 22); }
    public ConfigOption9 EnableHelp             { get => (ConfigOption9)((ConfigValue >> 23) & 1); set => ConfigValue = (ConfigValue & ~(1 << 23)) | ((((int)value) & 1) << 23); }
}

public enum ConfigOption9 : byte
{
    On = 0,
    Off = 1,
}

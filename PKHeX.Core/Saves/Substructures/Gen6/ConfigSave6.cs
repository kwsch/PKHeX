using System;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

public sealed class ConfigSave6 : SaveBlock<SAV6>
{
    /* ===32 bits===
     * talkSpeed:2      0,1
     * battleAnim:1     2
     * battleStyle:1    3
     * unknown:4        4..7
     * battleBG:5       8..12
     * buttonMode:2     13,14
     * forcedSave:1     15
     * flag1:1          16
     * enablePSS:1      17
     * enablePR:1       18
     * unknown:14       19..31
     */

    public ConfigSave6(SAV6XY sav, Memory<byte> raw) : base(sav, raw) { }
    public ConfigSave6(SAV6AO sav, Memory<byte> raw) : base(sav, raw) { }

    public int ConfigValue
    {
        get => ReadInt32LittleEndian(Data);
        set => WriteInt32LittleEndian(Data, value);
    }

    public int TalkingSpeed
    {
        get => ConfigValue & 3;
        set => ConfigValue = (ConfigValue & ~3) | (value & 3);
    }

    public BattleAnimationSetting BattleAnimation
    {
        // Effects OFF = 1, Effects ON = 0
        get => (BattleAnimationSetting)((ConfigValue >> 2) & 1);
        set => ConfigValue = (ConfigValue & ~(1 << 2)) | ((byte)value << 2);
    }

    public BattleStyleSetting BattleStyle
    {
        // SET = 1, SWITCH = 0
        get => (BattleStyleSetting)((ConfigValue >> 3) & 1);
        set => ConfigValue = (ConfigValue & ~(1 << 3)) | ((byte)value << 3);
    }

    // UNKNOWN?

    public int BattleBackground
    {
        // Only values 0-14 are used.
        get => (ConfigValue >> 8) & 0x1F;
        set => ConfigValue = (ConfigValue & ~(0x1F << 8)) | (value << 8);
    }

    public int ButtonMode
    {
        get => (ConfigValue >> 13) & 3;
        set => ConfigValue = (ConfigValue & ~(1 << 13)) | (value << 13);
    }

    public int ForceSaveBeforeOnline
    {
        get => (ConfigValue >> 15) & 1;
        set => ConfigValue = (ConfigValue & ~(1 << 15)) | (value << 15);
    }

    public int EnableFlag1
    {
        get => (ConfigValue >> 16) & 1;
        set => ConfigValue = (ConfigValue & ~(1 << 16)) | (value << 16);
    }

    public int EnablePSSFlag
    {
        get => (ConfigValue >> 17) & 1;
        set => ConfigValue = (ConfigValue & ~(1 << 17)) | (value << 17);
    }

    public int EnableTrainerPRFlag
    {
        get => (ConfigValue >> 18) & 1;
        set => ConfigValue = (ConfigValue & ~(1 << 18)) | (value << 18);
    }

    // NOTE: BELOW COMES FROM LGPE. MAYBE THIS IS WHAT THEY USE THE FLAGS FOR?

    /// <summary>
    /// <see cref="LanguageID"/> for messages, stored with <see cref="LanguageID.UNUSED_6"/> skipped in the enumeration.
    /// </summary>
    public int Language
    {
        get => GetLanguageID((ConfigValue >> 4) & 0xF);
        set => ConfigValue = ((ConfigValue & ~0xF0) | (SetLanguageID(value) << 4));
    }

    private static int GetLanguageID(int rawValue) => rawValue >= (int)LanguageID.UNUSED_6 ? rawValue + 1 : rawValue; // sets langBank to LanguageID
    private static int SetLanguageID(int rawValue) => rawValue > (int)LanguageID.UNUSED_6 ? rawValue - 1 : rawValue; // sets LanguageID to langBank

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
}

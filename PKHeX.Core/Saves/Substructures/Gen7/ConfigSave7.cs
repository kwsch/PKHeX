using System;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

public sealed class ConfigSave7(SAV7 sav, Memory<byte> raw) : SaveBlock<SAV7>(sav, raw)
{
    /* ===32 bits===
     * talkSpeed:2      0,1
     * battleAnim:1     2
     * battleStyle:1    3
     * unknown:9        4..12
     * buttonMode:2     13,14
     * boxStatus:1      15
     * everything else: unknown
     */

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

    public int ButtonMode
    {
        get => (ConfigValue >> 13) & 3;
        set => ConfigValue = (ConfigValue & ~(1 << 13)) | (value << 13);
    }

    public int BoxStatus
    {
        // MANUAL = 1, AUTOMATIC = 0
        get => (ConfigValue >> 15) & 1;
        set => ConfigValue = (ConfigValue & ~(1 << 15)) | (value << 15);
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

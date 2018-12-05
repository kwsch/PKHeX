using System;

namespace PKHeX.Core
{
    public sealed class ConfigSave7b : SaveBlock
    {
        public ConfigSave7b(SAV7b sav) : base(sav)
        {
            Offset = sav.GetBlockOffset(BelugaBlockIndex.ConfigSave);
        }

        public int ConfigValue
        {
            get => BitConverter.ToInt32(Data, Offset);
            set => BitConverter.GetBytes(value).CopyTo(Data, Offset);
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

        public enum BattleAnimationSetting
        {
            EffectsON,
            EffectsOFF,
        }

        public enum BattleStyleSetting
        {
            SET,
            SWITCH,
        }
    }
}
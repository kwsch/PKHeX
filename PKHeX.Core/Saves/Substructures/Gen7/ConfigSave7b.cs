using System;

namespace PKHeX.Core
{
    public sealed class ConfigSave7b : SaveBlock
    {
        /* ===First 8 bits===
         * talkSpeed:2
         * battleAnim:1
         * battleStyle:1
         * language:4
         *
         * everything else: unknown
         */

        public ConfigSave7b(SAV7b sav, int offset) : base(sav) => Offset = offset;

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

        /// <summary>
        /// <see cref="LanguageID"/> for messages, stored with <see cref="LanguageID.UNUSED_6"/> skipped in the enumeration.
        /// </summary>
        public int Language
        {
            get => GetLanguageID((ConfigValue >> 4) & 0xF);
            set => ConfigValue = ((ConfigValue & ~0xF0) | SetLanguageID(value) << 4);
        }

        private static int GetLanguageID(int i) => i >= (int) LanguageID.UNUSED_6 ? i + 1 : i; // sets langBank to LanguageID
        private static int SetLanguageID(int i) => i > (int) LanguageID.UNUSED_6 ? i - 1 : i; // sets LanguageID to langBank

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
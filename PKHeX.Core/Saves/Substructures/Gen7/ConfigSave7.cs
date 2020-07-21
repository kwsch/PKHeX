using System;

namespace PKHeX.Core
{
    public sealed class ConfigSave7 : SaveBlock
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

        public ConfigSave7(SAV7SM sav, int offset) : base(sav) => Offset = offset;
        public ConfigSave7(SAV7USUM sav, int offset) : base(sav) => Offset = offset;

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
            set => ConfigValue = ((ConfigValue & ~0xF0) | SetLanguageID(value) << 4);
        }

        private static int GetLanguageID(int i) => i >= (int)LanguageID.UNUSED_6 ? i + 1 : i; // sets langBank to LanguageID
        private static int SetLanguageID(int i) => i > (int)LanguageID.UNUSED_6 ? i - 1 : i; // sets LanguageID to langBank

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
}

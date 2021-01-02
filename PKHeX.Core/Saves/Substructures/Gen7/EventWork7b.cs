using System;

namespace PKHeX.Core
{
    public sealed class EventWork7b : SaveBlock, IEventWork<int>
    {
        public EventWork7b(SAV7b sav, int offset) : base(sav)
        {
            Offset = offset;
            // Zone @ 0x21A0 - 0x21AF (128 flags)
            // System @ 0x21B0 - 0x21EF (512 flags) -- is this really 256 instead, with another 256 region after for the small vanish?
            // Vanish @ 0x21F0 - 0x22AF (1536 flags)
            // Event @ 0x22B0 - 0x23A7 (rest of the flags) (512) -- I think trainer flags are afterwards.... For now, this is a catch-all

            // time flags (39 used flags of 42) = 6 bytes 0x22F0-0x22F5
            // trainer flags (???) = 0x22F6 - end?
        }

        // Overall Layout
        private const int WorkCount = 1000;
        private const int WorkSize = sizeof(int);
        private const int FlagStart = WorkCount * WorkSize;
        private const int FlagCount = EventFlagStart + EventFlagCount;

        // Breakdown!
        private const int ZoneWorkCount = 0x20; // 32
        private const int SystemWorkCount = 0x80; // 128
        private const int SceneWorkCount = 0x200; // 512
        private const int EventWorkCount = 0x100; // 256
        // private const int UnusedWorkCount = 72;

        private const int ZoneWorkStart = 0;
        private const int SystemWorkStart = ZoneWorkStart + ZoneWorkCount;
        private const int SceneWorkStart = SystemWorkStart + SystemWorkCount;
        private const int EventWorkStart = SceneWorkStart + SceneWorkCount;

        private const int ZoneFlagCount = 0x80; // 128
        private const int SystemFlagCount = 0x200; // 512
        private const int VanishFlagCount = 0x600; // 1536
        private const int EventFlagCount = 0x7C0; // 1984

        private const int ZoneFlagStart = 0;
        private const int SystemFlagStart = ZoneFlagStart + ZoneFlagCount;
        private const int VanishFlagStart = SystemFlagStart + SystemFlagCount;
        private const int EventFlagStart = VanishFlagStart + VanishFlagCount;

        public int MaxFlag => FlagCount;
        public int MaxWork => WorkCount;

        public int GetWork(int index) => BitConverter.ToInt32(Data, Offset + (index * WorkSize));
        public void SetWork(int index, int value) => BitConverter.GetBytes(value).CopyTo(Data, Offset + (index * WorkSize));
        public int GetWork(EventVarType type, int index) => GetWork(GetWorkRawIndex(type, index));
        public void SetWork(EventVarType type, int index, int value) => SetWork(GetWorkRawIndex(type, index), value);
        public bool GetFlag(EventVarType type, int index) => GetFlag(GetFlagRawIndex(type, index));
        public void SetFlag(EventVarType type, int index, bool value = true) => SetFlag(GetFlagRawIndex(type, index), value);

        public int GetFlagRawIndex(EventVarType type, int index)
        {
            int max = GetFlagCount(type);
            if ((uint)index > max)
                throw new ArgumentException(nameof(index));
            var start = GetFlagStart(type);
            return start + index;
        }

        public int GetWorkRawIndex(EventVarType type, int index)
        {
            int max = GetWorkCount(type);
            if ((uint)index > max)
                throw new ArgumentException(nameof(index));
            var start = GetWorkStart(type);
            return start + index;
        }

        public bool GetFlag(int index)
        {
            var offset = Offset + FlagStart + (index >> 3);
            var current = Data[offset];
            return (current & (1 << (index & 7))) != 0;
        }

        public void SetFlag(int index, bool value = true)
        {
            var offset = Offset + FlagStart + (index >> 3);
            var bit = 1 << (index & 7);
            if (value)
                Data[offset] |= (byte)bit;
            else
                Data[offset] &= (byte)~bit;
        }

        public EventVarType GetFlagType(int index, out int subIndex)
        {
            subIndex = index;
            if (index < ZoneFlagCount)
                return EventVarType.Zone;
            subIndex -= ZoneFlagCount;

            if (subIndex < SystemFlagCount)
                return EventVarType.System;
            subIndex -= SystemFlagCount;

            if (subIndex < VanishFlagCount)
                return EventVarType.Vanish;
            subIndex -= VanishFlagCount;

            if (subIndex < EventFlagCount)
                return EventVarType.Event;

            throw new ArgumentException(nameof(index));
        }

        public EventVarType GetWorkType(int index, out int subIndex)
        {
            subIndex = index;
            if (subIndex < ZoneWorkCount)
                return EventVarType.Zone;
            subIndex -= ZoneWorkCount;

            if (subIndex < SystemWorkCount)
                return EventVarType.System;
            subIndex -= SystemWorkCount;

            if (subIndex < SceneWorkCount)
                return EventVarType.Scene;
            subIndex -= SceneWorkCount;

            if (subIndex < EventWorkCount)
                return EventVarType.Event;

            throw new ArgumentException(nameof(index));
        }

        private static int GetFlagStart(EventVarType type) => type switch
        {
            EventVarType.Zone => ZoneFlagStart,
            EventVarType.System => SystemFlagStart,
            EventVarType.Vanish => VanishFlagStart,
            EventVarType.Event => EventFlagStart,
            _ => throw new ArgumentException(nameof(type))
        };

        private static int GetWorkStart(EventVarType type) => type switch
        {
            EventVarType.Zone => ZoneWorkStart,
            EventVarType.System => SystemWorkStart,
            EventVarType.Scene => SceneWorkStart,
            EventVarType.Event => EventWorkStart,
            _ => throw new ArgumentException(nameof(type))
        };

        private static int GetFlagCount(EventVarType type) => type switch
        {
            EventVarType.Zone => ZoneFlagCount,
            EventVarType.System => SystemFlagCount,
            EventVarType.Vanish => VanishFlagCount,
            EventVarType.Event => EventFlagCount,
            _ => throw new ArgumentException(nameof(type))
        };

        private static int GetWorkCount(EventVarType type) => type switch
        {
            EventVarType.Zone => ZoneWorkCount,
            EventVarType.System => SystemWorkCount,
            EventVarType.Scene => SceneWorkCount,
            EventVarType.Event => EventWorkCount,
            _ => throw new ArgumentException(nameof(type))
        };
    }
}
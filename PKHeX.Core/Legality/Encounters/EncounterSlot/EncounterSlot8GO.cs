using System;

namespace PKHeX.Core
{
    public sealed class EncounterSlot8GO : EncounterSlot, IPogoSlotTime
    {
        public override int Generation => 8;
        public GameVersion OriginGroup { get; }

        public PogoType Type { get; }
        public Shiny Shiny { get; }
        public int Start { get; }
        public int End { get; }

        public override string LongName
        {
            get
            {
                var init = $"{Name} ({Type})";
                if (Start == 0 && End == 0)
                    return init;
                return $"{init}: {GetDateString(Start)}-{GetDateString(End)}";
            }
        }

        private static string GetDateString(int time) => GetDate(time).ToString("yyyy.MM.dd");

        private static DateTime GetDate(int time)
        {
            var d = time & 0xFF;
            var m = (time >> 8) & 0xFF;
            var y = time >> 16;
            return new DateTime(y, m, d);
        }

        public bool IsWithinStartEnd(int y, int m, int d)
        {
            var stamp = GetTimeStamp(y, m, d);
            return IsWithinStartEnd(stamp);
        }

        public bool IsWithinStartEnd(int stamp)
        {
            if (End == 0)
                return Start <= stamp;
            if (Start == 0)
                return stamp <= End;
            return Start <= stamp && stamp <= End;
        }

        public static int GetTimeStamp(int y, int m, int d) => (y << 16) | (m << 8) | d;

        public EncounterSlot8GO(EncounterArea8g area, int species, int form, GameVersion gameVersion, PogoType type, Shiny shiny, int start, int end) : base(area)
        {
            Species = species;
            Form = form;
            LevelMin = type.GetMinLevel();
            LevelMax = 40;
            Start = start;
            End = end;

            Shiny = shiny;
            Type = type;

            OriginGroup = gameVersion;
        }

        public int GetMinIV() => Type.GetMinIV();
        public bool IsBallValid(Ball ball) => Type.IsBallValid(ball);

        protected override void ApplyDetails(ITrainerInfo sav, EncounterCriteria criteria, PKM pk)
        {
            base.ApplyDetails(sav, criteria, pk);
            if (Start != 0 || End != 0)
                pk.MetDate = GetRandomValidDate();
        }

        public DateTime GetRandomValidDate()
        {
            if (Start == 0)
                return End == 0 ? DateTime.Now : GetDate(End);

            var end = Math.Max(Start, End);
            var stamp = Start + Util.Rand.Next(0, Start - end + 1);
            return GetDate(stamp);
        }
    }

    public interface IPogoSlot
    {
        Shiny Shiny { get; }
        PogoType Type { get; }
    }

    public interface IPogoSlotTime : IPogoSlot
    {
        int Start { get; }
        int End { get; }
    }

    public enum PogoType : byte
    {
        None, // Don't use this.

        Wild,
        Egg,

        /// <summary> Raid Boss, requires Lv. 15 and IV=1 </summary>
        Raid15 = 10,
        /// <summary> Raid Boss, requires Lv. 20 and IV=10 </summary>
        Raid20,

        /// <summary> Field Research, requires Lv. 15 and IV=1 </summary>
        Field15 = 20,
        /// <summary> Field Research, requires Lv. 15 and IV=10 (Mythicals) </summary>
        FieldM,
        /// <summary> Field Research, requires Lv. 15 and IV=10 (Mythicals, Poké Ball only) </summary>
        FieldP,
        /// <summary> Field Research, requires Lv. 20 and IV=10 (GBL Mythicals) </summary>
        Field20,

        /// <summary> Purified, requires Lv. 8 and IV=1 (Premier Ball) </summary>
        Shadow = 30,
        /// <summary> Purified, requires Lv. 8 and IV=1 (No Premier Ball) </summary>
        ShadowPGU,
    }

    public static class PogoTypeExtensions
    {
        public static int GetMinLevel(this PogoType t) => t switch
        {
            PogoType.Raid15 => 15,
            PogoType.Raid20 => 20,
            PogoType.Field15 => 15,
            PogoType.FieldM => 15,
            PogoType.Field20 => 20,
            PogoType.Shadow => 8,
            PogoType.ShadowPGU => 8,
            _ => 1,
        };

        public static int GetMinIV(this PogoType t) => t switch
        {
            PogoType.Wild => 0,
            PogoType.Raid20 => 10,
            PogoType.FieldM => 10,
            PogoType.FieldP => 10,
            PogoType.Field20 => 10,
            _ => 1,
        };

        public static bool IsBallValid(this PogoType t, Ball b) => t switch
        {
            PogoType.Egg => b == Ball.Poke,
            PogoType.FieldP => b == Ball.Poke,
            PogoType.Raid15 => b == Ball.Premier,
            PogoType.Raid20 => b == Ball.Premier,
            _ => (uint)(b - 2) <= 2, // Poke, Great, Ultra
        };
    }
}

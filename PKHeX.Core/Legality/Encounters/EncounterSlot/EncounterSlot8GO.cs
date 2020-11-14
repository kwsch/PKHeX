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
                return $"{init}: {GetDate(Start)}-{GetDate(End)}";
            }
        }

        private static string GetDate(int time)
        {
            var d = time & 0xFF;
            var m = (time >> 8) & 0xFF;
            var y = time >> 16;
            return $"{y:0000}.{m:00}.{d:00}";
        }

        public bool IsWithinStartEnd(int y, int m, int d)
        {
            var stamp = GetTimeStamp(y, m, d);
            return IsWithinStartEnd(stamp);
        }

        public bool IsWithinStartEnd(int stamp)
        {
            if (End == 0)
                return stamp >= Start;
            if (Start == 0)
                return stamp <= Start;
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

        /// <summary> requires level 15 and IV=1 </summary>
        Raid15 = 10,
        /// <summary> requires level 20 and IV=10 </summary>
        Raid20,

        /// <summary> requires level 15 and IV=10 </summary>
        Field15 = 20,

        Shadow = 30,
    }

    public static class PogoTypeExtensions
    {
        public static int GetMinLevel(this PogoType t) => t switch
        {
            _ => 1,
        };

        public static int GetMinIV(this PogoType t) => t switch
        {
            _ => 1,
        };

        public static bool IsBallValid(this PogoType t, Ball b) => t switch
        {
            PogoType.Egg => b == Ball.Poke,
            PogoType.Raid15 => b == Ball.Premier,
            PogoType.Raid20 => b == Ball.Premier,
            _ => b != Ball.Premier,
        };
    }
}

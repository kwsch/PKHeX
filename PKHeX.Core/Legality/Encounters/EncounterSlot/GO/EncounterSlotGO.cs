using System;

namespace PKHeX.Core
{
    public abstract class EncounterSlotGO : EncounterSlot, IPogoSlotTime
    {
        public PogoType Type { get; }
        public Shiny Shiny { get; }
        public int Start { get; }
        public int End { get; }

        protected EncounterSlotGO(EncounterArea area, int start, int end, PogoType type, Shiny shiny) : base(area)
        {
            LevelMin = type.GetMinLevel();
            LevelMax = EncountersGO.MAX_LEVEL;

            Start = start;
            End = end;

            Shiny = shiny;
            Type = type;
        }

        public sealed override string LongName
        {
            get
            {
                var init = $"{Name} ({Type})";
                if (Start == 0 && End == 0)
                    return init;
                return $"{init}: {GetDateString(Start)}-{GetDateString(End)}";
            }
        }

        private static string GetDateString(int time) => time == 0 ? "X" : GetDate(time).ToString("yyyy.MM.dd");

        private static DateTime GetDate(int time)
        {
            var d = time & 0xFF;
            var m = (time >> 8) & 0xFF;
            var y = time >> 16;
            return new DateTime(y, m, d);
        }

        public bool IsWithinStartEnd(int stamp)
        {
            // Events are in UTC, but time zones exist (and delayed captures too).
            // Allow an extra day past the end date.
            const int tolerance = 1;

            if (End == 0)
                return Start <= stamp;
            if (Start == 0)
                return stamp <= End + tolerance;
            return Start <= stamp && stamp <= End + tolerance;
        }

        public static int GetTimeStamp(int y, int m, int d) => (y << 16) | (m << 8) | d;

        public DateTime GetRandomValidDate()
        {
            if (Start == 0)
                return End == 0 ? DateTime.Now : GetDate(End);

            var end = Math.Max(Start, End);
            var delta = end - Start;
            var bias = Util.Rand.Next(0, delta + 1);
            return GetDate(Start).AddDays(bias);
        }

        protected override void ApplyDetails(ITrainerInfo sav, EncounterCriteria criteria, PKM pk)
        {
            base.ApplyDetails(sav, criteria, pk);
            if (Start != 0 || End != 0)
                pk.MetDate = GetRandomValidDate();
            pk.SetRandomIVsGO(Type.GetMinIV());
        }
    }
}
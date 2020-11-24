using System;

namespace PKHeX.Core
{
    /// <summary>
    /// Contains details about an encounter that can be found in <see cref="GameVersion.GO"/>
    /// </summary>
    public abstract class EncounterSlotGO : EncounterSlot, IPogoSlot
    {
        /// <summary> Start Date timestamp of when the encounter became available. </summary>
        public int Start { get; }

        /// <summary> End Date timestamp of when the encounter became unavailable. </summary>
        /// <remarks> If there is no end date (yet), we'll try to clamp to a date in the near-future to prevent it from being open-ended. </remarks>
        public int End { get; }

        /// <summary> Possibility of being shiny. </summary>
        public Shiny Shiny { get; }

        /// <summary> How the encounter was captured. </summary>
        public PogoType Type { get; }

        protected EncounterSlotGO(EncounterArea area, int start, int end, Shiny shiny, PogoType type) : base(area)
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
            if (End == 0)
                return Start <= stamp && GetDate(stamp) <= DateTime.Now;
            if (Start == 0)
                return stamp <= End;
            return Start <= stamp && stamp <= End;
        }

        /// <summary>
        /// Converts a split timestamp into a single integer.
        /// </summary>
        public static int GetTimeStamp(int year, int month, int day) => (year << 16) | (month << 8) | day;

        /// <summary>
        /// Gets a random date within the availability range.
        /// </summary>
        public DateTime GetRandomValidDate()
        {
            if (Start == 0)
                return End == 0 ? DateTime.Now : GetDate(End);

            var start = GetDate(Start);
            if (End == 0)
                return start;
            var end = GetDate(End);
            var delta = end - start;
            var bias = Util.Rand.Next(0, delta.Days + 1);
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
using System;

namespace PKHeX.Core
{
    /// <summary>
    /// Contains details about an encounter that can be found in <see cref="GameVersion.GO"/>.
    /// </summary>
    public abstract record EncounterSlotGO : EncounterSlot, IPogoSlot
    {
        /// <inheritdoc/>
        public int Start { get; }

        /// <inheritdoc/>
        public int End { get; }

        /// <inheritdoc/>
        public Shiny Shiny { get; }

        /// <inheritdoc/>
        public PogoType Type { get; }

        protected EncounterSlotGO(EncounterArea area, int species, int form, int start, int end, Shiny shiny, PogoType type) : base(area, species, form, type.GetMinLevel(), EncountersGO.MAX_LEVEL)
        {
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
                return Start <= stamp && GetDate(stamp) <= DateTime.UtcNow;
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
                return End == 0 ? DateTime.UtcNow : GetDate(End);

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

        public abstract bool GetIVsValid(PKM pkm);
    }
}

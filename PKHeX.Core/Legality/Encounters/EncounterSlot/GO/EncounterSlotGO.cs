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

        /// <inheritdoc/>
        public Gender Gender { get; }

        public override bool IsShiny => Shiny.IsShiny();

        protected EncounterSlotGO(EncounterArea area, int species, int form, int start, int end, Shiny shiny, Gender gender, PogoType type) : base(area, species, form, type.GetMinLevel(), EncountersGO.MAX_LEVEL)
        {
            Start = start;
            End = end;

            Shiny = shiny;
            Gender = gender;
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
            return DateUtil.GetRandomDateWithin(start, end);
        }

        protected override void ApplyDetails(ITrainerInfo sav, EncounterCriteria criteria, PKM pk)
        {
            base.ApplyDetails(sav, criteria, pk);
            if (Start != 0 || End != 0)
                pk.MetDate = GetRandomValidDate();
            if (Gender != Gender.Random)
                pk.Gender = (int)Gender;
            pk.SetRandomIVsGO(Type.GetMinIV());
        }

        public bool GetIVsAboveMinimum(PKM pkm)
        {
            int min = Type.GetMinIV();
            return GetIVsAboveMinimum(pkm, min);
        }

        private static bool GetIVsAboveMinimum(PKM pkm, int min)
        {
            if (pkm.IV_ATK >> 1 < min) // ATK
                return false;
            if (pkm.IV_DEF >> 1 < min) // DEF
                return false;
            return pkm.IV_HP >> 1 >= min; // HP
        }

        public bool GetIVsValid(PKM pkm)
        {
            if (!GetIVsAboveMinimum(pkm))
                return false;

            // HP * 2 | 1 -> HP
            // ATK * 2 | 1 -> ATK&SPA
            // DEF * 2 | 1 -> DEF&SPD
            // Speed is random.

            // All IVs must be odd (except speed) and equal to their counterpart.
            if ((pkm.GetIV(1) & 1) != 1 || pkm.GetIV(1) != pkm.GetIV(4)) // ATK=SPA
                return false;
            if ((pkm.GetIV(2) & 1) != 1 || pkm.GetIV(2) != pkm.GetIV(5)) // DEF=SPD
                return false;
            return (pkm.GetIV(0) & 1) == 1; // HP
        }

        protected override void SetPINGA(PKM pk, EncounterCriteria criteria)
        {
            base.SetPINGA(pk, criteria);
            switch (Shiny)
            {
                case Shiny.Random when !pk.IsShiny && criteria.Shiny.IsShiny():
                case Shiny.Always when !pk.IsShiny: // Force Square
                    pk.PID = (uint)(((pk.TID ^ pk.SID ^ (pk.PID & 0xFFFF) ^ 0) << 16) | (pk.PID & 0xFFFF));
                    break;

                case Shiny.Random when pk.IsShiny && !criteria.Shiny.IsShiny():
                case Shiny.Never when pk.IsShiny: // Force Not Shiny
                    pk.PID ^= 0x1000_0000;
                    break;
            }
        }
    }
}

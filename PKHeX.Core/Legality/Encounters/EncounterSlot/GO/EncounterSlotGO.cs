using System;

namespace PKHeX.Core;

/// <summary>
/// Contains details about an encounter that can be found in <see cref="GameVersion.GO"/>.
/// </summary>
public abstract record EncounterSlotGO : EncounterSlot, IPogoSlot
{
    /// <inheritdoc/>
    public int StartDate { get; }

    /// <inheritdoc/>
    public int EndDate { get; }

    public override Shiny Shiny { get; }

    /// <inheritdoc/>
    public PogoType Type { get; }

    /// <inheritdoc/>
    public Gender Gender { get; }

    public override bool IsShiny => Shiny.IsShiny();

    public override Ball FixedBall => Type.GetValidBall();

    protected EncounterSlotGO(EncounterArea area, ushort species, byte form, int start, int end, Shiny shiny, Gender gender, PogoType type) : base(area, species, form, type.GetMinLevel(), EncountersGO.MAX_LEVEL)
    {
        StartDate = start;
        EndDate = end;

        Shiny = shiny;
        Gender = gender;
        Type = type;
    }

    public sealed override string LongName
    {
        get
        {
            var init = $"{Name} ({Type})";
            if (StartDate == 0 && EndDate == 0)
                return init;
            return $"{init}: {GetDateString(StartDate)}-{GetDateString(EndDate)}";
        }
    }

    private static string GetDateString(int time) => time == 0 ? "X" : $"{GetDate(time):yyyy.MM.dd}";

    private static DateTime GetDate(int time)
    {
        var d = time & 0xFF;
        var m = (time >> 8) & 0xFF;
        var y = time >> 16;
        return new DateTime(y, m, d);
    }

    public bool IsWithinStartEnd(int stamp)
    {
        if (EndDate == 0)
            return StartDate <= stamp && GetDate(stamp) <= GetMaxDateTime();
        if (StartDate == 0)
            return stamp <= EndDate;
        return StartDate <= stamp && stamp <= EndDate;
    }

    /// <summary>
    /// Converts a split timestamp into a single integer.
    /// </summary>
    public static int GetTimeStamp(int year, int month, int day) => (year << 16) | (month << 8) | day;

    private static DateTime GetMaxDateTime() => DateTime.UtcNow.AddHours(12); // UTC+12 for Kiribati, no daylight savings

    /// <summary>
    /// Gets a random date within the availability range.
    /// </summary>
    public DateTime GetRandomValidDate()
    {
        if (StartDate == 0)
            return EndDate == 0 ? GetMaxDateTime() : GetDate(EndDate);

        var start = GetDate(StartDate);
        if (EndDate == 0)
            return start;
        var end = GetDate(EndDate);
        return DateUtil.GetRandomDateWithin(start, end);
    }

    protected override void ApplyDetails(ITrainerInfo sav, EncounterCriteria criteria, PKM pk)
    {
        base.ApplyDetails(sav, criteria, pk);
        if (StartDate != 0 || EndDate != 0)
            pk.MetDate = GetRandomValidDate();
        if (Gender != Gender.Random)
            pk.Gender = (int)Gender;
        pk.SetRandomIVsGO(Type.GetMinIV());
    }

    public bool GetIVsAboveMinimum(PKM pk)
    {
        int min = Type.GetMinIV();
        if (min == 0)
            return true;
        return GetIVsAboveMinimum(pk, min);
    }

    private static bool GetIVsAboveMinimum(PKM pk, int min)
    {
        if (pk.IV_ATK >> 1 < min) // ATK
            return false;
        if (pk.IV_DEF >> 1 < min) // DEF
            return false;
        return pk.IV_HP >> 1 >= min; // HP
    }

    public bool GetIVsValid(PKM pk)
    {
        if (!GetIVsAboveMinimum(pk))
            return false;

        // HP * 2 | 1 -> HP
        // ATK * 2 | 1 -> ATK&SPA
        // DEF * 2 | 1 -> DEF&SPD
        // Speed is random.

        // All IVs must be odd (except speed) and equal to their counterpart.
        if ((pk.GetIV(1) & 1) != 1 || pk.GetIV(1) != pk.GetIV(4)) // ATK=SPA
            return false;
        if ((pk.GetIV(2) & 1) != 1 || pk.GetIV(2) != pk.GetIV(5)) // DEF=SPD
            return false;
        return (pk.GetIV(0) & 1) == 1; // HP
    }

    protected override void SetPINGA(PKM pk, EncounterCriteria criteria)
    {
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

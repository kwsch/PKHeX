using System;
using System.Collections.Generic;

namespace PKHeX.Core;

/// <summary>
/// Stat/misc data for individual species or their associated alternate form data.
/// </summary>
public abstract class PersonalInfo : IPersonalInfo
{
    public abstract byte[] Write();
    public abstract int HP { get; set; }
    public abstract int ATK { get; set; }
    public abstract int DEF { get; set; }
    public abstract int SPE { get; set; }
    public abstract int SPA { get; set; }
    public abstract int SPD { get; set; }
    public abstract int EV_HP { get; set; }
    public abstract int EV_ATK { get; set; }
    public abstract int EV_DEF { get; set; }
    public abstract int EV_SPE { get; set; }
    public abstract int EV_SPA { get; set; }
    public abstract int EV_SPD { get; set; }
    public abstract int Type1 { get; set; }
    public abstract int Type2 { get; set; }
    public abstract int EggGroup1 { get; set; }
    public abstract int EggGroup2 { get; set; }
    public abstract int CatchRate { get; set; }
    public virtual int EvoStage { get; set; }
    public abstract int Gender { get; set; }
    public abstract int HatchCycles { get; set; }
    public abstract int BaseFriendship { get; set; }
    public abstract int EXPGrowth { get; set; }
    public abstract IReadOnlyList<int> Abilities { get; set; }
    public abstract int GetAbilityIndex(int abilityID);
    public abstract int EscapeRate { get; set; }
    public virtual byte FormCount { get; set; } = 1;
    public virtual int FormStatsIndex { get; set; }
    public abstract int BaseEXP { get; set; }
    public abstract int Color { get; set; }
    public virtual int Height { get; set; }
    public virtual int Weight { get; set; }

    /// <summary>
    /// TM/HM learn compatibility flags for individual moves.
    /// </summary>
    public bool[] TMHM = Array.Empty<bool>();

    /// <summary>
    /// Grass-Fire-Water-Etc typed learn compatibility flags for individual moves.
    /// </summary>
    public bool[] TypeTutors = Array.Empty<bool>();

    /// <summary>
    /// Special tutor learn compatibility flags for individual moves.
    /// </summary>
    public bool[][] SpecialTutors = Array.Empty<bool[]>();

    protected static bool[] GetBits(ReadOnlySpan<byte> data)
    {
        bool[] result = new bool[data.Length << 3];
        for (int i = result.Length - 1; i >= 0; i--)
            result[i] = ((data[i >> 3] >> (i & 7)) & 0x1) == 1;
        return result;
    }

    protected static void SetBits(ReadOnlySpan<bool> bits, Span<byte> data)
    {
        for (int i = bits.Length - 1; i >= 0; i--)
            data[i>>3] |= (byte)(bits[i] ? 1 << (i&0x7) : 0);
    }

    public void AddTMHM(ReadOnlySpan<byte> data) => TMHM = GetBits(data);
    public void AddTypeTutors(ReadOnlySpan<byte> data) => TypeTutors = GetBits(data);

    public int FormIndex(ushort species, byte form)
    {
        if (!HasForm(form))
            return species;
        return FormStatsIndex + form - 1;
    }

    public bool HasForm(byte form)
    {
        if (form == 0) // no form requested
            return false;
        if (FormStatsIndex <= 0) // no forms present
            return false;
        if (form >= FormCount) // beyond range of species' forms
            return false;
        return true;
    }

    public const int RatioMagicGenderless = 255;
    public const int RatioMagicFemale = 254;
    public const int RatioMagicMale = 0;

    public static bool IsSingleGender(int gt) => (uint)(gt - 1) >= 253;

    public bool IsDualGender => (uint)(Gender - 1) < 253;
    public bool Genderless => Gender == RatioMagicGenderless;
    public bool OnlyFemale => Gender == RatioMagicFemale;
    public bool OnlyMale => Gender == RatioMagicMale;

    public bool HasForms => FormCount > 1;

    /// <summary>
    /// Checks to see if the <see cref="PKM.Form"/> is valid within the <see cref="FormCount"/>
    /// </summary>
    /// <param name="form"></param>
    public bool IsFormWithinRange(byte form)
    {
        if (form == 0)
            return true;
        return form < FormCount;
    }
}

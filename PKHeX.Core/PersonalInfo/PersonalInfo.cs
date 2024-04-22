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
    public abstract byte Type1 { get; set; }
    public abstract byte Type2 { get; set; }
    public abstract int EggGroup1 { get; set; }
    public abstract int EggGroup2 { get; set; }
    public abstract byte CatchRate { get; set; }
    public virtual int EvoStage { get; set; }
    public abstract byte Gender { get; set; }
    public abstract byte HatchCycles { get; set; }
    public abstract byte BaseFriendship { get; set; }
    public abstract byte EXPGrowth { get; set; }
    public abstract int GetIndexOfAbility(int abilityID);
    public abstract int GetAbilityAtIndex(int abilityIndex);
    public abstract int AbilityCount { get; }
    public abstract int EscapeRate { get; set; }
    public virtual byte FormCount { get; set; } = 1;
    public virtual int FormStatsIndex { get; set; }
    public abstract int BaseEXP { get; set; }
    public abstract int Color { get; set; }
    public virtual int Height { get; set; }
    public virtual int Weight { get; set; }

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

    public const byte RatioMagicGenderless = 255;
    public const byte RatioMagicFemale = 254;
    public const byte RatioMagicMale = 0;

    public static bool IsSingleGender(byte gt) => gt - 1u >= 253;

    public bool IsDualGender => Gender - 1u < 253;
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

    /// <summary>
    /// Gets the span of values for a given Gender
    /// </summary>
    /// <param name="gender">Gender</param>
    /// <param name="ratio">Gender Ratio</param>
    /// <returns>Returns the maximum or minimum gender value that corresponds to the input gender ratio.</returns>
    public static (byte Min, byte Max) GetGenderMinMax(byte gender, byte ratio) => ratio switch
    {
        RatioMagicMale => (0, 255),
        RatioMagicFemale => (0, 255),
        RatioMagicGenderless => (0, 255),
        _ => gender switch
        {
            0 => (ratio, 255), // male
            1 => (0, --ratio), // female
            _ => (0, 255),
        },
    };
}

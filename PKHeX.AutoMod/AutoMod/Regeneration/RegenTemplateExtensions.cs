using System;
using System.Collections.Generic;
using System.Linq;

namespace PKHeX.Core.AutoMod;

public static class RegenTemplateExtensions
{
    public static void SanitizeForm(this RegenTemplate set, byte gen)
    {
        //Zygarde can only be forms 2 or 3 until HOME compatibility
        if (set.Species == (ushort)Species.Zygarde && set.Context == EntityContext.Gen9a)
            set.Form = set.Form == 0 ? (byte)3 : set.Form == 1 ? (byte)2 : set.Form;
        // Scatterbug must be Meadow, Spewpa must be Meadow or Marine (M&Ms only) until HOME compatibility
        if (set.Context == EntityContext.Gen9a && ((set.Species == (ushort)Species.Scatterbug && set.Form is not 4) || (set.Species == (ushort)Species.Spewpa && set.Form is not 8 && set.Form is not 4) || (set.Species == (ushort)Species.Vivillon && set.Form is not 8 && set.Form is not 4)))
            set.Form = 6;
        
        if (!FormInfo.IsBattleOnlyForm(set.Species, set.Form, gen))
            return;

        set.Form = FormInfo.GetOutOfBattleForm(set.Species, set.Form, gen);
    }

    /// <summary>
    /// Showdown quirks lets you have battle only moves in battle only forms. Transform back to base move.
    /// </summary>
    public static void SanitizeBattleMoves(this IBattleTemplate set)
    {
        switch (set.Species)
        {
            case (ushort)Species.Zacian:
            case (ushort)Species.Zamazenta:
                {
                    // Behemoth Blade and Behemoth Bash -> Iron Head
                    // Duplicate moves aren't worth considering, so just replace once for each.
                    var moves = set.Moves.AsSpan();
                    var indexBlade = moves.IndexOf<ushort>(781);
                    if (indexBlade != -1)
                        moves[indexBlade] = 442;
                    var indexBash = moves.IndexOf<ushort>(782);
                    if (indexBash != -1)
                        moves[indexBash] = 442;
                    break;
                }
        }
    }

    /// <summary>
    /// TeraType restrictions being fixed before the set is even generated
    /// </summary>
    public static void SanitizeTeraTypes(this RegenTemplate set)
    {
        if (set.Species == (int)Species.Ogerpon && !TeraTypeUtil.IsValidOgerpon((byte)set.TeraType, set.Form))
            set.TeraType = ShowdownEdits.GetValidOpergonTeraType(set.Form);
    }

    /// <summary>
    /// General method to preprocess sets excluding invalid forms. (handled in a future method)
    /// </summary>
    /// <param name="set">Showdown set passed to the function</param>
    /// <param name="personal">Personal data for the desired form</param>
    public static void FixGender(this RegenTemplate set, PersonalInfo personal)
    {
        if (personal.OnlyFemale && set.Gender != 1)
            set.Gender = 1;
        else if (personal.OnlyMale && set.Gender != 0)
            set.Gender = 0;
        else if (personal.Genderless && set.Gender != 2)
            set.Gender = 2;
    }

    public static string GetRegenText(this PKM pk) => pk.Species == 0 ? string.Empty : new RegenTemplate(pk).Text;

    public static IEnumerable<string> GetRegenSets(this Span<PKM> data) => data.ToArray().Where(p => p.Species != 0).Select(GetRegenText);

    public static string GetRegenSets(this Span<PKM> data, string separator) => string.Join(separator, data.GetRegenSets());
}

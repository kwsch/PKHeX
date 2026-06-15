using System;

namespace PKHeX.Core;

/// <summary>
/// Utility to create blank <see cref="PKM"/> instances.
/// </summary>
public static class EntityBlank
{
    /// <summary>
    /// Gets a Blank <see cref="PKM"/> object of the specified type.
    /// </summary>
    /// <param name="type">Type of <see cref="PKM"/> instance desired.</param>
    /// <returns>New instance of a blank <see cref="PKM"/> object.</returns>
    public static PKM GetBlank(Type type) => GetBlank(type.Name);

    /// <inheritdoc cref="GetBlank(Type)"/>
    public static PKM GetBlank(ReadOnlySpan<char> type) => type switch
    {
        nameof(PK1) => new PK1(),
        nameof(PK2) => new PK2(),
        nameof(SK2) => new SK2(),
        nameof(PK3) => new PK3(),
        nameof(CK3) => new CK3(),
        nameof(XK3) => new XK3(),
        nameof(PK4) => new PK4(),
        nameof(BK4) => new BK4(),
        nameof(RK4) => new RK4(),
        nameof(PK5) => new PK5(),
        nameof(PK6) => new PK6(),
        nameof(PK7) => new PK7(),
        nameof(PB7) => new PB7(),
        nameof(PK8) => new PK8(),
        nameof(PA8) => new PA8(),
        nameof(PB8) => new PB8(),
        nameof(PK9) => new PK9(),
        nameof(PA9) => new PA9(),
        nameof(PKH) => new PKH(),
        _ => throw new ArgumentOutOfRangeException(nameof(type), type.ToString(), null),
    };

    /// <summary>
    /// Gets a Blank <see cref="PKM"/> object compatible with the provided inputs.
    /// </summary>
    /// <param name="context">The context of the entity.</param>
    /// <param name="language">The language of the entity. Only used for Gen 1 Japanese PKM, otherwise ignored.</param>
    /// <returns>A blank <see cref="PKM"/> object.</returns>
    public static PKM GetBlank(EntityContext context, LanguageID language = LanguageID.None) => context switch
    {
        EntityContext.Gen1 => new PK1(language == LanguageID.Japanese),
        EntityContext.Gen7b => new PB7(),
        EntityContext.Gen8b => new PB8(),
        EntityContext.Gen8a => new PA8(),
        EntityContext.Gen9a => new PA9(),
        _ => GetBlank(context.Generation),
    };

    /// <summary>
    /// Gets a Blank <see cref="PKM"/> object compatible with the provided inputs.
    /// </summary>
    public static PKM GetBlank(ITrainerInfo tr)
    {
        if (tr is SaveFile s)
            return s.BlankPKM;
        return GetBlank(tr.Context, (LanguageID)tr.Language);
    }

    /// <inheritdoc cref="GetBlank(ITrainerInfo)"/>
    public static PKM GetBlank(byte gen) => gen switch
    {
        1 => new PK1(),
        2 => new PK2(),
        3 => new PK3(),
        4 => new PK4(),
        5 => new PK5(),
        6 => new PK6(),
        7 => new PK7(),
        8 => new PK8(),
        9 => new PK9(),
        _ => throw new ArgumentOutOfRangeException(nameof(gen), gen, null),
    };

    public static PKM GetIdealBlank(ushort species, byte form)
    {
        if (PersonalTable.LA.IsPresentInGame(species, form))
            return new PA8();
        if (PersonalTable.SWSH.IsPresentInGame(species, form))
            return new PK8();
        if (PersonalTable.BDSP.IsPresentInGame(species, form))
            return new PB8();
        if (PersonalTable.USUM.IsPresentInGame(species, form))
            return new PK7();
        if (PersonalTable.SV.IsPresentInGame(species, form))
            return new PK9();
        if (PersonalTable.ZA.IsPresentInGame(species, form))
            return new PA9();
        return new PB7();
    }
}

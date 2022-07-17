using System;
using System.Linq;
using System.Reflection;

namespace PKHeX.Core;

/// <summary>
/// Reflection utility to create blank <see cref="PKM"/> without specifying a constructor.
/// </summary>
public static class EntityBlank
{
    /// <summary>
    /// Gets a Blank <see cref="PKM"/> object of the specified type.
    /// </summary>
    /// <param name="type">Type of <see cref="PKM"/> instance desired.</param>
    /// <returns>New instance of a blank <see cref="PKM"/> object.</returns>
    public static PKM GetBlank(Type type)
    {
        var constructors = type.GetTypeInfo().DeclaredConstructors.Where(z => !z.IsStatic);
        var argCount = constructors.Min(z => z.GetParameters().Length);
        return (PKM)Activator.CreateInstance(type, new object[argCount]);
    }

    public static PKM GetBlank(int gen, GameVersion ver) => gen switch
    {
        1 when ver == GameVersion.BU => new PK1(true),
        7 when GameVersion.Gen7b.Contains(ver) => new PB7(),
        8 when GameVersion.BDSP.Contains(ver) => new PB8(),
        8 when GameVersion.PLA == ver => new PA8(),
        _ => GetBlank(gen),
    };

    /// <summary>
    /// Gets a Blank <see cref="PKM"/> object compatible with the provided inputs.
    /// </summary>
    public static PKM GetBlank(ITrainerInfo tr)
    {
        if (tr is SaveFile s)
            return s.BlankPKM;
        return GetBlank(tr.Generation, tr.Game);
    }

    /// <inheritdoc cref="GetBlank(ITrainerInfo)"/>
    public static PKM GetBlank(int gen, int ver) => GetBlank(gen, (GameVersion)ver);

    /// <inheritdoc cref="GetBlank(ITrainerInfo)"/>
    public static PKM GetBlank(int gen)
    {
        var type = Type.GetType($"PKHeX.Core.PK{gen}");
        if (type is null)
            throw new InvalidCastException($"Unable to get the type for PK{gen}.");

        return GetBlank(type);
    }

    public static PKM GetIdealBlank(int species, int form)
    {
        if (PersonalTable.LA.IsPresentInGame(species, form))
            return new PA8();
        if (PersonalTable.SWSH.IsPresentInGame(species, form))
            return new PK8();
        if (PersonalTable.BDSP.IsPresentInGame(species, form))
            return new PB8();
        if (PersonalTable.USUM.IsPresentInGame(species, form))
            return new PK7();
        return new PB7();
    }
}

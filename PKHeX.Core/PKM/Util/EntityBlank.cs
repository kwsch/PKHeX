using System;
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
        var typeInfo = type.GetTypeInfo();
        return GetBlank(typeInfo);
    }

    /// <inheritdoc cref="GetBlank(Type)"/>
    public static PKM GetBlank(TypeInfo type)
    {
        // Not all derived types have a parameter-less constructor, so find the minimal constructor and use that.
        ConstructorInfo? info = null;
        int count = int.MaxValue;
        foreach (var ctor in type.DeclaredConstructors)
        {
            if (ctor.IsStatic)
                continue;
            var parameters = ctor.GetParameters();
            int length = parameters.Length;
            if (length >= count)
                continue;
            count = length;
            info = ctor;
        }

        ArgumentNullException.ThrowIfNull(info);
        var result = info.Invoke(new object?[count]);
        if (result is not PKM x)
            throw new InvalidCastException($"Unable to cast {result} to {typeof(PKM)}");
        return x;
    }

    public static PKM GetBlank(byte gen, GameVersion version) => gen switch
    {
        1 when version is GameVersion.BU => new PK1(true),
        7 when version is GameVersion.GP or GameVersion.GE => new PB7(),
        8 when version is GameVersion.BD or GameVersion.SP => new PB8(),
        8 when version is GameVersion.PLA => new PA8(),
        9 when version is GameVersion.ZA => new PA9(),
        _ => GetBlank(gen),
    };

    /// <summary>
    /// Gets a Blank <see cref="PKM"/> object compatible with the provided inputs.
    /// </summary>
    public static PKM GetBlank(ITrainerInfo tr)
    {
        if (tr is SaveFile s)
            return s.BlankPKM;
        return GetBlank(tr.Generation, tr.Version);
    }

    /// <inheritdoc cref="GetBlank(ITrainerInfo)"/>
    public static PKM GetBlank(byte gen)
    {
        var type = Type.GetType($"PKHeX.Core.PK{gen}");
        ArgumentNullException.ThrowIfNull(type);

        return GetBlank(type);
    }

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

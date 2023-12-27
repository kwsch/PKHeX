namespace PKHeX.Core;

/// <summary>
/// Interface that exposes an indication of the Pokémon's Tera Type.
/// </summary>
public interface ITeraType : ITeraTypeReadOnly
{
    /// <summary> Tera Type value the entity is originally encountered with. </summary>
    MoveType TeraTypeOriginal { get; set; }
    /// <summary> If the type was modified, this value will indicate accordingly. </summary>
    MoveType TeraTypeOverride { get; set; }
}

public interface ITeraTypeReadOnly
{
    /// <summary> Elemental type the Pokémon's Tera Type is. </summary>
    MoveType TeraType { get; }
}

/// <summary>
/// Logic for interacting with Tera Types.
/// </summary>
public static class TeraTypeUtil
{
    /// <summary>
    /// Magic value to indicate that a Tera Type has not been overriden from the original value.
    /// </summary>
    public const byte OverrideNone = 19;

    /// <summary>
    /// Magic value to indicate that a Tera Type is the Stellar type.
    /// </summary>
    public const byte Stellar = 99;

    /// <summary>
    /// Max amount of Tera Types possible. Range is [0,17].
    /// </summary>
    public const byte MaxType = 17;

    /// <summary>
    /// String resource index for the Stellar type.
    /// </summary>
    public const byte StellarTypeDisplayStringIndex = 18;

    /// <summary>
    /// For out of range values, we fall back to this Tera Type.
    /// </summary>
    private const MoveType Fallback = MoveType.Normal;

    /// <summary>
    /// Calculates the effective Tera Type based on the inputs.
    /// </summary>
    /// <param name="t">Entity to calculate for.</param>
    public static MoveType GetTeraType(this ITeraType t)
    {
        return GetTeraType((byte)t.TeraTypeOriginal, (byte)t.TeraTypeOverride);
    }

    /// <summary>
    /// Indicates if the Tera Type value is valid (changed from anything to anything).
    /// </summary>
    /// <param name="override">Current override value</param>
    /// <returns>True if valid.</returns>
    public static bool IsValid(byte @override) => @override is <= MaxType or OverrideNone or Stellar;

    /// <summary>
    /// Indicates if the Tera Type value is valid (changed to anything).
    /// </summary>
    /// <param name="override">Current override value</param>
    /// <returns>True if valid.</returns>
    public static bool IsOverrideValid(byte @override) => @override is <= MaxType or Stellar;

    /// <summary>
    /// Checks if Ogerpon's Tera Type is valid.
    /// </summary>
    /// <param name="type">Tera Type to check</param>
    /// <param name="form">Ogerpon's form</param>
    /// <returns>True if the Tera Type is valid.</returns>
    public static bool IsValidOgerpon(byte type, byte form) => (form & 3) switch
    {
        0 => type is (byte)MoveType.Grass or OverrideNone,
        1 => type is (byte)MoveType.Water,
        2 => type is (byte)MoveType.Fire,
        3 => type is (byte)MoveType.Rock,
        _ => false,
    };

    /// <summary>
    /// Checks if Terapagos' Tera Type is valid.
    /// </summary>
    /// <param name="type">Tera Type to check</param>
    /// <returns>True if the Tera Type is valid.</returns>
    public static bool IsValidTerapagos(byte type) => type == OverrideNone;

    /// <summary>
    /// Calculates the effective Tera Type based on the inputs.
    /// </summary>
    /// <param name="original">Unmodified Tera Type value initially encountered with.</param>
    /// <param name="override">If the type was modified, this value will indicate accordingly.</param>
    public static MoveType GetTeraType(byte original, byte @override)
    {
        if (IsOverrideValid(@override))
            return (MoveType)@override;
        if (@override != OverrideNone)
            return Fallback; // 18 or out of range.

        if (original <= Stellar)
            return (MoveType)original;
        return Fallback; // out of range.
    }

    /// <summary>
    /// Applies a new Tera Type value to the entity.
    /// </summary>
    /// <param name="t">Entity to set the value to.</param>
    /// <param name="type">Value to update with.</param>
    public static void SetTeraType(this ITeraType t, MoveType type)
    {
        if ((byte)type > Stellar)
            type = Fallback;

        var original = t.TeraTypeOriginal;
        if (original == type)
            t.TeraTypeOverride = (MoveType)OverrideNone;
        else
            t.TeraTypeOverride = type;
    }

    /// <summary>
    /// Applies a new Tera Type value to the entity.
    /// </summary>
    /// <param name="t">Entity to set the value to.</param>
    /// <param name="type">Value to update with.</param>
    public static void SetTeraType(this ITeraType t, byte type) => t.SetTeraType((MoveType)type);

    /// <summary>
    /// Gets the preferred Tera Type to set for the given <see cref="IPersonalType"/>.
    /// </summary>
    public static MoveType GetTeraTypeImport(byte type1, byte type2)
    {
        var type = (MoveType)type1;
        return type != (byte)MoveType.Normal ? type : (MoveType)type2;
    }

    /// <summary>
    /// Resets the <see cref="ITeraType.TeraTypeOriginal"/> and <see cref="ITeraType.TeraTypeOverride"/> values based on the <see cref="IEncounterTemplate"/> data.
    /// </summary>
    /// <param name="pk">Entity to check for</param>
    /// <param name="enc">Original encounter</param>
    public static void ResetTeraType(PK9 pk, IEncounterTemplate enc)
    {
        pk.TeraTypeOverride = enc is not ITeraType x ? (MoveType)OverrideNone : x.TeraTypeOverride; // WC9
        pk.TeraTypeOriginal = enc switch
        {
            ITeraTypeReadOnly t => t.TeraType,
            ITeraRaid9 t9 => (MoveType)Tera9RNG.GetTeraType(Tera9RNG.GetOriginalSeed(pk), t9.TeraType, enc.Species, enc.Form),
            _ => (MoveType)Tera9RNG.GetTeraTypeFromPersonal(enc.Species, enc.Form, Util.Rand.Rand64()),
        };
    }

    /// <summary>
    /// Checks if the given species can have its Tera Type changed.
    /// </summary>
    /// <param name="species">Species to check</param>
    /// <returns>True if the species can have its Tera Type changed.</returns>
    public static bool CanChangeTeraType(ushort species) => species is not ((int)Species.Ogerpon or (int)Species.Terapagos);
}

using System;

namespace PKHeX.Core;

/// <summary>
/// One edge/state in a successful chain-breeding proof.
/// </summary>
public readonly record struct ChainBreedStep : ISpeciesForm
{
    public ushort Species { get; }
    public byte Form { get; }

    public ChainBreedStepKind Kind { get; }
    public GameVersion Version { get; }
    public GameVersion OriginVersion { get; }

    public ushort MotherSpecies { get; private init; }
    public byte MotherForm { get; private init; }

    public ushort FatherSpecies { get; private init; }
    public byte FatherForm { get; private init; }

    public byte MoveCount { get; }
    public ushort Move1 { get; }
    public ushort Move2 { get; }
    public ushort Move3 { get; }
    public ushort Move4 { get; }

    private ChainBreedStep(ushort species, byte form, GameVersion version, GameVersion originVersion, ChainBreedStepKind kind,
        ushort motherSpecies, ushort fatherSpecies, byte motherForm, byte fatherForm, scoped ReadOnlySpan<ushort> moves)
    {
        Species = species;
        Form = form;
        Version = version;
        OriginVersion = originVersion;
        Kind = kind;
        MotherSpecies = motherSpecies;
        MotherForm = motherForm;
        FatherSpecies = fatherSpecies;
        FatherForm = fatherForm;
        MoveCount = (byte)moves.Length;
        Move1 = moves.Length > 0 ? moves[0] : (ushort)0;
        Move2 = moves.Length > 1 ? moves[1] : (ushort)0;
        Move3 = moves.Length > 2 ? moves[2] : (ushort)0;
        Move4 = moves.Length > 3 ? moves[3] : (ushort)0;
    }

    /// <summary>
    /// Creates a new <see cref="ChainBreedStep"/> representing a Pokémon that was bred in the same game version, including its species, form, version, and moves.
    /// </summary>
    /// <param name="species">The species of the Pokémon.</param>
    /// <param name="form">The form of the Pokémon.</param>
    /// <param name="version">The game version where the breeding occurs.</param>
    /// <param name="moves">The moves of the Pokémon.</param>
    /// <returns>A <see cref="ChainBreedStep"/> representing the bred Pokémon.</returns>
    internal static ChainBreedStep CreateEgg(ushort species, byte form, GameVersion version, scoped ReadOnlySpan<ushort> moves)
        => new(species, form, version, version, ChainBreedStepKind.Breed, 0, 0, 0, 0, moves);

    /// <summary>
    /// Creates a new <see cref="ChainBreedStep"/> with the specified parent species and forms, while keeping the other properties unchanged.
    /// </summary>
    /// <param name="motherSpecies">The species of the mother Pokémon.</param>
    /// <param name="motherForm">The form of the mother Pokémon.</param>
    /// <param name="fatherSpecies">The species of the father Pokémon.</param>
    /// <param name="fatherForm">The form of the father Pokémon.</param>
    /// <returns>A new <see cref="ChainBreedStep"/> with the specified parent species and forms.</returns>
    internal ChainBreedStep WithParents(ushort motherSpecies, byte motherForm, ushort fatherSpecies, byte fatherForm)
        => this with { MotherSpecies = motherSpecies, FatherSpecies = fatherSpecies, MotherForm = motherForm, FatherForm = fatherForm };

    /// <summary>
    /// Saves the information of a Pokémon that was transferred from one game version to another, including its species, form, destination version, origin version, and moves.
    /// </summary>
    /// <param name="species">The species of the Pokémon.</param>
    /// <param name="form">The form of the Pokémon.</param>
    /// <param name="destination">The destination game version.</param>
    /// <param name="origin">The origin game version where the transfer occurs.</param>
    /// <param name="moves">The moves of the Pokémon at the time of transfer.</param>
    /// <returns>A <see cref="ChainBreedStep"/> representing the transfer.</returns>
    internal static ChainBreedStep CreateTransfer(ushort species, byte form, GameVersion destination, GameVersion origin, scoped ReadOnlySpan<ushort> moves)
        => new(species, form, destination, origin, ChainBreedStepKind.Transfer, 0, 0, 0, 0, moves);

    /// <summary>
    /// Compares the current <see cref="ChainBreedStep"/> with another <see cref="ChainBreedStep"/> for equality based on their properties.
    /// Skips the parent species and forms in the comparison, focusing only on the core properties of the step.
    /// </summary>
    /// <param name="other">The other <see cref="ChainBreedStep"/> to compare with.</param>
    /// <returns><c>true</c> if the current <see cref="ChainBreedStep"/> is equal to the other; otherwise, <c>false</c>.</returns>
    internal bool QueryEquals(in ChainBreedStep other)
        => Kind == other.Kind && Species == other.Species && Form == other.Form
           && Version == other.Version && OriginVersion == other.OriginVersion && MoveCount == other.MoveCount
           && Move1 == other.Move1 && Move2 == other.Move2 && Move3 == other.Move3 && Move4 == other.Move4;

    /// <summary>
    /// Copies the moves of the current <see cref="ChainBreedStep"/> into the provided destination span.
    /// The number of moves copied is determined by the <see cref="MoveCount"/> property.
    /// </summary>
    /// <param name="destination">The span where the moves will be copied to.</param>
    public void SetMoves(scoped Span<ushort> destination)
    {
        if (MoveCount > 0) destination[0] = Move1;
        if (MoveCount > 1) destination[1] = Move2;
        if (MoveCount > 2) destination[2] = Move3;
        if (MoveCount > 3) destination[3] = Move4;
    }
}

/// <summary>
/// The kind of step in a chain-breeding proof, indicating whether the Pokémon was bred or transferred from another game version.
/// </summary>
public enum ChainBreedStepKind : byte
{
    /// <summary>
    /// No step; this is the default empty value and is invalid for any real step in a chain-breeding proof.
    /// </summary>
    None = 0,

    /// <summary>
    /// A breeding step, indicating that the Pokémon was bred from two parents in the same game version.
    /// </summary>
    Breed,

    /// <summary>
    /// A transfer step, indicating that the Pokémon was transferred from one game version to another.
    /// </summary>
    Transfer,
}

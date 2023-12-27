using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace PKHeX.Core;

/// <summary>
/// Generation specific Evolution Tree data.
/// </summary>
/// <remarks>
/// Used to determine if a <see cref="PKM.Species"/> can evolve from prior steps in its evolution branch.
/// </remarks>
public sealed class EvolutionTree : EvolutionNetwork
{
    public const int MaxEvolutions = 3;
    public static readonly EvolutionTree Evolves1  = GetViaSpecies (PersonalTable.Y,    EvolutionSet.GetArray(GetReader("g1", "g1"u8)));
    public static readonly EvolutionTree Evolves2  = GetViaSpecies (PersonalTable.C,    EvolutionSet.GetArray(GetReader("g2", "g2"u8)));
    public static readonly EvolutionTree Evolves3  = GetViaSpecies (PersonalTable.RS,   EvolutionSet.GetArray(GetReader("g3", "g3"u8)));
    public static readonly EvolutionTree Evolves4  = GetViaSpecies (PersonalTable.DP,   EvolutionSet.GetArray(GetReader("g4", "g4"u8)));
    public static readonly EvolutionTree Evolves5  = GetViaSpecies (PersonalTable.BW,   EvolutionSet.GetArray(GetReader("g5", "g5"u8)));
    public static readonly EvolutionTree Evolves6  = GetViaSpecies (PersonalTable.AO,   EvolutionSet.GetArray(GetReader("g6", "g6"u8)));
    public static readonly EvolutionTree Evolves7  = GetViaPersonal(PersonalTable.USUM, EvolutionSet.GetArray(GetReader("uu", "uu"u8)));
    public static readonly EvolutionTree Evolves7b = GetViaPersonal(PersonalTable.GG,   EvolutionSet.GetArray(GetReader("gg", "gg"u8)));
    public static readonly EvolutionTree Evolves8  = GetViaPersonal(PersonalTable.SWSH, EvolutionSet.GetArray(GetReader("ss", "ss"u8)));
    public static readonly EvolutionTree Evolves8a = GetViaPersonal(PersonalTable.LA,   EvolutionSet.GetArray(GetReader("la", "la"u8), 0));
    public static readonly EvolutionTree Evolves8b = GetViaPersonal(PersonalTable.BDSP, EvolutionSet.GetArray(GetReader("bs", "bs"u8)));
    public static readonly EvolutionTree Evolves9  = GetViaPersonal(PersonalTable.SV,   EvolutionSet.GetArray(GetReader("sv", "sv"u8)));

    private static ReadOnlySpan<byte> GetResource([ConstantExpected] string resource) => Util.GetBinaryResource($"evos_{resource}.pkl");
    private static BinLinkerAccessor GetReader([ConstantExpected] string resource, [Length(2, 2)] ReadOnlySpan<byte> identifier) => BinLinkerAccessor.Get(GetResource(resource), identifier);
    private EvolutionTree(IEvolutionForward forward, IEvolutionReverse reverse) : base(forward, reverse) { }

    private static EvolutionTree GetViaSpecies(IPersonalTable t, EvolutionMethod[][] entries)
    {
        var forward = new EvolutionForwardSpecies(entries);
        var reverse = new EvolutionReverseSpecies(entries, t);
        return new EvolutionTree(forward, reverse);
    }

    private static EvolutionTree GetViaPersonal(IPersonalTable t, EvolutionMethod[][] entries)
    {
        var forward = new EvolutionForwardPersonal(entries, t);
        var reverse = new EvolutionReversePersonal(entries, t);
        return new EvolutionTree(forward, reverse);
    }

    /// <summary>
    /// Get the <see cref="EvolutionTree"/> for the given <see cref="EntityContext"/>.
    /// </summary>
    /// <param name="context">Context to get the <see cref="EvolutionTree"/> for.</param>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public static EvolutionTree GetEvolutionTree(EntityContext context) => context switch
    {
        EntityContext.Gen1 => Evolves1,
        EntityContext.Gen2 => Evolves2,
        EntityContext.Gen3 => Evolves3,
        EntityContext.Gen4 => Evolves4,
        EntityContext.Gen5 => Evolves5,
        EntityContext.Gen6 => Evolves6,
        EntityContext.Gen7 => Evolves7,
        EntityContext.Gen8 => Evolves8,
        EntityContext.Gen9 => Evolves9,
        EntityContext.Gen7b => Evolves7b,
        EntityContext.Gen8a => Evolves8a,
        EntityContext.Gen8b => Evolves8b,
        _ => throw new ArgumentOutOfRangeException(nameof(context), context, null),
    };
}

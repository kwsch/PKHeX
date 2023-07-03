using System;

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
    public static readonly EvolutionTree Evolves1  = GetViaSpecies (PersonalTable.Y,    EvolutionSet1.GetArray(GetResource("rby"), 151));
    public static readonly EvolutionTree Evolves2  = GetViaSpecies (PersonalTable.C,    EvolutionSet1.GetArray(GetResource("gsc"), 251));
    public static readonly EvolutionTree Evolves3  = GetViaSpecies (PersonalTable.RS,   EvolutionSet3.GetArray(GetResource("g3")));
    public static readonly EvolutionTree Evolves4  = GetViaSpecies (PersonalTable.DP,   EvolutionSet4.GetArray(GetResource("g4")));
    public static readonly EvolutionTree Evolves5  = GetViaSpecies (PersonalTable.BW,   EvolutionSet5.GetArray(GetResource("g5")));
    public static readonly EvolutionTree Evolves6  = GetViaSpecies (PersonalTable.AO,   EvolutionSet6.GetArray(GetReader("ao")));
    public static readonly EvolutionTree Evolves7  = GetViaPersonal(PersonalTable.USUM, EvolutionSet7.GetArray(GetReader("uu"), true));
    public static readonly EvolutionTree Evolves7SM= GetViaPersonal(PersonalTable.USUM, EvolutionSet7.GetArray(GetReader("sm"), true));
    public static readonly EvolutionTree Evolves7b = GetViaPersonal(PersonalTable.GG,   EvolutionSet7.GetArray(GetReader("gg"), false));
    public static readonly EvolutionTree Evolves8  = GetViaPersonal(PersonalTable.SWSH, EvolutionSet7.GetArray(GetReader("ss"), false));
    public static readonly EvolutionTree Evolves8a = GetViaPersonal(PersonalTable.LA,   EvolutionSet7.GetArray(GetReader("la"), true));
    public static readonly EvolutionTree Evolves8b = GetViaPersonal(PersonalTable.BDSP, EvolutionSet7.GetArray(GetReader("bs"), false));
    public static readonly EvolutionTree Evolves9  = GetViaPersonal(PersonalTable.SV,   EvolutionSet7.GetArray(GetReader("sv"), false));

    private static ReadOnlySpan<byte> GetResource(string resource) => Util.GetBinaryResource($"evos_{resource}.pkl");
    private static BinLinkerAccessor GetReader(string resource) => BinLinkerAccessor.Get(GetResource(resource), resource);
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

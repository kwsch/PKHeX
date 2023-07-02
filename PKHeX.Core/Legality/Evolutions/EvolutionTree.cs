using System;
using static PKHeX.Core.GameVersion;

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

    static EvolutionTree()
    {
        // Add in banned evolution data!
        Evolves7.FixEvoTreeSM();
        Evolves8.FixEvoTreeSS();
        Evolves8b.FixEvoTreeBS();
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

    private void FixEvoTreeSM()
    {
        // Sun/Moon lack Ultra's Kantonian evolution methods.
        static bool BanOnlySM(PKM pk) => pk is { IsUntraded: true, SM: true };
        BanEvo((int)Species.Raichu, 0, BanOnlySM);
        BanEvo((int)Species.Marowak, 0, BanOnlySM);
        BanEvo((int)Species.Exeggutor, 0, BanOnlySM);
    }

    private void FixEvoTreeSS()
    {
        // Gigantamax Pikachu, Meowth-0, and Eevee are prevented from evolving.
        // Raichu cannot be evolved to the Alolan variant at this time.
        static bool BanGmax(PKM pk) => pk is IGigantamax { CanGigantamax: true };
        BanEvo((int)Species.Raichu, 0, BanGmax);
        BanEvo((int)Species.Raichu, 1, pk => (pk is IGigantamax {CanGigantamax: true}) || pk.Version is (int)GO or >= (int)GP);
        BanEvo((int)Species.Persian, 0, BanGmax);
        BanEvo((int)Species.Persian, 1, BanGmax);
        BanEvo((int)Species.Perrserker, 0, BanGmax);

        BanEvo((int)Species.Exeggutor, 1, pk => pk.Version is (int)GO or >= (int)GP);
        BanEvo((int)Species.Marowak, 1, pk => pk.Version is (int)GO or >= (int)GP);
        BanEvo((int)Species.Weezing, 0, pk => pk.Version >= (int)SW);
        BanEvo((int)Species.MrMime, 0, pk => pk.Version >= (int)SW);

        foreach (var s in Forward.GetEvolutions((int)Species.Eevee, 0)) // Eeveelutions
            BanEvo(s.Species, s.Form, BanGmax);
    }

    private void FixEvoTreeBS()
    {
        BanEvo((int)Species.Glaceon, 0, pk => pk.CurrentLevel == pk.Met_Level); // Ice Stone is unreleased, requires Route 217 Ice Rock Level Up instead
        BanEvo((int)Species.Milotic, 0, pk => pk is IContestStatsReadOnly { CNT_Beauty: < 170 } || pk.CurrentLevel == pk.Met_Level); // Prism Scale is unreleased, requires 170 Beauty Level Up instead
    }
}

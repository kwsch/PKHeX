using System.Collections.Generic;

namespace PKHeX.Core;

/// <summary>
/// Calculated Information storage with properties useful for parsing the legality of the input <see cref="PKM"/>.
/// </summary>
public sealed class LegalInfo : IGeneration
{
    /// <summary>The <see cref="PKM"/> object used for comparisons.</summary>
    public readonly PKM Entity;

    /// <summary>The generation of games the <see cref="PKM"/> originated from.</summary>
    public int Generation { get; private set; }

    /// <summary>The matched Encounter details for the <see cref="PKM"/>. </summary>
    public IEncounterable EncounterMatch
    {
        get => _match;
        set
        {
            if (!ReferenceEquals(_match, EncounterInvalid.Default) && (value.LevelMin != _match.LevelMin || value.Species != _match.Species))
                _evochains = null; // clear if evo chain has the potential to be different
            _match = value;
            Parse.Clear();
        }
    }

    /// <summary>
    /// Original encounter data for the <see cref="Entity"/>.
    /// </summary>
    /// <remarks>
    /// Generation 1/2 <see cref="Entity"/> that are transferred forward to Generation 7 are restricted to new encounter details.
    /// By retaining their original match, more information can be provided by the parse.
    /// </remarks>
    public IEncounterable EncounterOriginal => EncounterOriginalGB ?? EncounterMatch;

    internal IEncounterable? EncounterOriginalGB;
    private IEncounterable _match = EncounterInvalid.Default;

    /// <summary>Top level Legality Check result list for the <see cref="EncounterMatch"/>.</summary>
    internal readonly List<CheckResult> Parse;

    private const int MoveCount = 4;
    public readonly MoveResult[] Relearn = new MoveResult[MoveCount];
    public readonly MoveResult[] Moves = new MoveResult[MoveCount];

    public EvolutionHistory EvoChainsAllGens => _evochains ??= EvolutionChain.GetEvolutionChainsAllGens(Entity, EncounterMatch);
    private EvolutionHistory? _evochains;

    /// <summary><see cref="RNG"/> related information that generated the <see cref="PKM.PID"/>/<see cref="PKM.IVs"/> value(s).</summary>
    public PIDIV PIDIV
    {
        get => _pidiv;
        internal set
        {
            _pidiv = value;
            PIDParsed = true;
        }
    }

    public bool PIDParsed { get; private set; }
    private PIDIV _pidiv;

    /// <summary>Indicates whether or not the <see cref="PIDIV"/> can originate from the <see cref="EncounterMatch"/>.</summary>
    /// <remarks>This boolean is true until all valid <see cref="PIDIV"/> encounters are tested, after which it is false.</remarks>
    public bool PIDIVMatches { get; internal set; } = true;

    /// <summary>Indicates whether or not the <see cref="PIDIV"/> can originate from the <see cref="EncounterMatch"/> with explicit <see cref="RNG"/> <see cref="Frame"/> matching.</summary>
    /// <remarks>This boolean is true until all valid <see cref="Frame"/> entries are tested for all possible <see cref="EncounterSlot"/> matches, after which it is false.</remarks>
    public bool FrameMatches { get; internal set; } = true;

    public LegalInfo(PKM pk, List<CheckResult> parse)
    {
        Entity = pk;
        Parse = parse;
        StoreMetadata(pk.Generation);
    }

    internal void StoreMetadata(int gen)
    {
        // We can call this method at the start for any Gen3+ encounter iteration.
        // We need to call this for each Gen1/2 encounter as Version is not stored for those origins.
        Generation = gen;
    }
}

using System.Collections.Generic;

namespace PKHeX.Core;

/// <summary>
/// Calculated Information storage with properties useful for parsing the legality of the input <see cref="PKM"/>.
/// </summary>
public sealed class LegalInfo : IGeneration
{
    /// <summary>The <see cref="PKM"/> object used for comparisons.</summary>
    public readonly PKM Entity;

    /// <summary>The generation of games the <see cref="Entity"/> originated from.</summary>
    public byte Generation { get; private set; }

    /// <summary>The matched Encounter details for the <see cref="Entity"/>. </summary>
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

    /// <summary>RNG related information that generated the <see cref="PKM.PID"/>/<see cref="PKM.IVs"/> value(s).</summary>
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

    public EncounterYieldFlag ManualFlag { get; internal set; }
    public bool FrameMatches => ManualFlag != EncounterYieldFlag.InvalidFrame;
    public bool PIDIVMatches => ManualFlag != EncounterYieldFlag.InvalidPIDIV;

    public LegalInfo(PKM pk, List<CheckResult> parse)
    {
        Entity = pk;
        Parse = parse;
        StoreMetadata(pk.Generation);
    }

    /// <summary>
    /// We can call this method at the start for any Gen3+ encounter iteration.
    /// Additionally, We need to call this for each Gen1/2 encounter as Version is not stored for those origins.
    /// </summary>
    /// <param name="generation">Encounter generation</param>
    internal void StoreMetadata(byte generation) => Generation = generation switch
    {
        0 when Entity is PK9 { IsUnhatchedEgg: true } => 9,
        _ => generation,
    };
}

public enum EncounterYieldFlag : byte
{
    None = 0,
    InvalidPIDIV,
    InvalidFrame,
}

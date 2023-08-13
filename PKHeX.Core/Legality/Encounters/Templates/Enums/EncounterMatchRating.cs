using System.Collections;
using System.Collections.Generic;

namespace PKHeX.Core;

/// <summary>
/// Enumerates encounter match quality.
/// </summary>
public enum EncounterMatchRating : ushort
{
    /// <summary> Matches all data, no other matches will be better. </summary>
    Match,

    /// <summary> Matches most data, might have a better match later. </summary>
    Deferred,

    /// <summary> Matches most data, might have a better match later. Less preferred than <see cref="Deferred"/> due to small errors in secondary data. </summary>
    DeferredErrors,

    /// <summary> Matches some data, but will likely have a better match later. </summary>
    PartialMatch,

    /// <summary> Unused -- only used as an initial "max" value that anything else will be more suitable of a match. </summary>
    MaxNotMatch,
}

public readonly record struct MatchedEncounter<T>(T Encounter, EncounterMatchRating Rating);

public static class EncounterMatchUtil
{
    public static EncounterEnumerator<T> Enumerate<T>(this IReadOnlyList<T> encounters, EvoCriteria[] chain, PKM pk)
        where T : IEncounterMatch, IEncounterable
    {
        return new EncounterEnumerator<T>(encounters, chain, pk);
    }

    public struct EncounterEnumerator<T> : IEnumerator<MatchedEncounter<T>> where T : IEncounterMatch, IEncounterable
    {
        private readonly IReadOnlyList<T> _encounters;
        private readonly EvoCriteria[] _chain;
        private int _index = 0;
        private readonly PKM _pk;

        public EncounterEnumerator(IReadOnlyList<T> encounters, EvoCriteria[] chain, PKM pk)
        {
            _encounters = encounters;
            _chain = chain;
            _pk = pk;
        }

        public MatchedEncounter<T> Current { get; private set; } = default!;
        readonly object IEnumerator.Current => Current;

        public bool MoveNext()
        {
            for (; _index < _encounters.Count;)
            {
                var enc = _encounters[_index++];
                foreach (var evo in _chain)
                {
                    if (enc.Species != evo.Species)
                        continue;

                    var exact = enc.IsMatchExact(_pk, evo);
                    if (!exact)
                        break;

                    Current = new(enc, enc.GetMatchRating(_pk));
                    return true;
                }
            }

            return false;
        }

        public void Reset() => _index = 0;
        public readonly void Dispose() { }
        public readonly IEnumerator<MatchedEncounter<T>> GetEnumerator() => this;
    }
}

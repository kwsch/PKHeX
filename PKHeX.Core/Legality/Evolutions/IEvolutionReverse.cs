using System;
using System.Collections.Generic;

namespace PKHeX.Core;

public interface IEvolutionReverse
{
    EvolutionNode GetReverse(ushort species, byte form);
    EvolutionReverseLookup Lineage { get; }

    /// <summary>
    /// Gets all species the <see cref="species"/>-<see cref="form"/> can evolve from, yielded in order of increasing evolution stage.
    /// </summary>
    /// <param name="species">Species ID</param>
    /// <param name="form">Form ID</param>
    /// <returns>Enumerable of species IDs (with the Form IDs included, left shifted by 11).</returns>
    IEnumerable<(ushort Species, byte Form)> GetPreEvolutions(ushort species, byte form);
    void BanEvo(ushort species, byte form, Func<PKM, bool> func);
}

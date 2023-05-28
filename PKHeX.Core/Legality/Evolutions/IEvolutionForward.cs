using System;
using System.Collections.Generic;

namespace PKHeX.Core;

public interface IEvolutionForward
{
    ReadOnlyMemory<EvolutionMethod> GetForward(ushort species, byte form);

    /// <summary>
    /// Gets all species the <see cref="species"/>-<see cref="form"/> can evolve to, yielded in order of increasing evolution stage.
    /// </summary>
    /// <param name="species">Species ID</param>
    /// <param name="form">Form ID</param>
    /// <returns>Enumerable of species IDs (with the Form IDs included, left shifted by 11).</returns>
    IEnumerable<(ushort Species, byte Form)> GetEvolutions(ushort species, byte form);
}

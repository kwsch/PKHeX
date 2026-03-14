using System.Collections.Generic;

namespace PKHeX.Core;

/// <summary>
/// Large-block properties common to RS & Emerald save files.
/// </summary>
public interface ISaveBlock3LargeHoenn : ISaveBlock3Large
{
    PokeBlock3Case PokeBlocks { get; set; }
    ushort GetBerryBlenderRPMRecord(int index);
    void SetBerryBlenderRPMRecord(int index, ushort value);
    DecorationInventory3 Decorations { get; }
    Swarm3 Swarm { get; set; }

    IReadOnlyList<Swarm3> DefaultSwarms { get; }
    int SwarmIndex { get; set; }

    RecordMixing3Gift RecordMixingGift { get; set; }
    SecretBaseManager3 SecretBases { get; }

    Paintings3 GetPainting(int index, bool japanese);
    void SetPainting(int index, Paintings3 value);
}

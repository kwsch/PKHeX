namespace PKHeX.Core;

/// <summary>
/// General interface for describing a Species/Form in general.
/// </summary>
public interface IPersonalInfo : IBaseStat, IEffortValueYield, IGenderDetail, IPersonalFormInfo, IPersonalAbility, IPersonalEgg, IPersonalEncounter, IPersonalType, IPersonalMisc
{
    /// <summary>
    /// Writes entry to raw bytes.
    /// </summary>
    byte[] Write();

    /// <summary>
    /// Gender Ratio value determining if the entry is a fixed gender or bi-gendered.
    /// </summary>
    byte Gender { get; set; }

    /// <summary>
    /// Experience-Level Growth Rate type
    /// </summary>
    byte EXPGrowth { get; set; }
}

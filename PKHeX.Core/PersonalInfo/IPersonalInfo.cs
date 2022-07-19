namespace PKHeX.Core;

public interface IPersonalInfo : IBaseStat, IEffortValueYield, IGenderDetail, IPersonalFormInfo, IPersonalAbility, IPersonalEgg, IPersonalEncounter, IPersonalType, IPersonalMisc
{
    /// <summary>
    /// Writes entry to raw bytes.
    /// </summary>
    byte[] Write();

    /// <summary>
    /// Gender Ratio value determining if the entry is a fixed gender or bi-gendered.
    /// </summary>
    int Gender { get; set; }

    /// <summary>
    /// Experience-Level Growth Rate type
    /// </summary>
    int EXPGrowth { get; set; }
}

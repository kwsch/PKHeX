namespace PKHeX.Core;

/// <summary>
/// Stores IVs in separate properties.
/// </summary>
public interface ISeparateIVs
{
    byte IV_HP { get; set; }
    byte IV_ATK { get; set; }
    byte IV_DEF { get; set; }
    byte IV_SPA { get; set; }
    byte IV_SPD { get; set; }
    byte IV_SPE { get; set; }
}

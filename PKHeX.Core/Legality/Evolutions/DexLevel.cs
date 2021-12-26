namespace PKHeX.Core;

/// <summary>
/// Small general purpose value passing object with misc data pertaining to an encountered Species.
/// </summary>
public record DexLevel(int Species, int Form) : ISpeciesForm
{
    /// <summary>
    /// Maximum Level
    /// </summary>
    public int Level { get; set; }

    public override string ToString() => $"{(Species)Species}{(Form == 0 ? "" : $"-{Form}")} [{Level}]";
}

namespace PKHeX.Core;

/// <summary>
/// Small general purpose value passing object with misc data pertaining to an encountered Species.
/// </summary>
public record DexLevel(int Species, int Form) : ISpeciesForm
{
    public DexLevel(DexLevel source)
    {
        Species = source.Species;
        Form = source.Form;
    }
    /// <summary>
    /// Maximum Level
    /// </summary>
    public int Level { get; set; }

    public override string ToString() => $"{(Species)Species}{(Form == 0 ? "" : $"-{Form}")} [{Level}]";
}

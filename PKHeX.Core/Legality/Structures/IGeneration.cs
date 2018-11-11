namespace PKHeX.Core
{
    /// <summary>
    /// Interface that exposes a Generation value for the object.
    /// </summary>
    public interface IGeneration
    {
        int Generation { get; set; }
    }

    public static partial class Extensions
    {
        internal static PKM GetBlank(this IGeneration gen) => PKMConverter.GetBlank(gen.Generation);
    }
}

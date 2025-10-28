namespace PKHeX.Core;
/// <summary>
/// Interface that exposes Amie stats for a Pokémon.
/// </summary>
public interface IFullnessEnjoyment
{
    /// <summary> Fullness value of the Pokémon. </summary>
    byte Fullness { get; set; }
    /// <summary> Enjoyment value of the Pokémon. </summary>
    byte Enjoyment { get; set; }
}

using PKHeX.Core;

namespace PKHeX.Web.State;

/// <summary>
/// Where a Pokémon's experience points sit on its species' growth curve: the level Core derives from them and the range that level spans.
/// </summary>
/// <param name="Level">The level Core derives from <paramref name="Experience"/>.</param>
/// <param name="Experience">The stored experience points.</param>
/// <param name="LevelMinimum">The fewest experience points for <paramref name="Level"/>, which a level edit sets.</param>
/// <param name="NextLevel">The experience points that reach the next level, or null at the maximum level.</param>
/// <param name="Maximum">The most experience points the growth rate counts: the threshold of the maximum level.</param>
public sealed record LevelProgress(byte Level, uint Experience, uint LevelMinimum, uint? NextLevel, uint Maximum)
{
    /// <summary>The progress of <paramref name="pk"/> on the growth curve of its species and form.</summary>
    /// <remarks>Stored experience points above <see cref="Maximum"/> read as the maximum level, as Core reads them.</remarks>
    public static LevelProgress Of(PKM pk)
    {
        var growth = pk.PersonalInfo.EXPGrowth;
        var level = Core.Experience.GetLevel(pk.EXP, growth);
        uint? next = level < Core.Experience.MaxLevel ? Core.Experience.GetEXP((byte)(level + 1), growth) : null;
        return new LevelProgress(level, pk.EXP, Core.Experience.GetEXP(level, growth), next, Core.Experience.GetEXP(Core.Experience.MaxLevel, growth));
    }
}

using System.Collections.Generic;
using System.Linq;

namespace PKHeX.Core;

/// <summary>
/// Contains many <see cref="ITrainerInfo"/> instances to match against a <see cref="GameVersion"/>.
/// </summary>
public sealed class TrainerDatabase
{
    private readonly Dictionary<GameVersion, List<ITrainerInfo>> Database = [];

    /// <summary>
    /// Fetches an appropriate trainer based on the requested <see cref="version"/>.
    /// </summary>
    /// <param name="version">Version the trainer should originate from</param>
    /// <param name="language">Language to request for</param>
    /// <returns>Null if no trainer found for this version.</returns>
    public ITrainerInfo? GetTrainer(int version, LanguageID? language = null) => GetTrainer((GameVersion)version, language);

    /// <summary>
    /// Fetches an appropriate trainer based on the requested <see cref="ver"/>.
    /// </summary>
    /// <param name="ver">Version the trainer should originate from</param>
    /// <param name="language">Language to request for</param>
    /// <returns>Null if no trainer found for this version.</returns>
    public ITrainerInfo? GetTrainer(GameVersion ver, LanguageID? language = null)
    {
        if (ver <= 0)
            return null;

        if (!ver.IsValidSavedVersion())
            return GetTrainerFromGroup(ver, language);

        if (Database.TryGetValue(ver, out var list))
            return GetRandomChoice(list);

        return null;
    }

    private static T GetRandomChoice<T>(IReadOnlyList<T> list)
    {
        if (list.Count == 1)
            return list[0];
        return list[Util.Rand.Next(list.Count)];
    }

    /// <summary>
    /// Fetches an appropriate trainer based on the requested <see cref="ver"/> group.
    /// </summary>
    /// <param name="ver">Version the trainer should originate from</param>
    /// <param name="lang">Language to request for</param>
    /// <returns>Null if no trainer found for this version.</returns>
    private ITrainerInfo? GetTrainerFromGroup(GameVersion ver, LanguageID? lang = null)
    {
        var possible = Database.Where(z => ver.Contains(z.Key)).ToList();
        if (lang != null)
        {
            possible = possible.Select(z =>
            {
                var filtered = z.Value.Where(x => x.Language == (int)lang).ToList();
                return new KeyValuePair<GameVersion, List<ITrainerInfo>>(z.Key, filtered);
            }).Where(z => z.Value.Count != 0).ToList();
        }
        return GetRandomTrainer(possible);
    }

    /// <summary>
    /// Fetches an appropriate trainer based on the requested <see cref="generation"/>.
    /// </summary>
    /// <param name="generation">Generation the trainer should inhabit</param>
    /// <param name="lang">Language to request for</param>
    /// <returns>Null if no trainer found for this version.</returns>
    public ITrainerInfo? GetTrainerFromGen(int generation, LanguageID? lang = null)
    {
        var possible = Database.Where(z => z.Key.GetGeneration() == generation).ToList();
        if (lang != null)
        {
            possible = possible.Select(z =>
            {
                var filtered = z.Value.Where(x => x.Language == (int)lang).ToList();
                return new KeyValuePair<GameVersion, List<ITrainerInfo>>(z.Key, filtered);
            }).Where(z => z.Value.Count != 0).ToList();
        }
        return GetRandomTrainer(possible);
    }

    private static ITrainerInfo? GetRandomTrainer(IReadOnlyList<KeyValuePair<GameVersion, List<ITrainerInfo>>> possible)
    {
        if (possible.Count == 0)
            return null;
        var group = GetRandomChoice(possible);
        return GetRandomChoice(group.Value);
    }

    /// <summary>
    /// Adds the <see cref="trainer"/> to the <see cref="Database"/>.
    /// </summary>
    /// <param name="trainer">Trainer details to add.</param>
    public void Register(ITrainerInfo trainer)
    {
        var ver = (GameVersion)trainer.Game;
        if (ver <= 0 && trainer is SaveFile s)
            ver = s.Version;
        if (!Database.TryGetValue(ver, out var list))
        {
            Database.Add(ver, [trainer]);
            return;
        }

        if (list.Contains(trainer))
            return;
        list.Add(trainer);
    }

    /// <summary>
    /// Adds the trainer details of the <see cref="pk"/> to the <see cref="Database"/>.
    /// </summary>
    /// <param name="pk">Pokémon with Trainer details to add.</param>
    /// <remarks>A copy of the object will be made to prevent modifications, just in case.</remarks>
    public void RegisterCopy(PKM pk) => Register(GetTrainerReference(pk));

    /// <summary>
    /// Adds the trainer details of the <see cref="info"/> to the <see cref="Database"/>.
    /// </summary>
    /// <param name="info">Pokémon with Trainer details to add.</param>
    /// <remarks>A copy of the object will be made to prevent modifications, just in case.</remarks>
    public void RegisterCopy(ITrainerInfo info) => Register(new SimpleTrainerInfo(info));

    private static SimpleTrainerInfo GetTrainerReference(PKM pk)
    {
        var result = new SimpleTrainerInfo((GameVersion)pk.Version)
        {
            TID16 = pk.TID16, SID16 = pk.SID16, OT = pk.OT_Name, Gender = pk.OT_Gender,
            Language = pk.Language,
            Generation = pk.Generation,
        };

        if (pk is IRegionOrigin r)
            r.CopyRegionOrigin(result);
        else
            result.SetDefaultRegionOrigins(result.Language);

        return result;
    }

    /// <summary>
    /// Clears all trainer details from the <see cref="Database"/>.
    /// </summary>
    public void Clear() => Database.Clear();
}

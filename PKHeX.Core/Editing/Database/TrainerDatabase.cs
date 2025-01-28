using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

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
    public ITrainerInfo? GetTrainer(GameVersion version, LanguageID? language = null)
    {
        if (version <= 0)
            return null;

        if (!version.IsValidSavedVersion())
            return GetTrainerFromGroup(version, language);

        if (Database.TryGetValue(version, out var list))
            return list[GetRandomIndex(list.Count)];

        return null;
    }

    private static int GetRandomIndex(int count) => count == 1 ? 0 : Util.Rand.Next(count);

    /// <summary>
    /// Fetches an appropriate trainer based on the requested <see cref="version"/> group.
    /// </summary>
    /// <param name="version">Version the trainer should originate from</param>
    /// <param name="lang">Language to request for</param>
    /// <returns>Null if no trainer found for this version.</returns>
    private ITrainerInfo? GetTrainerFromGroup(GameVersion version, LanguageID? lang = null)
    {
        var possible = Database.Where(z => version.Contains(z.Key)).ToList();
        if (possible.Count == 0)
            return null;

        if (lang is not null)
        {
            possible = possible.Select(z =>
            {
                var filtered = z.Value.Where(x => x.Language == (int)lang).ToList();
                return new KeyValuePair<GameVersion, List<ITrainerInfo>>(z.Key, filtered);
            }).Where(z => z.Value.Count != 0).ToList();
        }
        var span = CollectionsMarshal.AsSpan(possible);
        return GetRandomTrainer(span);
    }

    /// <summary>
    /// Fetches an appropriate trainer based on the requested <see cref="generation"/>.
    /// </summary>
    /// <param name="generation">Generation the trainer should inhabit</param>
    /// <param name="lang">Language to request for</param>
    /// <returns>Null if no trainer found for this version.</returns>
    public ITrainerInfo? GetTrainerFromGen(byte generation, LanguageID? lang = null)
    {
        var possible = Database.Where(z => z.Key.GetGeneration() == generation).ToList();
        if (possible.Count == 0)
            return null;

        if (lang is not null)
        {
            possible = possible.Select(z =>
            {
                var filtered = z.Value.Where(x => x.Language == (int)lang).ToList();
                return new KeyValuePair<GameVersion, List<ITrainerInfo>>(z.Key, filtered);
            }).Where(z => z.Value.Count != 0).ToList();
        }
        var span = CollectionsMarshal.AsSpan(possible);
        return GetRandomTrainer(span);
    }

    private static ITrainerInfo? GetRandomTrainer(ReadOnlySpan<KeyValuePair<GameVersion, List<ITrainerInfo>>> possible)
    {
        if (possible.Length == 0)
            return null;
        var group = possible[GetRandomIndex(possible.Length)];
        var span = group.Value;
        return span[GetRandomIndex(span.Count)];
    }

    /// <summary>
    /// Adds the <see cref="trainer"/> to the <see cref="Database"/>.
    /// </summary>
    /// <param name="trainer">Trainer details to add.</param>
    public void Register(ITrainerInfo trainer)
    {
        var version = trainer.Version;
        if (!Database.TryGetValue(version, out var list))
        {
            Database.Add(version, [trainer]);
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
        var result = new SimpleTrainerInfo(pk.Version)
        {
            TID16 = pk.TID16, SID16 = pk.SID16, OT = pk.OriginalTrainerName, Gender = pk.OriginalTrainerGender,
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

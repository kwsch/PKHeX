using System.IO;
using System.Linq;
using PKHeX.Core;

namespace PKHeX.Application.UseCases;

/// <summary>
/// Classifies what a single dropped file resolves to.
/// </summary>
public enum EntityFileDropKind
{
    /// <summary>File could not be recognized as a Pokémon entity or save file.</summary>
    Unsupported,

    /// <summary>File is a save file; the caller should route it through the normal "open save" path.</summary>
    SaveFile,

    /// <summary>File is a Pokémon entity, but not compatible with the target save file.</summary>
    Incompatible,

    /// <summary>File is a Pokémon entity compatible with the target save file.</summary>
    Entity,
}

public readonly record struct EntityFileDropResult(EntityFileDropKind Kind, PKM? Entity, string? Message);

/// <summary>
/// Resolves a single dropped file path against a <see cref="SaveFile"/>, reusing the same
/// detection and conversion pipeline as the existing folder import feature
/// (<see cref="FileUtil.GetSupportedFile(string, SaveFile?)"/> + <see cref="SaveExtensions.GetCompatible"/>).
/// </summary>
public sealed class ImportEntityFileUseCase
{
    public EntityFileDropResult Execute(SaveFile sav, string path)
    {
        object? obj;
        try
        {
            obj = FileUtil.GetSupportedFile(path, sav);
        }
        catch
        {
            return new EntityFileDropResult(EntityFileDropKind.Unsupported, null, $"'{Path.GetFileName(path)}' could not be read.");
        }

        if (obj is SaveFile)
            return new EntityFileDropResult(EntityFileDropKind.SaveFile, null, null);

        PKM? candidate;
        try
        {
            candidate = obj switch
            {
                PKM pk => pk,
                MysteryGift { IsEntity: true } g => g.ConvertToPKM(sav),
                IEncounterInfo { Species: > 0 } g => g.ConvertToPKM(sav),
                _ => null,
            };
        }
        catch
        {
            return new EntityFileDropResult(EntityFileDropKind.Unsupported, null, $"'{Path.GetFileName(path)}' could not be converted to a Pokémon file.");
        }

        if (candidate is null)
            return new EntityFileDropResult(EntityFileDropKind.Unsupported, null, $"'{Path.GetFileName(path)}' is not a supported Pokémon file.");

        PKM? compatible;
        try
        {
            compatible = sav.GetCompatible([candidate]).FirstOrDefault();
        }
        catch
        {
            return new EntityFileDropResult(EntityFileDropKind.Incompatible, null,
                $"'{Path.GetFileName(path)}' ({candidate.GetType().Name}) is not compatible with this save file.");
        }

        if (compatible is null)
        {
            return new EntityFileDropResult(EntityFileDropKind.Incompatible, null,
                $"'{Path.GetFileName(path)}' ({candidate.GetType().Name}) is not compatible with this save file.");
        }

        return new EntityFileDropResult(EntityFileDropKind.Entity, compatible, null);
    }
}

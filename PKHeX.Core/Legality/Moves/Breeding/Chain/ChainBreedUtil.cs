using System;
using System.Text;

namespace PKHeX.Core;

/// <summary>
/// Utility for visualizing a successful chain-breeding proof.
/// </summary>
public static class ChainBreedUtil
{
    /// <summary>
    /// Summarizes a successful chain-breeding proof into a human-readable string.
    /// </summary>
    /// <param name="strings">The game strings for localization.</param>
    /// <param name="species">The species of the Pokémon egg.</param>
    /// <param name="form">The form of the Pokémon egg.</param>
    /// <param name="version">The game version the egg was obtained on.</param>
    /// <param name="moves">The moves to be summarized.</param>
    /// <returns>A tuple containing the result and the human-readable message.</returns>
    public static (bool Result, string Message) Summarize(GameStrings strings, ushort species, byte form, GameVersion version, params ReadOnlySpan<ushort> moves)
    {
        Span<ChainBreedStep> buffer = stackalloc ChainBreedStep[ChainBreedLegality.MaxChainDepth];
        var trace = new ChainBreedTrace(buffer);
        var result = ChainBreedLegality.IsValid(species, form, version, moves, ref trace);
        if (!result)
            return (false, string.Empty);

        var sb = new StringBuilder();
        Summarize(strings, trace, sb);
        return (true, sb.ToString());
    }

    /// <inheritdoc cref="Summarize(GameStrings, ushort, byte, GameVersion, ReadOnlySpan{ushort})"/>
    public static bool Summarize(GameStrings strings, ushort species, byte form, GameVersion version, ReadOnlySpan<ushort> moves, StringBuilder sb)
    {
        Span<ChainBreedStep> buffer = stackalloc ChainBreedStep[ChainBreedLegality.MaxChainDepth];
        var trace = new ChainBreedTrace(buffer);
        var result = ChainBreedLegality.IsValid(species, form, version, moves, ref trace);
        if (!result)
            return false;

        Summarize(strings, trace, sb);
        return true;
    }

    #pragma warning disable RCS1242
    /// <inheritdoc cref="Summarize(GameStrings, ushort, byte, GameVersion, ReadOnlySpan{ushort})"/>
    public static string Summarize(GameStrings strings, in ChainBreedTrace trace)
    {
        var sb = new StringBuilder();
        WriteHeader(trace, sb);
        return sb.ToString();
    }

    /// <inheritdoc cref="Summarize(GameStrings, ushort, byte, GameVersion, ReadOnlySpan{ushort})"/>
    public static void Summarize(GameStrings strings, in ChainBreedTrace trace, StringBuilder sb)
    {
        WriteHeader(trace, sb);
        var steps = trace.Steps;
        for (var i = steps.Length - 1; i >= 0; i--)
        {
            var entry = steps[i];
            WriteEntry(entry, sb, strings, i);
        }
    }

    private static void WriteHeader(in ChainBreedTrace trace, StringBuilder sb)
    {
        int count = trace.Count;
        var breed = trace.GetDepth(ChainBreedStepKind.Breed);
        var transfer = trace.GetDepth(ChainBreedStepKind.Transfer);
        if (transfer == 0)
            sb.AppendLine($"{count} 🥚.");
        else
            sb.AppendLine($"{count} = {breed} 🥚 & {transfer} ♻️.");
    }
    #pragma warning restore RCS1242

    private static void WriteEntry(in ChainBreedStep entry, StringBuilder sb, GameStrings strings, int index)
    {
        var speciesName = GetSpeciesName(strings, entry.Species, entry.Form);
        var versionName = GetGameVersionName(strings, entry.Version);
        sb.AppendLine($"{index}) {speciesName} ({versionName})");

        if (entry.Kind == ChainBreedStepKind.Breed)
        {
            var motherName = GetSpeciesName(strings, entry.MotherSpecies, entry.MotherForm);
            var fatherName = GetSpeciesName(strings, entry.FatherSpecies, entry.FatherForm);
            sb.Append($"    ♀: {motherName} + ♂: {fatherName}");
        }
        else if (entry.Kind == ChainBreedStepKind.Transfer)
        {
            sb.Append($"    {entry.OriginVersion.Context} => {entry.Version.Context}");
        }
        sb.AppendLine();
        if (entry.MoveCount == 0)
            return;

        sb.Append("    ");
        for (int moveIndex = 0; moveIndex < entry.MoveCount; moveIndex++)
        {
            ushort moveID = moveIndex switch
            {
                0 => entry.Move1,
                1 => entry.Move2,
                2 => entry.Move3,
                3 => entry.Move4,
                _ => 0, // shouldn't happen, but just in case
            };
            if (moveID == 0)
                continue;
            var moveName = GetMoveName(strings, moveID);
            sb.Append(moveName);
            if (moveIndex < entry.MoveCount - 1)
                sb.Append(", ");
        }
        sb.AppendLine();
    }

    private static string GetMoveName(GameStrings strings, ushort moveId)
    {
        var moves = strings.movelist;
        return moves[moveId];
    }

    private static string GetGameVersionName(GameStrings strings, GameVersion version)
    {
        if (!version.IsValidSavedVersion())
            version = version.GetSingleVersion();

        var games = strings.gamelist;
        return games[(int)version];
    }

    private static string GetSpeciesName(GameStrings strings, ushort entrySpecies, byte entryForm)
    {
        var species = strings.specieslist;
        var result = species[entrySpecies];
        if (entryForm != 0)
            result += $"-{entryForm}";
        return result;
    }
}

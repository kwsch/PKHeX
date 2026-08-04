using System;
using System.Collections.Generic;
using System.Threading;
using PKHeX.Application.Abstractions;
using PKHeX.Application.UseCases;
using PKHeX.Core;
using PKHeX.Core.AutoMod;

namespace PKHeX.Infrastructure.AutoLegality;

/// <summary>
/// Adapter implementing the Application-layer <see cref="ILivingDexService"/> port (issue #123, ALM
/// Phase 2 — Living Dex generator).
/// </summary>
/// <remarks>
/// <para>
/// This does NOT call the vendored engine's own <c>ModLogic.GenerateLivingDex</c>. That method's
/// per-species helper (<c>GetRandomEncounter</c> → <c>tr.TryAPIConvert(set, template)</c>) never calls
/// <c>EncounterMovesetGenerator.OptimizeCriteria(template, tr)</c> before converting — the step
/// <c>Legalizer.GetLegalFromSet(ITrainerInfo, IBattleTemplate)</c> (the path <see cref="AutoLegalityService"/>
/// and the app's single-set "Auto Legality Mod" tool use) always performs first. Without it, the encounter
/// search finds nothing: verified directly against blank Red/Crystal/Emerald/Sword saves, where
/// <c>GenerateLivingDex</c> returns an empty list for every species in every one of them. That is a defect
/// in the vendored <c>PKHeX.AutoMod</c> project itself, which this task's hard rules forbid modifying.
/// </para>
/// <para>
/// So this adapter drives the species/form loop itself and calls the proven, working
/// <c>Legalizer.GetLegalFromSet</c> path per candidate instead — the exact same call
/// <see cref="AutoLegalityService"/> already relies on for the single-set tool. A future PKHeX.AutoMod
/// re-sync that fixes <c>ModLogic.GenerateLivingDex</c> upstream could let this adapter go back to calling
/// it directly; until then, this routes around the defect instead of shipping a generator that produces
/// nothing.
/// </para>
/// <para>
/// This loop intentionally builds a *minimal* per-species template (species, form, gender, shiny) rather
/// than reproducing every special case <c>ModLogic</c>'s own (unreachable) helper handles — form-specific
/// held items (Arceus plates, Genesect drives, etc.), Keldeo's Secret Sword, Zygarde's cell-count suffix,
/// and Alcremie's decoration sweep are not replicated. Species/forms that need those to legalize will
/// simply fail to generate and be reported in <see cref="LivingDexGenerationResult.SkippedSpeciesNames"/>
/// like any other failure, rather than being silently wrong or crashing.
/// </para>
/// </remarks>
public sealed class LivingDexService : ILivingDexService
{
    private readonly LivingDexVerificationUseCase _verification = new();

    public LivingDexGenerationResult Generate(SaveFile sav, LivingDexOptions options, IProgress<LivingDexGenerationProgress>? progress = null, CancellationToken cancellationToken = default, int? maxSpeciesId = null)
    {
        ArgumentNullException.ThrowIfNull(sav);

        if (cancellationToken.IsCancellationRequested)
            return LivingDexGenerationResult.Cancel();

        var tr = (ITrainerInfo)sav;
        var personal = sav.Personal;
        var context = sav.Context;
        var generation = sav.Generation;
        var strings = GameInfo.Strings;

        var maxSpecies = maxSpeciesId is { } cap ? Math.Min(cap, personal.MaxSpeciesID) : personal.MaxSpeciesID;
        var candidates = new List<PKM>();
        var failedNames = new List<string>();
        var completed = 0;

        try
        {
            for (ushort s = 1; s <= maxSpecies; s++)
            {
                cancellationToken.ThrowIfCancellationRequested();

                if (personal.IsSpeciesInGame(s))
                    ProcessSpecies(tr, personal, strings, context, generation, s, options, candidates, failedNames, cancellationToken);

                completed++;
                progress?.Report(new LivingDexGenerationProgress(completed, maxSpecies));
            }
        }
        catch (OperationCanceledException)
        {
            return LivingDexGenerationResult.Cancel();
        }

        // Never trust our own per-species search either (same independent-re-verification pattern as
        // AutoLegalityService, issue #89): re-check every candidate and report failures by name instead
        // of silently including or crashing on them.
        var verified = _verification.Verify(candidates);
        var allSkipped = new List<string>(failedNames.Count + verified.SkippedSpeciesNames.Count);
        allSkipped.AddRange(failedNames);
        allSkipped.AddRange(verified.SkippedSpeciesNames);

        return LivingDexGenerationResult.Ok(verified.Accepted, allSkipped);
    }

    private static void ProcessSpecies(ITrainerInfo tr, IPersonalTable personal, GameStrings strings, EntityContext context, byte generation, ushort species, LivingDexOptions options, List<PKM> candidates, List<string> failedNames, CancellationToken cancellationToken)
    {
        var numForms = personal[species].FormCount;
        if (numForms == 1 && options.IncludeForms)
            numForms = (byte)FormConverter.GetFormList(species, strings.types, strings.forms, GameInfo.GenderSymbolUnicode, context).Length;

        for (byte f = 0; f < numForms; f++)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var form = options.IncludeForms ? f : ModLogic.GetBaseForm((Species)species, f, tr);
            if (!personal.IsPresentInGame(species, form)
                || FormInfo.IsLordForm(species, form, context)
                || FormInfo.IsBattleOnlyForm(species, form, generation)
                || FormInfo.IsFusedForm(species, form, generation)
                || (FormInfo.IsTotemForm(species, form) && context is not EntityContext.Gen7))
                continue;

            var pk = TryGenerateOne(tr, species, form, options.SetShiny);
            if (pk is null)
            {
                failedNames.Add(LivingDexVerificationUseCase.GetDisplayName(species, form, strings));
                if (!options.IncludeForms)
                    break;
                continue;
            }

            candidates.Add(pk);
            if (!options.IncludeForms)
                break;
        }
    }

    /// <summary>
    /// Builds a minimal template for <paramref name="species"/>/<paramref name="form"/> and legalizes it
    /// via the proven <c>Legalizer.GetLegalFromSet</c> path (see class remarks) instead of the vendored
    /// engine's defective <c>ModLogic.GenerateLivingDex</c>/<c>GetRandomEncounter</c>.
    /// </summary>
    private static PKM? TryGenerateOne(ITrainerInfo tr, ushort species, byte form, bool shiny)
    {
        PKM blank;
        try
        {
            blank = EntityBlank.GetBlank(tr);
            blank.Species = species;
            blank.Gender = blank.GetSaneGender();
            blank.Form = form;
        }
        catch
        {
            return null;
        }

        string setText;
        try
        {
            // Render species/form to Showdown text from the PKM itself (handles form-name suffixes
            // correctly) rather than building the string by hand — but keep ONLY the first line (species
            // [+ gender/form annotation]). The rest of a freshly-blanked PKM's rendered text (Level: 1,
            // Hardy Nature, IVs: 0 all around, an incidental "Shiny: Yes" from the blank's default PID,
            // etc.) over-constrains the set to a combination no real encounter can satisfy, which is why
            // an earlier version of this loop that reused the whole rendered block failed for every
            // species. A bare species (plus an explicit shiny line when requested) leaves the encounter
            // search free to pick a matching level/nature/IVs/ability, exactly like the app's single-set
            // "Auto Legality Mod" tool does when the user only types a species name.
            var rendered = new ShowdownSet(blank).Text.Replace("\r", "");
            var firstLine = rendered.Split('\n')[0];
            setText = firstLine;
            if (shiny && !SimpleEdits.IsShinyLockedSpeciesForm(species, blank.Form))
                setText += "\nShiny: Yes";
        }
        catch
        {
            return null;
        }

        ShowdownSet set;
        try
        {
            set = new ShowdownSet(setText);
        }
        catch
        {
            return null;
        }

        if (set.Species == 0)
            return null;

        APILegality.AsyncLegalizationResult result;
        try
        {
            result = tr.GetLegalFromSet(set);
        }
        catch
        {
            return null;
        }

        if (result.Status != LegalizationResult.Regenerated)
            return null;

        var pk = result.Created;
        pk.Heal();
        return pk;
    }
}

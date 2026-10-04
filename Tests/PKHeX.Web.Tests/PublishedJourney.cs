using System.Text.RegularExpressions;
using Microsoft.Playwright;
using PKHeX.Core;
using PKHeX.Web.State;
using Xunit;
using static Microsoft.Playwright.Assertions;
using static PKHeX.Web.Tests.FileDrops;
using static PKHeX.Web.Tests.ProofPage;

namespace PKHeX.Web.Tests;

/// <summary>
/// The full published journey in one session: open through the picker and then a drop, download unchanged, edit one field of each
/// group across a boxed Pokémon and a party member, compare legality with native Core, apply, download, reopen natively and in the app, and
/// check the privacy trace. Shared by the E2E tier (synthetic saves) and the RealSave tier (private saves), so values are kept out of every
/// failure message.
/// </summary>
internal static class PublishedJourney
{
    /// <summary>Saved values shorter than this are not looked for in the console, where they could match unrelated text.</summary>
    private const int MinSentinelLength = 4;

    /// <summary>
    /// Runs the journey of <paramref name="plan"/> on <paramref name="save"/> in <paramref name="session"/>, which must be booted with nothing open.
    /// </summary>
    /// <param name="session">A booted session.</param>
    /// <param name="plan">The edits and native expectations, planned on <paramref name="save"/>.</param>
    /// <param name="save">The save the journey edits, opened by a drop.</param>
    /// <param name="fileName">The name it is dropped under; a privacy sentinel.</param>
    /// <param name="decoy">Another save, opened first through the picker and replaced by the drop.</param>
    /// <param name="sentinels">Further saved values that must never reach the console, such as a box name.</param>
    public static async Task RunAsync(AppSession session, JourneyPlan plan, byte[] save, string fileName, byte[] decoy, IEnumerable<string> sentinels)
    {
        var page = session.Page;
        var url = page.Url;
        var (saveCopy, decoyCopy) = (save.ToArray(), decoy.ToArray());

        // The picker opens one save, and a drop replaces it at once, since it has no changes to lose.
        await Load(page, decoy, "decoy.sav");
        await Expect(page.Locator("#overview-file")).ToHaveTextAsync("decoy.sav");
        Assert.True(await Drop(page, "#save-drop", [(fileName, save)]), "The drop zone did not take over the drop.");
        await Expect(page.Locator("#overview-file")).ToHaveTextAsync(fileName);
        await Expect(page.Locator("#exit")).ToHaveCountAsync(0);
        Assert.True((await Download(page, fileName)).AsSpan().SequenceEqual(plan.NoOp), "The no-op download differs from native Core (bytes withheld).");

        // The boxed Pokémon, then the party member: each edited, its legality compared with native Core, then applied.
        await Select(page, plan.Box);
        await MakeAsync(page, plan.BoxSteps);
        await CheckLegality(page, plan.ExpectedBox, plan.Changed, StorageSlotType.Box);
        await Apply(page);
        await Expect(page.Locator("#session-state")).ToHaveTextAsync("Edited in memory");
        await Select(page, plan.Party);
        await MakeAsync(page, plan.PartySteps);
        await CheckLegality(page, plan.ExpectedParty, plan.Changed, StorageSlotType.Party);
        await Apply(page);
        await Expect(page.Locator("#draft-state")).ToHaveTextAsync("No draft changes");

        // The export is native Core's, reopens natively with both edits, and changes nothing outside the two slots and the checksum footer.
        var (edited, editedName) = await DownloadNamed(page);
        var stem = Path.GetFileNameWithoutExtension(fileName);
        Assert.Matches($"^{Regex.Escape(stem)}-modified-\\d{{4}}-\\d{{2}}-\\d{{2}}-\\d{{6}}{Regex.Escape(Path.GetExtension(fileName))}$", editedName);
        Assert.True(edited.AsSpan().SequenceEqual(plan.ExpectedEdited), "The edited download differs from native Core (bytes withheld).");
        plan.AssertReopened(SaveFixtures.Parse(edited));
        plan.AssertOnlyTheEditedSlotsDiffer(edited);

        // Reopened in the app, every control holds what was edited, legality agrees with native Core, and a no-op download is the same file.
        // The current revision was downloaded, so the replace waits only for the confirmation that the export was checked.
        await Load(page, edited, editedName);
        await page.Locator("#exit-continue").ClickAsync();
        await Expect(page.Locator("#session-state")).ToHaveTextAsync("Unmodified session");
        await Select(page, plan.Box);
        await AssertShownAsync(page, plan.BoxSteps);
        await CheckLegality(page, plan.ExpectedBox, plan.Changed, StorageSlotType.Box);
        await Select(page, plan.Party);
        await AssertShownAsync(page, plan.PartySteps);
        await CheckLegality(page, plan.ExpectedParty, plan.Changed, StorageSlotType.Party);
        Assert.True((await Download(page, editedName)).AsSpan().SequenceEqual(edited), "The reopened export did not download unchanged (bytes withheld).");

        // Privacy trace: the address never carried anything, no saved or typed value reached the console, nothing was asked, nothing reached
        // the network or storage after boot, and the files given were not changed.
        Assert.True(page.Url == url, "The page address changed during the journey.");
        AssertConsoleClean(session, [plan.Nickname, fileName, plan.Native.OT, .. sentinels]);
        Assert.True(session.Dialogs.IsEmpty, "A browser dialog was raised during the journey.");
        await session.AssertNoNetworkOrPersistenceAsync();
        Assert.True(session.PageErrors == 0, "Browser runtime errors occurred during the journey.");
        Assert.True(save.AsSpan().SequenceEqual(saveCopy) && decoy.AsSpan().SequenceEqual(decoyCopy), "The journey changed the files it was given.");
    }

    /// <summary>
    /// Makes <paramref name="steps"/> in the editor, checking each changes its control (a step that sets the value already shown would pass every
    /// later check without editing anything), is accepted, and leaves its control holding the value. The control's id is the only detail reported.
    /// </summary>
    public static async Task MakeAsync(IPage page, IEnumerable<JourneyStep> steps)
    {
        foreach (var step in steps)
        {
            var control = page.Locator($"#{step.ControlId}");
            if (step.Action != JourneyAction.Ticked)
            {
                Assert.True(await control.InputValueAsync() != step.Value, $"The {step.ControlId} step would not change the value shown (values withheld).");
            }
            switch (step.Action)
            {
                case JourneyAction.Fill:
                    await control.FillAsync(step.Value);
                    break;
                case JourneyAction.Ticked:
                    await Expect(control).ToBeCheckedAsync();
                    break;
                case JourneyAction.Choose:
                    await control.SelectOptionAsync(step.Value);
                    break;
                case JourneyAction.ConfirmSpecies:
                    await control.SelectOptionAsync(step.Value);
                    await page.Locator("#species-confirm").ClickAsync();
                    await Expect(page.Locator("#species-confirm")).ToHaveCountAsync(0);
                    break;
                case JourneyAction.Search:
                    await page.Locator($"#{step.ControlId}-search").FillAsync(step.Query!);
                    await control.SelectOptionAsync(step.Value);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(steps), step.Action, null);
            }
            await Expect(control).Not.ToHaveAttributeAsync("aria-invalid", "true");
        }
        await AssertShownAsync(page, steps);
    }

    /// <summary>Checks every control of <paramref name="steps"/> holds its step's value (a checkbox is ticked).</summary>
    public static async Task AssertShownAsync(IPage page, IEnumerable<JourneyStep> steps)
    {
        foreach (var step in steps)
        {
            var control = page.Locator($"#{step.ControlId}");
            if (step.Action == JourneyAction.Ticked)
            {
                await Expect(control).ToBeCheckedAsync();
            }
            else
            {
                await Expect(control).ToHaveValueAsync(step.Value);
            }
        }
    }

    /// <summary>Fails if any console message of <paramref name="session"/> contains one of <paramref name="sentinels"/>; neither is put in the message.</summary>
    public static void AssertConsoleClean(AppSession session, IEnumerable<string> sentinels)
    {
        var looked = sentinels.Where(s => s.Length >= MinSentinelLength).ToList();
        Assert.True(looked.Count > 0, "No sentinel was long enough to look for.");
        var leaked = session.ConsoleMessages.Any(m => looked.Any(s => m.Contains(s, StringComparison.OrdinalIgnoreCase)));
        Assert.False(leaked, "A saved or typed value reached the browser console (details withheld).");
    }
}

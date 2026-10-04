using System.Globalization;
using Microsoft.Playwright;
using PKHeX.Core;
using PKHeX.Web.State;
using Xunit;
using static Microsoft.Playwright.Assertions;
using static PKHeX.Web.Tests.ProofPage;

namespace PKHeX.Web.Tests;

/// <summary>
/// The full published journey with synthetic saves in every engine, at the root and under <c>/PKHeX/</c> (WEB-TEST-005, WEB-SEC-001):
/// once with every field group, compared with native Core, and once by keyboard alone.
/// </summary>
/// <remarks>
/// The family follows the hosting path, XY at the root and ORAS under <c>/PKHeX/</c>, so each engine covers both families, and both an Invalid
/// result (the XY entity, acknowledged before apply and download) and a Valid one, without running every journey twice.
/// </remarks>
[Collection(PublishedAppCollection.Name)]
[Trait(TestCategory.Name, TestCategory.E2E)]
public sealed class JourneyBrowserTests(PublishedAppFixture app)
{
    /// <summary>The journey save's first box name; a privacy sentinel.</summary>
    private const string BoxName = "Waystation";

    /// <summary>The name the journey save is dropped under; a privacy sentinel.</summary>
    private const string FileName = "journey-sentinel.sav";

    /// <summary>Most key presses <see cref="TabToAsync"/> makes looking for a control before failing.</summary>
    private const int MaxTabs = 400;

    /// <summary>
    /// Counts trusted pointer, mouse and touch presses on the page, so a keyboard-only run can show that none reached it. Playwright's evaluate
    /// runs outside the page's CSP, so the app's policy is not loosened for it.
    /// </summary>
    private const string PointerCounter = """
        () => {
            window.__pkhexPointer = 0;
            for (const type of ['pointerdown', 'mousedown', 'touchstart']) {
                addEventListener(type, e => { if (e.isTrusted) window.__pkhexPointer++; }, true);
            }
        }
        """;

    /// <summary>The journey save: the known entity in box 1, slot 1 and a party member, with a sentinel box name.</summary>
    private static byte[] JourneySave(bool oras) => SaveFixtures.Synthetic(oras, customize: SaveFixtures.All(
        SaveFixtures.WithPartyMember(),
        save => ((IBoxDetailName)save).SetBoxName(0, BoxName)));

    /// <summary>Another save of the same family, opened first and replaced: the known illegal entity, with a party member of its own.</summary>
    private static byte[] Decoy(bool oras) => SaveFixtures.Synthetic(oras, legal: false, customize: SaveFixtures.WithPartyMember("Decoy"));

    [TierTheory(TestCategory.E2E)]
    [MemberData(nameof(PublishedAppFixture.BrowserCases), MemberType = typeof(PublishedAppFixture))]
    public async Task FullJourneyMatchesNativeCore(string engine, string prefix)
    {
        var oras = prefix.Length != 0;
        var save = JourneySave(oras);
        var decoy = Decoy(oras);
        var plan = JourneyPlan.For(SaveFixtures.Parse(save));

        await using var session = await app.BootAsync(engine, prefix);
        await PublishedJourney.RunAsync(session, plan, save, FileName, decoy, [BoxName]);

        // WEB-SEC-001: a fresh visit that opens only the other save and another Pokémon makes the same requests, all at boot, so nothing the
        // app fetches depends on the save or the selection.
        await using var other = await app.BootAsync(engine, prefix);
        await Load(other.Page, decoy);
        await Select(other.Page, SlotRef.InParty(0));
        await other.AssertNoNetworkOrPersistenceAsync();
        Assert.Equal(session.BootRequests.Order(StringComparer.Ordinal), other.BootRequests.Order(StringComparer.Ordinal));
    }

    [TierTheory(TestCategory.E2E)]
    [MemberData(nameof(PublishedAppFixture.BrowserCases), MemberType = typeof(PublishedAppFixture))]
    public async Task KeyboardOnlyJourney(string engine, string prefix)
    {
        var oras = prefix.Length != 0;
        var save = JourneySave(oras);
        const string nickname = "Keyfarer";

        // Native Core: the party member one level on and its friendship changed (stats recalculated by the PK6 party-stat policy), and the boxed
        // Pokémon nicknamed.
        var changed = SaveFixtures.Parse(save);
        var member = (PK6)changed.GetPartySlotAtIndex(0);
        var level = JourneyPlan.LevelStep(member.CurrentLevel);
        var friendship = member.OriginalTrainerFriendship == 200 ? 201 : 200;
        var (hp, status) = (member.Stat_HPCurrent, member.Status_Condition);
        member.EXP = Experience.GetEXP(level, member.PersonalInfo.EXPGrowth);
        member.OriginalTrainerFriendship = (byte)friendship;
        member.ResetPartyStats();
        member.Status_Condition = status;
        member.Stat_HPCurrent = Math.Min(hp, member.Stat_HPMax);
        changed.SetPartySlotAtIndex(member, 0, EntityImportSettings.None);
        var boxed = changed.GetBoxSlotAtIndex(0, 0);
        Assert.False(boxed.IsNicknamed, "The boxed fixture is already nicknamed, so ticking the flag would clear it.");
        boxed.Nickname = nickname;
        boxed.IsNicknamed = true;
        changed.SetBoxSlotAtIndex(boxed, 0, 0, EntityImportSettings.None);
        var expected = changed.Write().ToArray();

        await using var session = await app.BootAsync(engine, prefix);
        var page = session.Page;
        await page.EvaluateAsync(PointerCounter);

        // Open: the file input is reached and activated from the keyboard; the operating system's dialog is answered by the test.
        await TabToAsync(page, engine, "#save-file");
        var chooser = await page.RunAndWaitForFileChooserAsync(() => page.Keyboard.PressAsync("Space"));
        await chooser.SetFilesAsync(new FilePayload { Name = FileName, MimeType = "application/octet-stream", Buffer = save });
        await Expect(page.Locator("#overview-file")).ToHaveTextAsync(FileName);

        // The party member: opened from the grid's tab stop, then its level and friendship typed.
        await TabToAsync(page, engine, "#party-grid-0");
        await page.Keyboard.PressAsync("Enter");
        await Expect(page.Locator("#draft-slot")).ToHaveTextAsync(Components.SlotText.Position(SlotRef.InParty(0)));
        await TypeAsync(page, engine, "#level", level.ToString(CultureInfo.InvariantCulture));
        await TypeAsync(page, engine, "#ot-friendship", friendship.ToString(CultureInfo.InvariantCulture));
        await ApplyByKeyboardAsync(page, engine);
        await Expect(page.Locator("#session-state")).ToHaveTextAsync("Edited in memory");

        // The boxed Pokémon: opened from the box grid's tab stop, then nicknamed; typing a name other than the species' ticks the flag.
        await TabToAsync(page, engine, "#box-grid-0");
        await page.Keyboard.PressAsync("Enter");
        await Expect(page.Locator("#draft-slot")).ToHaveTextAsync(Components.SlotText.Position(SaveFixtures.FirstBoxSlot));
        await TypeAsync(page, engine, "#nickname", nickname);
        await Expect(page.Locator("#nicknamed")).ToBeCheckedAsync();
        await ApplyByKeyboardAsync(page, engine);
        await Expect(page.Locator("#draft-state")).ToHaveTextAsync("No draft changes");

        // Download: the acknowledgement of flagged changes (XY) ticked with Space, then Download pressed.
        if (await page.Locator("#export-ack").CountAsync() > 0)
        {
            await TabToAsync(page, engine, "#export-ack");
            await page.Keyboard.PressAsync("Space");
            await Expect(page.Locator("#export-ack")).ToBeCheckedAsync();
        }
        await TabToAsync(page, engine, "#download");
        var download = await page.RunAndWaitForDownloadAsync(() => page.Keyboard.PressAsync("Enter"));
        var path = await download.PathAsync();
        Assert.True(path is not null, "No local download was produced.");
        Assert.True((await File.ReadAllBytesAsync(path!)).AsSpan().SequenceEqual(expected), "The keyboard journey's download differs from native Core (bytes withheld).");

        Assert.True(await page.EvaluateAsync<int>("() => window.__pkhexPointer") == 0, "A pointer press reached the page during the keyboard-only journey.");
        PublishedJourney.AssertConsoleClean(session, [nickname, FileName, BoxName]);
        await session.AssertNoNetworkOrPersistenceAsync();
        Assert.True(session.PageErrors == 0, "Browser runtime errors occurred.");
    }

    /// <summary>
    /// Moves focus from where it is, one key press at a time, until it is on <paramref name="selector"/>, failing after <see cref="MaxTabs"/>
    /// presses. It goes forward with Tab, or back with Shift+Tab when the control comes earlier in the page, as a keyboard user would: past the
    /// last control Firefox moves focus into its own toolbar, which the page cannot follow. WebKit adds Option, Safari's key that reaches every control.
    /// </summary>
    private static async Task TabToAsync(IPage page, string engine, string selector)
    {
        var back = await page.EvaluateAsync<bool>("""
            s => {
                const target = document.querySelector(s), active = document.activeElement;
                return !!target && !!active && active !== document.body && (active.compareDocumentPosition(target) & Node.DOCUMENT_POSITION_PRECEDING) !== 0;
            }
            """, selector);
        var key = (engine == "webkit" ? "Alt+" : "") + (back ? "Shift+Tab" : "Tab");
        for (var i = 0; i < MaxTabs; i++)
        {
            await page.Keyboard.PressAsync(key);
            if (await page.EvaluateAsync<bool>("s => document.activeElement?.matches(s) === true", selector))
            {
                return;
            }
        }
        Assert.Fail($"{selector} could not be reached with the keyboard.");
    }

    /// <summary>Reaches the text field <paramref name="selector"/> by keyboard, selects its text and types <paramref name="text"/> over it.</summary>
    private static async Task TypeAsync(IPage page, string engine, string selector, string text)
    {
        await TabToAsync(page, engine, selector);
        await page.Keyboard.PressAsync("ControlOrMeta+A");
        await page.Keyboard.TypeAsync(text);
        await Expect(page.Locator(selector)).ToHaveValueAsync(text);
    }

    /// <summary>Waits for the draft's verdict, ticks an Invalid or Unavailable result's acknowledgement with Space, then presses Apply.</summary>
    private static async Task ApplyByKeyboardAsync(IPage page, string engine)
    {
        await Expect(page.Locator("#legality-status")).ToHaveTextAsync(VerdictPattern);
        if (await page.Locator("#apply-ack").CountAsync() > 0)
        {
            await TabToAsync(page, engine, "#apply-ack");
            await page.Keyboard.PressAsync("Space");
            await Expect(page.Locator("#apply-ack")).ToBeCheckedAsync();
        }
        await TabToAsync(page, engine, "#apply");
        await Expect(page.Locator("#apply")).ToHaveAttributeAsync("aria-disabled", "false");
        await page.Keyboard.PressAsync("Enter");
    }
}

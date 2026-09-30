using System.Globalization;
using System.Text.Json;
using PKHeX.Core;
using PKHeX.Web.Services;
using Xunit;
using static Microsoft.Playwright.Assertions;
using static PKHeX.Web.Tests.ProofPage;

namespace PKHeX.Web.Tests;

/// <summary>
/// Published app round trips on the private real saves, compared byte-for-byte with the same edit made through native Core.
/// </summary>
[Collection(PublishedAppCollection.Name)]
[Trait(TestCategory.Name, TestCategory.RealSave)]
public sealed class RealSaveBrowserTests(PublishedAppFixture app)
{
    public static IEnumerable<object[]> RealCases()
    {
        foreach (var engine in PublishedAppFixture.Engines)
        {
            foreach (var prefix in PublishedAppFixture.Prefixes)
            {
                foreach (var family in RealSaves.Families)
                {
                    yield return [engine, prefix, family];
                }
            }
        }
    }

    [TierTheory(TestCategory.RealSave)]
    [MemberData(nameof(RealCases))]
    public async Task RealSavePublishedRoundTrip(string engine, string prefix, string family)
    {
        // Private fixture: validated natively before any browser run.
        var fixture = RealSaves.Read(family);
        var native = fixture.Native;

        var index = SaveFixtures.WritableSlot(native);
        var nativeSlot = SaveFixtures.Slot(native, index);
        var sourcePk = nativeSlot.Read(native);
        var expectedNoOp = native.Clone().Write().ToArray();

        // Native reference edit: the same nickname change, made directly through Core.
        var changed = native.Clone();
        var editedPk = SaveFixtures.Slot(changed, index).Read(changed);
        var nickname = editedPk.Nickname == "WASM Proof" ? "WASM Test" : "WASM Proof";
        editedPk.Nickname = nickname;
        editedPk.IsNicknamed = true;
        var beforeSet = changed.Data.ToArray();
        Assert.True(SaveFixtures.Slot(changed, index).WriteTo(changed, editedPk, EntityImportSettings.None));

        var start = changed.GetBoxSlotOffset(index.Box, index.Slot);
        var slotSize = changed.SIZE_BOXSLOT;
        Assert.True(changed.PartyCount == native.PartyCount, "Party count changed.");
        Assert.True(beforeSet.AsSpan(0, start).SequenceEqual(changed.Data[..start]), "Native edit changed data before the target slot.");
        Assert.True(beforeSet.AsSpan(start + slotSize).SequenceEqual(changed.Data[(start + slotSize)..]), "Native edit changed data after the target slot (including party/dex/records).");
        var expectedEdited = changed.Clone().Write().ToArray();

        await using var session = await app.BootAsync(engine, prefix);
        var page = session.Page;

        // No-op round trip: open, analyse, download unchanged.
        await Load(page, fixture.Bytes);
        await Expect(page.Locator("#overview-family")).ToHaveTextAsync(SupportMatrix.Families.Single(f => f.SaveType == native.GetType()).Games);
        // Compared with native Core values; the private values are kept out of the failure messages.
        Assert.True(await page.Locator("#overview-trainer").TextContentAsync() == native.OT, "Overview trainer differs from native (value withheld).");
        Assert.True(await page.Locator("#overview-tid").TextContentAsync() == native.DisplayTID.ToString("D5"), "Overview TID differs from native (value withheld).");
        Assert.True(await page.Locator("#overview-sid").TextContentAsync() == native.DisplaySID.ToString("D5"), "Overview SID differs from native (value withheld).");
        Assert.True(await page.Locator("#overview-money").TextContentAsync() == string.Create(CultureInfo.InvariantCulture, $"{native.Money:#,0} Pokédollars"), "Overview money differs from native (value withheld).");
        await Select(page, index);
        await CheckLegality(page, sourcePk, native, nativeSlot.Type);
        var noOp = await Download(page);
        Assert.True(noOp.AsSpan().SequenceEqual(expectedNoOp), "No-op browser/native output differs (bytes withheld).");

        // Edited round trip: reopen the no-op output, edit the nickname, apply, download.
        await Load(page, noOp);
        await Expect(page.Locator("#message")).ToHaveTextAsync("Save loaded locally; checksums valid.");
        await Select(page, index);
        await page.Locator("#nickname").FillAsync(nickname);
        await page.Locator("#nicknamed").CheckAsync();
        await CheckLegality(page, editedPk, changed, nativeSlot.Type);
        await page.Locator("#apply").ClickAsync();
        await Expect(page.Locator("#session-state")).ToHaveTextAsync("Edited in memory");
        var edited = await Download(page);
        Assert.True(edited.AsSpan().SequenceEqual(expectedEdited), "Edited browser/native output differs (bytes withheld).");

        // The edited export changes only the nickname fields of the target slot.
        var reopened = SaveFixtures.Parse(edited);
        var reopenedPk = SaveFixtures.Slot(reopened, index).Read(reopened);
        Assert.True(reopened.ChecksumsValid && reopenedPk.Nickname == nickname);
        Assert.True(reopened.PartyCount == native.PartyCount, "Export changed party count.");
        Assert.True(PartyBytes(reopened).SequenceEqual(PartyBytes(native)), "Export changed party data.");
        AssertOnlyRangeDiffers(noOp, edited, start, slotSize);
        AssertOnlyNicknameChanged(sourcePk, reopenedPk);

        // Replacing the edited session with its own export shows the edit persisted.
        await Load(page, edited);
        await page.Locator("#replace-confirm").ClickAsync();
        await Expect(page.Locator("#session-state")).ToHaveTextAsync("Unmodified session");
        await Select(page, index);
        Assert.True(await page.Locator("#nickname").InputValueAsync() == nickname, "Downloaded nickname did not survive reopening.");
        await CheckLegality(page, reopenedPk, reopened, nativeSlot.Type);

        // Privacy, runtime errors, and reload clearing the session.
        await session.AssertNoNetworkOrPersistenceAsync();
        Assert.True(session.PageErrors == 0, "Browser runtime errors occurred; no private traces retained.");
        await page.ReloadAsync();
        await Expect(page.Locator("#save-file")).ToBeVisibleAsync();
        await Expect(page.Locator("#overview-title")).ToHaveCountAsync(0);
        await Expect(page.Locator("#box-grid")).ToHaveCountAsync(0);

        // The reload is a second boot: check it the same way, then that it left nothing behind.
        await app.AssertStaticBootAsync(session);
        await session.AssertNoNetworkOrPersistenceAsync();
        Assert.True(session.PageErrors == 0, "Browser runtime errors occurred after reload.");
        fixture.AssertUnchanged();

        var evidence = TestEnvironment.Optional(TestEnvironment.Evidence);
        if (evidence is not null)
        {
            Directory.CreateDirectory(evidence);
            var result = new
            {
                family, engine, browserVersion = session.BrowserVersion, basePath = "/" + prefix, bootMs = session.BootMs, totalMs = session.ElapsedMs,
                slotKind = "box", noOpMatchesNative = true, editedMatchesNative = true, reopened = true, originalUnchanged = true, privacyPassed = true,
            };
            var file = $"{family}-{engine}-{(prefix.Length == 0 ? "root" : "subpath")}.json";
            await File.WriteAllTextAsync(Path.Combine(evidence, file), JsonSerializer.Serialize(result));
        }
    }
}

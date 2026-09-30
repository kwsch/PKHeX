using System.Globalization;
using Microsoft.Playwright;
using PKHeX.Core;
using PKHeX.Web.Components;
using PKHeX.Web.State;
using Xunit;
using static Microsoft.Playwright.Assertions;

namespace PKHeX.Web.Tests;

/// <summary>
/// Drives the current proof UI (nickname-only editor) and holds the byte-level oracles its tests share.
/// </summary>
/// <remarks>
/// Kept apart from <see cref="PublishedAppFixture"/> because the MVP screens replace this UI, while hosting and privacy checks stay.
/// </remarks>
internal static class ProofPage
{
    /// <summary>Opens <paramref name="bytes"/> through the file picker as a file named <paramref name="name"/>.</summary>
    public static async Task Load(IPage page, byte[] bytes, string name = "main")
    {
        await page.Locator("#save-file").SetInputFilesAsync(new FilePayload
        {
            Name = name, MimeType = "application/octet-stream", Buffer = bytes,
        });
    }

    /// <summary>Clicks Download and returns the downloaded bytes, checking the suggested name is <paramref name="expectedName"/>.</summary>
    public static async Task<byte[]> Download(IPage page, string expectedName = "main")
    {
        var download = await page.RunAndWaitForDownloadAsync(() => page.Locator("#download").ClickAsync());
        var path = await download.PathAsync();
        Assert.True(path is not null, "No local download was produced.");
        Assert.True(download.SuggestedFilename == expectedName, "Unexpected download naming.");
        return await File.ReadAllBytesAsync(path!);
    }

    /// <summary>Opens <paramref name="slot"/> from the party or box grid and waits for its editor to show that position.</summary>
    public static async Task Select(IPage page, SlotRef slot)
    {
        if (slot.IsParty)
        {
            await page.Locator($"#party-grid-{slot.Slot}").ClickAsync();
        }
        else
        {
            await page.Locator("#box-select").SelectOptionAsync(slot.Box.ToString(CultureInfo.InvariantCulture));
            await page.Locator($"#box-grid-{slot.Slot}").ClickAsync();
        }
        await Expect(page.Locator("#draft-slot")).ToHaveTextAsync(SlotText.Position(slot));
    }

    /// <summary>Opens box 1, slot 1, where the synthetic saves store their entity.</summary>
    public static Task Select(IPage page) => Select(page, SaveFixtures.FirstBoxSlot);

    /// <summary>The status text the app shows for <paramref name="analysis"/>.</summary>
    public static string Verdict(LegalityAnalysis analysis) => analysis.Parsed ? analysis.Valid ? "Valid" : "Invalid" : "Unavailable";

    /// <summary>Runs legality in the browser and compares verdict and report with native Core on the same entity.</summary>
    public static async Task CheckLegality(IPage page, PKM pk, SaveFile save, StorageSlotType type = StorageSlotType.Box)
    {
        var native = new LegalityAnalysis(pk.Clone(), save.Personal, type);
        await page.Locator("#analyze").ClickAsync();
        await Expect(page.Locator("#legality-status")).ToHaveTextAsync(Verdict(native));

        // Do not include private reports in assertion output or public test artifacts.
        var actual = await page.Locator("#legality-report").TextContentAsync();
        Assert.True(actual == native.Report(), "Browser/native legality reports differ (contents withheld).");
    }

    /// <summary>Concatenated stored party entity data.</summary>
    public static byte[] PartyBytes(SaveFile save) => save.PartyData.SelectMany(p => p.Data.ToArray()).ToArray();

    /// <summary>Fails if <paramref name="result"/> differs from <paramref name="original"/> in anything but nickname storage and the nickname flag.</summary>
    public static void AssertOnlyNicknameChanged(PKM original, PKM result)
    {
        // Restore only nickname storage and its flag, then compare every other entity byte.
        var restored = result.Clone();
        original.NicknameTrash.CopyTo(restored.NicknameTrash);
        restored.IsNicknamed = original.IsNicknamed;
        restored.RefreshChecksum();

        var reference = original.Clone();
        reference.RefreshChecksum();
        Assert.True(restored.Data.SequenceEqual(reference.Data), "Non-nickname entity data changed, including party stats/HP/status/ownership.");
    }

    /// <summary>
    /// Save-level check: every byte differing between the no-op and edited exports lies in the edited
    /// box slot or the Gen 6 block-info footer (block checksums). Party, dex, records and handler data are untouched.
    /// </summary>
    public static void AssertOnlyRangeDiffers(byte[] before, byte[] after, int start, int length)
    {
        Assert.True(before.Length == after.Length, "Export length changed.");
        var footer = before.Length - 0x200;
        for (int i = 0; i < before.Length; i++)
        {
            if (before[i] == after[i] || (i >= start && i < start + length) || i >= footer)
            {
                continue;
            }
            Assert.Fail("Export changed data outside the edited slot and checksum footer (details withheld).");
        }
    }

    /// <summary>A copy of <paramref name="bytes"/> with one bit flipped, to break a checksum.</summary>
    public static byte[] Corrupt(byte[] bytes)
    {
        var result = bytes.ToArray();
        result[0] ^= 1;
        return result;
    }
}

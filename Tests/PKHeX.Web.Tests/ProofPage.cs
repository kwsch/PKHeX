using System.Globalization;
using System.Text.RegularExpressions;
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
        var (bytes, name) = await DownloadNamed(page);
        Assert.True(name == expectedName, "Unexpected download naming.");
        return bytes;
    }

    /// <summary>
    /// Clicks Download on an edited session and returns the downloaded bytes, checking the suggested name is the edited name of
    /// <paramref name="stem"/> and <paramref name="extension"/>, stamped with some local date-time.
    /// </summary>
    public static async Task<byte[]> DownloadEdited(IPage page, string stem = "main", string extension = "")
    {
        var (bytes, name) = await DownloadNamed(page);
        Assert.Matches($"^{Regex.Escape(stem)}-modified-\\d{{4}}-\\d{{2}}-\\d{{2}}-\\d{{6}}{Regex.Escape(extension)}$", name);
        return bytes;
    }

    /// <summary>
    /// Clicks <paramref name="button"/> (Download by default) and returns the downloaded bytes and suggested name. When the download contains
    /// changes legality flagged, it first ticks the acknowledgement beside the button, as the user must (<c>#export-ack</c>, or
    /// <c>#exit-export-ack</c> in the exit panel); tests of the acknowledgement itself check it explicitly.
    /// </summary>
    public static async Task<(byte[] Bytes, string Name)> DownloadNamed(IPage page, string button = "#download")
    {
        var acknowledgement = page.Locator(button == "#exit-export" ? "#exit-export-ack" : "#export-ack");
        if (await acknowledgement.CountAsync() > 0)
        {
            await acknowledgement.CheckAsync();
        }
        var download = await page.RunAndWaitForDownloadAsync(() => page.Locator(button).ClickAsync());
        var path = await download.PathAsync();
        Assert.True(path is not null, "No local download was produced.");
        return (await File.ReadAllBytesAsync(path!), download.SuggestedFilename);
    }

    /// <summary>A legality verdict for the draft as it is now, as <c>#legality-status</c> shows it.</summary>
    public static readonly Regex VerdictPattern = new("^(Valid|Invalid|Unavailable)$");

    /// <summary>
    /// Waits until the changed draft can be applied, as the user must: for the legality result of the draft as it is now, then ticking the
    /// acknowledgement of an Invalid or Unavailable one (<c>#apply-ack</c>). Tests of the acknowledgement itself check it explicitly.
    /// </summary>
    public static async Task ReadyToApply(IPage page)
    {
        await Expect(page.Locator("#legality-status")).ToHaveTextAsync(VerdictPattern);
        var acknowledgement = page.Locator("#apply-ack");
        if (await acknowledgement.CountAsync() > 0)
        {
            await acknowledgement.CheckAsync();
        }
        await Expect(page.Locator("#apply")).ToBeEnabledAsync();
    }

    /// <summary><see cref="ReadyToApply"/>, then clicks Apply.</summary>
    public static async Task Apply(IPage page)
    {
        await ReadyToApply(page);
        await page.Locator("#apply").ClickAsync();
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

    /// <summary>Runs legality in the browser and compares verdict and report with native Core on the same entity.</summary>
    public static async Task CheckLegality(IPage page, PKM pk, SaveFile save, StorageSlotType type = StorageSlotType.Box)
    {
        var native = NativeLegality.Of(save, pk, type);
        await page.Locator("#analyze").ClickAsync();
        await Expect(page.Locator("#legality-status")).ToHaveTextAsync(native.Verdict);

        // Do not include private reports in assertion output or public test artifacts.
        var actual = await page.Locator("#legality-report").TextContentAsync();
        Assert.True(actual == native.Report, "Browser/native legality reports differ (contents withheld).");
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
    /// slot (a box slot or a party position) or the Gen 6 block-info footer (block checksums). Other slots, dex, records and handler data are untouched.
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

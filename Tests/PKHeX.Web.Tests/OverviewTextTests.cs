using System.Globalization;
using FluentAssertions;
using PKHeX.Core;
using PKHeX.Web.Components;
using PKHeX.Web.Services;
using Xunit;

namespace PKHeX.Web.Tests;

/// <summary>
/// The exact overview text: full values with units, unknown values labelled, and no dependence on the browser's locale.
/// </summary>
/// <remarks>The E2E tier builds its expected text from these pinned strings, not from the mapping under test.</remarks>
[Trait(TestCategory.Name, TestCategory.Unit)]
public sealed class OverviewTextTests
{
    private static readonly SaveOverview Known = new(
        GameVersion.X, true, SupportMatrix.Families[0], 6, "Serena", (int)LanguageID.French, "FRA (Français)",
        TrainerIDFormat.SixteenBit, 42, 54321, 123, 4, 5, 1234567, new DateTime(2024, 5, 6, 7, 8, 0), "main", SaveUtil.SIZE_G6XY);

    [Fact]
    public void KnownValuesAreShownInFull()
    {
        OverviewText.Game(Known).Should().Be("X");
        OverviewText.Family(Known).Should().Be("Pokémon X and Y");
        OverviewText.Trainer(Known).Should().Be("Serena");
        OverviewText.Language(Known).Should().Be("FRA (Français)");
        OverviewText.TrainerId(Known).Should().Be("00042", "Gen 6 shows five-digit, zero-padded IDs");
        OverviewText.SecretId(Known).Should().Be("54321");
        OverviewText.PlayTime(Known).Should().Be("123 h 04 min 05 s");
        OverviewText.Money(Known).Should().Be("1,234,567 Pokédollars");
        OverviewText.LastSaved(Known).Should().Be("2024-05-06 07:08");
        OverviewText.Size(Known).Should().Be("415,232 bytes (405.5 KiB)");
        OverviewText.Format(Known).Should().Be("Raw Generation 6 save");
        OverviewText.Integrity.Should().Be("Checksums valid and unchanged round trip verified when opened. Every download is revalidated.");
    }

    [Fact]
    public void OrasGameAndSizeAreNamed()
    {
        var oras = Known with { Version = GameVersion.AS, Family = SupportMatrix.Families[1], SizeBytes = SaveUtil.SIZE_G6ORAS };
        OverviewText.Game(oras).Should().Be("Alpha Sapphire");
        OverviewText.Family(oras).Should().Be("Pokémon Omega Ruby and Alpha Sapphire");
        OverviewText.Size(oras).Should().Be("483,328 bytes (472 KiB)");
    }

    [Fact]
    public void ATrainerNameIsShownWithoutBidiControls()
    {
        OverviewText.Trainer(Known with { TrainerName = "\u202E<b>Ser</b>" }).Should().Be("<b>Ser</b>");
        OverviewText.Trainer(Known with { TrainerName = "\u202E \u200F" }).Should().Be("Not set in this save", "nothing of it would be shown");
    }

    [Fact]
    public void UnknownValuesAreLabelledNotInvented()
    {
        var unknown = Known with { VersionValid = false, Version = (GameVersion)0, TrainerName = null, Language = 6, LanguageName = null, LastSaved = null };
        OverviewText.Game(unknown).Should().Be("Unknown (stored value 0)");
        OverviewText.Trainer(unknown).Should().Be("Not set in this save");
        OverviewText.Language(unknown).Should().Be("Unknown (stored value 6)");
        OverviewText.LastSaved(unknown).Should().Be("Not recorded");
    }

    [Fact]
    public void LargestValuesAreNotShortened()
    {
        var large = Known with { PlayedHours = 65535, PlayedMinutes = 59, PlayedSeconds = 59, Money = 9999999, DisplayTid = 65535, DisplaySid = 0 };
        OverviewText.PlayTime(large).Should().Be("65535 h 59 min 59 s");
        OverviewText.Money(large).Should().Be("9,999,999 Pokédollars");
        OverviewText.TrainerId(large).Should().Be("65535");
        OverviewText.SecretId(large).Should().Be("00000");
    }

    [Fact]
    public void TextDoesNotDependOnTheCurrentCulture()
    {
        var invariant = All(Known);
        var previous = CultureInfo.CurrentCulture;
        try
        {
            foreach (var name in new[] { "de-DE", "fr-FR", "ar-SA", "hi-IN" })
            {
                CultureInfo.CurrentCulture = new CultureInfo(name);
                All(Known).Should().Equal(invariant, "the page must read the same in every browser locale ({0})", name);
            }
        }
        finally
        {
            CultureInfo.CurrentCulture = previous;
        }
    }

    private static string[] All(SaveOverview o) =>
    [
        OverviewText.Game(o), OverviewText.Trainer(o), OverviewText.Language(o), OverviewText.TrainerId(o), OverviewText.SecretId(o),
        OverviewText.PlayTime(o), OverviewText.Money(o), OverviewText.LastSaved(o), OverviewText.Size(o), OverviewText.Format(o),
    ];
}

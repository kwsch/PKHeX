using Xunit;

namespace PKHeX.Web.Tests;

/// <summary>
/// Keeps <c>PKHeX.Web/THIRD-PARTY-NOTICES.md</c> in step with the Web restore graph, so a new or updated package cannot ship without a notice.
/// </summary>
/// <remarks>
/// The runtime pack, ILLink and WebAssembly SDK pack versions come from the installed .NET SDK, so these checks hold only on the SDK the publish is built with (pinned in CI).
/// They are in the opt-in E2E tier with the publish checks, not in Unit, so that builds on another SDK, such as upstream's Azure pipeline on its image's SDK, do not fail on notices they never publish.
/// The publish itself is checked against these tables by <see cref="PublishedAppTests.PublishesLicenseAndNotices"/>.
/// </remarks>
[Trait(TestCategory.Name, TestCategory.E2E)]
public sealed class NoticesRestoreGraphTests
{
    [TierFact(TestCategory.E2E)]
    public void ListsEveryRestoredPackageOnceWithItsVersion()
    {
        var rows = NoticesInventory.Rows();
        var restored = NoticesInventory.RestoredPackages();
        Assert.NotEmpty(restored);

        var duplicates = rows.GroupBy(r => r.Id, StringComparer.OrdinalIgnoreCase).Where(g => g.Count() > 1).Select(g => g.Key);
        Assert.Empty(duplicates);

        var listed = rows.ToDictionary(r => r.Id, r => r.Version, StringComparer.OrdinalIgnoreCase);
        var missing = restored.Where(p => !listed.TryGetValue(p.Key, out var version) || version != p.Value).Select(p => $"{p.Key} {p.Value}");
        Assert.Empty(missing);

        var stale = listed.Where(p => !restored.ContainsKey(p.Key)).Select(p => p.Key);
        Assert.Empty(stale);
    }

    [TierFact(TestCategory.E2E)]
    public void PublishedPackagesNameTheNoticesThatCoverThem()
    {
        var rows = NoticesInventory.Rows();
        var published = rows.Where(r => r.Section == NoticesInventory.Section.Published).ToList();
        Assert.NotEmpty(published);
        Assert.All(published, r => Assert.True(r.Notices is not null, $"{r.Id} is published but names no upstream notices file."));
        Assert.All(rows.Except(published), r => Assert.True(r.Notices is null, $"{r.Id} is not published but names a published notices file."));

        // Every package mapped to one published file must carry exactly that file upstream.
        foreach (var group in published.GroupBy(r => r.Notices))
        {
            var expected = NoticesInventory.UpstreamNotices(group.First().Id, group.First().Version);
            foreach (var row in group)
            {
                Assert.True(NoticesInventory.UpstreamNotices(row.Id, row.Version).AsSpan().SequenceEqual(expected), $"{row.Id} carries different upstream notices than the others mapped to {group.Key}.");
            }
        }
    }
}

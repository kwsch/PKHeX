using Xunit;

namespace PKHeX.Web.Tests;

/// <summary>
/// Maps published file names back to the package file names that <see cref="NoticesInventory"/> matches against.
/// </summary>
/// <remarks>
/// The notices tables are checked against the restore graph by <see cref="NoticesRestoreGraphTests"/>
/// and against the publish by <see cref="PublishedAppTests.PublishesLicenseAndNotices"/>, both in the E2E tier.
/// </remarks>
[Trait(TestCategory.Name, TestCategory.Unit)]
public sealed class ThirdPartyNoticesTests
{
    [Theory]
    [InlineData("Microsoft.JSInterop.6toomhpa9w.wasm", new[] { "Microsoft.JSInterop.6toomhpa9w.wasm", "Microsoft.JSInterop.6toomhpa9w.dll", "Microsoft.JSInterop.wasm", "Microsoft.JSInterop.dll" })]
    [InlineData("dotnet.native.rw4kynp763.wasm", new[] { "dotnet.native.rw4kynp763.wasm", "dotnet.native.rw4kynp763.dll", "dotnet.native.wasm", "dotnet.native.dll" })]
    [InlineData("blazor.webassembly.js", new[] { "blazor.webassembly.js" })]
    [InlineData("System.Collections.Concurrent.wasm", new[] { "System.Collections.Concurrent.wasm", "System.Collections.Concurrent.dll" })]
    public void MapsPublishedNamesToPackageFileNames(string published, string[] expected) => Assert.Equal(expected, NoticesInventory.PackageFileNames(published));
}

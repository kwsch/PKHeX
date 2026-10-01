using System.Reflection;

namespace PKHeX.Web.Services;

/// <summary>
/// Identifies the build that is running, so a report can name the exact Web version and source revision without a remote lookup.
/// </summary>
/// <remarks>
/// The values are embedded as <see cref="AssemblyMetadataAttribute"/>s by the <c>AddBuildProvenance</c> target in <c>PKHeX.Web.csproj</c>.
/// Core is built from the same repository, so <see cref="SourceCommit"/> also identifies the Core revision.
/// A value that was not recorded is reported as <see cref="Unknown"/>, never guessed.
/// </remarks>
public static class BuildInfo
{
    /// <summary>Reported for any value that the build did not record.</summary>
    public const string Unknown = "unknown";

    /// <summary>Metadata key for the Web version.</summary>
    internal const string VersionKey = "PKHeXWebVersion";

    /// <summary>Metadata key for the source commit.</summary>
    internal const string CommitKey = "PKHeXSourceCommit";

    /// <summary>Metadata key recording whether the sprite atlas was published with this build.</summary>
    internal const string SpritesKey = "PKHeXWebSprites";

    private static readonly AssemblyMetadataAttribute[] Metadata = [.. typeof(BuildInfo).Assembly.GetCustomAttributes<AssemblyMetadataAttribute>()];

    /// <summary>Release version of this build, shared with the rest of the repository.</summary>
    public static string WebVersion { get; } = Read(Metadata, VersionKey);

    /// <summary>Full git commit of the source tree this build was made from, or <see cref="Unknown"/>.</summary>
    public static string SourceCommit { get; } = Read(Metadata, CommitKey);

    /// <summary>
    /// True when this build was published with the sprite atlas (<c>-p:PKHeXWebSprites=true</c>). Otherwise slots are shown as text only,
    /// and nothing under <c>sprites/</c> is ever requested.
    /// </summary>
    public static bool SpritesIncluded { get; } = ReadFlag(Metadata, SpritesKey);

    /// <summary>True only when <paramref name="key"/> is recorded as <c>true</c>; a missing or other value is false.</summary>
    internal static bool ReadFlag(IEnumerable<AssemblyMetadataAttribute> metadata, string key) =>
        string.Equals(Read(metadata, key), "true", StringComparison.OrdinalIgnoreCase);

    /// <summary>Returns the value recorded under <paramref name="key"/>, or <see cref="Unknown"/> if it is missing or blank.</summary>
    internal static string Read(IEnumerable<AssemblyMetadataAttribute> metadata, string key)
    {
        var value = metadata.FirstOrDefault(m => m.Key == key)?.Value;
        return string.IsNullOrWhiteSpace(value) ? Unknown : value.Trim();
    }
}

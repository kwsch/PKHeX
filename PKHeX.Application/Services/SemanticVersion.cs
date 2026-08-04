using System.Linq;
using System.Text.RegularExpressions;

namespace PKHeX.Application.Services;

/// <summary>
/// Minimal SemVer 2.0.0 parser/comparer (major.minor.patch[-prerelease][+build]). Pure and
/// dependency-free so it can be unit tested in isolation — used to compare GitHub release tags
/// (e.g. "v1.26.0", "1.26.0-beta.1") against the running <c>UIVersion</c>.
/// </summary>
public readonly struct SemanticVersion : IComparable<SemanticVersion>, IEquatable<SemanticVersion>
{
    private static readonly Regex Pattern = new(
        @"^[vV]?(?<major>\d+)\.(?<minor>\d+)\.(?<patch>\d+)(?:-(?<pre>[0-9A-Za-z\-.]+))?(?:\+[0-9A-Za-z\-.]+)?$",
        RegexOptions.Compiled);

    public int Major { get; }
    public int Minor { get; }
    public int Patch { get; }

    /// <summary>Dot-separated pre-release identifiers (e.g. ["beta", "1"]). Empty for a stable release.</summary>
    public IReadOnlyList<string> PreRelease { get; }

    public bool IsPreRelease => PreRelease.Count > 0;

    private SemanticVersion(int major, int minor, int patch, IReadOnlyList<string> preRelease)
    {
        Major = major;
        Minor = minor;
        Patch = patch;
        PreRelease = preRelease;
    }

    public static bool TryParse(string? input, out SemanticVersion version)
    {
        version = default;
        if (string.IsNullOrWhiteSpace(input))
            return false;

        var match = Pattern.Match(input.Trim());
        if (!match.Success)
            return false;

        var major = int.Parse(match.Groups["major"].Value);
        var minor = int.Parse(match.Groups["minor"].Value);
        var patch = int.Parse(match.Groups["patch"].Value);
        var preRelease = match.Groups["pre"].Success
            ? match.Groups["pre"].Value.Split('.')
            : [];

        version = new SemanticVersion(major, minor, patch, preRelease);
        return true;
    }

    public static SemanticVersion Parse(string input)
    {
        if (!TryParse(input, out var version))
            throw new FormatException($"'{input}' is not a valid semantic version.");
        return version;
    }

    public int CompareTo(SemanticVersion other)
    {
        var major = Major.CompareTo(other.Major);
        if (major != 0) return major;

        var minor = Minor.CompareTo(other.Minor);
        if (minor != 0) return minor;

        var patch = Patch.CompareTo(other.Patch);
        if (patch != 0) return patch;

        // SemVer 2.0.0 precedence rule #11: a stable release outranks a pre-release of the
        // same major.minor.patch; among two pre-releases, compare identifiers left to right.
        if (!IsPreRelease && !other.IsPreRelease) return 0;
        if (!IsPreRelease) return 1;
        if (!other.IsPreRelease) return -1;

        var count = Math.Min(PreRelease.Count, other.PreRelease.Count);
        for (var i = 0; i < count; i++)
        {
            var cmp = ComparePreReleaseIdentifier(PreRelease[i], other.PreRelease[i]);
            if (cmp != 0) return cmp;
        }

        // All shared identifiers equal: the longer set has higher precedence.
        return PreRelease.Count.CompareTo(other.PreRelease.Count);
    }

    private static int ComparePreReleaseIdentifier(string a, string b)
    {
        var aNumeric = IsNumeric(a);
        var bNumeric = IsNumeric(b);

        if (aNumeric && bNumeric)
            return long.Parse(a).CompareTo(long.Parse(b));
        if (aNumeric) return -1; // numeric identifiers always have lower precedence than alphanumeric
        if (bNumeric) return 1;
        return string.CompareOrdinal(a, b);
    }

    private static bool IsNumeric(string s) => s.Length > 0 && s.All(char.IsAsciiDigit);

    public bool Equals(SemanticVersion other) => CompareTo(other) == 0;
    public override bool Equals(object? obj) => obj is SemanticVersion other && Equals(other);
    public override int GetHashCode() => HashCode.Combine(Major, Minor, Patch);

    public override string ToString() =>
        IsPreRelease ? $"{Major}.{Minor}.{Patch}-{string.Join('.', PreRelease)}" : $"{Major}.{Minor}.{Patch}";

    public static bool operator ==(SemanticVersion left, SemanticVersion right) => left.CompareTo(right) == 0;
    public static bool operator !=(SemanticVersion left, SemanticVersion right) => left.CompareTo(right) != 0;
    public static bool operator <(SemanticVersion left, SemanticVersion right) => left.CompareTo(right) < 0;
    public static bool operator >(SemanticVersion left, SemanticVersion right) => left.CompareTo(right) > 0;
    public static bool operator <=(SemanticVersion left, SemanticVersion right) => left.CompareTo(right) <= 0;
    public static bool operator >=(SemanticVersion left, SemanticVersion right) => left.CompareTo(right) >= 0;
}

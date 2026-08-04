using System.Security.Cryptography;
using System.Text;

namespace PKHeX.Application.Services;

/// <summary>
/// Computes a stable, filesystem-safe folder name for a save file's backups, derived from its path.
/// Same path always maps to the same identity (so backups accumulate in one place); different paths
/// with the same file name still get distinct folders because a short hash of the full path is appended.
/// </summary>
public static class SaveIdentity
{
    public static string Compute(string savePath)
    {
        var fullPath = Path.GetFullPath(savePath);
        var baseName = Path.GetFileNameWithoutExtension(fullPath);
        var safeName = Sanitize(baseName);

        var hashInput = Encoding.UTF8.GetBytes(NormalizeForHash(fullPath));
        var hash = Convert.ToHexString(SHA256.HashData(hashInput))[..8].ToLowerInvariant();

        return string.IsNullOrEmpty(safeName) ? hash : $"{safeName}_{hash}";
    }

    // Case-insensitive filesystems (Windows/macOS) should treat differently-cased paths as the same save.
    private static string NormalizeForHash(string fullPath) => fullPath.ToLowerInvariant();

    private static string Sanitize(string name)
    {
        var invalid = Path.GetInvalidFileNameChars();
        var sb = new StringBuilder(name.Length);
        foreach (var c in name)
            sb.Append(invalid.Contains(c) ? '_' : c);
        return sb.ToString();
    }
}

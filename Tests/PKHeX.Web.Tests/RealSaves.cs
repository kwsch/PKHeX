using System.Security.Cryptography;
using PKHeX.Core;

namespace PKHeX.Web.Tests;

/// <summary>
/// Private real XY/ORAS saves for the <see cref="TestCategory.RealSave"/> tier.
/// </summary>
/// <remarks>
/// Saves are supplied only through <see cref="TestEnvironment.XYSave"/> and <see cref="TestEnvironment.ORASSave"/>; they are never committed and never used in CI.
/// A missing variable or a fixture that fails native validation fails the test.
/// Failure messages name the variable, never the file path, bytes, names or identifiers.
/// </remarks>
internal static class RealSaves
{
    /// <summary>Real-save families, as reported by the Web session.</summary>
    public static readonly string[] Families = ["XY", "ORAS"];

    /// <summary>A validated private save: its bytes, their SHA-256, and the native Core parse.</summary>
    public sealed class Fixture
    {
        private readonly string path;
        private readonly string variable;

        internal Fixture(string path, string variable, byte[] bytes, SaveFile native)
        {
            this.path = path;
            this.variable = variable;
            Bytes = bytes;
            Hash = SHA256.HashData(bytes);
            Native = native;
        }

        /// <summary>The file contents read at the start of the test.</summary>
        public byte[] Bytes { get; }

        /// <summary>SHA-256 of <see cref="Bytes"/>.</summary>
        public byte[] Hash { get; }

        /// <summary>Native Core parse of <see cref="Bytes"/>.</summary>
        public SaveFile Native { get; }

        /// <summary>Fails if the file on disk no longer matches the bytes read at the start of the test.</summary>
        public void AssertUnchanged()
        {
            if (!SHA256.HashData(ReadPrivate(path, variable)).AsSpan().SequenceEqual(Hash))
            {
                throw new InvalidOperationException("The original fixture changed.");
            }
        }

        /// <summary>Withholds the path, which a default or record <c>ToString</c> would print.</summary>
        public override string ToString() => $"{variable} fixture (details withheld)";
    }

    /// <summary>
    /// Reads the private save for <paramref name="family"/> and validates it natively before any Web code sees it.
    /// </summary>
    public static Fixture Read(string family)
    {
        var variable = family switch
        {
            "XY" => TestEnvironment.XYSave,
            "ORAS" => TestEnvironment.ORASSave,
            _ => throw new ArgumentOutOfRangeException(nameof(family), family, null),
        };
        var path = TestEnvironment.Required(variable);
        var bytes = ReadPrivate(path, variable);
        var native = SaveFixtures.Parse(bytes);

        if (native is not (SAV6XY or SAV6AO))
        {
            throw new InvalidOperationException("Real fixture is not a raw XY/ORAS save.");
        }
        if ((native is SAV6XY ? "XY" : "ORAS") != family)
        {
            throw new InvalidOperationException("Real fixture family mismatch.");
        }
        if (!native.ChecksumsValid || !native.State.Exportable)
        {
            throw new InvalidOperationException("Real fixture failed native integrity validation.");
        }
        return new Fixture(path, variable, bytes, native);
    }

    /// <summary>Reads a private file; IO errors are rethrown without the inner exception, whose message contains the path.</summary>
    private static byte[] ReadPrivate(string path, string variable)
    {
        try
        {
            return File.ReadAllBytes(path);
        }
        catch (Exception e) when (e is IOException or UnauthorizedAccessException)
        {
            throw new InvalidOperationException($"{variable} does not point to a readable file ({e.GetType().Name}; path withheld).");
        }
    }
}

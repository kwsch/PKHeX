using System.Security.Cryptography;
using PKHeX.Core;
using PKHeX.Web.Services;
using Xunit;

namespace PKHeX.Web.Tests;

/// <summary>Native Core validation of the private real fixtures before any browser run.</summary>
public sealed class RealSavePreflightTests
{
    [Theory]
    [InlineData("PKHEX_XY_SAVE", "XY")]
    [InlineData("PKHEX_ORAS_SAVE", "ORAS")]
    public void RealSaveNativePreflight(string variable, string family)
    {
        var path = Environment.GetEnvironmentVariable(variable)
            ?? throw new InvalidOperationException($"Set {variable}; this proof must not silently skip required evidence.");
        var bytes = File.ReadAllBytes(path);
        var hash = SHA256.HashData(bytes);
        var native = ProofFixtures.Parse(bytes);
        Assert.True(native is SAV6XY or SAV6AO, "Real fixture is not a raw XY/ORAS save.");
        Assert.True((native is SAV6XY ? "XY" : "ORAS") == family, "Real fixture family mismatch.");
        Assert.True(native.ChecksumsValid && native.State.Exportable, "Real fixture failed native integrity validation.");
        var index = ProofFixtures.WritableSlot(native);
        Assert.True(ProofFixtures.Slot(native, index).Read(native) is PK6 { ChecksumValid: true });

        // The Web session algorithm must agree with native Core on the same bytes.
        var session = SaveLoader.Load(bytes);
        Assert.True(session.Family == family);
        Assert.Contains(index, session.OccupiedSlots);
        Assert.True(SaveExporter.Export(session, null).AsSpan().SequenceEqual(native.Clone().Write().Span), "Session no-op export differs from native.");
        Assert.True(SHA256.HashData(File.ReadAllBytes(path)).AsSpan().SequenceEqual(hash), "The original fixture changed.");
    }
}

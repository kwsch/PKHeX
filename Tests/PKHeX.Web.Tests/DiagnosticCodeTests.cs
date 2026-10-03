using FluentAssertions;
using PKHeX.Core;
using PKHeX.Web.Services;
using PKHeX.Web.Services.Diagnostics;
using PKHeX.Web.State;
using Xunit;

namespace PKHeX.Web.Tests;

/// <summary>
/// Redacted failure codes and the diagnostic log (WEB-SEC-003, WEB-SEC-005): built from the app's own names only, never from an exception's
/// message, and written to the console in the same redacted form.
/// </summary>
[Trait(TestCategory.Name, TestCategory.Unit)]
public sealed class DiagnosticCodeTests
{
    /// <summary>Stands in for a save-derived value (a nickname) carried in an exception's message.</summary>
    private const string Sentinel = "ZZSECRETNAME";

    [Fact]
    public void EveryLoadFailureAndSessionErrorHasItsOwnCode()
    {
        var load = Enum.GetValues<LoadFailure>().Where(f => f is not LoadFailure.RecognizedNotEnabled and not LoadFailure.IntegrityFailed)
            .Select(f => DiagnosticCode.For(SaveLoadOutcome.Failed(f)).Name)
            .Concat(Enum.GetValues<IntegrityProblem>().Select(p => DiagnosticCode.For(SaveLoadOutcome.IntegrityFailed(Recognized, p)).Name))
            .Append(DiagnosticCode.For(SaveLoadOutcome.NotEnabled(Recognized)).Name)
            .ToArray();
        var session = Enum.GetValues<SessionError>().Select(e => DiagnosticCode.For(e).Name).ToArray();

        load.Should().OnlyHaveUniqueItems().And.OnlyContain(n => n.StartsWith("open.", StringComparison.Ordinal));
        session.Should().OnlyHaveUniqueItems().And.OnlyContain(n => n.StartsWith("session.", StringComparison.Ordinal));
        load.Concat(session).Should().OnlyContain(n => n == n.ToLowerInvariant() && !n.Contains(' '));
    }

    [Fact]
    public void CodesNameTheOutcome()
    {
        DiagnosticCode.For(SaveLoadOutcome.Failed(LoadFailure.ParserFault)).Name.Should().Be("open.parser-fault");
        DiagnosticCode.For(SaveLoadOutcome.Failed(LoadFailure.TooLarge)).Name.Should().Be("open.too-large");
        DiagnosticCode.For(SaveLoadOutcome.IntegrityFailed(Recognized, IntegrityProblem.RoundTripMismatch)).Name.Should().Be("open.integrity.round-trip-mismatch");
        DiagnosticCode.For(SessionError.StagedEditMismatch).Name.Should().Be("session.staged-edit-mismatch");
        DiagnosticCode.For(new SessionException(SessionError.ExportEntityMismatch)).Name.Should().Be("session.export-entity-mismatch");
    }

    [Fact]
    public void ARecognisedSaveIsNamedByItsCoreType()
    {
        var code = DiagnosticCode.For(SaveLoadOutcome.NotEnabled(Recognized));
        code.Name.Should().Be("open.recognized-not-enabled");
        code.SaveType.Should().Be(nameof(SAV5BW));
        DiagnosticCode.For(SaveLoadOutcome.Failed(LoadFailure.Unrecognized)).SaveType.Should().BeNull();
    }

    [Fact]
    public void ASuccessfulOpenHasNoCode()
    {
        var opened = SaveLoadOutcome.Opened(SaveFixtures.Open(SaveFixtures.Synthetic(false)));
        FluentActions.Invoking(() => DiagnosticCode.For(opened)).Should().Throw<ArgumentException>();
    }

    [Fact]
    public void AnUnexpectedExceptionKeepsItsTypesAndFramesButNeverItsMessage()
    {
        var thrown = Catch(() => Thrower.Outer());
        var code = DiagnosticCode.For(thrown);

        code.Name.Should().Be("unexpected");
        code.ExceptionTypes.Should().Equal(typeof(InvalidOperationException).FullName, typeof(ArgumentOutOfRangeException).FullName);
        code.Frames.Should().NotBeEmpty();
        code.Frames[0].Should().Be($"{typeof(Thrower).FullName!.Replace('+', '.')}.{nameof(Thrower.Inner)}", "frames come from the innermost exception, throwing frame first");
        code.Frames.Should().OnlyContain(f => !f.Contains('(') && !f.Contains(" in ") && !f.Contains(":line"), "arguments, files and lines are removed");
        code.ToString().Should().NotContain(Sentinel);
        string.Join("\n", code.ExceptionTypes.Concat(code.Frames)).Should().NotContain(Sentinel);
    }

    [Fact]
    public void FramesKeepOnlyTheMethodNames()
    {
        const string trace = """
               at PKHeX.Core.SaveUtil.GetSaveFile(Byte[] data, SaveFileType type) in /build/PKHeX.Core/Saves/SaveUtil.cs:line 120
               at PKHeX.Web.Services.SaveLoader.<>c.<Load>b__0_0(Byte[] d)
            --- End of stack trace from previous location ---
               at PKHeX.Web.Services.SaveLoader.Load in /build/SaveLoader.cs:line 40
               at Some.Wasm.Frame
            """;
        DiagnosticCode.ParseFrames(trace).Should().Equal(
            "PKHeX.Core.SaveUtil.GetSaveFile",
            "PKHeX.Web.Services.SaveLoader.<>c.<Load>b__0_0",
            "PKHeX.Web.Services.SaveLoader.Load",
            "Some.Wasm.Frame");
        DiagnosticCode.ParseFrames(null).Should().BeEmpty();
        // Seen in the published app: generic arguments are assembly-qualified, and would use up the length cap before the method name.
        DiagnosticCode.ParseFrames("   at Microsoft.JSInterop.JSRuntime.<InvokeAsync>d__23`1[[System.Boolean, System.Private.CoreLib, Version=10.0.0.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e]].MoveNext()")
            .Should().Equal("Microsoft.JSInterop.JSRuntime.<InvokeAsync>d__23`1.MoveNext");
        DiagnosticCode.ParseFrames("   at A.B`2[[X.Y, Lib, Version=1.0.0.0],[Z[[Q, Lib]], Lib]].C()").Should().Equal("A.B`2.C");
        DiagnosticCode.ParseFrames(string.Concat(Enumerable.Repeat("   at A.B()\n", 40))).Should().HaveCount(DiagnosticCode.MaxFrames);
        DiagnosticCode.ParseFrames($"   at {new string('x', 500)}()").Single().Should().HaveLength(200);
    }

    [Fact]
    public void ADeepInnerChainIsCapped()
    {
        Exception e = new InvalidOperationException(Sentinel);
        for (int i = 0; i < 10; i++)
        {
            e = new InvalidOperationException(Sentinel, e);
        }
        DiagnosticCode.For(e).ExceptionTypes.Should().HaveCount(DiagnosticCode.MaxTypes);
    }

    [Fact]
    public void ANotParsedAnalysisIsNamedByItsType()
    {
        DiagnosticCode.For(new LegalityNotParsedException()).ExceptionTypes.Should().Equal(typeof(LegalityNotParsedException).FullName);
    }

    [Fact]
    public void TheLogKeepsTheNewestEntriesAndWritesThemRedactedToTheConsole()
    {
        var log = DiagnosticFixtures.NewLog(out var console);
        var changes = 0;
        log.Changed += () => changes++;

        log.Record(DiagnosticOperation.Apply, Catch(() => Thrower.Outer()));
        for (int i = 0; i < DiagnosticLog.Capacity; i++)
        {
            log.Record(DiagnosticOperation.Open, DiagnosticCode.For(SaveLoadOutcome.Failed(LoadFailure.Unrecognized)));
        }

        log.Entries.Should().HaveCount(DiagnosticLog.Capacity).And.OnlyContain(e => e.Operation == DiagnosticOperation.Open, "the oldest is dropped first");
        log.Entries[0].Time.Should().Be(DiagnosticFixtures.Start);
        changes.Should().Be(DiagnosticLog.Capacity + 1);
        console.Messages.Should().HaveCount(DiagnosticLog.Capacity + 1);
        console.Messages[0].Should().Contain("Apply").And.Contain("unexpected").And.Contain(typeof(ArgumentOutOfRangeException).FullName!).And.NotContain(Sentinel);
        console.Exceptions.Should().BeEmpty("the exception, with its message, is never handed to the logger");

        log.Clear();
        log.Entries.Should().BeEmpty();
        changes.Should().Be(DiagnosticLog.Capacity + 2);
    }

    [Theory]
    [InlineData(SessionError.LevelOutOfRange, false)]
    [InlineData(SessionError.NicknameTooLong, false)]
    [InlineData(SessionError.LegalityNotAcknowledged, false)]
    [InlineData(SessionError.StagedEditMismatch, true)]
    [InlineData(SessionError.ExportRevalidationFailed, true)]
    [InlineData(SessionError.ResetFailed, true)]
    public void OnlyFailuresAreRecordedNotRefusalsOfInput(SessionError error, bool recorded)
    {
        var log = DiagnosticFixtures.NewLog();
        log.RecordIfFailure(DiagnosticOperation.Edit, new SessionException(error)).Should().Be(recorded);
        log.Entries.Should().HaveCount(recorded ? 1 : 0);

        log.RecordIfFailure(DiagnosticOperation.Edit, new InvalidOperationException(Sentinel)).Should().BeTrue("anything unexpected is a failure");
    }

    private static readonly RecognizedSave Recognized = new(typeof(SAV5BW), GameVersion.B, 5);

    private static Exception Catch(Action action)
    {
        try
        {
            action();
        }
        catch (Exception e)
        {
            return e;
        }
        throw new InvalidOperationException("Nothing was thrown.");
    }

    /// <summary>Throws an exception carrying the sentinel, wrapped in another that carries it too.</summary>
    private static class Thrower
    {
        public static void Outer()
        {
            try
            {
                Inner(Sentinel);
            }
            catch (ArgumentOutOfRangeException e)
            {
                throw new InvalidOperationException($"Wrapped {Sentinel}", e);
            }
        }

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.NoInlining)]
        public static void Inner(string name) => throw new ArgumentOutOfRangeException(nameof(name), name, $"Bad name {name}");
    }
}

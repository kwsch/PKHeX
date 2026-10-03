using System.Text;
using PKHeX.Web.State;

namespace PKHeX.Web.Services.Diagnostics;

/// <summary>
/// A redacted description of a failure, safe to show in a diagnostic report or write to the browser console.
/// </summary>
/// <remarks>
/// Every part comes from the app's own code, never from the input: a typed outcome's name, the Core save type a refused file was recognised as,
/// exception type names and method names from the stack. An exception's message, data and <see cref="Exception.ToString"/> are never read,
/// because they can carry values from the save (a nickname, an offset, a length). Frames are read from <see cref="Exception.StackTrace"/> as text,
/// not through reflection (<see cref="Exception.TargetSite"/>), which trimming does not preserve.
/// </remarks>
public sealed class DiagnosticCode
{
    /// <summary>Most stack frames kept, starting from the frame that threw.</summary>
    public const int MaxFrames = 12;

    /// <summary>Most exception types kept, from the outermost to its innermost cause.</summary>
    public const int MaxTypes = 4;

    /// <summary>Longest frame kept, in UTF-16 code units; a longer one is cut.</summary>
    private const int MaxFrameLength = 200;

    /// <summary>The code, such as <c>open.parser-fault</c>, <c>session.staged-edit-mismatch</c> or <c>unexpected</c>.</summary>
    public string Name { get; }

    /// <summary>The Core save type a refused file was recognised as, such as <c>SAV5BW</c>, or null.</summary>
    public string? SaveType { get; }

    /// <summary>The exception's type and its inner exceptions' types (full names), outermost first; empty when no exception was involved.</summary>
    public IReadOnlyList<string> ExceptionTypes { get; }

    /// <summary>
    /// Method names from the stack of the innermost exception that has one (<c>Namespace.Type.Method</c>), the throwing frame first, with no
    /// arguments, files or line numbers.
    /// </summary>
    public IReadOnlyList<string> Frames { get; }

    private DiagnosticCode(string name, string? saveType = null, IReadOnlyList<string>? types = null, IReadOnlyList<string>? frames = null)
    {
        Name = name;
        SaveType = saveType;
        ExceptionTypes = types ?? [];
        Frames = frames ?? [];
    }

    /// <summary>The code for a failed open. A parser fault carries the redacted exception the loader caught, when it kept one.</summary>
    /// <exception cref="ArgumentException"><paramref name="outcome"/> succeeded.</exception>
    public static DiagnosticCode For(SaveLoadOutcome outcome)
    {
        if (outcome.Failure is not { } failure)
        {
            throw new ArgumentException("The outcome is not a failure.", nameof(outcome));
        }
        if (outcome.Fault is { } fault)
        {
            return fault;
        }
        var name = failure == LoadFailure.IntegrityFailed && outcome.Integrity is { } problem
            ? $"open.integrity.{Kebab(problem.ToString())}"
            : $"open.{Kebab(failure.ToString())}";
        return new(name, outcome.Recognized?.SaveType.Name);
    }

    /// <summary>The code for a refused session, draft or export operation.</summary>
    public static DiagnosticCode For(SessionError error) => new($"session.{Kebab(error.ToString())}");

    /// <summary>The code for an exception: a <see cref="SessionException"/> by its error, anything else as <c>unexpected</c> with its types and frames.</summary>
    public static DiagnosticCode For(Exception exception) => exception is SessionException refused
        ? For(refused.Error)
        : FromException("unexpected", exception);

    /// <summary>A code named <paramref name="name"/> carrying the redacted <paramref name="exception"/>.</summary>
    internal static DiagnosticCode FromException(string name, Exception exception)
    {
        var types = new List<string>(MaxTypes);
        string? stack = null;
        for (var e = exception; e is not null && types.Count < MaxTypes; e = e.InnerException)
        {
            types.Add(e.GetType().FullName ?? e.GetType().Name);
            // The innermost exception with a stack is nearest the cause; wrappers add little.
            if (!string.IsNullOrEmpty(e.StackTrace))
            {
                stack = e.StackTrace;
            }
        }
        return new(name, null, types, ParseFrames(stack));
    }

    /// <summary>
    /// The method names in a stack trace, keeping for each <c>at …</c> line only the text before its argument list or source location.
    /// Other lines (such as "--- End of stack trace from previous location ---") are skipped.
    /// </summary>
    internal static IReadOnlyList<string> ParseFrames(string? stackTrace)
    {
        if (string.IsNullOrEmpty(stackTrace))
        {
            return [];
        }
        var frames = new List<string>(MaxFrames);
        foreach (var raw in stackTrace.Split('\n'))
        {
            var line = raw.Trim();
            if (!line.StartsWith("at ", StringComparison.Ordinal))
            {
                continue;
            }
            var method = line[3..];
            var end = method.IndexOf('(');
            if (end < 0)
            {
                end = method.IndexOf(" in ", StringComparison.Ordinal);
            }
            if (end >= 0)
            {
                method = method[..end];
            }
            method = StripGenericArguments(method).Trim();
            if (method.Length == 0)
            {
                continue;
            }
            frames.Add(method.Length > MaxFrameLength ? method[..MaxFrameLength] : method);
            if (frames.Count == MaxFrames)
            {
                break;
            }
        }
        return frames;
    }

    /// <summary>
    /// Removes the bracketed, assembly-qualified type arguments of a generic instantiation (<c>`1[[System.Boolean, …]]</c>), keeping the arity,
    /// so they do not use up <see cref="MaxFrameLength"/> before the method name.
    /// </summary>
    private static string StripGenericArguments(string method)
    {
        var result = new StringBuilder(method.Length);
        int depth = 0;
        for (int i = 0; i < method.Length; i++)
        {
            var c = method[i];
            if (depth > 0)
            {
                depth += c == '[' ? 1 : c == ']' ? -1 : 0;
                continue;
            }
            if (c == '[' && i + 1 < method.Length && method[i + 1] == '[')
            {
                depth = 1;
                continue;
            }
            result.Append(c);
        }
        return result.ToString();
    }

    /// <summary>A PascalCase name in lower-case kebab form, such as <c>RoundTripMismatch</c> to <c>round-trip-mismatch</c>.</summary>
    internal static string Kebab(string name)
    {
        var result = new StringBuilder(name.Length + 4);
        for (int i = 0; i < name.Length; i++)
        {
            var c = name[i];
            if (char.IsUpper(c) && i > 0)
            {
                result.Append('-');
            }
            result.Append(char.ToLowerInvariant(c));
        }
        return result.ToString();
    }

    /// <summary>One line for the browser console: the name, the recognised type, the exception types and the frames.</summary>
    public override string ToString()
    {
        var text = new StringBuilder(Name);
        if (SaveType is not null)
        {
            text.Append(" (").Append(SaveType).Append(')');
        }
        if (ExceptionTypes.Count > 0)
        {
            text.Append(" [").AppendJoin(" <- ", ExceptionTypes).Append(']');
        }
        if (Frames.Count > 0)
        {
            text.Append(" at ").AppendJoin(" < ", Frames);
        }
        return text.ToString();
    }
}

using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Numerics;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using PKHeX.Core;

namespace PKHeX.WinForms;

public static class BatchScriptCompiler
{
    public sealed class BatchScriptGlobals
    {
        public required PKM pk { get; init; }
    }

    public static bool TryCompile(string code, [NotNullWhen(true)] out Func<PKM, bool>? modifier, [NotNullWhen(false)] out string? error)
    {
        ArgumentNullException.ThrowIfNull(code);

        modifier = null;
        error = null;
        if (string.IsNullOrWhiteSpace(code))
        {
            error = string.Empty;
            return false;
        }

        var normalized = NormalizeCode(code);
        var options = ScriptOptions.Default
            .WithReferences(typeof(PKM).Assembly, typeof(Enumerable).Assembly, typeof(BigInteger).Assembly)
            .WithImports("System", "System.Linq", "System.Numerics", "PKHeX.Core");

        var script = CSharpScript.Create<bool>(normalized, options, typeof(BatchScriptGlobals));
        var diagnostics = script.Compile();
        var errors = diagnostics.Where(d => d.Severity == DiagnosticSeverity.Error).ToArray();
        if (errors.Length != 0)
        {
            error = string.Join(Environment.NewLine, errors.Select(e => e.ToString()));
            return false;
        }

        var runner = script.CreateDelegate();
        modifier = pk => ExecuteScript(runner, pk);
        return true;
    }

    private static bool ExecuteScript(ScriptRunner<bool> runner, PKM pk)
    {
        var task = runner(new BatchScriptGlobals { pk = pk });
        return task.GetAwaiter().GetResult();
    }

    private static string NormalizeCode(string code)
        => code.EndsWith("return true;", StringComparison.Ordinal)
            ? code
            : code + Environment.NewLine + "return true;";
}

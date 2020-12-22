#if !NET5
#pragma warning disable
// ReSharper disable once UnusedType.Global

namespace System.Runtime.CompilerServices
{
    using Diagnostics;
    using Diagnostics.CodeAnalysis;

    /// <summary>
    ///     Reserved to be used by the compiler for tracking metadata.
    ///     This class should not be used by developers in source code.
    /// </summary>
    [ExcludeFromCodeCoverage, DebuggerNonUserCode]
#endif
    internal static class IsExternalInit
    {
    }
}

#pragma warning restore

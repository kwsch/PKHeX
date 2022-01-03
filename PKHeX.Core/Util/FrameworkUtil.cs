#if !NET6
#pragma warning disable
// ReSharper disable once UnusedType.Global
global using static PKHeX.Core.Buffers.Binary.Extra.BinaryPrimitives;
using System;
using System.Runtime.InteropServices;

namespace PKHeX.Core.Buffers.Binary.Extra
{
    internal static class BinaryPrimitives
    {
        public static float ReadSingleLittleEndian(ReadOnlySpan<byte> data) => MemoryMarshal.Read<float>(data);
        public static void WriteSingleLittleEndian(Span<byte> data, float value) => MemoryMarshal.Write(data, ref value);
        public static double ReadDoubleLittleEndian(ReadOnlySpan<byte> data) => MemoryMarshal.Read<double>(data);
        public static void WriteDoubleLittleEndian(Span<byte> data, double value) => MemoryMarshal.Write(data, ref value);
    }
}

namespace System.Runtime.CompilerServices
{
    using Diagnostics;
    using Diagnostics.CodeAnalysis;

    /// <summary>
    ///     Reserved to be used by the compiler for tracking metadata.
    ///     This class should not be used by developers in source code.
    /// </summary>
    [ExcludeFromCodeCoverage, DebuggerNonUserCode]
    internal static class IsExternalInit
    {
    }
}

namespace System.Diagnostics.CodeAnalysis
{
    [AttributeUsage(AttributeTargets.Parameter)]
    internal sealed class NotNullWhenAttribute : Attribute
    {
        /// <summary>Initializes the attribute with the specified return value condition.</summary>
        /// <param name="returnValue">
        /// The return value condition. If the method returns this value, the associated parameter will not be null.
        /// </param>
        public NotNullWhenAttribute(bool returnValue) => ReturnValue = returnValue;

        /// <summary>Gets the return value condition.</summary>
        public bool ReturnValue { get; }
    }
}
#pragma warning restore
#endif

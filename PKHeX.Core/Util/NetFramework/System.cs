#if !NET6_0_OR_GREATER
namespace System;

public static class FutureFeatures
{
    public static bool StartsWith(this string str, char value) => str.Length != 0 && str[0] == value;

    public static bool Contains<T>(this ReadOnlySpan<T> data, T value) where T : IEquatable<T> => data.IndexOf(value) != -1;
    public static bool Contains<T>(this Span<T> data, T value) where T : IEquatable<T> => data.IndexOf(value) != -1;
}
#endif

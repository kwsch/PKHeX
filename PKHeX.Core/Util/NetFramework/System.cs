#if !NET6
namespace System;

public static class FutureFeatures
{
    public static bool StartsWith(this string str, char value) => str.Length != 0 && str[0] == value;
}
#endif

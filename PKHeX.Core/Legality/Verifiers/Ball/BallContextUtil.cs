using System;
using System.Text;

namespace PKHeX.Core;

#if DEBUG
// ReSharper disable once UnusedType.Global
public static class BallContextUtil
{
    /// <summary>
    /// Creates a new string with the table formatted as is shown in the source code.
    /// </summary>
    public static string Export(ReadOnlySpan<byte> permit)
    {
        var sb = new StringBuilder(8 * permit.Length);
        const int entriesPerLine = 20;
        for (int i = 0; i < permit.Length; i++)
        {
            sb.Append($"0x{permit[i]:X2}, ");
            if (i % entriesPerLine == entriesPerLine - 1)
                sb.AppendLine($"// {i - 19:000}-{i:000}");
        }
        return sb.ToString();
    }
}
#endif

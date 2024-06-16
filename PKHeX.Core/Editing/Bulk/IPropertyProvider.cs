using System.Diagnostics.CodeAnalysis;

namespace PKHeX.Core;

public interface IPropertyProvider
{
    bool TryGetProperty(PKM pk, string prop, [NotNullWhen(true)] out string? result);
}

public sealed class DefaultPropertyProvider : IPropertyProvider
{
    public static readonly DefaultPropertyProvider Instance = new();

    public bool TryGetProperty(PKM pk, string prop, [NotNullWhen(true)] out string? result)
    {
        result = null;
        if (!BatchEditing.TryGetHasProperty(pk, prop, out var pi))
            return false;
        try
        {
            var value = pi.GetValue(pk);
            result = value?.ToString();
            return result != null;
        }
        catch
        {
            return false;
        }
    }
}

using System.Diagnostics.CodeAnalysis;

namespace PKHeX.Core;

/// <summary>
/// Default property provider that uses an <see cref="IBatchEditor{TObject}"/> for reflection.
/// </summary>
public class BatchPropertyProvider<TEditor, TObject>(TEditor editor) : IPropertyProvider<TObject> where TObject : notnull where TEditor : IBatchEditor<TObject>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="BatchPropertyProvider{TEditor, TObject}"/> class with the specified editor.
    /// </summary>
    public bool TryGetProperty(TObject obj, string prop, [NotNullWhen(true)] out string? result)
    {
        result = null;
        if (!editor.TryGetHasProperty(obj, prop, out var pi))
            return false;

        var value = pi.GetValue(obj);
        result = value?.ToString();
        return result is not null;
    }
}

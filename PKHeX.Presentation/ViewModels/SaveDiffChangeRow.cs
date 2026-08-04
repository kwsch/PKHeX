using PKHeX.Application.UseCases;

namespace PKHeX.Presentation.ViewModels;

/// <summary>Display-ready wrapper around a <see cref="SaveDiffChange"/> for the Save Diff viewer grid.</summary>
public sealed class SaveDiffChangeRow(SaveDiffChange change)
{
    public string Category => change.Category.ToString();
    public string Description => change.Description;
    public string OldValue => change.OldValue ?? string.Empty;
    public string NewValue => change.NewValue ?? string.Empty;
}

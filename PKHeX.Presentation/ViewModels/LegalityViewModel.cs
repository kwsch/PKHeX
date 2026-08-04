namespace PKHeX.Presentation.ViewModels;

/// <summary>Holds a legality analysis report for display in the Legality dialog.</summary>
public sealed class LegalityViewModel(string report) : ViewModelBase
{
    public string Report { get; } = report;
}

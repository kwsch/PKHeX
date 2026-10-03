namespace PKHeX.Web.Tests;

/// <summary>Expected text shapes shared by tests, written out rather than taken from the code under test.</summary>
internal static class TestText
{
    /// <summary>A name as the app places it in a sentence or label: inside a first-strong isolate (U+2068 … U+2069).</summary>
    public static string Isolated(string name) => $"\u2068{name}\u2069";
}

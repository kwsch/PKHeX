namespace PKHeX.Web.Tests;

/// <summary>
/// The timeout for bUnit waits on a render that happens on its own (after an awaited step, outside any event handler).
/// </summary>
/// <remarks>
/// bUnit's default of one second is shorter than a loaded CI runner can take to reach even the first check: the check is queued on the
/// renderer's dispatcher and can wait that long for a thread. A wait that passes still returns as soon as it passes.
/// </remarks>
internal static class RenderWait
{
    /// <summary>Passed to <c>WaitForAssertion</c> in place of bUnit's default.</summary>
    public static readonly TimeSpan Timeout = TimeSpan.FromSeconds(10);
}

namespace PKHeX.Core;

/// <summary>
/// Provides a <see cref="IMysteryGiftStorage"/> instance.
/// </summary>
public interface IMysteryGiftStorageProvider
{
    /// <summary>
    /// Provides a <see cref="IMysteryGiftStorage"/> instance.
    /// </summary>
    IMysteryGiftStorage MysteryGiftStorage { get; }
}

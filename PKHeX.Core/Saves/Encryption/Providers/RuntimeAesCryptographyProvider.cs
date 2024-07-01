namespace PKHeX.Core.Saves.Encryption.Providers;

/// <summary>
/// Holds the singleton instance that provides the AES implementation to the app running this library
/// </summary>
public static class RuntimeAesCryptographyProvider
{
    public static IAesCryptographyProvider Aes { get; private set; }

    static RuntimeAesCryptographyProvider()
    {
        Aes = IAesCryptographyProvider.Default;
    }

    public static void Change(IAesCryptographyProvider aesProvider)
    {
        Aes = aesProvider;
    }
}

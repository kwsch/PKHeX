namespace PKHeX.Core.Saves.Encryption.Providers;

/// <summary>
/// Holds the singleton instance that provides the AES implementation to the app running this library
/// </summary>
public static class RuntimeCryptographyProvider
{
    public static IAesCryptographyProvider Aes { get; private set; }
    public static IMd5Provider Md5 { get; private set; }

    static RuntimeCryptographyProvider()
    {
        Aes = IAesCryptographyProvider.Default;
        Md5 = IMd5Provider.Default;
    }

    public static void Change(IAesCryptographyProvider aesProvider)
    {
        Aes = aesProvider;
    }

    public static void Change(IMd5Provider md5Provider)
    {
        Md5 = md5Provider;
    }
}

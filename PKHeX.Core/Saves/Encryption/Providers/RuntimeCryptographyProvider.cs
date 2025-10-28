namespace PKHeX.Core;

/// <summary>
/// Holds the singleton instance that provides the AES implementation to the app running this library
/// </summary>
public static class RuntimeCryptographyProvider
{
    public static IAesCryptographyProvider Aes { get; set; } = IAesCryptographyProvider.Default;
    public static IMd5Provider Md5 { get; set; } = IMd5Provider.Default;
}

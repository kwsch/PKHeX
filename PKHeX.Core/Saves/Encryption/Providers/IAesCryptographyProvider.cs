using System;
using System.Security.Cryptography;

namespace PKHeX.Core.Saves.Encryption.Providers;

/// <summary>
///  Provide an implementation of the Aes algorithm
///
///  The <see cref="IAesCryptographyProvider.Default"/> property will use the .NET implementation that will return an implementation that's specific for each platform
///  except browser (web assembly).
///
///  This interface is intended to allow any runtime that's not supported to provide its own implementation.
///
///  See more at https://learn.microsoft.com/en-us/dotnet/core/compatibility/cryptography/5.0/cryptography-apis-not-supported-on-blazor-webassembly
/// </summary>
public interface IAesCryptographyProvider
{
    IAes Create(byte[] key);

    public static readonly IAesCryptographyProvider Default = new DefaultAes();

    public interface IAes : IDisposable
    {
        void EncryptEcb(ReadOnlySpan<byte> origin, Span<byte> destination);
        void DecryptEcb(ReadOnlySpan<byte> origin, Span<byte> destination);
    }

    private class DefaultAes : IAesCryptographyProvider
    {
        public IAes Create(byte[] key) => new AesSession(key);

        private class AesSession : IAes
        {
            private readonly Aes _aes = Aes.Create();

            public AesSession(byte[] key)
            {
                _aes.Mode = CipherMode.ECB;
                _aes.Padding = PaddingMode.None;
                _aes.Key = key;
            }

            public void Dispose()
            {
                _aes.Dispose();
            }

            public void EncryptEcb(ReadOnlySpan<byte> origin, Span<byte> destination)
            {
                _aes.EncryptEcb(origin, destination, _aes.Padding);
            }

            public void DecryptEcb(ReadOnlySpan<byte> origin, Span<byte> destination)
            {
                _aes.DecryptEcb(origin, destination, _aes.Padding);
            }
        }
    }
}

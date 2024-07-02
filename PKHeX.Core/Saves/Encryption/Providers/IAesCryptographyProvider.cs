using System;
using System.Security.Cryptography;

namespace PKHeX.Core;

/// <summary>
/// Provide an implementation of the Aes algorithm
/// </summary>
/// <remarks>
/// <p>The <see cref="Default"/> property will use the .NET implementation that will return an implementation that is specific for each platform except browser (web assembly).</p>
/// <p>This interface is intended to allow any runtime that's not supported to provide its own implementation.</p>
/// <p>See more at https://learn.microsoft.com/en-us/dotnet/core/compatibility/cryptography/5.0/cryptography-apis-not-supported-on-blazor-webassembly</p>
/// </remarks>
public interface IAesCryptographyProvider
{
    IAes Create(byte[] key, CipherMode mode, PaddingMode padding, byte[]? iv = null);

    internal static readonly IAesCryptographyProvider Default = new DefaultAes();

    public interface IAes : IDisposable
    {
        void EncryptEcb(ReadOnlySpan<byte> plaintext, Span<byte> destination);
        void DecryptEcb(ReadOnlySpan<byte> ciphertext, Span<byte> destination);
        void EncryptCbc(ReadOnlySpan<byte> plaintext, Span<byte> destination);
        void DecryptCbc(ReadOnlySpan<byte> ciphertext, Span<byte> destination);
    }

    private sealed class DefaultAes : IAesCryptographyProvider
    {
        public IAes Create(byte[] key, CipherMode mode, PaddingMode padding, byte[]? iv = null) => new AesSession(key, mode, padding, iv);

        private class AesSession : IAes
        {
            private readonly Aes _aes = Aes.Create();

            public AesSession(byte[] key, CipherMode mode, PaddingMode padding, byte[]? iv)
            {
                _aes.Mode = mode;
                _aes.Padding = padding;
                _aes.Key = key;
                if (iv != null)
                    _aes.IV = iv;
            }

            public void Dispose() => _aes.Dispose();
            public void EncryptEcb(ReadOnlySpan<byte> plaintext, Span<byte> destination) => _aes.EncryptEcb(plaintext, destination, _aes.Padding);
            public void DecryptEcb(ReadOnlySpan<byte> ciphertext, Span<byte> destination) => _aes.DecryptEcb(ciphertext, destination, _aes.Padding);
            public void EncryptCbc(ReadOnlySpan<byte> plaintext, Span<byte> destination) => _aes.EncryptCbc(plaintext, _aes.IV, destination, _aes.Padding);
            public void DecryptCbc(ReadOnlySpan<byte> ciphertext, Span<byte> destination) => _aes.DecryptCbc(ciphertext, _aes.IV, destination, _aes.Padding);
        }
    }
}

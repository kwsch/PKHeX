using System;
using System.Security.Cryptography;

namespace PKHeX.Core.Saves.Encryption.Providers;

public interface IMd5Provider
{
    IMd5Hash Create();

    public static readonly IMd5Provider Default = new DefaultMd5();

    public interface IMd5Hash : IDisposable
    {
        void AppendData(ReadOnlySpan<byte> data);
        void GetCurrentHash(Span<byte> hash);
    }

    private class DefaultMd5 : IMd5Provider
    {
        public IMd5Hash Create() => new Md5HashHash();

        private class Md5HashHash : IMd5Hash
        {
            private readonly IncrementalHash _hasher = IncrementalHash.CreateHash(HashAlgorithmName.MD5);

            public void Dispose()
            {
                _hasher.Dispose();
            }

            public void AppendData(ReadOnlySpan<byte> data) => _hasher.AppendData(data);
            public void GetCurrentHash(Span<byte> hash) => _hasher.GetCurrentHash(hash);
        }
    }
}

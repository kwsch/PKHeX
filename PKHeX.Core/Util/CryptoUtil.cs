using System;
using System.Security.Cryptography;

namespace PKHeX.Core
{
    public partial class Util
    {

        /// <summary>
        /// Creates a new instance of <see cref="SHA1CryptoServiceProvider"/>, or <see cref="SHA1Managed"/> if not supported by the current platform.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown if FIPS mode is enabled on a platform that does not support <see cref="SHA1CryptoServiceProvider"/>.</exception>
        public static SHA1 GetSHA1Provider()
        {
            return SHA1.Create();
        }

        /// <summary>
        /// Creates a new instance of <see cref="SHA256CryptoServiceProvider"/>, or <see cref="SHA256Managed"/> if not supported by the current platform.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown if FIPS mode is enabled on a platform that does not support <see cref="SHA256CryptoServiceProvider"/>.</exception>
        public static SHA256 GetSHA256Provider()
        {
            return SHA256.Create();
        }

        /// <summary>
        /// Creates a new instance of <see cref="AesCryptoServiceProvider"/>, or <see cref="AesManaged"/> if not supported by the current platform.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown if FIPS mode is enabled on a platform that does not support <see cref="AesCryptoServiceProvider"/>.</exception>
        public static Aes GetAesProvider()
        {
            return Aes.Create();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace PKHeX
{
    public partial class Util
    {

        /// <summary>
        /// Creates a new instance of <see cref="SHA1CryptoServiceProvider"/>, or <see cref="SHA1Managed"/> if not supported by the current platform.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown if FIPS mode is enabled on a platform that does not support <see cref="SHA1CryptoServiceProvider"/>.</exception>
        public static SHA1 GetSHA1Provider()
        {
            SHA1 provider;
            try
            {
                provider = new SHA1CryptoServiceProvider();
            }
            catch (PlatformNotSupportedException)
            {
                provider = new SHA1Managed();
            }
            return provider;
        }

        /// <summary>
        /// Creates a new instance of <see cref="SHA256CryptoServiceProvider"/>, or <see cref="SHA256Managed"/> if not supported by the current platform.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown if FIPS mode is enabled on a platform that does not support <see cref="SHA256CryptoServiceProvider"/>.</exception>
        public static SHA256 GetSHA256Provider()
        {
            SHA256 provider;
            try
            {
                provider = new SHA256CryptoServiceProvider();
            }
            catch (PlatformNotSupportedException)
            {
                provider = new SHA256Managed();
            }
            return provider;
        }

        /// <summary>
        /// Creates a new instance of <see cref="AesCryptoServiceProvider"/>, or <see cref="AesManaged"/> if not supported by the current platform.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown if FIPS mode is enabled on a platform that does not support <see cref="AesCryptoServiceProvider"/>.</exception>
        public static Aes GetAesProvider()
        {
            Aes provider;
            try
            {
                provider = new AesCryptoServiceProvider();
            }
            catch (PlatformNotSupportedException)
            {
                provider = new AesManaged();
            }
            return provider;
        }
    }
}

using System;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Security.Cryptography;

namespace PKHeX.Core
{
    public sealed class MemeKey
    {
        /// <summary> Distinguished Encoding Rules </summary>
        private readonly byte[] DER;

        /// <summary> Private Exponent, BigInteger </summary>
        private readonly BigInteger D;

        /// <summary> Public Exponent, BigInteger </summary>
        private readonly BigInteger E;

        /// <summary> Modulus, BigInteger </summary>
        private readonly BigInteger N;

        // Constructor
        public MemeKey(MemeKeyIndex key)
        {
            DER = GetMemeData(key);
            var _N = new byte[0x61];
            var _E = new byte[0x3];
            Array.Copy(DER, 0x18, _N, 0, 0x61);
            Array.Copy(DER, 0x7B, _E, 0, 3);
            Array.Reverse(_N);
            N = new BigInteger(_N);
            Array.Reverse(_E);
            E = new BigInteger(_E);

            if (key == MemeKeyIndex.PokedexAndSaveFile)
            {
                var _D = (byte[])D_3.Clone();
                Array.Reverse(_D);
                D = new BigInteger(_D);
            }
            else
            {
                D = INVALID;
            }
        }

        /// <summary>
        /// Indicates if this key can be used to resign messages.
        /// </summary>
        public bool CanResign => D != INVALID;

        /// <summary>
        /// Get the AES key for this MemeKey
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private byte[] GetAesKey(byte[] data)
        {
            if (data.Length < 0x60)
                throw new ArgumentException("Memebuffers must be atleast 0x60 bytes long!");

            var key = new byte[0x10];
            var buffer = new byte[DER.Length + data.Length - 0x60];
            Array.Copy(DER, 0, buffer, 0, DER.Length);
            Array.Copy(data, 0, buffer, DER.Length, buffer.Length - DER.Length);

            using var sha1 = SHA1.Create();
            Array.Copy(sha1.ComputeHash(buffer), 0, key, 0, 0x10);
            return key;
        }

        /// <summary>
        /// Performs Aes Decryption
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        internal byte[] AesDecrypt(byte[] input)
        {
            var key = GetAesKey(input);
            var data = new byte[0x60];
            Array.Copy(input, input.Length - 0x60, data, 0, 0x60);
            var temp = new byte[0x10];
            var subkey = new byte[0x10];
            var outdata = new byte[data.Length];
            for (var i = 0; i < data.Length / 0x10; i++) // Reverse Phase 2
            {
                var curblock = new byte[0x10];
                Array.Copy(data, ((data.Length / 0x10) - 1 - i) * 0x10, curblock, 0, 0x10);
                temp = AesEcbDecrypt(key, temp.Xor(curblock));
                temp.CopyTo(outdata, ((data.Length / 0x10) - 1 - i) * 0x10);
            }

            // At this point we have Phase1(buf) ^ subkey.
            // Subkey is (block first ^ block last) << 1
            // We don't have block first or block last, though?
            // How can we derive subkey?
            // Well, (a ^ a) = 0. so (block first ^ subkey) ^ (block last ^ subkey)
            // = block first ^ block last ;)
            Array.Copy(outdata, ((data.Length / 0x10) - 1) * 0x10, temp, 0, 0x10);
            temp = temp.Xor(outdata.Take(0x10).ToArray());
            for (var ofs = 0; ofs < 0x10; ofs += 2) // Imperfect ROL implementation
            {
                byte b1 = temp[ofs + 0], b2 = temp[ofs + 1];
                subkey[ofs + 0] = (byte)((2 * b1) + (b2 >> 7));
                subkey[ofs + 1] = (byte)(2 * b2);
                if (ofs + 2 < temp.Length)
                    subkey[ofs + 1] += (byte)(temp[ofs + 2] >> 7);
            }
            if ((temp[0] & 0x80) != 0)
                subkey[0xF] ^= 0x87;

            for (var i = 0; i < data.Length / 0x10; i++)
            {
                var curblock = new byte[0x10];
                Array.Copy(outdata, 0x10 * i, curblock, 0, 0x10);
                Array.Copy(curblock.Xor(subkey), 0, outdata, 0x10 * i, 0x10);
            }

            // Now we have Phase1Encrypt(buf).
            temp = new byte[0x10]; // Clear to all zero
            for (var i = 0; i < data.Length / 0x10; i++) // Phase 1: CBC Encryption.
            {
                var curblock = new byte[0x10];
                Array.Copy(outdata, i * 0x10, curblock, 0, 0x10);
                AesEcbDecrypt(key, curblock).Xor(temp).CopyTo(outdata, i * 0x10);
                curblock.CopyTo(temp, 0);
            }

            var outbuf = (byte[]) input.Clone();
            Array.Copy(outdata, 0, outbuf, outbuf.Length - 0x60, 0x60);

            return outbuf;
        }

        /// <summary>
        /// Perform Aes Encryption
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        internal byte[] AesEncrypt(byte[] input)
        {
            var key = GetAesKey(input);
            var data = new byte[0x60];
            Array.Copy(input, input.Length - 0x60, data, 0, 0x60);
            var temp = new byte[0x10];
            var subkey = new byte[0x10];
            var outdata = new byte[data.Length];
            for (var i = 0; i < data.Length / 0x10; i++) // Phase 1: CBC Encryption.
            {
                var curblock = new byte[0x10];
                Array.Copy(data, i * 0x10, curblock, 0, 0x10);
                temp = AesEcbEncrypt(key, temp.Xor(curblock));
                temp.CopyTo(outdata, i * 0x10);
            }

            // In between - CMAC stuff
            temp = temp.Xor(outdata.Take(0x10).ToArray());
            for (var ofs = 0; ofs < 0x10; ofs += 2) // Imperfect ROL implementation
            {
                byte b1 = temp[ofs + 0], b2 = temp[ofs + 1];
                subkey[ofs + 0] = (byte)((2 * b1) + (b2 >> 7));
                subkey[ofs + 1] = (byte)(2 * b2);
                if (ofs + 2 < temp.Length)
                    subkey[ofs + 1] += (byte)(temp[ofs + 2] >> 7);
            }
            if ((temp[0] & 0x80) != 0)
                subkey[0xF] ^= 0x87;

            temp = new byte[0x10]; // Memcpy from an all-zero buffer
            for (var i = 0; i < data.Length / 0x10; i++)
            {
                var curblock = new byte[0x10];
                Array.Copy(outdata, ((data.Length / 0x10) - 1 - i) * 0x10, curblock, 0, 0x10);
                byte[] temp2 = curblock.Xor(subkey);
                Array.Copy(AesEcbEncrypt(key, temp2).Xor(temp), 0, outdata, ((data.Length / 0x10) - 1 - i) * 0x10, 0x10);
                temp2.CopyTo(temp, 0);
            }

            var outbuf = (byte[])input.Clone();
            Array.Copy(outdata, 0, outbuf, outbuf.Length - 0x60, 0x60);

            return outbuf;
        }

        /// <summary>
        /// Perform Rsa Decryption
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        internal byte[] RsaPrivate(byte[] data)
        {
            var _M = new byte[data.Length + 1];
            data.CopyTo(_M, 1);
            Array.Reverse(_M);
            var M = new BigInteger(_M);

            return Exponentiate(M, D);
        }

        /// <summary>
        /// Perform Rsa Encryption
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        internal byte[] RsaPublic(byte[] data)
        {
            var _M = new byte[data.Length + 1];
            data.CopyTo(_M, 1);
            Array.Reverse(_M);
            var M = new BigInteger(_M);

            return Exponentiate(M, E);
        }

        #region MemeKey Helper Methods
        /// <summary> Indicator value for a bad Exponent </summary>
        private static readonly BigInteger INVALID = BigInteger.MinusOne;

        // Helper method for Modular Exponentiation
        private byte[] Exponentiate(BigInteger M, BigInteger Power)
        {
            var rawSig = BigInteger.ModPow(M, Power, N).ToByteArray();
            Array.Reverse(rawSig);
            var outSig = new byte[0x60];
            if (rawSig.Length < 0x60)
                Array.Copy(rawSig, 0, outSig, 0x60 - rawSig.Length, rawSig.Length);
            else if (rawSig.Length > 0x60)
                Array.Copy(rawSig, rawSig.Length - 0x60, outSig, 0, 0x60);
            else
                Array.Copy(rawSig, outSig, 0x60);
            return outSig;
        }

        private static byte[] GetMemeData(MemeKeyIndex key)
        {
            return key switch
            {
                MemeKeyIndex.LocalWireless => DER_LW,
                MemeKeyIndex.FriendlyCompetition => DER_0,
                MemeKeyIndex.LiveCompetition => DER_1,
                MemeKeyIndex.RentalTeam => DER_2,
                MemeKeyIndex.PokedexAndSaveFile => DER_3,
                MemeKeyIndex.GaOle => DER_4,
                MemeKeyIndex.MagearnaEvent => DER_5,
                MemeKeyIndex.MoncolleGet => DER_6,
                MemeKeyIndex.IslandScanEventSpecial => DER_7,
                MemeKeyIndex.TvTokyoDataBroadcasting => DER_8,
                MemeKeyIndex.CapPikachuEvent => DER_9,
                MemeKeyIndex.Unknown10 => DER_A,
                MemeKeyIndex.Unknown11 => DER_B,
                MemeKeyIndex.Unknown12 => DER_C,
                MemeKeyIndex.Unknown13 => DER_D,
                _ => throw new ArgumentOutOfRangeException(nameof(key), key, null)
            };
        }

        // Helper Method to perform AES ECB Encryption
        private static byte[] AesEcbEncrypt(byte[] key, byte[] data)
        {
            using var ms = new MemoryStream();
            using var aes = Aes.Create();
            aes.Mode = CipherMode.ECB;
            aes.Padding = PaddingMode.None;

            using var cs = new CryptoStream(ms, aes.CreateEncryptor(key, new byte[0x10]), CryptoStreamMode.Write);
            cs.Write(data, 0, data.Length);
            cs.FlushFinalBlock();

            return ms.ToArray();
        }
        // Helper Method to perform AES ECB Decryption
        private static byte[] AesEcbDecrypt(byte[] key, byte[] data)
        {
            using var ms = new MemoryStream();
            using var aes = Aes.Create();
            aes.Mode = CipherMode.ECB;
            aes.Padding = PaddingMode.None;

            using var cs = new CryptoStream(ms, aes.CreateDecryptor(key, new byte[0x10]), CryptoStreamMode.Write);
            cs.Write(data, 0, data.Length);
            cs.FlushFinalBlock();

            return ms.ToArray();
        }

        public static bool IsValidPokeKeyIndex(int index)
        {
            if (!Enum.IsDefined(typeof(MemeKeyIndex), index))
                return false;
            return (MemeKeyIndex)index != MemeKeyIndex.LocalWireless;
        }
        #endregion

        #region Official Keydata
        private static readonly byte[] DER_0 = "307C300D06092A864886F70D0101010500036B003068026100B3D68C9B1090F6B1B88ECFA9E2F60E9C62C3033B5B64282F262CD393B433D97BD3DB7EBA470B1A77A3DB3C18A1E7616972229BDAD54FB02A19546C65FA4773AABE9B8C926707E7B7DDE4C867C01C0802985E438656168A4430F3F3B9662D7D010203010001".ToByteArray();
        private static readonly byte[] DER_1 = "307C300D06092A864886F70D0101010500036B003068026100C10F4097FD3C781A8FDE101EF3B2F091F82BEE4742324B9206C581766EAF2FBB42C7D60D749B999C529B0E22AD05E0C880231219AD473114EC454380A92898D7A8B54D9432584897D6AFE4860235126190A328DD6525D97B9058D98640B0FA050203010001".ToByteArray();
        private static readonly byte[] DER_2 = "307C300D06092A864886F70D0101010500036B003068026100C3C8D89F55D6A236A115C77594D4B318F0A0A0E3252CC0D6345EB9E33A43A5A56DC9D10B7B59C135396159EC4D01DEBC5FB3A4CAE47853E205FE08982DFCC0C39F0557449F97D41FED13B886AEBEEA918F4767E8FBE0494FFF6F6EE3508E3A3F0203010001".ToByteArray();
        private static readonly byte[] DER_3 = "307C300D06092A864886F70D0101010500036B003068026100B61E192091F90A8F76A6EAAA9A3CE58C863F39AE253F037816F5975854E07A9A456601E7C94C29759FE155C064EDDFA111443F81EF1A428CF6CD32F9DAC9D48E94CFB3F690120E8E6B9111ADDAF11E7C96208C37C0143FF2BF3D7E831141A9730203010001".ToByteArray();
        private static readonly byte[] DER_4 = "307C300D06092A864886F70D0101010500036B003068026100A0F2AC80B408E2E4D58916A1C706BEE7A24758A62CE9B50AF1B31409DFCB382E885AA8BB8C0E4AD1BCF6FF64FB3037757D2BEA10E4FE9007C850FFDCF70D2AFAA4C53FAFE38A9917D467862F50FE375927ECFEF433E61BF817A645FA5665D9CF0203010001".ToByteArray();
        private static readonly byte[] DER_5 = "307C300D06092A864886F70D0101010500036B003068026100D046F2872868A5089205B226DE13D86DA552646AC152C84615BE8E0A5897C3EA45871028F451860EA226D53B68DDD5A77D1AD82FAF857EA52CF7933112EEC367A06C0761E580D3D70B6B9C837BAA3F16D1FF7AA20D87A2A5E2BCC6E383BF12D50203010001".ToByteArray();
        private static readonly byte[] DER_6 = "307C300D06092A864886F70D0101010500036B003068026100D379919001D7FF40AC59DF475CF6C6368B1958DD4E870DFD1CE11218D5EA9D88DD7AD530E2806B0B092C02E25DB092518908EDA574A0968D49B0503954B24284FA75445A074CE6E1ABCEC8FD01DAA0D21A0DD97B417BC3E54BEB7253FC06D3F30203010001".ToByteArray();
        private static readonly byte[] DER_7 = "307C300D06092A864886F70D0101010500036B003068026100B751CB7D282625F2961A7138650ABE1A6AA80D69548BA3AE9DFF065B2805EB3675D960C62096C2835B1DF1C290FC19411944AFDF3458E3B1BC81A98C3F3E95D0EE0C20A0259E614399404354D90F0C69111A4E525F425FBB31A38B8C558F23730203010001".ToByteArray();
        private static readonly byte[] DER_8 = "307C300D06092A864886F70D0101010500036B003068026100B328FE4CC41627882B04FBA0A396A15285A8564B6112C1203048766D827E8E4E5655D44B266B2836575AE68C8301632A3E58B1F4362131E97B0AA0AFC38F2F7690CBD4F3F4652072BFD8E9421D2BEEF177873CD7D08B6C0D1022109CA3ED5B630203010001".ToByteArray();
        private static readonly byte[] DER_9 = "307C300D06092A864886F70D0101010500036B003068026100C4B32FD1161CC30D04BD569F409E878AA2815C91DD009A5AE8BFDAEA7D116BF24966BF10FCC0014B258DFEF6614E55FB6DAB2357CD6DF5B63A5F059F724469C0178D83F88F45048982EAE7A7CC249F84667FC393684DA5EFE1856EB10027D1D70203010001".ToByteArray();
        private static readonly byte[] DER_A = "307C300D06092A864886F70D0101010500036B003068026100C5B75401E83352A64EEC8916C4206F17EC338A24A6F7FD515260696D7228496ABC1423E1FF30514149FC199720E95E682539892E510B239A8C7A413DE4EEE74594F073815E9B434711F6807E8B9E7C10C281F89CF3B1C14E3F0ADF83A2805F090203010001".ToByteArray();
        private static readonly byte[] DER_B = "307C300D06092A864886F70D0101010500036B003068026100AC36B88D00C399C660B4846287FFC7F9DF5C07487EAAE3CD4EFD0029D3B86ED3658AD7DEE4C7F5DA25F9F6008885F343122274994CAB647776F0ADCFBA1E0ECEC8BF57CAAB8488BDD59A55195A0167C7D2C4A9CF679D0EFF4A62B5C8568E09770203010001".ToByteArray();
        private static readonly byte[] DER_C = "307C300D06092A864886F70D0101010500036B003068026100CAC0514D4B6A3F70771C461B01BDE3B6D47A0ADA078074DDA50703D8CC28089379DA64FB3A34AD3435D24F7331383BDADC4877662EFB555DA2077619B70AB0342EBE6EE888EBF3CF4B7E8BCCA95C61E993BDD6104C10D11115DC84178A5894350203010001".ToByteArray();
        private static readonly byte[] DER_D = "307C300D06092A864886F70D0101010500036B003068026100B906466740F5A9428DA84B418C7FA6146F7E24C783373D671F9214B40948A4A317C1A4460111B45D2DADD093815401573E52F0178890D35CBD95712EFAAE0D20AD47187648775CD9569431B1FC3C784113E3A48436D30B2CD162218D6781F5ED0203010001".ToByteArray();

        private static readonly byte[] DER_LW = "307C300D06092A864886F70D0101010500036B003068026100B756E1DCD8CECE78E148107B1BAC115FDB17DE843453CAB7D4E6DF8DD21F5A3D17B4477A8A531D97D57EB558F0D58A4AF5BFADDDA4A0BC1DC22FF87576C7268B942819D4C83F78E1EE92D406662F4E68471E4DE833E5126C32EB63A868345D1D0203010001".ToByteArray();

        private static readonly byte[] D_3 = "00775455668FFF3CBA3026C2D0B26B8085895958341157AEB03B6B0495EE57803E2186EB6CB2EB62A71DF18A3C9C6579077670961B3A6102DABE5A194AB58C3250AED597FC78978A326DB1D7B28DCCCB2A3E014EDBD397AD33B8F28CD525054251".ToByteArray();
        #endregion
    }

    public static class StringExtentions
    {
        public static byte[] ToByteArray(this string toTransform)
        {
            return Enumerable
                .Range(0, toTransform.Length / 2)
                .Select(i => Convert.ToByte(toTransform.Substring(i * 2, 2), 16))
                .ToArray();
        }
    }

    public static class ByteArrayExtensions
    {
        public static byte[] Xor(this byte[] b1, byte[] b2)
        {
            if (b1.Length != b2.Length)
                throw new ArgumentException("Cannot xor two arrays of uneven length!");
            var x = new byte[b1.Length];
            for (var i = 0; i < b1.Length; i++)
                x[i] = (byte)(b1[i] ^ b2[i]);
            return x;
        }
    }
}
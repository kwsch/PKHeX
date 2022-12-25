using System;
using System.Diagnostics;
using System.Numerics;
using System.Security.Cryptography;
using static PKHeX.Core.MemeKeyIndex;

namespace PKHeX.Core;

/// <summary>
/// Key for crypto with <see cref="MemeCrypto"/> binaries.
/// </summary>
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
        N = new BigInteger(DER.AsSpan(0x18, 0x61), isUnsigned: true, isBigEndian: true);
        E = new BigInteger(DER.AsSpan(0x7B, 0x03), isUnsigned: true, isBigEndian: true);
        D = key == PokedexAndSaveFile ? new BigInteger(D_3, isUnsigned: true, isBigEndian: true) : default;
    }

    /// <summary>
    /// Indicates if this key can be used to resign messages.
    /// </summary>
    public bool CanResign => D != default;

    public const int SignatureLength = 0x60;
    private const int chunk = 0x10;

    /// <summary>
    /// Performs Aes Decryption
    /// </summary>
    internal void AesDecrypt(Span<byte> data)
    {
        var payload = data[..^SignatureLength];
        var sig = data[^SignatureLength..];
        AesDecrypt(payload, sig);
    }

    /// <summary>
    /// Perform Aes Encryption
    /// </summary>
    internal void AesEncrypt(Span<byte> data)
    {
        var payload = data[..^SignatureLength];
        var sig = data[^SignatureLength..];
        AesEncrypt(payload, sig);
    }
    
    private void AesDecrypt(ReadOnlySpan<byte> data, Span<byte> sig)
    {
        using var aes = GetAesImpl(data);
        Span<byte> temp = stackalloc byte[chunk];
        Span<byte> nextXor = stackalloc byte[chunk];

        for (var i = sig.Length - chunk; i >= 0; i -= chunk)
        {
            var slice = sig.Slice(i, chunk);
            Xor(temp, slice);
            aes.DecryptEcb(temp, temp, PaddingMode.None);
            temp.CopyTo(slice);
        }

        Xor(temp, sig[^chunk..]);
        GetSubKey(temp, nextXor);
        for (var i = 0; i < sig.Length; i += chunk)
            Xor(sig.Slice(i, chunk), nextXor);

        nextXor.Clear();
        for (var i = 0; i < sig.Length; i += chunk)
        {
            var slice = sig.Slice(i, chunk);
            slice.CopyTo(temp);
            aes.DecryptEcb(slice, slice, PaddingMode.None);
            Xor(slice, nextXor);
            temp.CopyTo(nextXor);
        }
    }

    private void AesEncrypt(Span<byte> data, Span<byte> sig)
    {
        using var aes = GetAesImpl(data);
        Span<byte> temp = stackalloc byte[chunk];
        Span<byte> nextXor = stackalloc byte[chunk];

        for (var i = 0; i < sig.Length; i += chunk)
        {
            var slice = sig.Slice(i, chunk);
            Xor(slice, temp);
            aes.EncryptEcb(slice, slice, PaddingMode.None);
            slice.CopyTo(temp);
        }

        Xor(temp, sig[..chunk]);
        GetSubKey(temp, nextXor);
        for (var i = 0; i < sig.Length; i += chunk)
            Xor(sig.Slice(i, chunk), nextXor);

        temp.Clear();
        for (var i = sig.Length - chunk; i >= 0; i -= chunk)
        {
            var slice = sig.Slice(i, chunk);
            slice.CopyTo(nextXor);
            aes.EncryptEcb(slice, slice, PaddingMode.None);
            Xor(slice, temp);
            nextXor.CopyTo(temp);
        }
    }

    private Aes GetAesImpl(ReadOnlySpan<byte> payload)
    {
        // The C# implementation of AES isn't fully allocation-free, so some allocation on key & implementation is needed.
        var key = GetAesKey(payload);

        // Don't dispose in this method, let the consumer dispose.
        var aes = Aes.Create();
        aes.Mode = CipherMode.ECB;
        aes.Padding = PaddingMode.None;
        aes.Key = key;
        // no IV -- all zero.
        return aes;
    }

    /// <summary>
    /// Get the AES key for this MemeKey
    /// </summary>
    private byte[] GetAesKey(ReadOnlySpan<byte> data)
    {
        // HashLengthInBytes is 20.
        Span<byte> hash = stackalloc byte[20];
        using var h = IncrementalHash.CreateHash(HashAlgorithmName.SHA1);
        h.AppendData(DER);
        h.AppendData(data);
        h.TryGetCurrentHash(hash, out _);
        return hash[..chunk].ToArray(); // need a byte[0x10] (not 0x14) for the AES impl
    }

    private static void GetSubKey(ReadOnlySpan<byte> temp, Span<byte> subkey)
    {
        for (var i = 0; i < temp.Length; i += 2) // Imperfect ROL implementation
        {
            byte b1 = temp[i + 0], b2 = temp[i + 1];
            subkey[i + 0] = (byte)((2 * b1) + (b2 >> 7));
            subkey[i + 1] = (byte)(2 * b2);
            if (i + 2 < temp.Length)
                subkey[i + 1] += (byte)(temp[i + 2] >> 7);
        }
        if ((temp[0] & 0x80) != 0)
            subkey[0xF] ^= 0x87;
    }

    private static void Xor(Span<byte> b1, Span<byte> b2)
    {
        Debug.Assert(b1.Length <= b2.Length);
        for (var i = 0; i < b2.Length; i++)
            b1[i] ^= b2[i];
    }

    /// <summary>
    /// Perform Rsa Decryption
    /// </summary>
    internal void RsaPrivate(ReadOnlySpan<byte> data, Span<byte> outSig)
    {
        var M = new BigInteger(data, isUnsigned: true, isBigEndian: true);
        Exponentiate(M, D, outSig);
    }

    /// <summary>
    /// Perform Rsa Encryption
    /// </summary>
    internal void RsaPublic(ReadOnlySpan<byte> data, Span<byte> outSig)
    {
        var M = new BigInteger(data, isUnsigned: true, isBigEndian: true);
        Exponentiate(M, E, outSig);
    }

    #region MemeKey Helper Methods

    // Helper method for Modular Exponentiation
    private void Exponentiate(BigInteger M, BigInteger Power, Span<byte> result)
    {
        var raw = BigInteger.ModPow(M, Power, N);
        raw.TryWriteBytes(result, out _, isUnsigned: true, isBigEndian: true);
    }

    private static byte[] GetMemeData(MemeKeyIndex key) => key switch
    {
        LocalWireless           => DER_LW,
        FriendlyCompetition     => DER_0,
        LiveCompetition         => DER_1,
        RentalTeam              => DER_2,
        PokedexAndSaveFile      => DER_3,
        GaOle                   => DER_4,
        MagearnaEvent           => DER_5,
        MoncolleGet             => DER_6,
        IslandScanEventSpecial  => DER_7,
        TvTokyoDataBroadcasting => DER_8,
        CapPikachuEvent         => DER_9,
        Unknown10               => DER_A,
        Unknown11               => DER_B,
        Unknown12               => DER_C,
        Unknown13               => DER_D,
        _ => throw new ArgumentOutOfRangeException(nameof(key), key, null),
    };

    public static bool IsValidPokeKeyIndex(int index)
    {
        if (!Enum.IsDefined(typeof(MemeKeyIndex), index))
            return false;
        return (MemeKeyIndex)index != LocalWireless;
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

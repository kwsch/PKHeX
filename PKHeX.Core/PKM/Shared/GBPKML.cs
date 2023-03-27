using System;

namespace PKHeX.Core;

/// <summary>
/// Mainline format for Generation 1 &amp; 2 <see cref="PKM"/> objects.
/// </summary>
/// <remarks>This format stores <see cref="PKM.Nickname"/> and <see cref="PKM.OT_Name"/> in buffers separate from the rest of the details.</remarks>
public abstract class GBPKML : GBPKM
{
    internal const int StringLengthJapanese = 6;
    internal const int StringLengthNotJapan = 11;
    public sealed override int MaxStringLengthOT => Japanese ? 5 : 7;
    public sealed override int MaxStringLengthNickname => Japanese ? 5 : 10;
    public sealed override bool Japanese => RawOT.Length == StringLengthJapanese;

    internal readonly byte[] RawOT;
    internal readonly byte[] RawNickname;

    // Trash Bytes
    public sealed override Span<byte> Nickname_Trash => RawNickname;
    public sealed override Span<byte> OT_Trash => RawOT;

    protected GBPKML(int size, bool jp = false) : base(size)
    {
        int strLen = jp ? StringLengthJapanese : StringLengthNotJapan;

        // initialize string buffers
        RawOT = new byte[strLen];
        RawNickname = new byte[strLen];
        RawOT.AsSpan().Fill(StringConverter12.G1TerminatorCode);
        RawNickname.AsSpan().Fill(StringConverter12.G1TerminatorCode);
    }

    protected GBPKML(byte[] data, bool jp = false) : base(data)
    {
        int strLen = jp ? StringLengthJapanese : StringLengthNotJapan;

        // initialize string buffers
        RawOT = new byte[strLen];
        RawNickname = new byte[strLen];
        RawOT.AsSpan().Fill(StringConverter12.G1TerminatorCode);
        RawNickname.AsSpan().Fill(StringConverter12.G1TerminatorCode);
    }

    public override void SetNotNicknamed(int language) => GetNonNickname(language, RawNickname);

    protected override void GetNonNickname(int language, Span<byte> data)
    {
        var name = SpeciesName.GetSpeciesNameGeneration(Species, language, Format);
        SetString(name, data, data.Length, StringConverterOption.Clear50);
        if (Korean)
            return;

        // Decimal point<->period fix
        foreach (ref var c in data)
        {
            if (c == 0xF2)
                c = 0xE8;
        }
    }

    private string GetString(ReadOnlySpan<byte> span)
    {
        if (Korean)
            return StringConverter2KOR.GetString(span);
        return StringConverter12.GetString(span, Japanese);
    }

    private int SetString(ReadOnlySpan<char> value, Span<byte> destBuffer, int maxLength, StringConverterOption option = StringConverterOption.None)
    {
        if (Korean)
            return StringConverter2KOR.SetString(destBuffer, value, maxLength, option);
        return StringConverter12.SetString(destBuffer, value, maxLength, Japanese, option);
    }

    public sealed override string Nickname
    {
        get => GetString(RawNickname);
        set
        {
            if (!IsNicknamed && Nickname == value)
                return;

            SetStringKeepTerminatorStyle(value, RawNickname);
        }
    }

    public sealed override string OT_Name
    {
        get => GetString(RawOT);
        set
        {
            if (value == OT_Name)
                return;
            SetStringKeepTerminatorStyle(value, RawOT);
        }
    }

    private void SetStringKeepTerminatorStyle(ReadOnlySpan<char> value, Span<byte> exist)
    {
        // Reset the destination buffer based on the termination style of the existing string.
        bool zeroed = exist.IndexOf((byte)0) != -1;
        StringConverterOption converterOption = (zeroed) ? StringConverterOption.ClearZero : StringConverterOption.Clear50;
        SetString(value, exist, value.Length, converterOption);
    }
}

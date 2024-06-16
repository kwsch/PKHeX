using System;
using System.Diagnostics.CodeAnalysis;

namespace PKHeX.Core;

/// <summary>
/// Mainline format for Generation 1 &amp; 2 <see cref="PKM"/> objects.
/// </summary>
/// <remarks>This format stores <see cref="PKM.Nickname"/> and <see cref="PKM.OriginalTrainerName"/> in buffers separate from the rest of the details.</remarks>
public abstract class GBPKML : GBPKM
{
    internal const int StringLengthJapanese = 6;
    internal const int StringLengthNotJapan = 11;
    public sealed override int MaxStringLengthTrainer => Japanese ? 5 : 7;
    public sealed override int MaxStringLengthNickname => Japanese ? 5 : 10;
    public sealed override bool Japanese => RawOT.Length == StringLengthJapanese;

    private readonly byte[] RawOT;
    private readonly byte[] RawNickname;

    // Trash Bytes
    public sealed override Span<byte> NicknameTrash => RawNickname;
    public sealed override Span<byte> OriginalTrainerTrash => RawOT;
    public override int TrashCharCountTrainer => RawOT.Length;
    public override int TrashCharCountNickname => RawNickname.Length;

    protected GBPKML([ConstantExpected] int size, bool jp = false) : base(size)
    {
        int strLen = jp ? StringLengthJapanese : StringLengthNotJapan;

        // initialize string buffers
        RawOT = new byte[strLen];
        RawNickname = new byte[strLen];
        OriginalTrainerTrash.Fill(StringConverter1.TerminatorCode);
        NicknameTrash.Fill(StringConverter1.TerminatorCode);
    }

    protected GBPKML(byte[] data, bool jp = false) : base(data)
    {
        int strLen = jp ? StringLengthJapanese : StringLengthNotJapan;

        // initialize string buffers
        RawOT = new byte[strLen];
        RawNickname = new byte[strLen];
        OriginalTrainerTrash.Fill(StringConverter1.TerminatorCode);
        NicknameTrash.Fill(StringConverter1.TerminatorCode);
    }

    public override void SetNotNicknamed(int language) => GetNonNickname(language, RawNickname);

    protected override void GetNonNickname(int language, Span<byte> data)
    {
        var name = SpeciesName.GetSpeciesNameGeneration(Species, language, Format);
        SetString(data, name, data.Length, StringConverterOption.Clear50);
        if (Korean)
            return;

        // Decimal point<->period fix
        data.Replace<byte>(0xF2, 0xE8);
    }

    public sealed override string Nickname
    {
        get => GetString(NicknameTrash);
        set
        {
            if (!IsNicknamed && Nickname == value)
                return;

            SetStringKeepTerminatorStyle(value, NicknameTrash);
        }
    }

    public sealed override string OriginalTrainerName
    {
        get => GetString(OriginalTrainerTrash);
        set
        {
            if (value == OriginalTrainerName)
                return;
            SetStringKeepTerminatorStyle(value, OriginalTrainerTrash);
        }
    }

    private void SetStringKeepTerminatorStyle(ReadOnlySpan<char> value, Span<byte> exist)
    {
        // Reset the destination buffer based on the termination style of the existing string.
        bool zeroed = exist.Contains<byte>(0);
        StringConverterOption converterOption = (zeroed) ? StringConverterOption.ClearZero : StringConverterOption.Clear50;
        SetString(exist, value, value.Length, converterOption);
    }
}

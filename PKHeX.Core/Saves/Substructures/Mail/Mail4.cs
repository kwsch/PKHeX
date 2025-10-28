using System;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

public sealed class Mail4 : MailDetail
{
    public const int SIZE = 0x38;

    public Mail4(Memory<byte> raw, int ofs) : base(raw, ofs) { }

    public Mail4(Memory<byte> raw) : base(raw, -1) { }

    public Mail4(byte? lang, byte? version) : base(new byte[SIZE])
    {
        if (lang is not null) AuthorLanguage = (byte)lang;
        if (version is not null) AuthorVersion = (byte)version;
        ResetData();
    }

    public override void CopyTo(SaveFile sav) => sav.SetData(((SAV4)sav).General[DataOffset..], Data);

    private void ResetData()
    {
        AuthorTID = 0;
        AuthorSID = 0;
        AuthorGender = 0;
        MailType = 0xFF;
        AuthorName = string.Empty;
        for (int i = 0; i < 3; i++)
            SetAppearSpecies(i, ushort.MaxValue);

        for (int y = 0; y < 3; y++)
        {
            for (int x = 0; x < 4; x++)
                SetMessage(y, x, x == 1 ? (ushort)0 : ushort.MaxValue);
        }
    }

    public override void CopyTo(PK4 pk4) => Data.CopyTo(pk4.HeldMail);
    public override ushort AuthorTID { get => ReadUInt16LittleEndian(Data); set => WriteUInt16LittleEndian(Data, value); }
    public override ushort AuthorSID { get => ReadUInt16LittleEndian(Data[2..]); set => WriteUInt16LittleEndian(Data[2..], value); }
    public override byte AuthorGender { get => Data[4]; set => Data[4] = value; }
    public override byte AuthorLanguage { get => Data[5]; set => Data[5] = value; }
    public override byte AuthorVersion { get => Data[6]; set => Data[6] = value; }
    public override int MailType { get => Data[7]; set => Data[7] = (byte)value; }
    public override string AuthorName { get => StringConverter4.GetString(Data.Slice(8, 0x10)); set => StringConverter4.SetString(Data.Slice(8, 0x10), value, 7, AuthorLanguage, StringConverterOption.ClearFF); }
    public ushort GetAppearSpecies(int index) => ReadUInt16LittleEndian(Data[(0x1C - (index * 2))..]);
    public void SetAppearSpecies(int index, ushort value) => WriteUInt16LittleEndian(Data[(0x1C - (index * 2))..], (ushort)(value == 0 ? 0xFFFF : value));
    public override ushort GetMessage(int index1, int index2) => ReadUInt16LittleEndian(Data[(0x20 + (((index1 * 4) + index2) * 2))..]);
    public override void SetMessage(int index1, int index2, ushort value) => WriteUInt16LittleEndian(Data[(0x20 + (((index1 * 4) + index2) * 2))..], value);

    public override bool? IsEmpty => MailType switch
    {
        0xFF => true,
        <= 11 => false,
        _ => null,
    };

    public override void SetBlank() => SetBlank(0, 0);

    public void SetBlank(byte lang, byte version)
    {
        Data[..SIZE].Clear();
        AuthorLanguage = lang;
        AuthorVersion = version;
    }
}

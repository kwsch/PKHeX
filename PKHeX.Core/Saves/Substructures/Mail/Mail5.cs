using System;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

public sealed class Mail5 : MailDetail
{
    public const int SIZE = 0x38;

    public Mail5(byte[] data, int ofs = -1) : base(data, ofs) { }

    public Mail5(byte? lang, byte? version) : base(new byte[SIZE])
    {
        if (lang != null) AuthorLanguage = (byte)lang;
        if (version != null) AuthorVersion = (byte)version;
        ResetData();
    }

    private void ResetData()
    {
        AuthorTID = 0;
        AuthorSID = 0;
        AuthorGender = 0;
        MailType = 0xFF;
        AuthorName = string.Empty;
        for (int i = 0; i < 3; i++)
            SetMisc(i, 0);
        MessageEnding = 0xFFFF;
        for (int y = 0; y < 3; y++)
        {
            for (int x = 0; x < 4; x++)
                SetMessage(y, x, x == 1 ? (ushort)0 : (ushort)0xFFFF);
        }
    }

    public override void CopyTo(PK5 pk5) => Data.CopyTo(pk5.HeldMail);
    public override ushort AuthorTID { get => ReadUInt16LittleEndian(Data.AsSpan(0)); set => WriteUInt16LittleEndian(Data.AsSpan(0), value); }
    public override ushort AuthorSID { get => ReadUInt16LittleEndian(Data.AsSpan(2)); set => WriteUInt16LittleEndian(Data.AsSpan(2), value); }
    public override byte AuthorGender { get => Data[4]; set => Data[4] = value; }
    public override byte AuthorLanguage { get => Data[5]; set => Data[5] = value; }
    public override byte AuthorVersion { get => Data[6]; set => Data[6] = value; }
    public override int MailType { get => Data[7]; set => Data[7] = (byte)value; }
    public override string AuthorName { get => StringConverter5.GetString(Data.AsSpan(8, 0x10)); set => StringConverter5.SetString(Data.AsSpan(8, 0x10), value, 7, AuthorLanguage, StringConverterOption.ClearZero); }
    public int GetMisc(int index) => ReadUInt16LittleEndian(Data.AsSpan(0x1C - (index * 2)));
    public void SetMisc(int index, int value) => WriteUInt16LittleEndian(Data.AsSpan(0x1C - (index * 2)), (ushort)value);
    public ushort MessageEnding { get => ReadUInt16LittleEndian(Data.AsSpan(0x1E)); set => WriteUInt16LittleEndian(Data.AsSpan(0x1E), value); }
    public override ushort GetMessage(int index1, int index2) => ReadUInt16LittleEndian(Data.AsSpan(0x20 + (((index1 * 4) + index2) * 2)));
    public override void SetMessage(int index1, int index2, ushort value) => WriteUInt16LittleEndian(Data.AsSpan(0x20 + (((index1 * 4) + index2) * 2)), value);

    public override bool? IsEmpty => MailType switch
    {
        0xFF => true,
        <= 11 => false,
        _ => null,
    };

    public override void SetBlank() => SetBlank(null, null);
    public void SetBlank(byte? lang, byte? version) => new Mail5(lang: lang, version: version).Data.CopyTo(Data, 0);
}

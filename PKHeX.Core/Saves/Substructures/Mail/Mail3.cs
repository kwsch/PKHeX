using System;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

public sealed class Mail3 : MailDetail
{
    public const int SIZE = 0x24;
    private bool JP;

    public Mail3() : base(new byte[SIZE], -1) => ResetData();
    public Mail3(Memory<byte> data, int ofs) : base(data, ofs) => JP = Data[0x12 + 5] == 0xFF; // author name length < 6

    private void ResetData()
    {
        for (int y = 0; y < 3; y++)
        {
            for (int x = 0; x < 3; x++)
                SetMessage(y, x, 0xFFFF);
        }

        AuthorName = string.Empty;
        AuthorTID = 0;
        AuthorTID = 0;
        AppearPKM = 1;
        MailType = 0;
    }

    public override ushort GetMessage(int index1, int index2) => ReadUInt16LittleEndian(Data.Slice(((index1 * 3) + index2) * 2));
    public override void SetMessage(int index1, int index2, ushort value) => WriteUInt16LittleEndian(Data.Slice(((index1 * 3) + index2) * 2), value);
    public override void CopyTo(SaveFile sav) => sav.SetData(((SAV3)sav).Large[_dataOffset..], Data);

    public override string AuthorName
    {
        get => StringConverter3.GetString(Data.Slice(0x12, 7), JP);
        set
        {
            var span = Data.Slice(0x12, 8);
            if (value.Length == 0)
            {
                span.Fill(0xFF);
                return;
            }
            int len = JP ? 5 : 7;
            StringConverter3.SetString(span[..(len + 1)], value, len, JP, StringConverterOption.ClearFF);
            if (!JP)
                span[..(len - 1)].Replace<byte>(0xFF, 0x00); // Pad with spaces to at least 6 characters
            else
                span[^2] = 0x00; // Last two bytes of OT names should be zeroes
            span[^1] = 0xFF; // Ensure terminator
        }
    }

    public override byte AuthorLanguage
    {
        get => JP ? (byte)LanguageID.Japanese : (byte)LanguageID.English;
        set => JP = value == (byte)LanguageID.Japanese;
    }

    public override ushort AuthorTID { get => ReadUInt16LittleEndian(Data.Slice(0x1A)); set => WriteUInt16LittleEndian(Data.Slice(0x1A), value); }
    public override ushort AuthorSID { get => ReadUInt16LittleEndian(Data.Slice(0x1C)); set => WriteUInt16LittleEndian(Data.Slice(0x1C), value); }
    public override ushort AppearPKM { get => ReadUInt16LittleEndian(Data.Slice(0x1E)); set => WriteUInt16LittleEndian(Data.Slice(0x1E), (ushort)(value == 0 ? 1 : value)); }
    public override int MailType { get => ReadUInt16LittleEndian(Data.Slice(0x20)); set => WriteUInt16LittleEndian(Data.Slice(0x20), (ushort)value); }

    public override bool? IsEmpty => MailType switch
    {
        0 => true,
        >= 0x79 and <= 0x84 => false,
        _ => null,
    };

    public override void SetBlank() => new Mail3().Data.CopyTo(Data);
}

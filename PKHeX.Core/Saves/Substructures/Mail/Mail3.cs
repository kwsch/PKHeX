using System;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

public sealed class Mail3 : MailDetail
{
    public const int SIZE = 0x24;
    private readonly bool JP;

    public Mail3() : base(new byte[SIZE], -1) => ResetData();
    public Mail3(byte[] data, int ofs, bool japanese) : base(data, ofs) => JP = japanese;

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

    public override ushort GetMessage(int index1, int index2) => ReadUInt16LittleEndian(Data.AsSpan(((index1 * 3) + index2) * 2));
    public override void SetMessage(int index1, int index2, ushort value) => WriteUInt16LittleEndian(Data.AsSpan(((index1 * 3) + index2) * 2), value);
    public override void CopyTo(SaveFile sav) => sav.SetData(((SAV3)sav).Large, DataOffset);

    public override string AuthorName
    {
        get => StringConverter3.GetString(Data.AsSpan(0x12, 7), JP);
        set
        {
            var span = Data.AsSpan(0x12, 8);
            StringConverter3.SetString(span, value, 7, JP, StringConverterOption.ClearFF);
        }
    }

    public override ushort AuthorTID { get => ReadUInt16LittleEndian(Data.AsSpan(0x1A)); set => WriteUInt16LittleEndian(Data.AsSpan(0x1A), value); }
    public override ushort AuthorSID { get => ReadUInt16LittleEndian(Data.AsSpan(0x1C)); set => WriteUInt16LittleEndian(Data.AsSpan(0x1C), value); }
    public override ushort AppearPKM { get => ReadUInt16LittleEndian(Data.AsSpan(0x1E)); set => WriteUInt16LittleEndian(Data.AsSpan(0x1E), (ushort)(value == 0 ? 1 : value)); }
    public override int MailType { get => ReadUInt16LittleEndian(Data.AsSpan(0x20)); set => WriteUInt16LittleEndian(Data.AsSpan(0x20), (ushort)value); }

    public override bool? IsEmpty => MailType switch
    {
        0 => true,
        >= 0x79 and <= 0x84 => false,
        _ => null,
    };

    public override void SetBlank() => (new Mail3()).Data.CopyTo(Data, 0);
}

using System;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

// warning: international only
public sealed class Mail2 : MailDetail
{
    private readonly int Language;
    private bool Japanese => Language == 1;
    private bool Korean => Language == (int)LanguageID.Korean;

    // structure:
    private const int LINE_LENGTH = 0x10;
    private const int MESSAGE_LENGTH = LINE_LENGTH + LINE_LENGTH + 1; // each line has a single char at end
    private const int OFS_AUTHOR = MESSAGE_LENGTH;
    private const int OFS_AUTHOR_NATION = OFS_AUTHOR + AUTHOR_LENGTH + 1;
    private const int OFS_AUTHOR_ID = OFS_AUTHOR_NATION + 2;
    private const int OFS_APPEAR = OFS_AUTHOR_ID + 2;
    private const int OFS_TYPE = OFS_APPEAR + 1;
    private const int SIZE = OFS_TYPE + 1;

    private const int COUNT_PARTY = 6;
    private const int COUNT_MAILBOX = 10;

    private const int AUTHOR_LENGTH = 7;

    public Mail2(SAV2 sav, int index) : base(sav.Data.AsSpan(GetMailOffset(index), 0x2F).ToArray(), GetMailOffset(index))
    {
        Language = sav.Language;
    }

    private static int GetMailOffset(int index)
    {
        if (index < COUNT_PARTY)
            return GetPartyMailOffset(index);
        return GetMailboxMailOffset(index - COUNT_PARTY);
    }

    private static int GetPartyMailOffset(int index)
    {
        if ((uint)index >= COUNT_PARTY)
            throw new ArgumentOutOfRangeException(nameof(index));
        return (index * SIZE) + 0x600;
    }

    private static int GetMailboxMailOffset(int index)
    {
        if ((uint)index >= COUNT_MAILBOX)
            throw new ArgumentOutOfRangeException(nameof(index));
        return (index * SIZE) + 0x835;
    }

    private string GetString(Span<byte> span)
    {
        if (Korean)
            return StringConverter2KOR.GetString(span);
        var result = StringConverter2.GetString(span, Language);
        if (!Korean)
            result = StringConverter2.InflateLigatures(result, Language);
        return result;
    }

    private void SetString(Span<byte> span, ReadOnlySpan<char> value, int maxLength)
    {
        if (Korean)
        {
            StringConverter2KOR.SetString(span, value, maxLength);
            return;
        }
        Span<char> deflated = stackalloc char[maxLength];
        int len = StringConverter2.DeflateLigatures(value, deflated, Language);
        StringConverter2.SetString(span, deflated[..len], maxLength, Language);
    }

    public string Line1
    {
        get => GetString(Data.AsSpan(0, LINE_LENGTH - 1));
        set
        {
            var span = Data.AsSpan(0, LINE_LENGTH);
            SetString(span[..^1], value, LINE_LENGTH - 1);
            span[^1] = 0x4E;
        }
    }

    public string Line2
    {
        get => GetString(Data.AsSpan(LINE_LENGTH, LINE_LENGTH - 1));
        set
        {
            var span = Data.AsSpan(LINE_LENGTH, LINE_LENGTH);
            SetString(span[..^1], value, LINE_LENGTH - 1);
            span[^1] = 0x4E;
        }
    }

    public override string GetMessage(bool isLastLine) => isLastLine ? Line2 : Line1;
    public override void SetMessage(string line1, string line2) => (Line1, Line2) = (line1, line2);

    public override string AuthorName
    {
        get => GetString(Data.AsSpan(OFS_AUTHOR, AUTHOR_LENGTH + 1));
        set
        {
            SetString(Data.AsSpan(OFS_AUTHOR, 8), value, AUTHOR_LENGTH);
            Nationality = 0; // ??
        }
    }

    public ushort Nationality
    {
        get => ReadUInt16BigEndian(Data.AsSpan(OFS_AUTHOR_NATION, 2));
        set => WriteUInt16LittleEndian(Data.AsSpan(OFS_AUTHOR_NATION, 2), value);
    }

    public override ushort AuthorTID
    {
        get => ReadUInt16BigEndian(Data.AsSpan(OFS_AUTHOR_ID + 2));
        set => WriteUInt16BigEndian(Data.AsSpan(OFS_AUTHOR_ID, 2), value);
    }

    public override ushort AppearPKM { get => Data[OFS_APPEAR]; set => Data[OFS_APPEAR] = (byte)value; }
    public override int MailType  { get => Data[OFS_TYPE];   set => Data[OFS_TYPE]   = (byte)value; }

    public override bool? IsEmpty => MailType switch
    {
        0 => true,
        0x9E => false,
        >= 0xB5 and <= 0xBD => false,
        _ => null,
    };

    public override void SetBlank() => Data.AsSpan(0, SIZE).Clear();
}

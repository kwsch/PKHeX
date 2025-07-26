using System;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

public sealed class Mail2 : MailDetail
{
    private readonly bool EnglishGS;
    private readonly bool Japanese;
    private readonly bool Korean;

    // structure:
    private int LINE_LENGTH => Korean ? 0x20 : 0x10;
    private int MESSAGE_LENGTH => LINE_LENGTH + 1 + LINE_LENGTH; // line break in the middle
    private int OFS_AUTHOR => MESSAGE_LENGTH;
    private int OFS_AUTHOR_NATION => OFS_AUTHOR + AUTHOR_LENGTH; // not in Japanese/Korean
    private int OFS_AUTHOR_ID => OFS_AUTHOR_NATION + ((Japanese || Korean) ? 0 : 2);
    private int OFS_APPEAR => OFS_AUTHOR_ID + 2;
    private int OFS_TYPE => OFS_APPEAR + 1;
    private int SIZE => OFS_TYPE + 1;

    private const int SIZE_J = 0x2A;
    private const int SIZE_U = 0x2F;
    private const int SIZE_K = 0x4F;

    private const int COUNT_PARTY = 6;
    private const int COUNT_MAILBOX = 10;
    private const int COUNT_MAILBOX_STADIUM2 = SAV2Stadium.MailboxMailCount; // 50
    private const int COUNT_PARTY_STADIUM2 = SAV2Stadium.MailboxHeldMailCount; // 30

    private int AUTHOR_LENGTH => Japanese ? 5 : (Korean ? 10 : 8);

    private byte LineBreakCode => Korean ? StringConverter2KOR.LineBreakCode : StringConverter2.LineBreakCode;
    private char LineBreak => Korean ? StringConverter2KOR.LineBreak : StringConverter2.LineBreak;

    public Mail2(SAV2 sav, int index) : base(sav.Data.Slice(GetMailOffset(index, GetMailSize(sav.Language)), GetMailSize(sav.Language)).ToArray(), GetMailOffset(index, GetMailSize(sav.Language)))
    {
        EnglishGS = sav.Version != GameVersion.C && sav.Language == (int)LanguageID.English;
        Japanese = sav.Japanese;
        Korean = sav.Korean;
    }

    public Mail2(SAV2Stadium sav, int index) : base(sav.Data.Slice(GetMailOffsetStadium2(index, GetMailSize(sav.Language)), GetMailSize(sav.Language)).ToArray(), GetMailOffsetStadium2(index, GetMailSize(sav.Language)))
    {
        EnglishGS = false;
        Japanese = sav.Japanese;
        Korean = sav.Korean;
    }

    public static int GetMailSize(int language) => language switch
    {
        (int)LanguageID.Japanese => SIZE_J,
        (int)LanguageID.Korean => SIZE_K,
        _ => SIZE_U,
    };

    #region Offsets
    public static int GetMailboxOffset(int language) => 0x600 + (COUNT_PARTY * 2 * GetMailSize(language));

    private static int GetMailOffset(int index, int size)
    {
        if (index < COUNT_PARTY)
            return GetPartyMailOffset(index, size);
        return GetMailboxMailOffset(index - COUNT_PARTY, size);
    }

    private static int GetPartyMailOffset(int index, int size)
    {
        if ((uint)index >= COUNT_PARTY)
            throw new ArgumentOutOfRangeException(nameof(index));
        return (index * size) + 0x600;
    }

    private static int GetMailboxMailOffset(int index, int size)
    {
        if ((uint)index >= COUNT_MAILBOX)
            throw new ArgumentOutOfRangeException(nameof(index));
        return (index * size) + (0x600 + (COUNT_PARTY * 2 * size) + 1);
    }

    public static int GetMailboxOffsetStadium2(int language) => SAV2Stadium.MailboxBlockOffset(language) + 1;

    private static int GetMailOffsetStadium2(int index, int size)
    {
        if (index < COUNT_PARTY_STADIUM2)
            return GetHeldMailOffsetStadium2(index, size);
        return GetMailboxMailOffsetStadium2(index - COUNT_PARTY_STADIUM2, size);
    }

    private static int GetMailboxMailOffsetStadium2(int index, int size)
    {
        if ((uint)index >= COUNT_MAILBOX_STADIUM2)
            throw new ArgumentOutOfRangeException(nameof(index));
        return (index * size) + SAV2Stadium.MailboxBlockOffset(size == SIZE_J ? (int)LanguageID.Japanese : (int)LanguageID.English) + 2;
    }

    private static int GetHeldMailOffsetStadium2(int index, int size)
    {
        if ((uint)index >= COUNT_PARTY_STADIUM2)
            throw new ArgumentOutOfRangeException(nameof(index));
        return (index * size) + SAV2Stadium.MailboxHeldBlockOffset(size == SIZE_J ? (int)LanguageID.Japanese : (int)LanguageID.English) + 2;
    }
    #endregion

    private string GetString(Span<byte> span)
    {
        if (Korean)
            return StringConverter2KOR.GetString(span);
        if (EnglishGS)
            StringConverter2.DecodeMailEnglishGS(span, AuthorLanguage);
        var result = StringConverter2.GetString(span, AuthorLanguage);
        if (!Korean)
            result = StringConverter2.InflateLigatures(result, AuthorLanguage);
        return result;
    }

    private void SetString(Span<byte> span, ReadOnlySpan<char> value, int maxLength, StringConverterOption option = StringConverterOption.Clear50)
    {
        if (Korean)
        {
            StringConverter2KOR.SetString(span, value, maxLength, option);
            return;
        }
        Span<char> deflated = stackalloc char[maxLength];
        int len = StringConverter2.DeflateLigatures(value, deflated, AuthorLanguage);
        StringConverter2.SetString(span, deflated[..len], maxLength, AuthorLanguage, option);
        if (EnglishGS)
            StringConverter2.EncodeMailEnglishGS(span, AuthorLanguage);
    }

    public override string GetMessage(bool isLastLine)
    {
        var span = Data[..MESSAGE_LENGTH];
        var index = span.IndexOf(LineBreakCode);
        return index == -1 ? string.Empty : GetString(isLastLine ? span[(index + 1)..] : span[..index]);
    }

    public override void SetMessage(string line1, string line2, bool userEntered)
    {
        if (IsEmpty == true && line1.Length == 0 && line2.Length == 0)
        {
            Data[..MESSAGE_LENGTH].Clear();
            return;
        }

        if (Korean || !userEntered)
        {
            // Japanese/international Randy's mail has a line break in different place.
            // Korean always puts a line break after the first line, even if it's not full.
            var span = Data[..MESSAGE_LENGTH];
            var message = string.Join(LineBreak, line1, line2);
            SetString(span, message, MESSAGE_LENGTH,
                // Randy's mail can have trash bytes after the end of the message, so don't clear it.
                userEntered ? StringConverterOption.Clear50 : StringConverterOption.None);
            return;
        }

        // Japanese/international user-entered mail always has a line break at index 0x10
        var span1 = Data[..LINE_LENGTH];
        SetString(span1, line1, LINE_LENGTH);
        if (line2.Length != 0) // Pad the first line with spaces if needed
            span1.Replace<byte>(0x50, 0x7F);
        Data[LINE_LENGTH] = LineBreakCode;
        var span2 = Data.Slice(LINE_LENGTH + 1, LINE_LENGTH);
        SetString(span2, line2, LINE_LENGTH);
    }

    public override string AuthorName
    {
        get => GetString(Data.Slice(OFS_AUTHOR, AUTHOR_LENGTH));
        set => SetString(Data.Slice(OFS_AUTHOR, AUTHOR_LENGTH), value,
            // Japanese/Korean don't have an extra byte for the terminator.
            AUTHOR_LENGTH - ((Japanese || Korean) ? 0 : 1),
            // Randy's mail can have trash bytes after the end of the OT, so don't clear it.
            UserEntered ? StringConverterOption.Clear50 : StringConverterOption.None);
    }

    public override byte AuthorLanguage
    {
        get
        {
            if (Japanese)
                return (byte)LanguageID.Japanese;
            if (Korean)
                return (byte)LanguageID.Korean;
            return (byte)(Nationality switch
            {
                0x0000 => LanguageID.English,
                0x8485 => LanguageID.French,  // "EF"
                0x8486 => LanguageID.German,  // "EG"
                0x8488 => LanguageID.Italian, // "EI"
                0x8492 => LanguageID.Spanish, // "ES"
                _ => LanguageID.English,
            });
        }
        set
        {
            if (Japanese || Korean)
                return;
            Nationality = (LanguageID)value switch
            {
                LanguageID.English => 0x0000,
                LanguageID.French  => 0x8485, // "EF"
                LanguageID.German  => 0x8486, // "EG"
                LanguageID.Italian => 0x8488, // "EI"
                LanguageID.Spanish => 0x8492, // "ES"
                _ => Nationality, // Invalid, don't change.
            };
        }
    }

    public ushort Nationality
    {
        get => ReadUInt16BigEndian(Data.Slice(OFS_AUTHOR_NATION, 2));
        set => WriteUInt16BigEndian(Data.Slice(OFS_AUTHOR_NATION, 2), value);
    }

    public override ushort AuthorTID
    {
        get => ReadUInt16BigEndian(Data.Slice(OFS_AUTHOR_ID, 2));
        set => WriteUInt16BigEndian(Data.Slice(OFS_AUTHOR_ID, 2), value);
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

    public override bool UserEntered
    {
        get
        {
            // Blank mail, prepare for writing
            if (MailType == 0)
                return true;

            // Japanese/international user-entered mail always has a line break at index 0x10
            // Randy's mail instead has it at index 0x0D (Japanese) or 0x0F (international)
            if (!Korean)
                return Data[LINE_LENGTH] == LineBreakCode;

            // Korean mail can have a line break anywhere, so look at trash bytes instead
            // User-entered mail always fills the message buffer with 0x50
            // Randy's mail has trash bytes after the terminator, so check for any trash bytes
            var span = Data[..MESSAGE_LENGTH];
            var terminator = span.IndexOf<byte>(0x50);
            return terminator == -1 || !span[terminator..].ContainsAnyExcept<byte>(0x50);
        }
    }

    public override void SetBlank() => Data[..SIZE].Clear();
}

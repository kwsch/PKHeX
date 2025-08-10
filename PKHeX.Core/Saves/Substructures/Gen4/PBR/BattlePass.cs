using System;
using System.Text;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

/// <summary>
/// Pokémon Battle Revolution Battle Pass Structure
/// </summary>
public class BattlePass(Memory<byte> raw)
{
    public const int Size = 0x6EC;
    public const int PokeSize = PokeCrypto.SIZE_4STORED + 4;
    public const int Count = 6;

    public Span<byte> Data => raw.Span;

    /// <inheritdoc cref="SaveFile.SID16"/>
    /// <remarks>Used for NPC Trainers. 00000 for player-made Battle Passes.</remarks>
    public ushort SID { get => ReadUInt16BigEndian(Data); set => WriteUInt16BigEndian(Data, value); }

    /// <inheritdoc cref="SaveFile.TID16"/>
    /// <remarks>Used for NPC Trainers. 00000 for player-made Battle Passes.</remarks>
    public ushort TID { get => ReadUInt16BigEndian(Data[0x02..]); set => WriteUInt16BigEndian(Data[0x02..], value); }

    // Japanese: 5 characters + terminator (0000) + 6 unused (FFFF)
    // Western:  9 characters + terminator (0000) + 2 unused (FFFF)
    public Span<byte> NameTrash => Data.Slice(0x04, 0x18);
    public string Name
    {
        get => StringConverter4GC.GetStringUnicodeBR(NameTrash);
        set => StringConverter4GC.SetStringUnicodeBR(value, NameTrash);
    }

    public short TrainerTitle { get => ReadInt16BigEndian(Data[0x1C..]); set => WriteInt16BigEndian(Data[0x1C..], value); }

    public bool Unknown1E_0 { get => FlagUtil.GetFlag(Data, 0x1E, 0); set => FlagUtil.SetFlag(Data, 0x1E, 0, value); } // 0 for Rental/NPC, 1 for Custom/Friend
    public bool PresetGreeting { get => FlagUtil.GetFlag(Data, 0x1E, 1); set => FlagUtil.SetFlag(Data, 0x1E, 1, value); }
    public bool PresetSentOut { get => FlagUtil.GetFlag(Data, 0x1E, 2); set => FlagUtil.SetFlag(Data, 0x1E, 2, value); }
    public bool PresetShift1 { get => FlagUtil.GetFlag(Data, 0x1E, 3); set => FlagUtil.SetFlag(Data, 0x1E, 3, value); }
    public bool PresetShift2 { get => FlagUtil.GetFlag(Data, 0x1E, 4); set => FlagUtil.SetFlag(Data, 0x1E, 4, value); }
    public bool PresetWin { get => FlagUtil.GetFlag(Data, 0x1E, 5); set => FlagUtil.SetFlag(Data, 0x1E, 5, value); }
    public bool PresetLose { get => FlagUtil.GetFlag(Data, 0x1E, 6); set => FlagUtil.SetFlag(Data, 0x1E, 6, value); }
    public bool Unknown1E_7 { get => FlagUtil.GetFlag(Data, 0x1E, 7); set => FlagUtil.SetFlag(Data, 0x1E, 7, value); } // 0 for Rental/NPC/uninitialized, 1 for Custom/Friend

    // 0x1F-0x20 Unused

    public int Model { get => Data[0x21]; set => Data[0x21] = (byte)value; }
    public int Head { get => Data[0x22]; set => Data[0x22] = (byte)value; }
    public int Hair { get => Data[0x23]; set => Data[0x23] = (byte)value; }
    public int Face { get => Data[0x24]; set => Data[0x24] = (byte)value; }
    public int Top { get => Data[0x25]; set => Data[0x25] = (byte)value; }
    public int Bottom { get => Data[0x26]; set => Data[0x26] = (byte)value; }
    public int Shoes { get => Data[0x27]; set => Data[0x27] = (byte)value; }
    public int Hands { get => Data[0x28]; set => Data[0x28] = (byte)value; }
    public int Bag { get => Data[0x29]; set => Data[0x29] = (byte)value; }
    public int Glasses { get => Data[0x2A]; set => Data[0x2A] = (byte)value; }
    public int Badge { get => Data[0x2B]; set => Data[0x2B] = (byte)value; }

    public Span<byte> GreetingTrash => Data.Slice(0x2C, 0x34);
    public string Greeting
    {
        get => StringConverter4GC.GetStringUnicodeBR(GreetingTrash);
        set => StringConverter4GC.SetStringUnicodeBR(value, GreetingTrash);
    }

    public Span<byte> SentOutTrash => Data.Slice(0x60, 0x38);
    public string SentOut
    {
        get => StringConverter4GC.GetStringUnicodeBR(SentOutTrash);
        set => StringConverter4GC.SetStringUnicodeBR(value, SentOutTrash);
    }

    public Span<byte> Shift1Trash => Data.Slice(0x98, 0x34);
    public string Shift1
    {
        get => StringConverter4GC.GetStringUnicodeBR(Shift1Trash);
        set => StringConverter4GC.SetStringUnicodeBR(value, Shift1Trash);
    }

    public Span<byte> Shift2Trash => Data.Slice(0xCC, 0x34);
    public string Shift2
    {
        get => StringConverter4GC.GetStringUnicodeBR(Shift2Trash);
        set => StringConverter4GC.SetStringUnicodeBR(value, Shift2Trash);
    }

    public Span<byte> WinTrash => Data.Slice(0x100, 0x68);
    public string Win
    {
        get => StringConverter4GC.GetStringUnicodeBR(WinTrash);
        set => StringConverter4GC.SetStringUnicodeBR(value, WinTrash);
    }

    public Span<byte> LoseTrash => Data.Slice(0x168, 0x68);
    public string Lose
    {
        get => StringConverter4GC.GetStringUnicodeBR(LoseTrash);
        set => StringConverter4GC.SetStringUnicodeBR(value, LoseTrash);
    }

    // 0x1D0-1EB: Unused

    public short Unknown1EC { get => ReadInt16BigEndian(Data[0x1EC..]); set => WriteInt16BigEndian(Data[0x1EC..], value); } // -1 for Custom/Friend/Other, 0 for Rental/uninitialized
    public int Skin { get => Data[0x1EE]; set => Data[0x1EE] = (byte)value; }

    // 0x1EF: Unused

    // These values correspond to the string indexes for NPC Trainer's catchphrases.
    // For player-created Passes, these are set based on the character style selected when the Pass is first issued.
    // However, the game instead chooses the appropriate strings based on the current character style and Trainer Title.
    public ushort PresetGreetingIndex { get => ReadUInt16BigEndian(Data[0x1F0..]); set => WriteUInt16BigEndian(Data[0x1F0..], value); }
    public ushort PresetSentOutIndex { get => ReadUInt16BigEndian(Data[0x1F2..]); set => WriteUInt16BigEndian(Data[0x1F2..], value); }
    public ushort PresetShift1Index { get => ReadUInt16BigEndian(Data[0x1F4..]); set => WriteUInt16BigEndian(Data[0x1F4..], value); }
    public ushort PresetShift2Index { get => ReadUInt16BigEndian(Data[0x1F6..]); set => WriteUInt16BigEndian(Data[0x1F6..], value); }
    public ushort PresetWinIndex { get => ReadUInt16BigEndian(Data[0x1F8..]); set => WriteUInt16BigEndian(Data[0x1F8..], value); }
    public ushort PresetLoseIndex { get => ReadUInt16BigEndian(Data[0x1FA..]); set => WriteUInt16BigEndian(Data[0x1FA..], value); }

    /// <summary>
    /// Resets the preset catchphrase string indexes to the default values based on the current character style and pass type.
    /// </summary>
    public void ResetPresetIndexes()
    {
        ushort index = ((ModelBR)Model switch
        {
            ModelBR.YoungBoy => 6872,
            ModelBR.CoolBoy => 7058,
            ModelBR.MuscleMan => 7244,
            ModelBR.YoungGirl => 7430,
            ModelBR.CoolGirl => 7616,
            ModelBR.LittleGirl => 7802,
            _ => 0,
        });
        if (index == 0)
            return;

        (PresetGreetingIndex, PresetSentOutIndex, PresetShift1Index, PresetShift2Index, PresetWinIndex, PresetLoseIndex) =
            ((ushort, ushort, ushort, ushort, ushort, ushort))(Rental ?
                (index, index + 1, index + 2, index + 3, index + 4, index + 5) :
                (index + 6, index + 1, index + 2, index + 3, index + 7, index + 8));
    }

    // 0x1FC-283 Party Member 1
    // 0x288-30F Party Member 2
    // 0x314-39B Party Member 3
    // 0x3A0-427 Party Member 4
    // 0x42C-4B3 Party Member 5
    // 0x4B8-53F Party Member 6
    private static int GetPartyOffset(int index) => 0x1FC + (PokeSize * index);
    private Span<byte> GetPartySpan(int index) => Data.Slice(GetPartyOffset(index), PokeSize);
    public BK4 GetPartySlotAtIndex(int index) => BK4.ReadUnshuffle(GetPartySpan(index));
    public void SetPartySlotAtIndex(PKM pk, int index)
    {
        while (index > 0 && !GetPartySlotPresent(index - 1))
            index--;

        pk.EncryptedBoxData.CopyTo(GetPartySpan(index));
        SetPartySlotPresent(index, pk.Species != 0);
    }

    public void DeletePartySlot(int index)
    {
        while (index < Count - 1 && GetPartySlotPresent(index + 1))
        {
            GetPartySpan(index + 1).CopyTo(GetPartySpan(index));
            index++;
        }

        GetPartySpan(index).Clear();
    }

    // Box Numbers: 0 Blank/Removed, 0 Party, 1-18 Boxes, 255 Disappeared/Rental
    // Slot Numbers: 0 Blank/Removed, 0-5 Party, 0-29 Boxes, 255 Disappeared, 0 Rental
    public (byte Box, byte Slot) GetPartySlotBoxSlot(int index)
    {
        var span = GetPartySpan(index);
        return (span[^4], span[^3]);
    }
    public void SetPartySlotBoxSlot(int index, byte box, byte slot)
    {
        var span = GetPartySpan(index);
        span[^4] = box;
        span[^3] = slot;
    }

    public bool GetPartySlotPresent(int index) => FlagUtil.GetFlag(GetPartySpan(index)[^2..], 7);
    public void SetPartySlotPresent(int index, bool value) => FlagUtil.SetFlag(GetPartySpan(index)[^2..], 7, value);

    public ushort GetPartySlotFlags(int index) => (ushort)(ReadUInt16BigEndian(GetPartySpan(index)[^2..]) & 0x7FFF);
    public void SetPartySlotFlags(int index, ushort value)
    {
        var span = GetPartySpan(index)[^2..];
        WriteUInt16BigEndian(span, (ushort)((ReadUInt16BigEndian(span) & 0x8000) | (value & 0x7FFF)));
    }

    public int PictureType { get => FlagUtil.GetFlag(Data, 0x544, 0) ? 1 : 0; set => FlagUtil.SetFlag(Data, 0x544, 0, value != 0); } // 0: Full Body, 1: Head Shot
    public int PassDesign { get => Data[0x544] >> 1; set => Data[0x544] = (byte)((value << 1) | (Data[0x544] & 0x01)); }

    // 0x545, 0-2 Unused

    /// <summary>
    /// Whether the Battle Pass has been unlocked (Custom Passes), or is available to borrow at Gateway Colosseum (Rental Passes).
    /// </summary>
    public bool Available { get => FlagUtil.GetFlag(Data, 0x545, 3); set => FlagUtil.SetFlag(Data, 0x545, 3, value); }

    /// <summary>
    /// Whether the Battle Pass has been issued (Custom Passes), or unlocked after being used to clear Gateway Colosseum (Rental Passes).
    /// </summary>
    public bool Issued { get => FlagUtil.GetFlag(Data, 0x545, 4); set => FlagUtil.SetFlag(Data, 0x545, 4, value); }

    // 0x545, 5 Unused

    public bool Rental { get => FlagUtil.GetFlag(Data, 0x545, 6); set => FlagUtil.SetFlag(Data, 0x545, 6, value); }
    public bool Friend { get => FlagUtil.GetFlag(Data, 0x545, 7); set => FlagUtil.SetFlag(Data, 0x545, 7, value); }

    // 0x546-547 Padding (always 0)

    #region Creator
    public Span<byte> CreatorNameTrash => Data.Slice(0x548, 0x20);
    public string CreatorName
    {
        get => StringConverter4GC.GetStringUnicodeBR(CreatorNameTrash);
        set => StringConverter4GC.SetStringUnicodeBR(value, CreatorNameTrash);
    }

    public Span<byte> BirthMonthTrash => Data.Slice(0x568, 0x8);
    public string BirthMonth
    {
        get => StringConverter4GC.GetStringUnicodeBR(BirthMonthTrash);
        set => StringConverter4GC.SetStringUnicodeBR(value, BirthMonthTrash);
    }

    public Span<byte> BirthDayTrash => Data.Slice(0x570, 0x8);
    public string BirthDay
    {
        get => StringConverter4GC.GetStringUnicodeBR(BirthDayTrash);
        set => StringConverter4GC.SetStringUnicodeBR(value, BirthDayTrash);
    }

    public int Country { get => ReadUInt16BigEndian(Data[0x578..]); set => WriteUInt16BigEndian(Data[0x578..], (ushort)value); }
    public int Region { get => ReadUInt16BigEndian(Data[0x57A..]); set => WriteUInt16BigEndian(Data[0x57A..], (ushort)value); }

    public Span<byte> SelfIntroductionTrash => Data.Slice(0x57C, 0x6C);

    /// <inheritdoc cref="SAV4BR.SelfIntroduction"/>
    public string SelfIntroduction
    {
        get => StringConverter4GC.GetStringUnicodeBR(SelfIntroductionTrash);
        set => StringConverter4GC.SetStringUnicodeBR(value, SelfIntroductionTrash);
    }

    public int Battles { get => ReadInt32BigEndian(Data[0x5E8..]); set => WriteInt32BigEndian(Data[0x5E8..], value); }

    /// <summary>
    /// The creator's region code, based on the game and console's region.
    /// </summary>
    /// <remarks>
    /// "\0\0\0\0" for JPN, "USA " for USA, "EURO" for PAL
    /// </remarks>
    public string RegionCode
    {
        get => Encoding.ASCII.GetString(Data.Slice(0x5ED, 4));
        set
        {
            for (int i = 0; i < 4; i++)
                Data[0x5ED + i] = (byte)(value.Length > i ? value[i] : '\0');
        }
    }

    public int RecordColosseumBattles { get => ReadInt32BigEndian(Data[0x6C4..]); set => WriteInt32BigEndian(Data[0x6C4..], value); }
    public int RecordFreeBattles { get => ReadInt32BigEndian(Data[0x6C8..]); set => WriteInt32BigEndian(Data[0x6C8..], value); }
    public int RecordWiFiBattles { get => ReadInt32BigEndian(Data[0x6CC..]); set => WriteInt32BigEndian(Data[0x6CC..], value); }
    public int RecordGatewayColosseumClears { get => Data[0x6D0]; set => Data[0x6D0] = (byte)value; }
    public int RecordMainStreetColosseumClears { get => Data[0x6D1]; set => Data[0x6D1] = (byte)value; }
    public int RecordWaterfallColosseumClears { get => Data[0x6D2]; set => Data[0x6D2] = (byte)value; }
    public int RecordNeonColosseumClears { get => Data[0x6D3]; set => Data[0x6D3] = (byte)value; }
    public int RecordCrystalColosseumClears { get => Data[0x6D4]; set => Data[0x6D4] = (byte)value; }
    public int RecordSunnyParkColosseumClears { get => Data[0x6D5]; set => Data[0x6D5] = (byte)value; }
    public int RecordMagmaColosseumClears { get => Data[0x6D6]; set => Data[0x6D6] = (byte)value; }
    public int RecordCourtyardColosseumClears { get => Data[0x6D7]; set => Data[0x6D7] = (byte)value; }
    public int RecordSunsetColosseumClears { get => Data[0x6D8]; set => Data[0x6D8] = (byte)value; }
    public int RecordStargazerColosseumClears { get => Data[0x6D9]; set => Data[0x6D9] = (byte)value; }

    /// <inheritdoc cref="SAV4BR.PlayerID"/>
    public ulong PlayerID { get => ReadUInt64BigEndian(Data[0x6DC..]); set => WriteUInt64BigEndian(Data[0x6DC..], value); }

    public BattlePassLanguage Language { get => (BattlePassLanguage)Data[0x6E6]; set => Data[0x6E6] = (byte)value; }
    #endregion
}

public enum PictureTypeBR : byte
{
    FullBody = 0,
    HeadShot = 1,
}

public enum SkinColorBR : byte
{
    Light = 0, // Always 0 in Japanese
    Tan = 1,
    Dark = 2,
}

using System;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

/// <summary> Generation 8 <see cref="PKM"/> format. </summary>
public sealed class PK8 : G8PKM, IHandlerUpdate
{
    public override ReadOnlySpan<ushort> ExtraBytes =>
    [
        // Alignment bytes
        0x17, 0x1A, 0x1B, 0x23, 0x33, 0x3E, 0x3F,
        0x4C, 0x4D, 0x4E, 0x4F,
        0x52, 0x53, 0x54, 0x55, 0x56, 0x57,

        0x91, 0x92, 0x93,
        0x9C, 0x9D, 0x9E, 0x9F, 0xA0, 0xA1, 0xA2, 0xA3, 0xA4, 0xA5, 0xA6, 0xA7,

        0xC5,
        0xCE, 0xCF, 0xD0, 0xD1, 0xD2, 0xD3, 0xD4, 0xD5, 0xD6, 0xD7, 0xD8, 0xD9, 0xDA, 0xDB, // Pokejob
        0xE0, 0xE1, // Old Console Region / Region
        0xE9, 0xEA, 0xEB, 0xEC, 0xED, 0xEE, 0xEF, 0xF0, 0xF1, 0xF2, 0xF3, 0xF4, 0xF5, 0xF6, 0xF7,
        0x115, 0x11F, // Alignment

        0x13D, 0x13E, 0x13F,
        0x140, 0x141, 0x142, 0x143, 0x144, 0x145, 0x146, 0x147,
    ];

    public override PersonalInfo8SWSH PersonalInfo => PersonalTable.SWSH.GetFormEntry(Species, Form);
    public override IPermitRecord Permit => PersonalInfo;
    public override EntityContext Context => EntityContext.Gen8;

    public PK8() => AffixedRibbon = -1; // 00 would make it show Kalos Champion :)
    public PK8(byte[] data) : base(data) { }
    public override PK8 Clone() => new((byte[])Data.Clone());

    // Synthetic Trading Logic
    public bool BelongsTo(ITrainerInfo tr)
    {
        if (tr.Version != Version)
            return false;
        if (tr.ID32 != ID32)
            return false;
        if (tr.Gender != OriginalTrainerGender)
            return false;

        Span<char> ot = stackalloc char[MaxStringLengthTrainer];
        int len = LoadString(OriginalTrainerTrash, ot);
        return ot[..len].SequenceEqual(tr.OT);
    }

    public void UpdateHandler(ITrainerInfo tr)
    {
        if (IsEgg)
        {
            // Eggs do not have any modifications done if they are traded
            // Apply link trade data, only if it left the OT (ignore if dumped & imported, or cloned, etc.)
            const ushort location = Locations.LinkTrade6;
            if (MetLocation != location && !BelongsTo(tr))
            {
                var date = EncounterDate.GetDateSwitch();
                SetLinkTradeEgg(date.Day, date.Month, date.Year, location);
            }
            return;
        }

        // Process to the HT if the OT of the Pokémon does not match the SAV's OT info.
        if (!TradeOT(tr))
            TradeHT(tr);
    }

    public int DynamaxType { get => ReadUInt16LittleEndian(Data.AsSpan(0x156)); set => WriteUInt16LittleEndian(Data.AsSpan(0x156), (ushort)value); }

    public void FixMemories()
    {
        if (IsEgg) // No memories if is egg.
        {
            this.ClearMemoriesOT();
            this.ClearMemoriesHT();
            HandlingTrainerGender = HandlingTrainerFriendship = HandlingTrainerLanguage = 0;
            HandlingTrainerTrash.Clear();
            return;
        }

        if (IsUntraded)
        {
            this.ClearMemoriesHT();
            HandlingTrainerGender = HandlingTrainerFriendship = HandlingTrainerLanguage = 0;
            HandlingTrainerTrash.Clear();
        }
        else
        {
            var gen = Generation;
            if (gen < 6)
                this.ClearMemoriesOT();
        }
    }

    private bool TradeOT(ITrainerInfo tr)
    {
        // Check to see if the OT matches the SAV's OT info.
        if (!BelongsTo(tr))
            return false;

        CurrentHandler = 0;
        return true;
    }

    private void TradeHT(ITrainerInfo tr) => PKH.UpdateHandler(this, tr);

    // Maximums
    public override ushort MaxMoveID => Legal.MaxMoveID_8;
    public override ushort MaxSpeciesID => Legal.MaxSpeciesID_8;
    public override int MaxAbilityID => Legal.MaxAbilityID_8;
    public override int MaxItemID => Legal.MaxItemID_8;
    public override int MaxBallID => Legal.MaxBallID_8;
    public override GameVersion MaxGameID => Legal.MaxGameID_8;
    public bool IsSideTransfer => LocationsHOME.IsLocationSWSH(MetLocation);
    public override bool SV => MetLocation is LocationsHOME.SWSL or LocationsHOME.SHVL;
    public override bool BDSP => MetLocation is LocationsHOME.SWBD or LocationsHOME.SHSP;
    public override bool LA => MetLocation is LocationsHOME.SWLA;
    public override bool HasOriginalMetLocation => base.HasOriginalMetLocation && !IsSideTransfer;

    public override string GetString(ReadOnlySpan<byte> data)
        => StringConverter8.GetString(data);
    public override int LoadString(ReadOnlySpan<byte> data, Span<char> destBuffer)
        => StringConverter8.LoadString(data, destBuffer);
    public override int SetString(Span<byte> destBuffer, ReadOnlySpan<char> value, int maxLength, StringConverterOption option)
        => StringConverter8.SetString(destBuffer, value, maxLength, option);
    public override int GetStringTerminatorIndex(ReadOnlySpan<byte> data)
        => TrashBytesUTF16.GetTerminatorIndex(data);
    public override int GetStringLength(ReadOnlySpan<byte> data)
        => TrashBytesUTF16.GetStringLength(data);
    public override int GetBytesPerChar() => 2;
}

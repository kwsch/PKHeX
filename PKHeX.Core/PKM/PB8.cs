using System;
using System.Collections.Generic;

namespace PKHeX.Core;

/// <summary> Generation 8 <see cref="PKM"/> format. </summary>
public sealed class PB8 : G8PKM
{
    private static readonly ushort[] Unused =
    {
        // Alignment bytes
        0x17, 0x1A, 0x1B, 0x23, 0x33, 0x3E, 0x3F,
        0x4C, 0x4D, 0x4E, 0x4F,
        0x52, 0x53, 0x54, 0x55, 0x56, 0x57,

        0x91, 0x92, 0x93,
        0x9C, 0x9D, 0x9E, 0x9F, 0xA0, 0xA1, 0xA2, 0xA3, 0xA4, 0xA5, 0xA6, 0xA7,

        0xC5,
        0xCE, 0xCF, 0xD0, 0xD1, 0xD2, 0xD3, 0xD4, 0xD5, 0xD6, 0xD7, 0xD8, 0xD9, 0xDA, 0xDB,
        0xE0, 0xE1, // Old Console Region / Region
        0xE9, 0xEA, 0xEB, 0xEC, 0xED, 0xEE, 0xEF, 0xF0, 0xF1, 0xF2, 0xF3, 0xF4, 0xF5, 0xF6, 0xF7,
        0x115, 0x11F, // Alignment

        0x13D, 0x13E, 0x13F,
        0x140, 0x141, 0x142, 0x143, 0x144, 0x145, 0x146, 0x147,
    };

    public override IReadOnlyList<ushort> ExtraBytes => Unused;
    public override PersonalInfo PersonalInfo => PersonalTable.BDSP.GetFormEntry(Species, Form);
    public override bool IsNative => BDSP;
    public override EntityContext Context => EntityContext.Gen8b;

    public PB8()
    {
        Egg_Location = Met_Location = Locations.Default8bNone;
        AffixedRibbon = -1; // 00 would make it show Kalos Champion :)
    }

    public PB8(byte[] data) : base(data) { }
    public override PKM Clone() => new PB8((byte[])Data.Clone());

    public bool IsDprIllegal
    {
        get => Data[0x52] != 0;
        set => Data[0x52] = (byte)((Data[0x52] & 0xFE) | (value ? 1 : 0));
    }

    public void Trade(ITrainerInfo tr, int Day = 1, int Month = 1, int Year = 2015)
    {
        if (IsEgg)
        {
            // Apply link trade data, only if it left the OT (ignore if dumped & imported, or cloned, etc)
            if ((tr.TID != TID) || (tr.SID != SID) || (tr.Gender != OT_Gender) || (tr.OT != OT_Name))
                SetLinkTradeEgg(Day, Month, Year, Locations.LinkTrade6NPC);

            // Unfortunately, BDSP doesn't return if it's an egg, and can update the HT details & handler.
            // Continue to the rest of the method.
            // return;
        }

        // Process to the HT if the OT of the Pokémon does not match the SAV's OT info.
        if (!TradeOT(tr))
            TradeHT(tr);
    }

    public void FixMemories()
    {
        if (BDSP)
        {
            OT_TextVar = OT_Memory = OT_Intensity = OT_Feeling = 0;
            HT_TextVar = HT_Memory = HT_Intensity = HT_Feeling = 0; // future inter-format conversion?
        }

        if (IsEgg) // No memories if is egg.
        {
            HT_TextVar = HT_Memory = HT_Intensity = HT_Feeling = 0;
            OT_TextVar = OT_Memory = OT_Intensity = OT_Feeling = 0;

            // Clear Handler
            if (!IsTradedEgg)
            {
                HT_Friendship = HT_Gender = HT_Language = 0;
                HT_Trash.Clear();
            }
            return;
        }

        if (IsUntraded)
            HT_Gender = HT_Friendship = HT_TextVar = HT_Memory = HT_Intensity = HT_Feeling = HT_Language = 0;

        int gen = Generation;
        if (gen < 6)
            OT_TextVar = OT_Memory = OT_Intensity = OT_Feeling = 0;
        // if (gen != 8) // must be transferred via HOME, and must have memories
        //     this.SetTradeMemoryHT8(); // not faking HOME tracker.
    }

    private bool TradeOT(ITrainerInfo tr)
    {
        // Check to see if the OT matches the SAV's OT info.
        if (!(tr.TID == TID && tr.SID == SID && tr.Gender == OT_Gender && tr.OT == OT_Name))
            return false;

        CurrentHandler = 0;
        return true;
    }

    private void TradeHT(ITrainerInfo tr)
    {
        if (HT_Name != tr.OT)
        {
            HT_Friendship = PersonalInfo.BaseFriendship;
            HT_Name = tr.OT;
        }
        CurrentHandler = 1;
        HT_Gender = tr.Gender;
        HT_Language = (byte)tr.Language;
        //this.SetTradeMemoryHT8();
    }

    // Maximums
    public override ushort MaxMoveID => Legal.MaxMoveID_8b;
    public override ushort MaxSpeciesID => Legal.MaxSpeciesID_8b;
    public override int MaxAbilityID => Legal.MaxAbilityID_8b;
    public override int MaxItemID => Legal.MaxItemID_8b;
    public override int MaxBallID => Legal.MaxBallID_8b;
    public override int MaxGameID => Legal.MaxGameID_8b;

    public override bool WasEgg => IsEgg || Egg_Day != 0;

    public PK8 ConvertToPK8()
    {
        var pk = ConvertTo<PK8>();
        pk.SanitizeImport();
        pk.Egg_Location = GetEggLocationPK8();
        return pk;
    }

    private int GetEggLocationPK8()
    {
        var egg = Egg_Location;
        if (egg == Locations.Default8bNone)
            return 0;
        return Version switch
        {
            (int)GameVersion.BD => egg is Locations.LinkTrade6NPC ? Locations.HOME_SWBD : Locations.HOME_SWSHBDSPEgg,
            (int)GameVersion.SH => egg is Locations.LinkTrade6NPC ? Locations.HOME_SHSP : Locations.HOME_SWSHBDSPEgg,
            _ => egg,
        };
    }

    public override PA8 ConvertToPA8()
    {
        var pk = base.ConvertToPA8();
        if (pk.Egg_Location == Locations.Default8bNone)
            pk.Egg_Location = 0;
        return pk;
    }

    public override bool HasOriginalMetLocation => base.HasOriginalMetLocation && !(LA && Met_Location == Locations.HOME_SWLA);

    public override void ResetMoves()
    {
        var learnsets = Legal.LevelUpBDSP;
        var table = PersonalTable.BDSP;

        var index = table.GetFormIndex(Species, Form);
        var learn = learnsets[index];
        Span<ushort> moves = stackalloc ushort[4];
        learn.SetEncounterMoves(CurrentLevel, moves);
        SetMoves(moves);
        this.SetMaximumPPCurrent(moves);
    }
}

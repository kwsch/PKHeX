using static PKHeX.Core.Species;

namespace PKHeX.Core;

/// <summary>
/// Generation 2 Trade Encounter
/// </summary>
/// <inheritdoc cref="EncounterTradeGB"/>
public sealed record EncounterTrade2 : EncounterTradeGB
{
    public override int Generation => 2;
    public override EntityContext Context => EntityContext.Gen2;
    public override int Location => Locations.LinkTrade2NPC;

    public EncounterTrade2(ushort species, byte level, ushort tid) : base(species, level, GameVersion.GSC)
    {
        TID = tid;
    }

    public override bool IsMatchExact(PKM pk, EvoCriteria evo)
    {
        if (Level > pk.CurrentLevel) // minimum required level
            return false;
        if (TID != pk.TID)
            return false;

        if (pk.Format <= 2)
        {
            if (Gender >= 0 && Gender != pk.Gender)
                return false;
            if (IVs.Count != 0 && !Legal.GetIsFixedIVSequenceValidNoRand((int[])IVs, pk))
                return false;
            if (pk.Format == 2 && pk.Met_Location is not (0 or 126))
                return false;
        }

        if (!IsValidTradeOTGender(pk))
            return false;
        return IsValidTradeOTName(pk);
    }

    private bool IsValidTradeOTGender(PKM pk)
    {
        if (OTGender == 1)
        {
            // Female, can be cleared if traded to RBY (clears met location)
            if (pk.Format <= 2)
                return pk.OT_Gender == (pk.Met_Location != 0 ? 1 : 0);
            return pk.OT_Gender == 0 || !pk.VC1; // require male except if transferred from GSC
        }
        return pk.OT_Gender == 0;
    }

    private bool IsValidTradeOTName(PKM pk)
    {
        var OT = pk.OT_Name;
        if (pk.Japanese)
            return GetOT((int)LanguageID.Japanese) == OT;
        if (pk.Korean)
            return GetOT((int)LanguageID.Korean) == OT;

        var lang = GetInternationalLanguageID(OT);
        if (pk.Format < 7)
            return lang != -1;

        switch (Species)
        {
            case (int)Voltorb when pk.Language == (int)LanguageID.French:
                if (lang == (int)LanguageID.Spanish)
                    return false;
                if (lang != -1)
                    return true;
                return OT == "FALCçN"; // FALCÁN

            case (int)Shuckle when pk.Language == (int)LanguageID.French:
                if (lang == (int)LanguageID.Spanish)
                    return false;
                if (lang != -1)
                    return true;
                return OT == "MANôA"; // MANÍA

            default: return lang != -1;
        }
    }

    private int GetInternationalLanguageID(string OT)
    {
        const int start = (int)LanguageID.English;
        const int end = (int)LanguageID.Spanish;

        var tr = TrainerNames;
        for (int i = start; i <= end; i++)
        {
            if (tr[i] == OT)
                return i;
        }
        return -1;
    }
}

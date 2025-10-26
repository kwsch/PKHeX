using System;
using PKHeX.Core;

namespace PKHeX.WinForms.Controls;

public partial class PKMEditor
{
    private void PopulateFieldsPK2()
    {
        if (Entity is not (GBPKM pk2 and ICaughtData2 c2))
            throw new FormatException(nameof(Entity));

        if (Entity is SK2 sk2)
        {
            var sav = RequestSaveFile;
            CoerceStadium2Language(sk2, sav);
        }
        LoadMisc1(pk2);
        LoadMisc2(pk2);

        TID_Trainer.LoadIDValues(pk2, pk2.Format);
        TB_MetLevel.Text = c2.MetLevel.ToString();
        CB_MetLocation.SelectedValue = (int)c2.MetLocation;
        CB_MetTimeOfDay.SelectedIndex = c2.MetTimeOfDay;

        // Attempt to detect language
        var language = RequestSaveFile.Language;
        CB_Language.SelectedValue = pk2.IsSpeciesNameMatch(language) ? language : pk2.GuessedLanguage(language);

        LoadPartyStats(pk2);
        UpdateStats();
    }

    private static void CoerceStadium2Language(SK2 sk2, SaveFile sav)
    {
        if (sk2.Japanese == (sav.Language == 1))
            return;

        var la = new LegalityAnalysis(sk2);
        if (la.Valid || !sk2.IsPossible(sav.Language == 1))
            return;

        sk2.SwapLanguage();
        la = new LegalityAnalysis(sk2);
        if (la.Valid)
            return;

        Span<char> nickname = stackalloc char[sk2.MaxStringLengthNickname];
        int len = sk2.LoadString(sk2.NicknameTrash, nickname);
        var lang = SpeciesName.GetSpeciesNameLanguage(sk2.Species, nickname[..len], EntityContext.Gen2);
        if (lang >= 1 && (lang == 1 != sk2.Japanese)) // force match language
            sk2.SwapLanguage();
        else if (sk2.Japanese != (sav.Language == 1)) // force match save file
            sk2.SwapLanguage();
    }

    private GBPKM PreparePK2()
    {
        if (Entity is not (GBPKM pk2 and ICaughtData2 c2))
            throw new FormatException(nameof(Entity));

        SaveMisc1(pk2);
        SaveMisc2(pk2);

        c2.MetLevel = (byte)Util.ToInt32(TB_MetLevel.Text);
        c2.MetLocation = (ushort)WinFormsUtil.GetIndex(CB_MetLocation);
        c2.MetTimeOfDay = CB_MetTimeOfDay.SelectedIndex;

        SavePartyStats(pk2);
        pk2.FixMoves();
        return pk2;
    }
}

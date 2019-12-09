using System;
using System.Windows.Forms;
using PKHeX.Core;

namespace PKHeX.WinForms.Controls
{
    public partial class CatchRate : UserControl
    {
        private PK1 pk1;
        public CatchRate() => InitializeComponent();

        public void LoadPK1(PK1 pk) => NUD_CatchRate.Value = (pk1 = pk).Catch_Rate;
        private void ChangeValue(object sender, EventArgs e) => pk1.Catch_Rate = (int)NUD_CatchRate.Value;
        private void Clear(object sender, EventArgs e) => NUD_CatchRate.Value = 0;

        private void Reset(object sender, EventArgs e)
        {
            var sav = WinFormsUtil.FindFirstControlOfType<IMainEditor>(this).RequestSaveFile;
            NUD_CatchRate.Value = GetSuggestedPKMCatchRate(pk1, sav);
        }

        private static int GetSuggestedPKMCatchRate(PK1 pk1, SaveFile sav)
        {
            var la = new LegalityAnalysis(pk1);
            if (la.Valid)
                return pk1.Catch_Rate;

            if (la.Info.Generation == 2)
                return 0;

            var enc = la.EncounterOriginal;
            switch (enc)
            {
                case EncounterTradeCatchRate c:
                    return (int)c.Catch_Rate;
                case EncounterStatic s when s.Version == GameVersion.Stadium && s.Species == (int)Species.Psyduck:
                    return pk1.Japanese ? 167 : 168; // Amnesia Psyduck has different catch rates depending on language
                case IVersion v:
                {
                    if (sav.Version.Contains(v.Version) || v.Version.Contains(sav.Version))
                        return sav.Personal[enc.Species].CatchRate;
                    if (!GameVersion.RB.Contains(v.Version))
                        return PersonalTable.Y[enc.Species].CatchRate;
                    return PersonalTable.RB[enc.Species].CatchRate;
                }
                default:
                    return sav.Personal[enc.Species].CatchRate;
            }
        }
    }
}

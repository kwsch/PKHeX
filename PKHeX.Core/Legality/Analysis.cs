using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using static PKHeX.Core.LegalityCheckStrings;

namespace PKHeX.Core
{
    public partial class LegalityAnalysis
    {
        private PKM pkm;
        private readonly bool Error;
        private readonly List<CheckResult> Parse = new List<CheckResult>();

        private IEncounterable EncounterOriginalGB;
        private IEncounterable EncounterMatch => Info.EncounterMatch;
        private Type Type; // Parent class when applicable (EncounterStatic / MysteryGift)
        private Type MatchedType; // Child class if applicable (WC6, PGF, etc)
        private string EncounterName => Legal.GetEncounterTypeName(EncounterOriginalGB ?? EncounterMatch) + $" ({SpeciesStrings[EncounterMatch.Species]})";
        private CheckResult Encounter, History;

        public bool Parsed { get; }
        public bool Valid { get; }
        public LegalInfo Info { get; private set; }
        public bool ParsedValid => Parsed && Valid;
        public bool ParsedInvalid => Parsed && !Valid;
        public string Report(bool verbose = false) => verbose ? GetVerboseLegalityReport() : GetLegalityReport();
        private IEnumerable<int> AllSuggestedMoves
        {
            get
            {
                if (_allSuggestedMoves != null)
                    return _allSuggestedMoves;
                if (Error || pkm == null || !pkm.IsOriginValid)
                    return new int[4];
                return _allSuggestedMoves = GetSuggestedMoves(true, true, true);
            }
        }
        private IEnumerable<int> AllSuggestedRelearnMoves
        {
            get
            {
                if (_allSuggestedRelearnMoves != null)
                    return _allSuggestedRelearnMoves;
                if (Error || pkm == null || !pkm.IsOriginValid)
                    return new int[4];
                var gender = pkm.PersonalInfo.Gender;
                var inheritLvlMoves = gender > 0 && gender < 255 || Legal.MixedGenderBreeding.Contains(Info.EncounterMatch.Species);
                return _allSuggestedRelearnMoves = Legal.GetValidRelearn(pkm, Info.EncounterMatch.Species, inheritLvlMoves).ToArray();
            }
        }
        private int[] _allSuggestedMoves, _allSuggestedRelearnMoves;
        public int[] AllSuggestedMovesAndRelearn => AllSuggestedMoves.Concat(AllSuggestedRelearnMoves).ToArray();

        public LegalityAnalysis(PKM pk)
        {
            try
            {
                switch (pk.Format) // prior to storing GameVersion
                {
                    case 1: ParsePK1(pk); break;
                    case 2: ParsePK1(pk); break;
                }

                if (!Parse.Any())
                switch (pk.GenNumber)
                {
                    case 3: ParsePK3(pk); break;
                    case 4: ParsePK4(pk); break;
                    case 5: ParsePK5(pk); break;
                    case 6: ParsePK6(pk); break;

                    case 1: ParsePK7(pk); break;
                    case 7: ParsePK7(pk); break;
                }

                if (Parse.Count > 0)
                {
                    if (Parse.Any(chk => !chk.Valid))
                        Valid = false;
                    else if (Info.Moves.Any(m => m.Valid != true))
                        Valid = false;
                    else if (Info.Relearn.Any(m => m.Valid != true))
                        Valid = false;
                    else
                        Valid = true;

                    if (pkm.FatefulEncounter && Info.Relearn.Any(chk => !chk.Valid) && EncounterMatch == null)
                        AddLine(Severity.Indeterminate, V188, CheckIdentifier.Fateful);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Valid = false;
                AddLine(Severity.Invalid, V190, CheckIdentifier.Misc);
                pkm = pk;
                Error = true;
            }
            Parsed = true;
        }

        private void AddLine(Severity s, string c, CheckIdentifier i)
        {
            AddLine(new CheckResult(s, c, i));
        }
        private void AddLine(CheckResult chk)
        {
            Parse.Add(chk);
        }

        private void ParsePK1(PKM pk)
        {
            pkm = pk;
            if (!pkm.IsOriginValid)
            { AddLine(Severity.Invalid, V187, CheckIdentifier.GameOrigin); return; }
            UpdateTradebackG12();

            UpdateInfo();
            UpdateTypeInfo();
            if (pk.Format > 2) // transferred
            {
                EncounterOriginalGB = EncounterMatch;
                foreach (var z in VerifyVCEncounter(pkm, EncounterMatch.Species, EncounterMatch as GBEncounterData))
                    AddLine(z);
            }
            VerifyNickname();
            VerifyDVs();
            VerifyEVs();
            VerifyG1OT();
            VerifyMiscG1();
        }
        private void ParsePK3(PKM pk)
        {
            pkm = pk;
            if (!pkm.IsOriginValid)
            { AddLine(Severity.Invalid, V187, CheckIdentifier.GameOrigin); return; }

            UpdateInfo();
            UpdateTypeInfo();
            UpdateChecks();
            if (pkm.Format > 3)
                VerifyTransferLegalityG3();

            if (pkm.Version == 15)
                VerifyCXD();

            if (Info.EncounterMatch is WC3 z && z.NotDistributed)
                AddLine(Severity.Invalid, V413, CheckIdentifier.Encounter);
        }
        private void ParsePK4(PKM pk)
        {
            pkm = pk;
            if (!pkm.IsOriginValid)
            { AddLine(Severity.Invalid, V187, CheckIdentifier.GameOrigin); return; }

            UpdateInfo();
            UpdateTypeInfo();
            UpdateChecks();
            if (pkm.Format > 4)
                VerifyTransferLegalityG4();
        }
        private void ParsePK5(PKM pk)
        {
            pkm = pk;
            if (!pkm.IsOriginValid)
            { AddLine(Severity.Invalid, V187, CheckIdentifier.GameOrigin); return; }

            UpdateInfo();
            UpdateTypeInfo();
            UpdateChecks();
        }
        private void ParsePK6(PKM pk)
        {
            pkm = pk;
            if (!pkm.IsOriginValid)
            { AddLine(Severity.Invalid, V187, CheckIdentifier.GameOrigin); return; }

            UpdateInfo();
            UpdateTypeInfo();
            UpdateChecks();
        }
        private void ParsePK7(PKM pk)
        {
            pkm = pk;
            if (!pkm.IsOriginValid)
            { AddLine(Severity.Invalid, V187, CheckIdentifier.GameOrigin); return; }

            UpdateInfo();
            UpdateTypeInfo();
            UpdateChecks();
        }

        private void UpdateInfo()
        {
            Info = EncounterFinder.FindVerifiedEncounter(pkm);
            Encounter = Info.Parse[0];
            Parse.AddRange(Info.Parse);
        }
        
        private void UpdateTradebackG12()
        {
            if (pkm.Format == 1)
            {
                if (!Legal.AllowGen1Tradeback)
                {
                    pkm.TradebackStatus = TradebackType.Gen1_NotTradeback;
                    ((PK1)pkm).CatchRateIsItem = false;
                    return;
                }

                // Detect tradeback status by comparing the catch rate(Gen1)/held item(Gen2) to the species in the pkm's evolution chain.
                var catch_rate = ((PK1)pkm).Catch_Rate;

                // For species catch rate, discard any species that has no valid encounters and a different catch rate than their pre-evolutions
                var Lineage = Legal.GetLineage(pkm).Where(s => !Legal.Species_NotAvailable_CatchRate.Contains(s)).ToList();
                // Dragonite's Catch Rate is different than Dragonair's in Yellow, but there is no Dragonite encounter.
                var RGBCatchRate = Lineage.Any(s => catch_rate == PersonalTable.RB[s].CatchRate);
                var YCatchRate = Lineage.Any(s => s != 149 && catch_rate == PersonalTable.Y[s].CatchRate);
                // Krabby encounter trade special catch rate
                var TradeCatchRate = ((pkm.Species == 098 || pkm.Species == 099) && catch_rate == 204);
                var StadiumCatchRate = Legal.Stadium_GiftSpecies.Contains(pkm.Species) && Legal.Stadium_CatchRate.Contains(catch_rate);
                bool matchAny = RGBCatchRate || YCatchRate || TradeCatchRate || StadiumCatchRate;

                // If the catch rate value has been modified, the item has either been removed or swapped in Generation 2.
                var HeldItemCatchRate = catch_rate == 0 || Legal.HeldItems_GSC.Any(h => h == catch_rate);
                if (HeldItemCatchRate && !matchAny)
                    pkm.TradebackStatus = TradebackType.WasTradeback;
                else if (!HeldItemCatchRate && matchAny)
                    pkm.TradebackStatus = TradebackType.Gen1_NotTradeback;
                else
                    pkm.TradebackStatus = TradebackType.Any;


                // Update the editing settings for the PKM to acknowledge the tradeback status if the species is changed.
                ((PK1)pkm).CatchRateIsItem = !pkm.Gen1_NotTradeback && HeldItemCatchRate && !matchAny;
            }
            else if (pkm.Format == 2 || pkm.VC2)
            {
                // Eggs, pokemon with non-empty crystal met location, and generation 2 species without generation 1 preevolutions can't be traded to generation 1.
                if (pkm.IsEgg || pkm.HasOriginalMetLocation || (pkm.Species > Legal.MaxSpeciesID_1 && !Legal.FutureEvolutionsGen1.Contains(pkm.Species)))
                    pkm.TradebackStatus = TradebackType.Gen2_NotTradeback;
                else
                    pkm.TradebackStatus = TradebackType.Any;
            }
            else if (pkm.VC1)
            {
                // If VC2 is ever released, we can assume it will be TradebackType.Any.
                // Met date cannot be used definitively as the player can change their system clock.
                pkm.TradebackStatus = TradebackType.Gen1_NotTradeback;
            }
            else
            {
                pkm.TradebackStatus = TradebackType.Any;
            }
        }
        private void UpdateTypeInfo()
        {
            if (pkm.Format >= 7)
            {
                if (pkm.VC1)
                    Info.EncounterMatch = EncounterGenerator.GetRBYStaticTransfer(pkm.Species);
                else if (pkm.VC2)
                    Info.EncounterMatch = EncounterGenerator.GetGSStaticTransfer(pkm.Species);
            }

            if (pkm.GenNumber <= 2 && pkm.TradebackStatus == TradebackType.Any && (EncounterMatch as GBEncounterData)?.Generation != pkm.GenNumber)
                // Example: GSC Pokemon with only possible encounters in RBY, like the legendary birds
                pkm.TradebackStatus = TradebackType.WasTradeback;

            MatchedType = Type = (EncounterOriginalGB ?? EncounterMatch)?.GetType();
            var bt = Type.GetTypeInfo().BaseType;
            if (bt != null && !(bt == typeof(Array) || bt == typeof(object) || bt.GetTypeInfo().IsPrimitive)) // a parent exists
                Type = bt; // use base type
        }
        private void UpdateChecks()
        {
            VerifyECPID();
            VerifyNickname();
            VerifyOT();
            VerifyIVs();
            VerifyEVs();
            VerifyLevel();
            VerifyRibbons();
            VerifyAbility();
            VerifyBall();
            VerifyForm();
            VerifyMisc();
            VerifyGender();
            VerifyItem();
            if (pkm.Format >= 4)
                VerifyEncounterType();
            if (pkm.Format >= 6)
            {
                History = VerifyHistory();
                AddLine(History);
                VerifyOTMemory();
                VerifyHTMemory();
                VerifyHyperTraining();
                VerifyMedals();
                VerifyRegion();
                VerifyVersionEvolution();
            }

            // SecondaryChecked = true;
        }
        private string GetLegalityReport()
        {
            if (!Parsed || pkm == null)
                return V189;
            
            var lines = new List<string>();
            var vMoves = Info.Moves;
            var vRelearn = Info.Relearn;
            for (int i = 0; i < 4; i++)
                if (!vMoves[i].Valid)
                    lines.Add(string.Format(V191, vMoves[i].Judgement.Description(), i + 1, vMoves[i].Comment));

            if (pkm.Format >= 6)
            for (int i = 0; i < 4; i++)
                if (!vRelearn[i].Valid)
                    lines.Add(string.Format(V192, vRelearn[i].Judgement.Description(), i + 1, vRelearn[i].Comment));

            if (lines.Count == 0 && Parse.All(chk => chk.Valid) && Valid)
                return V193;
            
            // Build result string...
            var outputLines = Parse.Where(chk => !chk.Valid); // Only invalid
            lines.AddRange(outputLines.Select(chk => string.Format(V196, chk.Judgement.Description(), chk.Comment)));

            if (lines.Count == 0)
                return V190;

            return string.Join(Environment.NewLine, lines);
        }
        private string GetVerboseLegalityReport()
        {
            if (!Parsed)
                return V189;

            const string separator = "===";
            string[] br = {separator, ""};
            var lines = new List<string> {br[1]};
            lines.AddRange(br);
            int rl = lines.Count;

            var vMoves = Info.Moves;
            var vRelearn = Info.Relearn;
            for (int i = 0; i < 4; i++)
                if (vMoves[i].Valid)
                    lines.Add(string.Format(V191, vMoves[i].Judgement.Description(), i + 1, vMoves[i].Comment));

            if (pkm.Format >= 6)
            for (int i = 0; i < 4; i++)
                if (vRelearn[i].Valid)
                    lines.Add(string.Format(V192, vRelearn[i].Judgement.Description(), i + 1, vRelearn[i].Comment));

            if (rl != lines.Count) // move info added, break for next section
                lines.Add(br[1]);
            
            var outputLines = Parse.Where(chk => chk != null && chk.Valid && chk.Comment != V).OrderBy(chk => chk.Judgement); // Fishy sorted to top
            lines.AddRange(outputLines.Select(chk => string.Format(V196, chk.Judgement.Description(), chk.Comment)));

            lines.AddRange(br);
            lines.Add(string.Format(V195, EncounterName));
            var pidiv = Info.PIDIV ?? MethodFinder.Analyze(pkm);
            if (pidiv != null)
            {
                if (!pidiv.NoSeed)
                    lines.Add(string.Format(V248, pidiv.OriginSeed.ToString("X8")));
                lines.Add(string.Format(V249, pidiv.Type));
            }
            
            return GetLegalityReport() + string.Join(Environment.NewLine, lines);
        }

        public int[] GetSuggestedRelearn()
        {
            if (Info.RelearnBase == null || pkm.GenNumber < 6 || !pkm.IsOriginValid)
                return new int[4];

            if (!pkm.WasEgg)
                return Info.RelearnBase;

            List<int> window = new List<int>(Info.RelearnBase);
            var vMoves = Info.Moves;
            window.AddRange(pkm.Moves.Where((v, i) => !vMoves[i].Valid || vMoves[i].Flag));
            window = window.Distinct().ToList();
            if (window.Count < 4)
                window.AddRange(new int[4 - window.Count]);
            return window.Skip(window.Count - 4).ToArray();
        }
        public int[] GetSuggestedMoves(bool tm, bool tutor, bool reminder)
        {
            if (pkm == null || !pkm.IsOriginValid)
                return null;
            if (!Parsed)
                return new int[4];
            return Legal.GetValidMoves(pkm, Info.EvoChainsAllGens, Tutor: tutor, Machine: tm, MoveReminder: reminder).Skip(1).ToArray(); // skip move 0
        }

        public EncounterStatic GetSuggestedMetInfo()
        {
            if (pkm == null)
                return null;

            int loc = GetSuggestedTransferLocation(pkm);
            if (pkm.WasEgg)
            {
                int lvl = 1; // gen5+
                if (!pkm.IsNative)
                    lvl = pkm.CurrentLevel; // be generous with transfer conditions
                else if (pkm.Format < 5) // and native
                    lvl = 0;
                return new EncounterStatic
                {
                    Species = Legal.GetBaseSpecies(pkm),
                    Location = loc != -1 ? loc : GetSuggestedEggMetLocation(pkm),
                    Level = lvl,
                };
            }

            var area = EncounterGenerator.GetCaptureLocation(pkm);
            if (area != null)
            {
                var slots = area.Slots.OrderBy(s => s.LevelMin);
                return new EncounterStatic
                {
                    Species = slots.First().Species,
                    Location = loc != -1 ? loc : area.Location,
                    Level = slots.First().LevelMin,
                };
            }

            var encounter = EncounterGenerator.GetStaticLocation(pkm);
            if (loc != -1 && encounter != null)
                encounter.Location = loc;
            return encounter;
        }
        private static int GetSuggestedEggMetLocation(PKM pkm)
        {
            // Return one of legal hatch locations for game
            switch ((GameVersion)pkm.Version)
            {
                case GameVersion.R:
                case GameVersion.S:
                case GameVersion.E:
                case GameVersion.FR:
                case GameVersion.LG:
                    switch (pkm.Format)
                    {
                        case 3:
                            return pkm.FRLG ? 146 /* Four Island */ : 32; // Route 117
                        case 4:
                            return 0x37; // Pal Park
                        default:
                            return 30001; // Transporter
                    }

                case GameVersion.D:
                case GameVersion.P:
                case GameVersion.Pt:
                    return pkm.Format > 4 ? 30001 /* Transporter */ : 4; // Solaceon Town
                case GameVersion.HG:
                case GameVersion.SS:
                    return pkm.Format > 4 ? 30001 /* Transporter */ : 182; // Route 34

                case GameVersion.B:
                case GameVersion.W:
                    return 16; // Route 3

                case GameVersion.X:
                case GameVersion.Y:
                    return 38; // Route 7
                case GameVersion.AS:
                case GameVersion.OR:
                    return 318; // Battle Resort

                case GameVersion.SN:
                case GameVersion.MN:
                    return 50; // Route 4
            }
            return -1;
        }
        private static int GetSuggestedTransferLocation(PKM pkm)
        {
            // Return one of legal hatch locations for game
            if (pkm.HasOriginalMetLocation)
                return -1;
            if (pkm.VC1)
                return 30013;
            if (pkm.Format == 4) // Pal Park
                return 0x37;
            if (pkm.Format == 5) // Transporter
                return 30001;
            return -1;
        }
    }
}

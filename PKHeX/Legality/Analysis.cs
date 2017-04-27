using System;
using System.Collections.Generic;
using System.Linq;
using static PKHeX.Core.LegalityCheckStrings;

namespace PKHeX.Core
{
    public partial class LegalityAnalysis
    {
        private PKM pkm;
        private DexLevel[][] EvoChainsAllGens;
        private readonly List<CheckResult> Parse = new List<CheckResult>();

        private List<GBEncounterData> EncountersGBMatch;
        private object EncounterOriginalGB;
        private object EncounterMatch;
        private int EncounterSpecies;
        private Type Type; // Parent class when applicable (EncounterStatic / MysteryGift)
        private Type MatchedType; // Child class if applicable (WC6, PGF, etc)
        private string EncounterName => Legal.getEncounterTypeName(pkm, EncounterOriginalGB ?? EncounterMatch);
        private List<MysteryGift> EventGiftMatch;
        private List<EncounterStatic> EncounterStaticMatch;
        private CheckResult Encounter, History;
        private int[] RelearnBase;
        // private bool SecondaryChecked;

        public readonly bool Parsed;
        public readonly bool Valid;
        public readonly bool Error;
        public bool ParsedValid => Parsed && Valid;
        public bool ParsedInvalid => Parsed && !Valid;
        public CheckResult[] vMoves = new CheckResult[4];
        public CheckResult[] vRelearn = new CheckResult[4];
        public string Report(bool verbose = false) => verbose ? getVerboseLegalityReport() : getLegalityReport();
        private IEnumerable<int> AllSuggestedMoves
        {
            get
            {
                if (Error)
                    return new int[4];
                if (_allSuggestedMoves == null)
                    return _allSuggestedMoves = !pkm.IsOriginValid ? new int[4] : getSuggestedMoves(true, true, true);
                return _allSuggestedMoves;
            }
        }
        private IEnumerable<int> AllSuggestedRelearnMoves
        {
            get
            {
                if (Error)
                    return new int[4];
                if (_allSuggestedRelearnMoves == null)
                    return _allSuggestedRelearnMoves = !pkm.IsOriginValid ? new int[4] : Legal.getValidRelearn(pkm, -1).ToArray();
                return _allSuggestedRelearnMoves;
            }
        }
        private int[] _allSuggestedMoves, _allSuggestedRelearnMoves;
        public int[] AllSuggestedMovesAndRelearn => AllSuggestedMoves.Concat(AllSuggestedRelearnMoves).ToArray();

        public LegalityAnalysis(PKM pk)
        {
            for (int i = 0; i < 4; i++) 
            {
                vMoves[i] = new CheckResult(CheckIdentifier.Move);
                vRelearn[i] = new CheckResult(CheckIdentifier.RelearnMove);
            }

            try
            {
                switch (pk.Format) // prior to storing GameVersion
                {
                    case 1: parsePK1(pk); break;
                    case 2: parsePK1(pk); break;
                }

                if (!Parse.Any())
                switch (pk.GenNumber)
                {
                    case 3: if (pk.Version != 15) parsePK3(pk); break;
                    case 4: parsePK4(pk); break;
                    case 5: parsePK5(pk); break;
                    case 6: parsePK6(pk); break;

                    case 1: parsePK7(pk); break;
                    case 7: parsePK7(pk); break;
                }

                Valid = Parsed = Parse.Any();
                if (Parsed)
                {
                    if (Parse.Any(chk => !chk.Valid))
                        Valid = false;
                    else if (vMoves.Any(m => m.Valid != true))
                        Valid = false;
                    else if (vRelearn.Any(m => m.Valid != true))
                        Valid = false;

                    if (pkm.FatefulEncounter && vRelearn.Any(chk => !chk.Valid) && EncounterMatch == null)
                        AddLine(Severity.Indeterminate, V188, CheckIdentifier.Fateful);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Valid = false;
                Parsed = true;
                AddLine(Severity.Invalid, V190, CheckIdentifier.Misc);
                Error = true;
            }
        }

        private void AddLine(Severity s, string c, CheckIdentifier i)
        {
            AddLine(new CheckResult(s, c, i));
        }
        private void AddLine(CheckResult chk)
        {
            Parse.Add(chk);
        }

        private void parsePK1(PKM pk)
        {
            pkm = pk;
            if (!pkm.IsOriginValid)
            { AddLine(Severity.Invalid, V187, CheckIdentifier.None); return; }
            updateTradebackG12();
            updateEncounterChain();
            updateMoveLegality();
            updateTypeInfo();
            verifyNickname();
            verifyDVs();
            verifyG1OT();
            verifyMiscG1();
        }
        private void parsePK3(PKM pk)
        {
            pkm = pk;
            if (!pkm.IsOriginValid)
            { AddLine(Severity.Invalid, V187, CheckIdentifier.None); return; }
            
            updateEncounterChain();
            updateMoveLegality();
            updateTypeInfo();
            updateChecks();
        }
        private void parsePK4(PKM pk)
        {
            pkm = pk;
            if (!pkm.IsOriginValid)
            { AddLine(Severity.Invalid, V187, CheckIdentifier.None); return; }

            verifyPreRelearn();
            updateEncounterChain();
            updateMoveLegality();
            updateTypeInfo();
            updateChecks();
        }
        private void parsePK5(PKM pk)
        {
            pkm = pk;
            if (!pkm.IsOriginValid)
            { AddLine(Severity.Invalid, V187, CheckIdentifier.None); return; }

            verifyPreRelearn();
            updateEncounterChain();
            updateMoveLegality();
            updateTypeInfo();
            updateChecks();
        }
        private void parsePK6(PKM pk)
        {
            pkm = pk;
            if (!pkm.IsOriginValid)
            { AddLine(Severity.Invalid, V187, CheckIdentifier.None); return; }

            updateRelearnLegality();
            updateEncounterChain();
            updateMoveLegality();
            updateTypeInfo();
            updateChecks();
        }
        private void parsePK7(PKM pk)
        {
            pkm = pk;
            if (!pkm.IsOriginValid)
            { AddLine(Severity.Invalid, V187, CheckIdentifier.None); return; }

            updateRelearnLegality();
            updateEncounterChain();
            updateMoveLegality();
            updateTypeInfo();
            updateChecks();
        }

        private void updateRelearnLegality()
        {
            try { vRelearn = verifyRelearn(); }
            catch { for (int i = 0; i < 4; i++) vRelearn[i] = new CheckResult(Severity.Invalid, V190, CheckIdentifier.RelearnMove); }
            // SecondaryChecked = false;
        }
        private void updateMoveLegality()
        {
            try { vMoves = verifyMoves(); }
            catch { for (int i = 0; i < 4; i++) vMoves[i] = new CheckResult(Severity.Invalid, V190, CheckIdentifier.Move); }
            // SecondaryChecked = false;
        }

        private void updateEncounterChain()
        {
            if (EventGiftMatch?.Count > 1) // Multiple possible Mystery Gifts matched
                EncounterMatch = EventGiftMatch.First(); // temporarily set one so that Encounter can be verified

            Encounter = verifyEncounter();
            Parse.Add(Encounter);
            EvoChainsAllGens = Legal.getEvolutionChainsAllGens(pkm, EncounterOriginalGB ?? EncounterMatch);
        }
        private void updateTradebackG12()
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
                var Lineage = Legal.getLineage(pkm).Where(s => !Legal.Species_NotAvailable_CatchRate.Contains(s)).ToList();
                // Dragonite's Catch Rate is different than Dragonair's in Yellow, but there is no Dragonite encounter.
                var RGBCatchRate = Lineage.Any(s => catch_rate == PersonalTable.RB[s].CatchRate);
                var YCatchRate = Lineage.Any(s => s != 149 && catch_rate == PersonalTable.Y[s].CatchRate);

                bool matchAny = RGBCatchRate || YCatchRate;

                // If the catch rate value has been modified, the item has either been removed or swapped in Generation 2.
                var HeldItemCatchRate = catch_rate == 0 || Legal.HeldItems_GSC.Any(h => h == catch_rate);
                if (HeldItemCatchRate && !matchAny)
                    pkm.TradebackStatus = TradebackType.WasTradeback;
                else if (!HeldItemCatchRate && matchAny)
                    pkm.TradebackStatus = TradebackType.Gen1_NotTradeback;
                else
                    pkm.TradebackStatus = TradebackType.Any;


                // Update the editing settings for the PKM to acknowledge the tradeback status if the species is changed.
                ((PK1)pkm).CatchRateIsItem = !pkm.Gen1_NotTradeback && HeldItemCatchRate && matchAny;
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
        private void updateTypeInfo()
        {
            if (pkm.VC && pkm.Format == 7)
                EncounterMatch = Legal.getRBYStaticTransfer(pkm.Species);

            MatchedType = Type = (EncounterOriginalGB ?? EncounterMatch ?? pkm.Species)?.GetType();
            var bt = Type.BaseType;
            if (!(bt == typeof(Array) || bt == typeof(object) || bt.IsPrimitive)) // a parent exists
                Type = bt; // use base type
        }
        private void updateChecks()
        {
            verifyECPID();
            verifyNickname();
            verifyOT();
            verifyIVs();
            verifyEVs();
            verifyLevel();
            verifyRibbons();
            verifyAbility();
            verifyBall();
            verifyForm();
            verifyMisc();
            verifyGender();
            verifyItem();
            if (pkm.Format >= 4)
                verifyEncounterType();
            if (pkm.Format >= 6)
            {
                History = verifyHistory();
                AddLine(History);
                verifyOTMemory();
                verifyHTMemory();
                verifyHyperTraining();
                verifyMedals();
                verifyRegion();
                verifyVersionEvolution();
            }

            // SecondaryChecked = true;
        }
        private string getLegalityReport()
        {
            if (!Parsed)
                return V189;
            
            string r = "";
            for (int i = 0; i < 4; i++)
                if (!vMoves[i].Valid)
                    r += string.Format(V191, getString(vMoves[i].Judgement), i + 1, vMoves[i].Comment) + Environment.NewLine;

            if (pkm.Format >= 6)
            for (int i = 0; i < 4; i++)
                if (!vRelearn[i].Valid)
                    r += string.Format(V192, getString(vRelearn[i].Judgement), i + 1, vRelearn[i].Comment) + Environment.NewLine;

            if (r.Length == 0 && Parse.All(chk => chk.Valid) && Valid)
                return V193;
            
            // Build result string...
            var outputLines = Parse.Where(chk => !chk.Valid); // Only invalid
            r += string.Join(Environment.NewLine, outputLines.Select(chk => string.Format(V196, getString(chk.Judgement), chk.Comment)));

            if (r.Length == 0)
                r = V190;

            return r.TrimEnd();
        }
        private string getVerboseLegalityReport()
        {
            string r = getLegalityReport() + Environment.NewLine;
            if (pkm == null)
                return r;
            r += "===" + Environment.NewLine + Environment.NewLine;
            int rl = r.Length;

            for (int i = 0; i < 4; i++)
                if (vMoves[i].Valid)
                    r += string.Format(V191, getString(vMoves[i].Judgement), i + 1, vMoves[i].Comment) + Environment.NewLine;

            if (pkm.Format >= 6)
            for (int i = 0; i < 4; i++)
                if (vRelearn[i].Valid)
                    r += string.Format(V192, getString(vRelearn[i].Judgement), i + 1, vRelearn[i].Comment) + Environment.NewLine;

            if (rl != r.Length) // move info added, break for next section
                r += Environment.NewLine;
            
            var outputLines = Parse.Where(chk => chk != null && chk.Valid && chk.Comment != V).OrderBy(chk => chk.Judgement); // Fishy sorted to top
            r += string.Join(Environment.NewLine, outputLines.Select(chk => string.Format(V196, getString(chk.Judgement), chk.Comment)));

            r += Environment.NewLine;
            r += "===" + Environment.NewLine + Environment.NewLine;
            r += string.Format(V195, EncounterName);

            return r.TrimEnd();
        }

        public int[] getSuggestedRelearn()
        {
            if (RelearnBase == null || pkm.GenNumber < 6 || !pkm.IsOriginValid)
                return new int[4];

            if (!pkm.WasEgg)
                return RelearnBase;

            List<int> window = new List<int>(RelearnBase);
            window.AddRange(pkm.Moves.Where((v, i) => !vMoves[i].Valid || vMoves[i].Flag));
            window = window.Distinct().ToList();
            if (window.Count < 4)
                window.AddRange(new int[4 - window.Count]);
            return window.Skip(window.Count - 4).ToArray();
        }
        public int[] getSuggestedMoves(bool tm, bool tutor, bool reminder)
        {
            if (pkm == null || !pkm.IsOriginValid)
                return null;
            if (!Parsed)
                return new int[4];
            return Legal.getValidMoves(pkm, EvoChainsAllGens, Tutor: tutor, Machine: tm, MoveReminder: reminder).Skip(1).ToArray(); // skip move 0
        }

        public EncounterStatic getSuggestedMetInfo()
        {
            if (pkm == null)
                return null;

            int loc = getSuggestedTransferLocation(pkm);
            if (pkm.WasEgg)
            {
                int lvl = 1; // gen5+
                if (!pkm.IsNative)
                    lvl = pkm.CurrentLevel; // be generous with transfer conditions
                else if (pkm.Format < 5) // and native
                    lvl = 0;
                return new EncounterStatic
                {
                    Species = Legal.getBaseSpecies(pkm),
                    Location = loc != -1 ? loc : getSuggestedEggMetLocation(pkm),
                    Level = lvl,
                };
            }

            var area = Legal.getCaptureLocation(pkm);
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

            var encounter = Legal.getStaticLocation(pkm);
            if (loc != -1 && encounter != null)
                encounter.Location = loc;
            return encounter;
        }
        private static int getSuggestedEggMetLocation(PKM pkm)
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
        private static int getSuggestedTransferLocation(PKM pkm)
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
        private static int[] getGenMovesCheckOrder(PKM pkm)
        {
            if (pkm.Format == 1)
                return new [] { 1, 2 };
            if (pkm.Format == 2)
                return new [] { 2, 1 };
            if (pkm.Format == 7 && pkm.VC1)
                return new [] { 7, 1 };
            if (pkm.Format == 7 && pkm.VC2)
                return new [] { 7, 2, 1 };

            var order = new int[pkm.Format - pkm.GenNumber + 1];
            for (int i = 0; i < order.Length; i++)
                order[i] = pkm.Format - i;
            return order;
        }
    }
}

#define SUPPRESS

using System;
using System.Collections.Generic;
using System.Linq;
using static PKHeX.Core.LegalityCheckStrings;

namespace PKHeX.Core
{
    /// <summary>
    /// Legality Check object containing the <see cref="CheckResult"/> data and overview values from the parse.
    /// </summary>
    public partial class LegalityAnalysis
    {
        internal readonly PKM pkm;
        internal readonly PersonalInfo PersonalInfo;
        private readonly bool Error;
        private readonly List<CheckResult> Parse = new List<CheckResult>();

        /// <summary>
        /// Parse result list allowing view of the legality parse.
        /// </summary>
        public IReadOnlyList<CheckResult> Results => Parse;

        private IEncounterable EncounterOriginalGB;

        /// <summary>
        /// Matched encounter data for the <see cref="pkm"/>.
        /// </summary>
        public IEncounterable EncounterMatch => Info.EncounterMatch;

        /// <summary>
        /// Original encounter data for the <see cref="pkm"/>.
        /// </summary>
        /// <remarks>
        /// Generation 1/2 <see cref="pkm"/> that are transferred forward to Generation 7 are restricted to new encounter details.
        /// By retaining their original match, more information can be provided by the parse.
        /// </remarks>
        public IEncounterable EncounterOriginal => EncounterOriginalGB ?? EncounterMatch;

        /// <summary>
        /// Indicates if all checks ran to completion.
        /// </summary>
        /// <remarks>This value is false if any checks encountered an error.</remarks>
        public readonly bool Parsed;

        /// <summary>
        /// Indicates if all checks returned a <see cref="Severity.Valid"/> result.
        /// </summary>
        public readonly bool Valid;

        /// <summary>
        /// Contains various data reused for multiple checks.
        /// </summary>
        public LegalInfo Info { get; private set; }

        /// <summary>
        /// Creates a report message with optional verbosity for in-depth analysis.
        /// </summary>
        /// <param name="verbose">Include all details in the parse, including valid check messages.</param>
        /// <returns>Single line string</returns>
        public string Report(bool verbose = false) => verbose ? GetVerboseLegalityReport() : GetLegalityReport();

        private IEnumerable<int> AllSuggestedMoves
        {
            get
            {
                if (_allSuggestedMoves != null)
                    return _allSuggestedMoves;
                if (Error || Info == null)
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
                if (Error || Info == null)
                    return new int[4];
                return _allSuggestedRelearnMoves = Legal.GetValidRelearn(pkm, Info.EncounterMatch.Species, (GameVersion)pkm.Version).ToArray();
            }
        }

        private int[] _allSuggestedMoves, _allSuggestedRelearnMoves;
        public int[] AllSuggestedMovesAndRelearn => AllSuggestedMoves.Concat(AllSuggestedRelearnMoves).ToArray();

        private string EncounterName
        {
            get
            {
                var enc = EncounterOriginal;
                return $"{enc.GetEncounterTypeName()} ({SpeciesStrings[enc.Species]})";
            }
        }

        private string EncounterLocation
        {
            get
            {
                var enc = EncounterOriginal as ILocation;
                return enc?.GetEncounterLocation(Info.Generation, pkm.Version);
            }
        }

        /// <summary>
        /// Checks the input <see cref="PKM"/> data for legality.
        /// </summary>
        /// <param name="pk">Input data to check</param>
        /// <param name="table"><see cref="SaveFile"/> specific personal data</param>
        public LegalityAnalysis(PKM pk, PersonalTable table = null)
        {
            pkm = pk;
#if SUPPRESS
            try
#endif
            {
                PersonalInfo = table?.GetFormeEntry(pkm.Species, pkm.AltForm) ?? pkm.PersonalInfo;
                ParseLegality();

                if (Parse.Count <= 0)
                    return;

                Valid = Parse.All(chk => chk.Valid)
                    && Info.Moves.All(m => m.Valid)
                    && Info.Relearn.All(m => m.Valid);

                if (!Valid && pkm.FatefulEncounter && Info.Relearn.Any(chk => !chk.Valid) && EncounterMatch is EncounterInvalid)
                    AddLine(Severity.Indeterminate, LFatefulGiftMissing, CheckIdentifier.Fateful);
            }
#if SUPPRESS
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message);
                Valid = false;
                AddLine(Severity.Invalid, L_AError, CheckIdentifier.Misc);
                Error = true;
            }
#endif
            Parsed = true;
        }

        private void ParseLegality()
        {
            if (!pkm.IsOriginValid)
                AddLine(Severity.Invalid, LEncConditionBadSpecies, CheckIdentifier.GameOrigin);

            if (pkm.Format <= 2) // prior to storing GameVersion
            {
                ParsePK1();
                return;
            }
            switch (pkm.GenNumber)
            {
                case 3: ParsePK3(); return;
                case 4: ParsePK4(); return;
                case 5: ParsePK5(); return;
                case 6: ParsePK6(); return;

                case 1: case 2:
                case 7: ParsePK7(); return;
            }
        }

        private void ParsePK1()
        {
            pkm.TradebackStatus = GBRestrictions.GetTradebackStatusInitial(pkm);
            UpdateInfo();
            if (pkm.TradebackStatus == TradebackType.Any && Info.Generation != pkm.Format)
                pkm.TradebackStatus = TradebackType.WasTradeback; // Example: GSC Pokemon with only possible encounters in RBY, like the legendary birds

            Nickname.Verify(this);
            Level.Verify(this);
            Level.VerifyG1(this);
            Trainer.VerifyOTG1(this);
            Misc.VerifyMiscG1(this);
            if (pkm.Format == 2)
                Item.Verify(this);
        }

        private void ParsePK3()
        {
            UpdateInfo();
            UpdateChecks();
            if (pkm.Format > 3)
                Transfer.VerifyTransferLegalityG3(this);

            if (pkm.Version == (int)GameVersion.CXD)
                CXD.Verify(this);

            if (Info.EncounterMatch is WC3 z && z.NotDistributed)
                AddLine(Severity.Invalid, LEncUnreleased, CheckIdentifier.Encounter);
        }

        private void ParsePK4()
        {
            UpdateInfo();
            UpdateChecks();
            if (pkm.Format > 4)
                Transfer.VerifyTransferLegalityG4(this);
        }

        private void ParsePK5()
        {
            UpdateInfo();
            UpdateChecks();
            NHarmonia.Verify(this);
        }

        private void ParsePK6()
        {
            UpdateInfo();
            UpdateChecks();
        }

        private void ParsePK7()
        {
            UpdateInfo();
            if (pkm.VC)
                UpdateVCTransferInfo();
            UpdateChecks();
        }

        /// <summary>
        /// Adds a new Check parse value.
        /// </summary>
        /// <param name="s">Check severity</param>
        /// <param name="c">Check comment</param>
        /// <param name="i">Check type</param>
        internal void AddLine(Severity s, string c, CheckIdentifier i) => AddLine(new CheckResult(s, c, i));

        /// <summary>
        /// Adds a new Check parse value.
        /// </summary>
        /// <param name="chk">Check result to add.</param>
        internal void AddLine(CheckResult chk) => Parse.Add(chk);

        private void UpdateVCTransferInfo()
        {
            EncounterOriginalGB = EncounterMatch;
            if (EncounterOriginalGB is EncounterInvalid)
                return;
            Info.EncounterMatch = EncounterStaticGenerator.GetVCStaticTransferEncounter(pkm);
            if (!(Info.EncounterMatch is EncounterStatic s) || !EncounterStaticGenerator.IsVCStaticTransferEncounterValid(pkm, s))
            { AddLine(Severity.Invalid, LEncInvalid, CheckIdentifier.Encounter); return; }

            foreach (var z in Transfer.VerifyVCEncounter(pkm, EncounterOriginalGB, s, Info.Moves))
                AddLine(z);

            Transfer.VerifyTransferLegalityG12(this);
        }

        private void UpdateInfo()
        {
            Info = EncounterFinder.FindVerifiedEncounter(pkm);
            Parse.AddRange(Info.Parse);
        }

        private void UpdateChecks()
        {
            PIDEC.Verify(this);
            Nickname.Verify(this);
            Language.Verify(this);
            Trainer.Verify(this);
            IndividualValues.Verify(this);
            EffortValues.Verify(this);
            Level.Verify(this);
            Ribbon.Verify(this);
            Ability.Verify(this);
            Ball.Verify(this);
            Form.Verify(this);
            Misc.Verify(this);
            Gender.Verify(this);
            Item.Verify(this);
            if (pkm.Format <= 6 && pkm.Format >= 4)
                EncounterType.Verify(this); // Gen 6->7 transfer deletes encounter type data

            if (pkm.Format < 6)
                return;

            if (pkm.Format < 8 && !(pkm is PB7))
            {
                Memory.Verify(this);
                Medal.Verify(this);
                ConsoleRegion.Verify(this);
            }

            if (pkm.Format >= 7)
            {
                HyperTraining.Verify(this);
                Misc.VerifyVersionEvolution(this);
            }
        }

        private string GetLegalityReport()
        {
            if (Valid)
                return L_ALegal;
            if (!Parsed || Info == null)
                return L_AnalysisUnavailable;

            var lines = new List<string>();
            var vMoves = Info.Moves;
            var vRelearn = Info.Relearn;
            for (int i = 0; i < 4; i++)
            {
                if (!vMoves[i].Valid)
                    lines.Add(vMoves[i].Format(L_F0_M_1_2, i + 1));
            }

            if (pkm.Format >= 6)
            {
                for (int i = 0; i < 4; i++)
                {
                    if (!vRelearn[i].Valid)
                        lines.Add(vRelearn[i].Format(L_F0_RM_1_2, i + 1));
                }
            }

            // Build result string...
            var outputLines = Parse.Where(chk => !chk.Valid);
            lines.AddRange(outputLines.Select(chk => chk.Format(L_F0_1)));

            return string.Join(Environment.NewLine, lines);
        }

        private string GetVerboseLegalityReport()
        {
            if (!Parsed || Info == null)
                return L_AnalysisUnavailable;

            const string separator = "===";
            string[] br = {separator, string.Empty};
            var lines = new List<string> {br[1]};
            lines.AddRange(br);
            int rl = lines.Count;

            var vMoves = Info.Moves;
            var vRelearn = Info.Relearn;
            for (int i = 0; i < 4; i++)
            {
                var move = vMoves[i];
                if (!move.Valid)
                    continue;
                var msg = move.Format(L_F0_M_1_2, i + 1);
                if (pkm.Format != move.Generation)
                    msg += $" [Gen{move.Generation}]";
                lines.Add(msg);
            }

            if (pkm.Format >= 6)
            {
                for (int i = 0; i < 4; i++)
                {
                    if (vRelearn[i].Valid)
                    lines.Add(vRelearn[i].Format(L_F0_RM_1_2, i + 1));
                }
            }

            if (rl != lines.Count) // move info added, break for next section
                lines.Add(br[1]);

            var outputLines = Parse.Where(chk => chk?.Valid == true && chk.Comment != L_AValid).OrderBy(chk => chk.Judgement); // Fishy sorted to top
            lines.AddRange(outputLines.Select(chk => chk.Format(L_F0_1)));

            lines.AddRange(br);
            lines.Add(string.Format(L_FEncounterType_0, EncounterName));
            var loc = EncounterLocation;
            if (!string.IsNullOrEmpty(loc))
                lines.Add(string.Format(L_F0_1, "Location", loc));
            if (pkm.VC)
                lines.Add(string.Format(L_F0_1, nameof(GameVersion), Info.Game));
            var pidiv = Info.PIDIV ?? MethodFinder.Analyze(pkm);
            if (pidiv != null)
            {
                if (!pidiv.NoSeed)
                    lines.Add(string.Format(L_FOriginSeed_0, pidiv.OriginSeed.ToString("X8")));
                lines.Add(string.Format(L_FPIDType_0, pidiv.Type));
            }
            if (!Valid && Info.InvalidMatches != null)
            {
                lines.Add("Other match(es):");
                lines.AddRange(Info.InvalidMatches.Select(z => $"{z.LongName}: {z.Reason}"));
            }

            return GetLegalityReport() + string.Join(Environment.NewLine, lines);
        }

        /// <summary>
        /// Gets the current <see cref="PKM.RelearnMoves"/> array of four moves that might be legal.
        /// </summary>
        public int[] GetSuggestedRelearn()
        {
            if (Info?.RelearnBase == null || Info.Generation < 6)
                return new int[4];

            if (!EncounterMatch.EggEncounter)
                return Info.RelearnBase;

            List<int> window = new List<int>(Info.RelearnBase.Where(z => z != 0));
            window.AddRange(pkm.Moves.Where((_, i) => !Info.Moves[i].Valid || Info.Moves[i].Flag));
            window = window.Distinct().ToList();
            int[] moves = new int[4];
            int start = Math.Max(0, window.Count - 4);
            int count = Math.Min(4, window.Count);
            window.CopyTo(start, moves, 0, count);
            return moves;
        }

        /// <summary>
        /// Gets four moves which can be learned depending on the input arguments.
        /// </summary>
        /// <param name="tm">Allow TM moves</param>
        /// <param name="tutor">Allow Tutor moves</param>
        /// <param name="reminder">Allow Move Reminder</param>
        public int[] GetSuggestedMoves(bool tm, bool tutor, bool reminder)
        {
            if (!Parsed)
                return new int[4];
            if (pkm.IsEgg && pkm.Format <= 5) // pre relearn
                return Legal.GetBaseEggMoves(pkm, pkm.Species, (GameVersion)pkm.Version, pkm.CurrentLevel);
            if (!(tm || tutor || reminder) && (Info.Generation <= 2 || pkm.Species == EncounterOriginal.Species))
            {
                var lvl = Info.Generation <= 2 && pkm.Format >= 7 ? pkm.Met_Level : pkm.CurrentLevel;
                var ver = Info.Generation <= 2 && EncounterOriginal is IVersion v ? v.Version : (GameVersion)pkm.Version;
                return MoveLevelUp.GetEncounterMoves(pkm, lvl, ver);
            }
            var evos = Info.EvoChainsAllGens;
            return Legal.GetValidMoves(pkm, evos, Tutor: tutor, Machine: tm, MoveReminder: reminder).Skip(1).ToArray(); // skip move 0
        }

        /// <summary>
        /// Gets an object containing met data properties that might be legal.
        /// </summary>
        public EncounterStatic GetSuggestedMetInfo() => EncounterSuggestion.GetSuggestedMetInfo(pkm);
    }
}

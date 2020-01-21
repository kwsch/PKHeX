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
        private readonly List<CheckResult> Parse = new List<CheckResult>();

        /// <summary>
        /// Parse result list allowing view of the legality parse.
        /// </summary>
        public IReadOnlyList<CheckResult> Results => Parse;

        /// <summary>
        /// Only use this when trying to mutate the legality. Not for use when checking legality.
        /// </summary>
        public void ResetParse() => Parse.Clear();

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
        public IEncounterable EncounterOriginal => Info.EncounterOriginal;

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
        public readonly LegalInfo Info;

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
                if (!Parsed)
                    return new int[4];
                return _allSuggestedMoves ??= GetSuggestedMoves(true, true, true);
            }
        }

        private IEnumerable<int> AllSuggestedRelearnMoves
        {
            get
            {
                if (!Parsed)
                    return new int[4];
                return _allSuggestedRelearnMoves ??= Legal.GetValidRelearn(pkm, Info.EncounterMatch.Species, Info.EncounterMatch.Form, (GameVersion)pkm.Version).ToArray();
            }
        }

        private int[]? _allSuggestedMoves, _allSuggestedRelearnMoves;
        public int[] AllSuggestedMovesAndRelearn() => AllSuggestedMoves.Concat(AllSuggestedRelearnMoves).ToArray();

        private string EncounterName
        {
            get
            {
                var enc = EncounterOriginal;
                return $"{enc.LongName} ({SpeciesStrings[enc.Species]})";
            }
        }

        private string? EncounterLocation
        {
            get
            {
                var enc = EncounterOriginal as ILocation;
                return enc?.GetEncounterLocation(Info.Generation, pkm.Version);
            }
        }

        /// <summary>
        /// Checks the input <see cref="PKM"/> data for legality. This is the best method for checking with context, as some games do not have all Alternate Form data available.
        /// </summary>
        /// <param name="pk">Input data to check</param>
        /// <param name="table"><see cref="SaveFile"/> specific personal data</param>
        public LegalityAnalysis(PKM pk, PersonalTable table) : this(pk, table.GetFormeEntry(pk.Species, pk.AltForm)) { }

        /// <summary>
        /// Checks the input <see cref="PKM"/> data for legality.
        /// </summary>
        /// <param name="pk">Input data to check</param>
        public LegalityAnalysis(PKM pk) : this(pk, pk.PersonalInfo) { }

        /// <summary>
        /// Checks the input <see cref="PKM"/> data for legality.
        /// </summary>
        /// <param name="pk">Input data to check</param>
        /// <param name="pi">Personal info to parse with</param>
        public LegalityAnalysis(PKM pk, PersonalInfo pi)
        {
            pkm = pk;
            PersonalInfo = pi;

            if (pkm.Format <= 2) // prior to storing GameVersion
                pkm.TradebackStatus = GBRestrictions.GetTradebackStatusInitial(pkm);

#if SUPPRESS
            try
#endif
            {
                Info = EncounterFinder.FindVerifiedEncounter(pkm);
                if (!pkm.IsOriginValid)
                    AddLine(Severity.Invalid, LEncConditionBadSpecies, CheckIdentifier.GameOrigin);
                GetParseMethod()();

                if (Parse.Count == 0) // shouldn't ever happen as at least one is yielded above.
                {
                    AddLine(Severity.Invalid, L_AError, CheckIdentifier.Misc);
                    return;
                }

                Valid = Parse.All(chk => chk.Valid)
                    && Info.Moves.All(m => m.Valid)
                    && Info.Relearn.All(m => m.Valid);

                if (!Valid && pkm.FatefulEncounter && Info.Relearn.Any(chk => !chk.Valid) && EncounterMatch is EncounterInvalid)
                    AddLine(Severity.Indeterminate, LFatefulGiftMissing, CheckIdentifier.Fateful);
                Parsed = true;
            }
#if SUPPRESS
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message);
                Info = new LegalInfo(pkm);
                Valid = false;
                AddLine(Severity.Invalid, L_AError, CheckIdentifier.Misc);
            }
#endif
        }

        private Action GetParseMethod()
        {
            if (pkm.Format <= 2) // prior to storing GameVersion
                return ParsePK1;

            int gen = pkm.GenNumber;
            if (gen <= 0)
                gen = pkm.Format;
            return gen switch
            {
                3 => ParsePK3,
                4 => ParsePK4,
                5 => ParsePK5,
                6 => ParsePK6,

                1 => ParsePK7,
                2 => ParsePK7,
                7 => ParsePK7,

                8 => ParsePK8,

                _ => throw new Exception()
            };
        }

        private void ParsePK1()
        {
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

            if (pkm.Format >= 8)
                Transfer.VerifyTransferLegalityG8(this);
        }

        private void ParsePK4()
        {
            UpdateInfo();
            UpdateChecks();
            if (pkm.Format > 4)
                Transfer.VerifyTransferLegalityG4(this);
            if (pkm.Format >= 8)
                Transfer.VerifyTransferLegalityG8(this);
        }

        private void ParsePK5()
        {
            UpdateInfo();
            UpdateChecks();
            NHarmonia.Verify(this);
            if (pkm.Format >= 8)
                Transfer.VerifyTransferLegalityG8(this);
        }

        private void ParsePK6()
        {
            UpdateInfo();
            UpdateChecks();
            if (pkm.Format >= 8)
                Transfer.VerifyTransferLegalityG8(this);
        }

        private void ParsePK7()
        {
            UpdateInfo();
            if (pkm.VC)
                UpdateVCTransferInfo();
            UpdateChecks();
            if (pkm.Format >= 8)
                Transfer.VerifyTransferLegalityG8(this);
        }

        private void ParsePK8()
        {
            UpdateInfo();
            UpdateChecks();
            Transfer.VerifyTransferLegalityG8(this);
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
            var enc = (Info.EncounterOriginalGB = EncounterMatch);
            if (enc is EncounterInvalid)
                return;
            Info.EncounterMatch = EncounterStaticGenerator.GetVCStaticTransferEncounter(pkm);
            if (!(Info.EncounterMatch is EncounterStatic s) || !EncounterStaticGenerator.IsVCStaticTransferEncounterValid(pkm, s))
            { AddLine(Severity.Invalid, LEncInvalid, CheckIdentifier.Encounter); return; }

            foreach (var z in Transfer.VerifyVCEncounter(pkm, enc, s, Info.Moves))
                AddLine(z);

            Transfer.VerifyTransferLegalityG12(this);
        }

        private void UpdateInfo()
        {
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

            ConsoleRegion.Verify(this);
            History.Verify(this);
            Memory.Verify(this);
            if (pkm is ISuperTrain)
                Medal.Verify(this);

            if (pkm.Format < 7)
                return;

            HyperTraining.Verify(this);
            Misc.VerifyVersionEvolution(this);
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

            if (!Info.PIDParsed)
                Info.PIDIV = MethodFinder.Analyze(pkm);

            var pidiv = Info.PIDIV;
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
        public IReadOnlyList<int> GetSuggestedRelearn()
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
                return Legal.GetBaseEggMoves(pkm, pkm.Species, 0, (GameVersion)pkm.Version, pkm.CurrentLevel);

            if (!tm && !tutor && !reminder)
            {
                // try to give current moves
                if (Info.Generation <= 2)
                {
                    var lvl = pkm.Format >= 7 ? pkm.Met_Level : pkm.CurrentLevel;
                    var ver = EncounterOriginal is IVersion v ? v.Version : (GameVersion)pkm.Version;
                    return MoveLevelUp.GetEncounterMoves(EncounterOriginal.Species, 0, lvl, ver);
                }
                if (pkm.Species == EncounterOriginal.Species)
                {
                    return MoveLevelUp.GetEncounterMoves(pkm.Species, pkm.AltForm, pkm.CurrentLevel, (GameVersion)pkm.Version);
                }
            }
            var evos = Info.EvoChainsAllGens;
            return Legal.GetValidMoves(pkm, evos, Tutor: tutor, Machine: tm, MoveReminder: reminder).Skip(1).ToArray(); // skip move 0
        }
    }
}

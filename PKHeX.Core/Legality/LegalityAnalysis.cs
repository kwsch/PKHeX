#define SUPPRESS

using System;
using System.Collections.Generic;
using System.Linq;
using static PKHeX.Core.LegalityAnalyzers;
using static PKHeX.Core.LegalityCheckStrings;
using static PKHeX.Core.ParseSettings;

namespace PKHeX.Core
{
    /// <summary>
    /// Legality Check object containing the <see cref="CheckResult"/> data and overview values from the parse.
    /// </summary>
    public sealed class LegalityAnalysis
    {
        internal readonly PKM pkm;
        internal readonly PersonalInfo PersonalInfo;
        private readonly List<CheckResult> Parse = new();

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

        private string EncounterName
        {
            get
            {
                var enc = EncounterOriginal;
                var str = SpeciesStrings;
                var name = (uint) enc.Species < str.Count ? str[enc.Species] : enc.Species.ToString();
                return $"{enc.LongName} ({name})";
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
        public LegalityAnalysis(PKM pk, PersonalTable table) : this(pk, table.GetFormEntry(pk.Species, pk.Form)) { }

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
            // We want to swallow any error from malformed input data from the user. The Valid state is all that we really need.
#pragma warning disable CA1031 // Do not catch general exception types
            catch (Exception e)
#pragma warning restore CA1031 // Do not catch general exception types
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

            int gen = pkm.Generation;
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
            MiscValues.VerifyMiscG1(this);
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

            if (Info.EncounterMatch is WC3 {NotDistributed: true})
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
            var vc = EncounterStaticGenerator.GetVCStaticTransferEncounter(pkm, enc);
            Info.EncounterMatch = vc;

            foreach (var z in Transfer.VerifyVCEncounter(pkm, enc, vc, Info.Moves))
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
            LanguageIndex.Verify(this);
            Trainer.Verify(this);
            IndividualValues.Verify(this);
            EffortValues.Verify(this);
            Level.Verify(this);
            Ribbon.Verify(this);
            AbilityValues.Verify(this);
            BallIndex.Verify(this);
            FormValues.Verify(this);
            MiscValues.Verify(this);
            GenderValues.Verify(this);
            Item.Verify(this);
            Contest.Verify(this);

            var format = pkm.Format;
            if (format is 4 or 5)
                Gen4EncounterType.Verify(this); // Gen 6->7 transfer deletes encounter type data

            if (format < 6)
                return;

            History.Verify(this);
            if (format < 8)
                ConsoleRegion.Verify(this); // Gen 7->8 transfer deletes geolocation tracking data

            if (pkm is ITrainerMemories)
                Memory.Verify(this);
            if (pkm is ISuperTrain)
                Medal.Verify(this);

            if (format < 7)
                return;

            HyperTraining.Verify(this);
            MiscValues.VerifyVersionEvolution(this);

            if (format < 8)
                return;

            Mark.Verify(this);
        }

        private string GetLegalityReport()
        {
            if (Valid)
                return L_ALegal;
            if (!Parsed)
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
            if (!Parsed)
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
            if (Info.Generation <= 2)
                lines.Add(string.Format(L_F0_1, nameof(GameVersion), Info.Game));

            if (!Info.PIDParsed)
                Info.PIDIV = MethodFinder.Analyze(pkm);

            var pidiv = Info.PIDIV;
            {
                if (!pidiv.NoSeed)
                    lines.Add(string.Format(L_FOriginSeed_0, pidiv.OriginSeed.ToString("X8")));
                lines.Add(string.Format(L_FPIDType_0, pidiv.Type));
            }
            if (!Valid && Info.InvalidMatches != null)
            {
                lines.Add("Other match(es):");
                lines.AddRange(Info.InvalidMatches.Select(z => $"{z.Encounter.LongName}: {z.Reason}"));
            }

            return GetLegalityReport() + string.Join(Environment.NewLine, lines);
        }
    }
}

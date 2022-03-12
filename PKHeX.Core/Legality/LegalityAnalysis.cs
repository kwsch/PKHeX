#define SUPPRESS

using System;
using System.Collections.Generic;
using static PKHeX.Core.LegalityAnalyzers;
using static PKHeX.Core.LegalityCheckStrings;

namespace PKHeX.Core
{
    /// <summary>
    /// Legality Check object containing the <see cref="CheckResult"/> data and overview values from the parse.
    /// </summary>
    public sealed class LegalityAnalysis
    {
        /// <summary> The entity we are checking. </summary>
        internal readonly PKM pkm;

        /// <summary> The entity's <see cref="PersonalInfo"/>, which may have been sourced from the Save File it resides on. </summary>
        /// <remarks>We store this rather than re-fetching, as some games that use the same <see cref="PKM"/> format have different values.</remarks>
        internal readonly PersonalInfo PersonalInfo;

        private readonly List<CheckResult> Parse = new(8);

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

        public readonly SlotOrigin SlotOrigin;

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
        /// Checks the input <see cref="PKM"/> data for legality. This is the best method for checking with context, as some games do not have all Alternate Form data available.
        /// </summary>
        /// <param name="pk">Input data to check</param>
        /// <param name="table"><see cref="SaveFile"/> specific personal data</param>
        /// <param name="source">Details about where the <see cref="pk"/> originated from.</param>
        public LegalityAnalysis(PKM pk, PersonalTable table, SlotOrigin source = SlotOrigin.Party) : this(pk, table.GetFormEntry(pk.Species, pk.Form), source) { }

        /// <summary>
        /// Checks the input <see cref="PKM"/> data for legality.
        /// </summary>
        /// <param name="pk">Input data to check</param>
        /// <param name="source">Details about where the <see cref="pk"/> originated from.</param>
        public LegalityAnalysis(PKM pk, SlotOrigin source = SlotOrigin.Party) : this(pk, pk.PersonalInfo, source) { }

        /// <summary>
        /// Checks the input <see cref="PKM"/> data for legality.
        /// </summary>
        /// <param name="pk">Input data to check</param>
        /// <param name="pi">Personal info to parse with</param>
        /// <param name="source">Details about where the <see cref="pk"/> originated from.</param>
        public LegalityAnalysis(PKM pk, PersonalInfo pi, SlotOrigin source = SlotOrigin.Party)
        {
            pkm = pk;
            PersonalInfo = pi;
            SlotOrigin = source;

            Info = new LegalInfo(pkm, Parse);
#if SUPPRESS
            try
#endif
            {
                EncounterFinder.FindVerifiedEncounter(pkm, Info);
                if (!pkm.IsOriginValid)
                    AddLine(Severity.Invalid, LEncConditionBadSpecies, CheckIdentifier.GameOrigin);
                GetParseMethod()();

                Valid = Parse.TrueForAll(chk => chk.Valid)
                    && Array.TrueForAll(Info.Moves, m => m.Valid)
                    && Array.TrueForAll(Info.Relearn, m => m.Valid);

                if (!Valid && IsPotentiallyMysteryGift(Info, pkm))
                    AddLine(Severity.Indeterminate, LFatefulGiftMissing, CheckIdentifier.Fateful);
                Parsed = true;
            }
#if SUPPRESS
            // We want to swallow any error from malformed input data from the user. The Valid state is all that we really need.
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message);
                Valid = false;

                var moves = Info.Moves;
                // Moves and Relearn arrays can potentially be empty on error.
                // ReSharper disable once ConstantNullCoalescingCondition
                for (int i = 0; i < moves.Length; i++)
                    moves[i] ??= new CheckMoveResult(MoveSource.None, pkm.Format, Severity.Indeterminate, L_AError, CheckIdentifier.CurrentMove);

                var relearn = Info.Relearn;
                // ReSharper disable once ConstantNullCoalescingCondition
                for (int i = 0; i < relearn.Length; i++)
                    relearn[i] ??= new CheckMoveResult(MoveSource.None, 0, Severity.Indeterminate, L_AError, CheckIdentifier.RelearnMove);

                AddLine(Severity.Invalid, L_AError, CheckIdentifier.Misc);
            }
#endif
        }

        private static bool IsPotentiallyMysteryGift(LegalInfo info, PKM pk)
        {
            if (info.EncounterOriginal is not EncounterInvalid enc)
                return false;
            if (enc.Generation <= 3)
                return true;
            if (!pk.FatefulEncounter)
                return false;
            if (enc.Generation < 6)
                return true;
            if (Array.TrueForAll(info.Relearn, chk => !chk.Valid))
                return true;
            return false;
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

                _ => throw new Exception(),
            };
        }

        private void ParsePK1()
        {
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
            UpdateChecks();
            if (pkm.Format > 4)
                Transfer.VerifyTransferLegalityG4(this);
            if (pkm.Format >= 8)
                Transfer.VerifyTransferLegalityG8(this);
        }

        private void ParsePK5()
        {
            UpdateChecks();
            NHarmonia.Verify(this);
            if (pkm.Format >= 8)
                Transfer.VerifyTransferLegalityG8(this);
        }

        private void ParsePK6()
        {
            UpdateChecks();
            if (pkm.Format >= 8)
                Transfer.VerifyTransferLegalityG8(this);
        }

        private void ParsePK7()
        {
            if (pkm.VC)
                UpdateVCTransferInfo();
            UpdateChecks();
            if (pkm.Format >= 8)
                Transfer.VerifyTransferLegalityG8(this);
        }

        private void ParsePK8()
        {
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
            var vc = EncounterStaticGenerator.GetVCStaticTransferEncounter(pkm, enc, Info.EvoChainsAllGens[7]);
            Info.EncounterMatch = vc;

            foreach (var z in Transfer.VerifyVCEncounter(pkm, enc, vc, Info.Moves))
                AddLine(z);

            Transfer.VerifyTransferLegalityG12(this);
        }

        private void UpdateChecks()
        {
            PIDEC.Verify(this);
            Nickname.Verify(this);
            LanguageIndex.Verify(this);
            Trainer.Verify(this);
            TrainerID.Verify(this);
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
            if (format is 4 or 5 or 6) // Gen 6->7 transfer removes this property.
                Gen4GroundTile.Verify(this);

            if (format < 6)
                return;

            History.Verify(this);
            if (format < 8) // Gen 7->8 transfer removes these properties.
                ConsoleRegion.Verify(this);

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
            Arceus.Verify(this);
        }
    }
}

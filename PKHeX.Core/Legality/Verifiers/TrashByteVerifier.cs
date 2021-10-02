using System;
using static PKHeX.Core.TrashBytes16;

namespace PKHeX.Core
{
    public sealed class TrashByteVerifier : Verifier
    {
        protected override CheckIdentifier Identifier => CheckIdentifier.Trash;

        public override void Verify(LegalityAnalysis data)
        {
            var pkm = data.pkm;
            // Don't bother with previous generation formats yet.
            if (pkm.Format < 6)
                return;

            VerifyFinalTerminator(data, pkm);
            if (pkm.IsEgg)
            {
                VerifyTrashIsEgg(data, pkm);
                return;
            }

            var enc = data.EncounterOriginal;
            VerifyTrashNickname(data, pkm, enc);
            VerifyTrashOT(data, pkm, enc);
            VerifyTrashHT(data, pkm);
        }

        /// <summary>
        /// Starting in generation 6, \0 is used as a terminator, and all trash byte sections are consistently clean. Flag anything that has nonzero values in them.
        /// </summary>
        private void VerifyFinalTerminator(LegalityAnalysis data, PKM pkm)
        {
            if (!HasFinalTerminator(pkm.Nickname_Trash))
                data.AddLine(GetInvalid($"{nameof(PKM.Nickname_Trash)} detected at reserved terminator offset."));
            if (!HasFinalTerminator(pkm.OT_Trash))
                data.AddLine(GetInvalid($"{nameof(PKM.OT_Trash)} detected at reserved terminator offset."));
            if (!HasFinalTerminator(pkm.HT_Trash))
                data.AddLine(GetInvalid($"{nameof(PKM.HT_Trash)} detected at reserved terminator offset."));
        }

        private void VerifyTrashNickname(LegalityAnalysis data, PKM pkm, IEncounterTemplate enc)
        {
            var trash = pkm.Nickname_Trash;
            if (!HasTrash(trash))
                return;

            // Nicknames can set trash up to the max for the language.
            var firstTrash = FindTerminator(trash) + 2;
            var lastTrash = FindLastTrash(trash, firstTrash);
            var expectedEnd = Legal.GetMaxLengthNickname(enc.Generation, (LanguageID)pkm.Language);
            var lastTrashCharIndex = (lastTrash / 2);
            bool hasExtraTrash = expectedEnd < lastTrashCharIndex;

            // Some scenarios can set trash beyond.
            var allowed = !hasExtraTrash || IsExtraTrashValid(pkm, trash, enc, firstTrash);
            var severity = !allowed ? Severity.Invalid : Severity.Fishy;
            data.AddLine(Get($"{nameof(PKM.Nickname_Trash)} detected.", severity));
        }

        private void VerifyTrashHT(LegalityAnalysis data, PKM pkm)
        {
            var trash = pkm.HT_Trash;
            if (HasTrash(trash))
                data.AddLine(Get($"{nameof(PKM.HT_Trash)} detected.", Severity.Fishy));
        }

        private void VerifyTrashOT(LegalityAnalysis data, PKM pkm, IEncounterTemplate enc)
        {
            // Some encounters are first created with a fixed OT name, then when captured, the Trainer name is applied over top.
            if (enc is EncounterStatic8U { ShouldHaveScientistTrash: true })
            {
                if (EncounterStatic8U.HasScientistTrash(pkm) == false)
                    data.AddLine(GetInvalid($"{nameof(PKM.OT_Trash)} does not match expected trash."));
                return;
            }

            var trash = pkm.OT_Trash;
            bool hasTrash = HasTrash(trash);
            if (!hasTrash)
                return;

            // Traded eggs can have trash from the original OT name.
            if (enc.EggEncounter && pkm.WasTradedEgg)
                data.AddLine(Get($"{nameof(PKM.OT_Trash)} detected.", Severity.Fishy));
            else
                data.AddLine(GetInvalid($"{nameof(PKM.OT_Trash)} detected."));
        }

        /// <summary>
        /// Eggs are created first by creating a Box Pokémon, then put into an egg with the localized Egg name.
        /// The Nickname trash must have the species name beneath the egg name.
        /// </summary>
        private void VerifyTrashIsEgg(LegalityAnalysis data, PKM pkm)
        {
            if (HasTrash(pkm.Nickname_Trash))
            {
                // Eggs have the species name underneath the current egg name.
                var over = pkm.Nickname;
                var under = SpeciesName.GetSpeciesNameGeneration(pkm.Species, pkm.Language, data.Info.Generation);
                if (!HasUnderlayer(pkm.Nickname_Trash, under, over))
                    data.AddLine(GetInvalid($"{nameof(PKM.Nickname_Trash)} does not match expected trash."));
            }

            if (HasTrash(pkm.OT_Trash))
                data.AddLine(GetInvalid($"{nameof(PKM.OT_Trash)} detected."));
            if (!ArrayUtil.IsRangeEmpty(pkm.HT_Trash))
                data.AddLine(GetInvalid($"{nameof(PKM.HT_Trash)} detected."));
        }

        private static bool IsExtraTrashValid(PKM pkm, ReadOnlySpan<byte> trash, IEncounterTemplate enc, int firstTrash)
        {
            if (!AllowExtraTrash(pkm, enc))
                return false;

            return IsExtraTrashValid(trash, enc, firstTrash);
        }

        private static bool IsExtraTrashValid(ReadOnlySpan<byte> trash, IEncounterTemplate enc, int firstTrash)
        {
            // check all languages for original species
            var species = enc.Species;
            var generation = enc.Generation;
            return HasUnderlayerAnySpecies(trash, firstTrash, species, generation);
        }

        private static bool AllowExtraTrash(PKM pkm, IEncounterTemplate enc)
        {
            if (enc.EggEncounter && pkm.WasTradedEgg)
                return true;
            if (enc is EncounterStatic8N or EncounterStatic8ND or EncounterStatic8NC && pkm.Met_Location == Encounters8Nest.SharedNest)
                return true;
            return false;
        }
    }
}

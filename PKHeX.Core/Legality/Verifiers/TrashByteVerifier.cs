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

            // As of generation 8, cannot nickname something from another language.
            // Nicknames can have trash up to the max for the language.
            var firstTrash = FindTerminator(trash) + 2;
            var lastTrash = FindLastTrash(trash, firstTrash);
            var expectedEnd = Legal.GetMaxLengthNickname(enc.Generation, (LanguageID)pkm.Language);
            var lastTrashCharIndex = (lastTrash / 2);

            var severity = Severity.Fishy;
            if (lastTrashCharIndex > expectedEnd)
            {
                // Some scenarios can set trash beyond, where the encounter is from someone else.
                // Find the uppermost trash beginnings that is within the mutable region.
                var extraTrash = FindNextTrashBackwards(trash, (expectedEnd * 2) + 2);
                if (IsExtraTrashValid(pkm, trash, enc, extraTrash))
                    severity = extraTrash / 2 == expectedEnd + 1 ? Severity.Valid : Severity.Fishy; // multiple nicknames
                else
                    severity = Severity.Invalid;
            }
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
            // Traded eggs inherit the language of the hatching OT, so the encounter species could be from any language.
            if (enc.EggEncounter && pkm.WasTradedEgg)
                return HasUnderlayerAnySpecies(trash, firstTrash, enc.Species, enc.Generation);

            // Shared raids use the language ID of the host, so the encounter species could be from any language.
            if (enc is EncounterStatic8N or EncounterStatic8ND or EncounterStatic8NC && pkm.Met_Location == Encounters8Nest.SharedNest)
                return HasUnderlayerAnySpecies(trash, firstTrash, enc.Species, enc.Generation);

            // Force nicknamed events apply the species name of the redeeming language, then slap on the forced Nickname.
            if (enc is WC8 w && w.GetIsNicknamed(pkm.Language))
            {
                if (!pkm.IsNicknamed)
                    return false;
                var nick = SpeciesName.GetSpeciesNameGeneration(enc.Species, pkm.Language, enc.Generation);
                if (nick != pkm.Nickname) // shouldn't be flagged
                    return false;
                return HasUnderlayerAnySpecies(trash, firstTrash, enc.Species, enc.Generation);
            }
            return false;
        }
    }
}

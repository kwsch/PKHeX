using System;
using static PKHeX.Core.TrashBytes;

namespace PKHeX.Core
{
    public sealed class TrashByteVerifier : Verifier
    {
        protected override CheckIdentifier Identifier => CheckIdentifier.Trash;

        public override void Verify(LegalityAnalysis data)
        {
            var pkm = data.pkm;
            if (pkm.Format < 6)
                return;

            // Data in this format has immutable trash bytes.
            // Flag anything that has nonzero values in them.
            if (!HasFinalTerminator(pkm.Nickname_Trash))
                data.AddLine(GetInvalid($"{nameof(PKM.Nickname_Trash)} detected in reserved terminator."));
            if (!HasFinalTerminator(pkm.OT_Trash))
                data.AddLine(GetInvalid($"{nameof(PKM.OT_Trash)} detected in reserved terminator."));
            if (!HasFinalTerminator(pkm.HT_Trash))
                data.AddLine(GetInvalid($"{nameof(PKM.HT_Trash)} detected in reserved terminator."));

            if (pkm.IsEgg)
            {
                if (HasTrash2(pkm.Nickname_Trash))
                {
                    // Eggs have the species name underneath the current egg name.
                    var over = pkm.Nickname;
                    var under = SpeciesName.GetSpeciesNameGeneration(pkm.Species, pkm.Language, data.EncounterOriginal.Generation);
                    if (!HasUnderlayer(pkm.Nickname_Trash, under, over))
                        data.AddLine(GetInvalid($"{nameof(PKM.Nickname_Trash)} does not match expected trash."));
                }

                if (!ArrayUtil.IsRangeEmpty(pkm.HT_Trash))
                    data.AddLine(GetInvalid($"{nameof(PKM.HT_Trash)} detected."));
                if (HasTrash2(pkm.OT_Trash))
                    data.AddLine(GetInvalid($"{nameof(PKM.OT_Trash)} detected."));
            }
            else
            {
                var enc = data.EncounterOriginal;
                if (enc is EncounterStatic8U { ShouldHaveScientistTrash: true})
                {
                    if (EncounterStatic8U.HasScientistTrash(pkm) == false)
                        data.AddLine(GetInvalid($"{nameof(PKM.OT_Trash)} does not match expected trash."));
                }
                else if (enc.EggEncounter && pkm.WasTradedEgg)
                {
                    // Traded eggs can have trash from the original OT name.
                    if (HasTrash2(pkm.OT_Trash))
                        data.AddLine(Get($"{nameof(PKM.OT_Trash)} detected.", Severity.Fishy));
                }
                else if (HasTrash2(pkm.OT_Trash))
                {
                    data.AddLine(GetInvalid($"{nameof(PKM.OT_Trash)} detected."));
                }

                if (HasTrash2(pkm.HT_Trash))
                    data.AddLine(Get($"{nameof(PKM.HT_Trash)} detected.", Severity.Fishy));
                if (HasTrash2(pkm.Nickname_Trash))
                {
                    var trash = pkm.Nickname_Trash;
                    var firstTrash = FindTerminator2(trash) + 2;
                    var lastTrash = FindLastTrash2(trash, firstTrash);
                    var expectedEnd = Legal.GetMaxLengthNickname(enc.Generation, (LanguageID) pkm.Language);
                    var lastTrashCharIndex = (lastTrash / 2);
                    bool hasExtraTrash = expectedEnd < lastTrashCharIndex;
                    var allowed = !hasExtraTrash || IsExtraTrashValid(pkm, trash, enc, firstTrash);
                    var severity = !allowed ? Severity.Invalid : Severity.Fishy;
                    data.AddLine(Get($"{nameof(PKM.Nickname_Trash)} detected.", severity));
                }
            }
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

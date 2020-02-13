using System;
using System.Linq;

namespace PKHeX.Core
{
    /// <summary>
    /// Verifies the Friendship, Affection, and other miscellaneous stats that can be present for OT/HT data.
    /// </summary>
    public sealed class HistoryVerifier : Verifier
    {
        protected override CheckIdentifier Identifier => CheckIdentifier.Memory;

        public override void Verify(LegalityAnalysis data)
        {
            bool neverOT = !GetCanOTHandle(data.Info.EncounterMatch, data.pkm, data.Info.Generation);
            VerifyHandlerState(data, neverOT);
            VerifyTradeState(data);
            VerifyOTMisc(data, neverOT);
            VerifyHTMisc(data);
        }

        private void VerifyTradeState(LegalityAnalysis data)
        {
            var pkm = data.pkm;
            var Info = data.Info;

            if (data.pkm is IGeoTrack t)
                VerifyGeoLocationData(data, t, data.pkm);

            if (pkm.VC && pkm is PK7 g && g.Geo1_Country == 0) // VC transfers set Geo1 Country
                data.AddLine(GetInvalid(LegalityCheckStrings.LGeoMemoryMissing));
            if (pkm is IContestStats s && s.HasContestStats() && !CanHaveContestStats(pkm, Info.Generation))
                data.AddLine(GetInvalid(LegalityCheckStrings.LContestZero));

            if (!pkm.IsUntraded)
            {
                if (pkm.IsEgg) // Can't have HT details even as a Link Trade egg
                    data.AddLine(GetInvalid(LegalityCheckStrings.LMemoryArgBadHT));
                return;
            }

            if (pkm.CurrentHandler != 0) // Badly edited; PKHeX doesn't trip this.
                data.AddLine(GetInvalid(LegalityCheckStrings.LMemoryHTFlagInvalid));
            else if (pkm.HT_Friendship != 0)
                data.AddLine(GetInvalid(LegalityCheckStrings.LMemoryStatFriendshipHT0));
            else if (pkm.HT_Affection != 0)
                data.AddLine(GetInvalid(LegalityCheckStrings.LMemoryStatAffectionHT0));

            // Don't check trade evolutions if Untraded. The Evolution Chain already checks for trade evolutions.
        }

        /// <summary>
        /// Checks if the <see cref="PKM.CurrentHandler"/> state is set correctly.
        /// </summary>
        private void VerifyHandlerState(LegalityAnalysis data, bool neverOT)
        {
            var pkm = data.pkm;
            var Info = data.Info;

            // HT Flag
            if ((Info.Generation != pkm.Format || neverOT) && pkm.CurrentHandler != 1)
                data.AddLine(GetInvalid(LegalityCheckStrings.LTransferHTFlagRequired));
        }

        /// <summary>
        /// Checks the non-Memory data for the <see cref="PKM.OT_Name"/> details.
        /// </summary>
        private void VerifyOTMisc(LegalityAnalysis data, bool neverOT)
        {
            var pkm = data.pkm;
            var Info = data.Info;

            VerifyOTAffection(data, neverOT, Info.Generation, pkm);
            VerifyOTFriendship(data, neverOT, Info.Generation, pkm);
        }

        private void VerifyOTFriendship(LegalityAnalysis data, bool neverOT, int origin, PKM pkm)
        {
            if (origin <= 2)
            {
                // Verify the original friendship value since it cannot change from the value it was assigned in the original generation.
                // Since some evolutions have different base friendship values, check all possible evolutions for a match.
                // If none match, then it is not a valid OT friendship.
                const int vc = 7; // VC transfers use SM personal info
                var evos = data.Info.EvoChainsAllGens[vc];
                var fs = pkm.OT_Friendship;
                if (evos.All(z => GetBaseFriendship(vc, z.Species) != fs))
                    data.AddLine(GetInvalid(LegalityCheckStrings.LMemoryStatFriendshipOTBaseEvent));
            }
            else if (neverOT)
            {
                // Verify the original friendship value since it cannot change from the value it was assigned in the original generation.
                // If none match, then it is not a valid OT friendship.
                var fs = pkm.OT_Friendship;
                if (GetBaseFriendship(origin, data.Info.EncounterMatch.Species) != fs)
                    data.AddLine(GetInvalid(LegalityCheckStrings.LMemoryStatFriendshipOTBaseEvent));
            }
        }

        private void VerifyOTAffection(LegalityAnalysis data, bool neverOT, int origin, PKM pkm)
        {
            if (origin < 6)
            {
                // Can gain affection in Gen6 via the Contest glitch applying affection to OT rather than HT.
                // VC encounters cannot obtain OT affection since they can't visit Gen6.
                if ((origin <= 2 && pkm.OT_Affection != 0) || IsInvalidContestAffection(pkm))
                    data.AddLine(GetInvalid(LegalityCheckStrings.LMemoryStatAffectionOT0));
            }
            else if (origin == 8)
            {
                if (pkm.OT_Affection != 0)
                    data.AddLine(GetInvalid(LegalityCheckStrings.LMemoryStatAffectionOT0));
            }
            else if (neverOT)
            {
                if (origin == 6)
                {
                    if (pkm.IsUntraded && pkm.XY)
                    {
                        if (pkm.OT_Affection != 0)
                            data.AddLine(GetInvalid(LegalityCheckStrings.LMemoryStatAffectionOT0));
                    }
                    else if (IsInvalidContestAffection(pkm))
                    {
                        data.AddLine(GetInvalid(LegalityCheckStrings.LMemoryStatAffectionOT0));
                    }
                }
                else
                {
                    if (pkm.OT_Affection != 0)
                        data.AddLine(GetInvalid(LegalityCheckStrings.LMemoryStatAffectionOT0));
                }
            }
        }

        /// <summary>
        /// Checks the non-Memory data for the <see cref="PKM.HT_Name"/> details.
        /// </summary>
        private void VerifyHTMisc(LegalityAnalysis data)
        {
            var pkm = data.pkm;
            var htGender = pkm.HT_Gender;
            if (htGender > 1 || (pkm.IsUntraded && htGender != 0))
                data.AddLine(GetInvalid(string.Format(LegalityCheckStrings.LMemoryHTGender, htGender)));

            if (pkm is IHandlerLanguage h)
                VerifyHTLanguage(data, h, pkm);
        }

        private void VerifyHTLanguage(LegalityAnalysis data, IHandlerLanguage h, PKM pkm)
        {
            if (h.HT_Language == 0)
            {
                if (!string.IsNullOrWhiteSpace(pkm.HT_Name))
                    data.AddLine(GetInvalid(LegalityCheckStrings.LMemoryHTLanguage));
                return;
            }

            if (string.IsNullOrWhiteSpace(pkm.HT_Name))
                data.AddLine(GetInvalid(LegalityCheckStrings.LMemoryHTLanguage));
            else if (h.HT_Language > (int)LanguageID.ChineseT)
                data.AddLine(GetInvalid(LegalityCheckStrings.LMemoryHTLanguage));
        }

        private void VerifyGeoLocationData(LegalityAnalysis data, IGeoTrack t, PKM pkm)
        {
            var valid = t.GetValidity();
            if (valid == GeoValid.CountryAfterPreviousEmpty)
                data.AddLine(GetInvalid(LegalityCheckStrings.LGeoBadOrder));
            if (valid == GeoValid.RegionWithoutCountry)
                data.AddLine(GetInvalid(LegalityCheckStrings.LGeoNoRegion));
            if (t.Geo1_Country != 0 && pkm.IsUntraded) // traded
                data.AddLine(GetInvalid(LegalityCheckStrings.LGeoNoCountryHT));
        }

        // ORAS contests mistakenly apply 20 affection to the OT instead of the current handler's value
        private static bool IsInvalidContestAffection(PKM pkm) => pkm.OT_Affection != 255 && pkm.OT_Affection % 20 != 0;

        public static bool GetCanOTHandle(IEncounterable enc, PKM pkm, int gen)
        {
            if (gen < 6)
                return true;
            return enc switch
            {
                EncounterTrade _ => false,
                WC6 wc6 when wc6.OT_Name.Length > 0 => false,
                WC7 wc7 when wc7.OT_Name.Length > 0 && wc7.TID != 18075 => false, // Ash Pikachu QR Gift doesn't set Current Handler
                WC8 wc8 when wc8.GetHasOT(pkm.Language) => false,
                _ => true
            };
        }

        private static int GetBaseFriendship(int gen, int species)
        {
            return gen switch
            {
                1 => PersonalTable.USUM[species].BaseFriendship,
                2 => PersonalTable.USUM[species].BaseFriendship,

                6 => PersonalTable.AO[species].BaseFriendship,
                7 => PersonalTable.USUM[species].BaseFriendship,
                8 => PersonalTable.SWSH[species].BaseFriendship,
                _ => throw new IndexOutOfRangeException(),
            };
        }

        private static bool CanHaveContestStats(PKM pkm, int origin)
        {
            return origin switch
            {
                1 => false,
                2 => false,
                3 => true,
                4 => true,
                5 => (pkm.Format >= 6), // ORAS Contests
                6 => (!pkm.IsUntraded || pkm.AO),
                7 => false,
                _ => false
            };
        }
    }
}
using System;
using System.Linq;

namespace PKHeX.Core
{
    /// <summary>
    /// Contains extension logic for modifying <see cref="PKM"/> data.
    /// </summary>
    public static class CommonEdits
    {
        /// <summary>
        /// Setting which enables/disables automatic manipulation of <see cref="PKM.Markings"/> when importing from a <see cref="ShowdownSet"/>.
        /// </summary>
        public static bool ShowdownSetIVMarkings { get; set; } = true;

        /// <summary>
        /// Setting which causes the <see cref="PKM.StatNature"/> to the <see cref="PKM.Nature"/> in Gen8+ formats.
        /// </summary>
        public static bool ShowdownSetBehaviorNature { get; set; } = false;

        /// <summary>
        /// Sets the <see cref="PKM.Nickname"/> to the provided value.
        /// </summary>
        /// <param name="pk">Pokémon to modify.</param>
        /// <param name="nick"><see cref="PKM.Nickname"/> to set. If no nickname is provided, the <see cref="PKM.Nickname"/> is set to the default value for its current language and format.</param>
        public static void SetNickname(this PKM pk, string nick)
        {
            if (string.IsNullOrWhiteSpace(nick))
            {
                pk.ClearNickname();
                return;
            }
            pk.IsNicknamed = true;
            pk.Nickname = nick;
        }

        /// <summary>
        /// Clears the <see cref="PKM.Nickname"/> to the default value.
        /// </summary>
        /// <param name="pk"></param>
        public static string ClearNickname(this PKM pk)
        {
            pk.IsNicknamed = false;
            string nick = SpeciesName.GetSpeciesNameGeneration(pk.Species, pk.Language, pk.Format);
            pk.Nickname = nick;
            if (pk is GBPKM pk12)
                pk12.SetNotNicknamed();
            return nick;
        }

        /// <summary>
        /// Sets the <see cref="PKM.AltForm"/> value, with special consideration for <see cref="PKM.Format"/> values which derive the <see cref="PKM.AltForm"/> value.
        /// </summary>
        /// <param name="pk">Pokémon to modify.</param>
        /// <param name="form">Desired <see cref="PKM.AltForm"/> value to set.</param>
        public static void SetAltForm(this PKM pk, int form)
        {
            switch (pk.Format)
            {
                case 2:
                    while (pk.AltForm != form)
                        pk.SetRandomIVs();
                    break;
                case 3:
                    pk.SetPIDUnown3(form);
                    break;
                default:
                    pk.AltForm = form;
                    break;
            }
        }

        /// <summary>
        /// Sets the <see cref="PKM.Ability"/> value by sanity checking the provided <see cref="PKM.Ability"/> against the possible pool of abilities.
        /// </summary>
        /// <param name="pk">Pokémon to modify.</param>
        /// <param name="abil">Desired <see cref="PKM.Ability"/> to set.</param>
        public static void SetAbility(this PKM pk, int abil)
        {
            if (abil < 0)
                return;
            var abilities = pk.PersonalInfo.Abilities;
            int abilIndex = Array.IndexOf(abilities, abil);
            abilIndex = Math.Max(0, abilIndex);
            pk.SetAbilityIndex(abilIndex);
        }

        /// <summary>
        /// Sets the <see cref="PKM.Ability"/> value based on the provided ability index (0-2)
        /// </summary>
        /// <param name="pk">Pokémon to modify.</param>
        /// <param name="abilIndex">Desired <see cref="PKM.AbilityNumber"/> (shifted by 1) to set.</param>
        public static void SetAbilityIndex(this PKM pk, int abilIndex)
        {
            if (pk is PK5 pk5 && abilIndex == 2)
                pk5.HiddenAbility = true;
            else if (pk.Format <= 5)
                pk.PID = PKX.GetRandomPID(Util.Rand, pk.Species, pk.Gender, pk.Version, pk.Nature, pk.AltForm, (uint)(abilIndex * 0x10001));
            pk.RefreshAbility(abilIndex);
        }

        /// <summary>
        /// Sets a Random <see cref="PKM.EncryptionConstant"/> value. The <see cref="PKM.EncryptionConstant"/> is not updated if the value should match the <see cref="PKM.PID"/> instead.
        /// </summary>
        /// <remarks>Accounts for Wurmple evolutions.</remarks>
        /// <param name="pk">Pokémon to modify.</param>
        public static void SetRandomEC(this PKM pk)
        {
            int gen = pk.GenNumber;
            if (2 < gen && gen < 6)
            {
                pk.EncryptionConstant = pk.PID;
                return;
            }

            int wIndex = WurmpleUtil.GetWurmpleEvoGroup(pk.Species);
            if (wIndex != -1)
            {
                pk.EncryptionConstant = WurmpleUtil.GetWurmpleEC(wIndex);
                return;
            }
            pk.EncryptionConstant = Util.Rand32();
        }

        /// <summary>
        /// Sets the <see cref="PKM.IsShiny"/> derived value.
        /// </summary>
        /// <param name="pk">Pokémon to modify.</param>
        /// <param name="shiny">Desired <see cref="PKM.IsShiny"/> state to set.</param>
        /// <returns></returns>
        public static bool SetIsShiny(this PKM pk, bool shiny) => shiny ? SetShiny(pk) : pk.SetUnshiny();

        /// <summary>
        /// Makes a <see cref="PKM"/> shiny.
        /// </summary>
        /// <param name="pk">Pokémon to modify.</param>
        /// <param name="xor0">Square shiny</param>
        /// <returns>Returns true if the <see cref="PKM"/> data was modified.</returns>
        public static bool SetShiny(PKM pk, bool xor0 = false)
        {
            if (pk.IsShiny)
                return false;

            while (true)
            {
                pk.SetShiny();

                if (pk.Format <= 7)
                    return true;

                var xor = pk.ShinyXor;
                if (xor0 ? xor == 0 : xor != 0)
                    return true;
            }
        }

        /// <summary>
        /// Makes a <see cref="PKM"/> not-shiny.
        /// </summary>
        /// <param name="pk">Pokémon to modify.</param>
        /// <returns>Returns true if the <see cref="PKM"/> data was modified.</returns>
        public static bool SetUnshiny(this PKM pk)
        {
            if (!pk.IsShiny)
                return false;

            pk.SetPIDGender(pk.Gender);
            return true;
        }

        /// <summary>
        /// Sets the <see cref="PKM.Nature"/> value, with special consideration for the <see cref="PKM.Format"/> values which derive the <see cref="PKM.Nature"/> value.
        /// </summary>
        /// <param name="pk">Pokémon to modify.</param>
        /// <param name="nature">Desired <see cref="PKM.Nature"/> value to set.</param>
        public static void SetNature(this PKM pk, int nature)
        {
            var value = Math.Min((int)Nature.Quirky, Math.Max((int)Nature.Hardy, nature));
            if (pk.Format >= 8)
                pk.StatNature = value;
            else if (pk.Format <= 4)
                pk.SetPIDNature(value);
            else
                pk.Nature = value;
        }

        /// <summary>
        /// Copies <see cref="ShowdownSet"/> details to the <see cref="PKM"/>.
        /// </summary>
        /// <param name="pk">Pokémon to modify.</param>
        /// <param name="Set"><see cref="ShowdownSet"/> details to copy from.</param>
        public static void ApplySetDetails(this PKM pk, ShowdownSet Set)
        {
            pk.Species = Math.Min(pk.MaxSpeciesID, Set.Species);
            pk.SetMoves(Set.Moves, true);
            pk.ApplyHeldItem(Set.HeldItem, Set.Format);
            pk.CurrentLevel = Set.Level;
            pk.CurrentFriendship = Set.Friendship;
            pk.IVs = Set.IVs;
            pk.EVs = Set.EVs;

            pk.SetSuggestedHyperTrainingData(Set.IVs);
            if (ShowdownSetIVMarkings)
                pk.SetMarkings();

            pk.SetNickname(Set.Nickname);
            pk.SetAltForm(Set.FormIndex);
            pk.SetGender(Set.Gender);
            pk.SetMaximumPPUps(Set.Moves);
            pk.SetAbility(Set.Ability);
            pk.SetNature(Set.Nature);
            pk.SetIsShiny(Set.Shiny);
            pk.SetRandomEC();

            if (pk is IAwakened a)
            {
                a.SetSuggestedAwakenedValues(pk);
                if (pk is PB7 b)
                {
                    for (int i = 0; i < 6; i++)
                        pk.SetEV(i, 0);
                    b.ResetCalculatedValues();
                }
            }

            if (pk is IGigantamax c)
                c.CanGigantamax = Set.CanGigantamax;

            pk.ClearRecordFlags();
            pk.SetRecordFlags(Set.Moves);

            if (ShowdownSetBehaviorNature && pk.Format >= 8)
                pk.Nature = pk.StatNature;

            var legal = new LegalityAnalysis(pk);
            if (legal.Parsed && legal.Info.Relearn.Any(z => !z.Valid))
                pk.SetRelearnMoves(legal.GetSuggestedRelearnMoves());
            pk.ResetPartyStats();
            pk.RefreshChecksum();
        }

        /// <summary>
        /// Sets the <see cref="PKM.HeldItem"/> value depending on the current format and the provided item index &amp; format.
        /// </summary>
        /// <param name="pk">Pokémon to modify.</param>
        /// <param name="item">Held Item to apply</param>
        /// <param name="format">Format required for importing</param>
        public static void ApplyHeldItem(this PKM pk, int item, int format)
        {
            item = ItemConverter.GetFormatHeldItemID(item, format, pk.Format);
            pk.HeldItem = ((uint)item > pk.MaxItemID) ? 0 : item;
        }

        /// <summary>
        /// Sets one of the <see cref="PKM.EVs"/> based on its index within the array.
        /// </summary>
        /// <param name="pk">Pokémon to modify.</param>
        /// <param name="index">Index to set to</param>
        /// <param name="value">Value to set</param>
        public static void SetEV(this PKM pk, int index, int value)
        {
            switch (index)
            {
                case 0: pk.EV_HP = value; break;
                case 1: pk.EV_ATK = value; break;
                case 2: pk.EV_DEF = value; break;
                case 3: pk.EV_SPE = value; break;
                case 4: pk.EV_SPA = value; break;
                case 5: pk.EV_SPD = value; break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(index));
            }
        }

        /// <summary>
        /// Sets one of the <see cref="PKM.IVs"/> based on its index within the array.
        /// </summary>
        /// <param name="pk">Pokémon to modify.</param>
        /// <param name="index">Index to set to</param>
        /// <param name="value">Value to set</param>
        public static void SetIV(this PKM pk, int index, int value)
        {
            switch (index)
            {
                case 0: pk.IV_HP = value; break;
                case 1: pk.IV_ATK = value; break;
                case 2: pk.IV_DEF = value; break;
                case 3: pk.IV_SPE = value; break;
                case 4: pk.IV_SPA = value; break;
                case 5: pk.IV_SPD = value; break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(index));
            }
        }

        /// <summary>
        /// Fetches the highest value the provided <see cref="PKM.EVs"/> index can be while considering others.
        /// </summary>
        /// <param name="pk">Pokémon to modify.</param>
        /// <param name="index">Index to fetch for</param>
        /// <returns>Highest value the value can be.</returns>
        public static int GetMaximumEV(this PKM pk, int index)
        {
            if (pk.Format < 3)
                return ushort.MaxValue;

            var EVs = pk.EVs;
            EVs[index] = 0;
            var sum = EVs.Sum();
            int remaining = 510 - sum;
            return Math.Min(Math.Max(remaining, 0), 252);
        }

        /// <summary>
        /// Fetches the highest value the provided <see cref="PKM.IVs"/>.
        /// </summary>
        /// <param name="pk">Pokémon to modify.</param>
        /// <param name="index">Index to fetch for</param>
        /// <param name="allow30">Causes the returned value to be dropped down -1 if the value is already at a maxmimum.</param>
        /// <returns>Highest value the value can be.</returns>
        public static int GetMaximumIV(this PKM pk, int index, bool allow30 = false)
        {
            if (pk.GetIV(index) == pk.MaxIV && allow30)
                return pk.MaxIV - 1;
            return pk.MaxIV;
        }

        /// <summary>
        /// Force hatches a PKM by applying the current species name and a valid Met Location from the origin game.
        /// </summary>
        /// <param name="pk">PKM to apply hatch details to</param>
        /// <param name="reHatch">Re-hatch already hatched <see cref="PKM"/> inputs</param>
        public static void ForceHatchPKM(this PKM pk, bool reHatch = false)
        {
            if (!pk.IsEgg && !reHatch)
                return;
            pk.IsEgg = false;
            pk.ClearNickname();
            pk.CurrentFriendship = pk.PersonalInfo.BaseFriendship;
            if (pk.IsTradedEgg)
                pk.Egg_Location = pk.Met_Location;
            var loc = EncounterSuggestion.GetSuggestedEggMetLocation(pk);
            if (loc >= 0)
                pk.Met_Location = loc;
            pk.MetDate = DateTime.Today;
            if (pk.Gen6)
                pk.SetHatchMemory6();
        }

        /// <summary>
        /// Force hatches a PKM by applying the current species name and a valid Met Location from the origin game.
        /// </summary>
        /// <param name="pk">PKM to apply hatch details to</param>
        /// <param name="origin">Game the egg originated from</param>
        /// <param name="dest">Game the egg is currently present on</param>
        public static void SetEggMetData(this PKM pk, GameVersion origin, GameVersion dest)
        {
            bool traded = origin == dest;
            var today = pk.MetDate = DateTime.Today;
            pk.Egg_Location = EncounterSuggestion.GetSuggestedEncounterEggLocationEgg(pk, traded);
            pk.EggMetDate = today;
        }

        /// <summary>
        /// Maximizes the <see cref="PKM.CurrentFriendship"/>. If the <see cref="PKM.IsEgg"/>, the hatch counter is set to 1.
        /// </summary>
        /// <param name="pk">PKM to apply hatch details to</param>
        public static void MaximizeFriendship(this PKM pk)
        {
            if (pk.IsEgg)
                pk.OT_Friendship = 1;
            else
                pk.CurrentFriendship = byte.MaxValue;
            if (pk is PB7 pb)
                pb.ResetCP();
        }

        /// <summary>
        /// Maximizes the <see cref="PKM.CurrentLevel"/>. If the <see cref="PKM.IsEgg"/>, the <see cref="PKM"/> is ignored.
        /// </summary>
        /// <param name="pk">PKM to apply hatch details to</param>
        public static void MaximizeLevel(this PKM pk)
        {
            if (pk.IsEgg)
                return;
            pk.CurrentLevel = 100;
            if (pk is PB7 pb)
                pb.ResetCP();
        }

        /// <summary>
        /// Sets the <see cref="PKM.Nickname"/> to its default value.
        /// </summary>
        /// <param name="pk">Pokémon to modify.</param>
        /// <param name="la">Precomputed optional</param>
        public static void SetDefaultNickname(this PKM pk, LegalityAnalysis la)
        {
            if (la.Parsed && la.EncounterOriginal is EncounterTrade t && t.HasNickname)
                pk.SetNickname(t.GetNickname(pk.Language));
            else
                pk.ClearNickname();
        }

        /// <summary>
        /// Sets the <see cref="PKM.Nickname"/> to its default value.
        /// </summary>
        /// <param name="pk">Pokémon to modify.</param>
        public static void SetDefaultNickname(this PKM pk) => pk.SetDefaultNickname(new LegalityAnalysis(pk));

        private static readonly string[] PotentialUnicode = { "★☆☆☆", "★★☆☆", "★★★☆", "★★★★" };
        private static readonly string[] PotentialNoUnicode = { "+", "++", "+++", "++++" };

        /// <summary>
        /// Gets the Potential evaluation of the input <see cref="pk"/>.
        /// </summary>
        /// <param name="pk">Pokémon to analyze.</param>
        /// <param name="unicode">Returned value is unicode or not</param>
        /// <returns>Potential string</returns>
        public static string GetPotentialString(this PKM pk, bool unicode = true)
        {
            var arr = unicode ? PotentialUnicode : PotentialNoUnicode;
            return arr[pk.PotentialRating];
        }

        // Extensions
        /// <summary>
        /// Gets the Location Name for the <see cref="PKM"/>
        /// </summary>
        /// <param name="pk">PKM to fetch data for</param>
        /// <param name="eggmet">Location requested is the egg obtained location, not met location.</param>
        /// <returns>Location string</returns>
        public static string GetLocationString(this PKM pk, bool eggmet)
        {
            if (pk.Format < 2)
                return string.Empty;

            int locval = eggmet ? pk.Egg_Location : pk.Met_Location;
            return GameInfo.GetLocationName(eggmet, locval, pk.Format, pk.GenNumber, (GameVersion)pk.Version);
        }
    }
}

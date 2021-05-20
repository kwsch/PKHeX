using System.Collections.Generic;
using System.IO;
using System.Linq;

using static PKHeX.Core.MessageStrings;

namespace PKHeX.Core
{
    /// <summary>
    /// Utility logic for dealing with <see cref="MysteryGift"/> objects.
    /// </summary>
    public static class MysteryUtil
    {
        /// <summary>
        /// Gets <see cref="MysteryGift"/> objects from a folder.
        /// </summary>
        /// <param name="folder">Folder path</param>
        /// <returns>Consumable list of gifts.</returns>
        public static IEnumerable<MysteryGift> GetGiftsFromFolder(string folder)
        {
            foreach (var file in Directory.EnumerateFiles(folder, "*", SearchOption.AllDirectories))
            {
                var fi = new FileInfo(file);
                if (!MysteryGift.IsMysteryGift(fi.Length))
                    continue;

                var gift = MysteryGift.GetMysteryGift(File.ReadAllBytes(file), fi.Extension);
                if (gift != null)
                    yield return gift;
            }
        }

        /// <summary>
        /// Gets a description of the <see cref="MysteryGift"/> using the current default string data.
        /// </summary>
        /// <param name="gift">Gift data to parse</param>
        /// <returns>List of lines</returns>
        public static IEnumerable<string> GetDescription(this MysteryGift gift) => gift.GetDescription(GameInfo.Strings);

        /// <summary>
        /// Gets a description of the <see cref="MysteryGift"/> using provided string data.
        /// </summary>
        /// <param name="gift">Gift data to parse</param>
        /// <param name="strings">String data to use</param>
        /// <returns>List of lines</returns>
        public static IEnumerable<string> GetDescription(this MysteryGift gift, IBasicStrings strings)
        {
            if (gift.Empty)
                return new[] { MsgMysteryGiftSlotEmpty };

            var result = new List<string> { gift.CardHeader };
            if (gift.IsItem)
            {
                AddLinesItem(gift, strings, result);
            }
            else if (gift.IsPokémon)
            {
                try
                {
                    AddLinesPKM(gift, strings, result);
                }
#pragma warning disable CA1031 // Do not catch general exception types
                catch { result.Add(MsgMysteryGiftParseFail); }
#pragma warning restore CA1031 // Do not catch general exception types
            }
            else switch (gift)
            {
                case WC7 { IsBP: true } w7bp:
                    result.Add($"BP: {w7bp.BP}");
                    break;
                case WC7 { IsBean: true } w7bean:
                    result.Add($"Bean ID: {w7bean.Bean}");
                    result.Add($"Quantity: {w7bean.Quantity}");
                    break;
                default:
                    result.Add(MsgMysteryGiftParseTypeUnknown);
                    break;
            }
            switch (gift)
            {
                case WC7 w7:
                    result.Add($"Repeatable: {w7.GiftRepeatable}");
                    result.Add($"Collected: {w7.GiftUsed}");
                    result.Add($"Once Per Day: {w7.GiftOncePerDay}");
                    break;
            }
            return result;
        }

        private static void AddLinesItem(MysteryGift gift, IBasicStrings strings, ICollection<string> result)
        {
            result.Add($"Item: {strings.Item[gift.ItemID]} (Quantity: {gift.Quantity})");
            if (gift is not WC7 wc7)
                return;

            for (var ind = 1; wc7.GetItem(ind) != 0; ind++)
            {
                result.Add($"Item: {strings.Item[wc7.GetItem(ind)]} (Quantity: {wc7.GetQuantity(ind)})");
            }
        }

        private static void AddLinesPKM(MysteryGift gift, IBasicStrings strings, ICollection<string> result)
        {
            var id = gift.Generation < 7 ? $"{gift.TID:D5}/{gift.SID:D5}" : $"[{gift.TrainerSID7:D4}]{gift.TrainerID7:D6}";

            var first =
                $"{strings.Species[gift.Species]} @ {strings.Item[gift.HeldItem >= 0 ? gift.HeldItem : 0]}  --- "
                + (gift.IsEgg ? strings.EggName : $"{gift.OT_Name} - {id}");
            result.Add(first);
            result.Add(string.Join(" / ", gift.Moves.Select(z => strings.Move[z])));

            if (gift is WC7 wc7)
            {
                var addItem = wc7.AdditionalItem;
                if (addItem != 0)
                    result.Add($"+ {strings.Item[addItem]}");
            }
        }

        /// <summary>
        /// Checks if the <see cref="MysteryGift"/> data is compatible with the <see cref="SaveFile"/>. Sets appropriate data to the save file in order to receive the gift.
        /// </summary>
        /// <param name="g">Gift data to potentially insert to the save file.</param>
        /// <param name="sav">Save file receiving the gift data.</param>
        /// <param name="message">Error message if incompatible.</param>
        /// <returns>True if compatible, false if incompatible.</returns>
        public static bool IsCardCompatible(this MysteryGift g, SaveFile sav, out string message)
        {
            if (g.Generation != sav.Generation)
            {
                message = MsgMysteryGiftSlotSpecialReject;
                return false;
            }

            if (!sav.CanReceiveGift(g))
            {
                message = MsgMysteryGiftTypeDetails;
                return false;
            }

            if (g is WC6 && g.CardID == 2048 && g.ItemID == 726) // Eon Ticket (OR/AS)
            {
                if (sav is not SAV6AO)
                {
                    message = MsgMysteryGiftSlotSpecialReject;
                    return false;
                }
            }

            message = string.Empty;
            return true;
        }

        /// <summary>
        /// Checks if the gift values are receivable by the game.
        /// </summary>
        /// <param name="sav">Save file receiving the gift data.</param>
        /// <param name="gift">Gift data to potentially insert to the save file.</param>
        /// <returns>True if compatible, false if incompatible.</returns>
        public static bool CanReceiveGift(this SaveFile sav, MysteryGift gift)
        {
            if (gift.Species > sav.MaxSpeciesID)
                return false;
            if (gift.Moves.Any(move => move > sav.MaxMoveID))
                return false;
            if (gift.HeldItem > sav.MaxItemID)
                return false;
            return true;
        }
    }
}

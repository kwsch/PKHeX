using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using PKHeX.Core;
using PKHeX.WinForms.Properties;

namespace PKHeX.WinForms.Controls
{
    public static class Sorter
    {
        public static ContextMenuStrip GetSortStrip(this SAVEditor sav)
        {
            var sortMenu = new ContextMenuStrip();
            foreach (Level z in Enum.GetValues(typeof(Level)))
            {
                var ctrl = new ToolStripMenuItem(z.ToString(), GetImage(z));
                sortMenu.Items.Add(ctrl);
            }

            Image GetImage(Level l)
            {
                switch (l)
                {
                    case Level.Delete: return Resources.nocheck;
                    case Level.SortBox: return Resources.swapBox;
                    case Level.SortBoxAdvanced: return Resources.settings;
                    case Level.Modify: return Resources.wand;
                    default: return null;
                }
            }

            AddItem(Level.Delete, GetItem("All", "Clear", () => Clear(), Resources.nocheck));
            AddItem(Level.Delete, GetItem("Eggs", "Eggs", () => Clear(pk => pk.IsEgg), Resources.about));
            AddItem(Level.Delete, GetItem("Foreign", "Foreign", () => Clear(pk => !sav.SAV.IsOriginalHandler(pk, pk.Format > 2)), Resources.users));
            AddItem(Level.Delete, GetItem("Untrained", "Untrained", () => Clear(pk => pk.EVTotal == 0), Resources.gift));

            AddItem(Level.SortBox, GetItem("Species", "Pokédex No.", () => Sort(PKMSorting.OrderBySpecies), Resources.numlohi));
            AddItem(Level.SortBox, GetItem("SpeciesRev", "Pokédex No. (Reverse)", () => Sort(PKMSorting.OrderByDescendingSpecies), Resources.numhilo));
            AddItem(Level.SortBox, GetItem("Level", "Level (Low to High)", () => Sort(PKMSorting.OrderByLevel), Resources.vallohi));
            AddItem(Level.SortBox, GetItem("LevelRev", "Level (High to Low)", () => Sort(PKMSorting.OrderByDescendingLevel), Resources.valhilo));
            AddItem(Level.SortBox, GetItem("Date", "Met Date", () => Sort(PKMSorting.OrderByDateObtained), Resources.date));
            AddItem(Level.SortBox, GetItem("Name", "Species Name", () => Sort(list => list.OrderBySpeciesName(GameInfo.Strings.Species)), Resources.alphaAZ));
            AddItem(Level.SortBox, GetItem("Random", "Random", () => Sort(list => list.OrderByCustom(_ => Util.Rand32())), Resources.showdown));

            AddItem(Level.SortBoxAdvanced, GetItem("Usage", "Usage", () => Sort(PKMSorting.OrderByUsage), Resources.heart));
            AddItem(Level.SortBoxAdvanced, GetItem("Training", "Training", () => Sort(list => list.OrderByCustom(pk => pk.MaxEV * 6 - pk.EVTotal)), Resources.showdown));
            AddItem(Level.SortBoxAdvanced, GetItem("Owner", "Ownership", () => Sort(list => list.OrderByOwnership(sav.SAV)), Resources.users));
            AddItem(Level.SortBoxAdvanced, GetItem("Type", "Type", () => Sort(list => list.OrderByCustom(pk => pk.PersonalInfo.Type1, pk => pk.PersonalInfo.Type2)), Resources.main));
            AddItem(Level.SortBoxAdvanced, GetItem("Version", "Version", () => Sort(list => list.OrderByCustom(pk => pk.GenNumber, pk => pk.Version)), Resources.numlohi));
            AddItem(Level.SortBoxAdvanced, GetItem("BST", "BST", () => Sort(list => list.OrderByCustom(pk => pk.PersonalInfo.BST)), Resources.vallohi));
            AddItem(Level.SortBoxAdvanced, GetItem("Legal", "Legal", () => Sort(list => list.OrderByCustom(pk => !new LegalityAnalysis(pk).Valid)), Resources.export));

            AddItem(Level.Modify, GetItem("HatchEggs", "Hatch Eggs", () => Modify(z => z.ForceHatchPKM()), Resources.about));
            AddItem(Level.Modify, GetItem("MaxFriendship", "Max Friendship", () => Modify(z => z.CurrentFriendship = byte.MaxValue), Resources.heart));

            void AddItem(Level v, ToolStripItem t)
            {
                var item = (ToolStripMenuItem)sortMenu.Items[(int) v];
                t.Name = $"mnu_{v}{t.Name}";
                item.DropDownItems.Add(t);
            }

            void Clear(Func<PKM, bool> criteria = null)
            {
                if (Control.ModifierKeys.HasFlag(Keys.Shift))
                    sav.ClearAll(criteria);
                else
                    sav.ClearCurrent(criteria);
            }

            void Sort(Func<IEnumerable<PKM>, IEnumerable<PKM>> sorter)
            {
                if (Control.ModifierKeys.HasFlag(Keys.Shift))
                    sav.SortAll(sorter);
                else
                    sav.SortCurrent(sorter);
            }

            void Modify(Action<PKM> action)
            {
                if (Control.ModifierKeys.HasFlag(Keys.Shift))
                    sav.ModifyAll(action);
                else
                    sav.ModifyCurrent(action);
            }

            return sortMenu;
        }

        private static ToolStripItem GetItem(string name, string text, Action action, Image img)
        {
            var tsi = new ToolStripMenuItem {Name = name, Text = text, Image = img};
            tsi.Click += (s, e) => action();
            return tsi;
        }

        private enum Level
        {
            Delete,
            SortBox,
            SortBoxAdvanced,
            Modify,
        }
    }
}

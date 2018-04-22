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
            var options = new[]
            {
                GetItem("mnu_ClearBox", "Clear", Clear, Resources.nocheck),
                GetItem("mnu_SortBoxSpecies", "Sort: Pokédex No.", () => Sort(PKMSorting.OrderBySpecies), Resources.numlohi),
                GetItem("mnu_SortBoxSpeciesRev", "Sort: Pokédex No. (Reverse)", () => Sort(PKMSorting.OrderByDescendingSpecies), Resources.numhilo),
                GetItem("mnu_SortBoxLevel", "Sort: Level (Low to High)", () => Sort(PKMSorting.OrderByLevel), Resources.vallohi),
                GetItem("mnu_SortBoxLevelRev", "Sort: Level (High to Low)", () => Sort(PKMSorting.OrderByDescendingLevel), Resources.valhilo),
                GetItem("mnu_SortBoxDate", "Sort: Met Date", () => Sort(PKMSorting.OrderByDateObtained), Resources.date),
                GetItem("mnu_SortBoxUsage", "Sort: Usage", () => Sort(PKMSorting.OrderByUsage), Resources.heart),
                GetItem("mnu_SortBoxName", "Sort: Species Name", () => Sort(list => list.OrderBySpeciesName(GameInfo.Strings.Species)), Resources.alphaAZ),
                GetItem("mnu_SortBoxOwner", "Sort: Ownership", () => Sort(list => list.OrderByOwnership(sav.SAV)), Resources.users),
                GetItem("mnu_SortBoxRandom", "Sort: Random", () => Sort(list => list.OrderByCustom(_ => Util.Rand32())), Resources.showdown),
            };
            sortMenu.Items.AddRange(options);

            void Clear()
            {
                if (Control.ModifierKeys.HasFlag(Keys.Shift))
                    sav.ClearAll();
                else
                    sav.ClearCurrent();
            }

            void Sort(Func<IEnumerable<PKM>, IEnumerable<PKM>> sorter)
            {
                if (Control.ModifierKeys.HasFlag(Keys.Shift))
                    sav.SortAll(sorter);
                else
                    sav.SortCurrent(sorter);
            }

            return sortMenu;
        }

        private static ToolStripItem GetItem(string name, string text, Action action, Image img)
        {
            var tsi = new ToolStripMenuItem {Name = name, Text = text, Image = img};
            tsi.Click += (s, e) => action();
            return tsi;
        }
    }

    public interface ISortableDisplay
    {
        void ClearAll();
        void ClearCurrent();
        void SortAll(Func<IEnumerable<PKM>, IEnumerable<PKM>> sorter);
        void SortCurrent(Func<IEnumerable<PKM>, IEnumerable<PKM>> sorter);
    }
}

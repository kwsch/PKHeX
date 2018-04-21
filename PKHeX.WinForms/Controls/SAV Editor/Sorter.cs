using System;
using System.Collections.Generic;
using System.Windows.Forms;
using PKHeX.Core;

namespace PKHeX.WinForms.Controls
{
    public static class Sorter
    {
        public static ContextMenuStrip GetSortStrip(this SAVEditor sav)
        {
            var sortMenu = new ContextMenuStrip();
            var options = new[]
            {
                GetItem("mnu_ClearBox", "Clear", Clear),
                GetItem("mnu_SortBoxSpecies", "Sort: SpeciesID", () => Sort(PKMSorting.OrderBySpecies)),
                GetItem("mnu_SortBoxSpecies", "Sort: SpeciesIDRev", () => Sort(PKMSorting.OrderByDescendingSpecies)),
                GetItem("mnu_SortBoxSpecies", "Sort: Level Low->High", () => Sort(PKMSorting.OrderByLevel)),
                GetItem("mnu_SortBoxSpecies", "Sort: Level High->Low", () => Sort(PKMSorting.OrderByDescendingLevel)),
                GetItem("mnu_SortBoxSpecies", "Sort: Date", () => Sort(PKMSorting.OrderByDateObtained)),
                GetItem("mnu_SortBoxSpecies", "Sort: Usage", () => Sort(PKMSorting.OrderByDescendingLevel)),
                GetItem("mnu_SortBoxSpecies", "Sort: SpeciesName", () => Sort(list => list.OrderBySpeciesName(GameInfo.Strings.Species))),
                GetItem("mnu_SortBoxSpecies", "Sort: Ownership", () => Sort(list => list.OrderByOwnership(sav.SAV))),
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

        private static ToolStripItem GetItem(string name, string text, Action action)
        {
            var tsi = new ToolStripMenuItem {Name = name, Text = text};
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

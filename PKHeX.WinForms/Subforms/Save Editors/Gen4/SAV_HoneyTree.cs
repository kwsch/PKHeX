using System;
using System.Linq;
using System.Windows.Forms;
using PKHeX.Core;

namespace PKHeX.WinForms
{
    public partial class SAV_HoneyTree : Form
    {
        private readonly SAV4Sinnoh Origin;
        private readonly SAV4Sinnoh SAV;

        public SAV_HoneyTree(SAV4Sinnoh sav)
        {
            InitializeComponent();
            WinFormsUtil.TranslateInterface(this, Main.CurrentLanguage);
            SAV = (SAV4Sinnoh)(Origin = sav).Clone();

            Table = SAV switch
            {
                SAV4DP => HoneyTree.TableDP,
                SAV4Pt => HoneyTree.TablePt,
                _ => throw new Exception()
            };

            // Get Munchlax tree for this savegame in screen
            MunchlaxTrees = SAV.GetMunchlaxTrees();

            const string sep = "- ";
            L_Tree0.Text = string.Join(Environment.NewLine, MunchlaxTrees.Select(z => sep + CB_TreeList.Items[z]));

            CB_TreeList.SelectedIndex = 0;
        }

        private readonly int[] MunchlaxTrees;
        private readonly int[][] Table;
        private int entry;
        private bool loading;
        private HoneyTree? Tree;

        private int TreeSpecies => Table[(int)NUD_Group.Value][(int)NUD_Slot.Value];
        private void B_Catchable_Click(object sender, EventArgs e) => NUD_Time.Value = 1080;

        private void ChangeGroupSlot(object sender, EventArgs e)
        {
            int species = TreeSpecies;
            L_Species.Text = species != 266 // silcoon/cascoon
                ? GameInfo.Strings.specieslist[species]
                : GameInfo.Strings.specieslist[species + 0] + $" ({GameInfo.Strings.gamelist[10]})" + Environment.NewLine
                + GameInfo.Strings.specieslist[species + 2] + $" ({GameInfo.Strings.gamelist[11]})";

            if (loading)
                return;

            if (species == (int)Species.Munchlax && !MunchlaxTrees.Contains(CB_TreeList.SelectedIndex))
                WinFormsUtil.Alert("Catching Munchlax in this tree will make it illegal for this savegame's TID/SID combination.");
        }

        private void ChangeTree(object sender, EventArgs e)
        {
            SaveTree();
            entry = CB_TreeList.SelectedIndex;
            ReadTree();
        }

        private void ReadTree()
        {
            loading = true;
            Tree = SAV.GetHoneyTree(entry);

            NUD_Time.Value = Math.Min(NUD_Time.Maximum, Tree.Time);
            NUD_Shake.Value = Math.Min(NUD_Shake.Maximum, Tree.Shake);
            NUD_Group.Value = Math.Min(NUD_Group.Maximum, Tree.Group);
            NUD_Slot.Value = Math.Min(NUD_Slot.Maximum, Tree.Slot);

            ChangeGroupSlot(this, EventArgs.Empty);
            loading = false;
        }

        private void SaveTree()
        {
            if (Tree == null)
                return;

            Tree.Time = (uint)NUD_Time.Value;
            Tree.Shake = (int)NUD_Shake.Value;
            Tree.Group = (int)NUD_Group.Value;
            Tree.Slot = (int)NUD_Slot.Value;

            SAV.SetHoneyTree(Tree, entry);
        }

        private void B_Save_Click(object sender, EventArgs e)
        {
            SaveTree();
            Origin.CopyChangesFrom(SAV);
            Close();
        }

        private void B_Cancel_Click(object sender, EventArgs e) => Close();
    }
}

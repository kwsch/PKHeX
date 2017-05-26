using System;
using System.Linq;
using System.Windows.Forms;
using PKHeX.Core;

namespace PKHeX.WinForms
{
    public partial class SAV_HoneyTree : Form
    {
        private readonly SaveFile Origin;
        private readonly SAV4 SAV;
        public SAV_HoneyTree(SaveFile sav)
        {
            SAV = (SAV4)(Origin = sav).Clone();
            InitializeComponent();
            WinFormsUtil.TranslateInterface(this, Main.curlanguage);

            if (SAV.DP)
                Table = HoneyTree.TableDP;
            else if (SAV.Pt)
                Table = HoneyTree.TablePt;

            // Get Munchlax tree for this savegame in screen
            MunchlaxTrees = SAV.MunchlaxTrees;

            const string sep = "- ";
            L_Tree0.Text = string.Join(Environment.NewLine, MunchlaxTrees.Select(z => sep + CB_TreeList.Items[z]));

            CB_TreeList.SelectedIndex = 0;
        }
        
        private readonly int[] MunchlaxTrees;
        private readonly int[][] Table;
        private int entry;
        private bool loading;
        private HoneyTree Tree;

        private int TreeSpecies => Table[(int)NUD_Group.Value][(int)NUD_Slot.Value];
        private void B_Catchable_Click(object sender, EventArgs e) => NUD_Time.Value = 1080;
        private void changeGroupSlot(object sender, EventArgs e)
        {
            int species = TreeSpecies;
            L_Species.Text = species != 266 // silcoon/cascoon
                ? GameInfo.Strings.specieslist[species]
                : GameInfo.Strings.specieslist[species + 0] + $" ({GameInfo.Strings.gamelist[10]})" + Environment.NewLine
                + GameInfo.Strings.specieslist[species + 2] + $" ({GameInfo.Strings.gamelist[11]})";

            if (loading)
                return;

            if (species == 446 && !MunchlaxTrees.Contains(CB_TreeList.SelectedIndex))
                WinFormsUtil.Alert("Catching Munchlax in this tree will make it illegal for this savegame's TID/SID combination.");
        }
        private void changeTree(object sender, EventArgs e)
        {
            saveTree();
            entry = CB_TreeList.SelectedIndex;
            readTree();
        }
        private void readTree()
        {
            loading = true;
            Tree = SAV.getHoneyTree(entry);

            NUD_Time.Value = Math.Min(NUD_Time.Maximum, Tree.Time);
            NUD_Shake.Value = Math.Min(NUD_Shake.Maximum, Tree.Shake);
            NUD_Group.Value = Math.Min(NUD_Group.Maximum, Tree.Group);
            NUD_Slot.Value = Math.Min(NUD_Slot.Maximum, Tree.Slot);

            changeGroupSlot(null, null);
            loading = false;
        }
        private void saveTree()
        {
            if (Tree == null)
                return;
            
            Tree.Time = (uint)NUD_Time.Value;
            Tree.Shake = (int)NUD_Shake.Value;
            Tree.Group = (int)NUD_Group.Value;
            Tree.Slot = (int)NUD_Slot.Value;

            SAV.setHoneyTree(Tree, entry);
        }

        private void B_Save_Click(object sender, EventArgs e)
        {
            saveTree();
            Origin.setData(SAV.Data, 0);
            Close();
        }
        private void B_Cancel_Click(object sender, EventArgs e) => Close();
    }
}

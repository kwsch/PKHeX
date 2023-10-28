using System;
using System.Windows.Forms;
using PKHeX.Core;

namespace PKHeX.WinForms;

public partial class SAV_HoneyTree : Form
{
    private readonly SAV4Sinnoh Origin;
    private readonly SAV4Sinnoh SAV;

    public SAV_HoneyTree(SAV4Sinnoh sav)
    {
        InitializeComponent();
        WinFormsUtil.TranslateInterface(this, Main.CurrentLanguage);
        SAV = (SAV4Sinnoh)(Origin = sav).Clone();

        // Get Munchlax tree for this savegame in screen
        MunchlaxTrees = new byte[4];
        HoneyTreeUtil.CalculateMunchlaxTrees(SAV.ID32, MunchlaxTrees);

        const string sep = "- ";
        var names = CB_TreeList.Items;
        L_Tree0.Text = string.Join(Environment.NewLine,
            sep + names[MunchlaxTrees[0]],
            sep + names[MunchlaxTrees[1]],
            sep + names[MunchlaxTrees[2]],
            sep + names[MunchlaxTrees[3]]);

        CB_TreeList.SelectedIndex = 0;
    }

    private readonly byte[] MunchlaxTrees;
    private int entry;
    private bool loading;
    private HoneyTreeValue? Tree;

    private ushort TreeSpecies => SAV.GetHoneyTreeSpecies((int)NUD_Group.Value, (int)NUD_Slot.Value);
    private void B_Catchable_Click(object sender, EventArgs e) => NUD_Time.Value = 1080;

    private void ChangeGroupSlot(object sender, EventArgs e)
    {
        var species = TreeSpecies;
        L_Species.Text = GetLabelText(species);

        if (loading)
            return;

        if (species == (int)Species.Munchlax && !MunchlaxTrees.AsSpan().Contains((byte)CB_TreeList.SelectedIndex))
            WinFormsUtil.Alert("Catching Munchlax in this tree will make it illegal for this savegame's TID16/SID16 combination.");
    }

    private static string GetLabelText(ushort species)
    {
        var str = GameInfo.Strings;
        var arr = str.specieslist;
        if (species != (int)Species.Silcoon)
            return arr[species];

        // Silcoon/Cascoon
        var games = str.gamelist;
        return $"{arr[species + 0]} ({games[(int)GameVersion.D]})" + Environment.NewLine +
               $"{arr[species + 2]} ({games[(int)GameVersion.P]})";
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

using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using PKHeX.Core;
using PKHeX.WinForms.Properties;

namespace PKHeX.WinForms.Controls;

public sealed class BoxMenuStrip : ContextMenuStrip
{
    private readonly SAVEditor SAV;
    private readonly List<ItemVisibility> CustomItems = [];
    private readonly BoxManipulator Manipulator;

    public BoxMenuStrip(SAVEditor sav)
    {
        Manipulator = new BoxManipulatorWF(sav);
        SAV = sav;
        var categories = BoxManipUtil.ManipCategories;
        var names = BoxManipUtil.ManipCategoryNames;
        for (int i = 0; i < categories.Length; i++)
        {
            var category = categories[i];
            var sprite = TopLevelImages[i];
            var name = names[i];
            var parent = new ToolStripMenuItem {Name = $"mnu_{name}", Text = name, Image = sprite};
            foreach (var item in category)
                AddItem(sav, parent, item);
            Items.Add(parent);
        }
    }

    private void AddItem(ISaveFileProvider sav, ToolStripDropDownItem parent, IBoxManip item)
    {
        var name = item.Type.ToString();
        ManipTypeImage.TryGetValue(item.Type, out var img);
        var tsi = new ToolStripMenuItem { Name = $"mnu_{name}", Text = name, Image = img };
        tsi.Click += (_, _) => Manipulator.Execute(item, sav.CurrentBox, All, Reverse);
        parent.DropDownItems.Add(tsi);
        CustomItems.Add(new ItemVisibility(tsi, item));
    }

    private static readonly Dictionary<BoxManipType, Image> ManipTypeImage = new()
    {
        [BoxManipType.DeleteAll] = Resources.nocheck,
        [BoxManipType.DeleteEggs] = Resources.about,
        [BoxManipType.DeletePastGen] = Resources.bak,
        [BoxManipType.DeleteForeign] = Resources.language,
        [BoxManipType.DeleteUntrained] = Resources.gift,
        [BoxManipType.DeleteItemless] = Resources.main,
        [BoxManipType.DeleteIllegal] = Resources.export,
        [BoxManipType.DeleteClones] = Resources.users,

        [BoxManipType.SortSpecies] = Resources.numlohi,
        [BoxManipType.SortSpeciesReverse] = Resources.numhilo,
        [BoxManipType.SortLevel] = Resources.vallohi,
        [BoxManipType.SortLevelReverse] = Resources.valhilo,
        [BoxManipType.SortDate] = Resources.date,
        [BoxManipType.SortName] = Resources.alphaAZ,
        [BoxManipType.SortFavorite] = Resources.heart,
        [BoxManipType.SortParty] = Resources.users,
        [BoxManipType.SortShiny] = Resources.showdown,
        [BoxManipType.SortRandom] = Resources.wand,

        [BoxManipType.SortUsage] = Resources.heart,
        [BoxManipType.SortPotential] = Resources.numhilo,
        [BoxManipType.SortTraining] = Resources.showdown,
        [BoxManipType.SortOwner] = Resources.users,
        [BoxManipType.SortType] = Resources.main,
        [BoxManipType.SortTypeTera] = Resources.main,
        [BoxManipType.SortVersion] = Resources.numlohi,
        [BoxManipType.SortBST] = Resources.vallohi,
        [BoxManipType.SortCP] = Resources.vallohi,
        [BoxManipType.SortScale] = Resources.vallohi,
        [BoxManipType.SortRibbons] = Resources.valhilo,
        [BoxManipType.SortMarks] = Resources.valhilo,
        [BoxManipType.SortLegal] = Resources.export,
        [BoxManipType.SortEncounterType] = Resources.about,

        [BoxManipType.ModifyHatchEggs] = Resources.about,
        [BoxManipType.ModifyMaxFriendship] = Resources.users,
        [BoxManipType.ModifyMaxLevel] = Resources.showdown,
        [BoxManipType.ModifyResetMoves] = Resources.date,
        [BoxManipType.ModifyRandomMoves] = Resources.wand,
        [BoxManipType.ModifyHyperTrain] = Resources.vallohi,
        [BoxManipType.ModifyGanbaru] = Resources.vallohi,
        [BoxManipType.ModifyRemoveNicknames] = Resources.alphaAZ,
        [BoxManipType.ModifyRemoveItem] = Resources.gift,
        [BoxManipType.ModifyHeal] = Resources.heart,
    };

    private sealed class ItemVisibility(ToolStripItem toolStripItem, IBoxManip visible)
    {
        public void SetVisibility(SaveFile s) => toolStripItem.Visible = visible.Usable(s);
    }

    public void ToggleVisibility()
    {
        foreach (var s in CustomItems)
            s.SetVisibility(SAV.SAV);
    }

    private static readonly Image[] TopLevelImages =
    [
        Resources.nocheck,
        Resources.swapBox,
        Resources.settings,
        Resources.wand,
    ];

    public void Clear() => Manipulator.Execute(BoxManipType.DeleteAll, SAV.CurrentBox, All);
    public void Sort() => Manipulator.Execute(BoxManipType.SortSpecies, SAV.CurrentBox, All);

    private static bool All => (ModifierKeys & Keys.Shift) != 0;
    private static bool Reverse => (ModifierKeys & Keys.Control) != 0;
}

/// <summary>
/// Implementation of a WinForms box manipulator (using MessageBox prompts)
/// </summary>
public sealed class BoxManipulatorWF(SAVEditor editor) : BoxManipulator
{
    protected override SaveFile SAV => editor.SAV;

    protected override void FinishBoxManipulation(string message, bool all, int count) => editor.FinishBoxManipulation(message, all, count);

    protected override bool CanManipulateRegion(int start, int end, string prompt, string fail)
    {
        if (!string.IsNullOrEmpty(prompt) && WinFormsUtil.Prompt(MessageBoxButtons.YesNo, prompt) != DialogResult.Yes)
            return false;
        bool canModify = base.CanManipulateRegion(start, end, prompt, fail);
        if (!canModify && !string.IsNullOrEmpty(fail))
            WinFormsUtil.Alert(fail);
        return canModify;
    }
}

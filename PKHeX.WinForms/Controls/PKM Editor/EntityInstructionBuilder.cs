using System;
using System.Reflection;
using System.Windows.Forms;
using PKHeX.Core;
using static PKHeX.Core.MessageStrings;

namespace PKHeX.WinForms.Controls;

public partial class EntityInstructionBuilder : UserControl
{
    private readonly Func<PKM> Getter;
    private PKM Entity => Getter();

    private static ReadOnlySpan<char> Prefixes => StringInstruction.Prefixes;

    private int currentFormat = -1;
    private int requirementIndex;
    private bool readOnlyMode;
    private readonly ToolStripMenuItem[] requireMenuItems = new ToolStripMenuItem[Prefixes.Length];

    public EntityInstructionBuilder(Func<PKM> pk)
    {
        Getter = pk;
        InitializeComponent();
        for (int i = 0; i < Prefixes.Length; i++)
        {
            var text = i == 0 ? "Set" : Prefixes[i].ToString();
            var item = new ToolStripMenuItem(text)
            {
                Name = $"mnu_{text}",
                Tag = i,
            };
            item.Click += RequireItem_Click;
            requireMenu.Items.Add(item);
            requireMenuItems[i] = item;
        }

        // Allow translation of the menu item.
        WinFormsTranslator.TranslateControls("BatchEdit", requireMenuItems, Main.CurrentLanguage);

        B_Require.ContextMenuStrip = requireMenu;

        CB_Format.Items.Clear();
        CB_Format.Items.Add(MsgAny);
        foreach (Type t in BatchEditing.Types)
            CB_Format.Items.Add(t.Name.ToLowerInvariant());
        CB_Format.Items.Add(MsgAll);

        CB_Format.SelectedIndex = 0;
        SetRequirementIndex(0);
        UpdateRequireMenuVisibility();
        toolTip1.SetToolTip(CB_Property, MsgBEToolTipPropName);
        toolTip2.SetToolTip(L_PropType, MsgBEToolTipPropType);
        toolTip3.SetToolTip(L_PropValue, MsgBEToolTipPropValue);
    }

    private void CB_Format_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (currentFormat == CB_Format.SelectedIndex)
            return;

        byte format = (byte)CB_Format.SelectedIndex;
        CB_Property.Items.Clear();
        CB_Property.Items.AddRange(BatchEditing.Properties[format]);
        CB_Property.SelectedIndex = 0;
        currentFormat = format;
    }

    private void CB_Property_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (!BatchEditing.TryGetPropertyType(CB_Property.Text, out var type, CB_Format.SelectedIndex))
            type = "Unknown";
        L_PropType.Text = type;

        if (BatchEditing.TryGetHasProperty(Entity, CB_Property.Text, out var pi))
        {
            L_PropType.ResetForeColor();

            bool hasValue = GetPropertyDisplayText(pi, Entity, out var display);
            L_PropValue.Text = display;
            if (hasValue)
                L_PropValue.ResetForeColor();
            else
                L_PropValue.ForeColor = WinFormsUtil.ColorWarn;
        }
        else // no property, flag
        {
            L_PropValue.Text = string.Empty;
            L_PropType.ForeColor = WinFormsUtil.ColorWarn;
        }
    }

    private void B_Require_Click(object? sender, EventArgs e) => requireMenu.Show(B_Require, 0, B_Require.Height);

    private void RequireItem_Click(object? sender, EventArgs e)
    {
        if (sender is not ToolStripMenuItem { Tag: int index })
            return;

        SetRequirementIndex(index);
    }

    private void SetRequirementIndex(int index)
    {
        if ((uint)index >= Prefixes.Length)
            return;

        requirementIndex = index;
        B_Require.Text = requireMenuItems[index].Text;

        for (int i = 0; i < requireMenuItems.Length; i++)
            requireMenuItems[i].Checked = i == index;
    }

    private void UpdateRequireMenuVisibility()
    {
        requireMenuItems[0].Visible = !readOnlyMode;

        if (readOnlyMode && requirementIndex == 0)
            SetRequirementIndex(1);
    }

    private static bool GetPropertyDisplayText(PropertyInfo pi, PKM pk, out string display)
    {
        var type = pi.PropertyType;
        if (type.IsGenericType)
        {
            if (type.GetGenericTypeDefinition().IsByRefLike) // Span, ReadOnlySpan
            {
                display = pi.PropertyType.ToString();
                return false;
            }
        }

        var value = pi.GetValue(pk);
        if (value?.ToString() is not { } x)
        {
            display = "null";
            return false;
        }

        display = x;
        return true;
    }

    public string Create()
    {
        if (CB_Property.SelectedIndex < 0)
            return string.Empty;

        var property = CB_Property.Items[CB_Property.SelectedIndex];
        var prefix = Prefixes[requirementIndex];
        const char equals = StringInstruction.SplitInstruction;
        return $"{prefix}{property}{equals}";
    }

    public bool ReadOnly
    {
        set
        {
            readOnlyMode = value;
            UpdateRequireMenuVisibility();
        }
    }
}

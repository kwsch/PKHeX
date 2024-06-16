using System;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;
using PKHeX.Core;
using static PKHeX.Core.MessageStrings;

namespace PKHeX.WinForms.Controls;

public partial class EntityInstructionBuilder : UserControl
{
    private readonly Func<PKM> Getter;
    private PKM Entity => Getter();

    private int currentFormat = -1;

    public EntityInstructionBuilder(Func<PKM> pk)
    {
        Getter = pk;
        InitializeComponent();

        CB_Format.Items.Clear();
        CB_Format.Items.Add(MsgAny);
        foreach (Type t in BatchEditing.Types)
            CB_Format.Items.Add(t.Name.ToLowerInvariant());
        CB_Format.Items.Add(MsgAll);

        CB_Format.SelectedIndex = CB_Require.SelectedIndex = 0;
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
        L_PropType.Text = BatchEditing.GetPropertyType(CB_Property.Text, CB_Format.SelectedIndex);
        if (BatchEditing.TryGetHasProperty(Entity, CB_Property.Text, out var pi))
        {
            L_PropType.ResetForeColor();

            bool hasValue = GetPropertyDisplayText(pi, Entity, out var display);
            L_PropValue.Text = display;
            if (hasValue)
                L_PropValue.ResetForeColor();
            else
                L_PropValue.ForeColor = Color.Red;
        }
        else // no property, flag
        {
            L_PropValue.Text = string.Empty;
            L_PropType.ForeColor = Color.Red;
        }
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

        var prefixes = StringInstruction.Prefixes;
        var prefix = prefixes[CB_Require.SelectedIndex];
        var property = CB_Property.Items[CB_Property.SelectedIndex];
        const char equals = StringInstruction.SplitInstruction;
        return $"{prefix}{property}{equals}";
    }

    public bool ReadOnly
    {
        set
        {
            if (value)
            {
                CB_Require.Visible = false;
                CB_Require.SelectedIndex = 1;
            }
            else
            {
                CB_Require.Visible = true;
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using PKHeX.Core;

namespace PKHeX.WinForms
{
    public partial class RibbonEditor : Form
    {
        public RibbonEditor(PKM pk)
        {
            pkm = pk;
            InitializeComponent();
            int vertScrollWidth = SystemInformation.VerticalScrollBarWidth;
            TLP_Ribbons.Padding = FLP_Ribbons.Padding = new Padding(0, 0, vertScrollWidth, 0);
            
            // Updating a Control display with autosized elements on every row addition is cpu intensive. Disable layout updates while populating.
            TLP_Ribbons.SuspendLayout();
            FLP_Ribbons.Scroll += WinFormsUtil.PanelScroll;
            TLP_Ribbons.Scroll += WinFormsUtil.PanelScroll;
            populateRibbons();
            WinFormsUtil.TranslateInterface(this, Main.curlanguage);
            TLP_Ribbons.ResumeLayout();
        }

        private readonly List<RibbonInfo> riblist = new List<RibbonInfo>();
        private readonly PKM pkm;
        private const string PrefixNUD = "NUD_";
        private const string PrefixLabel = "L_";
        private const string PrefixCHK = "CHK_";
        private const string PrefixPB = "PB_";

        private void B_Cancel_Click(object sender, EventArgs e)
        {
            Close();
        }
        private void B_Save_Click(object sender, EventArgs e)
        {
            save();
            Close();
        }

        private void populateRibbons()
        {
            // Get a list of all Ribbon Attributes in the PKM
            var RibbonNames = ReflectUtil.getPropertiesStartWithPrefix(pkm.GetType(), "Ribbon");
            foreach (var RibbonName in RibbonNames)
            {
                object RibbonValue = ReflectUtil.GetValue(pkm, RibbonName);
                if (RibbonValue is int)
                    riblist.Add(new RibbonInfo(RibbonName, (int)RibbonValue));
                if (RibbonValue is bool)
                    riblist.Add(new RibbonInfo(RibbonName, (bool)RibbonValue));
            }
            TLP_Ribbons.ColumnCount = 2;
            TLP_Ribbons.RowCount = 0;
            
            // Add Ribbons
            foreach (var rib in riblist)
            {
                addRibbonSprite(rib);
                addRibbonChoice(rib);
            }
            
            // Force auto-size
            foreach (RowStyle style in TLP_Ribbons.RowStyles)
                style.SizeType = SizeType.AutoSize;
            foreach (ColumnStyle style in TLP_Ribbons.ColumnStyles)
                style.SizeType = SizeType.AutoSize;
        }
        private void addRibbonSprite(RibbonInfo rib)
        {
            PictureBox pb = new PictureBox { AutoSize = false, Size = new Size(40,40), BackgroundImageLayout = ImageLayout.Center, Visible = false, Name = PrefixPB + rib.Name };
            var img = PKMUtil.getRibbonSprite(rib.Name);
            if (img != null)
                pb.BackgroundImage = (Bitmap)img;
            if (img == null)
                return;

            FLP_Ribbons.Controls.Add(pb);
        }
        private void addRibbonChoice(RibbonInfo rib)
        {
            // Get row we add to
            int row = TLP_Ribbons.RowCount;
            TLP_Ribbons.RowCount++;

            var label = new Label
            {
                Anchor = AnchorStyles.Left,
                Name = PrefixLabel + rib.Name,
                Text = rib.Name,
                Padding = Padding.Empty,
                Margin = Padding.Empty,
                AutoSize = true,
            };
            TLP_Ribbons.Controls.Add(label, 1, row);

            if (rib.RibbonCount >= 0) // numeric count ribbon
            {
                var nud = new NumericUpDown
                {
                    Anchor = AnchorStyles.Right,
                    Name = PrefixNUD + rib.Name,
                    Minimum = 0,
                    Width = 35,
                    Increment = 1,
                    Padding = Padding.Empty,
                    Margin = Padding.Empty,
                };
                if (rib.Name.Contains("MemoryContest"))
                    nud.Maximum = 40;
                else if (rib.Name.Contains("MemoryBattle"))
                    nud.Maximum = 8;
                else nud.Maximum = 4; // g3 contest ribbons

                nud.ValueChanged += (sender, e) => 
                {
                    rib.RibbonCount = (int)nud.Value;
                    FLP_Ribbons.Controls[PrefixPB + rib.Name].Visible = rib.RibbonCount > 0;
                    if (nud.Maximum == 4)
                    {
                        string n = rib.Name.Replace("Count", "");
                        switch ((int)nud.Value)
                        {
                            case 2: n += "Super"; break;
                            case 3: n += "Hyper"; break;
                            case 4: n += "Master"; break;
                        }
                        FLP_Ribbons.Controls[PrefixPB + rib.Name].BackgroundImage = (Bitmap)Properties.Resources.ResourceManager.GetObject(n.ToLower());
                    }
                    else if (nud.Maximum == nud.Value)
                        FLP_Ribbons.Controls[PrefixPB + rib.Name].BackgroundImage = (Bitmap)Properties.Resources.ResourceManager.GetObject(rib.Name.ToLower() +"2");
                    else
                        FLP_Ribbons.Controls[PrefixPB + rib.Name].BackgroundImage = (Bitmap)Properties.Resources.ResourceManager.GetObject(rib.Name.ToLower());
                };
                nud.Value = rib.RibbonCount > nud.Maximum ? nud.Maximum : rib.RibbonCount;
                TLP_Ribbons.Controls.Add(nud, 0, row);
            }
            else // boolean ribbon
            {
                var chk = new CheckBox
                {
                    Anchor = AnchorStyles.Right,
                    Name = PrefixCHK + rib.Name,
                    AutoSize = true,
                    Padding = Padding.Empty,
                    Margin = Padding.Empty,
                };
                chk.CheckedChanged += (sender, e) => { rib.HasRibbon = chk.Checked; FLP_Ribbons.Controls[PrefixPB + rib.Name].Visible = rib.HasRibbon; };
                chk.Checked = rib.HasRibbon;
                TLP_Ribbons.Controls.Add(chk, 0, row);

                label.Click += (sender, e) => { chk.Checked ^= true; };
            }
        }
        private void save()
        {
            foreach (var rib in riblist)
                ReflectUtil.SetValue(pkm, rib.Name, rib.RibbonCount < 0 ? rib.HasRibbon : (object) rib.RibbonCount);
        }
        
        private class RibbonInfo
        {
            public readonly string Name;
            public bool HasRibbon;
            public int RibbonCount = -1;
            public RibbonInfo(string name, bool hasRibbon)
            {
                Name = name;
                HasRibbon = hasRibbon;
            }
            public RibbonInfo(string name, int count)
            {
                Name = name;
                RibbonCount = count;
            }
        }

        private void B_All_Click(object sender, EventArgs e)
        {
            foreach (var c in TLP_Ribbons.Controls.OfType<CheckBox>())
                c.Checked = true;
            foreach (var n in TLP_Ribbons.Controls.OfType<NumericUpDown>())
                n.Value = n.Maximum;
        }
        private void B_None_Click(object sender, EventArgs e)
        {
            foreach (var c in TLP_Ribbons.Controls.OfType<CheckBox>())
                c.Checked = false;
            foreach (var n in TLP_Ribbons.Controls.OfType<NumericUpDown>())
                n.Value = 0;
        }
    }
}

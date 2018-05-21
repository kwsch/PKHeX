using System;
using System.Windows.Forms;
using PKHeX.Core;

namespace PKHeX.WinForms
{
    public partial class MemoryAmie : Form
    {
        private readonly string[] args = new string[5];
        private readonly string[] vartypes = new string[5];
        public MemoryAmie(PKM pk)
        {
            InitializeComponent();
            WinFormsUtil.TranslateInterface(this, Main.CurrentLanguage);
            pkm = pk;
            cba = new[] { CB_Country0, CB_Country1, CB_Country2, CB_Country3, CB_Country4 };
            mta = new[] { CB_Region0, CB_Region1, CB_Region2, CB_Region3, CB_Region4, };
            CB_Country0.DisplayMember = CB_Country1.DisplayMember = CB_Country2.DisplayMember = CB_Country3.DisplayMember = CB_Country4.DisplayMember = nameof(ComboItem.Text);
            CB_Country0.ValueMember = CB_Country1.ValueMember = CB_Country2.ValueMember = CB_Country3.ValueMember = CB_Country4.ValueMember = nameof(ComboItem.Value);
            CB_Region0.DisplayMember = CB_Region1.DisplayMember = CB_Region2.DisplayMember = CB_Region3.DisplayMember = CB_Region4.DisplayMember = nameof(ComboItem.Text);
            CB_Region0.ValueMember = CB_Region1.ValueMember = CB_Region2.ValueMember = CB_Region3.ValueMember = CB_Region4.ValueMember = nameof(ComboItem.Value);
            string[] arguments = L_Arguments.Text.Split(new[] {" ; "}, StringSplitOptions.None);

            for (int i = 5; i < Math.Min(arguments.Length, vartypes.Length + 5); i++)
            {
                if (arguments[i] == null) continue;
                vartypes[i - 5] = arguments[i] + ":";
            }
            args[0] = arguments.Length > 0 ? arguments[0] ?? "Disabled" : "Disabled";
            args[1] = arguments.Length > 1 ? arguments[1] ?? "Never left" : "Never left";
            args[2] = arguments.Length > 2 ? arguments[2] ?? "OT" : "OT";
            args[3] = arguments.Length > 3 ? arguments[3] ?? "Past Gen": "Past Gen";
            args[4] = arguments.Length > 4 ? arguments[4] ?? "Memories with" : "Memories with";
            foreach (ComboBox comboBox in cba)
            {
                comboBox.DisplayMember = nameof(ComboItem.Text);
                comboBox.ValueMember = nameof(ComboItem.Value);
                Main.SetCountrySubRegion(comboBox, "countries");
            }
            GetLangStrings();
            LoadFields();
        }

        private bool init;
        private readonly ComboBox[] cba;
        private readonly ComboBox[] mta;
        private readonly PKM pkm;

        // Load/Save Actions
        private void LoadFields()
        {
            // Load the region/country values.
            CB_Country0.SelectedValue = pkm.Geo1_Country;
            CB_Country1.SelectedValue = pkm.Geo2_Country;
            CB_Country2.SelectedValue = pkm.Geo3_Country;
            CB_Country3.SelectedValue = pkm.Geo4_Country;
            CB_Country4.SelectedValue = pkm.Geo5_Country;
            CB_Region0.SelectedValue = pkm.Geo1_Region;
            CB_Region1.SelectedValue = pkm.Geo2_Region;
            CB_Region2.SelectedValue = pkm.Geo3_Region;
            CB_Region3.SelectedValue = pkm.Geo4_Region;
            CB_Region4.SelectedValue = pkm.Geo5_Region;

            // Load the Fullness, and Enjoyment
            M_Fullness.Text = pkm.Fullness.ToString();
            M_Enjoyment.Text = pkm.Enjoyment.ToString();

            // Load the CT Memories
            M_CT_Friendship.Text = pkm.HT_Friendship.ToString();
            M_CT_Affection.Text = pkm.HT_Affection.ToString();
            CB_CTQual.SelectedIndex = Math.Max(0, pkm.HT_Intensity - 1);
            CB_CTMemory.SelectedValue = pkm.HT_Memory;
            CB_CTVar.SelectedValue = pkm.HT_TextVar;
            CB_CTFeel.SelectedIndex = pkm.HT_Feeling;

            // Load the OT Memories
            M_OT_Friendship.Text = pkm.OT_Friendship.ToString();
            M_OT_Affection.Text = pkm.OT_Affection.ToString();
            CB_OTQual.SelectedIndex = Math.Max(0, pkm.OT_Intensity - 1);
            CB_OTMemory.SelectedValue = pkm.OT_Memory;
            CB_OTVar.SelectedValue = pkm.OT_TextVar;
            CB_OTFeel.SelectedIndex = pkm.OT_Feeling;

            CB_Handler.Items.Clear();
            CB_Handler.Items.AddRange(new object[] {$"{pkm.OT_Name} ({args[2]})"}); // OTNAME : OT

            if (Util.TrimFromZero(pkm.HT_Name) != "")
                CB_Handler.Items.AddRange(new object[] { pkm.HT_Name });
            else
                pkm.CurrentHandler = 0;

            tabControl1.SelectedIndex = CB_Handler.SelectedIndex = pkm.CurrentHandler;

            GB_M_OT.Enabled = GB_M_CT.Enabled = GB_Residence.Enabled =
            BTN_Save.Enabled = M_Fullness.Enabled = M_Enjoyment.Enabled =
            L_Fullness.Enabled = L_Enjoyment.Enabled = !pkm.IsEgg;

            if (!pkm.IsEgg)
            {
                bool enable;
                if (pkm.GenNumber < 6)
                {
                    // Previous Generation Mon
                    GB_M_OT.Text = $"{args[3]} {pkm.OT_Name}: {args[2]}"; // Past Gen OT : OTNAME
                    GB_M_CT.Text = $"{args[4]} {pkm.HT_Name}"; // Memories with : HTNAME
                    enable = false;
                    // Reset to no memory -- don't reset affection as ORAS can raise it
                    CB_OTQual.SelectedIndex = CB_OTFeel.SelectedIndex = 0;
                    CB_OTVar.SelectedValue = CB_OTMemory.SelectedValue = 0;
                }
                else
                {
                    enable = true;
                    GB_M_OT.Text = $"{args[4]} {pkm.OT_Name} ({args[2]})"; // Memories with : OTNAME
                    GB_M_CT.Text = $"{args[4]} {pkm.HT_Name}"; // Memories with : HTNAME
                    if (pkm.HT_Name.Length == 0)
                    {
                        CB_Country1.Enabled = CB_Country2.Enabled = CB_Country3.Enabled = CB_Country4.Enabled =
                        CB_Region1.Enabled = CB_Region2.Enabled = CB_Region3.Enabled = CB_Region4.Enabled =
                        GB_M_CT.Enabled = false;
                        GB_M_CT.Text = $"{args[1]} {args[2]} - {args[0]}"; // Never Left : OT : Disabled
                    }
                    else
                        GB_M_CT.Text = $"{args[4]} {pkm.HT_Name}";
                }
                RTB_OT.Visible = CB_OTQual.Enabled = CB_OTMemory.Enabled = CB_OTFeel.Enabled = CB_OTVar.Enabled = enable;
                M_OT_Affection.Enabled = true;
            }
            else
                GB_M_OT.Text = GB_M_CT.Text = $"N/A: {GameInfo.Strings.eggname}";

            init = true;

            // Manually load the Memory Parse
            RTB_CT.Text = GetMemoryString(CB_CTMemory, CB_CTVar, CB_CTQual, CB_CTFeel, pkm.HT_Name);
            RTB_OT.Text = GetMemoryString(CB_OTMemory, CB_OTVar, CB_OTQual, CB_OTFeel, pkm.OT_Name);
        }
        private void SaveFields()
        {
            // Save Region & Country Data
            pkm.Geo1_Region = WinFormsUtil.GetIndex(CB_Region0);
            pkm.Geo2_Region = WinFormsUtil.GetIndex(CB_Region1);
            pkm.Geo3_Region = WinFormsUtil.GetIndex(CB_Region2);
            pkm.Geo4_Region = WinFormsUtil.GetIndex(CB_Region3);
            pkm.Geo5_Region = WinFormsUtil.GetIndex(CB_Region4);
            pkm.Geo1_Country = WinFormsUtil.GetIndex(CB_Country0);
            pkm.Geo2_Country = WinFormsUtil.GetIndex(CB_Country1);
            pkm.Geo3_Country = WinFormsUtil.GetIndex(CB_Country2);
            pkm.Geo4_Country = WinFormsUtil.GetIndex(CB_Country3);
            pkm.Geo5_Country = WinFormsUtil.GetIndex(CB_Country4);

            // Save 0-255 stats
            pkm.HT_Friendship = Util.ToInt32(M_CT_Friendship.Text);
            pkm.HT_Affection = Util.ToInt32(M_CT_Affection.Text);
            pkm.OT_Friendship = Util.ToInt32(M_OT_Friendship.Text);
            pkm.OT_Affection = Util.ToInt32(M_OT_Affection.Text);
            pkm.Fullness = (byte)Util.ToInt32(M_Fullness.Text);
            pkm.Enjoyment = (byte)Util.ToInt32(M_Enjoyment.Text);

            // Save Memories
            pkm.HT_Memory = WinFormsUtil.GetIndex(CB_CTMemory);
            pkm.HT_TextVar = CB_CTVar.Enabled ? WinFormsUtil.GetIndex(CB_CTVar) : 0;
            pkm.HT_Intensity = CB_CTFeel.Enabled ? CB_CTQual.SelectedIndex + 1 : 0;
            pkm.HT_Feeling = CB_CTFeel.Enabled ? CB_CTFeel.SelectedIndex : 0;

            pkm.OT_Memory = WinFormsUtil.GetIndex(CB_OTMemory);
            pkm.OT_TextVar = CB_OTVar.Enabled ? WinFormsUtil.GetIndex(CB_OTVar) : 0;
            pkm.OT_Intensity = CB_OTFeel.Enabled ? CB_OTQual.SelectedIndex + 1 : 0;
            pkm.OT_Feeling = CB_OTFeel.Enabled ? CB_OTFeel.SelectedIndex : 0;
        }

        // Event Actions
        private void B_Save_Click(object sender, EventArgs e)
        {
            SaveFields();
            Close();
        }
        private void B_Cancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void GetLangStrings()
        {
            var strings = GameInfo.Strings.Memories;
            CB_OTMemory.DisplayMember = CB_CTMemory.DisplayMember = nameof(ComboItem.Text);
            CB_OTMemory.ValueMember = CB_CTMemory.ValueMember = nameof(ComboItem.Value);
            CB_OTMemory.DataSource = new BindingSource(strings.Memory, null);
            CB_CTMemory.DataSource = new BindingSource(strings.Memory, null);

            // Quality Chooser
            foreach (var q in strings.GetMemoryQualities())
            {
                CB_CTQual.Items.Add(q);
                CB_OTQual.Items.Add(q);
            }

            // Feeling Chooser
            foreach (var q in strings.GetMemoryFeelings())
            {
                CB_CTFeel.Items.Add(q);
                CB_OTFeel.Items.Add(q);
            }
        }
        private void UpdateMemoryDisplay(object sender)
        {
            int memory = WinFormsUtil.GetIndex((ComboBox) sender);
            var memIndex = Memories.GetMemoryArgType(memory);
            var argvals = GameInfo.Strings.Memories.GetArgumentStrings(memIndex);
            int index = (int) memIndex - 1;
            if (sender == CB_CTMemory)
            {
                CB_CTVar.DisplayMember = nameof(ComboItem.Text);
                CB_CTVar.ValueMember = nameof(ComboItem.Value);
                CB_CTVar.DataSource = new BindingSource(argvals, null);
                if (index >= 0)
                    LCTV.Text = vartypes[index];
                LCTV.Visible = CB_CTVar.Visible = CB_CTVar.Enabled = argvals.Count > 1;
            }
            else
            {
                CB_OTVar.DisplayMember = nameof(ComboItem.Text);
                CB_OTVar.ValueMember = nameof(ComboItem.Value);
                CB_OTVar.DataSource = new BindingSource(argvals, null);
                if (index >= 0)
                    LOTV.Text = vartypes[index];
                LOTV.Visible = CB_OTVar.Visible = CB_OTVar.Enabled = argvals.Count > 1;
            }
        }
        private string GetMemoryString(ComboBox m, Control arg, Control q, Control f, string tr)
        {
            string result;
            bool enabled;
            int mem = WinFormsUtil.GetIndex(m);
            if (mem == 0)
            {
                result = GameInfo.Strings.memories[38];
                enabled = false;
            }
            else
            {
                string nn = pkm.Nickname;
                string a = arg.Text;
                result = string.Format(GameInfo.Strings.memories[mem + 38], nn, tr, a, f.Text, q.Text);
                enabled = true;
            }

            // Show labels if the memory allows for them.
            if (q == CB_CTQual)
                L_CT_Quality.Visible = L_CT_Feeling.Visible = enabled;
            else
                L_OT_Quality.Visible = L_OT_Feeling.Visible = enabled;

            // Show Quality and Feeling.
            q.Visible = q.Enabled = f.Visible = f.Enabled = enabled;

            return result;
        }

        private void ChangeMemory(object sender, EventArgs e)
        {
            ComboBox m = (ComboBox)sender;
            if (m == CB_CTMemory || m == CB_OTMemory)
                UpdateMemoryDisplay(m);

            if (!init) return;
            RTB_OT.Text = GetMemoryString(CB_OTMemory, CB_OTVar, CB_OTQual, CB_OTFeel, pkm.OT_Name);
            RTB_CT.Text = GetMemoryString(CB_CTMemory, CB_CTVar, CB_CTQual, CB_CTFeel, pkm.HT_Name);
        }

        private void ChangeCountryIndex(object sender, EventArgs e)
        {
            int index = Array.IndexOf(cba, sender);
            int val;
            if (sender is ComboBox c && (val = WinFormsUtil.GetIndex(c)) > 0)
            {
                Main.SetCountrySubRegion(mta[index], $"sr_{val:000}");
                mta[index].Enabled = true;
            }
            else
            {
                mta[index].DataSource = new[] { new { Text = "", Value = 0 } };
                mta[index].Enabled = false;
                mta[index].SelectedValue = 0;
            }
        }
        private void ChangeCountryText(object sender, EventArgs e)
        {
            if (!(sender is ComboBox cb) || !string.IsNullOrWhiteSpace(cb.Text))
                return;
            cb.SelectedValue = 0;
            ChangeCountryIndex(sender, e);
        }

        private void Update255_MTB(object sender, EventArgs e)
        {
            if (!(sender is MaskedTextBox tb)) return;
            if (Util.ToInt32(tb.Text) > byte.MaxValue)
                tb.Text = "255";
        }

        private void ClickResetLocation(object sender, EventArgs e)
        {
            Label[] senderarr = { L_Geo0, L_Geo1, L_Geo2, L_Geo3, L_Geo4, };
            int index = Array.IndexOf(senderarr, sender);
            cba[index].SelectedValue = 0;

            mta[index].DisplayMember = nameof(ComboItem.Text);
            mta[index].ValueMember = nameof(ComboItem.Value);
            mta[index].DataSource = new[] { new { Text = "", Value = 0 } };
            mta[index].SelectedValue = 0;
        }

        private void B_ClearAll_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < 5; i++)
                cba[i].SelectedValue = 0;
        }
    }
}
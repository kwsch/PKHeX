using System;
using System.Windows.Forms;
using PKHeX.Core;

namespace PKHeX.WinForms
{
    public partial class MemoryAmie : Form
    {
        private readonly TextMarkup TextArgs;
        private readonly MemoryStrings MemStrings;

        public MemoryAmie(PKM pk)
        {
            InitializeComponent();
            WinFormsUtil.TranslateInterface(this, Main.CurrentLanguage);
            pkm = pk;
            MemStrings = new MemoryStrings(GameInfo.Strings, pkm.Format);
            PrevCountries = new[] { CB_Country0, CB_Country1, CB_Country2, CB_Country3, CB_Country4 };
            PrevRegions = new[] { CB_Region0, CB_Region1, CB_Region2, CB_Region3, CB_Region4, };
            string[] arguments = L_Arguments.Text.Split(new[] {" ; "}, StringSplitOptions.None);

            TextArgs = new TextMarkup(arguments);
            foreach (ComboBox comboBox in PrevCountries)
            {
                comboBox.InitializeBinding();
                Main.SetCountrySubRegion(comboBox, "countries");
            }
            foreach (var region in PrevRegions)
                region.InitializeBinding();
            GetLangStrings();
            LoadFields();

            if (!(pkm is IGeoTrack))
                tabControl1.TabPages.Remove(Tab_Residence);
        }

        private bool init;
        private readonly ComboBox[] PrevCountries;
        private readonly ComboBox[] PrevRegions;
        private readonly PKM pkm;

        // Load/Save Actions
        private void LoadFields()
        {
            // Load the region/country values.
            if (pkm is IGeoTrack g)
            {
                CB_Country0.SelectedValue = g.Geo1_Country;
                CB_Country1.SelectedValue = g.Geo2_Country;
                CB_Country2.SelectedValue = g.Geo3_Country;
                CB_Country3.SelectedValue = g.Geo4_Country;
                CB_Country4.SelectedValue = g.Geo5_Country;
                CB_Region0.SelectedValue  = g.Geo1_Region;
                CB_Region1.SelectedValue  = g.Geo2_Region;
                CB_Region2.SelectedValue  = g.Geo3_Region;
                CB_Region3.SelectedValue  = g.Geo4_Region;
                CB_Region4.SelectedValue  = g.Geo5_Region;
            }

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
            CB_Handler.Items.Add($"{pkm.OT_Name} ({TextArgs.OT})"); // OTNAME : OT

            if (!string.IsNullOrEmpty(pkm.HT_Name))
                CB_Handler.Items.Add(pkm.HT_Name);
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
                    GB_M_OT.Text = $"{TextArgs.PastGen} {pkm.OT_Name}: {TextArgs.OT}"; // Past Gen OT : OTNAME
                    GB_M_CT.Text = $"{TextArgs.MemoriesWith} {pkm.HT_Name}"; // Memories with : HTNAME
                    enable = false;
                    // Reset to no memory -- don't reset affection as ORAS can raise it
                    CB_OTQual.SelectedIndex = CB_OTFeel.SelectedIndex = 0;
                    CB_OTVar.SelectedValue = CB_OTMemory.SelectedValue = 0;
                }
                else
                {
                    enable = true;
                    GB_M_OT.Text = $"{TextArgs.MemoriesWith} {pkm.OT_Name} ({TextArgs.OT})"; // Memories with : OTNAME
                    GB_M_CT.Text = $"{TextArgs.MemoriesWith} {pkm.HT_Name}"; // Memories with : HTNAME
                    if (pkm.HT_Name.Length == 0)
                    {
                        CB_Country1.Enabled = CB_Country2.Enabled = CB_Country3.Enabled = CB_Country4.Enabled =
                        CB_Region1.Enabled = CB_Region2.Enabled = CB_Region3.Enabled = CB_Region4.Enabled =
                        GB_M_CT.Enabled = false;
                        GB_M_CT.Text = $"{TextArgs.NeverLeft} {TextArgs.OT} - {TextArgs.Disabled}"; // Never Left : OT : Disabled
                    }
                    else
                    {
                        GB_M_CT.Text = $"{TextArgs.MemoriesWith} {pkm.HT_Name}";
                    }
                }
                RTB_OT.Visible = CB_OTQual.Enabled = CB_OTMemory.Enabled = CB_OTFeel.Enabled = CB_OTVar.Enabled = enable;
                M_OT_Affection.Enabled = true;
            }
            else
            {
                GB_M_OT.Text = GB_M_CT.Text = $"N/A: {GameInfo.Strings.EggName}";
            }

            init = true;

            // Manually load the Memory Parse
            RTB_CT.Text = GetMemoryString(CB_CTMemory, CB_CTVar, CB_CTQual, CB_CTFeel, pkm.HT_Name);
            RTB_OT.Text = GetMemoryString(CB_OTMemory, CB_OTVar, CB_OTQual, CB_OTFeel, pkm.OT_Name);

            // Affection no longer stored in gen8+, so only show in gen6/7.
            L_OT_Affection.Visible = L_CT_Affection.Visible = M_OT_Affection.Visible = M_CT_Affection.Visible = pkm.Format <= 7;
        }

        private void SaveFields()
        {
            // Save Region & Country Data
            if (pkm is IGeoTrack g)
            {
                g.Geo1_Region = WinFormsUtil.GetIndex(CB_Region0);
                g.Geo2_Region = WinFormsUtil.GetIndex(CB_Region1);
                g.Geo3_Region = WinFormsUtil.GetIndex(CB_Region2);
                g.Geo4_Region = WinFormsUtil.GetIndex(CB_Region3);
                g.Geo5_Region = WinFormsUtil.GetIndex(CB_Region4);
                g.Geo1_Country = WinFormsUtil.GetIndex(CB_Country0);
                g.Geo2_Country = WinFormsUtil.GetIndex(CB_Country1);
                g.Geo3_Country = WinFormsUtil.GetIndex(CB_Country2);
                g.Geo4_Country = WinFormsUtil.GetIndex(CB_Country3);
                g.Geo5_Country = WinFormsUtil.GetIndex(CB_Country4);
            }

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
            var strings = MemStrings;
            CB_OTMemory.InitializeBinding();
            CB_CTMemory.InitializeBinding();
            CB_OTMemory.DataSource = new BindingSource(strings.Memory, null);
            CB_CTMemory.DataSource = new BindingSource(strings.Memory, null);

            // Quality Chooser
            foreach (var q in strings.GetMemoryQualities())
            {
                CB_CTQual.Items.Add(q);
                CB_OTQual.Items.Add(q);
            }

            // Feeling Chooser
            foreach (var q in strings.GetMemoryFeelings(pkm.Format))
            {
                CB_CTFeel.Items.Add(q);
                CB_OTFeel.Items.Add(q);
            }
        }

        private void UpdateMemoryDisplay(object sender)
        {
            if (sender == CB_CTMemory)
            {
                int memory = WinFormsUtil.GetIndex((ComboBox)sender);
                var memIndex = Memories.GetMemoryArgType(memory, pkm.GenNumber);
                var argvals = MemStrings.GetArgumentStrings(memIndex);
                CB_CTVar.InitializeBinding();
                CB_CTVar.DataSource = new BindingSource(argvals, null);
                LCTV.Text = TextArgs.GetMemoryCategory(memIndex, pkm.GenNumber);
                LCTV.Visible = CB_CTVar.Visible = CB_CTVar.Enabled = argvals.Count > 1;
            }
            else
            {
                int memory = WinFormsUtil.GetIndex((ComboBox)sender);
                var memIndex = Memories.GetMemoryArgType(memory, pkm.Format);
                var argvals = MemStrings.GetArgumentStrings(memIndex);
                CB_OTVar.InitializeBinding();
                CB_OTVar.DataSource = new BindingSource(argvals, null);
                LOTV.Text = TextArgs.GetMemoryCategory(memIndex, pkm.Format);
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
                string nn = pkm.Nickname;
                result = string.Format(GameInfo.Strings.memories[mem + 38], nn);
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
            int index = Array.IndexOf(PrevCountries, sender);
            int val;
            if (sender is ComboBox c && (val = WinFormsUtil.GetIndex(c)) > 0)
            {
                Main.SetCountrySubRegion(PrevRegions[index], $"sr_{val:000}");
                PrevRegions[index].Enabled = true;
            }
            else
            {
                PrevRegions[index].DataSource = new[] { new { Text = "", Value = 0 } };
                PrevRegions[index].Enabled = false;
                PrevRegions[index].SelectedValue = 0;
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
            PrevCountries[index].SelectedValue = 0;

            PrevRegions[index].InitializeBinding();
            PrevRegions[index].DataSource = new[] { new { Text = "", Value = 0 } };
            PrevRegions[index].SelectedValue = 0;
        }

        private void B_ClearAll_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < 5; i++)
                PrevCountries[i].SelectedValue = 0;
        }

        private class TextMarkup
        {
            public string Disabled { get; } = nameof(Disabled);
            public string NeverLeft { get; } = "Never left";
            public string OT { get; } = "OT";
            public string PastGen { get; } = "Past Gen";
            public string MemoriesWith { get; } = "Memories with";

            private string Species { get; } = "Species:";
            private string Area { get; } = "Area:";
            private string Item { get; } = "Item:";
            private string Move { get; } = "Move:";
            private string Location { get; } = "Location:";

            public TextMarkup(string[] args)
            {
                Array.Resize(ref args, 10);
                if (args[0] != null) Disabled = args[0];
                if (args[1] != null) NeverLeft = args[1];
                if (args[2] != null) OT = args[2];
                if (args[3] != null) PastGen = args[3];
                if (args[4] != null) MemoriesWith = args[4];

                // Pokémon ; Area ; Item(s) ; Move ; Location
                if (args[5] != null) Species = args[5] + ":";
                if (args[6] != null) Area = args[6] + ":";
                if (args[7] != null) Item = args[7] + ":";
                if (args[8] != null) Move = args[8] + ":";
                if (args[9] != null) Location = args[9] + ":";
            }

            public string GetMemoryCategory(MemoryArgType type, int format)
            {
                return type switch
                {
                    MemoryArgType.GeneralLocation => Area,
                    MemoryArgType.SpecificLocation when format == 6 => Location,
                    MemoryArgType.Species => Species,
                    MemoryArgType.Move => Move,
                    MemoryArgType.Item => Item,
                    _ => string.Empty
                };
            }
        }
    }
}
using System;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using PKHeX.Core;

namespace PKHeX.WinForms
{
    public partial class SAV_SecretBase : Form
    {
        private readonly SaveFile Origin;
        private readonly SAV6AO SAV;

        private byte[,] objdata;
        private byte[,] pkmdata;

        public SAV_SecretBase(SaveFile sav)
        {
            InitializeComponent();
            WinFormsUtil.TranslateInterface(this, Main.CurrentLanguage);
            SAV = (SAV6AO)(Origin = sav).Clone();
            abilitylist = GameInfo.Strings.abilitylist;

            SetupComboBoxes();
            PopFavorite();
            PopFavorite();
            TB_FOT.Font = TB_FT1.Font = TB_FT2.Font = TB_FSay1.Font = TB_FSay2.Font = TB_FSay3.Font = TB_FSay4.Font = LB_Favorite.Font = FontUtil.GetPKXFont();
            CB_Ability.InitializeBinding();

            LB_Favorite.SelectedIndex = 0;
            MT_Flags.Text = SAV.Records.GetRecord(080).ToString(); // read counter; also present in the Secret Base data block

            var offset = GetSecretBaseOffset(0);
            objdata = LoadObjectArray(offset);
            pkmdata = LoadPKMData(0, offset);
            B_SAV2FAV(this, EventArgs.Empty);
        }

        private bool editing;
        private bool loading = true;

        private readonly string[] abilitylist;

        private void SetupComboBoxes()
        {
            CB_Ball.InitializeBinding();
            CB_HeldItem.InitializeBinding();
            CB_Species.InitializeBinding();
            CB_Nature.InitializeBinding();

            CB_Ball.DataSource = new BindingSource(GameInfo.BallDataSource.Where(b => b.Value <= SAV.MaxBallID).ToList(), null);
            CB_HeldItem.DataSource = new BindingSource(GameInfo.ItemDataSource.Where(i => i.Value < SAV.MaxItemID).ToList(), null);
            CB_Species.DataSource = new BindingSource(GameInfo.SpeciesDataSource.Where(s => s.Value <= SAV.MaxSpeciesID).ToList(), null);
            CB_Nature.DataSource = new BindingSource(GameInfo.NatureDataSource, null);

            CB_Move1.InitializeBinding();
            CB_Move2.InitializeBinding();
            CB_Move3.InitializeBinding();
            CB_Move4.InitializeBinding();

            var MoveList = GameInfo.MoveDataSource;
            CB_Move1.DataSource = new BindingSource(MoveList, null);
            CB_Move2.DataSource = new BindingSource(MoveList, null);
            CB_Move3.DataSource = new BindingSource(MoveList, null);
            CB_Move4.DataSource = new BindingSource(MoveList, null);
        }

        // Repopulation Functions
        private void PopFavorite()
        {
            LB_Favorite.Items.Clear();

            int playeroff = SAV.SecretBase + 0x326;
            int favoff = SAV.SecretBase + 0x63A;
            string OT = StringConverter.GetString6(SAV.Data, playeroff + 0x218, 0x1A);
            LB_Favorite.Items.Add($"* {OT}");
            for (int i = 0; i < 30; i++)
            {
                string BaseTrainer = StringConverter.GetString6(SAV.Data, favoff + (i * 0x3E0) + 0x218, 0x1A);
                if (BaseTrainer.Length < 1 || BaseTrainer[0] == '\0')
                    BaseTrainer = "Empty";
                LB_Favorite.Items.Add($"{i} {BaseTrainer}");
            }
        }

        private void B_SAV2FAV(object sender, EventArgs e)
        {
            loading = true;
            int index = LB_Favorite.SelectedIndex;
            if (index < 0)
                return;
            var offset = GetSecretBaseOffset(index);

            var bdata = new SecretBase6(SAV.Data, offset);

            NUD_FBaseLocation.Value = bdata.BaseLocation;

            TB_FOT.Text = bdata.TrainerName;
            TB_FT1.Text = bdata.FlavorText1;
            TB_FT2.Text = bdata.FlavorText2;

            TB_FSay1.Text = bdata.Saying1;
            TB_FSay2.Text = bdata.Saying2;
            TB_FSay3.Text = bdata.Saying3;
            TB_FSay4.Text = bdata.Saying4;

            // Gather data for Object Array
            objdata = LoadObjectArray(offset);

            NUD_FObject.Value = 1; // Trigger Update
            ChangeObjectIndex(this, EventArgs.Empty);

            GB_PKM.Enabled = index > 0;

            // Trainer Pokemon
            pkmdata = LoadPKMData(index, offset);

            NUD_FPKM.Value = 1;
            ChangeFavPKM(this, EventArgs.Empty); // Trigger Update

            loading = false;
            B_Import.Enabled = B_Export.Enabled = index > 0;
            currentIndex = index;
        }

        private byte[,] LoadPKMData(int index, int offset)
        {
            var result = new byte[3, 0x34];
            if (index <= 0)
                return result;
            for (int i = 0; i < 3; i++)
            {
                for (int z = 0; z < 0x34; z++)
                    result[i, z] = SAV.Data[offset + 0x32E + (0x34 * i) + z];
            }
            return result;
        }

        private byte[,] LoadObjectArray(int offset)
        {
            byte[] data = SAV.Data;
            var result = new byte[25, 12];
            for (int i = 0; i < 25; i++)
            {
                for (int z = 0; z < 12; z++)
                    result[i, z] = data[offset + 2 + (12 * i) + z];
            }

            return result;
        }

        private int GetSecretBaseOffset(int index)
        {
            // OR/AS: Secret base @ 0x23A00
            if (index == 0) // Self, 0x314 bytes? Doesn't store pokemon data
                return SAV.SecretBase + 0x326;

            --index;
            // Received
            return SAV.SecretBase + 0x63A + (index * SecretBase6.SIZE);
        }

        private void B_FAV2SAV(object sender, EventArgs e)
        {
            // Write data back to save
            int index = LB_Favorite.SelectedIndex; // store for restoring
            if (!GB_PKM.Enabled && index > 0)
            { WinFormsUtil.Error("Sorry, no overwriting someone else's base with your own data."); return; }
            if (GB_PKM.Enabled && index == 0)
            { WinFormsUtil.Error("Sorry, no overwriting of your own base with someone else's."); return; }

            var name = LB_Favorite.Items[index].ToString();
            if (name == "* " || name == $"{index} Empty")
            { WinFormsUtil.Error("Sorry, no overwriting an empty base with someone else's."); return; }
            if (index < 0)
                return;
            int offset = GetSecretBaseOffset(index);

            var bdata = new SecretBase6(SAV.Data, offset);

            int baseloc = (int)NUD_FBaseLocation.Value;
            if (baseloc < 3)
                baseloc = 0; // skip 1/2 baselocs as they are dummied out ingame.
            bdata.BaseLocation = baseloc;

            bdata.TrainerName = TB_FOT.Text;
            bdata.FlavorText1 = TB_FT1.Text;
            bdata.FlavorText2 = TB_FT2.Text;

            bdata.Saying1 = TB_FSay1.Text;
            bdata.Saying2 = TB_FSay2.Text;
            bdata.Saying3 = TB_FSay3.Text;
            bdata.Saying4 = TB_FSay4.Text;

            // Copy back Objects
            for (int i = 0; i < 25; i++)
            {
                for (int z = 0; z < 12; z++)
                    SAV.Data[offset + 2 + (12 * i) + z] = objdata[i, z];
            }

            if (GB_PKM.Enabled) // Copy pkm data back in
            {
                SaveFavPKM();
                for (int i = 0; i < 3; i++)
                {
                    for (int z = 0; z < 0x34; z++)
                        SAV.Data[offset + 0x32E + (0x34 * i) + z] = pkmdata[i, z];
                }
            }
            PopFavorite();
            LB_Favorite.SelectedIndex = currentIndex = index;
        }

        private int currentIndex;

        // Button Specific
        private void B_Cancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void B_Save_Click(object sender, EventArgs e)
        {
            uint flags = Util.ToUInt32(MT_Flags.Text);
            SAV.Records.SetRecord(080, (int)flags);
            Array.Copy(BitConverter.GetBytes(flags), 0, SAV.Data, SAV.SecretBase + 0x62C, 4); // write counter
            Origin.CopyChangesFrom(SAV);
            Close();
        }

        private void B_GiveDecor_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < 173; i++)
            {
                // int qty = BitConverter.ToUInt16(sav, offset + i * 4);
                // int has = BitConverter.ToUInt16(sav, offset + i * 4 + 2);

                SAV.Data[SAV.SecretBase + (i * 4)] = 25;
                SAV.Data[SAV.SecretBase + (i * 4) + 2] = 1;
            }
        }

        private void ChangeObjectIndex(object sender, EventArgs e)
        {
            int objindex = (int)NUD_FObject.Value - 1;
            byte[] objinfo = new byte[12];
            for (int i = 0; i < 12; i++)
                objinfo[i] = objdata[objindex, i];

            // Array with object data acquired. Fill data.
            int val = objinfo[0]; if (val == 0xFF) val = -1;
            byte x = objinfo[2];
            byte y = objinfo[4];
            byte rot = objinfo[6];
            // byte unk1 = objinfo[7];
            // ushort unk2 = BitConverter.ToUInt16(objinfo, 0x8);

            // Set values to display
            editing = true;

            NUD_FObjType.Value = val;
            NUD_FX.Value = x;
            NUD_FY.Value = y;
            NUD_FRot.Value = rot;

            editing = false;
        }

        private void ChangeObjectQuality(object sender, EventArgs e)
        {
            if (editing) return;

            int objindex = (int)NUD_FObject.Value - 1;

            byte val = (byte)NUD_FObjType.Value;
            byte x = (byte)NUD_FX.Value;
            byte y = (byte)NUD_FY.Value;
            byte rot = (byte)NUD_FRot.Value;

            objdata[objindex, 0] = val;
            objdata[objindex, 2] = x;
            objdata[objindex, 4] = y;
            objdata[objindex, 6] = rot;
        }

        private int currentpkm;

        private void ChangeFavPKM(object sender, EventArgs e)
        {
            int index = (int)NUD_FPKM.Value;
            SaveFavPKM(); // Save existing PKM
            currentpkm = index;
            LoadFavPKM();
        }

        private void SaveFavPKM()
        {
            if (loading || !GB_PKM.Enabled) return;
            int index = currentpkm;
            byte[] pkm = new byte[0x34];

            BitConverter.GetBytes(Util.GetHexValue(TB_EC.Text)).CopyTo(pkm, 0);
            BitConverter.GetBytes((ushort)WinFormsUtil.GetIndex(CB_Species)).CopyTo(pkm, 8);
            BitConverter.GetBytes((ushort)WinFormsUtil.GetIndex(CB_HeldItem)).CopyTo(pkm, 0xA);
            pkm[0xC] = (byte)Array.IndexOf(abilitylist, CB_Ability.Text.Remove(CB_Ability.Text.Length - 4));
            pkm[0xD] = (byte)(CB_Ability.SelectedIndex << 1);
            pkm[0x14] = (byte)WinFormsUtil.GetIndex(CB_Nature);

            int fegform = 0;
            fegform += PKX.GetGenderFromString(Label_Gender.Text) << 1;
            fegform += CB_Form.SelectedIndex << 3;
            pkm[0x15] = (byte)fegform;

            pkm[0x16] = (byte)Math.Min(Convert.ToInt32(TB_HPEV.Text), 252);
            pkm[0x17] = (byte)Math.Min(Convert.ToInt32(TB_ATKEV.Text), 252);
            pkm[0x18] = (byte)Math.Min(Convert.ToInt32(TB_DEFEV.Text), 252);
            pkm[0x19] = (byte)Math.Min(Convert.ToInt32(TB_SPAEV.Text), 252);
            pkm[0x1A] = (byte)Math.Min(Convert.ToInt32(TB_SPDEV.Text), 252);
            pkm[0x1B] = (byte)Math.Min(Convert.ToInt32(TB_SPEEV.Text), 252);

            BitConverter.GetBytes((ushort)WinFormsUtil.GetIndex(CB_Move1)).CopyTo(pkm, 0x1C);
            BitConverter.GetBytes((ushort)WinFormsUtil.GetIndex(CB_Move2)).CopyTo(pkm, 0x1E);
            BitConverter.GetBytes((ushort)WinFormsUtil.GetIndex(CB_Move3)).CopyTo(pkm, 0x20);
            BitConverter.GetBytes((ushort)WinFormsUtil.GetIndex(CB_Move4)).CopyTo(pkm, 0x22);

            pkm[0x24] = (byte)CB_PPu1.SelectedIndex;
            pkm[0x25] = (byte)CB_PPu2.SelectedIndex;
            pkm[0x26] = (byte)CB_PPu3.SelectedIndex;
            pkm[0x27] = (byte)CB_PPu4.SelectedIndex;

            pkm[0x28] = (byte)(Convert.ToByte(TB_HPIV.Text) & 0x1F);
            pkm[0x29] = (byte)(Convert.ToByte(TB_ATKIV.Text) & 0x1F);
            pkm[0x2A] = (byte)(Convert.ToByte(TB_DEFIV.Text) & 0x1F);
            pkm[0x2B] = (byte)(Convert.ToByte(TB_SPAIV.Text) & 0x1F);
            pkm[0x2C] = (byte)(Convert.ToByte(TB_SPDIV.Text) & 0x1F);
            pkm[0x2D] = (byte)(Convert.ToByte(TB_SPEIV.Text) & 0x1F);
            int shiny = (CHK_Shiny.Checked? 1 : 0) << 6;
            pkm[0x2D] |= (byte)shiny;

            pkm[0x2E] = Convert.ToByte(TB_Friendship.Text);
            pkm[0x2F] = (byte)WinFormsUtil.GetIndex(CB_Ball);
            pkm[0x30] = Convert.ToByte(TB_Level.Text);

            for (int i = 0; i < 0x34; i++) // Copy data back to storage.
                pkmdata[index - 1, i] = pkm[i];
        }

        private void LoadFavPKM()
        {
            int index = currentpkm - 1;
            byte[] fpkm = new byte[0x34];
            for (int i = 0; i < 0x34; i++)
                fpkm[i] = pkmdata[index, i];

            uint ec = BitConverter.ToUInt32(fpkm, 0);
            // uint unk = BitConverter.ToUInt32(fpkm, 4);
            int species = BitConverter.ToInt16(fpkm, 8);
            int item = BitConverter.ToInt16(fpkm, 0xA);
            // int abil = fpkm[0xC];
            int abil_no = fpkm[0xD];
            MT_AbilNo.Text = abil_no.ToString();
            // 6 unknown bytes, contest?

            int nature = fpkm[0x14];
            byte genform = fpkm[0x15];
            genderflag = genform >> 1 & 0x3;
            SetGenderLabel();

            byte HP_EV = fpkm[0x16];
            byte AT_EV = fpkm[0x17];
            byte DE_EV = fpkm[0x18];
            byte SA_EV = fpkm[0x19];
            byte SD_EV = fpkm[0x1A];
            byte SP_EV = fpkm[0x1B];

            int move1 = BitConverter.ToInt16(fpkm, 0x1C);
            int move2 = BitConverter.ToInt16(fpkm, 0x1E);
            int move3 = BitConverter.ToInt16(fpkm, 0x20);
            int move4 = BitConverter.ToInt16(fpkm, 0x22);

            byte ppu1 = fpkm[0x24];
            byte ppu2 = fpkm[0x25];
            byte ppu3 = fpkm[0x26];
            byte ppu4 = fpkm[0x27];

            byte HP_IV = fpkm[0x28];
            byte AT_IV = fpkm[0x29];
            byte DE_IV = fpkm[0x2A];
            byte SA_IV = fpkm[0x2B];
            byte SD_IV = fpkm[0x2C];
            byte SP_IV = fpkm[0x2D];

            bool isshiny = (SP_IV & 0x40) > 0;
            SP_IV &= 0x1F;

            byte friendship = fpkm[0x2E];
            int ball = fpkm[0x2F];
            byte level = fpkm[0x30];

            // Put data into fields.
            TB_EC.Text = ec.ToString("X8");
            CB_Species.SelectedValue = species;
            CB_HeldItem.SelectedValue = item;

            CB_Nature.SelectedValue = nature;
            CB_Ball.SelectedValue = ball;

            TB_HPIV.Text = HP_IV.ToString();
            TB_ATKIV.Text = AT_IV.ToString();
            TB_DEFIV.Text = DE_IV.ToString();
            TB_SPAIV.Text = SA_IV.ToString();
            TB_SPDIV.Text = SD_IV.ToString();
            TB_SPEIV.Text = SP_IV.ToString();

            TB_HPEV.Text = HP_EV.ToString();
            TB_ATKEV.Text = AT_EV.ToString();
            TB_DEFEV.Text = DE_EV.ToString();
            TB_SPAEV.Text = SA_EV.ToString();
            TB_SPDEV.Text = SD_EV.ToString();
            TB_SPEEV.Text = SP_EV.ToString();

            TB_Friendship.Text = friendship.ToString();
            TB_Level.Text = level.ToString();

            CB_Move1.SelectedValue = move1;
            CB_Move2.SelectedValue = move2;
            CB_Move3.SelectedValue = move3;
            CB_Move4.SelectedValue = move4;
            CB_PPu1.SelectedIndex = ppu1;
            CB_PPu2.SelectedIndex = ppu2;
            CB_PPu3.SelectedIndex = ppu3;
            CB_PPu4.SelectedIndex = ppu4;

            CHK_Shiny.Checked = isshiny;

            // Set Form
            SetForms();
            int form = genform >> 3;
            CB_Form.SelectedIndex = form;

            // Set Ability
            SetAbilityList();
        }

        private void SetAbilityList()
        {
            int abilityIndex = Convert.ToInt16(MT_AbilNo.Text) >> 1;
            int species = WinFormsUtil.GetIndex(CB_Species);
            int form = CB_Form.SelectedIndex;
            var abilities = PersonalTable.AO.GetFormEntry(species, form).Abilities;

            CB_Ability.DataSource = GameInfo.FilteredSources.GetAbilityList(abilities, 6);
            CB_Ability.SelectedIndex = abilityIndex < 3 ? abilityIndex : 0;
        }

        private void SetForms()
        {
            int species = WinFormsUtil.GetIndex(CB_Species);
            bool hasForms = FormInfo.HasFormSelection(PersonalTable.AO[species], species, 6);
            CB_Form.Enabled = CB_Form.Visible = hasForms;

            CB_Form.InitializeBinding();
            CB_Form.DataSource = FormConverter.GetFormList(species, GameInfo.Strings.types, GameInfo.Strings.forms, Main.GenderSymbols, SAV.Generation);
        }

        private void UpdateSpecies(object sender, EventArgs e)
        {
            // Get Forms for Given Species
            SetForms();

            // Check for Gender Changes
            // Get Gender Threshold
            int gt = SAV.Personal[WinFormsUtil.GetIndex(CB_Species)].Gender;

            if (gt == 255)      // Genderless
                genderflag = 2;
            else if (gt == 254) // Female Only
                genderflag = 1;
            else if (gt == 0) // Male Only
                genderflag = 0;

            SetGenderLabel();
            SetAbilityList();
        }

        private void UpdateForm(object sender, EventArgs e)
        {
            SetAbilityList();

            // If form has a single gender, account for it.
            if (PKX.GetGenderFromString(CB_Form.Text) < 2)
                Label_Gender.Text = Main.GenderSymbols[CB_Form.SelectedIndex];
        }

        private int genderflag;

        private void Label_Gender_Click(object sender, EventArgs e)
        {
            var species = WinFormsUtil.GetIndex(CB_Species);
            var pi = SAV.Personal[species];
            var fg = pi.FixedGender;
            if (fg == -1) // dual gender
            {
                fg = PKX.GetGenderFromString(Label_Gender.Text);
                fg = (fg ^ 1) & 1;
            }
            Label_Gender.Text = Main.GenderSymbols[fg];
        }

        private void SetGenderLabel()
        {
            Label_Gender.Text = Main.GenderSymbols[genderflag];
        }

        private void B_FDelete_Click(object sender, EventArgs e)
        {
            if (LB_Favorite.SelectedIndex < 1) { WinFormsUtil.Alert("Cannot delete your Secret Base."); return; }
            int index = LB_Favorite.SelectedIndex - 1;

            int favoff = SAV.SecretBase + 0x63A;
            string BaseTrainer = StringConverter.GetString6(SAV.Data, favoff + (index * 0x3E0) + 0x218, 0x1A);
            if (string.IsNullOrEmpty(BaseTrainer))
                BaseTrainer = "Empty";

            if (DialogResult.Yes != WinFormsUtil.Prompt(MessageBoxButtons.YesNo, $"Delete {BaseTrainer}'s base (Entry {index}) from your records?"))
                return;

            const int max = 29;
            const int size = SecretBase6.SIZE;
            int offset = favoff + (index * size);
            if (index != max) Array.Copy(SAV.Data, offset + size, SAV.Data, offset, size * (max - index));
            // Ensure Last Entry is Cleared
            Array.Copy(new byte[size], 0, SAV.Data, size * max, size);
            PopFavorite();
        }

        private void B_Import_Click(object sender, EventArgs e)
        {
            using var ofd = new OpenFileDialog();
            if (ofd.ShowDialog() != DialogResult.OK)
                return;
            var path = ofd.FileName;
            if (new FileInfo(path).Length != SecretBase6.SIZE)
                return;
            var ofs = GetSecretBaseOffset(currentIndex);
            var data = File.ReadAllBytes(path);
            SAV.SetData(data, ofs);
            PopFavorite();
            LB_Favorite.SelectedIndex = currentIndex;
            B_SAV2FAV(sender, e); // load back from current index
        }

        private void B_Export_Click(object sender, EventArgs e)
        {
            LB_Favorite.SelectedIndex = currentIndex;
            B_FAV2SAV(sender, e); // save back to current index
            var ofs = GetSecretBaseOffset(currentIndex);
            var sb = new SecretBase6(SAV.Data, ofs);
            var tr = sb.TrainerName;
            if (string.IsNullOrWhiteSpace(tr))
                tr = "Trainer";
            using var sfd = new SaveFileDialog {Filter = "Secret Base Data|*.sb6", FileName = $"{sb.BaseLocation:D2} - {Util.CleanFileName(tr)}.sb6"};
            if (sfd.ShowDialog() != DialogResult.OK)
                return;

            var path = sfd.FileName;
            var data = SAV.GetData(ofs, SecretBase6.SIZE);
            File.WriteAllBytes(path, data);
        }
    }
}

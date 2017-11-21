using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using PKHeX.Core;

namespace PKHeX.WinForms
{
    public partial class SAV_FestivalPlaza : Form
    {
        private readonly SaveFile Origin;
        private readonly SAV7 SAV;
        public SAV_FestivalPlaza(SaveFile sav)
        {
            SAV = (SAV7)(Origin = sav).Clone();
            editing = true;
            InitializeComponent();
            typeMAX = SAV.USUM ? 0x7F : 0x7C;
            if (SAV.USUM)
            {
                PBs = new PictureBox[3] { ppkx1, ppkx2, ppkx3 };
                LoadBattleAgency();
            }
            else
                TC_Editor.TabPages.Remove(Tab_BattleAgency);
            if (Main.Unicode)
                try { TB_OTName.Font = FontUtil.GetPKXFont(11); }
                catch (Exception e) { WinFormsUtil.Alert("Font loading failed...", e.ToString()); }
            uint cc = SAV.FestaCoins;
            uint cu = SAV.UsedFestaCoins;
            NUD_FC_Current.Value = Math.Min(cc, NUD_FC_Current.Maximum);
            NUD_FC_Used.Value = Math.Min(cu, NUD_FC_Used.Maximum);
            L_FC_CollectedV.Text = (cc + cu).ToString();
            string[] res;
            switch (Main.CurrentLanguage)
            {
                case "ja":
                    res = new[] {
                        "おじさんの きんのたま だからね！","かがくの ちからって すげー","1 2の …… ポカン！","おーす！ みらいの チャンピオン！","おお！ あんたか！","みんな げんきに なりましたよ！","とっても 幸せそう！","なんでも ないです","いあいぎりで きりますか？","レポートを かきこんでいます",
                        "…… ぼくも もう いかなきゃ！","ボンジュール！","バイビー！","ばか はずれです……","やけどなおしの よういは いいか！","ウー！ ハーッ！","ポケモンは たたかわせるものさ","ヤドランは そっぽを むいた！","マサラは まっしろ はじまりのいろ","10000こうねん はやいんだよ！","おーい！ まてー！ まつんじゃあ！","こんちわ！ ぼく ポケモン……！","っだと こらあ！","ぐ ぐーッ！ そんな ばかなーッ！","みゅう！","タチサレ…… タチサレ……",
                        "カイリュー はかいこうせん","どっちか 遊んでくれないか？","ぬいぐるみ かっておいたわよ","ひとのこと じろじろ みてんなよ","なんのことだか わかんない","みんな ポケモン やってるやん","きょうから 24時間 とっくんだ！","あたいが ホンモノ！","でんげきで いちころ……","スイクンを おいかけて 10ねん","かんどうが よみがえるよ！","われわれ ついに やりましたよー！","ヤドンのシッポを うるなんて……","ショオーッ!!","ギャーアアス!!","だいいっぽを ふみだした！",
                        "いちばん つよくて すごいんだよね","にくらしいほど エレガント！","そうぞうりょくが たりないよ","キミは ビッグウェーブ！","おまえさんには しびれた わい","なに いってんだろ…… てへへ……","ぬいぐるみ なんか かってないよ","ここで ゆっくり して おいき！","はじけろ！ ポケモン トレーナー！","はいが はいに はいった……","…できる！","ぶつかった かいすう 5かい！","たすけて おくれーっ!!","マボロシじま みえんのう……","ひゅああーん！","しゅわーん！",
                        "あつい きもち つたわってくる！","こいつが！ おれの きりふだ！","ひとりじめとか そういうの ダメよ！","ワーオ！ ぶんせきどーり！","ぱるぱるぅ!!!","グギュグバァッ!!!","ばっきん 100まんえん な！","オレ つよくなる……","ながれる 時間は とめられない！","ぜったいに お願いだからね","きみたちから はどうを かんじる！","あたしのポケモンに なにすんのさ！","リングは おれの うみ～♪","オレの おおごえの ひとりごとを","そう コードネームは ハンサム！","……わたしが まけるかも だと!?",
                        "やめたげてよぉ！","ブラボー！ スーパー ブラボー！","ボクは チャンピオンを こえる","オレは いまから いかるぜッ！","ライモンで ポケモン つよいもん","キミ むしポケモン つかいなよ","ストップ！","ひとよんで メダルおやじ！","トレーナーさんも がんばれよ！","おもうぞんぶん きそおーぜ！","プラズマズイ！","ワタクシを とめることは できない！","けいさんずみ ですとも！","ババリバリッシュ！","ンバーニンガガッ！","ヒュラララ！",
                        "お友達に なっちゃお♪","じゃあ みんな またねえ！","このひとたち ムチャクチャです……","トレーナーとは なにか しりたい","スマートに くずれおちるぜ","いのち ばくはつッ!!","いいんじゃない いいんじゃないの！","あれだよ あれ おみごとだよ！","ぜんりょくでいけー！ ってことよ！","おまちなさいな！","つまり グッド ポイント なわけ！","ざんねん ですが さようなら","にくすぎて むしろ 好きよ","この しれもの が！","イクシャア!!","イガレッカ!!",
                        "フェスサークル ランク 100！",
                    };
                    break;
                default:
                    string musical8note = char.ConvertFromUtf32(0x266A);
                    string linedP = char.ConvertFromUtf32(0x20BD);//currency Ruble
                    res = new[] { //source:UltraMoon
                        /* (SM)Pokémon House */"There's nothing funny about Nuggets.","The Power of science is awesome.","1, 2, and... Ta-da!","How's the future Champ today?","Why, you!","There! All happy and healthy!","Your Pokémon seems to be very happy!","No thanks!","Would you like to use Cut?","Saving...",
                        /* (SM)Kanto Tent */"Well, I better get going!","Bonjour!","Smell ya later!","Sorry! Bad call!","You better have Burn Heal!","Hoo hah!","Pokémon are for battling!","Slowbro took a snooze...","Shades of your journey await!","You're 10,000 light-years from facing Brock!","Hey! Wait! Don't go out!","Hiya! I'm a Pokémon...","What do you want?","WHAT! This can't be!","Mew!","Be gone... Intruders...",
                        /* (SM)Joht Tent */"Dragonite, Hymer Beam.","Spread the fun around.","I bought an adorable doll with your money.","What are you staring at?","I just don't understand.","Everyone is into Pokémon.","I'm going to train 24 hours a day!","I'm the real deal!","With a jolt of electricity...","For 10 years I chased Suicune.","I am just so deeply moved!","We have finally made it!","...But selling Slowpoke Tails?","Shaoooh!","Gyaaas!","you've taken your first step!",
                        /* (SM)Hoenn Tent */"I'm just the strongest there is right now.","And confoundedly elegant!","You guys need some imagination.","You made a much bigger splash!","You ended up giving me a thrill!","So what am I talking about...","I'm not buying any Dolls.","Take your time and rest up!","Have a blast, Pokémon Trainers!","I got ashes in my eyelashes!","You're sharp!","Number of collisions: 5 times!","Please! Help me out!","I can't see Mirage Island today...","Hyahhn!","Shwahhn!",
                        /* (SM)Sinnoh Tent */"Your will is overwhelming me!","This is it! My trump card!","Trying to monopolize Pokémon just isn't...","See? Just as analyzed.","Gagyagyaah!","Gugyugubah!","It's a "+linedP+"10 million fine if you're late!","I'm going to get tougher...","You'll never be able to stem the flow of time!","Please come!","Your team! I sense your strong aura!","What do you think you're doing?!","The ring is my rolling sea. "+musical8note,"I was just thinking out loud.","My code name, it is Looker.","It's not possible that I lose!",
                        /* (SM)Unova Tent */"Knock it off!","Bravo! Excellent!!","I'll defeat the Champion.","You're about to feel my rage!","Nimbasa's Pokémon can dance a nimble bossa!","Use Bug-type Pokémon!","Stop!","People call me Mr. Medal!","Trainer, do your best, too!","See who's stronger!","Plasbad, for short!","I won't allow anyone to stop me!","I was expecting exactly that kind of move!","Bazzazzazzash!","Preeeeaah!","Haaahraaan!",
                        /* (SM)Kalos Tent */"We'll become friends. "+musical8note,"I'll see you all later!","These people have a few screws loose...","I want to know what a \"Trainer\" is.","When I lose, I go out in style!","Let's give it all we've got!","Fantastic! Just fantastic!","Outstanding!","Try as hard as possible!","Stop right there!","That really hit me right here...","But this is adieu to you all.","You're just too much, you know?","Fool! You silly, unseeing child!","Xsaaaaaah!","Yvaaaaaar!",
                        "I reached Festival Plaza Rank 100!",
                    };
                    break;
            }
            CLB_Phrases.Items.Clear();
            CLB_Phrases.Items.Add(res.Last(), SAV.GetFestaPhraseUnlocked(106)); //add Lv100 before TentPhrases
            for (int i = 0; i < res.Length - 1; i++)
                CLB_Phrases.Items.Add(res[i], SAV.GetFestaPhraseUnlocked(i));

            DateTime dt = SAV.FestaDate ?? new DateTime(2000, 1, 1);
            CAL_FestaStartDate.Value = dt;
            CAL_FestaStartTime.Value = dt;

            string[] res2 = { "Rank 4: missions","Rank 8: facility","Rank 10: fashion","Rank 20: rename","Rank 30: special menu","Rank 40: BGM","Rank 50: theme Glitz","Rank 60: theme Fairy","Rank 70: theme Tone","Rank 100: phrase","Current Rank", };
            CLB_Reward.Items.Clear();
            CLB_Reward.Items.Add(res2.Last(), (CheckState)r[SAV.GetFestPrizeReceived(10)]); //add CurrentRank before const-rewards
            for (int i = 0; i < res2.Length - 1; i++)
                CLB_Reward.Items.Add(res2[i], (CheckState)r[SAV.GetFestPrizeReceived(i)]);

            for (int i = 0; i < 7; i++)
                f[i] = new FestaFacility(SAV, i);

            string[] res3 = { "Meet", "Part", "Moved", "Disappointed" };
            CB_FacilityMessage.Items.Clear();
            CB_FacilityMessage.Items.AddRange(res3);
            string[] res5 = { "Ace Trainer", "Ace Trainer", "Veteran", "Veteran", "Office Worker", "Office Worker", "Punk Guy", "Punk Girl", "Breeder", "Breeder", "Youngster", "Lass" };
            CB_FacilityNPC.Items.Clear();
            for(int i = 0; i < res5.Length; i++)
            {
                switch (i)
                {
                    case 1:
                    case 3:
                    case 4:
                    case 8: CB_FacilityNPC.Items.Add(res5[i] + gendersymbols[0]); break;
                    case 0:
                    case 2:
                    case 5:
                    case 9: CB_FacilityNPC.Items.Add(res5[i] + gendersymbols[1]); break;
                    default: CB_FacilityNPC.Items.Add(res5[i]); break;
                }
            }
            string[] res6 = { "Lottery", "Haunted", "Goody", "Food", "Bouncy", "Fortune", "Dye","Exchange" };
            string[][] res7 = {
                new[]{"BigDream","GoldRush","TreasureHunt"},
                new[]{"GhostsDen","TrickRoom","ConfuseRay"},
                new[]{"Ball","General","Battle","SoftDrink","Pharmacy"},
                new[]{"Rare","Battle", "FriendshipCafé", "FriendshipParlor"},
                new[]{"Thump","Clink","Stomp"},
                new[]{"Kanto","Johto","Hoenn","Sinnoh","Unova","Kalos","Pokémon"},
                new[]{"Red","Yellow","Green","Blue","Orange","NavyBlue","Purple","Pink"},
                new[]{"Switcheroo"}
            };
            CB_FacilityType.Items.Clear();
            for (int k = 0; k < RES_FacilityLevelType.Length - (SAV.USUM ? 0 : 1); k++) //Exchange is USUM only
                for (int j = 0; j < RES_FacilityLevelType[k].Length; j++)
                {
                    if (RES_FacilityLevelType[k][j] != 4)
                        for (int i = 0; i < RES_FacilityLevelType[k][j]; i++)
                            CB_FacilityType.Items.Add(res6[k] + " " + res7[k][j] + " " + (i + 1));
                    else
                    {
                        CB_FacilityType.Items.Add(res6[k] + " " + res7[k][j] + " 1");
                        CB_FacilityType.Items.Add(res6[k] + " " + res7[k][j] + " 3");
                        CB_FacilityType.Items.Add(res6[k] + " " + res7[k][j] + " 5");
                    }
                }

            NUD_Rank.Value = SAV.FestaRank;
            LoadRankLabel(SAV.FestaRank);
            NUDs = new NumericUpDown[4] { NUD_MyMessageMeet, NUD_MyMessagePart, NUD_MyMessageMoved, NUD_MyMessageDissapointed };
            for (int i = 0; i < NUDs.Length; i++)
                NUDs[i].Value = SAV.GetFestaMessage(i);

            LB_FacilityIndex.SelectedIndex = 0;
            CB_FacilityMessage.SelectedIndex = 0;
            editing = false;

            entry = 0;
            LoadFacility();
        }
        private bool editing;
        private readonly byte[] r = { 0, 2, 1 }; // CheckState.Indeterminate <-> CheckState.Checked
        private readonly int typeMAX;
        private readonly FestaFacility[] f = new FestaFacility[7];
        private readonly string[] RES_Color = { "Red", "Blue", "Gold", "Black", "Purple", "Yellow", "Brown", "Green", "Orange", "NavyBlue", "Pink", "White" };
        private readonly byte[][] RES_FacilityColor = //facility appearance
        {
            new byte[]{0,1,2,3},//Lottery
            new byte[]{4,0,5,3},//Haunted
            new byte[]{1,0,5,3},//Goody
            new byte[]{6,7,0,3},//Food
            new byte[]{4,5,8,3},//Bouncy
            new byte[]{0,1,2,3},//Fortune
            new byte[]{0,7,8,4,5,1,9,10},//Dye
            new byte[]{11,1,5,3},//Exchange
        };
        private readonly byte[][] RES_FacilityLevelType = //3:123 4:135 5:12345
        {
            new byte[]{5,5,5},
            new byte[]{5,5,5},
            new byte[]{3,5,3,3,3},
            new byte[]{5,4,5,5},
            new byte[]{5,5,5},
            new byte[]{4,4,4,4,4,4,4},
            new byte[]{4,4,4,4,4,4,4,4},
            new byte[]{3}
        };
        private int TypeIndexToType(int typeIndex)
        {
            if (typeIndex < 0 || typeIndex > typeMAX) return -1;
            if (typeIndex < 0x0F) return 0;
            if (typeIndex < 0x1E) return 1;
            if (typeIndex < 0x2F) return 2;
            if (typeIndex < 0x41) return 3;
            if (typeIndex < 0x50) return 4;
            if (typeIndex < 0x65) return 5;
            if (typeIndex < 0x7D) return 6;
            return 7;
        }
        private int getColorCount(int i) =>
                i >= 0 && i < RES_FacilityColor.Length - (SAV.USUM ? 0 : 1)
                ? RES_FacilityColor[i].Length - 1
                : 3;

        private int entry = -1;
        private void LoadFacility()
        {
            editing = true;
            var facility = f[entry];
            CB_FacilityType.SelectedIndex =
                CB_FacilityType.Items.Count > facility.Type
                ? facility.Type
                : -1;
            int type = TypeIndexToType(CB_FacilityType.SelectedIndex);
            NUD_FacilityColor.Maximum = getColorCount(type);
            NUD_FacilityColor.Value = Math.Min(facility.Color, NUD_FacilityColor.Maximum);
            if (type >= 0) LoadColorLabel(type);
            NUD_Exchangable.Enabled = NUD_Exchangable.Visible = L_Exchangable.Visible = type == 7;
            if (type == 7) NUD_Exchangable.Value = facility.ExchangeLeftCount;
            CB_FacilityNPC.SelectedIndex =
                CB_FacilityNPC.Items.Count > facility.NPC
                ? facility.NPC
                : 0;
            CHK_FacilityIntroduced.Checked = facility.IsIntroduced;
            TB_OTName.Text = facility.OT_Name;
            LoadOTlabel(facility.Gender);
            if (CB_FacilityMessage.SelectedIndex >= 0) LoadFMessage(CB_FacilityMessage.SelectedIndex);
            TB_UsedFlags.Text = f[entry].UsedFlags.ToString("X8");
            TB_UsedStats.Text = f[entry].UsedRandStat.ToString("X8");
            var bytes = f[entry].TrainerFesID;
            var str = BitConverter.ToString(bytes).Replace("-", string.Empty);
            TB_FacilityID.Text = str;
            editing = false;
        }
        private void Save()
        {
            SAV.SetFestaPhraseUnlocked(106, CLB_Phrases.GetItemChecked(0));
            for (int i = 1; i < CLB_Phrases.Items.Count; i++)
                SAV.SetFestaPhraseUnlocked(i - 1, CLB_Phrases.GetItemChecked(i));

            SAV.UsedFestaCoins = (uint)NUD_FC_Used.Value;
            SAV.FestaCoins = (uint)NUD_FC_Current.Value;
            SAV.FestaDate = new DateTime(CAL_FestaStartDate.Value.Year, CAL_FestaStartDate.Value.Month, CAL_FestaStartDate.Value.Day, CAL_FestaStartTime.Value.Hour, CAL_FestaStartTime.Value.Minute, CAL_FestaStartTime.Value.Second);

            SAV.SetFestaPrizeReceived(10, r[(int)CLB_Reward.GetItemCheckState(0)]);
            for (int i = 1; i < CLB_Reward.Items.Count; i++)
                SAV.SetFestaPrizeReceived(i - 1, r[(int)CLB_Reward.GetItemCheckState(i)]);

            foreach (FestaFacility facility in f)
                facility.CopyTo(SAV);

            if (SAV.USUM)
                SaveBattleAgency();
        }
        private void LoadBattleAgency()
        {
            p[0] = SAV.GetPKM(SAV.DecryptPKM(SAV.GetData(0x6C200, 0xE8)));
            p[1] = SAV.GetPKM(SAV.DecryptPKM(SAV.GetData(0x6C2E8, 0x104)));
            p[2] = SAV.GetPKM(SAV.DecryptPKM(SAV.GetData(0x6C420, 0x104)));
            LoadPictureBox();
            B_ImportParty.Visible = SAV.HasParty;
            CHK_Choosed.Checked = SAV.GetFlag(0x6C55E, 1);
            CHK_TrainerInvited.Checked = BitConverter.ToUInt16(SAV.GetData(0x6C3EE, 2), 0) == 0x7FFF && BitConverter.ToUInt16(SAV.GetData(0x6C526, 2), 0) == 0x7FFF;
            ushort valus = BitConverter.ToUInt16(SAV.GetData(0x6C55C, 2), 0);
            int grade = valus >> 6 & 0x3F;
            NUD_Grade.Value = grade;
            int max = Math.Min(49, grade) / 10 * 3 + 2;
            int defeated = valus >> 12;
            NUD_Defeated.Value = defeated > max ? max : defeated;
            NUD_Defeated.Maximum = max;
        }
        private void LoadPictureBox()
        {
            for (int i = 0; i < 3; i++)
                PBs[i].Image = p[i].Sprite(SAV, -1, -1, false);
        }
        private readonly PKM[] p = new PKM[3];
        private readonly PictureBox[] PBs = new PictureBox[3];
        private void SaveBattleAgency()
        {
            SAV.SetFlag(0x6C55E, 1, CHK_Choosed.Checked);
            byte[] TrainerInvited = BitConverter.GetBytes((ushort)(CHK_TrainerInvited.Checked ? 0x7FFF : 0));
            SAV.SetData(TrainerInvited, 0x6C3EE);
            SAV.SetData(TrainerInvited, 0x6C526);
            SAV.SetData(p[0].EncryptedBoxData, 0x6C200);
            SAV.SetData(p[1].EncryptedPartyData, 0x6C2E8);
            SAV.SetData(p[2].EncryptedPartyData, 0x6C420);
            SAV.SetData(BitConverter.GetBytes((ushort)(((int)NUD_Defeated.Value & 0xF) << 12 | ((int)NUD_Grade.Value & 0x3F) << 6)), 0x6C55C);
        }
        private void NUD_FC_ValueChanged(object sender, EventArgs e)
        {
            if (editing) return;
            L_FC_CollectedV.Text = (NUD_FC_Current.Value + NUD_FC_Used.Value).ToString();
        }

        private void B_Cancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void B_Save_Click(object sender, EventArgs e)
        {
            Save();
            Origin.SetData(SAV.Data, 0);
            Close();
        }

        private void B_AllReceiveReward_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < CLB_Reward.Items.Count; i++)
                CLB_Reward.SetItemCheckState(i, CheckState.Checked);
        }

        private void B_AllReadyReward_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < CLB_Reward.Items.Count; i++)
                CLB_Reward.SetItemCheckState(i, CheckState.Indeterminate);
        }

        private void B_AllPhrases_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < CLB_Phrases.Items.Count; i++)
                CLB_Phrases.SetItemChecked(i, true);
        }

        private void TB_OTName_MouseDown(object sender, MouseEventArgs e)
        {
            TextBox tb = sender as TextBox ?? TB_OTName;
            // Special Character Form
            if (ModifierKeys != Keys.Control)
                return;

            var d = new TrashEditor(tb, null, SAV);
            d.ShowDialog();
            tb.Text = d.FinalString;
        }
        private readonly string[] gendersymbols = { "♂", "♀" };
        private void LoadOTlabel(int b)
        {
            Label_OTGender.Text = gendersymbols[b & 1];
            Label_OTGender.ForeColor = b == 1 ? Color.Red : Color.Blue;
        }
        private void Label_OTGender_Click(object sender, EventArgs e)
        {
            if (entry < 0) return;
            var b = f[entry].Gender;
            b ^= 1;
            f[entry].Gender = b;
            LoadOTlabel(b);
        }
        private void LoadFMessage(int fmIndex) => NUD_FacilityMessage.Value = f[entry].GetMessage(fmIndex);
        private void CB_FacilityMessage_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (editing) return;
            int fmIndex = CB_FacilityMessage.SelectedIndex;
            if (fmIndex < 0) return;
            if (entry < 0) return;
            editing = true;
            LoadFMessage(fmIndex);
            editing = false;
        }

        private void NUD_FacilityMessage_ValueChanged(object sender, EventArgs e)
        {
            if (editing) return;
            int fmIndex = CB_FacilityMessage.SelectedIndex;
            if (fmIndex < 0) return;
            if (entry < 0) return;
            f[entry].SetMessage(fmIndex, (ushort)NUD_FacilityMessage.Value);
        }

        private void HexTextBox_TextChanged(object sender, EventArgs e)
        {
            if (editing) return;
            if (entry < 0) return;
            string t = Util.GetOnlyHex(((TextBox)sender).Text);
            int maxlen = sender == TB_FacilityID ? 12 << 1 : 4 << 1;
            if (t.Length > maxlen)
            {
                t = t.Substring(0, maxlen);
                editing = true;
                ((TextBox)sender).Text = t;
                editing = false;
                System.Media.SystemSounds.Beep.Play();
            }
            if (sender == TB_UsedFlags)
                f[entry].UsedFlags = Convert.ToUInt32(t, 16);
            else if (sender == TB_UsedStats)
                f[entry].UsedRandStat = Convert.ToUInt32(t, 16);
            else if (sender == TB_FacilityID)
            {
                if (t.Length != 12 * 2)
                    t = t.PadLeft(24, '0');
                var bytes = t.ToByteArray();
                Array.Resize(ref bytes, 12);
                f[entry].TrainerFesID = bytes;
            }
        }
        private void LoadColorLabel(int type) => L_FacilityColorV.Text = RES_Color[RES_FacilityColor[type][(int)NUD_FacilityColor.Value]];
        private void NUD_FacilityColor_ValueChanged(object sender, EventArgs e)
        {
            if (editing) return;
            if (entry < 0) return;
            f[entry].Color = (byte)NUD_FacilityColor.Value;
            int type = TypeIndexToType(CB_FacilityType.SelectedIndex);
            if (type < 0) return;
            editing = true;
            LoadColorLabel(type);
            editing = false;
        }

        private void CB_FacilityType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (editing) return;
            if (entry < 0) return;
            int typeIndex = CB_FacilityType.SelectedIndex;
            if (typeIndex < 0) return;
            f[entry].Type = (byte)typeIndex;
            int type = TypeIndexToType(typeIndex);
            int colorCount = getColorCount(type);
            editing = true;
            if (colorCount < NUD_FacilityColor.Value)
            {
                NUD_FacilityColor.Value = colorCount;
                f[entry].Color = (byte)colorCount;
            }
            NUD_FacilityColor.Maximum = colorCount;
            LoadColorLabel(type);
            NUD_Exchangable.Enabled = NUD_Exchangable.Visible = L_Exchangable.Visible = type == 7;
            if (type == 7) NUD_Exchangable.Value = f[entry].ExchangeLeftCount;
            editing = false;
        }

        private void SaveFacility()
        {
            if (entry < 0)
                return;
            var facility = f[entry];
            facility.Type = CB_FacilityType.SelectedIndex;
            facility.Color = (int)NUD_FacilityColor.Value;
            facility.OT_Name = TB_OTName.Text;
            facility.NPC = CB_FacilityNPC.SelectedIndex;
            facility.IsIntroduced = CHK_FacilityIntroduced.Checked;
            facility.ExchangeLeftCount = (byte)(TypeIndexToType(facility.Type) == 7 ? NUD_Exchangable.Value : 0);
        }
        private void LoadRankLabel(int rank)
        {
            int i, j;
            if (rank < 1) L_RankFC.Text = "";
            else if (rank == 1) L_RankFC.Text = "0 - 5";
            else if (rank == 2) L_RankFC.Text = "6 - 15";
            else if (rank == 3) L_RankFC.Text = "16 - 30";
            else if (rank <= 10)
            {
                i = (rank - 1) * (rank - 2) * 5 + 1;
                L_RankFC.Text = i.ToString() + " - " + (i + (rank - 1) * 10 - 1).ToString();
            }
            else if (rank <= 20)
            {
                i = rank * 100 - 649;
                L_RankFC.Text = i.ToString() + " - " + (i + 99).ToString();
            }
            else if (rank <= 70)
            {
                j = (rank - 1) / 10;
                i = rank * (j * 30 + 60) - (j * j * 150 + j * 180 + 109);
                L_RankFC.Text = i.ToString() + " - " + (i + j * 30 + 59).ToString();
            }
            else if (rank <= 100)
            {
                i = rank * 270 - 8719;
                L_RankFC.Text = i.ToString() + " - " + (i + 269).ToString();
            }
            else if (rank <= 998)
            {
                i = rank * 300 - 11749;
                L_RankFC.Text = i.ToString() + " - " + (i + 299).ToString();
            }
            else if (rank == 999) L_RankFC.Text = "287951 - ";
            else L_RankFC.Text = "";
        }
        private void NUD_Rank_ValueChanged(object sender, EventArgs e)
        {
            if (editing) return;
            int rank = (int)NUD_Rank.Value;
            SAV.FestaRank = (ushort)rank;
            LoadRankLabel(rank);
        }
        private readonly NumericUpDown[] NUDs;
        private void NUD_MyMessage_ValueChanged(object sender, EventArgs e)
        {
            if (editing) return;
            int mmIndex = Array.IndexOf(NUDs, (NumericUpDown)sender);
            if (mmIndex < 0) return;
            SAV.SetFestaMessage(mmIndex, (ushort)((NumericUpDown)sender).Value);
        }

        private void CHK_FacilityIntroduced_CheckedChanged(object sender, EventArgs e)
        {
            if (editing) return;
            if (entry < 0) return;
            f[entry].IsIntroduced = CHK_FacilityIntroduced.Checked;
        }

        private void TB_OTName_TextChanged(object sender, EventArgs e)
        {
            if (editing) return;
            if (entry < 0) return;
            f[entry].OT_Name = TB_OTName.Text;
        }

        private void LB_FacilityIndex_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (editing) return;
            SaveFacility();
            entry = LB_FacilityIndex.SelectedIndex;
            if (entry < 0) return;
            LoadFacility();
        }

        private void LB_FacilityIndex_DrawItem(object sender, DrawItemEventArgs e)
        {
            e.DrawBackground();
            e.Graphics.DrawString(((ListBox)sender).Items[e.Index].ToString(), e.Font, new SolidBrush(e.ForeColor), new RectangleF(e.Bounds.X, e.Bounds.Y + (e.Bounds.Height - 12 >> 1), e.Bounds.Width, 12));
            e.DrawFocusRectangle();
        }

        private void B_DelVisitor_Click(object sender, EventArgs e)
        {
            if (entry < 0) return;
            var facility = f[entry];
            // there is a unknown value when not introduced...no reproducibility, just mistake?
            if (facility.IsIntroduced)
                facility.TrainerFesID = new byte[12];
            facility.IsIntroduced = false;
            facility.OT_Name = "";
            facility.Gender = 0;
            for (int i = 0; i < 4; i++)
                facility.SetMessage(i, 0);
            LoadFacility();
        }
        private string GetSpeciesNameFromPKM(PKM pkm) => PKX.GetSpeciesName(pkm.Species, SAV.Language);
        private void B_ImportParty_Click(object sender, EventArgs e)
        {
            if (!SAV.HasParty) return;
            var party = SAV.PartyData;
            string msg = "";
            for(int i = 0; i < 3; i++)
            {
                if (i < party.Count)
                    msg += Environment.NewLine + GetSpeciesNameFromPKM(p[i]) + " -> " + GetSpeciesNameFromPKM(party[i]);
                else
                    msg += Environment.NewLine + "not replace: " + GetSpeciesNameFromPKM(p[i]);
            }
            if (DialogResult.Yes == WinFormsUtil.Prompt(MessageBoxButtons.YesNo, "Replace PKM?", msg))
            {
                for (int i = 0, min = Math.Min(3, party.Count); i < min; i++)
                    p[i] = party[i];
                LoadPictureBox();
            }
        }

        private void mnuSave_Click(object sender, EventArgs e)
        {
            int i = Array.IndexOf(PBs, ((sender as ToolStripItem)?.Owner as ContextMenuStrip)?.SourceControl ?? sender as PictureBox);
            if (i < 0) return;
            WinFormsUtil.SavePKMDialog(p[i]);
        }

        private void NUD_Grade_ValueChanged(object sender, EventArgs e)
        {
            if (editing) return;
            int max = Math.Min(49, (int)NUD_Grade.Value) / 10 * 3 + 2;
            editing = true;
            if (NUD_Defeated.Value > max)
                NUD_Defeated.Value = max;
            NUD_Defeated.Maximum = max;
            editing = false;
        }
    }
    public class FestaFacility
    {
        private readonly byte[] Data;
        private readonly int Language;
        private const int SIZE = 0x48;

        public int Type { get => Data[0x00]; set => Data[0x00] = (byte)value; }
        public int Color { get => Data[0x01]; set => Data[0x01] = (byte)value; }
        public bool IsIntroduced { get => Data[0x02] != 0; set => Data[0x02] = (byte)(value ? 1 : 0); }
        public int Gender { get => Data[0x03]; set => Data[0x03] = (byte)value; }
        public string OT_Name { get => StringConverter.GetString7(Data, 0x04, 0x1A); set => StringConverter.SetString7(value, 12, Language).CopyTo(Data, 0x04); }

        private int MessageMeet { get => BitConverter.ToUInt16(Data, 0x1E); set => BitConverter.GetBytes((ushort)value).CopyTo(Data, 0x1E); }
        private int MessagePart { get => BitConverter.ToUInt16(Data, 0x20); set => BitConverter.GetBytes((ushort)value).CopyTo(Data, 0x20); }
        private int MessageMoved { get => BitConverter.ToUInt16(Data, 0x22); set => BitConverter.GetBytes((ushort)value).CopyTo(Data, 0x22); }
        private int MessageDisappointed { get => BitConverter.ToUInt16(Data, 0x24); set => BitConverter.GetBytes((ushort)value).CopyTo(Data, 0x24); }

        public uint UsedFlags { get => BitConverter.ToUInt32(Data, 0x28); set => BitConverter.GetBytes(value).CopyTo(Data, 0x28); }
        public uint UsedRandStat { get => BitConverter.ToUInt32(Data, 0x2C); set => BitConverter.GetBytes(value).CopyTo(Data, 0x2C); }

        public int NPC { get => Math.Max(0, BitConverter.ToInt32(Data, 0x30)); set => BitConverter.GetBytes(Math.Max(0, value)).CopyTo(Data, 0x30); }
        public byte[] TrainerFesID { get => Data.Skip(0x34).Take(12).ToArray(); set => value.CopyTo(Data, 0x34); }
        public byte ExchangeLeftCount { get => Data[0x40]; set => Data[0x40] = value; } //over 9 shows "?"
        private readonly int ofs;
        public FestaFacility(SAV7 sav, int index)
        {
            ofs = index * SIZE + sav.JoinFestaData + 0x310;
            Data = sav.GetData(ofs, SIZE);
            Language = sav.Language;
        }
        public void CopyTo(SAV7 sav) => sav.SetData(Data, ofs);
        public int GetMessage(int index)
        {
            switch (index)
            {
                case 0: return MessageMeet;
                case 1: return MessagePart;
                case 2: return MessageMoved;
                case 3: return MessageDisappointed;
                default: return 0;
            }
        }
        public void SetMessage(int index, ushort value)
        {
            switch (index)
            {
                case 0: MessageMeet = value; break;
                case 1: MessagePart = value; break;
                case 2: MessageMoved = value; break;
                case 3: MessageDisappointed = value; break;
                default: return;
            }
        }
    }
}

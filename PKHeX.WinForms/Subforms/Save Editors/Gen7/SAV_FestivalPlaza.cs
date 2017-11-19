using System;
using System.Drawing;
using System.Linq;
using System.Text;
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
                    res = new string[107] {
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
                case "en":
                default:
                    string musical8note = char.ConvertFromUtf32(0x266A);
                    string linedP = char.ConvertFromUtf32(0x20BD);//currency Ruble
                    res = new string[107] { //source:UltraMoon
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
            CLB_Phrases.Items.Add(res.Last(), SAV.getFestaPhraseUnlocked(106)); //add Lv100 before TentPhrases
            for (int i = 0; i < res.Length - 1; i++)
                CLB_Phrases.Items.Add(res[i], SAV.getFestaPhraseUnlocked(i));

            DateTime dt = SAV.FestaDate ?? new DateTime(2000, 1, 1);
            CAL_FestaStartDate.Value = dt;
            CAL_FestaStartTime.Value = dt;

            string[] res2 = { "Rank 4: missions","Rank 8: facility","Rank 10: facion","Rank 20: rename","Rank 30: special menu","Rank 40: BGM","Rank 50: theme Glitz","Rank 60: theme Fairy","Rank 70: theme Tone","Rank 100: phrase","Current Rank", };
            CLB_Reward.Items.Clear();
            CLB_Reward.Items.Add(res2.Last(), (CheckState)r[SAV.getFestPrizeReceived(10)]); //add CurrentRank before const-rewards
            for (int i = 0; i < res2.Length - 1; i++)
                CLB_Reward.Items.Add(res2[i], (CheckState)r[SAV.getFestPrizeReceived(i)]);

            for (int i = 0; i < 7; i++)
                f[i] = new FestaFacility(SAV, i);

            string[] res3 = { "Meet", "Part", "Moved", "Disappointed" };
            CB_FacilityMessage.Items.Clear();
            CB_FacilityMessage.Items.AddRange(res3);
            CB_MyMessage.Items.Clear();
            CB_MyMessage.Items.AddRange(res3);
            string[] res4 = { "misc1", "misc2", "FestaID" };
            CB_FacilityID.Items.Clear();
            CB_FacilityID.Items.AddRange(res4);
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
                new string[]{"BigDream","GoldRush","TreasureHunt"},
                new string[]{"GhostsDen","TrickRoom","ConfuseRay"},
                new string[]{"Ball","General","Battle","SoftDrink","Pharmacy"},
                new string[]{"Rare","Battle", "FriendshipCafé", "FriendshipParlor"},
                new string[]{"Thump","Clink","Stomp"},
                new string[]{"Kanto","Johto","Hoenn","Sinnoh","Unova","Kalos","Pokémon"},
                new string[]{"Red","Yellow","Green","Blue","Orange","NavyBlue","Purple","Pink"},
                new string[]{"Switcheroo"}
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
            CB_MyMessage.SelectedIndex = 0;
            loadMMessage(0);
            NUD_FacilityIndex.Value = 0;
            CB_FacilityMessage.SelectedIndex = 0;
            CB_FacilityID.SelectedIndex = 0;
            editing = false;
            loadFacility(0);
        }
        private bool editing;
        private readonly byte[] r = { 0, 2, 1 }; // CheckState.Indeterminate <-> CheckState.Checked
        private readonly int typeMAX;
        private FestaFacility[] f = new FestaFacility[7];
        private string[] RES_Color = { "Red", "Blue", "Gold", "Black", "Purple", "Yellow", "Brown", "Green", "Orange", "NavyBlue", "Pink", "White" };
        private byte[][] RES_FacilityColor = //facility appearance
        {
            new byte[]{0,1,2,3},//Lottery
            new byte[]{4,0,5,3},//Haunted
            new byte[]{1,0,5,3},//Goody
            new byte[]{6,7,0,3},//Food
            new byte[]{4,5,8,3},//Bouncy
            new byte[]{0,1,2,3},//Fortune
            new byte[]{0,7,8,4,5,2,9,10},//Dye
            new byte[]{11,1,5,3},//Exchange
        };
        private byte[][] RES_FacilityLevelType = //3:123 4:135 5:12345
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
        private int typeIndexToType(int typeIndex)
        {
            if (typeIndex < 0 || typeIndex > typeMAX) return -1;
            else if (typeIndex < 0x0F) return 0;
            else if (typeIndex < 0x1E) return 1;
            else if (typeIndex < 0x2F) return 2;
            else if (typeIndex < 0x41) return 3;
            else if (typeIndex < 0x50) return 4;
            else if (typeIndex < 0x65) return 5;
            else if (typeIndex < 0x7D) return 6;
            else return 7;
        }
        private int getColorCount(int i) =>
                i >= 0 && i < RES_FacilityColor.Length - (SAV.USUM ? 0 : 1)
                ? RES_FacilityColor[i].Length - 1
                : 3;
        private void loadFacility(int index)
        {
            editing = true;
            CB_FacilityType.SelectedIndex =
                CB_FacilityType.Items.Count > f[index].type
                ? f[index].type
                : -1;
            int type = typeIndexToType(CB_FacilityType.SelectedIndex);
            NUD_FacilityColor.Maximum = getColorCount(type);
            NUD_FacilityColor.Value = Math.Min(f[index].color, NUD_FacilityColor.Maximum);
            if (type >= 0) loadColorLabel(type);
            CB_FacilityNPC.SelectedIndex =
                CB_FacilityNPC.Items.Count > f[index].npc
                ? f[index].npc
                : 0;
            CHK_FacilityIntroduced.Checked = f[index].introduced;
            TB_OTName.Text = f[index].trainerName;
            loadOTlabel(f[index].trainerGender);
            if (CB_FacilityMessage.SelectedIndex >= 0) loadFMessage(CB_FacilityMessage.SelectedIndex);
            if (CB_FacilityID.SelectedIndex >= 0) loadFestID(CB_FacilityID.SelectedIndex);
            editing = false;
        }
        private void Save()
        {
            SAV.setFestaPhraseUnlocked(106, CLB_Phrases.GetItemChecked(0));
            for (int i = 1; i < CLB_Phrases.Items.Count; i++)
                SAV.setFestaPhraseUnlocked(i - 1, CLB_Phrases.GetItemChecked(i));

            SAV.UsedFestaCoins = (uint)NUD_FC_Used.Value;
            SAV.FestaCoins = (uint)NUD_FC_Current.Value;
            SAV.FestaDate = new DateTime(CAL_FestaStartDate.Value.Year, CAL_FestaStartDate.Value.Month, CAL_FestaStartDate.Value.Day, CAL_FestaStartTime.Value.Hour, CAL_FestaStartTime.Value.Minute, CAL_FestaStartTime.Value.Second);

            SAV.setFestaPrizeReceived(10, r[(int)CLB_Reward.GetItemCheckState(0)]);
            for (int i = 1; i < CLB_Reward.Items.Count; i++)
                SAV.setFestaPrizeReceived(i - 1, r[(int)CLB_Reward.GetItemCheckState(i)]);

            for (int i = 0; i < f.Length; i++)
                f[i].CopyTo(SAV);
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
        private void loadOTlabel(bool b)
        {
            Label_OTGender.Text = gendersymbols[b ? 1 : 0];
            Label_OTGender.ForeColor = b ? Color.Red : Color.Blue;
        }
        private void Label_OTGender_Click(object sender, EventArgs e)
        {
            int fIndex = (int)NUD_FacilityIndex.Value;
            bool b = f[fIndex].trainerGender;
            b = !b;
            f[fIndex].trainerGender = b;
            loadOTlabel(b);
        }
        private void loadFMessage(int fmIndex) => NUD_FacilityMessage.Value = f[(int)NUD_FacilityIndex.Value].GetMessage(fmIndex);
        private void CB_FacilityMessage_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (editing) return;
            int fmIndex = CB_FacilityMessage.SelectedIndex;
            if (fmIndex < 0) return;
            editing = true;
            loadFMessage(fmIndex);
            editing = false;
        }

        private void NUD_FacilityMessage_ValueChanged(object sender, EventArgs e)
        {
            if (editing) return;
            int fmIndex = CB_FacilityMessage.SelectedIndex;
            if (fmIndex < 0) return;
            f[(int)NUD_FacilityIndex.Value].SetMessage(fmIndex, (ushort)NUD_FacilityMessage.Value);
        }
        private void loadFestID(int m)
        {
            int fIndex = (int)NUD_FacilityIndex.Value;
            switch (m)
            {
                case 0: TB_FacilityID.Text = f[fIndex].misc1.ToString("X8"); break;
                case 1: TB_FacilityID.Text = f[fIndex].misc2.ToString("X8"); break;
                case 2:
                    StringBuilder stringBuilder = new StringBuilder(f[fIndex].trainerFesID.Length << 1);
                    for (int j = f[fIndex].trainerFesID.Length - 1; j >= 0; j--)
                        stringBuilder.Append(f[fIndex].trainerFesID[j].ToString("X2"));
                    TB_FacilityID.Text = stringBuilder.ToString();
                    break;
            }

        }
        private void CB_FacilityID_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (editing) return;
            int fiIndex = CB_FacilityID.SelectedIndex;
            if (fiIndex < 0) return;
            editing = true;
            loadFestID(fiIndex);
            editing = false;
        }

        private void TB_FacilityID_TextChanged(object sender, EventArgs e)
        {
            if (editing) return;
            int fIndex = (int)NUD_FacilityIndex.Value;
            int fiIndex = CB_FacilityID.SelectedIndex;
            string t = System.Text.RegularExpressions.Regex.Replace(TB_FacilityID.Text, "[^A-Fa-f0-9]", "0");
            int maxlen = fiIndex == 2 ? 12 << 1 : 4 << 1;
            if (t.Length > maxlen)
            {
                t = t.Substring(0, maxlen);
                editing = true;
                TB_FacilityID.Text = t;
                editing = false;
                System.Media.SystemSounds.Beep.Play();
            }
            switch (fiIndex)
            {
                case 0: f[fIndex].misc1 = Convert.ToUInt32(t, 16); break;
                case 1: f[fIndex].misc2 = Convert.ToUInt32(t, 16); break;
                case 2:
                    int w = 0;
                    for (int j = 0, k = t.Length - 2; k + 1 >= 0; j++, k -= 2)
                        f[fIndex].trainerFesID[w++] = Convert.ToByte(k < 0 ? t.Substring(0, 1) : t.Substring(k, 2), 16);
                    for (int j = w; j < f[fIndex].trainerFesID.Length; j++)
                        f[fIndex].trainerFesID[j] = 0;
                    break;
            }
        }
        private void loadColorLabel(int type) => L_FacilityColorV.Text = RES_Color[RES_FacilityColor[type][(int)NUD_FacilityColor.Value]];
        private void NUD_FacilityColor_ValueChanged(object sender, EventArgs e)
        {
            if (editing) return;
            f[(int)NUD_FacilityIndex.Value].type = (byte)NUD_FacilityColor.Value;
            int type = typeIndexToType(CB_FacilityType.SelectedIndex);
            if (type < 0) return;
            editing = true;
            loadColorLabel(type);
            editing = false;
        }

        private void CB_FacilityType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (editing) return;
            int fIndex = (int)NUD_FacilityIndex.Value;
            int typeIndex = CB_FacilityType.SelectedIndex;
            if (typeIndex < 0) return;
            f[fIndex].type = (byte)typeIndex;
            int type = typeIndexToType(typeIndex);
            int colorCount = getColorCount(type);
            editing = true;
            if (colorCount < NUD_FacilityColor.Value)
            {
                NUD_FacilityColor.Value = colorCount;
                f[fIndex].color = (byte)colorCount;
            }
            NUD_FacilityColor.Maximum = colorCount;
            loadColorLabel(type);
            editing = false;
        }

        private void NUD_FacilityIndex_ValueChanged(object sender, EventArgs e)
        {
            if (editing) return;
            loadFacility((int)NUD_FacilityIndex.Value);
        }

        private void NUD_Rank_ValueChanged(object sender, EventArgs e)
        {
            if (editing) return;
            SAV.FestaRank = (ushort)NUD_Rank.Value;
        }
        private void loadMMessage(int mmIndex) => NUD_MyMessage.Value = SAV.getFestaMessage(mmIndex);
        private void CB_MyMessage_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (editing) return;
            int mmIndex = CB_MyMessage.SelectedIndex;
            if (mmIndex < 0) return;
            editing = true;
            loadMMessage(mmIndex);
            editing = false;
        }

        private void NUD_MyMessage_ValueChanged(object sender, EventArgs e)
        {
            if (editing) return;
            int mmIndex = CB_MyMessage.SelectedIndex;
            if (mmIndex < 0) return;
            SAV.setFestaMessage(mmIndex, (ushort)NUD_MyMessage.Value);
        }

        private void CHK_FacilityIntroduced_CheckedChanged(object sender, EventArgs e)
        {
            if (editing) return;
            int fIndex = (int)NUD_FacilityIndex.Value;
            f[fIndex].introduced = CHK_FacilityIntroduced.Checked;
            if (!L_Note.Visible && !f[fIndex].introduced && (
                f[fIndex].trainerName.Length > 0
                || f[fIndex].GetMessage(0) != 0
                || f[fIndex].GetMessage(1) != 0
                || f[fIndex].GetMessage(2) != 0
                || f[fIndex].GetMessage(3) != 0
                || f[fIndex].misc1 != 0
                || f[fIndex].misc2 != 0
                || f[fIndex].trainerFesID.Any(v => v != 0)
                )) L_Note.Visible = true;
        }

        private void TB_OTName_TextChanged(object sender, EventArgs e)
        {
            if (editing) return;
            f[(int)NUD_FacilityIndex.Value].trainerName = TB_OTName.Text;
        }
    }
    public class FestaFacility
    {
        public byte type;
        public byte color;
        public bool introduced;
        public bool trainerGender;
        public string trainerName;
        private ushort messageMeet, messagePart, messageMoved, messageDisappointed;
        public uint misc1, misc2;
        public int npc;
        public byte[] trainerFesID;
        private int ofs;
        public FestaFacility(SAV7 sav, int facilityIndex)
        {
            ofs = facilityIndex * 0x48 + (sav.USUM ? 0x51110 : 0x50B10);
            type = sav.Data[ofs];
            color = sav.Data[ofs + 1];
            introduced = sav.Data[ofs + 2] != 0;
            trainerGender = sav.Data[ofs + 3] != 0;
            trainerName = sav.GetString(ofs + 4, 12 << 1);
            messageMeet = messageFilter(BitConverter.ToUInt16(sav.Data, ofs + 0x1E));
            messagePart = messageFilter(BitConverter.ToUInt16(sav.Data, ofs + 0x20));
            messageMoved = messageFilter(BitConverter.ToUInt16(sav.Data, ofs + 0x22));
            messageDisappointed = messageFilter(BitConverter.ToUInt16(sav.Data, ofs + 0x24));
            misc1 = BitConverter.ToUInt32(sav.Data, ofs + 0x28);
            misc2 = BitConverter.ToUInt32(sav.Data, ofs + 0x2C);
            npc = Math.Max(0, BitConverter.ToInt32(sav.Data, ofs + 0x30));
            trainerFesID = sav.Data.Skip(ofs + 0x34).Take(12).ToArray();
        }
        private ushort messageFilter(ushort inp) => Math.Min(inp, (ushort)0xFFF);
        public void CopyTo(SAV7 sav)
        {
            sav.Data[ofs] = type;
            sav.Data[ofs + 1] = color;
            if (introduced)
            {
                sav.Data[ofs + 2] = Convert.ToByte(introduced);
                sav.Data[ofs + 3] = Convert.ToByte(trainerGender);
                sav.SetString(trainerName, 12).CopyTo(sav.Data, ofs + 4);
                BitConverter.GetBytes(messageMeet).CopyTo(sav.Data, ofs + 0x1E);
                BitConverter.GetBytes(messagePart).CopyTo(sav.Data, ofs + 0x20);
                BitConverter.GetBytes(messageMoved).CopyTo(sav.Data, ofs + 0x22);
                BitConverter.GetBytes(messageDisappointed).CopyTo(sav.Data, ofs + 0x24);
                BitConverter.GetBytes(misc1).CopyTo(sav.Data, ofs + 0x28);
                BitConverter.GetBytes(misc2).CopyTo(sav.Data, ofs + 0x2C);
                BitConverter.GetBytes(npc).CopyTo(sav.Data, ofs + 0x30);
                trainerFesID.CopyTo(sav.Data, ofs + 0x34);
            }
            else
            {
                byte[] fid = new byte[0x40 - 2];
                BitConverter.GetBytes(npc).CopyTo(fid, 0x2E);
                fid.CopyTo(sav.Data, ofs + 2);
            }
        }
        public ushort GetMessage(int index)
        {
            switch (index)
            {
                case 0: return messageMeet;
                case 1: return messagePart;
                case 2: return messageMoved;
                case 3: return messageDisappointed;
                default: return 0;
            }
        }
        public void SetMessage(int index, ushort value)
        {
            value = messageFilter(value);
            switch (index)
            {
                case 0: messageMeet = value; break;
                case 1: messagePart = value; break;
                case 2: messageMoved = value; break;
                case 3: messageDisappointed = value; break;
                default: return;
            }
        }
    }
}

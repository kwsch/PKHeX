using System;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;
using PKHeX.Core;
using PKHeX.Drawing.PokeSprite;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.WinForms;

public partial class SAV_FestivalPlaza : Form
{
    private readonly SaveFile Origin;
    private readonly SAV7 SAV;

    private int entry;

    public SAV_FestivalPlaza(SAV7 sav)
    {
        InitializeComponent();
        SAV = (SAV7)(Origin = sav).Clone();
        editing = true;
        entry = -1;
        typeMAX = SAV is SAV7USUM ? 0x7F : 0x7C;
        TB_PlazaName.Text = SAV.Festa.FestivalPlazaName;

        if (SAV is SAV7USUM)
        {
            PBs = [ppkx1, ppkx2, ppkx3];
            NUD_Trainers = [NUD_Trainer1, NUD_Trainer2, NUD_Trainer3];
            LoadBattleAgency();
        }
        else
        {
            TC_Editor.TabPages.Remove(Tab_BattleAgency);
        }

        if (Main.Unicode)
        {
            TB_OTName.Font = FontUtil.GetPKXFont();
        }

        var cc = SAV.Festa.FestaCoins;
        var cu = SAV.GetRecord(038);
        NUD_FC_Current.Value = Math.Min(cc, NUD_FC_Current.Maximum);
        NUD_FC_Used.Value = Math.Min(cu, NUD_FC_Used.Maximum);
        L_FC_CollectedV.Text = (cc + cu).ToString();
        string[] res;
        switch (Main.CurrentLanguage)
        {
            case "ja":
                res = [
                    "おじさんの きんのたま だからね！","かがくの ちからって すげー","1 2の …… ポカン！","おーす！ みらいの チャンピオン！","おお！ あんたか！","みんな げんきに なりましたよ！","とっても 幸せそう！","なんでも ないです","いあいぎりで きりますか？","レポートを かきこんでいます",
                    "…… ぼくも もう いかなきゃ！","ボンジュール！","バイビー！","ばか はずれです……","やけどなおしの よういは いいか！","ウー！ ハーッ！","ポケモンは たたかわせるものさ","ヤドランは そっぽを むいた！","マサラは まっしろ はじまりのいろ","10000こうねん はやいんだよ！","おーい！ まてー！ まつんじゃあ！","こんちわ！ ぼく ポケモン……！","っだと こらあ！","ぐ ぐーッ！ そんな ばかなーッ！","みゅう！","タチサレ…… タチサレ……",
                    "カイリュー はかいこうせん","どっちか 遊んでくれないか？","ぬいぐるみ かっておいたわよ","ひとのこと じろじろ みてんなよ","なんのことだか わかんない","みんな ポケモン やってるやん","きょうから 24時間 とっくんだ！","あたいが ホンモノ！","でんげきで いちころ……","スイクンを おいかけて 10ねん","かんどうが よみがえるよ！","われわれ ついに やりましたよー！","ヤドンのシッポを うるなんて……","ショオーッ!!","ギャーアアス!!","だいいっぽを ふみだした！",
                    "いちばん つよくて すごいんだよね","にくらしいほど エレガント！","そうぞうりょくが たりないよ","キミは ビッグウェーブ！","おまえさんには しびれた わい","なに いってんだろ…… てへへ……","ぬいぐるみ なんか かってないよ","ここで ゆっくり して おいき！","はじけろ！ ポケモン トレーナー！","はいが はいに はいった……","…できる！","ぶつかった かいすう 5かい！","たすけて おくれーっ!!","マボロシじま みえんのう……","ひゅああーん！","しゅわーん！",
                    "あつい きもち つたわってくる！","こいつが！ おれの きりふだ！","ひとりじめとか そういうの ダメよ！","ワーオ！ ぶんせきどーり！","ぱるぱるぅ!!!","グギュグバァッ!!!","ばっきん 100まんえん な！","オレ つよくなる……","ながれる 時間は とめられない！","ぜったいに お願いだからね","きみたちから はどうを かんじる！","あたしのポケモンに なにすんのさ！","リングは おれの うみ～♪","オレの おおごえの ひとりごとを","そう コードネームは ハンサム！","……わたしが まけるかも だと!?",
                    "やめたげてよぉ！","ブラボー！ スーパー ブラボー！","ボクは チャンピオンを こえる","オレは いまから いかるぜッ！","ライモンで ポケモン つよいもん","キミ むしポケモン つかいなよ","ストップ！","ひとよんで メダルおやじ！","トレーナーさんも がんばれよ！","おもうぞんぶん きそおーぜ！","プラズマズイ！","ワタクシを とめることは できない！","けいさんずみ ですとも！","ババリバリッシュ！","ンバーニンガガッ！","ヒュラララ！",
                    "お友達に なっちゃお♪","じゃあ みんな またねえ！","このひとたち ムチャクチャです……","トレーナーとは なにか しりたい","スマートに くずれおちるぜ","いのち ばくはつッ!!","いいんじゃない いいんじゃないの！","あれだよ あれ おみごとだよ！","ぜんりょくでいけー！ ってことよ！","おまちなさいな！","つまり グッド ポイント なわけ！","ざんねん ですが さようなら","にくすぎて むしろ 好きよ","この しれもの が！","イクシャア!!","イガレッカ!!",
                    "フェスサークル ランク 100！",
                ];
                break;
            default:
                const string musical8note = "♪";
                const string linedP = "₽"; //currency Ruble
                res = [ //source:UltraMoon
                    /* (SM)Pokémon House */"There's nothing funny about Nuggets.","The Power of science is awesome.","1, 2, and... Ta-da!","How's the future Champ today?","Why, you!","There! All happy and healthy!","Your Pokémon seems to be very happy!","No thanks!","Would you like to use Cut?","Saving...",
                    /* (SM)Kanto Tent */"Well, I better get going!","Bonjour!","Smell ya later!","Sorry! Bad call!","You better have Burn Heal!","Hoo hah!","Pokémon are for battling!","Slowbro took a snooze...","Shades of your journey await!","You're 10,000 light-years from facing Brock!","Hey! Wait! Don't go out!","Hiya! I'm a Pokémon...","What do you want?","WHAT! This can't be!","Mew!","Be gone... Intruders...",
                    /* (SM)Joht Tent */"Dragonite, Hymer Beam.","Spread the fun around.","I bought an adorable doll with your money.","What are you staring at?","I just don't understand.","Everyone is into Pokémon.","I'm going to train 24 hours a day!","I'm the real deal!","With a jolt of electricity...","For 10 years I chased Suicune.","I am just so deeply moved!","We have finally made it!","...But selling Slowpoke Tails?","Shaoooh!","Gyaaas!","you've taken your first step!",
                    /* (SM)Hoenn Tent */"I'm just the strongest there is right now.","And confoundedly elegant!","You guys need some imagination.","You made a much bigger splash!","You ended up giving me a thrill!","So what am I talking about...","I'm not buying any Dolls.","Take your time and rest up!","Have a blast, Pokémon Trainers!","I got ashes in my eyelashes!","You're sharp!","Number of collisions: 5 times!","Please! Help me out!","I can't see Mirage Island today...","Hyahhn!","Shwahhn!",
                    /* (SM)Sinnoh Tent */"Your will is overwhelming me!","This is it! My trump card!","Trying to monopolize Pokémon just isn't...","See? Just as analyzed.","Gagyagyaah!","Gugyugubah!","It's a "+linedP+"10 million fine if you're late!","I'm going to get tougher...","You'll never be able to stem the flow of time!","Please come!","Your team! I sense your strong aura!","What do you think you're doing?!","The ring is my rolling sea. "+musical8note,"I was just thinking out loud.","My code name, it is Looker.","It's not possible that I lose!",
                    /* (SM)Unova Tent */"Knock it off!","Bravo! Excellent!!","I'll defeat the Champion.","You're about to feel my rage!","Nimbasa's Pokémon can dance a nimble bossa!","Use Bug-type Pokémon!","Stop!","People call me Mr. Medal!","Trainer, do your best, too!","See who's stronger!","Plasbad, for short!","I won't allow anyone to stop me!","I was expecting exactly that kind of move!","Bazzazzazzash!","Preeeeaah!","Haaahraaan!",
                    /* (SM)Kalos Tent */"We'll become friends. "+musical8note,"I'll see you all later!","These people have a few screws loose...","I want to know what a \"Trainer\" is.","When I lose, I go out in style!","Let's give it all we've got!","Fantastic! Just fantastic!","Outstanding!","Try as hard as possible!","Stop right there!","That really hit me right here...","But this is adieu to you all.","You're just too much, you know?","Fool! You silly, unseeing child!","Xsaaaaaah!","Yvaaaaaar!",
                    "I reached Festival Plaza Rank 100!",
                ];
                break;
        }
        CLB_Phrases.Items.Clear();
        CLB_Phrases.Items.Add(res[^1], SAV.Festa.GetFestaPhraseUnlocked(106)); //add Lv100 before TentPhrases
        for (int i = 0; i < res.Length - 1; i++)
            CLB_Phrases.Items.Add(res[i], SAV.Festa.GetFestaPhraseUnlocked(i));

        DateTime dt = SAV.Festa.FestaDate ?? new DateTime(2000, 1, 1);
        CAL_FestaStartDate.Value = CAL_FestaStartTime.Value = dt;

        string[] res2 = ["Rank 4: missions", "Rank 8: facility", "Rank 10: fashion", "Rank 20: rename", "Rank 30: special menu", "Rank 40: BGM", "Rank 50: theme Glitz", "Rank 60: theme Fairy", "Rank 70: theme Tone", "Rank 100: phrase", "Current Rank"];
        CLB_Reward.Items.Clear();
        CLB_Reward.Items.Add(res2[^1], (CheckState)RewardState[SAV.Festa.GetFestPrizeReceived(10)]); //add CurrentRank before const-rewards
        for (int i = 0; i < res2.Length - 1; i++)
            CLB_Reward.Items.Add(res2[i], (CheckState)RewardState[SAV.Festa.GetFestPrizeReceived(i)]);

        for (int i = 0; i < JoinFesta7.FestaFacilityCount; i++)
            f[i] = SAV.Festa.GetFestaFacility(i);

        string[] res3 = ["Meet", "Part", "Moved", "Disappointed"];
        CB_FacilityMessage.Items.Clear();
        CB_FacilityMessage.Items.AddRange(res3);
        string[] res5 =
        [
            "Ace Trainer" + gendersymbols[1],
            "Ace Trainer" + gendersymbols[0],
            "Veteran" + gendersymbols[1],
            "Veteran" + gendersymbols[0],
            "Office Worker" + gendersymbols[0],
            "Office Worker" + gendersymbols[1],
            "Punk Guy",
            "Punk Girl",
            "Breeder" + gendersymbols[0],
            "Breeder" + gendersymbols[1],
            "Youngster",
            "Lass",
        ];
        CB_FacilityNPC.Items.Clear();
        CB_FacilityNPC.Items.AddRange(res5);
        string[] res6 = ["Lottery", "Haunted", "Goody", "Food", "Bouncy", "Fortune", "Dye", "Exchange"];
        string[][] res7 = [
            ["BigDream","GoldRush","TreasureHunt"],
            ["GhostsDen","TrickRoom","ConfuseRay"],
            ["Ball","General","Battle","SoftDrink","Pharmacy"],
            ["Rare","Battle", "FriendshipCafé", "FriendshipParlor"],
            ["Thump","Clink","Stomp"],
            ["Kanto","Johto","Hoenn","Sinnoh","Unova","Kalos","Pokémon"],
            ["Red","Yellow","Green","Blue","Orange","NavyBlue","Purple","Pink"],
            ["Switcheroo"],
        ];

        CB_FacilityType.Items.Clear();
        for (int k = 0; k < RES_FacilityLevelType.Length - (SAV is SAV7USUM ? 0 : 1); k++) // Exchange is US/UM only
        {
            var arr = RES_FacilityLevelType[k];
            for (int j = 0; j < arr.Length; j++)
            {
                var x = res6[k];
                var y = res7[k];
                var name = $"{x} {y[j]}";

                var count = arr[j];
                if (count == 4)
                {
                    CB_FacilityType.Items.Add($"{name} 1");
                    CB_FacilityType.Items.Add($"{name} 3");
                    CB_FacilityType.Items.Add($"{name} 5");
                }
                else
                {
                    for (int i = 0; i < count; i++)
                        CB_FacilityType.Items.Add($"{name} {i + 1}");
                }
            }
        }

        string[] types = ["GTS", "Wonder Trade", "Battle Spot", "Festival Plaza", "mission", "lottery shop", "haunted house"];
        string[] lvl = ["+", "++", "+++"];
        CB_LuckyResult.Items.Clear();
        CB_LuckyResult.Items.Add("none");
        foreach (string type in types)
        {
            foreach (string lv in lvl)
                CB_LuckyResult.Items.Add($"{lv} {type}");
        }

        NUD_Rank.Value = SAV.Festa.FestaRank;
        LoadRankLabel(SAV.Festa.FestaRank);
        NUD_Messages = [NUD_MyMessageMeet, NUD_MyMessagePart, NUD_MyMessageMoved, NUD_MyMessageDissapointed];
        for (int i = 0; i < NUD_Messages.Length; i++)
            NUD_Messages[i].Value = SAV.Festa.GetFestaMessage(i);

        LB_FacilityIndex.SelectedIndex = 0;
        CB_FacilityMessage.SelectedIndex = 0;
        editing = false;

        entry = 0;
        LoadFacility();
    }

    private bool editing;
    private static ReadOnlySpan<byte> RewardState => [ 0, 2, 1 ]; // CheckState.Indeterminate <-> CheckState.Checked
    private readonly int typeMAX;
    private readonly FestaFacility[] f = new FestaFacility[JoinFesta7.FestaFacilityCount];
    private readonly string[] RES_Color = WinFormsTranslator.GetEnumTranslation<FestivalPlazaFacilityColor>(Main.CurrentLanguage);

    private readonly byte[][] RES_FacilityColor = //facility appearance
    [
        [0,1,2,3],//Lottery
        [4,0,5,3],//Haunted
        [1,0,5,3],//Goody
        [6,7,0,3],//Food
        [4,5,8,3],//Bouncy
        [0,1,2,3],//Fortune
        [0,7,8,4,5,1,9,10],//Dye
        [11,1,5,3],//Exchange
    ];

    private readonly byte[][] RES_FacilityLevelType = //3:123 4:135 5:12345
    [
        [5,5,5],
        [5,5,5],
        [3,5,3,3,3],
        [5,4,5,5],
        [5,5,5],
        [4,4,4,4,4,4,4],
        [4,4,4,4,4,4,4,4],
        [3],
    ];

    private int TypeIndexToType(int typeIndex)
    {
        if ((uint)typeIndex > typeMAX + 1)
            return -1;
        return typeIndex switch
        {
            < 0x0F => 0,
            < 0x1E => 1,
            < 0x2F => 2,
            < 0x41 => 3,
            < 0x50 => 4,
            < 0x65 => 5,
            < 0x7D => 6,
            _ => 7,
        };
    }

    private int GetColorCount(int type)
    {
        var colors = RES_FacilityColor;
        if (type >= 0 && type < colors.Length - (SAV is SAV7USUM ? 0 : 1))
            return colors[type].Length - 1;
        return 3;
    }

    private void LoadFacility()
    {
        editing = true;
        var facility = f[entry];
        CB_FacilityType.SelectedIndex =
            CB_FacilityType.Items.Count > facility.Type
                ? facility.Type
                : -1;
        int type = TypeIndexToType(CB_FacilityType.SelectedIndex);
        NUD_FacilityColor.Maximum = GetColorCount(type);
        NUD_FacilityColor.Value = Math.Min(facility.Color, NUD_FacilityColor.Maximum);
        if (type >= 0) LoadColorLabel(type);
        CB_LuckyResult.Enabled = CB_LuckyResult.Visible = L_LuckyResult.Visible = type == 5;
        NUD_Exchangable.Enabled = NUD_Exchangable.Visible = L_Exchangable.Visible = type == 7;
        switch (type)
        {
            case 5:
                int lucky = (facility.UsedLuckyPlace * 3) + facility.UsedLuckyRank - 3;
                if ((uint)lucky >= CB_LuckyResult.Items.Count)
                    lucky = 0;
                CB_LuckyResult.SelectedIndex = lucky;
                break;
            case 7:
                NUD_Exchangable.Value = facility.ExchangeLeftCount;
                break;
        }
        CB_FacilityNPC.SelectedIndex =
            CB_FacilityNPC.Items.Count > facility.NPC
                ? facility.NPC
                : 0;
        CHK_FacilityIntroduced.Checked = facility.IsIntroduced;
        TB_OTName.Text = facility.OriginalTrainerName;
        LoadOTlabel(facility.Gender);
        if (CB_FacilityMessage.SelectedIndex >= 0)
            LoadFMessage(CB_FacilityMessage.SelectedIndex);

        var obj = f[entry];
        TB_UsedFlags.Text = obj.UsedFlags.ToString("X8");
        TB_UsedStats.Text = obj.UsedRandStat.ToString("X8");
        TB_FacilityID.Text = Util.GetHexStringFromBytes(obj.TrainerFesID);
        editing = false;
    }

    private void Save()
    {
        SAV.Festa.SetFestaPhraseUnlocked(106, CLB_Phrases.GetItemChecked(0));
        for (int i = 1; i < CLB_Phrases.Items.Count; i++)
            SAV.Festa.SetFestaPhraseUnlocked(i - 1, CLB_Phrases.GetItemChecked(i));

        SAV.SetRecord(038, (int)NUD_FC_Used.Value);
        SAV.Festa.FestaCoins = (int)NUD_FC_Current.Value;
        SAV.Festa.FestaDate = new DateTime(CAL_FestaStartDate.Value.Year, CAL_FestaStartDate.Value.Month, CAL_FestaStartDate.Value.Day, CAL_FestaStartTime.Value.Hour, CAL_FestaStartTime.Value.Minute, CAL_FestaStartTime.Value.Second);

        SAV.Festa.SetFestaPrizeReceived(10, RewardState[(int)CLB_Reward.GetItemCheckState(0)]);
        for (int i = 1; i < CLB_Reward.Items.Count; i++)
            SAV.Festa.SetFestaPrizeReceived(i - 1, RewardState[(int)CLB_Reward.GetItemCheckState(i)]);

        SaveFacility();
        if (SAV is SAV7USUM)
            SaveBattleAgency();
    }

    private void LoadBattleAgency()
    {
        p[0] = SAV.GetStoredSlot(SAV.Data.AsSpan(0x6C200));
        p[1] = SAV.GetPartySlot(SAV.Data.AsSpan(0x6C2E8));
        p[2] = SAV.GetPartySlot(SAV.Data.AsSpan(0x6C420));
        LoadPictureBox();
        B_ImportParty.Visible = SAV.HasParty;
        CHK_Choosed.Checked = SAV.GetFlag(0x6C55E, 1);
        CHK_TrainerInvited.Checked = IsTrainerInvited();
        ushort valus = ReadUInt16LittleEndian(SAV.Data.AsSpan(0x6C55C));
        int grade = (valus >> 6) & 0x3F;
        NUD_Grade.Value = grade;
        int max = (Math.Min(49, grade) / 10 * 3) + 2;
        int defeated = valus >> 12;
        NUD_Defeated.Value = defeated > max ? max : defeated;
        NUD_Defeated.Maximum = max;
        NUD_DefeatMon.Value = ReadUInt16LittleEndian(SAV.Data.AsSpan(0x6C558));
        for (int i = 0; i < NUD_Trainers.Length; i++)
        {
            int j = GetSavData16(0x6C56C + (0x14 * i));
            var m = (int)NUD_Trainers[i].Maximum;
            NUD_Trainers[i].Value = (uint)j > m ? m : j;
        }
        B_AgentGlass.Enabled = (SAV.Fashion.Data[0xD0] & 1) == 0;
    }

    private void LoadPictureBox()
    {
        for (int i = 0; i < 3; i++)
            PBs[i].Image = p[i].Sprite(SAV, flagIllegal: true);
    }

    private readonly NumericUpDown[] NUD_Trainers = new NumericUpDown[3];
    private ushort GetSavData16(int offset) => ReadUInt16LittleEndian(SAV.Data.AsSpan(offset));
    private const ushort InvitedValue = 0x7DFF;
    private readonly PKM[] p = new PKM[3];
    private readonly PictureBox[] PBs = new PictureBox[3];
    private bool IsTrainerInvited() => (GetSavData16(0x6C3EE) & InvitedValue) == InvitedValue && (GetSavData16(0x6C526) & InvitedValue) == InvitedValue;

    private void SaveBattleAgency()
    {
        SAV.SetFlag(0x6C55E, 1, CHK_Choosed.Checked);
        if (IsTrainerInvited() != CHK_TrainerInvited.Checked)
        {
            WriteUInt16LittleEndian(SAV.Data.AsSpan(0x6C3EE), (ushort)(CHK_TrainerInvited.Checked ? GetSavData16(0x6C3EE) | InvitedValue : 0));
            WriteUInt16LittleEndian(SAV.Data.AsSpan(0x6C526), (ushort)(CHK_TrainerInvited.Checked ? GetSavData16(0x6C526) | InvitedValue : 0));
        }
        SAV.SetData(p[0].EncryptedBoxData, 0x6C200);
        SAV.SetData(p[1].EncryptedPartyData, 0x6C2E8);
        SAV.SetData(p[2].EncryptedPartyData, 0x6C420);

        var gradeDefeated = ((((int)NUD_Defeated.Value & 0xF) << 12) | (((int)NUD_Grade.Value & 0x3F) << 6) | (SAV.Data[0x6C55C] & 0x3F));
        WriteUInt16LittleEndian(SAV.Data.AsSpan(0x6C558), (ushort)NUD_DefeatMon.Value);
        WriteUInt16LittleEndian(SAV.Data.AsSpan(0x6C55C), (ushort)gradeDefeated);
        for (int i = 0; i < NUD_Trainers.Length; i++)
            WriteUInt16LittleEndian(SAV.Data.AsSpan(0x6C56C + (0x14 * i)), (ushort)NUD_Trainers[i].Value);
        SAV.Festa.FestivalPlazaName = TB_PlazaName.Text;
    }

    private void NUD_FC_ValueChanged(object sender, EventArgs e)
    {
        if (editing) return;
        L_FC_CollectedV.Text = (NUD_FC_Current.Value + NUD_FC_Used.Value).ToString(CultureInfo.InvariantCulture);
    }

    private void B_Cancel_Click(object sender, EventArgs e)
    {
        Close();
    }

    private void B_Save_Click(object sender, EventArgs e)
    {
        Save();
        Origin.CopyChangesFrom(SAV);
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

        var d = new TrashEditor(tb, SAV, SAV.Generation);
        d.ShowDialog();
        tb.Text = d.FinalString;
    }

    private readonly string[] gendersymbols = ["♂", "♀"];

    private void LoadOTlabel(int b)
    {
        Label_OTGender.Text = gendersymbols[b & 1];
        Label_OTGender.ForeColor = b == 1 ? Color.Red : Color.Blue;
    }

    private void Label_OTGender_Click(object sender, EventArgs e)
    {
        if (entry < 0)
            return;
        var b = f[entry].Gender;
        b ^= 1;
        f[entry].Gender = b;
        LoadOTlabel(b);
    }

    private void LoadFMessage(int fmIndex) => NUD_FacilityMessage.Value = f[entry].GetMessage(fmIndex);

    private void CB_FacilityMessage_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (editing)
            return;

        int fmIndex = CB_FacilityMessage.SelectedIndex;
        if (fmIndex < 0)
            return;
        if (entry < 0)
            return;

        editing = true;
        LoadFMessage(fmIndex);
        editing = false;
    }

    private void NUD_FacilityMessage_ValueChanged(object sender, EventArgs e)
    {
        if (editing)
            return;

        int fmIndex = CB_FacilityMessage.SelectedIndex;
        if (fmIndex < 0)
            return;
        if (entry < 0)
            return;

        f[entry].SetMessage(fmIndex, (ushort)NUD_FacilityMessage.Value);
    }

    private void HexTextBox_TextChanged(object sender, EventArgs e)
    {
        if (editing)
            return;
        if (entry < 0)
            return;

        string t = Util.GetOnlyHex(((TextBox)sender).Text);
        if (string.IsNullOrWhiteSpace(t))
            t = "0";
        int maxlen = sender == TB_FacilityID ? 12 << 1 : 4 << 1;
        if (t.Length > maxlen)
        {
            t = t[..maxlen];
            editing = true;
            ((TextBox)sender).Text = t;
            editing = false;
            System.Media.SystemSounds.Asterisk.Play();
        }
        if (sender == TB_UsedFlags)
        {
            f[entry].UsedFlags = Convert.ToUInt32(t, 16);
        }
        else if (sender == TB_UsedStats)
        {
            f[entry].UsedRandStat = Convert.ToUInt32(t, 16);
        }
        else if (sender == TB_FacilityID)
        {
            var updated = Util.GetBytesFromHexString(t.PadLeft(24, '0'));
            updated.CopyTo(f[entry].TrainerFesID);
        }
    }

    private void LoadColorLabel(int type) => L_FacilityColorV.Text = RES_Color[RES_FacilityColor[type][(int)NUD_FacilityColor.Value]];

    private void NUD_FacilityColor_ValueChanged(object sender, EventArgs e)
    {
        if (editing)
            return;
        if (entry < 0)
            return;
        f[entry].Color = (byte)NUD_FacilityColor.Value;
        int type = TypeIndexToType(CB_FacilityType.SelectedIndex);
        if (type < 0)
            return;

        editing = true;
        LoadColorLabel(type);
        editing = false;
    }

    private void CB_FacilityType_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (editing)
            return;
        if (entry < 0)
            return;

        int typeIndex = CB_FacilityType.SelectedIndex;
        if (typeIndex < 0)
            return;

        var facility = f[entry];
        facility.Type = typeIndex;
        // reset color
        int type = TypeIndexToType(typeIndex);
        int colorCount = GetColorCount(type);
        editing = true;
        if (colorCount < NUD_FacilityColor.Value)
        {
            NUD_FacilityColor.Value = colorCount;
            facility.Color = colorCount;
        }
        NUD_FacilityColor.Maximum = colorCount;
        LoadColorLabel(type);
        // reset forms
        CB_LuckyResult.Enabled = CB_LuckyResult.Visible = L_LuckyResult.Visible = type == 5;
        NUD_Exchangable.Enabled = NUD_Exchangable.Visible = L_Exchangable.Visible = type == 7;
        switch (type)
        {
            case 5:
                int lucky = (facility.UsedLuckyPlace * 3) + facility.UsedLuckyRank - 3;
                if (lucky < 0 || lucky >= CB_LuckyResult.Items.Count) lucky = 0;
                CB_LuckyResult.SelectedIndex = lucky;
                break;
            case 7:
                NUD_Exchangable.Value = facility.ExchangeLeftCount;
                break;
        }
        editing = false;
    }

    private void SaveFacility()
    {
        if (entry < 0)
            return;
        var facility = f[entry];
        if (CB_FacilityType.SelectedIndex >= 0)
            facility.Type = CB_FacilityType.SelectedIndex;
        facility.Color = (byte)NUD_FacilityColor.Value;
        facility.OriginalTrainerName = TB_OTName.Text;
        if (CB_FacilityNPC.SelectedIndex >= 0)
            facility.NPC = CB_FacilityNPC.SelectedIndex;
        facility.IsIntroduced = CHK_FacilityIntroduced.Checked;
        int type = TypeIndexToType(facility.Type);
        facility.ExchangeLeftCount = type == 7 ? (byte)NUD_Exchangable.Value : 0;
        int lucky = CB_LuckyResult.SelectedIndex - 1;
        bool writeLucky = type == 5 && lucky >= 0;
        facility.UsedLuckyRank = writeLucky ? (lucky % 3) + 1 : 0;
        facility.UsedLuckyPlace = writeLucky ? (lucky / 3) + 1 : 0;
    }

    private void LoadRankLabel(int rank) => L_RankFC.Text = GetRankText(rank);

    private static string GetRankText(int rank)
    {
        if (rank < 1) return string.Empty;
        if (rank == 1) return "0 - 5";
        if (rank == 2) return "6 - 15";
        if (rank == 3) return "16 - 30";
        if (rank <= 10)
        {
            int i = ((rank - 1) * (rank - 2) * 5) + 1;
            return $"{i} - {i + ((rank - 1) * 10) - 1}";
        }
        if (rank <= 20)
        {
            int i = (rank * 100) - 649;
            return $"{i} - {i + 99}";
        }
        if (rank <= 70)
        {
            int j = (rank - 1) / 10;
            int i = (rank * ((j * 30) + 60)) - ((j * j * 150) + (j * 180) + 109); // 30 * (rank - 5 * j + 4) * (j + 2) - 349;
            return $"{i} - {i + (j * 30) + 59}";
        }
        if (rank <= 100)
        {
            int i = (rank * 270) - 8719;
            return $"{i} - {i + 269}";
        }
        if (rank <= 998)
        {
            int i = (rank * 300) - 11749;
            return $"{i} - {i + 299}";
        }
        if (rank == 999)
            return "287951 - ";
        return string.Empty;
    }

    private void NUD_Rank_ValueChanged(object sender, EventArgs e)
    {
        if (editing) return;
        int rank = (int)NUD_Rank.Value;
        SAV.Festa.FestaRank = (ushort)rank;
        LoadRankLabel(rank);
    }

    private readonly NumericUpDown[] NUD_Messages;

    private void NUD_MyMessage_ValueChanged(object sender, EventArgs e)
    {
        if (editing)
            return;

        int mmIndex = Array.IndexOf(NUD_Messages, (NumericUpDown)sender);
        if (mmIndex < 0)
            return;

        SAV.Festa.SetFestaMessage(mmIndex, (ushort)((NumericUpDown)sender).Value);
    }

    private void CHK_FacilityIntroduced_CheckedChanged(object sender, EventArgs e)
    {
        if (editing)
            return;
        if (entry < 0)
            return;

        f[entry].IsIntroduced = CHK_FacilityIntroduced.Checked;
    }

    private void TB_OTName_TextChanged(object sender, EventArgs e)
    {
        if (editing)
            return;
        if (entry < 0)
            return;

        f[entry].OriginalTrainerName = TB_OTName.Text;
    }

    private void LB_FacilityIndex_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (editing)
            return;

        SaveFacility();
        entry = LB_FacilityIndex.SelectedIndex;
        if (entry < 0)
            return;
        LoadFacility();
    }

    private void LB_FacilityIndex_DrawItem(object? sender, DrawItemEventArgs? e)
    {
        if (e is null || sender is not ListBox lb)
            return;
        e.DrawBackground();

        var font = e.Font ?? Font;
        e.Graphics.DrawString(lb.Items[e.Index].ToString(), font, new SolidBrush(e.ForeColor), new RectangleF(e.Bounds.X, e.Bounds.Y + ((e.Bounds.Height - 12) >> 1), e.Bounds.Width, 12));
        e.DrawFocusRectangle();
    }

    private void B_DelVisitor_Click(object sender, EventArgs e)
    {
        if (entry < 0)
            return;
        var facility = f[entry];
        // there is an unknown value when not introduced...no reproducibility, just mistake?
        if (facility.IsIntroduced)
            facility.ClearTrainerFesID();
        facility.IsIntroduced = false;
        facility.OriginalTrainerName = string.Empty;
        facility.Gender = 0;
        for (int i = 0; i < 4; i++)
            facility.SetMessage(i, 0);
        LoadFacility();
    }

    private string GetSpeciesNameFromPKM(PKM pk) => SpeciesName.GetSpeciesName(pk.Species, SAV.Language);

    private void B_ImportParty_Click(object sender, EventArgs e)
    {
        if (!SAV.HasParty)
            return;
        var party = SAV.PartyData;
        string msg = string.Empty;
        for (int i = 0; i < 3; i++)
        {
            if (i < party.Count)
                msg += $"{Environment.NewLine}{GetSpeciesNameFromPKM(p[i])} -> {GetSpeciesNameFromPKM(party[i])}";
            else
                msg += $"{Environment.NewLine}not replaced: {GetSpeciesNameFromPKM(p[i])}";
        }
        if (DialogResult.Yes != WinFormsUtil.Prompt(MessageBoxButtons.YesNo, "Replace PKM?", msg))
            return;

        for (int i = 0, min = Math.Min(3, party.Count); i < min; i++)
            p[i] = party[i];
        LoadPictureBox();
    }

    private void MnuSave_Click(object sender, EventArgs e)
    {
        var pb = WinFormsUtil.GetUnderlyingControl<PictureBox>(sender);
        int i = Array.IndexOf(PBs, pb);
        if (i < 0)
            return;
        WinFormsUtil.SavePKMDialog(p[i]);
    }

    private void NUD_Grade_ValueChanged(object sender, EventArgs e)
    {
        if (editing)
            return;
        int max = (Math.Min(49, (int)NUD_Grade.Value) / 10 * 3) + 2;
        editing = true;
        if (NUD_Defeated.Value > max)
            NUD_Defeated.Value = max;
        NUD_Defeated.Maximum = max;
        editing = false;
    }

    private void NUD_Exchangable_ValueChanged(object sender, EventArgs e)
    {
        if (editing)
            return;
        if (entry < 0)
            return;
        f[entry].ExchangeLeftCount = (byte)NUD_Exchangable.Value;
    }

    private void CB_LuckyResult_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (editing)
            return;
        if (entry < 0)
            return;
        int lucky = CB_LuckyResult.SelectedIndex;
        if (lucky-- < 0)
            return;
        // both 0 if "none"
        f[entry].UsedLuckyRank = lucky < 0 ? 0 : (lucky % 3) + 1;
        f[entry].UsedLuckyPlace = lucky < 0 ? 0 : (lucky / 3) + 1;
    }

    private void B_AgentGlass_Click(object sender, EventArgs e)
    {
        if (NUD_Grade.Value < 30 && DialogResult.Yes != WinFormsUtil.Prompt(MessageBoxButtons.YesNo, "Agent Sunglasses is reward of Grade 30.", "Continue?"))
            return;
        SAV.Fashion.GiveAgentSunglasses();
        B_AgentGlass.Enabled = false;
        System.Media.SystemSounds.Asterisk.Play();
    }
}

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Drawing;
using System.Windows.Forms;
using PKHeX.Core;
using static PKHeX.Core.MessageStrings;

namespace PKHeX.WinForms;

/// <summary>
/// Battle Video editor for Generation 4 save files.
/// </summary>
public partial class SAV_DLC4 : Form
{
    private const int BattleVideoCount = 4;

    private readonly SAV4 Origin;
    private readonly SAV4 SAV;

    public SAV_DLC4(SAV4 sav)
    {
        InitializeComponent();
        WinFormsUtil.TranslateInterface(this, Main.CurrentLanguage);
        Origin = sav;
        SAV = (SAV4)sav.Clone();
        Debug.Assert(sav is SAV4Pt or SAV4HGSS); // none in DP

        LoadBattleVideos();
        if (LB_BattleVideos.Items.Count != 0)
            LB_BattleVideos.SelectedIndex = 0;
    }

    private void LoadBattleVideos()
    {
        LB_BattleVideos.Items.Clear();

        for (int i = 0; i < BattleVideoCount; i++)
        {
            var video = SAV.GetBattleVideo(i);
            if (video is null)
            {
                LB_BattleVideos.Items.Add($"{i + 1:00} - N/A");
                continue;
            }

            var name = video.GetName();
            LB_BattleVideos.Items.Add($"{i + 1:00} - {name}");
        }
    }

    private void LB_BattleVideos_SelectedIndexChanged(object sender, EventArgs e)
    {
        LoadSelectedBattleVideo();
    }

    private void LoadSelectedBattleVideo()
    {
        TL_Teams.SuspendLayout();
        TL_Teams.Controls.Clear();
        TL_Teams.RowStyles.Clear();
        TL_Teams.RowCount = 1;

        int index = LB_BattleVideos.SelectedIndex;
        if ((uint)index >= BattleVideoCount)
        {
            L_VideoTitle.Text = MsgBattleVideo;
            L_VideoStatus.Text = MsgBattleVideoInvalidIndex;
            SetBattleVideoControlsEnabled(false);
            TL_Teams.ResumeLayout();
            return;
        }

        var video = SAV.GetBattleVideo(index);
        if (video is null)
        {
            L_VideoTitle.Text = string.Format(MsgBattleVideoIndex, index + 1);
            L_VideoStatus.Text = MsgBattleVideoInvalidSlot;
            SetBattleVideoControlsEnabled(false);
            TL_Teams.ResumeLayout();
            return;
        }

        SetBattleVideoControlsEnabled(true);

        var names = video.GetTrainerNames();
        var teams = video.GetTeams();
        var activePlayers = Math.Max(2, teams.Count(z => z.Length != 0));

        L_VideoTitle.Text = string.Format(MsgBattleVideoIndex, index + 1);
        L_VideoStatus.Text = GetStatusText(video, names, teams);

        TL_Teams.ColumnCount = 2;
        TL_Teams.ColumnStyles.Clear();
        TL_Teams.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
        TL_Teams.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));

        TL_Teams.RowCount = (activePlayers + 1) / 2;
        TL_Teams.RowStyles.Clear();
        for (int row = 0; row < TL_Teams.RowCount; row++)
            TL_Teams.RowStyles.Add(new RowStyle(SizeType.AutoSize));

        for (int trainer = 0; trainer < BattleVideo4.TrainerCount; trainer++)
        {
            var team = teams[trainer];
            if (team.Length == 0 && trainer >= activePlayers)
                continue;

            var name = names[trainer];
            if (string.IsNullOrWhiteSpace(name))
                name = team.FirstOrDefault()?.OriginalTrainerName;
            if (string.IsNullOrWhiteSpace(name))
                name = "Unknown Player";

            var panel = CreatePlayerPanel(trainer + 1, name, team);
            int row = trainer / 2;
            int column = trainer % 2;
            TL_Teams.Controls.Add(panel, column, row);
        }

        TL_Teams.ResumeLayout(true);
    }

    private static string GetStatusText(BattleVideo4 video, IReadOnlyList<string> names, PK4[][] teams)
    {
        var members = teams.Sum(z => z.Length);
        var state = video.IsDecrypted ? "Decrypted view" : "Encrypted";
        var checksum = video.ChecksumValid ? "checksum valid" : "checksum invalid";
        var players = names.Count(z => !string.IsNullOrWhiteSpace(z)).ToString();
        return $"{state} • {players}/4 players • {members} Pokémon - {checksum} - Block ID 0x{video.BlockID:X4}";
    }

    private static GroupBox CreatePlayerPanel(int playerIndex, string playerName, ReadOnlySpan<PK4> team)
    {
        var group = new GroupBox
        {
            AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowAndShrink,
            Dock = DockStyle.Fill,
            Margin = new Padding(5),
            Padding = new Padding(8),
            Text = string.Format(MsgBattleVideoPlayerIndex, playerIndex, playerName),
        };

        var table = new TableLayoutPanel
        {
            AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowAndShrink,
            ColumnCount = 4,
            Dock = DockStyle.Fill,
            Margin = new Padding(0),
            Padding = new Padding(0),
        };
        table.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 38));
        table.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 38));
        table.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 42));
        table.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 52));

        AddHeader(table, "#", 0);
        AddHeader(table, "Pokémon", 1);
        AddHeader(table, "Nickname", 2);
        AddHeader(table, "Level", 3);

        for (int i = 0; i < team.Length; i++)
        {
            var pk = team[i];
            AddCell(table, (i + 1).ToString(), i + 1, 0, HorizontalAlignment.Center);
            AddCell(table, GetSpeciesName(pk.Species), i + 1, 1);
            AddCell(table, string.IsNullOrWhiteSpace(pk.Nickname) ? "-" : pk.Nickname, i + 1, 2);
            AddCell(table, pk.CurrentLevel.ToString(), i + 1, 3, HorizontalAlignment.Center);
        }

        if (team.Length == 0)
        {
            table.RowCount = 2;
            table.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            AddCell(table, "-", 1, 0);
            table.Controls[^1].Margin = new Padding(2, 4, 2, 4);
            table.SetColumnSpan(table.Controls[^1], 4);
        }

        group.Controls.Add(table);
        return group;
    }

    private static string GetSpeciesName(ushort species)
    {
        if (species < GameInfo.Strings.Species.Count)
            return GameInfo.Strings.Species[species];
        return $"{species:000}";
    }

    private static void AddHeader(TableLayoutPanel table, string text, int column)
    {
        table.Controls.Add(new Label
        {
            AutoSize = true,
            Dock = DockStyle.Fill,
            Margin = new Padding(2, 2, 2, 4),
            Text = text,
            TextAlign = ContentAlignment.MiddleLeft,
        }, column, 0);
    }

    private static void AddCell(TableLayoutPanel table, string text, int row, int column, HorizontalAlignment alignment = HorizontalAlignment.Left)
    {
        if (table.RowCount <= row)
            table.RowCount = row + 1;
        if (table.RowStyles.Count < row + 1)
            table.RowStyles.Add(new RowStyle(SizeType.AutoSize));

        var label = new Label
        {
            AutoSize = true,
            Dock = DockStyle.Fill,
            Margin = new Padding(2, 2, 2, 2),
            Text = text,
            TextAlign = alignment switch
            {
                HorizontalAlignment.Center => ContentAlignment.MiddleCenter,
                HorizontalAlignment.Right => ContentAlignment.MiddleRight,
                _ => ContentAlignment.MiddleLeft,
            },
        };
        table.Controls.Add(label, column, row);
    }

    private void B_Import_Click(object sender, EventArgs e)
    {
        int index = LB_BattleVideos.SelectedIndex;
        if ((uint)index >= BattleVideoCount)
            return;

        var target = SAV.GetBattleVideo(index);
        if (target is null)
        {
            WinFormsUtil.Error(MsgBattleVideoInvalidSlot);
            return;
        }

        using var ofd = new OpenFileDialog();
        InitializeFileDialog(ofd, index, MsgBattleVideoImport);
        if (ofd.ShowDialog() != DialogResult.OK)
            return;

        try
        {
            var data = File.ReadAllBytes(ofd.FileName);
            if (!BattleVideo4.IsValid(data))
            {
                WinFormsUtil.Error(string.Format(MsgBattleVideoInvalidSize, BattleVideo4.SIZE_USED));
                return;
            }

            var encryptionState = BattleVideo4.DetectEncryption(data);
            if (encryptionState == BattleVideo4DecryptionState.Invalid)
            {
                WinFormsUtil.Error(MsgBattleVideoInvalid);
                return;
            }

            var imported = new BattleVideo4(data)
            {
                IsDecrypted = encryptionState == BattleVideo4DecryptionState.Decrypted,
                // The slot owns its block metadata. Preserve the destination's magic/revision/id while replacing the actual Battle Video payload.
                Magic = target.Magic,
                Revision = target.Revision,
                BlockSize = target.BlockSize,
                BlockID = target.BlockID
            };

            imported.RefreshChecksums();
            imported.Encrypt();

            imported.Data.CopyTo(target.Data);

            LoadBattleVideos();
            LB_BattleVideos.SelectedIndex = index;
            WinFormsUtil.Asterisk();
        }
        catch (Exception ex)
        {
            WinFormsUtil.Error(ex.Message);
        }
    }

    private static void InitializeFileDialog(FileDialog fd, int index, string title)
    {
        fd.Filter = $@"{MsgBattleVideo}|*.{BattleVideo4.Extension}";
        fd.FileName = $"{MsgBattleVideo} {index + 1:00}.{BattleVideo4.Extension}";
        fd.Title = title;
    }

    private void B_Export_Click(object sender, EventArgs e)
    {
        int index = LB_BattleVideos.SelectedIndex;
        if ((uint)index >= BattleVideoCount)
            return;

        var video = SAV.GetBattleVideo(index);
        if (video is null)
            return;

        var data = video.Data.ToArray();

        using var sfd = new SaveFileDialog();
        InitializeFileDialog(sfd, index, MsgBattleVideoExport);
        sfd.FileName = $"{PathUtil.CleanFileName(video.ToString())}.{BattleVideo4.Extension}";
        if (sfd.ShowDialog() != DialogResult.OK)
            return;

        if (CHK_ExportDecrypted.Checked)
        {
            var export = new BattleVideo4(data) { IsDecrypted = false };
            export.Decrypt();
        }

        File.WriteAllBytes(sfd.FileName, data);
        WinFormsUtil.Asterisk();
    }

    private void SetBattleVideoControlsEnabled(bool enabled)
    {
        B_Import.Enabled = enabled;
        B_Export.Enabled = enabled;
        CHK_ExportDecrypted.Enabled = enabled;
    }

    private void B_Save_Click(object sender, EventArgs e)
    {
        Origin.CopyChangesFrom(SAV);
        Close();
    }

    private void B_Cancel_Click(object sender, EventArgs e) => Close();
}

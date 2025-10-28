using System;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Forms;
using PKHeX.Core;

namespace PKHeX.WinForms;

public partial class SAV_DLC5 : Form
{
    private readonly SAV5 Origin;
    private readonly SAV5 SAV;

    private readonly CGearBackground bg;
    private readonly PokeDexSkin5 dex;
    private const string CGearFilterBW = $"PokeStock C-Gear Skin Background|*.{CGearBackgroundBW.Extension}";
    private const string CGearFilterB2W2 = $"C-Gear Background|*.{CGearBackgroundB2W2.Extension}";

    private const string PWTFileName = "PWT";
    private const string MusicalShowFileName = "Musical Show";
    private const string BattleVideoFileName = "Generation 5 Battle Video";
    private const string PokeStarMovieFileName = "Pokestar Studio Movie";
    private const string MemoryLinkFileName = "Memory Link";
    private const string PokeDexFileName = "Pokédex Skin";
    private const string BattleTestFileName = "Battle Test";

    private const string MemoryLinkExtension = "ml5";

    public SAV_DLC5(SAV5 sav)
    {
        InitializeComponent();
        WinFormsUtil.TranslateInterface(this, Main.CurrentLanguage);
        SAV = (SAV5)(Origin = sav).Clone();

        var data = SAV.CGearSkinData;
        bg = sav is SAV5BW ? new CGearBackgroundBW(data) : new CGearBackgroundB2W2(data);
        dex = new PokeDexSkin5(sav.PokedexSkinData);

        LoadTabs();

        Tab_BattleTest.Controls.Add(new Label
        {
            Text = "Not working. Needs research.",
            ForeColor = Color.Red, Location = new(20,20),
            AutoSize = true,
        });
    }

    private void LoadTabs()
    {
        if (SAV is not SAV5B2W2 b2w2)
        {
            TC_Tabs.Controls.Remove(Tab_PWT);
            TC_Tabs.Controls.Remove(Tab_Pokestar);
        }
        else
        {
            LoadPWT(b2w2);
            LoadPokestar(b2w2);
        }

        LoadCGear(SAV);
        LoadPokedexSkin(SAV);
        LoadBattleVideos(SAV);
        LoadMusical(SAV);
        LoadMemoryLink(SAV);
        LoadBattleTest(SAV);
    }

    private void LoadCGear(SAV5 sav)
    {
        if (bg.IsUninitialized)
        {
            B_ExportCGB.Enabled = B_ExportPNG.Enabled = false;
            PB_CGearBackground.BackColor = Color.Black;
            return;
        }
        PB_CGearBackground.Image = CGearImage.GetBitmap(bg);
    }

    private void LoadPokedexSkin(SAV5 sav)
    {
        if (dex.IsUninitialized)
        {
            B_PokeDexSkinSave.Enabled = false;
            PB_PokeDexBackground.BackColor = PB_PokeDexForeground.BackColor = Color.Black;
            return;
        }
        LoadPokedexSkin(dex.Data);
    }

    private void LoadPokedexSkin(ReadOnlySpan<byte> data)
    {
        // not implemented
    }

    private void LoadBattleTest(SAV5 sav)
    {
        // not implemented
    }

    private void LoadMusical(SAV5 sav) { }
    private void LoadMemoryLink(SAV5 sav) { }

    private void LoadBattleVideos(SAV5 sav)
    {
        for (int i = 0; i < 4; i++)
        {
            var data = sav.GetBattleVideo(i);
            var bvid = new BattleVideo5(data);
            var name = bvid.IsUninitialized ? "Empty" : bvid.GetTrainerNames();
            LB_BattleVideo.Items.Add($"{i:00} - {name}");
        }
        LB_BattleVideo.SelectedIndex = 0;
    }

    private void LoadPokestar(SAV5B2W2 sav)
    {
        for (int i = 0; i < SAV5B2W2.PokestarCount; i++)
        {
            var data = sav.GetPokestarMovie(i);
            var movie = new PokestarMovie5(data);
            LB_Pokestar.Items.Add($"{i + 1:00} - {movie.Name}");
        }
        LB_Pokestar.SelectedIndex = 0;
    }

    private void LoadPWT(SAV5B2W2 sav)
    {
        for (int i = 0; i < SAV5B2W2.PWTCount; i++)
        {
            var data = sav.GetPWT(i);
            var pwt = new WorldTournament5(data);
            var name = pwt.Name;
            if (string.IsNullOrWhiteSpace(name))
                name = "Empty";
            LB_PWT.Items.Add($"{i + 1:00} - {name}");
        }
        LB_PWT.SelectedIndex = 0;
    }

    private string? LastImportedFile;

    private bool ImportFile(string extension, string name, int expectSize, out byte[] data, string? initialName = null, int otherSize = -1)
    {
        data = [];

        using var ofd = new OpenFileDialog();
        ofd.Filter = $"{name}|*.{extension}";
        ofd.FileName = $"{initialName ?? name}.{extension}";
        if (ofd.ShowDialog() != DialogResult.OK)
            return false;
        var fi = new FileInfo(ofd.FileName);
        if (fi.Length != expectSize && fi.Length != otherSize)
        {
            WinFormsUtil.Error($"Invalid file size. Expected {expectSize} bytes, got {fi.Length} bytes.");
            return false;
        }

        data = File.ReadAllBytes(ofd.FileName);
        if (data.Length == otherSize)
            Array.Resize(ref data, expectSize);

        System.Media.SystemSounds.Asterisk.Play();
        LastImportedFile = ofd.FileName;
        return true;
    }

    private static void ExportFile(string extension, string name, ReadOnlySpan<byte> data, string? initialName = null)
    {
        using var sfd = new SaveFileDialog();
        sfd.Filter = $"{name}|*.{extension}";
        if (string.IsNullOrWhiteSpace(initialName))
            initialName = string.IsNullOrWhiteSpace(name) ? "Empty" : name;
        sfd.FileName = $"{initialName}.{extension}";
        if (sfd.ShowDialog() != DialogResult.OK)
            return;
        File.WriteAllBytes(sfd.FileName, data);
    }

    private void B_ImportPNGCGear_Click(object sender, EventArgs e)
    {
        using var ofd = new OpenFileDialog();
        ofd.Filter = "PNG File|*.png";
        ofd.FileName = "Background.png";
        if (ofd.ShowDialog() != DialogResult.OK)
            return;

        try
        {
            using var img = (Bitmap)Image.FromFile(ofd.FileName);
            if (!IsInputCorrect<CGearBackground>(img, out var msg))
            {
                WinFormsUtil.Alert(msg);
                return;
            }
            var result = CGearImage.GetCGearBackground(img, bg);
            SAV.SetCGearSkin(bg.Data);
            PB_CGearBackground.Image = CGearImage.GetBitmap(bg); // regenerate rather than reuse input
            B_ExportCGB.Enabled = B_ExportPNG.Enabled = true;
            if (CheckResult<CGearBackground>(result, out msg))
                System.Media.SystemSounds.Asterisk.Play();
            else
                WinFormsUtil.Alert(msg);
        }
        catch (Exception ex)
        {
            WinFormsUtil.Error(ex.Message);
        }
    }

    private static bool IsInputCorrect<T>(Bitmap img, [NotNullWhen(false)] out string? msg) where T : ITiledImage
    {
        if (img.Width != T.Width || img.Height != T.Height)
        {
            msg = $"Incorrect image dimensions. Expected {T.Width}x{T.Height}";
            return false;
        }
        if (img.PixelFormat != PixelFormat.Format32bppArgb)
        {
            msg = $"Incorrect image format. Expected {PixelFormat.Format32bppArgb}";
            return false;
        }
        msg = null;
        return true;
    }

    private static bool CheckResult<T>(TiledImageStat result, [NotNullWhen(false)] out string? msg)
        where T : ITiledImage
    {
        bool tooManyColors = result.ColorCount > T.ColorCount;
        bool tooManyTiles = result.TileCount > T.TilePoolCount;
        if (!tooManyColors && !tooManyTiles)
        {
            msg = null;
            return true; // Success
        }

        msg = "";
        if (tooManyColors)
            msg += $"Too many colors. Expected: {T.ColorCount}, received {result.ColorCount}";
        if (tooManyTiles)
            msg += (msg.Length != 0 ? Environment.NewLine : "") + $"Too many tiles. Expected {T.TilePoolCount}, received {result.TileCount}";
        return false;
    }

    private void B_ExportPNGCGear_Click(object sender, EventArgs e)
    {
        if (PB_CGearBackground.Image is not { } img)
            return;
        using var sfd = new SaveFileDialog();
        sfd.Filter = "PNG File|*.png";
        sfd.FileName = "Background.png";
        if (sfd.ShowDialog() != DialogResult.OK)
            return;

        img.Save(sfd.FileName, ImageFormat.Png);
    }

    private void B_ImportCGB_Click(object sender, EventArgs e)
    {
        using var ofd = new OpenFileDialog();
        bool isBW = SAV is SAV5BW;
        ofd.Filter = isBW ? $"{CGearFilterBW}|{CGearFilterB2W2}" : $"{CGearFilterB2W2}|{CGearFilterBW}";

        if (ofd.ShowDialog() != DialogResult.OK)
            return;

        var path = ofd.FileName;
        var len = new FileInfo(path).Length;
        if (len != CGearBackground.SIZE)
        {
            WinFormsUtil.Error($"Incorrect size, got {len} bytes, expected {CGearBackground.SIZE} bytes.");
            return;
        }

        byte[] data = File.ReadAllBytes(path);

        // Load the data and adjust it to the correct game format if not matching.
        CGearBackground temp = isBW ? new CGearBackgroundBW(data) : new CGearBackgroundB2W2(data);
        bool isPSK = PaletteTileSelection.IsPaletteShiftFormat(temp.Arrange);

        try
        {
            if (isBW && !isPSK)
                PaletteTileSelection.ConvertToShiftFormat<CGearBackgroundBW>(temp.Arrange);
            else if (!isBW && isPSK)
                PaletteTileSelection.ConvertFromShiftFormat(temp.Arrange);
        }
        catch (Exception ex)
        {
            WinFormsUtil.Error(ex.Message);
            return;
        }

        SAV.SetCGearSkin(temp.Data);
        PB_CGearBackground.Image = CGearImage.GetBitmap(bg);
        B_ExportCGB.Enabled = B_ExportPNG.Enabled = true;
    }

    private void B_ExportCGB_Click(object sender, EventArgs e)
    {
        using var sfd = new SaveFileDialog();
        sfd.Filter = SAV is SAV5BW ? CGearFilterBW : CGearFilterB2W2;

        if (sfd.ShowDialog() != DialogResult.OK)
            return;
        File.WriteAllBytes(sfd.FileName, bg.Data);
    }

    private void B_Save_Click(object sender, EventArgs e)
    {
        Origin.CopyChangesFrom(SAV);
        Close();
    }

    private void B_Cancel_Click(object sender, EventArgs e) => Close();

    private void B_PWTImport_Click(object sender, EventArgs e)
    {
        const int pporg = 0x1314;
        if (!ImportFile(WorldTournament5.Extension, PWTFileName, WorldTournament5.SIZE, out var data, otherSize: pporg))
            return;
        var index = LB_PWT.SelectedIndex;
        var b2w2 = (SAV5B2W2)SAV;
        b2w2.SetPWT(index, data);
        var pwt = new WorldTournament5(data);
        var name = pwt.Name;
        if (string.IsNullOrWhiteSpace(name))
            name = "Empty";
        LB_PWT.Items[index] = $"{index + 1:00} - {name}";
        LB_PWTIndex_SelectedIndexChanged(sender, e);
    }

    private void B_PWTExport_Click(object sender, EventArgs e)
    {
        var b2w2 = (SAV5B2W2)SAV;
        var data = b2w2.GetPWT(LB_PWT.SelectedIndex);
        var pwt = new WorldTournament5(data);
        var name = pwt.Name;
        if (string.IsNullOrWhiteSpace(name))
            name = "Empty";
        ExportFile(WorldTournament5.Extension, PWTFileName, data.Span, name);
    }

    private void B_MusicalImport_Click(object sender, EventArgs e)
    {
        var size = SAV is SAV5B2W2 ? MusicalShow5.SIZE_B2W2 : MusicalShow5.SIZE_BW;
        const int pporg = 0x17D78;
        if (!ImportFile(MusicalShow5.Extension, MusicalShowFileName, size, out var data, otherSize: pporg))
            return;

        var musical = new MusicalShow5(data);
        SAV.SetMusical(data);
        if (LastImportedFile is { } name)
            SAV.Musical.MusicalName = musical.IsUninitialized ? "" : Path.GetFileNameWithoutExtension(name).Trim();
    }

    private void B_MusicalExport_Click(object sender, EventArgs e)
    {
        var data = SAV.MusicalDownloadData;
        ExportFile(MusicalShow5.Extension, SAV.Musical.MusicalName, data.Span);
    }

    private void B_BattleVideoImport_Click(object sender, EventArgs e)
    {
        if (!ImportFile(BattleVideo5.Extension, BattleVideoFileName, BattleVideo5.SIZE, out var data))
            return;
        bool decrypted = BattleVideo5.GetIsDecrypted(data);
        var bvid = new BattleVideo5(data) { IsDecrypted = decrypted };
        bvid.Encrypt();
        if (!bvid.IsUninitialized)
            bvid.RefreshChecksums();

        var index = LB_BattleVideo.SelectedIndex;
        SAV.SetBattleVideo(index, data);
        var name = bvid.IsUninitialized ? "Empty" : bvid.GetTrainerNames();
        LB_BattleVideo.Items[index] = $"{index:00} - {name}";
        LB_BattleVideo_SelectedIndexChanged(sender, e);
    }

    private void B_BattleVideoExport_Click(object sender, EventArgs e)
    {
        var index = LB_BattleVideo.SelectedIndex;
        var data = SAV.GetBattleVideo(index);
        ExportFile(BattleVideo5.Extension, BattleVideoFileName, data.Span);
    }

    private void B_BattleVideoExportDecrypted_Click(object sender, EventArgs e)
    {
        var index = LB_BattleVideo.SelectedIndex;
        var data = SAV.GetBattleVideo(index);
        var bvid = new BattleVideo5(data);
        bool actual = !bvid.IsUninitialized;
        if (actual)
            bvid.Decrypt();
        ExportFile(BattleVideo5.Extension, BattleVideoFileName, data.Span);
        if (actual)
            bvid.Encrypt();
    }

    private void B_PokestarImport_Click(object sender, EventArgs e)
    {
        if (!ImportFile(PokestarMovie5.Extension, PokeStarMovieFileName, PokestarMovie5.SIZE, out var data))
            return;
        var index = LB_Pokestar.SelectedIndex;
        var b2w2 = (SAV5B2W2)SAV;
        b2w2.SetPokestarMovie(index, data);
        var movie = new PokestarMovie5(data);
        LB_Pokestar.Items[index] = $"{+1:00} - {movie.Name}";
        LB_Pokestar_SelectedIndexChanged(sender, e);
    }

    private void B_PokestarExport_Click(object sender, EventArgs e)
    {
        var b2w2 = (SAV5B2W2)SAV;
        var data = b2w2.GetPokestarMovie(LB_Pokestar.SelectedIndex);
        ExportFile(PokestarMovie5.Extension, PokeStarMovieFileName, data.Span);
    }

    private void LB_Pokestar_SelectedIndexChanged(object sender, EventArgs e) { }
    private void LB_PWTIndex_SelectedIndexChanged(object sender, EventArgs e) { }
    private void LB_BattleVideo_SelectedIndexChanged(object sender, EventArgs e) { }

    private void B_Memory1Import_Click(object sender, EventArgs e)
    {
        if (!ImportFile(MemoryLinkExtension, MemoryLinkFileName, SAV.Link1Data.Length, out var data))
            return;
        SAV.SetLink1Data(data);
    }

    private void B_Memory2Import_Click(object sender, EventArgs e)
    {
        if (!ImportFile(MemoryLinkExtension, MemoryLinkFileName, SAV.Link2Data.Length, out var data))
            return;
        SAV.SetLink2Data(data);
    }

    private void B_Memory1Export_Click(object sender, EventArgs e)
        => ExportFile(MemoryLinkExtension, MemoryLinkFileName, SAV.Link1Data.Span);
    private void B_Memory2Export_Click(object sender, EventArgs e)
        => ExportFile(MemoryLinkExtension, MemoryLinkFileName, SAV.Link2Data.Span);
    private void B_PokeDexSkinSave_Click(object sender, EventArgs e)
        => ExportFile(PokeDexSkin5.Extension, PokeDexFileName, SAV.PokedexSkinData.Span);
    private void B_BattleTestExport_Click(object sender, EventArgs e)
        => ExportFile(BattleTest5.Extension, BattleTestFileName, SAV.BattleTest.Span);

    private void B_PokeDexSkinLoad_Click(object sender, EventArgs e)
    {
        const int pporg = 0x6200; // missing 4 bytes, flag
        if (!ImportFile(PokeDexSkin5.Extension, PokeDexFileName, SAV.PokedexSkinData.Length, out var data, otherSize: pporg))
            return;
        SAV.SetPokeDexSkin(data);
        LoadPokedexSkin(data);
    }

    private void B_BattleTestImport_Click(object sender, EventArgs e)
    {
        if (!ImportFile(BattleTest5.Extension, BattleTestFileName, BattleTest5.SIZE, out var data))
            return;

        // Ensure checksums are valid for user-fuzzed data.
        var test = new BattleTest5(data);
        if (!test.IsUninitialized)
        {
            test.Magic = BattleTest5.Sentinel;
            test.RefreshChecksums();
        }
        SAV.SetBattleTest(data);
    }
}

using System;
using System.Diagnostics;
using System.IO;
using System.Media;
using System.Windows.Forms;
using PKHeX.Core;

namespace PKHeX.WinForms;
public partial class SAV_Chatter : Form
{
    private readonly SaveFile Origin;
    private readonly SaveFile SAV;
    private readonly IChatter Chatter;

    private readonly SoundPlayer Sounds = new();

    public SAV_Chatter(SaveFile sav)
    {
        InitializeComponent();
        WinFormsUtil.TranslateInterface(this, Main.CurrentLanguage);
        SAV = (Origin = sav).Clone();
        Chatter = SAV is SAV5 s5 ? s5.Chatter : ((SAV4)SAV).Chatter;

        CHK_Initialized.Checked = Chatter.Initialized;
        MT_Confusion.Text = Chatter.ConfusionChance.ToString();
    }

    private void CHK_Initialized_CheckedChanged(object sender, EventArgs e)
    {
        Chatter.Initialized = CHK_Initialized.Checked;
        MT_Confusion.Text = Chatter.ConfusionChance.ToString();
    }

    private void B_ImportPCM_Click(object sender, EventArgs e)
    {
        using var ofd = new OpenFileDialog();
        ofd.Filter = "PCM File|*.pcm";

        if (ofd.ShowDialog() != DialogResult.OK)
            return;

        var path = ofd.FileName;
        var len = new FileInfo(path).Length;
        if (len != IChatter.SIZE_PCM)
        {
            WinFormsUtil.Error($"Incorrect size, got {len} bytes, expected {IChatter.SIZE_PCM} bytes.");
            return;
        }

        byte[] data = File.ReadAllBytes(path);
        data.CopyTo(Chatter.Recording);
        CHK_Initialized.Checked = Chatter.Initialized = true;
        MT_Confusion.Text = Chatter.ConfusionChance.ToString();
    }

    private void B_ExportPCM_Click(object sender, EventArgs e)
    {
        using var sfd = new SaveFileDialog();
        sfd.Filter = "PCM File|*.pcm";
        sfd.FileName = "Recording.pcm";
        if (sfd.ShowDialog() != DialogResult.OK)
            return;

        File.WriteAllBytes(sfd.FileName, Chatter.Recording.ToArray());
    }

    private void B_ExportWAV_Click(object sender, EventArgs e)
    {
        using var sfd = new SaveFileDialog();
        sfd.Filter = "WAV File|*.wav";
        sfd.FileName = "Recording.wav";
        if (sfd.ShowDialog() != DialogResult.OK)
            return;

        File.WriteAllBytes(sfd.FileName, ConvertPCMToWAV(Chatter.Recording));
    }

    private void B_PlayRecording_Click(object sender, EventArgs e)
    {
        if (!Chatter.Initialized && !Chatter.Recording.ContainsAnyExcept<byte>(0x00))
            return;

        var data = ConvertPCMToWAV(Chatter.Recording);
        Sounds.Stream = new MemoryStream(data);
        try
        {
            Sounds.Play();
        }
        catch { Debug.WriteLine("Failed to play sound."); }
    }

    private void B_Save_Click(object sender, EventArgs e)
    {
        Origin.CopyChangesFrom(SAV);
        Sounds.Stop();
        Close();
    }

    private void B_Cancel_Click(object sender, EventArgs e)
    {
        Sounds.Stop();
        Close();
    }

    private static int GetWAVExpectedLength() => WAVHeader.Length + (IChatter.SIZE_PCM * 2);

    /// <summary>
    /// Size: 2x <see cref="IChatter.SIZE_PCM"/>"/>
    /// </summary>
    private static ReadOnlySpan<byte> WAVHeader =>
    [
        // RIFF chunk
        0x52, 0x49, 0x46, 0x46, // chunk name: "RIFF"
        0xF4, 0x07, 0x00, 0x00, // chunk size: 2036
        0x57, 0x41, 0x56, 0x45, // format: "WAVE"

        // fmt subchunk
        0x66, 0x6D, 0x74, 0x20, // subchunk name: "fmt "
        0x10, 0x00, 0x00, 0x00, // subchunk size: 16
        0x01, 0x00,             // wFormatTag: WAVE_FORMAT_PCM (1)
        0x01, 0x00,             // nChannels: mono (1)
        0xD0, 0x07, 0x00, 0x00, // nSamplesPerSec: 2000
        0xD0, 0x07, 0x00, 0x00, // nAvgBytesPerSec: 2000
        0x01, 0x00,             // nBlockAlign: 1
        0x08, 0x00,             // wBitsPerSample: 8

        // data subchunk
        0x64, 0x61, 0x74, 0x61, // subchunk name: "data"
        0xD0, 0x07, 0x00, 0x00, // subchunk size: 2000
    ];

    /// <summary>
    /// Convert 4-bit PCM to 8-bit PCM and adds the WAV file header.
    /// </summary>
    /// <param name="pcm">Unsigned 4-bit PCM data</param>
    /// <returns>WAV file with unsigned 8-bit PCM data</returns>
    private static byte[] ConvertPCMToWAV(ReadOnlySpan<byte> pcm)
    {
        byte[] data = new byte[GetWAVExpectedLength()];
        ConvertPCMToWAV(pcm, data);
        return data;
    }

    private static void ConvertPCMToWAV(ReadOnlySpan<byte> pcm, Span<byte> result)
    {
        WAVHeader.CopyTo(result);
        var i = WAVHeader.Length;
        foreach (byte b in pcm)
        {
            result[i++] = (byte)((b & 0x0F) << 4);
            result[i++] = (byte)(b & 0xF0);
        }
    }
}

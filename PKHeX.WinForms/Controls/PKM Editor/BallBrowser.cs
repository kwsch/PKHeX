using System;
using System.Collections.Generic;
using System.Windows.Forms;

using PKHeX.Core;
using PKHeX.Drawing.PokeSprite;
using PKHeX.WinForms.Controls;

namespace PKHeX.WinForms;

public partial class BallBrowser : Form
{
    public BallBrowser() => InitializeComponent();

    public int BallChoice { get; private set; } = -1;

    public void LoadBalls(PKM pk)
    {
        var legal = BallApplicator.GetLegalBalls(pk);
        LoadBalls(legal, pk.MaxBallID + 1);
    }

    private void LoadBalls(IEnumerable<Ball> legal, int max)
    {
        Span<bool> flags = stackalloc bool[max];
        foreach (var ball in legal)
            flags[(int)ball] = true;

        int countLegal = 0;
        List<PictureBox> controls = [];
        var names = GameInfo.BallDataSource;
        for (int ballID = 1; ballID < flags.Length; ballID++)
        {
            var name = GetBallName(ballID, names);
            var pb = GetBallView(ballID, name, flags[ballID]);
            if (Main.Settings.EntityEditor.ShowLegalBallsFirst && flags[ballID])
                controls.Insert(countLegal++, pb);
            else
                controls.Add(pb);
        }

        int countInRow = 0;
        var container = flp.Controls;
        foreach (var pb in controls)
        {
            container.Add(pb);
            const int width = 5; // balls wide
            if (++countInRow != width)
                continue;
            flp.SetFlowBreak(pb, true);
            countInRow = 0;
        }
    }

    private static string GetBallName(int ballID, IEnumerable<ComboItem> names)
    {
        foreach (var x in names)
        {
            if (x.Value == ballID)
                return x.Text;
        }
        throw new ArgumentOutOfRangeException(nameof(ballID));
    }

    private SelectablePictureBox GetBallView(int ballID, string name, bool valid)
    {
        var img = SpriteUtil.GetBallSprite(ballID);
        var pb = new SelectablePictureBox
        {
            Size = img.Size,
            Image = img,
            BackgroundImage = valid ? SpriteUtil.Spriter.Set : SpriteUtil.Spriter.Delete,
            BackgroundImageLayout = ImageLayout.Tile,
            Name = name,
            AccessibleDescription = name,
            AccessibleName = name,
            AccessibleRole = AccessibleRole.Graphic,
        };

        pb.MouseEnter += (_, _) => Text = name;
        pb.Click += (_, _) => SelectBall(ballID);
        pb.KeyDown += (_, e) =>
        {
            if (e.KeyCode == Keys.Enter)
                SelectBall(ballID);
        };
        return pb;
    }

    private void SelectBall(int b)
    {
        BallChoice = b;
        Close();
    }
}

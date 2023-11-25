namespace WolfpackSimulation;

public partial class AreaTile : UserControl
{
    public AreaTile(PictureBox pictureBox)
    {
        InitializeComponent();
        targetPictureBox = pictureBox;
    }

    private readonly PictureBox targetPictureBox;

    public void Draw(TileContent tileContent = TileContent.Nothing)
    {
        var targetColor = tileContent switch
        {
            TileContent.Nothing => Color.White,
            TileContent.WolfPack => Color.Blue,
            TileContent.Prey => Color.Green,
            TileContent.Scent => Color.Red,
            _ => Color.White
        };

        indicator.Size = Size;
        indicator.Location = Location;
        var g = targetPictureBox.CreateGraphics();
        var b = new SolidBrush(targetColor);
        g.FillRectangle(b, indicator);
        ControlPaint.DrawBorder(g, indicator, Color.Black, ButtonBorderStyle.Solid);
    }
}

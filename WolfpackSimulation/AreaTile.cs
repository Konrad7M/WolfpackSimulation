namespace WolfpackSimulation;

public partial class AreaTile : UserControl
{
    public AreaTile()
    {
        InitializeComponent();
    }

    public void Draw(PictureBox pictureBox)
    {
        indicator.Size = Size;
        indicator.Location = Location;
        var g = pictureBox.CreateGraphics();
        Pen p = new Pen(Color.White);
        Brush b = new SolidBrush(Color.Black);
        g.DrawRectangle(p, indicator);
        g.FillRectangle(b, indicator);
        ControlPaint.DrawBorder(g, indicator, Color.DarkGray, ButtonBorderStyle.Solid);
        ControlPaint.DrawBorder(g, indicator, Color.DarkGray, ButtonBorderStyle.Solid);
    }
}

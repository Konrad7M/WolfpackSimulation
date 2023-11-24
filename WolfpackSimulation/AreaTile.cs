namespace WolfpackSimulation;

public partial class AreaTile : UserControl
{
    public AreaTile()
    {
        InitializeComponent();
    }

    bool preyPresent;
    public TerritoryData TerritoryData;

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
    public void OnUpdate()
    {
        if (TerritoryData.scentCoeficient > 0)
        {
            TerritoryData.scentCoeficient -= 0.125f;
            if(TerritoryData.scentCoeficient <= 0)
            {
                TerritoryData.scentCoeficient = 0;
                TerritoryData.packId = -1;
            }
        }
    }
    public TerritoryData GetTerritoryData()
    {
        return TerritoryData;
    }
}

public struct TerritoryData
{
    public float scentCoeficient;
    public int packId;
}

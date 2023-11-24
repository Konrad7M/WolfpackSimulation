using System.Drawing.Drawing2D;

namespace WolfpackSimulation;

public partial class Form1 : Form
{
    public List<AreaTile> listRec = new List<AreaTile>();
    Graphics g;
    public Form1()
    {
        InitializeComponent();
    }
    private void button1_Click_1(object sender, EventArgs e)
    {
        AreaTile rect = new AreaTile();
        rect.Size = new Size(9, 9);
        for (int x = 0; x < 100; x++)
        {
            //rect.X = x * rect.Width;
            for (int y = 0; y < 100; y++)
            {
                rect.Location = new Point(x * rect.Width, y * rect.Height);
                //rect.Y = y * rect.Height;
                listRec.Add(rect);
                rect.Draw(pictureBox1);
            }
        }
    }

    public void ChangeColor(Rectangle target, Color targetColor)
    {
        Pen p = new Pen(targetColor);
        g.DrawRectangle(p, target.X, target.Y, target.Width, target.Height);
    }

    private void Form1_KeyDown(object sender, KeyEventArgs e)
    {
        switch (e.KeyCode)
        {
            //case Keys.D0:
            //    ChangeColor(listRec[0], Color.Red);
            //    break;
            //case Keys.D1:
            //    ChangeColor(listRec[1], Color.Red);
            //    break;
            //    //..more code to handle all keys..
        }
    }

    private void button4_Click(object sender, EventArgs e)
    {

    }
}

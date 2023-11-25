namespace WolfpackSimulation;

public partial class SimulationView : Form
{
    public List<AreaTile> listRec = new();

    private readonly Simulation simulation;

    private const int SimulationSize = 100;

    public SimulationView()
    {
        InitializeComponent();
        simulation = new Simulation(SimulationSize);
    }

    private async void StartButton_Click(object sender, EventArgs e)
    {
        if (simulation.isRunning) return;

        if (listRec.Count == 0)
        {
            for (int x = 0; x < SimulationSize; x++)
            {
                for (int y = 0; y < SimulationSize; y++)
                {
                    var rect = new AreaTile(pictureBox1)
                    {
                        Size = new Size(9, 9)
                    };
                    rect.Location = new Point(x * rect.Width, y * rect.Height);
                    listRec.Add(rect);
                    rect.Draw();
                }
            }
        }
        else 
        { 
            simulation.Reset();
            listRec.ForEach(tile => tile.Draw());
        }

        simulation.isRunning = true;
        simulation.Start();
        await RunSimulation();
    }

    private async Task RunSimulation()
    {
        while (simulation.isRunning)
        {
            var state = simulation.GetSimulationState();
            for (var i = 0; i < state.Count; i++) listRec[i].Draw(state[i]);
            await Task.Delay(500);
        }
    }

    private void StopButton_Click(object sender, EventArgs e)
    {
        simulation.isRunning = false;
    }

    private void MultiButton_Click(object sender, EventArgs e)
    {

    }

    private void SingleButton_Click(object sender, EventArgs e)
    {

    }
}

using System.Numerics;

namespace WolfpackSimulation;

public enum TileContent
{
    Nothing,
    WolfPack,
    Scent,
    Prey
}

internal class Simulation
{
    private class SimulationTile
    {
        public int x,
            y;

        public TileContent tileContent;
    }

    private class Scent
    {
        public int x,
            y;

        public int packId = -1;
        public float value;
    }

    private class WolfPack
    {
        public int x,
            y;

        public int packId;
    }

    private class Prey
    {
        public int x,
            y;
    }

    public bool isRunning;
    public bool useThreads;

    private readonly List<SimulationTile> tiles = new();
    private readonly List<WolfPack> packs = new();
    private readonly List<Scent> scents = new();
    private readonly List<Prey> preys = new();
    private const int MaxPrey = 10;
    private const int MaxPacks = 3;
    private readonly Random random = new();
    private Thread PreyThread;
    private Thread ScentThread;
    private Thread PackThread;
    int tick = 50;


    public Simulation(int size)
    {
        for (var i = 0; i < size; i++)
            for (var j = 0; j < size; j++)
            {
                tiles.Add(
                    new SimulationTile
                    {
                        x = i,
                        y = j,
                        tileContent = TileContent.Nothing
                    }
                );
            }
    }

    private void GeneratePrey()
    {
        for (
            var i = 0;
            i
                < Math.Min(
                    MaxPrey - tiles.Count(tile => tile.tileContent == TileContent.Prey),
                    MaxPrey
                );
            i++
        )
        {
            var freeTiles = tiles.FindAll(tile => tile.tileContent == TileContent.Nothing);
            var randomTile = freeTiles[random.Next(0, freeTiles.Count)];
            randomTile.tileContent = TileContent.Prey;
            preys.Add(new Prey { x = randomTile.x, y = randomTile.y });
        }
    }

    private void GeneratePacks()
    {
        for (var i = 0; i < MaxPacks; i++)
        {
            var freeTiles = tiles.FindAll(tile => tile.tileContent is TileContent.Nothing);
            var randomTile = freeTiles[random.Next(0, freeTiles.Count)];
            randomTile.tileContent = TileContent.WolfPack;
            packs.Add(
                new WolfPack
                {
                    packId = i,
                    x = randomTile.x,
                    y = randomTile.y
                }
            );
        }
    }

    private Prey? FindClosestAvailablePrey(WolfPack pack) =>
        preys
            .OrderBy(
                prey => Vector2.Distance(new Vector2(pack.x, pack.y), new Vector2(prey.x, prey.y))
            )
            .FirstOrDefault(prey =>
            {
                var scentPackId = scents.Find(s => s?.x == prey.x && s?.y == prey.y)?.packId;
                return scentPackId == null || scentPackId == -1 || scentPackId == pack.packId;
            });

    private void MovePacksTowardsClosestPrey()
    {
        packs.ForEach(pack =>
        {
            var targetPrey = FindClosestAvailablePrey(pack);
            if (targetPrey != null)
            {
                var sourceTile = tiles.Find(tile => tile.x == pack.x && tile.y == pack.y);
                if (sourceTile != null)
                    sourceTile.tileContent = TileContent.Scent;

                // TODO: Przed ruszeniem watahy sprawdzić, czy na docelowym klocku nie znajduje się obcy zapach (dopisac warunek do ifa)
                if (targetPrey.x != pack.x)
                    pack.x += targetPrey.x - pack.x < 0 ? -1 : 1;
                else if (targetPrey.y != pack.y)
                    pack.y += targetPrey.y - pack.y < 0 ? -1 : 1;

                var targetTile = tiles.Find(tile => tile.x == pack.x && tile.y == pack.y);
                if (targetTile != null)
                {
                    if (targetTile.tileContent == TileContent.Prey)
                        preys.RemoveAll(prey => prey.x == targetTile.x && prey.y == targetTile.y);
                    targetTile.tileContent = TileContent.WolfPack;
                }
                var targetScent = scents.Find(scent => scent.x == pack.x && scent.y == pack.y);
                if (targetScent != null)
                    targetScent.value = 1;
                else
                    scents.Add(
                        new Scent()
                        {
                            x = pack.x,
                            y = pack.y,
                            packId = pack.packId,
                            value = 1
                        }
                    );
            }
        });
    }

    private void DecreaseScentIntensity()
    {
        scents.ForEach(scent => scent.value -= 0.125f);
        scents.RemoveAll(scent => scent.value <= 0);
        tiles
            .FindAll(
                tile =>
                    tile.tileContent == TileContent.Scent
                    && scents.Find(scent => scent?.x == tile.x && scent?.y == tile.y) == null
            )
            .ForEach(tile => tile.tileContent = TileContent.Nothing);
    }

    public async void Start()
    {
        GeneratePacks();
        GeneratePrey();
        await Simulate();
    }

    private async Task Simulate()
    {
        if (useThreads)
        {
            PreyThread = new Thread(async () =>
            {
                while (isRunning)
                {
                    try {
                        GeneratePrey();
                        await Task.Delay(tick);
                    }
                    catch (Exception e)
                    {

                    }
                    
                }
            });
            ScentThread = new Thread(async () =>
            {
                while (isRunning)
                {
                    try
                    {
                        DecreaseScentIntensity();
                        await Task.Delay(tick);
                    }
                    catch (Exception e)
                    {

                    }
  
                }
            });
            PackThread = new Thread(async () =>
            {
                while (isRunning)
                {
                    try
                    {
                        MovePacksTowardsClosestPrey();
                        await Task.Delay(tick);
                    }
                    catch (Exception e)
                    {

                    }
                }
            });
            PreyThread.Start();
            ScentThread.Start();
            PackThread.Start();
        }
        else
        {
            while (isRunning)
            {
                Update();
                await Task.Delay(tick);
            }
        }
    }

    public List<TileContent> GetSimulationState()
    {
        var result = new List<TileContent>();
        // TODO: Możliwe że będziesz musiał zrobić lock na tiles tutaj (do sprawdzenia)
        tiles.ForEach(tile => result.Add(tile.tileContent));
        return result;
    }

    public void Update()
    {
        // TODO: Każda z tych metod może chodzić na oddzielnym wątku. Możesz zostawić to tutaj jako przykład single thread.
        DecreaseScentIntensity();
        MovePacksTowardsClosestPrey();
        GeneratePrey();
    }

    public void Reset()
    {
        packs.Clear();
        preys.Clear();
        scents.Clear();
        tiles.ForEach(tile => tile.tileContent = TileContent.Nothing);
    }
}
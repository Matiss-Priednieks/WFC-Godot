using Godot;
using System.Collections.Generic;

public class WFC : TileMap
{
    // Vector2[] Directions = new Vector2[] { Vector2.Up, Vector2.Down, Vector2.Left, Vector2.Right };
    int Dim = 16;
    Cell[] Grid;
    bool Initialised = false;
    RandomNumberGenerator rng = new RandomNumberGenerator();

    Vector2[] Directions = { Vector2.Up, Vector2.Down, Vector2.Left, Vector2.Right };

    List<int> DefaultList = new List<int>() { 0, 1, 2 };
    int MaxEntropy;
    [Export] Vector2 StartPos = Vector2.Zero;
    int CurrentTile = 0;
    bool Started = false;

    public override void _Ready()
    {
        MaxEntropy = DefaultList.Count;
        rng.Randomize();
        Grid = new Cell[Dim * Dim];


        for (int i = 0; i < Dim * Dim; i++)
        {
            for (int j = 0; j < Dim * Dim; j++)
            {
                Grid[i] = new Cell(false, DefaultList);
                // SetCell(i, j, Grid[i].Options.Count - 1);
            }
        }

    }
    public override void _Process(float delta)
    {
        if (!Initialised)
        {
            Generate();
        }
    }
    public async void Generate()
    {
        for (int i = 0; i < Dim; i++)
        {
            if (Grid[i].Options.Count != 0 || !Grid[i].Collapsed)
            {
                rng.Randomize();
                var randomCellOption = rng.RandiRange(0, Grid[i].Options.Count);
                int opposite = 0;
                for (int j = 0; j < Dim; j++)
                {
                    if (Grid[i].Options.Count != 0 && !Grid[i].Collapsed)
                    {
                        rng.Randomize();
                        randomCellOption = rng.RandiRange(0, Grid[i].Options.Count);
                        switch (randomCellOption)
                        {
                            case 0: opposite = 2; break;
                            case 1: opposite = 0; break;
                            case 2: opposite = 1; break;
                            default: break;
                        }

                        Vector2 currPos = new Vector2(i, j);
                        Vector2 randDirection = Directions[rng.RandiRange(0, Directions.Length - 1)];
                        Vector2 nextCell = currPos + randDirection;
                        if (Grid[i].Options.Count != MaxEntropy)
                        {
                            int smallest = MaxEntropy;
                            //grabs list of entropy values relative to current cell.
                            Vector2 whereNext = CheckEntropy(currPos);

                            await ToSignal(GetTree().CreateTimer(0.01f), "timeout");
                            SetCell(Mathf.Abs((int)(currPos.x + whereNext.x)), Mathf.Abs((int)(currPos.y + whereNext.y)), randomCellOption);
                            Grid[i + 1].Options.Remove(opposite);
                            GD.Print(Grid[i + 1].Options.Count);
                        }
                        else if (Grid[i].Options.Count == MaxEntropy && !Started)
                        {
                            await ToSignal(GetTree().CreateTimer(0.01f), "timeout");
                            SetCell(Mathf.Abs((int)(currPos.x)), Mathf.Abs((int)(currPos.y)), randomCellOption);
                            Grid[i + 1].Options.Remove(opposite);
                            Started = true;
                        }
                        else if (Grid[i].Options.Count <= 1)
                        {
                            Grid[i].Collapsed = true;
                        }
                        else
                        {
                            // GD.Print("Max Entropy");
                        }
                    }
                    else if (Grid[i].Options.Count == 0 && !Grid[i].Collapsed)
                    {
                        // GD.Print(Grid[i].Options.Count);
                    }
                    else
                    {
                        // GD.Print(randomCellOption);
                        Grid[i].Collapsed = true;
                    }

                }
            }
            Initialised = true;
        }
    }

    //Issue with checking for entropy: Cell is only able to find out about behind itself and in front of it, because it's using a 1D array. It doesn't see side to side. Need to refactor Cell gen code up in _Ready to try work around this.
    public Vector2 CheckEntropy(Vector2 position)
    {
        //grabs relative position of each cell
        Vector2 upEntropy = position + new Vector2(0, -1);
        Vector2 downEntropy = position + new Vector2(0, 1);
        Vector2 leftEntropy = position + new Vector2(-1, 0);
        Vector2 rightEntropy = position + new Vector2(1, 0);


        //empty int array
        int[] cellPositions = { 0, 0, 0, 0 };

        //adds entropy value (index in this case) to the int array and returns it
        cellPositions[0] = GetCell((int)upEntropy.x, (int)upEntropy.y);
        cellPositions[1] = GetCell((int)downEntropy.x, (int)downEntropy.y);
        cellPositions[2] = GetCell((int)leftEntropy.x, (int)leftEntropy.y);
        cellPositions[3] = GetCell((int)rightEntropy.x, (int)rightEntropy.y);

        int smallest = 0;
        for (int x = 0; x < cellPositions.Length; x++)
        {
            if (cellPositions[x] < smallest) smallest = cellPositions[x];
        }

        switch (smallest)
        {
            case 0: return upEntropy;
            case 1: return downEntropy;
            case 2: return leftEntropy;
            case 3: return rightEntropy;
            default: return Vector2.Zero;
        }

    }
}


public class Cell : Godot.Object
{
    public bool Collapsed;
    public List<int> Options;

    public Cell(bool collapsed, List<int> options)
    {
        Collapsed = collapsed;
        Options = options;
    }
}

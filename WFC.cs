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

    public override void _Ready()
    {
        MaxEntropy = DefaultList.Count;
        rng.Randomize();
        Grid = new Cell[Dim * Dim];


        for (int i = 0; i < Dim * Dim; i++)
        {
            Grid[i] = new Cell(false, DefaultList);
            for (int j = 0; j < Dim * Dim; j++)
            {
                SetCell(i, j, Grid[i].Options.Count);
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
                for (int j = 0; j < Dim; j++)
                {
                    if (Grid[i].Options.Count != 0 || !Grid[i].Collapsed)
                    {
                        randomCellOption = rng.RandiRange(0, Grid[i].Options.Count);
                        Vector2 currPos = new Vector2(i, j);
                        Vector2 randDirection = Directions[rng.RandiRange(0, Directions.Length - 1)];
                        Vector2 nextCell = currPos + randDirection;
                        if (GetCell((int)nextCell.x, (int)nextCell.y) != MaxEntropy) //issue is here. I'm grabbing the cell index, which obviously changes based off of what image cell is being used. This needs to change to be relatvie to the cell array rather than the tileset.
                        {
                            // int smallest = MaxEntropy;
                            // //grabs list of entropy values relative to current cell.
                            // int[] entropy = CheckEntropy(currPos);
                            // for (int x = 0; x < entropy.Length; x++)
                            // {
                            //     if (entropy[x] < smallest) smallest = entropy[x];
                            // }
                            // GD.Print("X Position: " + nextCell.x + "\n " + "Y Position: " + nextCell.y + "\n" + "Cell Entropy: " + GetCell(Mathf.Abs((int)nextCell.x), Mathf.Abs((int)nextCell.y)) + "\n");
                            await ToSignal(GetTree().CreateTimer(0.1f), "timeout");
                            SetCell(Mathf.Abs((int)currPos.x), Mathf.Abs((int)currPos.y), randomCellOption);
                        }
                        else if (Grid[i].Options.Count <= 1)
                        {
                            // Grid[i].Collapsed = true;
                        }
                    }
                }
                Grid[i].Options.Remove(randomCellOption);
            }
            else if (Grid[i].Options.Count == 0)
            {
                Grid[i].Collapsed = true;
            }
            Initialised = true;
        }
    }

    //Issue with checking for entropy: Cell is only able to find out about behind itself and in front of it, because it's using a 1D array. It doesn't see side to side. Need to refactor Cell gen code up in _Ready to try work around this.
    public int[] CheckEntropy(Vector2 position)
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

        // var nextCellEntropy = Grid[(int)downEntropy.y].Options.Count; //Idea for later
        return cellPositions;

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

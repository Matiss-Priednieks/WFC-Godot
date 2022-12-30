using Godot;
using System.Collections.Generic;

public class WFCEntropyOnly : TileMap
{
    // Vector2[] Directions = new Vector2[] { Vector2.Up, Vector2.Down, Vector2.Left, Vector2.Right };
    int Dim = 16;
    Cell[] Grid;
    bool Initialised = false;
    RandomNumberGenerator rng = new RandomNumberGenerator();

    Vector2[] Directions = { Vector2.Up, Vector2.Down, Vector2.Left, Vector2.Right };

    List<int> DefaultList = new List<int>() { 0, 1, 2 };
    [Export] Vector2 StartPos = Vector2.Zero;
    int CurrentTile = 0;

    public override void _Ready()
    {
        rng.Randomize();
        Grid = new Cell[Dim * Dim];


        for (int i = 0; i < Dim * Dim; i++)
        {
            Grid[i] = new Cell(false, DefaultList);
            for (int j = 0; j < Dim * Dim; j++)
            {
                SetCell(i, j, 7);
            }

            // GD.Print(Grid[i].Collapsed);
        }

    }
    public override void _Process(float delta)
    {
        if (!Initialised)
        {
            Initalise();
        }
    }
    public void Initalise()
    {
        for (int i = 0; i < Dim; i++)
        {
            if (Grid[i].Options.Count != 0 || !Grid[i].Collapsed)
            {
                rng.Randomize();
                var randomCellOption = rng.RandiRange(0, Grid[i].Options.Count);
                for (int j = 0; j < Dim; j++)
                {
                    if (Grid[j].Options.Count != 0 || !Grid[j].Collapsed)
                    {
                        randomCellOption = rng.RandiRange(0, Grid[j].Options.Count);
                        Vector2 currPos = new Vector2(i, j);
                        Vector2 randDirection = Directions[rng.RandiRange(0, Directions.Length - 1)];
                        Vector2 nextCell = currPos + randDirection;
                        if (GetCell((int)nextCell.x, (int)nextCell.y) != 3)
                        {
                            int smallest = 7;
                            //grabs list of entropy values relative to current cell.
                            int[] entropy = CheckEntropy(currPos);
                            for (int x = 0; x < entropy.Length; x++)
                            {

                                if (entropy[x] < smallest)
                                {
                                    smallest = entropy[x];
                                }
                            }
                            GD.Print("X Position: " + nextCell.x + "\n " + "Y Position: " + nextCell.y + "\n" + "Cell Entropy: " + GetCell(Mathf.Abs((int)nextCell.x), Mathf.Abs((int)nextCell.y)) + "\n");
                            SetCell(Mathf.Abs((int)currPos.x), Mathf.Abs((int)currPos.y), randomCellOption);
                            GD.Print(rng.RandiRange(0, Grid[i].Options.Count));
                        }

                    }
                    Grid[j].Collapsed = true;
                    Grid[j].Options.Remove(randomCellOption);
                }
                Grid[i].Collapsed = true;
                Grid[i].Options.Remove(randomCellOption);
            }
            Initialised = true;
        }
    }
    public int[] CheckEntropy(Vector2 position)
    {
        //grabs relative position of each cell
        Vector2 upEntropy = position - new Vector2(0, -1);
        Vector2 downEntropy = position - new Vector2(0, 1);
        Vector2 leftEntropy = position - new Vector2(-1, 0);
        Vector2 rightEntropy = position - new Vector2(1, 0);

        //empty int array
        int[] cellEntropy = { 0, 0, 0, 0 };

        //adds entropy value (index in this case) to the int array and returns it
        cellEntropy[0] = GetCell((int)upEntropy.x, (int)upEntropy.y);
        cellEntropy[1] = GetCell((int)downEntropy.x, (int)downEntropy.y);
        cellEntropy[2] = GetCell((int)leftEntropy.x, (int)leftEntropy.y);
        cellEntropy[3] = GetCell((int)rightEntropy.x, (int)rightEntropy.y);
        return cellEntropy;

    }
}


// public class Cell : Godot.Object
// {
//     public bool Collapsed;
//     public List<int> Options;

//     public Cell(bool collapsed, List<int> options)
//     {
//         Collapsed = collapsed;
//         Options = options;
//     }
// }

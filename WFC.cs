using Godot;
using System.Collections.Generic;

public class WFC : TileMap
{
    // Vector2[] Directions = new Vector2[] { Vector2.Up, Vector2.Down, Vector2.Left, Vector2.Right };
    int Dim = 16;
    Cell[] Grid;
    bool Initialised = false;
    RandomNumberGenerator rng = new RandomNumberGenerator();

    List<int> DefaultList = new List<int>() { 0, 1, 2 };
    [Export] Vector2 StartPos = Vector2.Zero;
    int CurrentTile = 0;

    public override void _Ready()
    {
        Grid = new Cell[Dim * Dim];


        for (int i = 0; i < Dim * Dim; i++)
        {
            Grid[i] = new Cell(false, DefaultList);

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
            var randomTile = rng.RandiRange(0, Grid[i].Options.Count);
            if (Grid[i].Options.Count != 0)
            {
                Grid[i].Collapsed = true;
                CurrentTile = randomTile;
                for (int j = 0; j < Dim; j++)
                {
                    randomTile = rng.RandiRange(0, Grid[j].Options.Count);
                    if (Grid[j].Options.Count != 0)
                    {
                        Grid[j].Collapsed = true;
                        SetCell(i, j, CurrentTile);
                    }
                    GD.Print("Before remove: " + Grid[j].Options.Count + "\n");
                    Grid[j].Options.Remove((CurrentTile + 1) % 2);
                    GD.Print("After remove: " + Grid[j].Options.Count + "\n");
                }
            }
            Grid[i].Options.Remove((CurrentTile + 1) % 2);
            Initialised = true;
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

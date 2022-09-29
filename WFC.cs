using Godot;

public class WFC : Node2D
{
    // Vector2[] Directions = new Vector2[] { Vector2.Up, Vector2.Down, Vector2.Left, Vector2.Right };
    // NodePath[] CellPath;
    int Dim = 2;
    Cell[] Grid;

    int[] DefaultList = { 0, 1, 2 };

    public override void _Ready()
    {
        Grid = new Cell[Dim * Dim];


        for (int i = 0; i < Dim * Dim; i++)
        {
            Grid[i] = new Cell(false, DefaultList);

            GD.Print(Grid[i].Collapsed);
        }
    }
}


public class Cell : Object
{
    public bool Collapsed;
    public int[] Options;

    public Cell(bool collapsed, int[] options)
    {
        Collapsed = collapsed;
        Options = options;
    }
}

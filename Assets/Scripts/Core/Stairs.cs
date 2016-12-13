using RogueSharp;


public class Stairs
{
    public int X { get; set; }
    public int Y { get; set; }
    public bool IsUp { get; set; }

    public void Draw( IMap map )
    {
        if ( !map.GetCell( X, Y ).IsExplored )
        {
        return;
        }

        if ( map.IsInFov( X, Y ) )
        {
        if ( IsUp )
        {
            Display.CellAt(0, X, Y).SetContent("<", Colors.FloorBackground, Colors.Player);
        }
        else
        {
            Display.CellAt(0, X, Y).SetContent(">", Colors.FloorBackground, Colors.Player);
        }
        }
        else
        {
        if ( IsUp )
        {
            Display.CellAt(0, X, Y).SetContent("<", Colors.FloorBackground, Colors.Floor);
        }
        else
        {
            Display.CellAt(0, X, Y).SetContent(">", Colors.FloorBackground, Colors.Floor);
        }
        }
    }
}

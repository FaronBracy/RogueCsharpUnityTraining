using RogueSharp;
using UnityEngine;

public class Door : IDrawable
{
    public Door()
    {
        Symbol = '+';
        Color = Colors.Door;
        BackgroundColor = Colors.DoorBackground;
    }
    public bool IsOpen { get; set; }

    public Color Color { get; set; }
    public Color BackgroundColor { get; set; }
    public char Symbol { get; set; }
    public int X { get; set; }
    public int Y { get; set; }

    public void Draw(IMap map)
    {
        if (!map.GetCell(X, Y).IsExplored)
        {
            return;
        }

        Symbol = IsOpen ? '-' : '+';
        if (map.IsInFov(X, Y))
        {
            Color = Colors.DoorFov;
            BackgroundColor = Colors.DoorBackgroundFov;
        }
        else
        {
            Color = Colors.Door;
            BackgroundColor = Colors.DoorBackground;
        }

        Display.CellAt(0, X, Y).SetContent(Symbol.ToString(), BackgroundColor, Color);
    }
}

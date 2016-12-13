
using RogueSharp;
using UnityEngine;

public interface IDrawable
{
    Color Color { get; set; }
    char Symbol { get; set; }
    int X { get; set; }
    int Y { get; set; }

    void Draw( IMap map );
}

using RogueSharp;
using UnityEngine;


public class Item : IItem, ITreasure, IDrawable
{
    public Item()
    {
        Symbol = '!';
        Color = Color.yellow;
    }

    public string Name { get; protected set; }
    public int RemainingUses { get; protected set; }

    public bool Use()
    {
        return UseItem();
    }

    protected virtual bool UseItem()
    {
        return false;
    }

    public bool PickUp(IActor actor)
    {
        Player player = actor as Player;

        if (player != null)
        {
            if (player.AddItem(this))
            {
                Game.MessageLog.Add(string.Format("{0} picked up {1}", actor.Name,Name));
                return true;
            }
        }

        return false;
    }

    public Color Color { get; set; }

    public char Symbol { get; set; }

    public int X { get; set; }

    public int Y { get; set; }

    public void Draw(IMap map)
    {
        if (!map.IsExplored(X, Y))
        {
            return;
        }

        if (map.IsInFov(X, Y))
        {
            Display.CellAt(0, X, Y).SetContent(Symbol.ToString(), Colors.FloorBackgroundFov, Color);
        }
        else
        {
            Display.CellAt(0, X, Y).SetContent(Symbol.ToString(), Colors.FloorBackground, (Color + Color.gray) * 0.5f);
        }
    }
}


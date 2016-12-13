using RogueSharp;
using UnityEngine;

public class Ability : IAbility, ITreasure, IDrawable
{
    public Ability()
    {
        Symbol = '*';
        Color = Color.yellow;
    }

    public string Name { get; protected set; }

    public int TurnsToRefresh { get; protected set; }

    public int TurnsUntilRefreshed { get; protected set; }

    public bool Perform()
    {
        if (TurnsUntilRefreshed > 0)
        {
            return false;
        }

        TurnsUntilRefreshed = TurnsToRefresh;

        return PerformAbility();
    }

    protected virtual bool PerformAbility()
    {
        return false;
    }


    public void Tick()
    {
        if (TurnsUntilRefreshed > 0)
        {
            TurnsUntilRefreshed--;
        }
    }

    public bool PickUp(IActor actor)
    {
        Player player = actor as Player;

        if (player != null)
        {
            if (player.AddAbility(this))
            {
                Game.MessageLog.Add(string.Format("{0} learned the {1} ability", actor.Name,Name));
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


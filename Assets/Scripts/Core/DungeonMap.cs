﻿
using System.Collections.Generic;
using System.Linq;
using RogueSharp;

public class DungeonMap : Map
{
    public List<Rectangle> Rooms;
    private readonly List<Monster> _monsters;
    public DungeonMap()
    {
        Rooms = new List<Rectangle>();
        _monsters = new List<Monster>();
    }

    public void Draw()
    {
        foreach (var cell in GetAllCells())
        {
            SetConsoleSymbolForCell(cell);
        }

        foreach (Monster monster in _monsters)
        {
            if (IsInFov(monster.X, monster.Y))
            {
                monster.Draw(this);
                monster.DrawStats(Game.MonsterStat, Game.MonsterItem);
            }
        }

    }

    private void SetConsoleSymbolForCell(RogueSharp.Cell cell)
    {
        if (!cell.IsExplored)
            return;
        if (IsInFov(cell.X, cell.Y))
        {
            if (cell.IsWalkable)
                Display.CellAt(0, cell.X, cell.Y).SetContent(".", Colors.FloorBackgroundFov, Colors.FloorFov);
            else
                Display.CellAt(0, cell.X, cell.Y).SetContent("#", Colors.WallBackgroundFov, Colors.WallFov);

        }
        else
        {
            if (cell.IsWalkable)
                Display.CellAt(0, cell.X, cell.Y).SetContent(".", Colors.FloorBackground, Colors.Floor);
            else
                Display.CellAt(0, cell.X, cell.Y).SetContent("#", Colors.WallBackground, Colors.Wall);
        }
    }

    public void UpdatePlayerFieldOfView()
    {
        Player player=Game.Player;
        ComputeFov(player.X, player.Y, player.Awareness, true);
        foreach (var cell in GetAllCells())
        {
            if (IsInFov(cell.X, cell.Y))
                SetCellProperties(cell.X, cell.Y, cell.IsTransparent, cell.IsWalkable, true);
        }
    }

    public bool SetActorPosition(Actor actor, int x, int y)
    {
        if (GetCell(x, y).IsWalkable)
        {
            SetIsWalkable(actor.X, actor.Y, true);
            actor.X = x;
            actor.Y = y;
            SetIsWalkable(actor.X, actor.Y, false);
            if (actor is Player)
            {
                UpdatePlayerFieldOfView();
            }
            return true;
        }
        return false;
    }

    public void SetIsWalkable(int x, int y, bool isWalkable)
    {
        RogueSharp.Cell cell = GetCell(x, y);
        SetCellProperties(cell.X, cell.Y, cell.IsTransparent, isWalkable, cell.IsExplored);
    }

    public void AddPlayer(Player player)
    {
        Game.Player = player;
        SetIsWalkable(player.X, player.Y, false);
        UpdatePlayerFieldOfView();
        Game.SchedulingSystem.Add(player);
    }

    public void AddMonster(Monster monster)
    {
        _monsters.Add(monster);
        SetIsWalkable(monster.X, monster.Y, false);
        Game.SchedulingSystem.Add(monster);
    }

    public Point GetRandomWalkableLocationInRoom(Rectangle room)
    {
        if (DoesRoomHaveWalkableSpace(room))
        {
            for (int i = 0; i < 100; i++)
            {
                int x = Game.Random.Next(1, room.Width - 2) + room.X;
                int y = Game.Random.Next(1, room.Height - 2) + room.Y;
                if (IsWalkable(x, y))
                {
                    return new Point(x, y);
                }
            }
        }

        return null;
    }

    public bool DoesRoomHaveWalkableSpace(Rectangle room)
    {
        for (int x = 1; x <= room.Width - 2; x++)
        {
            for (int y = 1; y <= room.Height - 2; y++)
            {
                if (IsWalkable(x + room.X, y + room.Y))
                {
                    return true;
                }
            }
        }
        return false;
    }

    public void RemoveMonster(Monster monster)
    {
        _monsters.Remove(monster);
        SetIsWalkable(monster.X, monster.Y, true);
        Game.SchedulingSystem.Remove(monster);
    }

    public Monster GetMonsterAt(int x, int y)
    {
        return _monsters.FirstOrDefault(m => m.X == x && m.Y == y);
    }


}

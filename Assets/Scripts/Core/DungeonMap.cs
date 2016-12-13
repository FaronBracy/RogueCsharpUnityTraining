using System.Collections.Generic;
using System.Linq;
using RogueSharp;
using UnityEngine;


public class DungeonMap : Map
{
    private readonly List<Monster> _monsters;
    private readonly List<TreasurePile> _treasurePiles;

    public List<Rectangle> Rooms;
    public List<Door> Doors;
    public Stairs StairsUp;
    public Stairs StairsDown;

    public DungeonMap()
    {
        _monsters = new List<Monster>();
        _treasurePiles = new List<TreasurePile>();
        Game.SchedulingSystem.Clear();

        Rooms = new List<Rectangle>();
        Doors = new List<Door>();
    }

    public void AddMonster(Monster monster)
    {
        _monsters.Add(monster);
        SetIsWalkable(monster.X, monster.Y, false);
        Game.SchedulingSystem.Add(monster);
    }

    public void RemoveMonster(Monster monster)
    {
        _monsters.Remove(monster);
        SetIsWalkable(monster.X, monster.Y, true);
        Game.SchedulingSystem.Remove(monster);
    }

    public Monster GetMonsterAt(int x, int y)
    {
        // BUG: This should be single except sometiems monsters occupy the same space.
        return _monsters.FirstOrDefault(m => m.X == x && m.Y == y);
    }

    public IEnumerable<Point> GetMonsterLocations()
    {
        return _monsters.Select(m => new Point
        {
            X = m.X,
            Y = m.Y
        });
    }

    public IEnumerable<Point> GetMonsterLocationsInFieldOfView()
    {
        return _monsters.Where(monster => IsInFov(monster.X, monster.Y))
            .Select(m => new Point {X = m.X, Y = m.Y});
    }

    public void AddTreasure(int x, int y, ITreasure treasure)
    {
        _treasurePiles.Add(new TreasurePile(x, y, treasure));
    }

    public void AddPlayer(Player player)
    {
        Game.Player = player;
        SetIsWalkable(player.X, player.Y, false);
        UpdatePlayerFieldOfView();
        Game.SchedulingSystem.Add(player);
    }

    public void UpdatePlayerFieldOfView()
    {
        Player player = Game.Player;
        ComputeFov(player.X, player.Y, player.Awareness, true);
        foreach (RogueSharp.Cell cell in GetAllCells())
        {
            if (IsInFov(cell.X, cell.Y))
            {
                SetCellProperties(cell.X, cell.Y, cell.IsTransparent, cell.IsWalkable, true);
            }
        }
    }

    public bool SetActorPosition(Actor actor, int x, int y)
    {
        if (GetCell(x, y).IsWalkable)
        {
            PickUpTreasure(actor, x, y);
            SetIsWalkable(actor.X, actor.Y, true);
            actor.X = x;
            actor.Y = y;
            SetIsWalkable(actor.X, actor.Y, false);
            OpenDoor(actor, x, y);
            if (actor is Player)
            {
                UpdatePlayerFieldOfView();
            }
            return true;
        }
        return false;
    }

    public Door GetDoor(int x, int y)
    {
        return Doors.SingleOrDefault(d => d.X == x && d.Y == y);
    }

    private void OpenDoor(Actor actor, int x, int y)
    {
        Door door = GetDoor(x, y);
        if (door != null && !door.IsOpen)
        {
            door.IsOpen = true;
            var cell = GetCell(x, y);
            SetCellProperties(x, y, true, true, cell.IsExplored);

            Game.MessageLog.Add(string.Format("{0} opened a door", actor.Name));
        }
    }

    public void AddGold(int x, int y, int amount)
    {
        if (amount > 0)
        {
            AddTreasure(x, y, new Gold(amount));
        }
    }

    private void PickUpTreasure(Actor actor, int x, int y)
    {
        List<TreasurePile> treasureAtLocation = _treasurePiles.Where(g => g.X == x && g.Y == y).ToList();
        foreach (TreasurePile treasurePile in treasureAtLocation)
        {
            if (treasurePile.Treasure.PickUp(actor))
            {
                _treasurePiles.Remove(treasurePile);
            }
        }
    }

    public bool CanMoveDownToNextLevel()
    {
        Player player = Game.Player;

        return StairsDown.X == player.X && StairsDown.Y == player.Y;
    }

    public void SetIsWalkable(int x, int y, bool isWalkable)
    {
        RogueSharp.Cell cell = GetCell(x, y);
        SetCellProperties(cell.X, cell.Y, cell.IsTransparent, isWalkable, cell.IsExplored);
    }

    public Point GetRandomLocation()
    {
        int roomNumber = Game.Random.Next(0, Rooms.Count - 1);
        Rectangle randomRoom = Rooms[roomNumber];

        if (!DoesRoomHaveWalkableSpace(randomRoom))
        {
            GetRandomLocation();
        }

        return GetRandomLocationInRoom(randomRoom);
    }

    public Point GetRandomLocationInRoom(Rectangle room)
    {
        int x = Game.Random.Next(1, room.Width - 2) + room.X;
        int y = Game.Random.Next(1, room.Height - 2) + room.Y;
        if (!IsWalkable(x, y))
        {
            GetRandomLocationInRoom(room);
        }
        return new Point(x, y);
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

    public void Draw()
    {
        //mapConsole.Clear();
        foreach (RogueSharp.Cell cell in GetAllCells())
        {
            SetConsoleSymbolForCell(cell);
        }

        foreach (Door door in Doors)
        {
            door.Draw(this);
        }

        StairsUp.Draw(this);
        StairsDown.Draw(this);

        foreach (TreasurePile treasurePile in _treasurePiles)
        {
            IDrawable drawableTreasure = treasurePile.Treasure as IDrawable;
            if (drawableTreasure != null)
                drawableTreasure.Draw(this);
        }

        //statConsole.Clear();
        int i = 0;
        foreach (Monster monster in _monsters)
        {
            monster.Draw(this);
            if (IsInFov(monster.X, monster.Y))
            {
                monster.DrawStats(Game.MonsterStat, Game.MonsterItem);
                i++;
            }
        }

        Player player = Game.Player;

        player.Draw(this);
        player.DrawStats();
        player.DrawInventoryE();
        player.DrawInventoryA();
        player.DrawInventoryI();
    }

    private void SetConsoleSymbolForCell(RogueSharp.Cell cell)
    {
        if (!cell.IsExplored)
        {
            return;
        }

        if (IsInFov(cell.X, cell.Y))
        {
            if (cell.IsWalkable)
            {
                Display.CellAt(0, cell.X, cell.Y).SetContent(".", Colors.FloorBackgroundFov, Colors.FloorFov);
            }
            else
            {
                Display.CellAt(0, cell.X, cell.Y).SetContent("#", Colors.WallBackgroundFov, Colors.WallFov);
            }
        }
        else
        {
            if (cell.IsWalkable)
            {
                Display.CellAt(0, cell.X, cell.Y).SetContent(".", Colors.FloorBackground, Colors.Floor);
            }
            else
            {
                Display.CellAt(0, cell.X, cell.Y).SetContent("#", Colors.WallBackground, Colors.Wall);
            }
        }
    }
}

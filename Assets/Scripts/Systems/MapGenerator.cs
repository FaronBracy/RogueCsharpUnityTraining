
using System;
using System.Linq;
using RogueSharp;
using RogueSharp.DiceNotation;
using UnityEngine;

public class MapGenerator
{
    private readonly int _width;
    private readonly int _height;
    private readonly int _maxRooms;
    private readonly int _roomMaxSize;
    private readonly int _roomMinSize;

    private readonly DungeonMap _map;

    public MapGenerator(int width, int height,int maxRooms, int roomMaxSize, int roomMinSize)
    {
        _width = width;
        _height = height;
        _maxRooms = maxRooms;
        _roomMaxSize = roomMaxSize;
        _roomMinSize = roomMinSize;
        _map = new DungeonMap();
    }

    public DungeonMap CreateMap()
    {
        _map.Initialize(_width, _height);

        for (int r = 0; r < _maxRooms; r++)
        {
            // Determine a the size and position of the room randomly
            int roomWidth = Game.Random.Next(_roomMinSize, _roomMaxSize);
            int roomHeight = Game.Random.Next(_roomMinSize, _roomMaxSize);
            int roomXPosition = Game.Random.Next(0, _width - roomWidth - 1);
            int roomYPosition = Game.Random.Next(0, _height - roomHeight - 1);

            // All of our rooms can be represented as Rectangles
            var newRoom = new Rectangle(roomXPosition, roomYPosition, roomWidth, roomHeight);

            // Check to see if the room rectangle intersects with any other rooms
            bool newRoomIntersects = _map.Rooms.Any(room => newRoom.Intersects(room));
            // As long as it doesn't intersect add it to the list of rooms. Otherwise throw it out
            if (!newRoomIntersects)
            {
                _map.Rooms.Add(newRoom);
            }
        }

        for (int r = 0; r < _map.Rooms.Count; r++)
        {
            // Don't do anything with the first room
            if (r == 0)
            {
                continue;
            }
            // For all remaing rooms get the center of the room and the previous room
            int previousRoomCenterX = _map.Rooms[r - 1].Center.X;
            int previousRoomCenterY = _map.Rooms[r - 1].Center.Y;
            int currentRoomCenterX = _map.Rooms[r].Center.X;
            int currentRoomCenterY = _map.Rooms[r].Center.Y;

            // Give a 50/50 chance of which 'L' shaped connecting cooridors to make
            if (Game.Random.Next(1, 2) == 1)
            {
                CreateHorizontalTunnel(previousRoomCenterX, currentRoomCenterX, previousRoomCenterY);
                CreateVerticalTunnel(previousRoomCenterY, currentRoomCenterY, currentRoomCenterX);
            }
            else
            {
                CreateVerticalTunnel(previousRoomCenterY, currentRoomCenterY, previousRoomCenterX);
                CreateHorizontalTunnel(previousRoomCenterX, currentRoomCenterX, currentRoomCenterY);
            }
        }

        foreach (Rectangle room in _map.Rooms)
        {
            CreateRoom(room);
        }

        PlacePlayer();

        PlaceMonsters();
        return _map;
    }

    private void CreateRoom(Rectangle room)
    {
        for (int x = room.Left + 1; x < room.Right; x++)
        {
            for (int y = room.Top + 1; y < room.Bottom; y++)
            {
                _map.SetCellProperties(x, y, true, true, true);
            }
        }
    }

    private void PlacePlayer()
    {
        Player player = Game.Player;
        if (player == null)
        {
            player = new Player();
        }

        player.X = _map.Rooms[0].Center.X;
        player.Y = _map.Rooms[0].Center.Y;

        _map.AddPlayer(player);
    }

    private void PlaceMonsters()
    {
        foreach (var room in _map.Rooms)
        {
            // Each room has a 60% chance of having monsters
            if (Dice.Roll("1D10") < 7)
            {
                // Generate between 1 and 4 monsters
                var numberOfMonsters = Dice.Roll("1D4");
                for (int i = 0; i < numberOfMonsters; i++)
                {
                    // Find a random walkable location in the room to place the monster
                    Point randomRoomLocation = _map.GetRandomWalkableLocationInRoom(room);
                    // It's possible that the room doesn't have space to place a monster
                    // In that case skip creating the monster
                    if (randomRoomLocation != null)
                    {
                        // Temporarily hard code this monster to be created at level 1
                        var monster = Kobold.Create(1);
                        monster.X = randomRoomLocation.X;
                        monster.Y = randomRoomLocation.Y;
                        _map.AddMonster(monster);
                    }
                }
            }
        }
    }

    private void CreateHorizontalTunnel(int xStart, int xEnd, int yPosition)
    {
        for (int x = Math.Min(xStart, xEnd); x <= Math.Max(xStart, xEnd); x++)
        {
            _map.SetCellProperties(x, yPosition, true, true);
        }
    }

    private void CreateVerticalTunnel(int yStart, int yEnd, int xPosition)
    {
        for (int y = Math.Min(yStart, yEnd); y <= Math.Max(yStart, yEnd); y++)
        {
            _map.SetCellProperties(xPosition, y, true, true);
        }
    }


}


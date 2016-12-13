﻿using System.Collections.Generic;
using System.Linq;
using RogueSharp;
using UnityEngine;

public class TargetingSystem
{
    private enum SelectionType
    {
        None = 0,
        Target = 1,
        Area = 2,
        Line = 3
    }

    public bool IsPlayerTargeting { get; private set; }

    private Point _cursorPosition;
    private List<Point> _selectableTargets = new List<Point>();
    private int _currentTargetIndex;
    private ITargetable _targetable;
    private int _area;
    private SelectionType _selectionType;

    public bool SelectMonster( ITargetable targetable )
    {
        Initialize();
        _selectionType = SelectionType.Target;
        DungeonMap map = Game.DungeonMap;
        _selectableTargets = map.GetMonsterLocationsInFieldOfView().ToList();
        _targetable = targetable;
        _cursorPosition = _selectableTargets.FirstOrDefault();
        if ( _cursorPosition == null )
        {
        StopTargeting();
        return false;
        }

        IsPlayerTargeting = true;
        return true;
    }

    public bool SelectArea( ITargetable targetable, int area = 0 )
    {
        Initialize();
        _selectionType = SelectionType.Area;
        Player player = Game.Player;
        _cursorPosition = new Point { X = player.X, Y = player.Y };
        _targetable = targetable;
        _area = area;

        IsPlayerTargeting = true;
        return true;
    }

    public bool SelectLine( ITargetable targetable )
    {
        Initialize();
        _selectionType = SelectionType.Line;
        Player player = Game.Player;
        _cursorPosition = new Point { X = player.X, Y = player.Y };
        _targetable = targetable;

        IsPlayerTargeting = true;
        return true;
    }

    private void StopTargeting()
    {
        IsPlayerTargeting = false;
        Initialize();
    }

    private void Initialize()
    {
        _cursorPosition = null;
        _selectableTargets = new List<Point>();
        _currentTargetIndex = 0;
        _area = 0;
        _targetable = null;
        _selectionType = SelectionType.None;
    }

    public bool HandleKey( KeyCode key )
    {
        if ( _selectionType == SelectionType.Target )
        {
        HandleSelectableTargeting( key );
        }
        else if ( _selectionType == SelectionType.Area )
        {
        HandleLocationTargeting( key );
        }
        else if ( _selectionType == SelectionType.Line )
        {
        HandleLocationTargeting( key );
        }

        if (key == KeyCode.Return)
        {
            _targetable.SelectTarget(_cursorPosition);
            StopTargeting();
            return true;
        }

        return false;
    }

    private void HandleSelectableTargeting(KeyCode key )
    {
        if ( key == KeyCode.RightArrow || key == KeyCode.DownArrow )
        {
        _currentTargetIndex++;
        if ( _currentTargetIndex >= _selectableTargets.Count )
        {
            _currentTargetIndex = 0;
        }
        _cursorPosition = _selectableTargets[_currentTargetIndex];
        }
        else if ( key == KeyCode.LeftArrow || key == KeyCode.UpArrow )
        {
        _currentTargetIndex--;
        if ( _currentTargetIndex < 0 )
        {
            _currentTargetIndex = _selectableTargets.Count - 1;
        }
        _cursorPosition = _selectableTargets[_currentTargetIndex];
        }
    }

    private void HandleLocationTargeting(KeyCode key )
    {
        int x = _cursorPosition.X;
        int y = _cursorPosition.Y;
        DungeonMap map = Game.DungeonMap;

        if ( key == KeyCode.RightArrow )
        {
        x++;
        }
        else if ( key == KeyCode.LeftArrow )
        {
        x--;
        }
        else if ( key == KeyCode.UpArrow )
        {
        y--;
        }
        else if ( key == KeyCode.DownArrow )
        {
        y++;
        }

        if ( map.IsInFov( x, y ) )
        {
        _cursorPosition.X = x;
        _cursorPosition.Y = y;
        }
    }

    public void Draw()
    {
        if ( IsPlayerTargeting )
        {
        DungeonMap map = Game.DungeonMap;
        Player player = Game.Player;
        if ( _selectionType == SelectionType.Area )
        {
            foreach (RogueSharp.Cell cell in map.GetCellsInArea( _cursorPosition.X, _cursorPosition.Y, _area ) )
            {
                    Display.CellAt(0, cell.X, cell.Y).SetContent(" ", Colors.FloorBackground, Swatch.DbSun);
                }
        }
        else if ( _selectionType == SelectionType.Line )
        {
            foreach (RogueSharp.Cell cell in map.GetCellsAlongLine( player.X, player.Y, _cursorPosition.X, _cursorPosition.Y ) )
            {
                    Display.CellAt(0, cell.X, cell.Y).SetContent(" ", Colors.FloorBackground, Swatch.DbSun);
                }
        }

            Display.CellAt(0, _cursorPosition.X, _cursorPosition.Y).SetContent(" ", Colors.FloorBackground, Swatch.DbLight);
        }
    }
}


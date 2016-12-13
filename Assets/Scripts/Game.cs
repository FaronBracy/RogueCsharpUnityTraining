using System;
using System.Collections;
using System.Collections.Generic;
using RogueSharp.Random;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class Game : MonoBehaviour {
    private static readonly int _mapWidth = 80;
    private static readonly int _mapHeight = 48;



    private static int _mapLevel = 1;
    private static bool _renderRequired = true;

    public static Player Player { get; set; }
    public static DungeonMap DungeonMap { get; private set; }
    public static MessageLog MessageLog { get; private set; }
    public static CommandSystem CommandSystem { get; private set; }
    public static SchedulingSystem SchedulingSystem { get; private set; }
    public static TargetingSystem TargetingSystem { get; private set; }
    public static Text text { get; set; }
    public static GameObject PlayerStat { get; set; }
    public static GameObject MonsterStat { get; set; }
    public static GameObject MonsterItem { get; set; }
    public static GameObject EquipmentItems { get; set; }
    public static GameObject AbilitiesItems { get; set; }
    public static GameObject ItemItems { get; set; }

    public static IRandom Random { get; private set; }

    IEnumerator Start()
    {
        while (!Display.IsInitialized())
        {
            yield return null;
        }
        GameObject.Find("Main Camera").GetComponent<Camera>().orthographicSize = 41;
        int seed = (int)DateTime.UtcNow.Ticks;
        Random = new DotNetRandom(seed);
        text = GameObject.Find("massageText").GetComponent<Text>();
        text.color = Color.white;

        MessageLog = new MessageLog();
        MessageLog.Add("The rogue arrives on level 1");
        MessageLog.Add(string.Format("Level created with seed '{0}'", seed));

        Player = new Player();
        SchedulingSystem = new SchedulingSystem();

        MapGenerator mapGenerator = new MapGenerator(_mapWidth, _mapHeight, 20, 13, 7, _mapLevel);
        DungeonMap = mapGenerator.CreateMap();

        EquipmentItems = GameObject.Find("equipmentItems");
        AbilitiesItems = GameObject.Find("abilitiesItems");
        ItemItems = GameObject.Find("itemItems");

        PlayerStat = GameObject.Find("playerStat");
        MonsterStat = GameObject.Find("monsterStat");
        MonsterItem = Resources.Load<GameObject>("Prefabs/monsterItem");

        CommandSystem = new CommandSystem();
        TargetingSystem = new TargetingSystem();

        Player.Item1 = new RevealMapScroll();
        Player.Item2 = new RevealMapScroll();

        StartCoroutine(OnRootConsoleUpdate());
        StartCoroutine(OnRootConsoleRender());
    }

    private IEnumerator OnRootConsoleUpdate()
    {
        while (Application.isPlaying)
        {
            bool didPlayerAct = false;

            if (TargetingSystem.IsPlayerTargeting)
            {
                if (Input.anyKeyDown)
                {
                    _renderRequired = true;
                    foreach (KeyCode vKey in System.Enum.GetValues(typeof(KeyCode)))
                    {
                        if (Input.GetKey(vKey))
                            TargetingSystem.HandleKey(vKey);
                    }
                }
            }
            else if (CommandSystem.IsPlayerTurn)
            {
                if (Input.anyKeyDown)
                {
                    if (Input.GetKeyDown(KeyCode.UpArrow))
                    {
                        didPlayerAct = CommandSystem.MovePlayer(Direction.Up);
                    }
                    else if (Input.GetKeyDown(KeyCode.DownArrow))
                    {
                        didPlayerAct = CommandSystem.MovePlayer(Direction.Down);
                    }
                    else if (Input.GetKeyDown(KeyCode.LeftArrow))
                    {
                        didPlayerAct = CommandSystem.MovePlayer(Direction.Left);
                    }
                    else if (Input.GetKeyDown(KeyCode.RightArrow))
                    {
                        didPlayerAct = CommandSystem.MovePlayer(Direction.Right);
                    }
                    else if (Input.GetKeyDown(KeyCode.Escape))
                    {
                        //_rootConsole.Close();
                    }
                    else if (Input.GetKeyDown(KeyCode.Period))
                    {
                        if (DungeonMap.CanMoveDownToNextLevel())
                        {
                            
                            MapGenerator mapGenerator = new MapGenerator(_mapWidth, _mapHeight, 20, 13, 7, ++_mapLevel);
                            DungeonMap = mapGenerator.CreateMap();
                            MessageLog = new MessageLog();
                            CommandSystem = new CommandSystem();
                            //_rootConsole.Title = string.Format("RougeSharp RLNet Tutorial - Level {0}", _mapLevel);
                            didPlayerAct = true;
                        }
                    }
                    else
                    {
                        foreach (KeyCode vKey in System.Enum.GetValues(typeof(KeyCode)))
                        {
                            if (Input.GetKey(vKey))
                                didPlayerAct = CommandSystem.HandleKey(vKey);
                        }
                    }

                    if (didPlayerAct)
                    {
                        _renderRequired = true;
                        CommandSystem.EndPlayerTurn();
                    }
                }
            }
            else
            {
                CommandSystem.ActivateMonsters();
                _renderRequired = true;
            }

            yield return null;
        }
    }

    private IEnumerator OnRootConsoleRender()
    {
        while (Application.isPlaying)
        {
            if (_renderRequired)
            {
                for (int i = 0; i < MonsterStat.transform.childCount; i++)
                {
                    var go = MonsterStat.transform.GetChild(i).gameObject;
                    Destroy(go);
                }
//             _mapConsole.Clear();
//             _messageConsole.Clear();
//             _statConsole.Clear();
//             _inventoryConsole.Clear();
                DungeonMap.Draw();
                MessageLog.Draw();
                TargetingSystem.Draw();

                //_rootConsole.Draw();

                _renderRequired = false;
            }
            yield return null;
        }

    }
}

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
    private static bool _renderRequired = true;

    float t0;
    bool pressOn;

    public static CommandSystem CommandSystem { get; private set; }
    public static DungeonMap DungeonMap { get; private set; }
    public static Player Player { get; set; }
    public static MessageLog MessageLog { get; private set; }
    public static Text text { get; set; }
    public static SchedulingSystem SchedulingSystem { get; private set; }
    public static GameObject PlayerStat { get; set; }
    public static GameObject MonsterStat { get; set; }
    public static GameObject MonsterItem { get; set; }
    public static IRandom Random { get; private set; }

    IEnumerator Start () {
        while (!Display.IsInitialized())
        {
            yield return null;
        }
        int seed = (int)DateTime.UtcNow.Ticks;
        Random = new DotNetRandom(seed);
        var mainCam = GameObject.Find("Main Camera");
        SchedulingSystem = new SchedulingSystem();
        
        text = GameObject.Find("massageText").GetComponent<Text>();
        text.color = Colors.TextHeading;

        PlayerStat = GameObject.Find("playerStat");
        MonsterStat = GameObject.Find("monsterStat");
        MonsterItem = Resources.Load<GameObject>("Prefabs/monsterItem");

        MessageLog = new MessageLog();
        MessageLog.Add("The rogue arrives on level 1");
        MessageLog.Add(String.Format("Level created with seed {0}",seed));

        t0 = 0f;
        pressOn = false;
        Player = new Player();

        MapGenerator mapGenerator = new MapGenerator(_mapWidth, _mapHeight, 20, 13, 7);
        DungeonMap = mapGenerator.CreateMap();
        DungeonMap.UpdatePlayerFieldOfView();
        CommandSystem = new CommandSystem();

        mainCam.GetComponent<Camera>().orthographicSize = 40;

        StartCoroutine(OnRootConsoleUpdate());
        StartCoroutine(OnRootConsoleRender());
    }

    public IEnumerator OnRootConsoleUpdate()
    {
        while (Application.isPlaying)
        {
            bool didPlayerAct = false;
            if (CommandSystem.IsPlayerTurn)
            {

                if (Input.anyKey)
                {
                    t0++;
                    pressOn = true;
                    if (t0 < 20)
                    {
                        if (Input.GetKeyDown(KeyCode.UpArrow))
                            didPlayerAct = CommandSystem.MovePlayer(Direction.Up);
                        else if (Input.GetKeyDown(KeyCode.DownArrow))
                            didPlayerAct = CommandSystem.MovePlayer(Direction.Down);
                        else if (Input.GetKeyDown(KeyCode.LeftArrow))
                            didPlayerAct = CommandSystem.MovePlayer(Direction.Left);
                        else if (Input.GetKeyDown(KeyCode.RightArrow))
                            didPlayerAct = CommandSystem.MovePlayer(Direction.Right);
                    }
                    else
                    {
                        if (t0%3 == 0)
                        {
                            if (Input.GetKey(KeyCode.UpArrow))
                                didPlayerAct = CommandSystem.MovePlayer(Direction.Up);
                            else if (Input.GetKey(KeyCode.DownArrow))
                                didPlayerAct = CommandSystem.MovePlayer(Direction.Down);
                            else if (Input.GetKey(KeyCode.LeftArrow))
                                didPlayerAct = CommandSystem.MovePlayer(Direction.Left);
                            else if (Input.GetKey(KeyCode.RightArrow))
                                didPlayerAct = CommandSystem.MovePlayer(Direction.Right);
                        }
                    }


                }
                if (!Input.anyKey && pressOn)
                {
                    t0 = 0;
                    pressOn = false;
                }



                if (didPlayerAct)
                {
                    _renderRequired = true;
                    CommandSystem.EndPlayerTurn();
                }
            }
            else
            {
                Debug.Log("cat");
                CommandSystem.ActivateMonsters();
                _renderRequired = true;
            }


            yield return null;
        }
    }

    public IEnumerator OnRootConsoleRender()
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

                DungeonMap.Draw();
                Player.Draw(DungeonMap);
                MessageLog.Draw();
                Player.DrawStats();
                _renderRequired = false;
            }

            yield return null;
        }
    }

}

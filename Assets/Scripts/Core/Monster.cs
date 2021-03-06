﻿using System;
using UnityEngine;
using UnityEngine.UI;


public class Monster : Actor
   {
      public int? TurnsAlerted { get; set; }

      public void DrawStats(GameObject parent, GameObject item)
      {
        var go = UnityEngine.Object.Instantiate<GameObject>(item);
        go.transform.SetParent(parent.transform);

        var Children = go.GetComponentsInChildren<Transform>();
        foreach (var transform in Children)
        {
            transform.localScale = Vector3.one;
            switch (transform.gameObject.name)
            {
                case "symbol":
                    transform.GetComponent<Text>().text = Symbol.ToString();
                    break;
                case "Scrollbar":
                    transform.GetComponent<Scrollbar>().size = (float)Health / (float)MaxHealth;
                    break;
                case "monsterName":
                    transform.GetComponent<Text>().text = Name;
                    break;
            }
        }
    }



      public static Monster Clone( Monster anotherMonster )
      {
         return new Ooze {
            Attack = anotherMonster.Attack,
            AttackChance = anotherMonster.AttackChance,
            Awareness = anotherMonster.Awareness,
            Color = anotherMonster.Color,
            Defense = anotherMonster.Defense,
            DefenseChance = anotherMonster.DefenseChance,
            Gold = anotherMonster.Gold,
            Health = anotherMonster.Health,
            MaxHealth = anotherMonster.MaxHealth,
            Name = anotherMonster.Name,
            Speed = anotherMonster.Speed,
            Symbol = anotherMonster.Symbol
         };
      }

      public virtual void PerformAction( CommandSystem commandSystem )
      {
         var behavior = new StandardMoveAndAttack();
         behavior.Act( this, commandSystem );
      }
   }

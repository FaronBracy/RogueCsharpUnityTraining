
using System;
using UnityEngine.UI;

public class Player : Actor
{
    public Player()
    {
        Attack = 2;
        AttackChance = 50;
        Awareness = 15;
        Color = Colors.Player;
        Defense = 2;
        DefenseChance = 40;
        Gold = 0;
        Health = 100;
        MaxHealth = 100;
        Name = "Rogue";
        Speed = 10;
        Symbol = "@";
    }

    public void DrawStats()
    {
        var texts = Game.PlayerStat.GetComponentsInChildren<Text>();
        foreach (var text in texts )
        {
            switch (text.text)
            {
                case "Name:":
                    text.text = String.Format("Name:      {0}", Name);
                    break;
                case "Health:":
                    text.text = String.Format("Health:     {0}/{1}", Health,MaxHealth);
                    break;
                case "Attack:":
                    text.text = String.Format("Attack:      {0}/({1})%", Attack, AttackChance);
                    break;
                case "Defense:":
                    text.text = String.Format("Defense:  {0}/({1})%", Defense, DefenseChance);
                    break;
                case "Gold:":
                    text.text = String.Format("Gold:          {0}", Gold);
                    break;

            }

        }

    }
}

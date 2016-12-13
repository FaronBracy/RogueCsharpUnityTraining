
using RogueSharp;
using UnityEngine;


public class Gold : ITreasure, IDrawable 
   {
      public int Amount { get; set; }

      public Gold( int amount )
      {
         Amount = amount;
         Symbol = '$';
         Color = Color.yellow;
      }

      public bool PickUp( IActor actor )
      {
         actor.Gold += Amount;
         Game.MessageLog.Add( string.Format("{0} picked up {1} gold", actor.Name, Amount));
         return true;
      }

      public Color Color { get; set; }
      public char Symbol { get; set; }
      public int X { get; set; }
      public int Y { get; set; }
      public void Draw( IMap map )
      {
         if ( !map.IsExplored( X, Y ) )
         {
            return;
         }

         if ( map.IsInFov( X, Y ) )
         {
            Display.CellAt(0, X, Y).SetContent(Symbol.ToString(), Colors.FloorBackgroundFov, Color);
         }
         else
         {
            Display.CellAt(0, X, Y).SetContent(Symbol.ToString(), Colors.FloorBackground, (Color + Color.gray) * 0.5f);
         }
      }
   }


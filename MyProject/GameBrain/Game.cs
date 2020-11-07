using System;

namespace GameBrain
{
    public class Game
    {
        private Player PlayerA { get; set; }
        private Player PlayerB { get; set; }
        
        
        public Game(Player playerA, Player playerB)
        {
            
            PlayerA = playerA;
            PlayerB = playerB;
        }

        public void PlayRound()
        {
           PlaceShips();
        }

        private void PlaceShips()
        {
            Console.Clear();
            Console.Write($"Player {PlayerA.GetName()}, press ENTER to place ships...");
            Console.ReadLine();
            PlayerA.PlaceShips();
            Console.Clear();
            Console.Write($"Player {PlayerB.GetName()}, press ENTER to place ships...");
            Console.ReadLine();
            PlayerB.PlaceShips();
        }
    }
}
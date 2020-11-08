using System;

namespace GameBrain
{
    public class Game
    {
        private const int BoardWidth = 10;
        private const int BoardHeight = 10;

        private Player PlayerA { get; set; }
        private Player PlayerB { get; set; }

        private static bool _nextMoveByPlayerA = true;

        public Game()
        {
            PlayerA = new Player("PlayerA", BoardWidth, BoardHeight);
            PlayerB = new Player("PlayerB", BoardWidth, BoardHeight);
        }

        public Game(int boardWidth, int boardHeight)
        {
            Console.WriteLine("Enter player one name: ");
            string aName = Console.ReadLine() ?? "PlayerA";
            Console.WriteLine("Enter player two name: ");
            string bName = Console.ReadLine() ?? "PlayerB";
            
            PlayerA = new Player(aName, boardWidth, boardHeight);
            PlayerB = new Player(bName, boardWidth, boardHeight);
        }
        
        public void PlayRound()
        {
            Console.WriteLine("Press ENTER to place ships or type \"R\" for random ships: ");
            string input = Console.ReadLine() ?? "";
            if (input.ToLower() != "r")
            {
                PlaceShips();
            }
            else
            {
                PlaceRandomShips();
            }

            while (true)
            {
                PlaceBombs();
            }
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
            Console.Clear();
            Console.Write("Continue...");
        }

        private void PlaceRandomShips()
        {
            PlayerA.PlaceRandomShips();
            PlayerB.PlaceRandomShips();
        }

        private void PlaceBombs()
        {
            if (_nextMoveByPlayerA)
            {
                PlayerA.PlaceBomb(PlayerB.GetBoard());
                _nextMoveByPlayerA = !_nextMoveByPlayerA;
            }

            PlayerB.PlaceBomb(PlayerA.GetBoard());
            _nextMoveByPlayerA = true;
        }
        
    }
}
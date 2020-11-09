using System;
using System.Collections.Generic;

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
            PlayerA = new Player("Player A", BoardWidth, BoardHeight);
            PlayerB = new Player("Player B", BoardWidth, BoardHeight);
        }
        
        public void PlayRound()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("+---------------------------+\n" +
                              "|      BATTLESHIP BASIC     |\n" +
                              "+---------------------------+");
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write("Press ENTER to place ships or type \"R\" for random ships: ");
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
        
        public void CustomRound()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("+---------------------------+\n" +
                              "|     BATTLESHIP CUSTOM     |\n" +
                              "+---------------------------+");
            Console.ForegroundColor = ConsoleColor.Cyan;
            
            Console.Write("Enter player one name: ");
            string aName = Console.ReadLine() ?? "";
            
            Console.Write("Enter player two name: ");
            string bName = Console.ReadLine() ?? "";
            
            Console.Write("Enter game board width: ");
            string width = Console.ReadLine() ?? "";
            
            Console.Write("Enter game board height: ");
            string height = Console.ReadLine() ?? "";
            
            var boardWidth = int.Parse(width);
            var boardHeight = int.Parse(height);
            
            List<Ship> ships = new List<Ship>();
            
            Console.WriteLine("Enter number of Patrol (1x1) ships: ");
            string patrol = Console.ReadLine() ?? "";
            var nPatrol = int.Parse(patrol);
            
            Console.WriteLine("Enter number of Cruiser (2x1) ships: ");
            string cruiser = Console.ReadLine() ?? "";
            var nCruiser = int.Parse(cruiser);

            Console.WriteLine("Enter number of Submarine (3x1) ships: ");
            string submarine = Console.ReadLine() ?? "";
            var nSubmarine = int.Parse(submarine);

            Console.WriteLine("Enter number of Battleship (4x1) ships: ");
            string battleship = Console.ReadLine() ?? "";
            var nBattleship = int.Parse(battleship);

            Console.WriteLine("Enter number of Carrier (5x1) ships: ");
            string carrier = Console.ReadLine() ?? "";
            var nCarrier = int.Parse(carrier);

            for (var i = 0; i < nPatrol; i++)
            {
                ships.Add(new Ship(1));
            }
            for (var i = 0; i < nCruiser; i++)
            {
                ships.Add(new Ship(2));
            }
            for (var i = 0; i < nSubmarine; i++)
            {
                ships.Add(new Ship(3));
            }
            for (var i = 0; i < nBattleship; i++)
            {
                ships.Add(new Ship(4));
            }
            for (var i = 0; i < nCarrier; i++)
            {
                ships.Add(new Ship(5));
            }
            
            Console.WriteLine("Can ships touch? (Y/N): ");
            string canTouch = Console.ReadLine() ?? "";

            var shipsCanTouch = true;
            if (canTouch.ToLower() == "y")
            {
                shipsCanTouch = true;
            }

            if (canTouch.ToLower() == "n")
            {
                shipsCanTouch = false;
            }
            
            
            PlayerA = new Player(aName, boardWidth, boardHeight, shipsCanTouch);
            PlayerB = new Player(bName, boardWidth, boardHeight, shipsCanTouch);
            
            PlayerA.SetShips(ships);
            PlayerB.SetShips(ships);
            
            Console.Write("Press ENTER to place ships or type \"R\" for random ships: ");
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
                PlayerA.PlaceBomb(PlayerB);
                _nextMoveByPlayerA = !_nextMoveByPlayerA;
            }

            PlayerB.PlaceBomb(PlayerA);
            _nextMoveByPlayerA = true;
        }
        
    }
}
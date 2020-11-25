using System;
using System.Collections.Generic;
using System.Linq;
using Domain.Enums;

namespace GameBrain
{
    public class CustomOptions
    {
        public GameOptions GetCustomOptions()
        {
            Console.Write("Set board width: ");
            var width = int.Parse(Console.ReadLine());

            Console.Write("Set board height: ");
            var height = int.Parse(Console.ReadLine());

            Console.Write("Can ships touch? (Y/N): ");
            var canTouch = Console.ReadLine();
            var canShipsTouch = canTouch.ToLower() == "y" ? EShipsCanTouch.Yes : EShipsCanTouch.No;
            
            Console.Write("Can player move again after successful hit?(Y/N): ");
            var nextMove = Console.ReadLine();
            var nextMoveAfterHit = nextMove.ToLower() == "y" ? ENextMoveAfterHit.SamePlayer : ENextMoveAfterHit.OtherPlayer;
           

            Console.Write("Set player A name: ");
            string playerAName = Console.ReadLine();

            Console.Write("Set player B name: ");
            string playerBName = Console.ReadLine();

            List<Ship> ships = new List<Ship>();

            do
            {
                Console.WriteLine("Do you want Patrol (1x1) ships? (Y/N): ");
                if (Console.ReadLine().ToLower() != "n")
                {
                    ships.Add(new Ship(1));
                }

                Console.WriteLine("Do you want Cruiser (2x1) ships?: ");
                if (Console.ReadLine().ToLower() != "n")
                {
                    ships.Add(new Ship(2));
                }

                Console.WriteLine("Do you want Submarine (3x1) ships?: ");
                if (Console.ReadLine().ToLower() != "n")
                {
                    ships.Add(new Ship(3));
                }

                Console.WriteLine("Do you want Battleship (4x1) ships?: ");
                if (Console.ReadLine().ToLower() != "n")
                {
                    ships.Add(new Ship(4));
                }

                Console.WriteLine("Do you want Carrier (5x1) ships?: ");
                if (Console.ReadLine().ToLower() != "n")
                {
                    ships.Add(new Ship(5));
                }

                Console.WriteLine("Do you want Custom (?x1) ships?: ");
                var longestShip = ships.Select(ship => ship.Width).Prepend(0).Max();

                if (Console.ReadLine().ToLower() != "n")
                {
                    Console.WriteLine($"Select custom ship size {longestShip + 1} - {width}");
                    ships.Add(new Ship(int.Parse(Console.ReadLine())));
                }
            } while (ships.Count == 0);
            
            return new GameOptions(width, height, new Player(playerAName), new Player(playerBName), canShipsTouch, ships, nextMoveAfterHit)
            {
                // BoardWidth = width,
                // BoardHeight = height,
                // NextMoveAfterHit = nextMoveAfterHit,
                // // NextMoveByPlayerA = true,
                // PlayerA = new Player()
                // {
                //     // Nullable?
                //     Name = playerAName!
                // },
                // PlayerB = new Player()
                // {
                //     // Nullable?
                //     Name = playerBName!
                // },
                // Ships = ships,
                // ShipsCanTouch = canShipsTouch
            };
        }

        public void ValidateInput()
        {
            
        }
    }
}
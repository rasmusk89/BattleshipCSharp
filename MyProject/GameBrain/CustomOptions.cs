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
            Console.Clear();
            Console.Write("Set board width: ");
            var widthInput =Console.ReadLine() ?? "10";
            while (!NumericInputCorrect(widthInput!))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write("Please enter correct width: ");
                widthInput = Console.ReadLine();
                Console.ForegroundColor = ConsoleColor.Cyan;
            }
            var width = int.Parse(widthInput);

            Console.Write("Set board height: ");
            var heightInput = Console.ReadLine() ?? "10";
            while (!NumericInputCorrect(heightInput!))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write("Please enter correct height: ");
                heightInput = Console.ReadLine();
                Console.ForegroundColor = ConsoleColor.Cyan;
            }
            var height = int.Parse(heightInput);
            
            Console.Write("Can ships touch? (Y/N): ");
            var canTouch = Console.ReadLine();
            var canShipsTouch = canTouch.ToLower() == "y" ? EShipsCanTouch.Yes : EShipsCanTouch.No;
            
            Console.Write("Can player move again after successful hit?(Y/N): ");
            var nextMove = Console.ReadLine();
            var nextMoveAfterHit = nextMove.ToLower() == "y" ? ENextMoveAfterHit.SamePlayer : ENextMoveAfterHit.OtherPlayer;

            Console.Write("Set player one type (\"A\"-AI/\"H\"-Human)");
            var playerATypeInput = Console.ReadLine()?.ToLower() == "a" ? EPlayerType.Ai : EPlayerType.Human;
            
            Console.Write("Set player two type (\"A\"-AI/\"H\"-Human)");
            var playerBTypeInput = Console.ReadLine()?.ToLower() == "a" ? EPlayerType.Ai : EPlayerType.Human;
            
            
            List<Ship> ships = new ();

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

            return new GameOptions(width, height, canShipsTouch, nextMoveAfterHit, ships, playerATypeInput, playerBTypeInput);

        }

        private bool NumericInputCorrect(string number)
        {
            if (!IsNumeric(number))
            {
                return false;
            }

            return int.Parse(number) >= 1 && int.Parse(number) <= 60;
        }
        
        private bool IsNumeric(string x)
        {
            return int.TryParse(x, out _);
        }
    }
}
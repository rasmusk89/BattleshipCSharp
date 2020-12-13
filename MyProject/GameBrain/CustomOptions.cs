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
            Console.Write("Set game board width: ");
            var widthInput = Console.ReadLine();
            while (!NumericInputCorrect(widthInput!) || int.Parse(widthInput!) < 3 || int.Parse(widthInput) > 60)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write("Please enter correct numeric width value between 3 and 60: ");
                widthInput = Console.ReadLine();
                Console.ForegroundColor = ConsoleColor.Cyan;
            }

            var width = int.Parse(widthInput);


            Console.Write("Set game board height: ");
            var heightInput = Console.ReadLine();
            while (!NumericInputCorrect(heightInput!) || int.Parse(heightInput!) < 3 || int.Parse(heightInput) > 60)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write("Please enter correct numeric height value between 3 and 60: ");
                heightInput = Console.ReadLine();
                Console.ForegroundColor = ConsoleColor.Cyan;
            }

            var height = int.Parse(heightInput);

            Console.Write("Can ships touch? (Y/N): ");
            var canTouch = Console.ReadKey();
            var canShipsTouch = canTouch.Key == ConsoleKey.Y ? EShipsCanTouch.Yes : EShipsCanTouch.No;
            Console.WriteLine();

            Console.Write("Can player move again after successful hit?(Y/N): ");
            var nextMove = Console.ReadKey();
            var nextMoveAfterHit = nextMove.Key == ConsoleKey.Y
                ? ENextMoveAfterHit.SamePlayer
                : ENextMoveAfterHit.OtherPlayer;
            Console.WriteLine();

            Console.Write("Set player one type (A for AI/H for Human): ");
            var playerAType = Console.ReadKey().Key == ConsoleKey.A ? EPlayerType.Ai : EPlayerType.Human;
            Console.WriteLine();

            Console.Write("Set player two type (A for AI/H for Human): ");
            var playerBType = Console.ReadKey().Key == ConsoleKey.A ? EPlayerType.Ai : EPlayerType.Human;
            Console.WriteLine();

            var widthShips = width / 2;
            var heightShips = height / 2;

            var maxShips = widthShips <= heightShips ? widthShips : heightShips;

            if (width < 5 || height < 5)
            {
                maxShips = width < height ? width : height;
            }

            List<Ship> ships = new();

            for (var i = 1; i <= maxShips; i++)
            {
                ships.Add(new Ship(i));
            }

            Console.WriteLine("Current ships: ");
            foreach (var ship in ships)
            {
                Console.WriteLine(ship.Width + ": " + ship.Name);
            }

            Console.ReadLine();
            // do
            // {
            //     Console.Write("Do you want Patrol (1x1) ships? (Y/N): ");
            //     if (Console.ReadKey().Key == ConsoleKey.Y)
            //     {
            //         ships.Add(new Ship(1));
            //     }
            //     Console.WriteLine();
            //
            //     Console.Write("Do you want Cruiser (2x1) ships?: ");
            //     if (Console.ReadKey().Key == ConsoleKey.Y)
            //     {
            //         ships.Add(new Ship(2));
            //     }
            //     Console.WriteLine();
            //
            //     Console.Write("Do you want Submarine (3x1) ships?: ");
            //     if (Console.ReadKey().Key == ConsoleKey.Y)
            //     {
            //         ships.Add(new Ship(3));
            //     }
            //     Console.WriteLine();
            //
            //     Console.Write("Do you want Battleship (4x1) ships?: ");
            //     if (Console.ReadKey().Key == ConsoleKey.Y)
            //     {
            //         ships.Add(new Ship(4));
            //     }
            //     Console.WriteLine();
            //
            //     Console.Write("Do you want Carrier (5x1) ships?: ");
            //     if (Console.ReadKey().Key == ConsoleKey.Y)
            //     {
            //         ships.Add(new Ship(5));
            //     }
            //     Console.WriteLine();
            //
            //     Console.Write("Do you want Custom (?x1) ships?: ");
            //     
            //     if (Console.ReadKey().Key == ConsoleKey.Y)
            //     {
            //         Console.WriteLine();
            //         var longestShip = ships.Select(ship => ship.Width).Prepend(0).Max();
            //         Console.Write($"Select custom ship size {longestShip + 1} - {width - 2}");
            //         ships.Add(new Ship(int.Parse(Console.ReadLine())));
            //     }
            // } while (ships.Count == 0);

            return new GameOptions(width, height, canShipsTouch, nextMoveAfterHit, ships, playerAType, playerBType);
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
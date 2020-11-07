using System;
using System.Collections.Generic;

namespace GameBrain
{
    public class Player
    {
        private string Name { get; set; }
        private GameBoard PlayerBoard { get; set; }
        private List<Ship> Ships { get; set; }

        public Player(string name, int boardWidth, int boardHeight)
        {
            Name = name;
            Ships = new List<Ship>
            {
                new Ship(1),
                new Ship(2),
                new Ship(3),
                new Ship(4),
                new Ship(5)
            };

            PlayerBoard = new GameBoard(boardWidth, boardHeight);
        }

        public string GetName()
        {
            return Name;
        }

        public void PlaceShips()
        {
            var ui = new GameBoardUI();
            foreach (var ship in Ships)
            {
                Console.Clear();
                ui.DrawBoard(PlayerBoard);
                Console.WriteLine($"Ship: {ship.Name}, Size: {ship.Width}x1");

                Coordinates coordinates = AskCoordinates();
                var orientation = AskOrientation();

                while (!CoordinatesAreValid(coordinates, orientation, PlayerBoard, ship.Width))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Out of bounds!");
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    coordinates = AskCoordinates();
                    orientation = AskOrientation();
                }
                
                while (!AreaFree(coordinates, orientation, PlayerBoard, ship.Width))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Boat already placed there!");
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    coordinates = AskCoordinates();
                    orientation = AskOrientation();
                }


                PlaceShip(coordinates, ship, orientation);
            }
        }

        private Coordinates AskCoordinates()
        {
            Console.Write("Insert Row: ");
            string rowInput = Console.ReadLine() ?? "";
            while (!RowIsValid(rowInput, PlayerBoard))
            {
                Console.Write("Please enter correct row: ");
                rowInput = Console.ReadLine() ?? "";
            }
            var row = int.Parse(rowInput);
            
            
            Console.Write("Insert Column: ");
            string columnInput = Console.ReadLine() ?? "";
            while (!ColumnIsValid(columnInput, PlayerBoard))
            {
                Console.Write("Please enter correct column: ");
                columnInput = Console.ReadLine() ?? "";
            }
            var column = int.Parse(columnInput);

            return new Coordinates(row, column);
        }

        private static EOrientation AskOrientation()
        {
            Console.Write("Insert orientation Horizontal(H) or Vertical(V): ");
            string input = Console.ReadLine() ?? "";

            while (!OrientationIsValid(input))
            {
                Console.Write("Please enter correct orientation (H) or (V): ");
                input = Console.ReadLine() ?? "";
            }

            var orientation = input.ToLower() switch
            {
                "h" => EOrientation.Horizontal,
                "v" => EOrientation.Vertical,
                _ => EOrientation.Horizontal
            };

            return orientation;
        }

        private static bool OrientationIsValid(string input)
        {
            return input.ToLower() == "h" || input.ToLower() == "v";
        }

        private static bool RowIsValid(string input, GameBoard board)
        {
            var boardWidth = board.Board.GetUpperBound(0) + 1;

            if (IsNumeric(input))
            {
                return int.Parse(input) < boardWidth;
            }

            return false;
        }

        private static bool ColumnIsValid(string input, GameBoard board)
        {
            var boardHeight = board.Board.GetUpperBound(1) + 1;

            if (IsNumeric(input))
            {
                return int.Parse(input) < boardHeight;
            }

            return false;
        }
        
        private static bool IsNumeric(string x)
        {
            return int.TryParse(x, out _);
        }

        private static bool CoordinatesAreValid(Coordinates coordinates, EOrientation orientation, GameBoard board,
            int shipSize)
        {
            var boardWidth = board.Board.GetUpperBound(0) + 1;
            var boardHeight = board.Board.GetUpperBound(1) + 1;

            var row = coordinates.Row - 1;
            var column = coordinates.Column - 1;

            if (orientation == EOrientation.Horizontal)
            {
                return row + shipSize <= boardWidth;
            }
            return column + shipSize <= boardHeight;
        }

        private static bool AreaFree(Coordinates coordinates, EOrientation orientation, GameBoard board, int ShipSize)
        {
            var row = coordinates.Row - 1;
            var column = coordinates.Column - 1;
            

            var occupiedCells = 0;
            
            if (orientation == EOrientation.Horizontal)
            {
                for (var i = 0; i < ShipSize; i++)
                {
                    if (board.Board[row + i, column] == ECellState.Boat)
                    {
                        occupiedCells++;
                    }
                }
            }
            else
            {
                for (var i = 0; i < ShipSize; i++)
                {
                    if (board.Board[row, column + i] == ECellState.Boat)
                    {
                        occupiedCells++;
                    }
                }
            }

            return occupiedCells == 0;
        }

        private void PlaceShip(Coordinates coordinates, Ship ship, EOrientation orientation)
        {
            var row = coordinates.Row;
            var column = coordinates.Column;
            var shipSize = ship.Width;
            var board = PlayerBoard.Board;


            Console.WriteLine(ship.Name);
            if (orientation == EOrientation.Horizontal)
            {
                for (var i = row; i < row + shipSize; i++)
                {
                    board![i - 1, column - 1] = ECellState.Boat;
                }
            }

            if (orientation != EOrientation.Vertical) return;
            {
                for (var i = column; i < column + shipSize; i++)
                {
                    board![row - 1, i - 1] = ECellState.Boat;
                }
            }
        }
    }
}
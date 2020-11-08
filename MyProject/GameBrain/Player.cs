using System;
using System.Collections.Generic;
using System.Linq;

namespace GameBrain
{
    public class Player
    {
        private string Name { get; set; }
        private GameBoard PlayerBoard { get; set; }
        private FiringBoard OpponentBoard { get; set; }
        private List<Ship> Ships { get; set; }

        private readonly Validator _validator = new Validator();

        public bool HasLost
        {
            get { return Ships.All(x => x.IsSunk); }
        }

        public GameBoard GetBoard()
        {
            return PlayerBoard;
        }

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
            OpponentBoard = new FiringBoard(boardWidth, boardHeight);
        }

        public string GetName()
        {
            return Name;
        }

        public void SetName(string name)
        {
            Name = name;
        }

        public void PlaceBomb(GameBoard anotherBoard)
        {
            DrawBoardUI();
            Console.WriteLine("Place Bomb!");
            Coordinates coordinates = AskCoordinates();

            while (!Validator.BombCoordinateFree(coordinates, OpponentBoard))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Bomb has already placed there!");
                Console.ForegroundColor = ConsoleColor.Cyan;
                coordinates = AskCoordinates();
            }

            var column = coordinates.Column - 1;
            var row = coordinates.Row - 1;
            var board = OpponentBoard.Board;

            board[column, row] = ECellState.Bomb;

            if (anotherBoard.Board[column, row] == ECellState.Boat)
            {
                Console.WriteLine("HIT!");
                Console.ReadLine();
                OpponentBoard.Board[column, row] = ECellState.Hit;
                anotherBoard.Board[column, row] = ECellState.Hit;
            }

            if (anotherBoard.Board[column, row] == ECellState.Empty)
            {
                anotherBoard.Board[column, row] = ECellState.Bomb;
            }

            DrawBoardUI();
            Console.Write("Continue...");
            Console.ReadLine();
        }

        public void PlaceShips()
        {
            foreach (var ship in Ships)
            {
                DrawBoardUI();

                Console.WriteLine($"Ship: {ship.Name}, Size: {ship.Width}x1");

                Coordinates coordinates = AskCoordinates();
                var orientation = EOrientation.Horizontal;
                if (ship.Width != 1)
                {
                    orientation = AskOrientation();
                }

                while (!Validator.CoordinatesAreValid(coordinates, orientation, PlayerBoard, ship.Width))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Out of bounds!");
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    coordinates = AskCoordinates();
                    orientation = AskOrientation();
                }

                while (!Validator.AreaFree(coordinates, orientation, PlayerBoard, ship.Width))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Boat already placed there!");
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    coordinates = AskCoordinates();
                    orientation = AskOrientation();
                }

                PlaceShip(coordinates, ship, orientation);
            }

            DrawBoardUI();
            Console.Write("Continue...");
            Console.ReadLine();
        }

        private void PlaceShip(Coordinates coordinates, Ship ship, EOrientation orientation)
        {
            var row = coordinates.Row;
            var column = coordinates.Column;
            var shipSize = ship.Width;
            var board = PlayerBoard.Board;

            if (orientation == EOrientation.Horizontal)
            {
                for (var i = column; i < column + shipSize; i++)
                {
                    board[i - 1, row - 1] = ECellState.Boat;
                }
            }

            if (orientation != EOrientation.Vertical) return;
            {
                for (var i = row; i < row + shipSize; i++)
                {
                    board[column - 1, i - 1] = ECellState.Boat;
                }
            }
        }

        public void PlaceRandomShips()
        {
            Random rand = new Random(Guid.NewGuid().GetHashCode());
            var boardWidth = PlayerBoard.Board.GetUpperBound(0) + 1;
            var boardHeight = PlayerBoard.Board.GetUpperBound(1) + 1;
            var board = PlayerBoard.Board;


            foreach (var ship in Ships)
            {
                var isOpen = true;
                while (isOpen)
                {
                    var startColumn = rand.Next(1, boardWidth);    // 1
                    var startRow = rand.Next(1, boardHeight);     // 2
                    var endColumn = startColumn;                 // 1
                    var endRow = startRow;                      // 2
                    var orientation = rand.Next(1, 101) % 2; //0 for Horizontal

                    if (orientation == 0)
                    {
                        for (var i = 1; i < ship.Width; i++)
                        {
                            endRow++;
                        }
                    }

                    else
                    {
                        for (var i = 1; i < ship.Width; i++)
                        {
                            endColumn++;
                        }
                    }

                    //We cannot place ships beyond the boundaries of the board
                    if (endRow > boardHeight || endColumn > boardWidth)
                    {
                        isOpen = true;
                        continue; //Restart the while loop to select a new random panel
                    }

                    var occupiedCells = 0;

                    // i = 1
                    // j = 2
                    for (var i = startColumn; i <= endColumn; i++)
                    {
                        for (var j = startRow; j <= endRow; j++)
                        {
                            if (board[i - 1, j - 1] == ECellState.Boat)
                            {
                                occupiedCells++;
                            }
                        }
                    }

                    if (occupiedCells != 0)
                    {
                        isOpen = true;
                        continue;
                    }

                    for (var i = startColumn; i <= endColumn; i++)
                    {
                        for (var j = startRow; j <= endRow; j++)
                        {
                            board[i - 1, j - 1] = ECellState.Boat;
                        }
                    }
                    isOpen = false;    
                }
            }
        }

        private Coordinates AskCoordinates()
        {
            var boardWidth = PlayerBoard.Board.GetUpperBound(0) + 1;
            Console.Write($"Insert Column (A-{IntToAlphabeticValue(boardWidth - 1)}): ");
            string columnInput = Console.ReadLine() ?? "";

            while (!_validator.ColumnIsValid(columnInput, PlayerBoard))
            {
                Console.Write($"Please insert correct Column (A-{IntToAlphabeticValue(boardWidth - 1)}): ");
                columnInput = Console.ReadLine() ?? "";
            }

            var column = _validator.ConvertStringToInteger(columnInput);

            var boardHeight = PlayerBoard.Board.GetUpperBound(1) + 1;
            Console.Write($"Insert Row (1-{boardHeight}): ");
            string rowInput = Console.ReadLine() ?? "";
            while (!_validator.RowIsValid(rowInput, PlayerBoard))
            {
                Console.Write($"Please enter correct row (1-{boardHeight}): ");
                rowInput = Console.ReadLine() ?? "";
            }

            // Convert string to integer.
            var row = int.Parse(rowInput);


            return new Coordinates(column, row);
        }

        private static EOrientation AskOrientation()
        {
            Console.Write("Insert orientation Horizontal(H) or Vertical(V): ");
            string input = Console.ReadLine() ?? "";

            while (!Validator.OrientationIsValid(input))
            {
                Console.Write("Please enter correct orientation (H) or (V): ");
                input = Console.ReadLine() ?? "";
            }

            var orientation = input.Trim().ToLower() switch
            {
                "h" => EOrientation.Horizontal,
                "v" => EOrientation.Vertical,
                _ => EOrientation.Horizontal
            };

            return orientation;
        }

        private static string IntToAlphabeticValue(int index)
        {
            const string letters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";

            var value = "";

            if (index >= letters.Length)
                value += letters[index / letters.Length - 1];

            value += letters[index % letters.Length];

            return value;
        }

        private void DrawBoardUI()
        {
            Console.Clear();
            Console.WriteLine($"- - -{Name.ToUpper()} BOARD- - -");
            GameBoardUI.DrawBoard(PlayerBoard);
            Console.WriteLine();
            Console.WriteLine("- - -OPPONENT BOARD- - -");
            GameBoardUI.DrawBoard(OpponentBoard);
            Console.WriteLine();
        }

        private static bool BoatsCanTouch()
        {
            return true;
        }
    }
}
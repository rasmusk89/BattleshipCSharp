using System;
using System.Collections.Generic;
using System.Linq;

namespace GameBrain
{
    public class Player
    {
        private readonly bool _shipsCanTouch;
        private string Name { get; set; }
        private GameBoard PlayerBoard { get; set; }
        private FiringBoard OpponentBoard { get; set; }

        private List<Ship> Ships { get; set; } = new List<Ship>
        {
            new Ship(1),
            new Ship(2),
            new Ship(3),
            new Ship(4),
            new Ship(5)
        };

        private readonly Validator _validator = new Validator();

        public bool HasLost
        {
            get { return Ships.All(x => x.IsSunk); }
        }

        public Player(string name, int boardWidth, int boardHeight, bool shipsCanTouch=false)
        {
            _shipsCanTouch = shipsCanTouch;
            Name = name;

            PlayerBoard = new GameBoard(boardWidth, boardHeight);
            OpponentBoard = new FiringBoard(boardWidth, boardHeight);
        }

        private ECellState[,] GetBoard()
        {
            return PlayerBoard.Board;
        }

        public void SetShips(List<Ship> ships)
        {
            Ships = ships;
        }


        public string GetName()
        {
            return Name;
        }

        public void PlaceBomb(Player opponent)
        {
            var opponentBoard = opponent.GetBoard();
            var firingBoard = OpponentBoard.Board;

            DrawBoardUI();
            Console.WriteLine($"{Name}, place Bomb!");
            Coordinates coordinates = AskCoordinates();

            while (!_validator.BombCoordinateFree(coordinates, OpponentBoard))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Bomb has already placed there!");
                Console.ForegroundColor = ConsoleColor.Cyan;
                coordinates = AskCoordinates();
            }

            var column = coordinates.Column - 1;
            var row = coordinates.Row - 1;

            if (opponentBoard[column, row] != ECellState.Empty)
            {
                var cellState = opponentBoard[column, row];
                var ship = Ships.First(x => x.CellState == cellState);
                ship.Hits++;
                firingBoard[column, row] = ECellState.Hit;
                opponentBoard[column, row] = ECellState.Hit;
                DrawBoardUI();
                Console.WriteLine("HIT!");
                if (ship.IsSunk)
                {
                    Console.WriteLine($"You sunk opponents {ship.Name}");
                }

                Console.Write("Continue...");
                Console.ReadLine();
            }

            if (opponentBoard[column, row] != ECellState.Empty) return;
            firingBoard[column, row] = ECellState.Bomb;
            opponentBoard[column, row] = ECellState.Bomb;
            DrawBoardUI();
            Console.WriteLine("MISS!");
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

                while (!_validator.CoordinatesAreValid(coordinates, orientation, PlayerBoard, ship.Width))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Out of bounds!");
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    coordinates = AskCoordinates();
                    orientation = AskOrientation();
                }

                while (!_validator.AreaFree(coordinates, orientation, PlayerBoard, ship.Width))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Ship already placed there!");
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
            var column = coordinates.Column;
            var row = coordinates.Row;
            var shipSize = ship.Width;
            var board = PlayerBoard.Board;
            var state = ship.CellState;

            if (orientation == EOrientation.Horizontal)
            {
                for (var i = column; i < column + shipSize; i++)
                {
                    board[i - 1, row - 1] = state;
                }
            }

            if (orientation != EOrientation.Vertical) return;
            {
                for (var i = row; i < row + shipSize; i++)
                {
                    board[column - 1, i - 1] = state;
                }
            }
        }

        public void PlaceRandomShips()
        {
            Random rand = new Random(Guid.NewGuid().GetHashCode());
            var boardWidth = PlayerBoard.Board.GetUpperBound(0) + 1;
            var boardHeight = PlayerBoard.Board.GetUpperBound(1) + 1;
            var board = PlayerBoard.Board;
            var counter = 0;
            foreach (var ship in Ships)
            {
                var isOpen = true;
                while (isOpen)
                {
                    var startColumn = rand.Next(1, boardWidth);
                    var startRow = rand.Next(1, boardHeight);
                    var endColumn = startColumn;
                    var endRow = startRow;
                    var orientation = rand.Next(1, 101) % 2; //0 for Horizontal
                    var state = ship.CellState;

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

                    if (!_shipsCanTouch)
                    {
                        var checkStartColumn = startColumn;
                        var checkEndColumn = endColumn;
                        var checkStartRow = startRow;
                        var checkEndRow = endRow;
                        if (startColumn > 1)
                        {
                            checkStartColumn = startColumn - 1;
                        }

                        if (endColumn < boardWidth)
                        {
                            checkEndColumn = endColumn + 1;
                        }

                        if (startRow > 1)
                        {
                            checkStartRow = startRow - 1;
                        }

                        if (endRow < boardHeight)
                        {
                            checkEndRow = endRow + 1;
                        }

                        for (var i = checkStartColumn; i <= checkEndColumn; i++)
                        {
                            for (var j = checkStartRow; j <= checkEndRow; j++)
                            {
                                if (board[i - 1, j - 1] != ECellState.Empty)
                                {
                                    occupiedCells++;
                                }
                            }
                        }
                    }
                    else
                    {
                        for (var i = startColumn; i <= endColumn; i++)
                        {
                            for (var j = startRow; j <= endRow; j++)
                            {
                                if (board[i - 1, j - 1] == ECellState.Empty)
                                {
                                    continue;
                                }

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
                            board[i - 1, j - 1] = state;
                        }
                    }
                    counter++;
                    isOpen = false;
                }
            }
            
            Console.Clear();
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

        private EOrientation AskOrientation()
        {
            Console.Write("Insert orientation Horizontal(H) or Vertical(V): ");
            string input = Console.ReadLine() ?? "";

            while (!_validator.OrientationIsValid(input))
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
            Console.WriteLine($"- - - {Name.ToUpper()} BOARD - - -");
            GameBoardUI.DrawBoard(PlayerBoard);
            Console.WriteLine();
            Console.WriteLine("- - - OPPONENT BOARD - - -");
            GameBoardUI.DrawBoard(OpponentBoard);
            Console.WriteLine();
        }

    }
}
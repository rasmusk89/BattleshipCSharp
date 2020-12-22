using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Domain.Enums;

namespace GameBrain
{
    public class Player
    {
        public string Name { get; set; } = "Player";

        public EPlayerType PlayerType { get; set; }

        public List<Ship> Ships { get; set; } = new();

        public GameBoard GameBoard { get; set; } = null!;

        public EShipsCanTouch ShipsCanTouch { get; set; }


        public bool HasLost
        {
            get { return Ships.All(x => x.IsSunk); }
        }

        public void SetNewBoard(int width, int height)
        {
            GameBoard = new GameBoard(width, height);
        }

        public ECellState[,] GetPlayerBoard()
        {
            return GameBoard.Board;
        }

        public ECellState GetCell(int x, int y)
        {
            return GameBoard.Board[x, y];
        }

        public IEnumerable<Ship> GetShips()
        {
            return Ships;
        }

        public void SetShips(IEnumerable<Ship> ships)
        {
            foreach (var ship in ships)
            {
                Ships.Add(new Ship(ship.Width)
                {
                    CellState = ship.CellState,
                    Hits = ship.Hits,
                    Name = ship.Name,
                });
            }
        }

        public string GetName()
        {
            return Name;
        }

        public void SetName(string name)
        {
            Name = name;
        }

        public EPlayerType GetPlayerType()
        {
            return PlayerType;
        }

        public void SetPlayerType(EPlayerType playerType)
        {
            PlayerType = playerType;
        }

        // Check if bomb is hit on opponent board.
        private static bool IsHit(int column, int row, Player opponent)
        {
            var playerCell = opponent.GetPlayerBoard()[column, row];
            return playerCell != ECellState.Empty && playerCell != ECellState.Bomb && playerCell != ECellState.Hit;
        }

        // Add hit on opponent ship.
        private static void RegisterHit(int column, int row, Player opponent)
        {
            var shipState = opponent.GetPlayerBoard()[column, row];

            foreach (var opponentShip in opponent.Ships.Where(opponentShip =>
                opponentShip.CellState == shipState && !opponentShip.IsSunk))
            {
                opponentShip.Hits++;
            }
        }

        // Return true if bomb is hit, false if miss.
        public bool PlaceBomb(int column, int row, Player opponent)
        {
            if (IsHit(column, row, opponent))
            {
                RegisterHit(column, row, opponent);
                opponent.GetPlayerBoard()[column, row] = ECellState.Hit;
                return true;
            }

            opponent.GetPlayerBoard()[column, row] = ECellState.Bomb;
            return false;
        }

        // Get random bomb coordinates on opponent board, check if bomb not already placed there.
        public (int x, int y) GetRandomBombCoordinates(Player opponent)
        {
            Random rand = new(Guid.NewGuid().GetHashCode());
            var column = rand.Next(0, opponent.GetPlayerBoard().GetUpperBound(0));
            var row = rand.Next(0, opponent.GetPlayerBoard().GetUpperBound(1));

            while (opponent.GetPlayerBoard()[column, row] == ECellState.Bomb ||
                   opponent.GetPlayerBoard()[column, row] == ECellState.Hit)
            {
                column = rand.Next(0, opponent.GetPlayerBoard().GetUpperBound(0) + 1);
                row = rand.Next(0, opponent.GetPlayerBoard().GetUpperBound(1) + 1);
            }

            return (column, row);
        }

        // Return true if bomb is hit, false is miss.
        public bool PlaceRandomBomb(Player opponent)
        {
            Random rand = new(Guid.NewGuid().GetHashCode());
            var column = rand.Next(0, opponent.GetPlayerBoard().GetUpperBound(0));
            var row = rand.Next(0, opponent.GetPlayerBoard().GetUpperBound(1));

            while (opponent.GetPlayerBoard()[column, row] == ECellState.Bomb ||
                   opponent.GetPlayerBoard()[column, row] == ECellState.Hit)
            {
                column = rand.Next(0, opponent.GetPlayerBoard().GetUpperBound(0) + 1);
                row = rand.Next(0, opponent.GetPlayerBoard().GetUpperBound(1) + 1);
            }

            if (IsHit(column, row, opponent))
            {
                RegisterHit(column, row, opponent);
                opponent.GetPlayerBoard()[column, row] = ECellState.Hit;
                return true;
            }

            opponent.GetPlayerBoard()[column, row] = ECellState.Bomb;
            return false;
        }

        // Placing ships, and checking input,
        // return true if ship is placed, false if coordinates are invalid or other ship on the way.
        public bool PlaceShip(int column, int row, Ship ship, EOrientation orientation, EShipsCanTouch shipsCanTouch)
        {
            var shipSize = ship.Width;
            var board = GameBoard.Board;
            var boardWidth = board.GetUpperBound(0);
            var boardHeight = board.GetUpperBound(1);
            var state = ship.CellState;
            switch (orientation)
            {
                case EOrientation.Horizontal:
                {
                    if (!ShipCoordinatesAreValid(column, row, boardWidth, boardHeight, ship, EOrientation.Horizontal))
                    {
                        return false;
                    }

                    if (!ShipAreaFree(column, row, GameBoard, ship, EOrientation.Horizontal, shipsCanTouch))
                    {
                        return false;
                    }

                    for (var i = 0; i < shipSize; i++)
                    {
                        board[column + i, row] = state;
                    }

                    break;
                }
                case EOrientation.Vertical:
                {
                    if (!ShipCoordinatesAreValid(column, row, boardWidth, boardHeight, ship, EOrientation.Vertical))
                    {
                        return false;
                    }

                    if (!ShipAreaFree(column, row, GameBoard, ship, EOrientation.Vertical, shipsCanTouch))
                    {
                        return false;
                    }

                    for (var i = 0; i < shipSize; i++)
                    {
                        board[column, row + i] = state;
                    }

                    break;
                }
                default:
                    throw new ArgumentOutOfRangeException(nameof(orientation), orientation, null);
            }

            return true;
        }

        // Check if ship is not out of game board bounds.
        private static bool ShipCoordinatesAreValid(int column, int row, int boardWidth, int boardHeight, Ship ship,
            EOrientation orientation)
        {
            if (orientation == EOrientation.Horizontal)
            {
                return column + ship.Width <= boardHeight + 1;
            }

            return row + ship.Width <= boardWidth + 1;
        }

        // Check if there is not another ship on the way,
        // return true if ship area free, false if not.
        private static bool ShipAreaFree(int column, int row, GameBoard board, Ship ship, EOrientation orientation,
            EShipsCanTouch shipsCanTouch)
        {
            var boardWidth = board.Board.GetUpperBound(0);
            var boardHeight = board.Board.GetUpperBound(1);
            var startColumn = column;
            var startRow = row;
            var endColumn = startColumn;
            var endRow = startRow;
            var occupiedCells = 0;

            switch (orientation)
            {
                case EOrientation.Horizontal:
                {
                    for (var i = 1; i < ship.Width; i++)
                    {
                        endColumn++;
                    }

                    break;
                }
                case EOrientation.Vertical:
                {
                    for (var i = 1; i < ship.Width; i++)
                    {
                        endRow++;
                    }

                    break;
                }
                default:
                    throw new ArgumentOutOfRangeException(nameof(orientation), orientation, null);
            }

            switch (shipsCanTouch)
            {
                case EShipsCanTouch.Yes when ship.Width == 1:
                    return board.Board[column, row] == ECellState.Empty;
                case EShipsCanTouch.Yes:
                {
                    for (var i = startColumn; i <= endColumn; i++)
                    {
                        for (var j = startRow; j <= endRow; j++)
                        {
                            if (board.Board[i, j] == ECellState.Empty)
                            {
                                continue;
                            }

                            occupiedCells++;
                        }
                    }

                    break;
                }
                case EShipsCanTouch.No:
                {
                    if (startColumn < 1)
                    {
                        ++startColumn;
                        ++endColumn;
                    }

                    if (startRow < 1)
                    {
                        ++startRow;
                        ++endRow;
                    }

                    if (startColumn >= boardWidth)
                    {
                        --startColumn;
                        endColumn = startColumn;
                    }

                    if (startRow >= boardWidth)
                    {
                        --endRow;
                        endRow = startRow;
                    }

                    if (endColumn >= boardWidth)
                    {
                        --endColumn;
                    }

                    if (endRow >= boardHeight)
                    {
                        --endRow;
                    }

                    for (var i = startColumn - 1; i <= endColumn + 1; i++)
                    {
                        for (var j = startRow - 1; j <= endRow + 1; j++)
                        {
                            if (board.Board[i, j] == ECellState.Empty)
                            {
                                continue;
                            }

                            occupiedCells++;
                        }
                    }

                    break;
                }
                default:
                    throw new ArgumentOutOfRangeException(nameof(shipsCanTouch), shipsCanTouch, null);
            }

            return occupiedCells == 0;
        }

        public void PlaceRandomShips(EShipsCanTouch shipsCanTouch)
        {
            var random = new Random();
            var width = GameBoard.Board.GetUpperBound(0) + 1;
            var height = GameBoard.Board.GetUpperBound(1) + 1;
            foreach (var ship in Ships)
            {
                var x = random.Next(1, width);
                var y = random.Next(1, height);
                var orientation = EOrientation.Vertical;
                var orientationIndex = random.Next(1, 101) % 2;
                orientation = orientationIndex switch
                {
                    0 => EOrientation.Horizontal,
                    1 => EOrientation.Vertical,
                    _ => orientation
                };
                var shipPlaced = PlaceShip(x, y, ship, orientation, shipsCanTouch);
                while (!shipPlaced)
                {
                    x = random.Next(1, width);
                    y = random.Next(1, height);
                    orientationIndex = random.Next(1, 101) % 2;
                    orientation = orientationIndex switch
                    {
                        0 => EOrientation.Horizontal,
                        1 => EOrientation.Vertical,
                        _ => orientation
                    };
                    shipPlaced = PlaceShip(x, y, ship, orientation, shipsCanTouch);
                }
            }
        }

        public string GetSerializedGameBoardState()
        {
            var state = new GameBoardState();

            var width = GameBoard.Board.GetUpperBound(0) + 1;
            var height = GameBoard.Board.GetUpperBound(1) + 1;

            state.Board = new ECellState[width][];

            for (var i = 0; i < state.Board.Length; i++)
            {
                state.Board[i] = new ECellState[height];
            }

            for (var x = 0; x < width; x++)
            {
                for (var y = 0; y < height; y++)
                {
                    state.Board[x][y] = GameBoard.Board[x, y];
                }
            }

            var jsonOptions = new JsonSerializerOptions
            {
                WriteIndented = true
            };

            return JsonSerializer.Serialize(state, jsonOptions);
        }

        public void SetGameBoardStateFromJsonString(string json)
        {
            var width = GameBoard.Width;
            var height = GameBoard.Height;
            var cellState = JsonSerializer.Deserialize<GameBoardState>(json)!.Board;
            var gameBoard = new GameBoard(width, height);
            for (var x = 0; x < width; x++)
            {
                for (var y = 0; y < height; y++)
                {
                    gameBoard.Board[x, y] = cellState[x][y];
                }
            }

            GameBoard = gameBoard;
        }
    }
}
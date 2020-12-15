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

        public void SetBoard(int width, int height)
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
                Ships.Add(new Ship
                {
                    CellState = ship.CellState,
                    Hits = ship.Hits,
                    Name = ship.Name,
                    Width = ship.Width
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

        public bool IsHit(int column, int row, Player opponent)
        {
            var playerCell = opponent.GetPlayerBoard()[column, row];
            return playerCell != ECellState.Empty && playerCell != ECellState.Bomb && playerCell != ECellState.Hit;
        }

        private static void RegisterHit(int column, int row, Player opponent)
        {
            var shipState = opponent.GetPlayerBoard()[column, row];
            var ship = new Ship();

            foreach (var opponentShip in opponent.Ships.Where(opponentShip =>
                opponentShip.CellState == shipState && !opponentShip.IsSunk))
            {
                ship = opponentShip;
            }

            ship.Hits++;
        }

        // Return true if is hit.
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

        // Return true if is hit.
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

        private static bool ShipCoordinatesAreValid(int column, int row, int boardWidth, int boardHeight, Ship ship,
            EOrientation orientation)
        {
            var x = column;
            var y = row;
            if (orientation == EOrientation.Horizontal)
            {
                return x + ship.Width <= boardWidth;
            }

            return y + ship.Width <= boardHeight;
        }

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

        public bool ShipAreaFree(int column, int row, GameBoard board, Ship ship, EOrientation orientation,
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
                // while (!Validator.ShipCoordinatesAreValid(x, y, width, height, ship, orientation)
                //        || !ShipAreaFree(x, y, GameBoard, ship, orientation, shipsCanTouch))
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

                // PlaceShip(x, y, ship, orientation);
            }
        }

        private void RemoveAllShipsFromPlayerGameBoard()
        {
            var width = GameBoard.Board.GetUpperBound(0);
            var height = GameBoard.Board.GetUpperBound(1);

            for (var i = 0; i < height; i++)
            {
                for (var j = 0; j < width; j++)
                {
                    GameBoard.Board[i, j] = ECellState.Empty;
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